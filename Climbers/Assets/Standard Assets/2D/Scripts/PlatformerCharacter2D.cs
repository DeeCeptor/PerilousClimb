using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private bool m_Jump;
        //private Vector2 most_recent_ground_posiiton



        // Custom fields
        [SerializeField] private float climbing_max_speed = 2;
        private float slope_friction = 0.6f;
        [HideInInspector] public float normal_gravity_factor;
        [HideInInspector] public bool can_climb = false;  // If over a climbasble section, this is set to true
        [HideInInspector] public bool is_climbing = false;

        [HideInInspector] public float HP = 1;
        [HideInInspector] public float stamina = 1;   // Stamina dictates if we can do stressful actions. Low stamina means no.
        private float stamina_regen_per_second = 0.5f;  // How much stamina is regenerated per second
        [HideInInspector] public float encumbrance = 0;   // How much weight we're carrying

        // Drag these in from the UI
        public Slider HP_slider;
        public Slider stamina_slider;
        public Slider weight_slider;

        private float jump_stamina_cost = 0.7f;


        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            //m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            normal_gravity_factor = m_Rigidbody2D.gravityScale;

            AdjustHP(0);
            AdjustStamina(0);
        }


        private void Update()
        {
            AdjustStamina(Time.deltaTime * stamina_regen_per_second);

            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = Input.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    break;
                }
            }


            // If this current frame is now grounded, and our previous one wasn't, then we were in the air and we just landed
            // Check the velocity to see if this should cause damage


            //m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            ////m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            // Attempt vertical normalization
            // To prevent player from sliding down slopes, turn off gravity if they are touching the ground
            /*
            if (m_Grounded)
            {
                //m_Rigidbody2D.gravityScale = 0.5f;
                
                Debug.Log("A");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, m_WhatIsGround);
                if (hit.collider != null)
                    Debug.Log(hit.normal);
                if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
                {
                    Debug.Log("b");

                    Rigidbody2D body = GetComponent<Rigidbody2D>();
                    // Apply the opposite force against the slope force 
                    // You will need to provide your own slopeFriction to stabalize movement
                    body.velocity = new Vector2(body.velocity.x - (hit.normal.x * slope_friction), body.velocity.y);

                    //Move Player up or down to compensate for the slope below them
                    Vector3 pos = transform.position;
                    pos.y += -hit.normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
                    transform.position = pos;
                }
                */

            //}
            //else
            //    m_Rigidbody2D.gravityScale = normal_gravity_factor;

            // Read the inputs.
            bool climb = Input.GetKey(KeyCode.LeftShift);

            if (climb)
                this.StartClimbing();

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Pass all parameters to the character control script.
            this.Move(h, v, climb, m_Jump);
            m_Jump = false;
        }


        // Main method for resolving user input
        public void Move(float horizontal_input, float vertical_input, bool climbing, bool jump)
        {
            // If crouching, check to see if the character can stand up
            /*if (!crouch) // && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }*/

            // Set whether or not the character is crouching in the animator
            //m_Anim.SetBool("Crouch", crouch);

            // Are we climbing?
            if (climbing & is_climbing)
            {
                // Climbing sideways on wall
                m_Rigidbody2D.velocity = new Vector2(horizontal_input * climbing_max_speed, vertical_input * climbing_max_speed);
            }
            // Are we walking?
            else
            {
                // Normal ground movement
                // Only control the player if grounded or airControl is turned on
                if (m_Grounded || m_AirControl)
                {
                    // Reduce the speed if crouching by the crouchSpeed multiplier
                    //horizontal_input = (crouch ? horizontal_input * m_CrouchSpeed : horizontal_input);

                    // The Speed animator parameter is set to the absolute value of the horizontal input.
                    //m_Anim.SetFloat("Speed", Mathf.Abs(move));

                    // Move the character
                    m_Rigidbody2D.velocity = new Vector2(horizontal_input * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                    // If the input is moving the player right and the player is facing left...
                    if (horizontal_input > 0 && !m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (horizontal_input < 0 && m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                }
                // If the player should jump...
                if (m_Grounded && jump && stamina >= jump_stamina_cost) // && m_Anim.GetBool("Ground"))
                {
                    AdjustStamina(-jump_stamina_cost);

                    // Add a vertical force to the player.
                    m_Grounded = false;
                    //m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                }
            }
        }


        // Stamina is always between 0 and 1
        public void AdjustStamina(float amount)
        {
            stamina = Mathf.Clamp(stamina + amount, 0, 1);
            stamina_slider.value = stamina;
        }
        // HP is <= 1. If <= 0, the player is dead
        public void AdjustHP(float amount)
        {
            HP = Mathf.Clamp(stamina + amount, 0, 1);
            HP_slider.value = HP;

            if (HP <= 0)
            {
                Die();
            }
        }
        public void Die()
        {
            Debug.Log("Player died");
        }

        public void StartClimbing()
        {
            if (can_climb)
            {
                is_climbing = true;
                this.m_Rigidbody2D.gravityScale = 0;
            }
        }
        public void StopClimbing()
        {
            is_climbing = false;
            this.m_Rigidbody2D.gravityScale = normal_gravity_factor;
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
