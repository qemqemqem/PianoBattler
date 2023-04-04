using System.Collections;
using System.Collections.Generic;
using UnityEngine;    
using System.Collections;    
    
[RequireComponent (typeof (AudioSource))]
public class MicrophoneCapture : MonoBehaviour     
{    
    private bool micConnected = false;    
    private int minFreq;    
    private int maxFreq;    
    private AudioSource goAudioSource;
    public FrequencyVisualizer frequencyVisualizer;
    
    private float lastCallTime = 0f;
     
    void Start()     
    {    
        if(Microphone.devices.Length <= 0)    
        {    
            Debug.LogWarning("Microphone not connected!");    
        }    
        else  
        {    
            micConnected = true;    
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...    
            if(minFreq == 0 && maxFreq == 0)    
            {    
                //...meaning 44100 Hz can be used as the recording sampling rate    
                maxFreq = 500;    
            }    
            goAudioSource = this.GetComponent<AudioSource>();    
            Debug.Log("Got Microphone!"); 
        }    
    }    
    
    void Update()     
    {
        float timeSinceLastCall = Time.time - lastCallTime;
        if (timeSinceLastCall >= 0.2f)
        {
            lastCallTime = Time.time;
            StartStop();
        }
    }

    public void StartMicrophone()
    {
        Microphone.End(null);
        
        goAudioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);  
        goAudioSource.loop = true;
    }

    public void EndMicrophone()
    {
        // Debug.Log("Before Play: " + Microphone.GetPosition(null));
        goAudioSource.Play(); //Playback the recorded audio    // TODO This seems to be necessary?
        StartCoroutine(ProcessAudioData());
    }

    public void StartStop()
    {
        if (!Microphone.IsRecording(null))
        {
            StartMicrophone();
        }
        else
        {
            EndMicrophone();
        }
    }

    private IEnumerator ProcessAudioData()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        float[] freqData = new float[frequencyVisualizer.numFrequencies];
        // Debug.Log("Before: " + Microphone.GetPosition(null));
        goAudioSource.GetSpectrumData (freqData, 0, FFTWindow.BlackmanHarris);
        // Debug.Log("After: " + Microphone.GetPosition(null));
        frequencyVisualizer.ShowFreqs(freqData);
    }
}