using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Control : MonoBehaviour
{
    AudioSource m_MyAudioSource;
    ReviewInformation m_ReviewInfo;

    void Start()
    {
        //Fetch the AudioSource and ReviewInformation from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        m_ReviewInfo = GetComponent<ReviewInformation>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if Interacted With
        if (m_ReviewInfo.Interacted)
        {
            //Stop the audio
            m_MyAudioSource.Stop();
        }
    }
}
