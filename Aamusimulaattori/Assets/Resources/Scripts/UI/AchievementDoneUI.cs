using UnityEngine;
using UnityEngine.UI;

public class AchievementDoneUI : MonoBehaviour
{

    public static float fadeDuration = 2.5f; //how long it takes for the text to fade
    public static float visibleDuration = 1.5f; //how long the text stays fully visible
    public static float totalDuration = fadeDuration + visibleDuration;

    public static Color color;
    public static Color invisibleColor;

    float colorAlpha;

    static string achievementName; //the name of achievement
    static float timer = 100; //make sure that bonus text is hided at start

    static Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        colorAlpha = text.color.a; //Save current alpha value
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        color = new Color(BurnObject.uiColor.r, BurnObject.uiColor.g, BurnObject.uiColor.b, colorAlpha); //updates color values
        invisibleColor = new Color(color.r, color.g, color.b, 0f);

        if(timer > visibleDuration)
        {
            text.color = Color.Lerp(color, invisibleColor, (timer - visibleDuration) / fadeDuration); //animate color change
        }
        else
        {
            text.color = color;
        }

        text.text = achievementName;

        if (timer >= totalDuration) //clear the text
            text.text = "";

    }

    public static void SetDone(string name)
    {
        achievementName = name;
        timer = 0;
    }
}
