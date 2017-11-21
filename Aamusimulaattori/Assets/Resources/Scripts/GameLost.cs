using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLost : MonoBehaviour {

    public float animationTime = 0.5f;
    public float lockedWatchTime = 1f;

    public List<AudioClip> gameLostAudio = new List<AudioClip>();

    public static int mornings = 347;

    static float timer = 0; //for the animation
    static bool lost = false;
    static float score, record, achievements;
    static GameObject thisObject; //because static methods cannot use gameObject

    // Use this for initialization
    void Start () {
        thisObject = gameObject;

        thisObject.GetComponent<CanvasGroup>().alpha = 0; //hide menu
        lost = false;
	}

	// Update is called once per frame
	void Update () {

        if (lost)
        {
            timer += Time.deltaTime;

            GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, timer / animationTime); //smooth splash screen

            if(timer > lockedWatchTime) //make sure that player doesn't skip this screen by accident
            {
                if(Input.anyKeyDown && !Input.GetButton("ShowMenu") && !Input.GetButton("ShowAchievements"))
                {
                    Restart(); //if something is pressed, restart game
                }
            }

        }

	}

    //shows the end game splashscreen
    public static void EndGame(bool restart = false, string finalWords = "KAMINA ON SAMMUNUT")
    {

        if (!lost)
        {
            lost = true; //game is lost;

            //save all variables immediately
            Score.SetActive(false); //stop updating score
            score = Score.score;
            record = Mathf.Max(record, score);
            achievements = (int)(MissionHandler.GetCompletionPercentage() * 100);
            if (!restart) mornings--; //restart doesn't decrease morning amount

            //update UI
            GetChild(thisObject, "Title").GetComponent<Text>().text = finalWords;
            GetChild(thisObject, "UIScore").GetComponent<Text>().text = score.ToString("F0");
            GetChild(thisObject, "UIRecord").GetComponent<Text>().text = record.ToString("F0");
            GetChild(thisObject, "UIAchievements").GetComponent<Text>().text = achievements.ToString("F0") + "%";
            GetChild(thisObject, "UIMornings").GetComponent<Text>().text = mornings.ToString("F0");

            timer = 0; //make sure that animation starts at the beginning
            if (!restart) PlayRandomAudio(); //restart doesn't play random voices

            GameObject.FindWithTag("BackgroundMusic").GetComponent<AudioSource>().enabled = false;
        }

    }

    //plays random audio clip
    public static void PlayRandomAudio()
    {
        List<AudioClip> audio = thisObject.GetComponent<GameLost>().gameLostAudio;
        thisObject.GetComponent<AudioSource>().PlayOneShot(
                audio[Random.Range(0,audio.Count)]);
    }

    //restarts game immediately
    public static void Restart()
    {

        MissionHandler.SetCompleted("MissionMosaVirhe"); //mosa mission done

        Score.ResetScore(); //restart score
        Scene scene = SceneManager.GetActiveScene(); //reload scene
        Score.SetActive(true); //update score again
        SceneManager.LoadScene(scene.name);
    }

    //return child object by name
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
