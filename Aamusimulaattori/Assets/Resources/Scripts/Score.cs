using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public static float biggerTextDuration = 1f; //how long bigger text stays
    public static float biggerTextTimer = 0;

    public static float score { get; private set; }
    public static Vector3 normalScale = new Vector3(0.66f, 1f, 1f);
    public static Vector3 addScoreScale = new Vector3(0.99f,1.5f,1.5f);
    public static float fontDecreaseSpeed = 0.5f;

    static Text scoreText;
    static RectTransform rect;

    public static bool active = true;
    static Text bonusText;

    GameObject stoveBurnObject;

    // Use this for initialization
    void Start () {
        scoreText = GetComponent<Text>();
        rect = GetComponent<RectTransform>();

        stoveBurnObject = GameObject.FindWithTag("StoveBurnObject");

        ResetScore();
    }

    public static void ResetScore()
    {
        score = 0;
    }

    private static void IncrementScore(float amount)
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        //add score overtime
        if (active)
        {
            AddScore(stoveBurnObject.GetComponent<BurnObject>().cooldownSpeed * Time.deltaTime, false);
        }

        //update UI
        biggerTextTimer += Time.deltaTime;
        scoreText.text = score.ToString("F1");
        rect.localScale = Vector3.Lerp(addScoreScale, normalScale, biggerTextTimer / biggerTextDuration);
    }

    public static void AddScore(float amount, bool displayUi = true)
    {
        var multipliedScore = amount * (1 + MissionHandler.GetCompletionPercentage());
        score += multipliedScore;
        if (displayUi)
        {
            biggerTextTimer = 0;
            BonusUI.AddBonus(multipliedScore);
        }
    }

    public static void SetActive(bool a)
    {
        active = a;
    }

}
