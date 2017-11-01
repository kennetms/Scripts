using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns off the Halo component of spheres walked through.
/// Also creates a new path if the first path is complete
/// </summary>
public class TriggerSphere : MonoBehaviour
{
    //GameObject tutorial controller
    public GameObject m_TutorialContoller;

    public void OnTriggerEnter()
    {

        gameObject.SetActive(false);

        GameObject path1 = m_TutorialContoller.GetComponent<TutorialController>().Path1;
        GameObject path2 = m_TutorialContoller.GetComponent<TutorialController>().Path2;
        GameObject path3 = m_TutorialContoller.GetComponent<TutorialController>().Path3;
        GameObject path4 = m_TutorialContoller.GetComponent<TutorialController>().Path4;
        GameObject path5 = m_TutorialContoller.GetComponent<TutorialController>().Path5;
        GameObject path6 = m_TutorialContoller.GetComponent<TutorialController>().Path6;

        GameObject blockade1 = m_TutorialContoller.GetComponent<TutorialController>().m_blockade1;
        GameObject blockade2 = m_TutorialContoller.GetComponent<TutorialController>().m_blockade2;


        if (gameObject.name == "TriggerSphere")
        {
            path2.SetActive(true);
            path1.SetActive(false);
            m_TutorialContoller.GetComponent<TutorialController>().SetState(1); //FirstPath
            blockade1.SetActive(false);
            gameObject.SetActive(false);
        }

        if (gameObject.name == "TriggerSphere2")
        {
            path3.SetActive(true);
            path2.SetActive(false);
            m_TutorialContoller.GetComponent<TutorialController>().SetState(2); //SecondPath
            blockade2.SetActive(true);
            gameObject.SetActive(false);
        }

        if (gameObject.name == "TriggerSphere3")
        {
            m_TutorialContoller.GetComponent<TutorialController>().SetState(3); //ThirdPath
            path3.SetActive(false);
            gameObject.SetActive(false);
        }
        if (gameObject.name == "TriggerSphere4")
        {
            m_TutorialContoller.GetComponent<TutorialController>().SetState(5); //ForthPath
            path4.SetActive(false);
        }

        if (gameObject.name == "TriggerSphere5")
        {
            m_TutorialContoller.GetComponent<TutorialController>().SetState(7); //FifthPath
            path5.SetActive(false);
        }

        if (gameObject.name == "TriggerSphere6")
        {
            m_TutorialContoller.GetComponent<TutorialController>().SetState(9); //SixthPath
            path6.SetActive(false);
        }
    }
}
