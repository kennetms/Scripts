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

    /// <summary>
    /// the textbox that will display our object's review information
    /// </summary>
    public Text m_reviewPanelText;

    //the position in the review panel array of the object we are currently displaying
    private int currentReviewPos;

    //the GameObject being displayed on the review panel currently. 
    private GameObject currentlyDisplayed;

    void Start()
    {
        reviewPanel = new List<GameObject>();
    }

    public void AddReviewPanelObject(GameObject obj)
    {
        reviewPanel.Add(obj);
    }

    public void InitializeReviewPanel()
    {
        //we need a check for the situation where we have no review panel objects
        if (reviewPanel.Count == 0)
        {
            m_reviewPanelText.text = "No Review Panel Objects!";
            return;
        }

        //initialize the position in the review panel array of our first object to display
        currentReviewPos = 0;

        //display the first object
        DisplayReviewPanelObject(currentReviewPos);
    }
    private void DisplayReviewPanelObject(int pos)
    {
        //if we're already displaying something, move it out of camera view
        if(currentlyDisplayed != null)
            currentlyDisplayed.transform.position = new Vector3(0, 0, 0);

        //the GameObject we're displaying
        currentlyDisplayed = reviewPanel[pos];
        Collider objColl = currentlyDisplayed.GetComponent<Collider>();
        if (objColl != null)
            objColl.enabled = false;

        //we are getting the object's information from the review panel
        ReviewInformation reviewInfo = currentlyDisplayed.GetComponent<ReviewInformation>();

        //if we have no review info, we default to display text that this is an innocuous item.
        if (reviewInfo == null)
        {
            m_reviewPanelText.text = currentlyDisplayed.name + " is an innocuous object; it is neither a hazard nor a safety.";
            return;
        }

        //displaying our review info for the object
        m_reviewPanelText.text = reviewInfo.ReviewInfo;
    }
    
    void Update()
    {
        if(currentlyDisplayed != null)
            //display our object's image/move it infront of the camera
            DisplayRotation.CenterObject(currentlyDisplayed);
    }

    //going to the next object in the review panel
    public void NextReviewObject()
    {
        //looping to first object if we're currently displaying the last
        if (currentReviewPos < reviewPanel.Count - 1)
        {
            //no need to loop
            DisplayReviewPanelObject(++currentReviewPos);
        }
        else
        {
            //loop to beginning of list
            currentReviewPos = 0;
            DisplayReviewPanelObject(currentReviewPos);
        }
    }

    //backing up to the previous object displayed in the review panel
    public void PreviousReviewObject()
    {
        //loop to last object if we're currently displaying the first.
        if (currentReviewPos > 0)
        {
            //no need to loop
            DisplayReviewPanelObject(--currentReviewPos);
        }
        else
        {
            //loop to end of list
            currentReviewPos = reviewPanel.Count - 1;
            DisplayReviewPanelObject(currentReviewPos);
        }
    }
}