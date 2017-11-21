using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// make X amounts animation steps by using Lists rallyPointTags, sizeMultiplier and duration
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BurnObject : MonoBehaviour {

    List<BurnAnimationStep> burnAnimation = new List<BurnAnimationStep>();

    public static Color uiColor; //ui color
    public static float intensity = 1, intensitySkewed = 1;

    //next variables are for the Unity UI
    public List<string> rallyPointTags = new List<string>(); //animation rallypoint (coordinate)
    public List<float> sizeMultiplier = new List<float>(); //how much the size changes changes per animation step
    public List<float> duration = new List<float>(); //each step duration
    public List<AudioClip> clatterSounds = new List<AudioClip>(); //sound effects for inserting items

    public float maxTemperature = 60f;
    public float cooldownSpeed = 0.5f;
    public float currentTemperature = 30F;

    public float normalTemperature = 15f; //when the ui color is normal
    public Color hotUIColor, normalUIColor, coolUIColor; //thermometer colors

    GameObject thermometer, cooldownText;

    public float hotFuelDecreaseMultiplier = 0.75f; //how much the fuel power decreases relative to currentTemperature
    public float coolDownSmoothingMinMultiplier = 0.75f; //the smoothing speed when the temperatures are low
    public float coolDownSpeedMultiplier = 0.0175f; //multiplier to objects' cooldownAmount -> how much does cooldownspeed increase every burn

    public float audioMaximumFrequency = 0.1f; //maximum audioclip play frequency
    private AudioSource audioSource;
    private float audioCooldown = 0f;

    private bool isExplosionNewsShown = false;
    private bool isMurderNewsShown = false;

    AudioSource leaderAudioSource; //who shouts the poop shouts
    public AudioClip poopShout;

    // Use this for initialization
    void Start () {

        thermometer = GameObject.FindWithTag("ThermometerFill");
        cooldownText = GameObject.FindWithTag("CooldownText");
        leaderAudioSource = GameObject.FindWithTag("Leader").GetComponent<AudioSource>();

        audioSource = GetComponent<AudioSource>();

        for (int i=0; i<rallyPointTags.Count; i++)
        {
            burnAnimation.Add(new BurnAnimationStep(
                GameObject.FindWithTag(rallyPointTags[i]).transform.position, //find the rallypoint
                sizeMultiplier[i],
                duration[i]));
        }
    }

    void Update()
    {

        intensity = currentTemperature / maxTemperature; //updates intensity which is used for instance for stove's lightning
        intensitySkewed = currentTemperature / normalTemperature;

        float cooldownSmoothing = Mathf.Max(Mathf.Min(intensitySkewed * 1.5f, 1f), coolDownSmoothingMinMultiplier);
        currentTemperature -= cooldownSpeed * Time.deltaTime * cooldownSmoothing; //decrease cooldownSpeed per second

        ThermomterUI(); //changes thermometer color

        if(currentTemperature < -1f)
        {
            GameLost.EndGame();
        }

        audioCooldown = Mathf.Max(0f, audioCooldown - Time.deltaTime);
    }

    // when an object object touches the stove action collision box
    private void OnTriggerEnter(Collider collider)
    {
        var burnable = collider.gameObject.GetComponent<Burnable>();
        var explosive = collider.gameObject.GetComponent<Explosive>();

        if (burnable == null || !burnable.isActiveAndEnabled) return;

        //if leader
        if(collider.gameObject.name == "Leader")
        {
            MissionHandler.SetCompleted("MissionMurha");
            if (!isMurderNewsShown)
            {
                Aamusimulaattori.Newspaper.Show("Varusmies pahoinpiteli esimiehensä!", 80);
                isMurderNewsShown = true;
            }
        }

        //if poop
        if (collider.gameObject.name == "Shit")
        {
            leaderAudioSource.PlayOneShot(poopShout);
        }


        //if any explosive
        if (explosive)
        {
            explosive.Trigger(2);
            MissionHandler.SetCompleted("MissionKaBoom");
            if (!isExplosionNewsShown)
            {
                Aamusimulaattori.Newspaper.Show("Varusmies räjäytti teltan!");
                isExplosionNewsShown = true;
            }
        }

        //handles the audo
        if(audioCooldown == 0f)
        {
            Invoke("PlayClatter", 1);
            audioCooldown += audioMaximumFrequency;
        }

        //start burn animation
        burnable.Burn(burnAnimation, gameObject);
    }

    void PlayClatter()
    {
        if(clatterSounds.Count < 1) return;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(clatterSounds[Random.Range(0, clatterSounds.Count)]);
    }

    public void IncreaseCooldownSpeed(float amount)
    {
        cooldownSpeed += amount*coolDownSpeedMultiplier;
    }

    //lerps the colors
    //a = start, b = target
    Color ColorLerp(Color a, Color b, float time)
    {
        float t = - Mathf.Cos(Mathf.PI * time) / 2 + 0.5f; //interpolate smoothly
        return new Color(
            Mathf.Lerp(a.r, b.r, t),
            Mathf.Lerp(a.g, b.g, t),
            Mathf.Lerp(a.b, b.b, t) );
    }


    //changes the thermometer color and fill size
    void ThermomterUI()
    {
        float progress;

        if (currentTemperature >= normalTemperature) //if warmer than normaTemperature -> use red colors
        {
            progress = Mathf.Max(Mathf.Min((currentTemperature - normalTemperature) / (maxTemperature - normalTemperature), 1f), 0);
            uiColor = ColorLerp(normalUIColor, hotUIColor, progress);
        }
        else //otherwise inpterpolate colors to blueish
        {
            progress = Mathf.Max(Mathf.Min((currentTemperature) / (normalTemperature), 1f), 0);
            uiColor = ColorLerp(coolUIColor, normalUIColor, progress);
        }

        progress = Mathf.Max(Mathf.Min((currentTemperature) / (maxTemperature), 1f), 0);
        thermometer.GetComponent<RectTransform>().localScale = new Vector3(1, progress, 1); //change fill size

        cooldownText.GetComponent<Text>().text = cooldownSpeed.ToString("F2");

    }




    //add more fuel, increases the temperature
    //the lower the temperature is, the more temperature will increase
    public void AddFuel(float amount)
    {
        float multiplier = (1 - (currentTemperature - normalTemperature) / maxTemperature * hotFuelDecreaseMultiplier); //decrease fuel amount when already very hot
        currentTemperature += amount *multiplier;
        currentTemperature = Mathf.Min(currentTemperature, maxTemperature); //limi to max temperature

        //kaminanhenki has been summoned
        if (currentTemperature == maxTemperature)
            MissionHandler.SetCompleted("MissionKaminanHenki");

        //if something that doesn't burn well, has been put on the stove
        if (amount < 1f && amount > 0)
            MissionHandler.SetCompleted("MissionPalaa");
    }

}

//one step of a burning animation
public class BurnAnimationStep
{
    public Vector3 rallyPoint;
    public float sizeMultiplier;
    public float duration;

    public BurnAnimationStep(Vector3 rallyPoint, float sizeMultiplier, float duration)
    {
        this.rallyPoint = rallyPoint;
        this.sizeMultiplier = sizeMultiplier;
        this.duration = duration;
    }
}
