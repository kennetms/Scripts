using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles keeping track of and displaying the review panel.
/// </summary>
public class ReviewPanelManager : MonoBehaviour {

    /// <summary>
    /// our array of objects that makes up the review panel
    /// </summary>
    private List<GameObject> reviewPanel;

    //GameObject for our review panel canvas display
    public GameObject m_ReviewPanelCanvas;

    /// <summary>
    /// the textbox that will display our object's review information
    /// </summary>
    public Text m_reviewPanelText;

    //the position in the review panel array of the object we are currently displaying
    private int m_currentReviewPos;

    //the GameObject being displayed on the review panel currently. 
    private GameObject m_currentlyDisplayed;

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        m_ReviewPanelCanvas.SetActive(false);
        reviewPanel = new List<GameObject>();
        enabled = false;
    }

    /// <summary>
    /// Add a GameObject to the review panel
    /// </summary>
    /// <param name="obj">The object to add to the review panel</param>
    public void AddReviewPanelObject(GameObject obj)
    {
        reviewPanel.Add(obj);
    }

    /// <summary>
    /// Load the review panel
    /// </summary>
    public void LoadReviewPanel()
    {
        //initialize the review panel
        InitializeReviewPanel();

        //set our review panel canvas to active
        m_ReviewPanelCanvas.SetActive(true);

        enabled = true;
    }

    /// <summary>
    /// Initializes the review panel by initalizing the displaying of objects and information,
    /// starting with the first object added to the review panel.
    /// </summary>
    private void InitializeReviewPanel()
    {
        //we need a check for the situation where we have no review panel objects
        if (reviewPanel.Count == 0)
        {
            m_reviewPanelText.text = "No Review Panel Objects!";
            return;
        }

        //initialize the position in the review panel array of our first object to display
        m_currentReviewPos = 0;

        //display the first object
        DisplayReviewPanelObject(m_currentReviewPos);
    }


    /// <summary>
    /// Displays the review panel object in the review panel array at index index.
    /// </summary>
    /// <param name="index">the index of the object in the review panel to display</param>
    private void DisplayReviewPanelObject(int index)
    {
        //if we're already displaying something, move it out of camera view
        if(m_currentlyDisplayed != null)
            m_currentlyDisplayed.transform.position = new Vector3(0, 0, 0);

        //the GameObject we're displaying
        m_currentlyDisplayed = reviewPanel[index];
        /**
        Collider objColl = m_currentlyDisplayed.GetComponent<Collider>();
        if (objColl != null)
            objColl.enabled = false;*/

        //we are getting the object's information from the review panel
        ReviewInformation reviewInfo = m_currentlyDisplayed.GetComponent<ReviewInformation>();

        //if we have no review info, we default to display text that this is an innocuous item.
        if (reviewInfo == null)
        {
            m_reviewPanelText.text = m_currentlyDisplayed.name + " is an innocuous object; it is neither a hazard nor a safety.";
            return;
        }

        //displaying our review info for the object
        m_reviewPanelText.text = reviewInfo.ReviewInfo;
    }
    
    void Update()
    {
        if (m_currentlyDisplayed != null)
            //display our object's image/move it infront of the camera
            DisplayRotation.RotateView(m_currentlyDisplayed);
    }

    /// <summary>
    /// going to the next object in the review panel
    /// </summary>
    public void NextReviewObject()
    {
        //looping to first object if we're currently displaying the last
        if (m_currentReviewPos < reviewPanel.Count - 1)
        {
            //no need to loop
            DisplayReviewPanelObject(++m_currentReviewPos);
        }
        else
        {
            //loop to beginning of list
            m_currentReviewPos = 0;
            DisplayReviewPanelObject(m_currentReviewPos);
        }
    }

    /// <summary>
    /// backing up to the previous object displayed in the review panel
    /// </summary>
    public void PreviousReviewObject()
    {
        //loop to last object if we're currently displaying the first.
        if (m_currentReviewPos > 0)
        {
            //no need to loop
            DisplayReviewPanelObject(--m_currentReviewPos);
        }
        else
        {
            //loop to end of list
            m_currentReviewPos = reviewPanel.Count - 1;
            DisplayReviewPanelObject(m_currentReviewPos);
        }
    }
}