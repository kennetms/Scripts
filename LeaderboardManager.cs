using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {

    public GlobalController m_globalController;

    #region Text Objects
    public Text m_EasyText;

    public Text m_MediumText;

    public Text m_HardText;

    public Text m_CombinedText;
    #endregion

    // Use this for initialization
    void Start ()
    {
        m_globalController = GlobalController.GetInstance();

        SetupLeaderboard();
    }

    public void SetupLeaderboard()
    {
        //Setup the text for each leaderboard based on which text objects to update, using a PlayerInfo leaderboard.
        SetupSingleLeaderboard(m_EasyText,GlobalController.easyLeaderboard);
        SetupSingleLeaderboard(m_MediumText, GlobalController.mediumLeaderboard);
        SetupSingleLeaderboard(m_HardText, GlobalController.hardLeaderboard);
        SetupSingleLeaderboard(m_CombinedText, GlobalController.combinedLeaderboard,true);
    }

    /// <summary>
    /// Sets up a single leaderboard textbox to display the proper GlobalController.PlayerInfo
    /// </summary>
    /// <param name="leaderboardText">The Text object to modify</param>
    /// <param name="leaderboard">The array of player information that makes up the leaderboard</param>
    /// <param name="combinedLeaderboard">flag to tell us if this leaderboard is "combined"; it must contain difficulty in the text if so</param>
    private void SetupSingleLeaderboard(Text leaderboardText, GlobalController.PlayerInfo[] leaderboard, bool combinedLeaderboard = false)
    {
        if(!combinedLeaderboard)
        {
            //not a combined leaderboard, so all we display is player name & score
            leaderboardText.text = "";
            foreach (GlobalController.PlayerInfo player in leaderboard)
            {
                if (player != null)
                    leaderboardText.text += player.name + " : " + player.score + "\n";
            }
        }
        else
        {
            //the combined leaderboard, we must also display difficulty.
            foreach (GlobalController.PlayerInfo player in GlobalController.combinedLeaderboard)
            {
                if (player != null)
                    m_CombinedText.text += player.name + " : " + player.score + " (" + player.difficulty + ")" + "\n";
            }
        }

    }
}