using System.Collections.Generic;
using UnityEngine;


// http://answers.unity3d.com/questions/871729/pulling-up-a-rope.html
public class RopeGenerator : MonoBehaviour
{
    private LineRenderer line;
    private List<GameObject> joints;
    private int vertexCount;
    private float NTDistance;
    public GameObject emptyPrefab;
    public GameObject beginning;
    public GameObject end;

    //public Vector2 direction = (1, 0);

    void Start()
    {
        //vertexCount = 50;
        vertexCount = (((int)Vector2.Distance(beginning.transform.position, end.transform.position)) * 3) - 1;

        joints = new List<GameObject>();
        line = GetComponent<LineRenderer>();
        line.SetWidth(0.1f, 0.1f);  // 0.05f
        //line.SetColors(Color.black, Color.blue);


        for (int i = 0; i < vertexCount; i++)
        {
            joints.Add((GameObject)Instantiate(emptyPrefab, new Vector3(beginning.transform.position.x, beginning.transform.position.y, 0), Quaternion.identity));
        }

        // Connect all the joints and and make their parents this object
        for (int j = 0; j < joints.Count - 1; j++)
        {
            //joints[j].transform.parent = this.transform;
            joints[j].GetComponent<HingeJoint2D>().connectedBody = joints[j + 1].GetComponent<Rigidbody2D>();
        }

        // Disable joints on the end
        //joints[vertexCount - 1].GetComponent<HingeJoint2D>().enabled = false;

        // Throw the first one
        //joints[vertexCount - 1].GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 30000), ForceMode2D.Force);

        // Set connections on ends
        // Where player is
        /*SpringJoint2D jo = joints[0].AddComponent<SpringJoint2D>();
        jo.connectedBody = beginning.GetComponent<Rigidbody2D>();
        jo.frequency = 0;*/
        /*DistanceJoint2D jo = joints[0].AddComponent<DistanceJoint2D>();
        jo.connectedBody = beginning.GetComponent<Rigidbody2D>();
        jo.distance = 0.2f;*/
        HingeJoint2D jo = joints[0].AddComponent<HingeJoint2D>();
        jo.connectedBody = beginning.GetComponent<Rigidbody2D>();
   
        /*
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().connectedBody = end.GetComponent<Rigidbody2D>();
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().anchor = Vector2.zero;
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;*/
        joints[vertexCount - 1].GetComponent<Rigidbody2D>().isKinematic = true;
    }


    void Update()
    {
        line.SetVertexCount(joints.Count);
        for (int i = 0; i < joints.Count; i++)
        {
            line.SetPosition(i, joints[i].transform.position);
        }
    }
}