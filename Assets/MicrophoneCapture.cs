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
                maxFreq = 44100;    
            }    
            goAudioSource = this.GetComponent<AudioSource>();    
            Debug.Log("Got Microphone!"); 
        }    
    }    
    
    void Update()     
    {
        if(micConnected)    
        {    
            if(!Microphone.IsRecording(null))    
            {    
                // Debug.Log("Not Recording");
            }    
            else //Recording is in progress    
            {    
                // Debug.Log("Recording");
            }    
        }    
        else    
        {    
            // Debug.Log("No Microphone");
        }    
    
    }

    public void StartStop()
    {
        if (!Microphone.IsRecording(null))
        {
            goAudioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);  
            goAudioSource.loop = true;
        }
        else
        {
            Debug.Log("Before Play: " + Microphone.GetPosition(null));
            goAudioSource.Play(); //Playback the recorded audio    
            StartCoroutine(ProcessAudioData());
        }
    }

    private IEnumerator ProcessAudioData()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for 1 second
        float[] freqData = new float[frequencyVisualizer.numFrequencies];
        Debug.Log("Before: " + Microphone.GetPosition(null));
        goAudioSource.GetSpectrumData (freqData, 0, FFTWindow.Triangle); //, FFTWindow.BlackmanHarris);
        Debug.Log("After: " + Microphone.GetPosition(null));
        frequencyVisualizer.ShowFreqs(freqData);
    
        Microphone.End(null);
    }
}