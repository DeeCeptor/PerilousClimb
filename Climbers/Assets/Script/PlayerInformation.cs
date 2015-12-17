using UnityEngine;
using System.Collections.Generic;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation player_information;
    public List<PlatformerCharacter2D> players = new List<PlatformerCharacter2D>();

    void Awake()
    {
        player_information = this;
    }

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
}
