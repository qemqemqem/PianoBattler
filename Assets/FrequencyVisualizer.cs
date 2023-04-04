using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyVisualizer : MonoBehaviour
{
    public GameObject exampleValueDisplayer;
    private List<GameObject> valueDisplayers;
    public int numFrequencies = 256;
    
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

    public void ShowFreqs(float[] vals)
    {
        // Preprocess vals
        float valsSum = 0;
        float valsMax = 0;
        for (int i = 1; i < vals.Length; i++)
        {
            valsSum += vals[i] * (i+1);
            if (vals[i] > valsMax) valsMax = vals[i] * (i+1);
        }

        for (int i = 0; i < vals.Length; i++)
            vals[i] /= valsMax;

        float myHeight = gameObject.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < numFrequencies; i++)
        {
            float iy = (i / (float)numFrequencies) * myHeight;
            valueDisplayers[i].transform.localPosition = new Vector3(0, iy);
            Rect r = valueDisplayers[i].GetComponent<RectTransform>().rect;
            valueDisplayers[i].GetComponent<RectTransform>().sizeDelta =
                new Vector2(r.width, myHeight / numFrequencies);

            valueDisplayers[i].GetComponent<Image>().fillAmount = vals[i] * (i+1);
        }
    }
}
