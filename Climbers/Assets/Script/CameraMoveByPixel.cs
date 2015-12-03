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
