using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour {


    public static float duration = 1.5f; //how long the bonus text stays

    public static Color color;
    public static Color invisibleColor;

    float colorAlpha;

    static float bonus = 0;
    static float timer = 100; //make sure that bonus text is hided at start

    static Text bonusText;

    // Use this for initialization
    void Start () {
        bonusText = GetComponent<Text>();
        colorAlpha = bonusText.color.a; //Save current alpha value
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        color = new Color(BurnObject.uiColor.r, BurnObject.uiColor.g, BurnObject.uiColor.b, colorAlpha); //updates color values
        invisibleColor = new Color(color.r, color.g, color.b, 0f);

        bonusText.color = Color.Lerp(color, invisibleColor, timer / duration); //animate color change
        

        if(timer <= duration)
        {
            if (bonus > 0)
                bonusText.text = "+" + ((int)(bonus)).ToString();
            else if (bonus == 0)
                bonusText.text = "0";
            else
                bonusText.text = "-" + (Mathf.Abs((int)(bonus))).ToString();
        }
        else
        {
            bonusText.text = "";
            bonus = 0;
        }
        
    }

    public static void AddBonus(float amount)
    {
        bonus += amount;
        timer = 0;
        
    }
}
