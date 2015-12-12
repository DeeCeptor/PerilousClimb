using UnityEngine;
using System.Collections;

// Handles the joining and 
public class Player : MonoBehaviour
{
    public bool joined = false;     // Are we waiting for this player to join?
    public bool keyboard = false;  
    public int player_number;
    public string controls_prefix;  // Used for getting input
    public Color player_color;

    public PlatformerCharacter2D controller;

    private bool prev_left_trigger_down = false;
    private bool prev_right_trigger_down = false;

    void Start () {
	
	}
	
	void Update ()
    {
	    if (!joined)
        {
            if (IsJoining())
                Join();
        }
        else
        {

        }
	}
    void LateUpdate()
    {
        if (!keyboard)
        {
            // We want to treat controller triggers as buttons, so we must keep track of they were previously pressed
            prev_left_trigger_down = IsControllerTriggerDown("LeftTrigger");
            prev_right_trigger_down = IsControllerTriggerDown("RightTrigger");
        }
    }


    bool IsJoining()
    {
        return Input.GetButton(controls_prefix + "Start");
    }
    public void Join()
    {
        Debug.Log("Player " + player_number + " joined");
        joined = true;

        // Spawn a new player
        GameObject player_obj = Instantiate(Resources.Load("Player"), new Vector3(5, 5, 0), Quaternion.identity) as GameObject;
        PlatformerCharacter2D cont =  player_obj.GetComponent<PlatformerCharacter2D>();
        cont.player = this;
        this.controller = cont;
    }


    public Vector2 GetNormalizedAimingVector()
    {
        if (keyboard)
        {
            // Find where/if the player is aiming with the mouse
            Vector3 mouse = Input.mousePosition;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(controller.transform.localPosition);
            Vector2 look_direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
            look_direction.Normalize();
            return look_direction;
        }
        else
        {
            float x = Input.GetAxis(controls_prefix + "HorizontalRight");
            float y = Input.GetAxis(controls_prefix + "VerticalRight");
            Vector2 look_direction = new Vector2(x, y);
            Debug.Log(look_direction);
            look_direction.Normalize();
            return look_direction;
        }
    }


    // Pass in the button name you want to know is down. Ex: GetButtonDown("Start") will return the Start button for the current player
    public bool IsButtonPressed(string button)
    {
        if (!keyboard && button == "LeftTrigger")
        {
            return (!prev_left_trigger_down && IsControllerTriggerDown(button));
        }
        else if (!keyboard && button == "RightTrigger")
        {
            return (!prev_right_trigger_down && IsControllerTriggerDown(button));
        }
        else
        {
            return Input.GetButtonDown(controls_prefix + button);
        }
    }
    public bool IsButtonCurrentlyDown(string button)
    {
        if (!keyboard && (button == "LeftTrigger" || button == "RightTrigger"))
            return IsControllerTriggerDown(button);
        else
            return Input.GetButton(controls_prefix + button);
    }
    public bool IsButtonUp(string button)
    {
        if (!keyboard && button == "LeftTrigger")
        {
            return (prev_left_trigger_down && !IsControllerTriggerDown(button));
        }
        else if (!keyboard && button == "RightTrigger")
        {
            return (prev_right_trigger_down && !IsControllerTriggerDown(button));
        }
        else
            return Input.GetButtonUp(controls_prefix + button);
    }
    public float GetAxis(string axis)
    {
        return Input.GetAxis(controls_prefix + axis);
    }


    public bool IsControllerTriggerDown(string trigger_name)
    {
        if (trigger_name == "LeftTrigger" || trigger_name == "RightTrigger")
            return (Input.GetAxis(controls_prefix + trigger_name) > 0f);
        else
        {
            Debug.Log("No such controller trigger " + trigger_name);
            return false;
        }
    }
}
