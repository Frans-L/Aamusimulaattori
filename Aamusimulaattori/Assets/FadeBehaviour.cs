using UnityEngine;
using UnityEngine.UI;

public class FadeBehaviour : MonoBehaviour
{
    public enum FadeMode
    {
        FadeIn,
        FadeOut,
        None
    }

    public float fadeRate = 0.2f;
    public float fadeDelay = 3f;
    public FadeMode fadeMode = FadeMode.FadeIn;

    private RawImage overlay;

	void Start()
    {
        overlay = GetComponent<RawImage>();
	}
	
	void Update()
    {
        if (fadeMode == FadeMode.None) return;
        if (fadeDelay > 0f)
        {
            fadeDelay = Mathf.Max(0, fadeDelay - Time.deltaTime);
            return;
        }
        if(fadeMode == FadeMode.FadeIn)
        {
            overlay.color = new Color(
                overlay.color.r,
                overlay.color.g,
                overlay.color.b,
                Mathf.Max(0, overlay.color.a - fadeRate * Time.deltaTime)
            );
            if(overlay.color.a == 0)
            {
                fadeMode = FadeMode.None;
            }
        }
        else if (fadeMode == FadeMode.FadeOut)
        {
            overlay.color = new Color(
                overlay.color.r,
                overlay.color.g,
                overlay.color.b,
                Mathf.Min(1, overlay.color.a + fadeRate * Time.deltaTime)
            );
            if (overlay.color.a == 1)
            {
                fadeMode = FadeMode.None;
            }
        }
    }
}
