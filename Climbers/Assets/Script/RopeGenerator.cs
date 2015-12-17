using System.Collections.Generic;
using UnityEngine;


// http://answers.unity3d.com/questions/871729/pulling-up-a-rope.html
public class RopeGenerator : MonoBehaviour
{
    private LineRenderer line;
    public List<GameObject> joints;
    private int vertexCount;
    private float NTDistance;
    public GameObject emptyPrefab;
    public GameObject beginning_anchor;
    public GameObject end_anchor;

    public GameObject beginning_rope_piece;
    public GameObject end_rope_piece;

    public Vector2 direction;// = new Vector2(1, 0);

    public int number_of_segments = 40;
    public float size_of_rope_pieces = 0.2f;
    public List<PlatformerCharacter2D> players_on_rope = new List<PlatformerCharacter2D>();


    void Awake()
    {
        line = this.GetComponent<LineRenderer>();
    }
    void Start()
    {
        //Throw_Rope();
    }


    // Throw direction should be a normalized vector. Initial anchor is probably the player's body
    public void Throw_Rope(Vector3 start_position, Vector2 throw_direction, float throw_force, Rigidbody2D initial_anchor, GameObject thrower)
    {
        // Spawn a number of segments
        for (int i = 0; i < number_of_segments; i++)
        {
            GameObject segment = ((GameObject)Instantiate(emptyPrefab,
                new Vector3(start_position.x, start_position.y, 0), Quaternion.identity));
            joints.Add(segment);
            segment.transform.parent = transform;
        }

        // Connect them with hingejoints
        for (int j = 0; j < joints.Count - 1; j++)
        {
            joints[j].transform.parent = this.transform;
            joints[j].GetComponent<HingeJoint2D>().connectedBody = joints[j + 1].GetComponent<Rigidbody2D>();
        }

        // Set their neighbours
        for (int x = 0; x < number_of_segments; x++)
        {
            int above_int = Mathf.Clamp(x - 1, 0, number_of_segments - 1);
            int below_int = Mathf.Clamp(x + 1, 0, number_of_segments - 1);
            joints[x].GetComponent<Link>().above = joints[above_int];
            joints[x].GetComponent<Link>().below = joints[below_int];
            joints[x].GetComponent<Link>().top_most = joints[0];
            joints[x].GetComponent<Link>().bottom_most = joints[number_of_segments - 1];
            joints[x].GetComponent<Link>().position_from_top_in_rope = x;
            joints[x].GetComponent<Link>().position_from_bottom_in_rope = number_of_segments - x;
            joints[x].GetComponent<Link>().all_segments = joints;
            joints[x].GetComponent<Link>().rope = this;
        }

        // Disable the hingejoint on the last rope semgnet
        if (initial_anchor == null)
        {
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().enabled = false;
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
        }
        else
        {
            // Make the end tied to the player's waist
            /*
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().connectedBody = initial_anchor;
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;

            initial_anchor.GetComponent<PlatformerCharacter2D>().connected_joint = joints[joints.Count - 1].GetComponent<HingeJoint2D>();
            */
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().enabled = false;
            joints[joints.Count - 1].GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;

            // Activate the joint on the player object, and attach it to the end rope segment
            PlatformerCharacter2D player = initial_anchor.GetComponent<PlatformerCharacter2D>();
            player.connected_joint.enabled = true;
            player.connected_joint.connectedBody = joints[joints.Count - 1].GetComponent<Rigidbody2D>();

            //player.rope_follower.SetActive(true);
            //player.rope_follower.GetComponent<FollowObject>().object_to_follow = joints[joints.Count - 1].transform;
            //player.connected_joint.connectedBody = player.rope_follower.GetComponent<Rigidbody2D>();
        }

        // Make the start attachable to the terrain, like a grappling hook
        AttachToTerrain terrain = joints[0].AddComponent<AttachToTerrain>();
        terrain.thrower = thrower;
        terrain.attach_to_thrower_if_in_range = true;
        terrain.attach_range = joints[0].GetComponent<CircleCollider2D>().radius * 2 * joints.Count;

        // First segment always has a spring with which to attack the player to
        SpringJoint2D spring = joints[0].AddComponent<SpringJoint2D>();
        spring.dampingRatio = 0;
        spring.frequency = 0;
        spring.enabled = false;

        // Name the segments
        joints[0].name = "FirstPiece";
        joints[joints.Count - 1].name = "EndPiece";

        // Create attach points at the top and bottom of the rope
        GameObject top = (GameObject) Instantiate(Resources.Load("RopeAttachPoint"), joints[0].transform.position, Quaternion.identity);
        top.transform.parent = joints[0].transform;
        GameObject bottom = (GameObject)Instantiate(Resources.Load("RopeAttachPoint"), joints[joints.Count - 1].transform.position, Quaternion.identity);
        bottom.transform.parent = joints[joints.Count - 1].transform;

        // Make the first segment able to attach to other ropes
        top.AddComponent<RopeCombiner>();

        // Add a force to the first rope segment
        
        for (int z = 0; z < joints.Count; z++)
        {
            Vector2 force_to_add = throw_direction * throw_force * (1f - (float) ((float)(z) / (float)joints.Count));
            joints[z].GetComponent<Rigidbody2D>().AddForce(force_to_add);
        }

        /*
joints[0].GetComponent<Rigidbody2D>().AddForce(throw_direction * throw_force);
joints[1].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.5f));
joints[2].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.4f));
joints[3].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.3f));
joints[4].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.2f));
joints[5].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.1f));
joints[6].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.1f));
joints[7].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.1f));
joints[8].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.1f));
joints[9].GetComponent<Rigidbody2D>().AddForce(throw_direction * (throw_force * 0.1f));*/
        // Add rope to dictionary of ropes
        ObjectManager.object_manager.AddRope(this.gameObject);
    }


    public void Generate_Rope_Between_Anchors()
    {
        if (beginning_anchor == null || end_anchor == null)
        {
            beginning_anchor = gameObject;
            end_anchor = gameObject;
        }
        joints = new List<GameObject>();
        line = GetComponent<LineRenderer>();
        vertexCount = 8;
        // vertexCount = (((int)Vector2.Distance(beginning.transform.position, end.transform.position)) * 3) - 1;

        line.SetWidth(0.1f, 0.1f);  // 0.05f
        Vector3 dir = beginning_anchor.transform.position - end_anchor.transform.position;

        for (int i = 0; i < vertexCount; i++)
        {
            GameObject segment = ((GameObject)Instantiate(emptyPrefab, 
                new Vector3(beginning_anchor.transform.position.x, beginning_anchor.transform.position.y, 0) - ((dir / (float)vertexCount) * i), Quaternion.identity));
            joints.Add(segment);
            segment.transform.parent = transform;
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


        // Set connections on ends
        // Where player is
        /*SpringJoint2D jo = joints[0].AddComponent<SpringJoint2D>();
        jo.connectedBody = beginning.GetComponent<Rigidbody2D>();
        jo.frequency = 0;*/
        /*DistanceJoint2D jo = joints[0].AddComponent<DistanceJoint2D>();
        jo.connectedBody = beginning.GetComponent<Rigidbody2D>();
        jo.distance = 0.2f;*/
        HingeJoint2D jo = joints[0].AddComponent<HingeJoint2D>();
        jo.connectedBody = beginning_anchor.GetComponent<Rigidbody2D>();
        jo = joints[joints.Count - 1].GetComponent<HingeJoint2D>();
        jo.anchor = new Vector2(0, 0);
        jo.connectedBody = end_anchor.GetComponent<Rigidbody2D>();
        joints.Add(end_anchor);

        //end_anchor.GetComponent<Rigidbody2D>().AddForce(throwForce, ForceMode2D.Force);
        //end_anchor.AddComponent<RopeAttachScript>();
        /*
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().connectedBody = end.GetComponent<Rigidbody2D>();
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().anchor = Vector2.zero;
        joints[vertexCount - 1].GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;*/
        //joints[vertexCount - 1].GetComponent<Rigidbody2D>().isKinematic = true;
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