using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseFontScale : MonoBehaviour {


    public int index = 0; //index of which static timer slot, timer will be saved
    public float increaseDuration = 1.5f;
    public float increaseMultiplier = 1.25f;
    public float increasePosX = 30f;

    public static float[] timer = new float[2];  //to keep timer over restart scene
    RectTransform rect;

    Vector3 normalSize;
    Vector3 increasedSize;
    Vector2 normalPos;
    Vector2 increasedPos;
   

	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();
        normalSize = rect.localScale;
        increasedSize = normalSize * increaseMultiplier;

        normalPos = rect.anchoredPosition;
        increasedPos = new Vector2(normalPos.x + increasePosX, normalPos.y);

        if (timer[index] == 0) //make sure that it starts normally
            timer[index] = increaseDuration;
    }
	
	// Update is called once per frame
	void Update () {
        timer[index] += Time.deltaTime;
        rect.localScale = Vector3.Lerp(increasedSize, normalSize, timer[index] / increaseDuration);
        rect.anchoredPosition = Vector2.Lerp(increasedPos, normalPos, timer[index] / increaseDuration);
    }

    public void Increase()
    {

        timer[index] = 0.001f; //start animation
    }
}
