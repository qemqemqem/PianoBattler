﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SingleNoteMover : MonoBehaviour
{
    private float targetY = 0;
    static readonly float speed = 200f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO Analyze music or keys or something
        targetY = KeyListenerToPos.CurrentNote.getPosition();

        RectTransform rtrans = gameObject.GetComponent<RectTransform>();
        float curY = rtrans.localPosition.y;
        float diff = targetY - curY;
        float curSpeed = speed * Time.deltaTime;
        float nextY = curY + Math.Min(curSpeed, Math.Max(diff, -curSpeed));

        rtrans.localPosition = new Vector3(rtrans.localPosition.x, nextY, rtrans.localPosition.z);

        GetComponent<Image>().enabled = !KeyListenerToPos.NothingPlayed;
    }
}
