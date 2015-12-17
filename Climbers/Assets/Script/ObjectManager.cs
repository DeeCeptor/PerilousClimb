using UnityEngine;
using System.Collections.Generic;

// Keeps track of ropes and other objects we might want to remove due to it being computationally expensive
public class ObjectManager : MonoBehaviour 
{
    public static ObjectManager object_manager;

    public List<GameObject> all_ropes = new List<GameObject>();
    int max_number_of_ropes = 6;

	void Awake () 
	{
        object_manager = this;
	}
	
	void Update () 
	{
	
	}


    // Adds a rope to the list of ropes, deletes a rope if too many are onscreen already
    public void AddRope(GameObject rope_parent)
    {
        if (all_ropes.Count >= max_number_of_ropes)
            RemoveRope(all_ropes[0]);

        all_ropes.Add(rope_parent);
    }


    // Removes and deletes the rope from the list
    public void RemoveRope(GameObject rope_parent)
    {
        // Remove all instances of rope_parent from our lists
        while(all_ropes.Remove(rope_parent))    { }
        GameObject.Destroy(rope_parent);
    }
}
