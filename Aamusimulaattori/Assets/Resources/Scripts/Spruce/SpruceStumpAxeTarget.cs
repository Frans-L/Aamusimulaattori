using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpruceStumpAxeTarget : MonoBehaviour, IAxeTargetHit {

    public float maxHealth = 3f;
    public float health = 3f;
    public float logThrowSpeed = 0.3f;

    public GameObject log;

    public AudioClip chopSound;
    AudioSource audioSource; //component that plays sounds

    GameObject masterLog; //the object that is copied

    public float shrinkAnimationDuration = 0.4f;
    float animationTime = 0.0f;
    Boolean shrinkAnimation = false;
    float startHeight;

    // Use this for initialization
    void Start () {

        masterLog = log; //Just for TESTING, not final

        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if (shrinkAnimation)
        {
            animationTime += Time.deltaTime;

            transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                Mathf.Lerp(startHeight, startHeight*health/maxHealth, animationTime / shrinkAnimationDuration));

            if (animationTime >= shrinkAnimationDuration)
            {
                shrinkAnimation = false;
                if(health <= 0)
                {
                    Destroy(gameObject);
                }
            }
                
        }
	}

    //when object is hitted
    public int HitAction(Collider col)
    {

        if (gameObject.GetComponent<SpruceStumpAxeTarget>().isActiveAndEnabled)
        {
            health--;

            audioSource.PlayOneShot(chopSound, 1f); //play sound

            if (health >= 0)
            {
                GameObject log = Instantiate(masterLog); //copy new log

                Vector3 direction = new Vector3(
                    UnityEngine.Random.value * 2 - 1.0f,
                    UnityEngine.Random.value,
                    UnityEngine.Random.value * 2 - 1.0f);

                log.transform.position = gameObject.transform.position + direction * 0.2f + Vector3.up * 0.8f;
                log.SetActive(true);
                log.GetComponent<Rigidbody>().velocity = direction * logThrowSpeed; //shoot it somewhere

                shrinkAnimation = true; //start new animation
                animationTime = 0; 
                startHeight = transform.localScale.z;
            }

            
        }
            
        return 1;
    }
}
