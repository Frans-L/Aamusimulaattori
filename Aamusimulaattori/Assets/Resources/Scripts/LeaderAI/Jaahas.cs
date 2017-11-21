using System.Collections.Generic;
using UnityEngine;

public class Jaahas : MonoBehaviour {

    public GameObject Player;
    public List<AudioClip> audioClips;
    public AudioClip valokuriAudioClip;
    public float audioMaxFrequency = 10f;
    private float audioCooldown = 0f;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = transform.Find("LeaderVoicelines").GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioCooldown = Mathf.Max(0, audioCooldown - Time.deltaTime);
    }

    private bool GetIsFlashlightEnabled()
    {
        return Camera.main.transform.Find("FlashLight").gameObject.activeSelf;
    }

    void OnTriggerEnter (Collider collision)
    {
        if (collision.gameObject == Player && audioCooldown == 0)
        {
            var clip = audioClips[Random.Range(0, audioClips.Count)];
            if (GetIsFlashlightEnabled() && Random.Range(0, 1f) > 0.25)
            {
                clip = valokuriAudioClip;
            }

            audioSource.PlayOneShot(clip);
            audioCooldown = audioMaxFrequency;
        }
	}
}
