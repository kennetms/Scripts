using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles audio for hazard, safety, and innocuous objects.
/// </summary>
public class Audio_Control : MonoBehaviour
{
    private AudioSource m_MyAudioSource;
    private ObjectInformation m_objInfo;

    void Start()
    {
        //Fetch the AudioSource and ReviewInformation from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        m_ReviewInfo = GetComponent<ObjectInformation>();

        if(m_ReviewInfo == null)
        {
            Debug.LogError(gameObject.name + " has no object information for AudioController.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if Interacted With
        if (m_objInfo.Interacted)
        {
            //Stop the audio
            m_MyAudioSource.Stop();
        }
    }
}
