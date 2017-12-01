using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An extension of ObjectInformation that is exclusively used for hazards and safetys,
/// and is placed on those objects to show they are interactable & they have information
/// for the review panel.
/// </summary>
public class ReviewInformation : ObjectInformation {

    //the object's review information
    [SerializeField] private string m_ReviewInfo;

    //the object's hint information
    [SerializeField] private string m_HintInfo;

    //accessor for an object's Review Info
    public string ReviewInfo { get { return m_ReviewInfo; } }

    //accessor for an object's Hint Info
    public string HintInfo { get { return m_HintInfo; } }
}
