using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {

    public List<float> duration = new List<float>();

    public List<AudioClip> taskAudioClip = new List<AudioClip>();
    int audioCounter = 0;

    int step = 0;
    float timer = 0;

    public float fadeInDuration = 0.5f;

    CanvasGroup black;

	// Use this for initialization
	void Start () {

        Cursor.lockState = CursorLockMode.Locked; //lock mouse

        black = GetChild(gameObject, "Black").GetComponent<CanvasGroup>();

        black.alpha = 1;
        GetChild(gameObject, "Task").GetComponent<CanvasGroup>().alpha = 1;
        GetChild(gameObject, "Controls").GetComponent<CanvasGroup>().alpha = 1;
        GetChild(gameObject, "Warning").GetComponent<CanvasGroup>().alpha = 1;
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        black.alpha = Mathf.Lerp(1, 0, timer / fadeInDuration); //fade in next splash screen

 
        //play audio in task splash screen
        if (step == 1 && !GetComponent<AudioSource>().isPlaying && audioCounter < taskAudioClip.Count)
        {
            GetComponent<AudioSource>().PlayOneShot(taskAudioClip[audioCounter]);
            audioCounter++;
        }


        //if next splash screen
        if( ( timer >= duration[step] && duration[step] > 0) || (Input.anyKeyDown && timer > 0.5f))
        {
            step++;
            timer = 0;

            black.alpha = 1;

            if (step == 1)
            {
                GetChild(gameObject, "Warning").GetComponent<CanvasGroup>().alpha = 0;
                GetChild(gameObject, "Task").GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (step == 2)
            {
                GetChild(gameObject, "Warning").GetComponent<CanvasGroup>().alpha = 0;
                GetChild(gameObject, "Task").GetComponent<CanvasGroup>().alpha = 0;
                GetChild(gameObject, "Controls").GetComponent<CanvasGroup>().alpha = 1;
            }else if (step == 3)
            {
                SceneManager.LoadScene("SparkTurn"); //load the game
                step = 2; //make sure that this scene doesn't lag when loading next one
            }
        }

		
	}


    static GameObject GetChild(GameObject o, string name)
    {
        for (int i = 0; i < o.transform.childCount; i++)
        {
            if (o.transform.GetChild(i).name == name)
                return o.transform.GetChild(i).gameObject;
        }

        return null;
    }

}
