using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public float noteVal;

    public Note(float noteVal)
    {
        this.noteVal = noteVal;
    }

    public float getPosition()
    {
        return noteVal * 20f - 100f;
    }
}

public class KeyListenerToPos : MonoBehaviour
{
    private Dictionary<KeyCode, Note> keyToNote = new Dictionary<KeyCode, Note>();

    public static Note CurrentNote = new Note(1);
    public static bool NothingPlayed = true;

    // Start is called before the first frame update
    void Start()
    {
        keyToNote[KeyCode.A] = new Note(1);
        keyToNote[KeyCode.S] = new Note(2);
        keyToNote[KeyCode.D] = new Note(3);
        keyToNote[KeyCode.F] = new Note(4);
        keyToNote[KeyCode.G] = new Note(5);
        keyToNote[KeyCode.H] = new Note(6);
        keyToNote[KeyCode.J] = new Note(7);
        keyToNote[KeyCode.K] = new Note(8);
    }

    // Update is called once per frame
    void Update()
    {
        NothingPlayed = true;
        foreach (KeyCode key in keyToNote.Keys)
        {
            if (Input.GetKey(key))
            {
                CurrentNote = keyToNote[key];
                NothingPlayed = false;
            }
        }
    }
}
