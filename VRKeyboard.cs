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

    //Our game controller object
    public GameController m_GameController;

    //the m_KeyboardCanvas on which the keyboard is displayed
    public GameObject m_KeyboardCanvas;

    //the button prototype in which we will create each individual key
    public GameObject buttonPrototype;

    //The text displaying the user's name above the keyboard
    public Text displayText;

    //the X,Y origin of the keyboard position on the m_KeyboardCanvas
    public Vector2 origin;

    //the Key spacing and sizing of each key on the keyboard m_KeyboardCanvas
    public Vector2 spacing;
    public Vector3 keyTranslation;

    public bool capsShift = false;
    public bool capsLock = false;

    private bool prevCapsLock = false;

    public void LoadKeyboard()
    {
        m_KeyboardCanvas.SetActive(true);
    }

    class Key
    {
        public string name;             // name displayed on button
        public string character;
        public string shiftedCharacter;
        public KeyCode code;
        public GameObject button;
        public bool pressed;
        public bool special;
    };

    List<Key> keys;
    Dictionary<KeyCode, Key> keyCodeToKey = new Dictionary<KeyCode, Key>(); // map from key codes to keys

    // create a single key
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

    // create a row of keys
    void CreateKeyRow(string chars, string shiftedChars, KeyCode[] codes, Vector2 pos, float offset)
    {
        pos.x += offset*spacing.x;
        for (int i = 0; i < chars.Length; i++)
        {
            CreateKey(chars[i].ToString(), chars[i].ToString(), shiftedChars[i].ToString(), codes[i], pos, Vector2.one);
            pos.x += spacing.x;
        }
    }

    // create a whole keyboard
    void CreateKeyboard()
    {
        keys = new List<Key>();

        Vector2 pos = origin;

        CreateKeyRow(
            "1234567890-=",
            "!@#$%^&*()_+",
            new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals },
            pos,
            0.0f
        );
        CreateKey("Bksp", "\b", "\b", KeyCode.Backspace, pos + new Vector2(spacing.x * 12.5f, 0.0f), new Vector2(2.0f, 1.0f), true);
        //increments our next row spacing for y to be below this row
        pos.y -= spacing.y;

        CreateKeyRow(
            "qwertyuiop[]",
            "QWERTYUIOP{}",
            new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.LeftBracket, KeyCode.RightBracket },
            pos,
            0.5f
        );
        CreateKey("Enter", "\r", "\r", KeyCode.Return, pos + new Vector2(spacing.x * 12.85f, -spacing.y * 0.5f), new Vector2(1.3f, 2.0f), true);

        pos.y -= spacing.y;

        CreateKeyRow(
            "asdfghjkl;'\\",
            "ASDFGHJKL:\"||",
            new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon, KeyCode.Quote, KeyCode.Backslash },
            pos,
            0.7f
            );
        pos.y -= spacing.y;

        CreateKeyRow(
            "zxcvbnm,./",
            "ZXCVBNM<>?",
            new KeyCode[] { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M, KeyCode.Comma, KeyCode.Period, KeyCode.Slash},
            pos,
            1.0f
            );
        CreateKey("Shift", "", "", KeyCode.RightShift, pos + new Vector2(spacing.x * 12.0f, 0.0f), new Vector2(3.0f, 1.0f), true);

        pos.y -= spacing.y;

        CreateKey("Space", " ", " ", KeyCode.Space, pos + new Vector2(spacing.x * 5.5f, 0.0f), new Vector2(6.0f, 1.0f), true);
    }

    // Use this for initialization
    void Start()
    {
        CreateKeyboard();
    }

    // insert text into input field when button is pressed
    void ButtonPressed(Key key)
    {
        //partially fixes the double click problem
        key.button.GetComponent<Button>().interactable = false;
        key.button.GetComponent<Button>().interactable = true;

        //we need a displayText object to display what the user has entered so far.
        if(displayText == null)
        {
            Debug.LogError("No Display Text object to display user's name on.");
            return;
        }

        if (key.code == KeyCode.Return)
        {
            /**submit event data
             * var submittedEvent = new BaseEventData(EventSystem.current);
             * ExecutedEvents.Execute(inputField, submitEvent, ExectueEvents.submitHandler);
             **/
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
        else if(displayText.text.Length < 3) //do we have 3 or less characters?
        {
            //if <3 characters, we can add another initial to their name.
            displayText.text += capsShift ? key.shiftedCharacter : key.character;
        }
    }

    // change key text to show shifted character
    void SetCapsShift()
    {
        capsShift = !capsShift;
        foreach(var k in keys)
        {
            if (!k.special)
            {
                k.button.GetComponentInChildren<Text>().text = capsShift ? k.shiftedCharacter : k.character;
            }
        }
    }
}
