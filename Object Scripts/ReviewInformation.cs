using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///An extension of ObjectInformation that is exclusively used for hazards and safetys,
///and is placed on those objects to show they are interactable & they have information
//for the review panel.
public class ReviewInformation : ObjectInformation {
    [SerializeField] private string m_ReviewInfo;
    [SerializeField] private string m_HintInfo;

    //accessor for an object's Review Info
    public string ReviewInfo { get { return m_ReviewInfo; } }

    //accessor for an object's Hint Info
    public string HintInfo { get { return m_HintInfo; } }
}
