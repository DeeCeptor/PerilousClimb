using System;
using UnityEngine;
using UnityEngine.UI;


public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    public float rope_throw_force = 3000;

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .07f; // Radius of the overlap circle to determine if grounded
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
    [HideInInspector] public bool can_climb = false;  // If over a climbable section, this is set to true
    [HideInInspector] public bool is_climbing = false;
    [HideInInspector] public bool can_climb_rope = false;
    [HideInInspector] public bool is_climbing_rope = false;

    [HideInInspector] public float HP = 1;
    [HideInInspector] public float stamina = 1;   // Stamina dictates if we can do stressful actions. Low stamina means no.
    private float stamina_regen_per_second = 0.5f;  // How much stamina is regenerated per second
    [HideInInspector] public float encumbrance = 0;   // How much weight we're carrying

    // Drag these in from the UI
    public Slider HP_slider;
    public Slider stamina_slider;
    public Slider weight_slider;

    private float jump_stamina_cost = 0.4f;

    bool previously_grounded = false;
    Vector2 previous_velocity = Vector2.zero;

    float fall_damage_threshold = -15f;     // Anything more (> -15) does no damage when falling
    float jump_delay = 0.2f;    // Can't jump again jump_Delay seconds after the last jump. Prevents weird super jumping.
    float cur_jump_delay = 0;

    [HideInInspector]
    public HingeJoint2D connected_joint;    // Joinbt that is used to connect a player to a rope
    public GameObject rope_follower;

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        //m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        normal_gravity_factor = m_Rigidbody2D.gravityScale;
        connected_joint = this.GetComponent<HingeJoint2D>();

        AdjustHP(0);
        AdjustStamina(0);
    }


    private void Update()
    {
        // Find where/if the player is aiming with the mouse
        Vector3 mouse = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        Vector2 look_direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
        look_direction.Normalize();

        AdjustStamina(Time.deltaTime * stamina_regen_per_second);
        cur_jump_delay -= Time.deltaTime;

        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetButtonDown("Jump");
        }

        // Throw rope if clicking
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject rope_parent = (GameObject)Instantiate(Resources.Load("Rope"), transform.position, transform.rotation);

            // Call throw rope on the rope generator
            // 3000 on 1 mass ropes
            rope_parent.GetComponent<RopeGenerator>().Throw_Rope(this.transform.position, look_direction, rope_throw_force, m_Rigidbody2D);
        }
    }


    private void FixedUpdate()
    {
        previously_grounded = m_Grounded;
        previous_velocity = m_Rigidbody2D.velocity;
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
        // -Y velocity means we're heading downwards
        if (!previously_grounded && m_Grounded && previous_velocity.y < 0)
        {
            // An average jump will have you get ~-9 Y velocity
            // Deal damage if the value is over -15
            // Are we going fast enough to get hurt?
            if (previous_velocity.y < fall_damage_threshold)
            {
                // Calculate how much damage we should take
                float velocity_over_threshold = Mathf.Abs(previous_velocity.y) - Mathf.Abs(fall_damage_threshold);
                float damage = -(velocity_over_threshold / 10);
                Debug.Log("Took damage: " + damage + " " + velocity_over_threshold);
                AdjustHP(damage);
            }
        }

        //m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        ////m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        

        // Read the inputs.
        bool climb = Input.GetKey(KeyCode.LeftShift);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (climb)
        {
            if (can_climb_rope && !is_climbing_rope)
                this.StartClimbingRope();
            else
                this.StartClimbing();
        }

        if (m_Jump && is_climbing_rope)
            this.StopClimbingRope();



        // Change the way we interpret input if we're connected to a rope
        if (connected_joint != null)
        {
            // Stop the player from changing their horizontal velocity by checking to see if the connected joint
            // is tugging at the limit of its rope
            if (Math.Abs(h) > 0)
            {
                float maximum_expected_force = m_Rigidbody2D.mass * (1 / Time.deltaTime) - 1000;
                //Debug.Log(connected_joint.GetReactionForce(Time.deltaTime).x + " : " + maximum_expected_force);
                // 4000 force, 20 mass, 200 acceleration, for 0.02 timestep
                // F = MA
                // 3 x 10
                if (Mathf.Abs(connected_joint.GetReactionForce(Time.deltaTime).x) >= maximum_expected_force)
                {
                    h = 0;
                    Debug.Log(connected_joint.GetReactionForce(Time.deltaTime) + " : " + (maximum_expected_force));
                }
            }
        }

        // Pass all parameters to the character control script
        if (is_climbing_rope)
            MoveOnRope(h, v);
        else
            this.Move(h, v, climb, m_Jump);

        m_Jump = false;
    }


    public Transform current_segment;
    public Transform above_segment;
    public Transform below_segment;
    public Link rope_in_background;
    float climbing_speed = 0.5f;
    float distance_between_segments = 0;

    // Controls all movement on the rope
    public void MoveOnRope(float h, float v)
    {
        // If at the next segment, find the next segment
        if (distance_between_segments >= 1)
        {
            below_segment = current_segment;
            current_segment = current_segment.GetComponent<Link>().above.transform;
            above_segment = current_segment.GetComponent<Link>().above.transform;
            distance_between_segments = 0;
        }
        else if (distance_between_segments <= -1)
        {
            above_segment = current_segment;
            current_segment = current_segment.GetComponent<Link>().below.transform;
            below_segment = current_segment.GetComponent<Link>().below.transform;
            distance_between_segments = 0;
        }

        // Keep track of how far between the two segments we are
        distance_between_segments = Mathf.Clamp(distance_between_segments + v * climbing_speed, -1, 1);

        if (distance_between_segments >= 0)
        {
            this.transform.position = Vector2.Lerp(current_segment.transform.position, above_segment.transform.position, distance_between_segments);
        }
        else
        {
            this.transform.position = Vector2.Lerp(current_segment.transform.position, below_segment.transform.position, Mathf.Abs(distance_between_segments));
        }

        // Swing rope left and right
        if (h != 0)
        {
            current_segment.GetComponent<Rigidbody2D>().AddForce(new Vector2(h * 100, 0));
        }
    }


    // Main method for resolving user input
    public void Move(float horizontal_input, float vertical_input, bool climbing_button, bool jump)
    {
        // Stop climbing if we let go of the button
        if (!climbing_button && is_climbing)
            StopClimbing();

        // Are we climbing?
        if (climbing_button & is_climbing)
        {
            // Climbing sideways on wall
            m_Rigidbody2D.velocity = new Vector2(horizontal_input * climbing_max_speed, vertical_input * climbing_max_speed);
        }
        else if (connected_joint != null && vertical_input != 0)
        {
            // Move up or down
            connected_joint.connectedAnchor = new Vector2(connected_joint.connectedAnchor.x, connected_joint.connectedAnchor.y + vertical_input / 100f);
        }
        // Are we walking?
        else if (m_Grounded || m_AirControl)
        {
            // Normal ground movement
            // Only control the player if grounded or airControl is turned on

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
        if (jump && stamina >= jump_stamina_cost && cur_jump_delay <= 0)
        {
            // On the ground, jump normally
            if (m_Grounded) // && m_Anim.GetBool("Ground"))
            {
                AddJumpVelocity();
            }
            // Player jumped while not grounded and connect to a rope
            else if (!m_Grounded && connected_joint != null && connected_joint.enabled)
            {
                // Disconnect the rope
                connected_joint.enabled = false;
                // Jump
                AddJumpVelocity();
            }
        }
    }
    // Adds an upwards force to the player
    public void AddJumpVelocity()
    {
        AdjustStamina(-jump_stamina_cost);

        // Add a vertical force to the player.
        m_Grounded = false;

        //m_Anim.SetBool("Ground", false);

        // Add a delay so we can't jump again immediately
        cur_jump_delay = jump_delay;

        // Remove our current Y velocity so when moving up slopes we don't get a boost
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);

        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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
        HP = Mathf.Clamp(HP + amount, 0, 1);
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

    public void StartClimbingRope()
    {
        is_climbing_rope = true;

        this.GetComponent<Rigidbody2D>().isKinematic = true;
        //this.GetComponent<Rigidbody2D>().gravityScale = 0;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        current_segment = rope_in_background.transform;
        above_segment = rope_in_background.above.transform;
        below_segment = rope_in_background.below.transform;
    }
    public void StopClimbingRope()
    {
        is_climbing_rope = false;
        this.GetComponent<Rigidbody2D>().isKinematic = false;
        //this.GetComponent<Rigidbody2D>().gravityScale = normal_gravity_factor;
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
