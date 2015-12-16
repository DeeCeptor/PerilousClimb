using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField] public bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    public float forced_air_control_movement_speed = 50f;
    [SerializeField] public bool forced_air_control = false;     // Does not change the player's velocity in midair, instead allowing the player to use forces to move
    [SerializeField] public bool on_ice = false;     // Changes the controls from velocity changing to using forces.
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
    public float rope_throw_force = 3000;
    public float swing_force = 10f;     // The forced with which we swing left and right on the rope

    private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .09f; // Radius of the overlap circle to determine if grounded
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
    [HideInInspector] public float normal_gravity_factor;
    [HideInInspector] public float normal_mass;
    [HideInInspector] public bool can_climb = false;  // If over a climbable section, this is set to true
    [HideInInspector] public bool is_climbing = false;
    [HideInInspector] public bool can_climb_rope = false;
    [HideInInspector] public bool is_climbing_rope = false;
    bool is_climbing_button_down = false;
    SpriteRenderer aimer;   // Aims towards the mouse or the right stick of a controller

    [HideInInspector] public float HP = 1;
    [HideInInspector] public float stamina = 1;   // Stamina dictates if we can do stressful actions. Low stamina means no.
    private float stamina_regen_per_second = 0.5f;  // How much stamina is regenerated per second
    [HideInInspector] public float encumbrance = 0;   // How much weight we're carrying

    // Used to get input
    public Player player;

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
    public HingeJoint2D connected_joint;    // Joint that is used to connect a player to a rope
    public RopeGenerator attached_rope;


    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        //m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        normal_gravity_factor = m_Rigidbody2D.gravityScale;
        normal_mass = m_Rigidbody2D.mass;
        connected_joint = this.GetComponent<HingeJoint2D>();

        AdjustHP(0);
        AdjustStamina(0);
    }


    void Start()
    {
        PlayerInformation.player_information.players.Add(this);

        // Spawn an aimer for this player
        aimer = ((GameObject) Instantiate(Resources.Load("Aimer") as GameObject, this.transform.position, Quaternion.identity)).GetComponent<SpriteRenderer>();
        aimer.GetComponent<FollowObject>().object_to_follow = this.transform;
        aimer.enabled = false;
    }


    private void Update()
    {
        // Find where/if the player is aiming with the mouse or right joystick
        Vector2 look_direction = player.GetNormalizedAimingVector();
        
        // Show the aiming line if holding down a button
        if ((!player.keyboard && look_direction != Vector2.zero)    // Controller
            ||
            (player.IsButtonCurrentlyDown("LeftTrigger")    // Keyboard
            || player.IsButtonCurrentlyDown("RightTrigger")
            || player.IsButtonCurrentlyDown("LeftBumper")))
        {
            // Rotate the aimer in the right direction
            var angle = Mathf.Atan2(look_direction.y, look_direction.x) * Mathf.Rad2Deg;
            aimer.transform.rotation = Quaternion.Euler(0, 0, angle);
            aimer.enabled = true;
        }
        else
            aimer.enabled = false;


        AdjustStamina(Time.deltaTime * stamina_regen_per_second);
        cur_jump_delay -= Time.deltaTime;

        is_climbing_button_down = player.IsButtonCurrentlyDown("RightBumper");

        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = player.IsButtonPressed("Jump");
        }

        // Throw rope if clicking
        if (player.IsButtonUp("LeftTrigger"))   // Left click
        {
            // Call throw rope on the rope generator
            GameObject rope_parent = (GameObject)Instantiate(Resources.Load("Rope"), transform.position, transform.rotation);
            // 3000 on 1 mass ropes
            rope_parent.GetComponent<RopeGenerator>().Throw_Rope(this.transform.position, look_direction, rope_throw_force, null, this.gameObject);
        }
        // Throw rope that is tied around the player's waist
        if (player.IsButtonUp("LeftBumper"))   // Middle click
        {
            // Call throw rope on the rope generator
            GameObject rope_parent = (GameObject)Instantiate(Resources.Load("Rope"), transform.position, transform.rotation);
            // 3000 on 1 mass ropes
            rope_parent.GetComponent<RopeGenerator>().Throw_Rope(this.transform.position, look_direction, rope_throw_force, m_Rigidbody2D, this.gameObject);
        }
        // Throw grappling hook
        if (player.IsButtonUp("RightTrigger"))   // Right click
        {
            // Spawn a hook
            GameObject hook = (GameObject)Instantiate(Resources.Load("Hook"), transform.position, transform.rotation);
            // Set its velocity to fly towards the mouse point
            hook.GetComponent<Rigidbody2D>().velocity = look_direction * 20f;
            hook.GetComponent<HookToTerrain>().thrower = this.gameObject;
        }
        // Destroy any rope the player is touching
        if (player.IsButtonPressed("B"))
        {
            if (rope_in_background != null && can_climb_rope && !is_climbing_rope)
            {
                GameObject.Destroy(rope_in_background.transform.parent.gameObject);
                rope_in_background = null;
                this.can_climb_rope = false;
                if (this.connected_joint.enabled)
                    this.connected_joint.enabled = false;
            }
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

            if (is_grappling)
                this.DetachFromRope(false);

            forced_air_control = false;
            m_AirControl = true;
        }
        if (m_Grounded 
            || (forced_air_control && (m_Rigidbody2D.velocity.magnitude < m_MaxSpeed * 0.6f) && false))
        {
            forced_air_control = false;
            m_AirControl = true;
        }

        //m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        ////m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);


        // Read the inputs
        float h = player.GetAxis("Horizontal");
        float v = player.GetAxis("Vertical");



        if (is_climbing_button_down)
        {
            if (can_climb_rope && !is_climbing_rope)
                this.AttachToRope();
            else
                this.StartClimbing();   // Climbing ledges. No rope involved
        }

        if (m_Jump && is_climbing_rope)
            this.DetachFromRope(true);

        // Change the way we interpret input if we're connected to a rope 
        if (connected_joint != null && false)
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
        {
            MoveOnRope(h, v);   // Move on a rope
        }
        else if (is_climbing)
        {
            MoveOnLedge(h, v, is_climbing_button_down, m_Jump);    // Move while climbing on ledges
        }
        else
        {
            Move(h, v, is_climbing_button_down, m_Jump);     // Move while walking on ground or in air
        }

        m_Jump = false;
    }



    public Link rope_in_background;
    GameObject cur_obj;
    GameObject prev_obj = null;
    int counter = 99;
    float cur_distance;     // How far along the rope we are
    float rope_climbing_speed = 0.03f;  // How quickly we move up and down while on a rope
    int stuck_counter = 0;  // Are we stuck on the geometry for a while?
    float max_hooking_distance = 8f;
    public List<GameObject> rope_links;
    SpringJoint2D spring;
    public bool is_grappling = false;

    // Controls all movement on the rope
    public void MoveOnRope(float h, float v)
    {
        // Control the player via a springjoint where the visible rope simply follows the player
        m_Rigidbody2D.AddForce(new Vector2(h * swing_force, 0));

        cur_distance = Mathf.Clamp(cur_distance + (-v) * rope_climbing_speed, 0.005f, max_hooking_distance);
        spring.distance = cur_distance;

        float dist = Vector2.Distance(this.transform.position, this.transform.position);
        if (cur_distance + 0.2f < dist)
        {
            stuck_counter++;

            if (stuck_counter > 10)
            {
                // This means we're pulling against the terrain and are stuck. Give us an upward push to get over the ledge
                //owner.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 3000, ForceMode2D.Impulse);
                this.AddJumpVelocity(false);
                stuck_counter = 0;
            }
        }
        else
        {
            stuck_counter = 0;
        }

        if (!is_grappling)
        {
            // If on a rope, have the closest segment of rope follow the player, to make the rope look taut
            // Calculate how far down the rope we are
            int cur_segment = Mathf.Clamp((int)(cur_distance * 5), 1, attached_rope.number_of_segments - 1);
            cur_obj = rope_links[cur_segment];

            if (prev_obj != null && prev_obj != cur_obj)
            {
                //prev_obj.GetComponent<TargetJoint2D>().enabled = false;
                prev_obj.GetComponent<Rigidbody2D>().isKinematic = false;
            }
            cur_obj.GetComponent<Rigidbody2D>().isKinematic = true;
            cur_obj.GetComponent<Rigidbody2D>().velocity = m_Rigidbody2D.velocity;
            //Debug.Log(owner.GetComponent<Rigidbody2D>().angularVelocity);
            //cur_obj.transform.position = owner.transform.position;
            counter++;

            if (counter > 0)
            {
                cur_obj.transform.position = Vector3.Lerp(cur_obj.transform.position, this.transform.position, 0.5f);
                //cur_obj.transform.position = owner.transform.position;
                counter = 0;
            }
            prev_obj = cur_obj;
        }
    }

    public void AttachToRope(Link rope_object)
    {
        rope_in_background = rope_object;
        can_climb_rope = true;
        attached_rope = rope_object.rope;
        AttachToRope();
    }
    public void AttachToRope()
    {
        if (rope_in_background != null && can_climb_rope)
        {
            if (is_climbing_rope)
                DetachFromRope(false);

            is_climbing_rope = true;

            rope_links = rope_in_background.GetComponent<Link>().all_segments;
            // Get the total length of this rope
            max_hooking_distance = rope_links.Count * rope_in_background.GetComponent<CircleCollider2D>().radius * 2;   // Radius is half a circle. We need the diameter
            // Activate the springjoint on this object, and set the distance 
            spring = rope_links[0].GetComponent<SpringJoint2D>();
            spring.enabled = true;
            spring.connectedBody = m_Rigidbody2D;
            cur_distance = Vector2.Distance(this.transform.position, spring.gameObject.transform.position);
            spring.distance = cur_distance;
            this.m_AirControl = false;
        }
    }
    public void AttachToGrapple(GameObject object_containing_sprintjoint, float max_distance)
    {
        if (is_climbing_rope)
            DetachFromRope(false);

        is_grappling = true;
        is_climbing_rope = true;

        max_hooking_distance = max_distance;
        spring = object_containing_sprintjoint.GetComponent<SpringJoint2D>();
        spring.enabled = true;
        spring.connectedBody = m_Rigidbody2D;
        cur_distance = Vector2.Distance(this.transform.position, spring.gameObject.transform.position);
        spring.distance = cur_distance;
        this.m_AirControl = false;
    }
    // Detaches player rope
    public void DetachFromRope(bool add_jump)
    {
        is_climbing_rope = false;
        is_grappling = false;
        spring.enabled = false;

        if (add_jump)
            this.AddJumpVelocity(false);

        if (prev_obj != null)
            prev_obj.GetComponent<Rigidbody2D>().isKinematic = false;
        if (cur_obj != null)
            cur_obj.GetComponent<Rigidbody2D>().isKinematic = false;

        if (!m_Grounded)
            forced_air_control = true;  // Keep all momentum gotten from swinging
        else
            this.m_AirControl = true;   // Let us move precisely
    }


    // Character is climbing on a ledge, and not on a rope
    void MoveOnLedge(float horizontal_input, float vertical_input, bool climbing_button, bool jump)
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
    }


    // Main method for resolving user input while not on a rope
    public void Move(float horizontal_input, float vertical_input, bool climbing_button, bool jump)
    {
        // Do we have control of our character? Yes if we're on the ground, and yes in the air if we have air control
        if (m_Grounded || m_AirControl)
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
        else if (forced_air_control)    // Player can adjust speed in midair by using forces
        {
            Debug.Log("forced");
            m_Rigidbody2D.AddForce(new Vector2(horizontal_input * forced_air_control_movement_speed, 0));
        }

        // If the player should jump...
        if (jump && stamina >= jump_stamina_cost && cur_jump_delay <= 0)
        {
            // On the ground, jump normally
            if (m_Grounded) // && m_Anim.GetBool("Ground"))
            {
                AddJumpVelocity(true);
            }
            // Player jumped while not grounded and connect to a rope
            else if (!m_Grounded && connected_joint != null && connected_joint.enabled)
            {
                // Disconnect the rope
                connected_joint.enabled = false;
                // Jump
                AddJumpVelocity(false);
            }
        }
    }
    // Adds an upwards force to the player
    public void AddJumpVelocity(bool cancel_out__previous_y_velocity)
    {
        AdjustStamina(-jump_stamina_cost);

        // Add a vertical force to the player.
        m_Grounded = false;

        //m_Anim.SetBool("Ground", false);

        // Add a delay so we can't jump again immediately
        cur_jump_delay = jump_delay;

        // Remove our current Y velocity so when moving up slopes we don't get a boost
        if (cancel_out__previous_y_velocity)
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);

        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
    }

    // Stamina is always between 0 and 1
    public void AdjustStamina(float amount)
    {
        stamina = Mathf.Clamp(stamina + amount, 0, 1);
        //stamina_slider.value = stamina;
    }
    // HP is <= 1. If <= 0, the player is dead
    public void AdjustHP(float amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, 1);
        //HP_slider.value = HP;

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

