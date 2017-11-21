using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Explosion : MonoBehaviour {


	void Start()
	{
		var audioSource = GetComponent<AudioSource>();
		audioSource.Play();
		var particleSystem = GetComponent<ParticleSystem>();
		var duration = particleSystem.main.startLifetime.constant + particleSystem.main.duration;

        var maxDuration = Mathf.Max(duration, audioSource.clip.length);
		Destroy(gameObject, maxDuration);
	}
}
