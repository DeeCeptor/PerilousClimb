using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnOnTimeScript : MonoBehaviour {
    public static RespawnOnTimeScript Respawner;
    private PlayerInformation info;
    public PlatformerCharacter2D[] players;
    //array of whether any given player is awaiting respawn
    public bool[] playersToRespawn;
    //array of timer for respawns
    public float respawnTime = 2f;
    public float[] respawnTimers;
	// Use this for initialization
	void Start () {
        Respawner = this;
        info = PlayerInformation.player_information;
        playersToRespawn = new bool[info.players.Count];
        for (int i = 0; i < playersToRespawn.Length; i++)
        {
            playersToRespawn[i] = false;
        }
        respawnTimers = new float[info.players.Count];
        players = new PlatformerCharacter2D[info.players.Count];

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = info.players[i];
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    for(int i = 0;i < playersToRespawn.Length;i++)
        {
            if(playersToRespawn[i] && respawnTimers[i]<Time.time)
            {
                playersToRespawn[i] = false;
                players[i].transform.position = transform.position;
                //players[i].Respawn();
            }
        }
	}

    public void playerDied(int playerNumber)
    {
        playersToRespawn[playerNumber] = true;
        respawnTimers[playerNumber] = Time.time + respawnTime;
    }
}
