using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public int octave;
    public int note; // 0 is C
    public int sharpness; // -1 is flat, 1 is sharp, 0 is neutral

    public Note(int octave, int note, int sharpness=0)
    {
        this.octave = octave;
        this.note = note;
        this.sharpness = sharpness;
    }

    public float getPosition()
    {
        return note * 20f - 100f;
    }

    public int calculateNoteNum()
    {
        int o = octave * 12;
        int n = note;
        if (note >= 7) n++; // B
        if (note >= 6) n++; // A
        if (note >= 5) n++; // G
        if (note >= 3) n++; // E
        if (note >= 2) n++; // D
        // Debug.Log("Value is " + (o + n + sharpness));
        return o + n + sharpness;
    }

    public double calculateFrequency()
    {
        return Math.Pow(2.0, calculateNoteNum() / 12.0) * 15.4338531537; // Multiplier based on A4 at 440
    }
}

public class KeyListenerToPos : MonoBehaviour
{
    private Dictionary<KeyCode, Note> keyToNote = new Dictionary<KeyCode, Note>();

    public static Note CurrentNote = new Note(1, 1);

    // Start is called before the first frame update
    void Start()
    {
        keyToNote[KeyCode.A] = new Note(4, 1);
        keyToNote[KeyCode.S] = new Note(4, 2);
        keyToNote[KeyCode.D] = new Note(4, 3);
        keyToNote[KeyCode.F] = new Note(4, 4);
        keyToNote[KeyCode.G] = new Note(4, 5);
        keyToNote[KeyCode.H] = new Note(4, 6);
        keyToNote[KeyCode.J] = new Note(4, 7);
        keyToNote[KeyCode.K] = new Note(5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentNote = null;
        foreach (KeyCode key in keyToNote.Keys)
        {
            if (Input.GetKey(key))
            {
                CurrentNote = keyToNote[key];
                // Debug.Log("Playing frequency: " + keyToNote[key].calculateFrequency());
            }
        }
    }
}
