using UnityEngine;
using System.Collections;

public class CameraMoveByPixel : MonoBehaviour
{
    public Vector2 camera_offset;   // Applied if we are playing by ourselves
    private Camera this_camera;
    public float size = 10f;
    private float starting_z;

    public float orthoSize = 32f; // Check your sprite's in the inspector. This should be double the "Pixels Per Unit"
    private float lastPixelWidth = 0f;
    private float lastPixelHeight = 0f;
    private float lastOrtho;


    void Start ()
    {
        this_camera = this.GetComponent<Camera>();
        starting_z = this.transform.position.z;

        SetCameraSize();
    }

    // Set the orthographic size of the camera
    public void SetCameraSize()
    {

        // set the camera to the correct orthographic size
        // (so scene pixels are 1:1)
        //float s_baseOrthographicSize = Screen.height / 16.0f / 2.0f;
        //Camera.main.orthographicSize = s_baseOrthographicSize;


        //Camera.main.orthographicSize = Screen.width / (((Screen.width / Screen.height) * 2) * 16);
        Camera.main.orthographicSize = size;
        Camera.main.orthographicSize = Screen.height * this_camera.rect.height / 16 / 2.0f;//- 0.1f;
        //    x / (((x / y) * 2) * s)
        //Camera Size = y / (2 * s)
    }


    void LateUpdate ()
    {
        // Adjust orthographic size if screen has changed size
        if (this_camera.orthographicSize != Screen.height / orthoSize ||
             lastPixelWidth != this_camera.pixelWidth ||
             lastPixelHeight != this_camera.pixelHeight)
        {
            float new_size = Screen.height / orthoSize;
            Debug.Log("Changing screen resolution from " + this_camera.orthographicSize + " to " + new_size + ", " + Screen.height + "/" + orthoSize);
            this_camera.orthographicSize = new_size;
            lastPixelWidth = this_camera.pixelWidth;
            lastPixelHeight = this_camera.pixelHeight;
            //this_camera.rect = new Rect(0, 0, 2000, this_camera.rect.height);
        }


        int num_players = 0;
        Vector2 averaged_player_position = Vector2.zero;
        // Calculate the average position to follow
        foreach (PlatformerCharacter2D player in PlayerInformation.player_information.players)
        {
            num_players++;
            averaged_player_position += (Vector2) player.transform.position;
        }
        averaged_player_position += camera_offset;
        averaged_player_position = averaged_player_position / num_players;

        /*
        this.transform.position = new Vector3(RoundToNearestPixel(averaged_player_position.x, this_camera),
                                              RoundToNearestPixel(averaged_player_position.y, this_camera),
                                              starting_z);*/
        this.transform.position = new Vector3(averaged_player_position.x, averaged_player_position.y, starting_z);
	}


    public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }
}
