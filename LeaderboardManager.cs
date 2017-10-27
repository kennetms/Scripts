using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {

    public GlobalController m_globalController;

    public Text m_EasyText;

    public Text m_MediumText;

    public Text m_HardText;

    public Text m_CombinedText;

    // Use this for initialization
    void Start ()
    {
        //global is a general game object with the globalcontroller tag holding our GlobalController script.
        GameObject global = GameObject.FindGameObjectWithTag("globalcontroller");
        if (global == null)
            Debug.LogError("No GlobalController to set Leaderboard.");

        //setting our relation to global controller and getting our m_difficulty.
        m_globalController = global.GetComponent<GlobalController>();

        SetupLeaderboard();
    }

    public void SetupLeaderboard()
    {
        SetupSingleLeaderboard(m_EasyText,GlobalController.easyLeaderboard);
        SetupSingleLeaderboard(m_MediumText, GlobalController.mediumLeaderboard);
        SetupSingleLeaderboard(m_HardText, GlobalController.hardLeaderboard);
        SetupSingleLeaderboard(m_CombinedText, GlobalController.combinedLeaderboard,true);
    }

    private void SetupSingleLeaderboard(Text leaderboardText, GlobalController.PlayerInfo[] leaderboard, bool combinedLeaderboard = false)
    {
        if(!combinedLeaderboard)
        {
            leaderboardText.text = "";
            foreach (GlobalController.PlayerInfo player in leaderboard)
            {
                if (player != null)
                    leaderboardText.text += player.name + " : " + player.score + " (" + player.difficulty + ")" + "\n";
            }
        }
        else
        {
            foreach (GlobalController.PlayerInfo player in GlobalController.combinedLeaderboard)
            {
                if (player != null)
                    m_CombinedText.text += player.name + " : " + player.score + " (" + player.difficulty + ")" + "\n";
            }
        }

    }
}
