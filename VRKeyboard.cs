/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

// simple on-screen keyboard using Unity UI system

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VRKeyboard : MonoBehaviour {

    ///Our game controller object
    //public GameController m_GameController;

    public TransitionManager m_TransitionManager;

    ///the m_KeyboardCanvas on which the keyboard is displayed
    public GameObject m_KeyboardCanvas;

    ///the button prototype in which we will create each individual key
    public GameObject buttonPrototype;

    ///The text displaying the user's name above the keyboard
    public Text displayText;

    ///the X,Y origin of the keyboard position on the m_KeyboardCanvas
    public Vector2 origin;

    ///the Key spacing and sizing of each key on the keyboard m_KeyboardCanvas
    public Vector2 spacing;

    //public Vector3 keyTranslation;

    ///flag to tell us if we're in caps mode or not
    public bool capsShift = false;

    /// <summary>
    /// Loads the keyboard to be displayed
    /// </summary>
    public void LoadKeyboard()
    {
        enabled = true;
        m_KeyboardCanvas.SetActive(true);
    }

    public void DisableKeyboard()
    {
        m_KeyboardCanvas.SetActive(false);
        enabled = false;
    }

    /// <summary>
    /// A class to contain all the information we need to know about a key on the keyboard
    /// </summary>

    class Key
    {
        //the name of the key
        public string name;

        //the lowercase character
        public string character;

        //uppercase character
        public string shiftedCharacter;

        //the Keycode of this key
        public KeyCode code;

        //the button the key uses
        public GameObject button;
        public bool pressed;

        //flag to indicate special keys (backspace,enter,spacebar,shift)
        public bool special;
    };

    ///the list of all keys on the keyboard
    List<Key> keys;

    ///map from key codes to keys
    Dictionary<KeyCode, Key> keyCodeToKey = new Dictionary<KeyCode, Key>(); 

    /// <summary>
    /// Creates a single key
    /// </summary>
    /// <param name="name">the name of the key</param>
    /// <param name="_char">the lowercase character of the key</param>
    /// <param name="shiftedChar">the uppercase character of the key</param>
    /// <param name="code">the Keycode of the key</param>
    /// <param name="pos">the position on the keyboard we will offset by</param>
    /// <param name="scale">the scaling of the specific key</param>
    /// <param name="special">flag to tell us if we have a special key, such as backspace, spacebar, enter, or shift</param>
    void CreateKey(string name, string _char, string shiftedChar, KeyCode code, Vector2 pos, Vector2 scale, bool special=false)
    {
        Key key = new Key();
        key.name = name;
        key.character = _char;
        key.shiftedCharacter = shiftedChar;
        key.code = code;
        key.pressed = false;
        key.special = special;

        // create button with our button prototype object as a template
        GameObject obj = (GameObject)Object.Instantiate(buttonPrototype);
        obj.name = name;
        obj.transform.SetParent(m_KeyboardCanvas.transform, false);
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
        obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spacing.x * scale.x);
        obj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spacing.y * scale.y);

        obj.GetComponent<Button>().onClick.AddListener(() => ButtonPressed(key));
        obj.GetComponentInChildren<Text>().text = name;
        obj.GetComponentInChildren<Text>().fontSize = 30;

        key.button = obj;

        keys.Add(key);
        keyCodeToKey.Add(code, key);
    }

    /// <summary>
    /// creates a row of keys
    /// </summary>
    /// <param name="chars">the continuous string of lowercase characters which we want to create keys for</param>
    /// <param name="shiftedChars">the continuous string of uppercase characters which we want to create keys for</param>
    /// <param name="codes">the array of keycodes to map keycodes to the strings of characters to create</param>
    /// <param name="pos">the posion on the keyboard by which we will offset to place the key</param>
    /// <param name="offset">the offset by which we space each key</param>
    void CreateKeyRow(string chars, string shiftedChars, KeyCode[] codes, Vector2 pos, float offset)
    {
        pos.x += offset*spacing.x;
        for (int i = 0; i < chars.Length; i++)
        {
            CreateKey(chars[i].ToString(), chars[i].ToString(), shiftedChars[i].ToString(), codes[i], pos, Vector2.one);
            pos.x += spacing.x;
        }
    }

    /// <summary>
    /// Creates the whole keyboard
    /// </summary>
    void CreateKeyboard()
    {
        keys = new List<Key>();

        Vector2 pos = origin;

        //create the first row of our keyboard; 1234567890-= lowercase, !@#$%^&*()_+ uppercase
        CreateKeyRow(
            "1234567890-=",
            "!@#$%^&*()_+",
            new KeyCode[] {
                KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
                KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
                KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
                KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals },
            pos,
            0.0f
            );

        //create the special key backspace at the end of the keyrow we just created
        CreateKey("Bksp", "\b", "\b", KeyCode.Backspace, pos + new Vector2(spacing.x * 12.5f, 0.0f), new Vector2(2.0f, 1.0f), true);

        //increment our next row spacing for y to be below the previous row we created
        pos.y -= spacing.y;

        //create the second row of our keyboard
        CreateKeyRow(
            "qwertyuiop[]",
            "QWERTYUIOP{}",
            new KeyCode[] {
                KeyCode.Q, KeyCode.W, KeyCode.E,
                KeyCode.R, KeyCode.T, KeyCode.Y,
                KeyCode.U, KeyCode.I, KeyCode.O,
                KeyCode.P, KeyCode.LeftBracket, KeyCode.RightBracket },
            pos,
            0.5f
        );

        //create the special key enter at the end of the keyrow we just created
        CreateKey("Enter", "\r", "\r", KeyCode.Return, pos + new Vector2(spacing.x * 12.85f, -spacing.y * 0.5f), new Vector2(1.3f, 2.0f), true);

        //increment y spacing
        pos.y -= spacing.y;

        //create the third row of our keyboard
        CreateKeyRow(
            "asdfghjkl;'\\",
            "ASDFGHJKL:\"||",
            new KeyCode[] {
                KeyCode.A, KeyCode.S, KeyCode.D,
                KeyCode.F, KeyCode.G, KeyCode.H,
                KeyCode.J, KeyCode.K, KeyCode.L,
                KeyCode.Semicolon, KeyCode.Quote, KeyCode.Backslash },
            pos,
            0.7f
            );

        //increment y spacing
        pos.y -= spacing.y;

        //create the fourth row of our keyboard
        CreateKeyRow(
            "zxcvbnm,./",
            "ZXCVBNM<>?",
            new KeyCode[] {
                KeyCode.Z, KeyCode.X, KeyCode.C,
                KeyCode.V, KeyCode.B, KeyCode.N,
                KeyCode.M, KeyCode.Comma, KeyCode.Period, KeyCode.Slash },
            pos,
            1.0f
            );

        //create the special key shift at the end of the keyrow we just created
        CreateKey("Shift", "", "", KeyCode.RightShift, pos + new Vector2(spacing.x * 12.0f, 0.0f), new Vector2(3.0f, 1.0f), true);

        //increment y spacing
        pos.y -= spacing.y;

        //create the special key spacebar at the bottom of the keyboard
        CreateKey("Space", " ", " ", KeyCode.Space, pos + new Vector2(spacing.x * 5.5f, 0.0f), new Vector2(6.0f, 1.0f), true);
    }

    // Use this for initialization
    void Start()
    {
        //our inscene keyboard needs to start off disabled.
        m_KeyboardCanvas.SetActive(false);
        CreateKeyboard();
        enabled = false;
    }

    /// <summary>
    /// Handle when a key is pressed; insert text into input field
    /// </summary>
    /// <param name="key">the key that was pressed</param>
    void ButtonPressed(Key key)
    {
        //partially fixes the double click problem
        key.button.GetComponent<Button>().interactable = false;
        key.button.GetComponent<Button>().interactable = true;

        //we need a displayText object to display what the user has entered so far.
        if (displayText == null)
        {
            Debug.LogError("No Display Text object to display user's name on.");
            return;
        }

        //deciding which key was pressed
        switch (key.code)
        {
            //on enter, we want the gamecontroller to set the player name
            case KeyCode.Return:
                //Get the global controller instance so we can set the current player's name
                GlobalController.GetInstance().SetPlayerName(m_displayText.text);

                //now that we've entered the name we want to switch to the review panel.
                m_TransitionManager.TransitionToReviewPanel();
                break;

            //on backspace, delete a character
            case KeyCode.Backspace:
                //if we have a character to backspace, delete it.
                if (displayText.text.Length > 0)
                    displayText.text = displayText.text.Substring(0, displayText.text.Length - 1);
                break;

            //on shift, set caps on the keyboard
            case KeyCode.RightShift:
                SetCapsShift();
                break;

            //default case is just a nonspecial character or spacebar.
            default:
                //do we have less than 3 characters in the name currently?
                //if true then we can add another character.
                if (displayText.text.Length < 3)
                    displayText.text += capsShift ? key.shiftedCharacter : key.character;
                break;
        }
        /**
        if (key.code == KeyCode.Return)
        {
            /**submit event data
             * var submittedEvent = new BaseEventData(EventSystem.current);
             * ExecutedEvents.Execute(inputField, submitEvent, ExectueEvents.submitHandler);
             **
            //The user entered their name, so we want to enter in our GlobalController that information, then switch the scene.
            m_GameController.SetPlayerName(displayText.text);
        }
        else if (key.code == KeyCode.Backspace && displayText.text.Length > 0)
        {
            // remove last char
            displayText.text = displayText.text.Substring(0, displayText.text.Length - 1);
        }
        else if(key.code == KeyCode.RightShift)
        {
            SetCapsShift();
        }
        else if(displayText.text.Length < 3)
        {
            //if <3 characters, we can add another initial to their name.
            displayText.text += capsShift ? key.shiftedCharacter : key.character;
        }*/
    }

    /// <summary>
    /// change key text to show shifted character
    /// </summary>
    void SetCapsShift()
    {
        //switch our capsShift
        capsShift = !capsShift;

        //iterate through each key and switch the key (button) displayed case.
        foreach(var k in keys)
        {
            //if our key isn't a special key, we can switch its case.
            if (!k.special)
                k.button.GetComponentInChildren<Text>().text = capsShift ? k.shiftedCharacter : k.character;
        }
    }
}
