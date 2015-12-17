using UnityEngine;
using System.Collections.Generic;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation player_information;
    public List<PlatformerCharacter2D> players = new List<PlatformerCharacter2D>();

    void Awake()
    {
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
        player_information = this;
        for(int i = 0;i<transform.childCount;i++)
        {
            players.Add(playerArray[i].GetComponent<PlatformerCharacter2D>());
        }
    }

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
}
