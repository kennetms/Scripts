using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the transitions from In game action to
/// Keyboard to review panel
/// inheirits from SceneSwitcher to allow for management of scene switches as well
/// </summary>
public class TransitionManager : SceneSwitcher
{
    //the keyboard interface to display to the user
    public VRKeyboard m_Keyboard;

    //the manager of the review panel, used to load the review panel
    public ReviewPanelManager m_ReviewPanel;

    /// <summary>
    /// Transitions to the keyboard in scene
    /// </summary>
    public void TransitionToKeyboard()
    {
        //Load the keyboard for the user to enter their name for the leaderboard.
        m_Keyboard.LoadKeyboard();
    }

    /// <summary>
    /// Transitions to the Review Panel in scene.
    /// </summary>
    public void TransitionToReviewPanel()
    {
        //disable the keyboard on the transition to the review panel
        m_Keyboard.DisableKeyboard();

        //load the review panel in scene
        m_ReviewPanel.LoadReviewPanel();
    }
}