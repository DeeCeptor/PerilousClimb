using UnityEngine;
using System.Collections;

public class PlayerRopeThrower : MonoBehaviour {

    public GameObject ropePiece;
    //how many rope Sections to spawn
    public int ropeSections = 10;
    private Vector3 aimSpot;
    public int playerRopes = 10;
    public float throwForce = 5000;


    void Start ()
    {
	
	}


    void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Mouse0) && playerRopes > 0)
        {
            playerRopes--;
            aimSpot = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = aimSpot - transform.position;
            GameObject ropeParent = (GameObject)Instantiate(Resources.Load("ropeParent"),transform.position,transform.rotation);
            
            RopeGenerator rope = ropeParent.AddComponent<RopeGenerator>();
            GameObject start = (GameObject)Instantiate(ropePiece, transform.position, transform.rotation);
            start.transform.SetParent(ropeParent.transform);
            rope.beginning_anchor = start;
            GameObject end = (GameObject)Instantiate(ropePiece, transform.position, transform.rotation);
            end.transform.SetParent(ropeParent.transform);
            rope.end_anchor = end;
            rope.emptyPrefab = (GameObject)Resources.Load("RopePiece");
            //rope.throwForce = dir.normalized*throwForce;
            //rope.Throw_Rope();
        }
	}
}
