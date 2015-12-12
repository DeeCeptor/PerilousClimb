using UnityEngine;
using System.Collections;

public class CameraMoveByPixel : MonoBehaviour
{
    public Transform object_to_follow;  // Object we are tracking
    private Camera this_camera;
    private float starting_z;


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
        Camera.main.orthographicSize = Screen.height / (2 * 16);
        //    x / (((x / y) * 2) * s)
        //Camera Size = y / (2 * s)
    }


    void LateUpdate ()
    {
        this.transform.position = new Vector3(RoundToNearestPixel(object_to_follow.transform.position.x, this_camera),
                                              RoundToNearestPixel(object_to_follow.transform.position.y, this_camera),
                                              starting_z);
	}


    public static float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }
}
