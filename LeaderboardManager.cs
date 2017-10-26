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

        SetLeaderBoardText();
    }

    public void SetLeaderBoardText()
    {
        m_EasyText.text = m_MediumText.text = m_HardText.text = m_CombinedText.text = "";
        foreach (GlobalController.PlayerInfo player in GlobalController.easyLeaderboard)
        {
            if (player != null)
                m_EasyText.text += player.name + " : " + player.score + "\n";
        }
        foreach (GlobalController.PlayerInfo player in GlobalController.mediumLeaderboard)
        {
            if (player != null)
                m_MediumText.text += player.name + " : " + player.score + "\n";
        }
        foreach (GlobalController.PlayerInfo player in GlobalController.hardLeaderboard)
        {
            if (player != null)
                m_HardText.text += player.name + " : " + player.score + "\n";
        }
        foreach (GlobalController.PlayerInfo player in GlobalController.combinedLeaderboard)
        {
            if (player != null)
                m_CombinedText.text += player.name + " : " + player.score + " (" + player.difficulty + ")" +  "\n";
        }
    }
}
