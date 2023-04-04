using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SingleNoteMover : MonoBehaviour
{
    private float targetY = 0;
    static readonly float speed = 500f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().enabled = true;//(KeyListenerToPos.CurrentNote != null);

        // if (KeyListenerToPos.CurrentNote == null) return;
        
        // targetY = KeyListenerToPos.CurrentNote.getPosition();
        targetY = FrequencyVisualizer.peakFreq * 500f / 256f - 250;

        RectTransform rtrans = gameObject.GetComponent<RectTransform>();
        float curY = rtrans.localPosition.y;
        float diff = targetY - curY;
        float curSpeed = speed * Time.deltaTime;
        float nextY = curY + Math.Min(curSpeed, Math.Max(diff, -curSpeed));

        rtrans.localPosition = new Vector3(rtrans.localPosition.x, nextY, rtrans.localPosition.z);
    }
}
