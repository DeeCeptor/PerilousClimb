using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnOnTimeScript : MonoBehaviour {
    public static RespawnOnTimeScript Respawner;
    private PlayerInformation info;
    public List<PlatformerCharacter2D> players;
    //array of whether any given player is awaiting respawn
    public List<bool> playersToRespawn;
    //array of timer for respawns
    public float respawnTime = 2f;
    public List<float> respawnTimers;
	// Use this for initialization
	void Start () {
        Respawner = this;
        info = PlayerInformation.player_information;
        playersToRespawn = new List<bool>();
        for (int i = 0; i < info.players.Count; i++)
        {
            playersToRespawn.Add(false);
        }
        respawnTimers = new List<float>();

        for (int i = 0; i < info.players.Count; i++)
        {
            respawnTimers.Add(0);
        }

        players = new List<PlatformerCharacter2D>();
        for (int i = 0; i < info.players.Count; i++)
        {
            players.Add(info.players[i]);
        }
    }

    public void playerJoined()
    {
        playersToRespawn.Add(false);
        
        respawnTimers.Add(0);
        players.Add(info.players[info.players.Count-1]);
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    for(int i = 0;i < playersToRespawn.Count;i++)
        {
            if(playersToRespawn[i] && respawnTimers[i]<Time.time)
            {
                playersToRespawn[i] = false;
                players[i].transform.position = transform.position;
                players[i].Respawn();
            }
        }
	}

    public void playerDied(int playerNumber)
    {
        playersToRespawn[playerNumber] = true;
        respawnTimers[playerNumber] = Time.time + respawnTime;
    }
}
