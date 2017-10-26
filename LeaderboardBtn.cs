using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// script for the leaderboard button on the main menu; loads leaderboard scene on press
public class LeaderboardBtn : MonoBehaviour
{

    public void leaderboardGameBtn(string newGameLvl)
    {
        SceneManager.LoadScene(newGameLvl);
    }
}
