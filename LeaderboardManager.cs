using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {

    public GlobalController m_globalController;

    public Text m_leaderboardText;

	// Use this for initialization
	void Start ()
    {
        //global is a general game object with the globalcontroller tag holding our GlobalController script.
        GameObject global = GameObject.FindGameObjectWithTag("globalcontroller");
        if (global == null)
            Debug.LogError("No GlobalController to set Leaderboard.");

        //setting our relation to global controller and getting our m_difficulty.
        m_globalController = global.GetComponent<GlobalController>();

        string leaderText = "";
        foreach (GlobalController.PlayerInfo player in GlobalController.leaderboard)
        {
            if(player!= null)
            leaderText += player.name + " : " + player.score + "\n";
        }

        m_leaderboardText.text = leaderText;
    }
}
