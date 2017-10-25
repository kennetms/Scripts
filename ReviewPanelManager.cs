using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewPanelManager : MonoBehaviour {

    /// <summary>
    /// our array of objects that makes up the review panel
    /// </summary>
    public List<GameObject> reviewPanel;

    /// <summary>
    /// the textbox that will display our object's review information
    /// </summary>
    public Text m_reviewPanelText;

    //the position in the review panel array of the object we are currently displaying
    private int currentReviewPos;

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
        m_reviewPanelText.text = reviewInfo.m_ReviewInfo;
    }
    
    void Update()
    {
        if(currentlyDisplayed != null)
            //display our object's image/move it infront of the camera
            DisplayRotation.CenterObject(currentlyDisplayed);
    }

    //switching review panel
    public void NextReviewObject()
    {
        if (currentReviewPos < reviewPanel.Count - 1)
        {
            DisplayReviewPanelObject(++currentReviewPos);
        }
        else
        {
            currentReviewPos = 0;
            DisplayReviewPanelObject(currentReviewPos);
        }
    }

    //switching review panel
    public void PreviousReviewObject()
    {
        if (currentReviewPos > 0)
        {
            DisplayReviewPanelObject(--currentReviewPos);
        }
        else
        {
            currentReviewPos = reviewPanel.Count - 1;
            DisplayReviewPanelObject(currentReviewPos);
        }
    }
}
