using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///An extension of ObjectInformation that is exclusively used for hazards and safetys,
///and is placed on those objects to show they are interactable & they have information
//for the review panel.
public class ReviewInformation : ObjectInformation {
    public string m_ReviewInfo;
    public string m_HintInfo;
}
