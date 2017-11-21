using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderShouts : MonoBehaviour {

    static int playCount = 0; //make sure that at first try, hurry up starts immediately
    float audioGapDuration; //time to wait before next audio clip

    public float audioGapDurationMin = 70f; //seconds
    public float audioGapDurationMax = 120f;
    public List<AudioClip> hurryUp = new List<AudioClip>();

    int nextAudio; //index of next audio clip

    float timer = 0;

    AudioSource audioSource;


	// Use this for initialization
	void Start () {

        audioSource = GetComponent<AudioSource>();

        if (playCount == 0)
        {
            nextAudio = 0; //play first audio clip
            audioGapDuration = 0;
        }
        else
        {
            nextAudio = Random.Range(0, hurryUp.Count);
            audioGapDuration = Random.Range(audioGapDurationMin, audioGapDurationMax);
        }

    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        if(BurnObject.intensitySkewed <= 0.75 && timer >= audioGapDuration)
        {
            audioSource.PlayOneShot(hurryUp[nextAudio]);

            timer = 0; //restart timer
            audioGapDuration = Random.Range(audioGapDurationMin, audioGapDurationMax); //new random time
            nextAudio = Random.Range(0, hurryUp.Count);

        }
		
	}
}
