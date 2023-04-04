using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMusicSounds : MonoBehaviour
{
    AudioSource audioSource;
    
    // TODO DELETE
    int timeIndex = 0;
    // private float frequency1 = 440;
    // private float frequency2 = 600;
    float sampleRate = 44100;
    float waveLengthInSeconds = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
    }

    // Update is called once per frame
    void Update()
    {
        if(KeyListenerToPos.CurrentNote != null && !audioSource.isPlaying)
        {
            timeIndex = 0;  //resets timer before playing sound
            audioSource.Play();
        }
        else if(KeyListenerToPos.CurrentNote == null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (KeyListenerToPos.CurrentNote == null) return;
        float frequency1 = (float)KeyListenerToPos.CurrentNote.calculateFrequency();
        for(int i = 0; i < data.Length; i+= channels)
        {          
            data[i] = CreateSine(timeIndex, frequency1, sampleRate);
           
            // if(channels == 2)
            //     data[i+1] = CreateSine(timeIndex, frequency2, sampleRate);
           
            timeIndex++;
           
            //if timeIndex gets too big, reset it to 0
            if(timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }
   
    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }
}
