using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureTracker : MonoBehaviour
{
    public static TreasureTracker scoreTrack;
    public List<int> playerScores;
    //TODO make UI
    public GameObject Scorepanel;

    // Use this for initialization
    void Start()
    {
        scoreTrack = this;
        for (int i = 0; i < PlayerInformation.player_information.players.Count; i++)
        {
            playerScores.Add(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerInformation.player_information.players.Count != playerScores.Count)
        {
            playerScores.Clear();
            for (int i = 0; i < PlayerInformation.player_information.players.Count; i++)
            {
                playerScores.Add(0);
            }
        }

    }
}
