using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class that applies a material outline to a GameObject.
/// 
/// This class is utilized by the GameController to apply an outline to
/// objects that the user has interacted with in scene.
/// </summary>
public class OutlineApplier : MonoBehaviour
{
    //an enumeration to determine the color outline selected
    private enum OutlineColor { Green, Red, Orange }

    //our outline materials
    public Material m_RedOutline;
    public Material m_GreenOutline;
    public Material m_OrangeOutline;

    /// <summary>
    /// abstraction/encapsulation for applying a generic outline; we can get the OutlineColor
    /// to use from public methods and handle the code for applying the outline here
    /// </summary>
    /// <param name="obj">The GameObject we're applying the outline to.
    /// obj can be either a 3D GameObject with a Renderer component or an image with an Outline component.</param>
    /// <param name="oc">The outine color we want for the object</param>
    private void ApplyOutline(GameObject obj, OutlineColor oc)
    {
        //some hazards in game are just images on a canvas; we want to check for those
        //GameObjects that may already have an outline on them.
        Outline imgOutline = obj.GetComponent<Outline>();

        //no outline; not an image hazard, apply outlines through materials.
        if (imgOutline == null)
        {
            for(int i = 0; i < obj.transform.childCount; ++i)
                ApplyOutline(obj.transform.GetChild(i).gameObject,oc);

            Renderer rend = obj.GetComponent<Renderer>();

            if(rend != null)
            {
                Texture currentTexture = rend.material.mainTexture;
                //Texture currentNorm = rend.material.GetTexture("_BumpMap");
                switch (oc)
                {
                    case OutlineColor.Green:
                        //set green material
                        rend.material = m_GreenOutline;
                        rend.material.mainTexture = currentTexture;
                        break;
                    case OutlineColor.Orange:
                        //set orange material
                        rend.material = m_OrangeOutline;
                        rend.material.mainTexture = currentTexture;
                        //rend.material.SetTexture("_BumpMap", currentNorm);
                        //rend.material.SetFloat("_BumpScale", 1.0f);
                        //rend.material.EnableKeyword("_NORMALMAP");

                        //Debug.LogError(rend.material.GetTexture("_BumpMap").name);
                        break;
                    case OutlineColor.Red:
                        //set red matieral
                        rend.material = m_RedOutline;
                        rend.material.mainTexture = currentTexture;
                        break;
                }
            }

        }
        else //obj has an outline component (it's an image), so apply the proper color to the outline and enable it.
        {
            //we need to pass in a color for changing an Outline component's effectColor
            Color outline = Color.white;

            switch (oc)
            {
                case OutlineColor.Green:
                    outline = Color.green;
                    break;
                case OutlineColor.Orange:
                    //Color has no predefined orange; this new Color is just a basic orange.
                    outline = new Color(1, .647f, 0);
                    break;
                case OutlineColor.Red:
                    outline = Color.red;
                    break;
            }

            //actually applying the color & enabling.
            imgOutline.effectColor = outline;
            imgOutline.enabled = true;
            imgOutline.effectDistance = new Vector2(5.0f, 5.0f);
        }
    }

    /// <summary>
    /// Applies a green outline to the object parameter
    /// </summary>
    /// <param name="obj">The GameObject we're applying the outline to.
    /// obj can be either a 3D GameObject with a Renderer component or an image with an outline component.</param>
    public void ApplyGreenOutline(GameObject obj)
    {
        ApplyOutline(obj, OutlineColor.Green);
    }
    /// <summary>
    /// Applies a orange outline to the object parameter
    /// </summary>
    /// <param name="obj">The GameObject we're applying the outline to.
    /// obj can be either a 3D GameObject with a Renderer component or an image with an outline component.</param>
    public void ApplyOrangeOutline(GameObject obj)
    {
        ApplyOutline(obj, OutlineColor.Orange);
    }
    /// <summary>
    /// Applies a red outline to the object parameter
    /// </summary>
    /// <param name="obj">The GameObject we're applying the outline to.
    /// obj can be either a 3D GameObject with a Renderer component or an image with an outline component.</param>
    public void ApplyRedOutline(GameObject obj)
    {
        ApplyOutline(obj, OutlineColor.Red);
    }
}
