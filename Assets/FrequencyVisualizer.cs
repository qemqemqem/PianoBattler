﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyVisualizer : MonoBehaviour
{
    public GameObject exampleValueDisplayer;
    private List<GameObject> valueDisplayers;
    public int numFrequencies = 256;
    public int maxFindingSlidingWindow = 10;

    public static int peakFreq = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        valueDisplayers = new List<GameObject>(numFrequencies);
        for (int i = 0; i < numFrequencies; i++)
        {
            GameObject vdi = GameObject.Instantiate(exampleValueDisplayer, gameObject.transform);
            vdi.name = "Number_" + i;
            valueDisplayers.Add(vdi);
        }
        exampleValueDisplayer.SetActive(false);
    }

    public float ProcessFreqFloat(float val, int i)
    {
        if (i < 5) return 0;
        return val * i;
    }

    public void FindPeak(float[] vals) {
        // Find the segment of length maxFindingSlidingWindow with the highest value
        float maxVal = 0;
        int maxIndex = 0;
        float rollingSum = 0;
        for (int i = 0; i < vals.Length - maxFindingSlidingWindow; i++)
        {
            rollingSum += ProcessFreqFloat(vals[i], i);
            if (i >= maxFindingSlidingWindow)
            {
                rollingSum -= ProcessFreqFloat(vals[i - maxFindingSlidingWindow], i - maxFindingSlidingWindow);
            }
         
            if (rollingSum > maxVal)
            {
                maxVal = rollingSum;
                maxIndex = i - maxFindingSlidingWindow / 2;
                if (maxIndex < 0) maxIndex = 0;
            }
        }
        peakFreq = maxIndex;
    }

    public void ShowFreqs(float[] vals)
    {
        FindPeak(vals);
        
        // Preprocess vals
        float valsSum = 0;
        float valsMax = 0;
        for (int i = 1; i < vals.Length; i++)
        {
            valsSum += ProcessFreqFloat(vals[i], i);
            if (vals[i] > valsMax) valsMax = ProcessFreqFloat(vals[i], i);
        }

        float valsAve = valsSum / vals.Length;

        // for (int i = 0; i < vals.Length; i++)
        //     vals[i] /= valsMax;

        float myHeight = gameObject.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < numFrequencies; i++)
        {
            float iy = (i / (float)numFrequencies) * myHeight;
            valueDisplayers[i].transform.localPosition = new Vector3(0, iy);
            Rect r = valueDisplayers[i].GetComponent<RectTransform>().rect;
            valueDisplayers[i].GetComponent<RectTransform>().sizeDelta =
                new Vector2(r.width, myHeight / numFrequencies);

            valueDisplayers[i].GetComponent<Image>().fillAmount = ProcessFreqFloat(vals[i], i) / (valsAve * 3);
        }
    }
}
