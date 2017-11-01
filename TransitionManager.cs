using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that manages the transitions from In game action to
/// Keyboard to review panel
/// </summary>
public class TransitionManager : MonoBehaviour
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

    /// <summary>
    /// If a button is clicked, use a Unity Event Trigger that is
    /// associated with the ButtonManager and the ButtonClicked function
    /// loads the appropriate scene
    /// </summary>
    /// <param name="scene">A string denoting the name of the scene to load</param>
    public void ButtonClicked(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}