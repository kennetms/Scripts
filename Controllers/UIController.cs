using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    //GameController object that keeps track of score, hints, hazard/safety modes, and time
    public GameController gc;

    //GameObject for our leaderboard entry keyboard
    public GameObject m_KeyboardCanvas;

    //GameObject for our review panel canvas display
    public GameObject m_ReviewPanelCanvas;

    //ReviewPanelManager object that keeps track of the objects that needs to be displayed on the review panel
    public ReviewPanelManager m_ReviewManager;

    //Text object for our score
    public Text m_scoreText;

    //Text object for hints
    public Text m_hintText;

    //Text object for our remaining time
    public Text m_timeText;

    //Text object for our current selection mode.
    public Text m_modeText;

    //Text that shows how much score was added or subtracted
    public Text m_plusOrMinus;

    //Amount of time that the plus or minus text displayed
    public float m_time;


    void Start ()
    {
        //on start we initialize the first UI display
        UpdateUI();
    }

    /// <summary>
    /// UpdateUI Updates each relevant UI component to display new information
    /// </summary>
    public void UpdateUI()
    {
        UpdateSelectionMode();
        UpdateHintsText();
        UpdateScoreText();
        UpdateTimeText();
        UpdatePlusOrMinusText();
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdatePlusOrMinusText()
    {
        m_time -= Time.deltaTime;
        if (m_time < 0)
            m_plusOrMinus.text = "";
    }

    /// <summary>
    /// Updates m_scoreText UI element to reflect new score.
    /// </summary>
    void UpdateScoreText()
    {
        m_scoreText.text = "Score: " + gc.m_Score;
    }

    /// <summary>
    /// Updates m_plusOrMinus UI text to display the user's recent score change.
    /// </summary>
    /// <param name="scoreChange">The value by which the score is changing</param>
    public void DisplayPlusOrMinusText(int scoreChange)
    {
        m_time = 2.0f;

        //increase in score
        if (scoreChange > 0)
        {
            m_plusOrMinus.color = Color.green;
            m_plusOrMinus.text = "+" + scoreChange;
        }
        else //decrease in score
        {
            m_plusOrMinus.color = Color.red;
            m_plusOrMinus.text = "" + scoreChange;
        }

    }

    /// <summary>
    /// Converts the time format of our remaining ingame time to a more user-readable time.
    /// </summary>
    /// <returns>The format for displaying the time</returns>
    private string ConvertTimeFormat()
    {
        //getting how many seconds & minutes we have
        int seconds = (int)gc.m_timeLeft % 60;
        int minutes = (int)gc.m_timeLeft / 60;

        //returning the proper format of the time string
        if(seconds < 10)
            return minutes.ToString() + ":0" + seconds.ToString();       
        else
        return minutes.ToString() + ":" + seconds.ToString();
    }

    /// <summary>
    /// Updates the m_timeText UI element to display the proper time.
    /// </summary>
    void UpdateTimeText()
    {
        m_timeText.text = "Time: " + ConvertTimeFormat();
    }

    /// <summary>
    /// Updates the m_modeText UI element to display the proper selection mode.
    /// </summary>
    void UpdateSelectionMode()
    {
        string updatedText = "Selection Mode: ";

        //m_selectionmode true -> hazard mode
        if (gc.m_HazardMode)
        {
            updatedText += "Hazard";
        }
        else
        {
            updatedText += "Safety";
        }

        m_modeText.text = updatedText;
    }

    /// <summary>
    /// Updates the m_hintText UI element to display the proper amount of hints remaining.
    /// </summary>
    void UpdateHintsText()
    {
        m_hintText.text = "Hints: " + gc.m_Hints;
    }

    /// <summary>
    /// Sets up the keyboard after the ingame round is over.
    /// </summary>
    public void SetupKeyboard()
    {
        //disable the ingame UI
        Canvas UICanvas= m_scoreText.GetComponentInParent<Canvas>();
        UICanvas.enabled = false;

        //load our keyboard object
        LoadKeyboard();
    }

    /// <summary>
    /// Activates our keyboard to display for the user
    /// </summary>
    private void LoadKeyboard()
    {
        m_KeyboardCanvas.SetActive(true);
    }   

    /// <summary>
    /// Sets up the review panel after the user is done entering their initials for the leaderboard.
    /// </summary>
    public void SetupReviewPanel()
    {
        //disabling the keyboard
        m_KeyboardCanvas.SetActive(false);

        //initializing & enabling the review panel
        m_ReviewManager.InitializeReviewPanel();
        m_ReviewPanelCanvas.SetActive(true);
    }
}
