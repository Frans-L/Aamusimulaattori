using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicIntensity : MonoBehaviour {

    public float smoothDown = 1f;
    public float smoothUp = 0.07f;
    public float hotPitch;
    public float coldPitch;

    public float maxVolume = 0.25f;
    public float volumeFadeInRate = 0.1f;
    public float startVolume = 0f;
    public float startDelay = 4f;

    private float smoothIntensity;
    private bool started;
    private AudioSource audioSource;

    void Start()
    {
        smoothIntensity = 1f;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = startVolume;
        started = false;
        Invoke("BeginPlaying", startDelay);
    }

    void BeginPlaying()
    {
        started = true;
        audioSource.Play();
    }

    void Update()
    {
        if (!started) return;
        smoothIntensity += Mathf.Min(Mathf.Max((BurnObject.intensitySkewed - smoothIntensity), -smoothDown), smoothUp) * Time.deltaTime; //smooth the transition
        audioSource.pitch = Mathf.Lerp(coldPitch, hotPitch, Mathf.Min(smoothIntensity, 1));
        audioSource.volume = Mathf.Min(maxVolume, audioSource.volume + volumeFadeInRate * Time.deltaTime);
    }
}
