using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//updates static missionhandler
public class MissionHandlerUpdater : MonoBehaviour {

    void Start()
    {
        MissionHandler.Start();
    }

    void Update()
    {
        MissionHandler.Update();
    }
}

public static class MissionHandler
{
    static float showDuration = 30;
    static float showDurationRandom = 20;
    static float showCompletedDuration = 5;

    static string title = "Arvomerkit ";

    //hotspot means left-up corner, with missions shown all the time
    static bool hotSpotStarted = false; //if already selected two missions to be shown
    const int hotSpotLineAmount = 2; //how many lines

    static Dictionary<string, MissionData> missions = new Dictionary<string, MissionData>();
    static List<GameObject> uiAchievements = new List<GameObject>(); //achievemtent's UI row elements

    static GameObject uiHotSpotTitle;
    static GameObject uiScoreMultiplier;
    static GameObject[] uiHotSpotLine = new GameObject[hotSpotLineAmount]; //two lines
    static MissionData[] hotSpotMission = new MissionData[hotSpotLineAmount]; //contain reference to missions' MissionData

    // Use this for initialization
    public static void Start()
    {
        uiAchievements.Clear(); //since static, lists can hold item already

        uiHotSpotLine[0] = GameObject.FindWithTag("UIHotSpotMission1");
        uiHotSpotLine[1] = GameObject.FindWithTag("UIHotSpotMission2");
        uiHotSpotTitle = GameObject.FindWithTag("UIHotSpotTitle");
        uiScoreMultiplier = GameObject.FindWithTag("UIScoreMultiplier");

        //add all UI elements under the tag UIMissionStack to UI elements
        uiAchievements.AddRange(GetChildObjects(GameObject.FindWithTag("UIMissionStack1")));
        uiAchievements.AddRange(GetChildObjects(GameObject.FindWithTag("UIMissionStack2")));
        uiAchievements.AddRange(GetChildObjects(GameObject.FindWithTag("UIMissionStack3")));
    }

    // Update is called once per frame
    public static void Update()
    {
        for (int i = hotSpotLineAmount-1; i >= 0; i--)
        {
            hotSpotMission[i].showTime = Mathf.Max(hotSpotMission[i].showTime - Time.deltaTime, 0);
            GetChild(uiHotSpotLine[i], "CheckMark").SetActive(hotSpotMission[i].completed);

            if (hotSpotMission[i].showTime <= 0)
            {
                hotSpotMission[i] = FindNewHotSpotMission();
                hotSpotMission[i].showTime = showDuration + UnityEngine.Random.value * showDurationRandom;
                UpdateHotSpotUI(i);
            }
        }

        UpdateUI();
    }

    public static bool IsCompleted(string tag)
    {
        return missions.ContainsKey(tag) && missions[tag].completed;
    }

    //upadtes hotspot timer and UI
    public static void UpdateHotSpotUI(int i)
    {
        GetChild(uiHotSpotLine[i], "Text").GetComponent<Text>().text = hotSpotMission[i].name;
    }

    //updates new hotspot
    public static void StartHotSpot()
    {
        if (!hotSpotStarted) //if not started before
        {
            hotSpotMission[0] = FindNewHotSpotMission(); //add first mission to hotspot
            hotSpotMission[1] = FindNewHotSpotMission();
            hotSpotMission[0].showTime = showDuration + UnityEngine.Random.value * showDurationRandom; //add show time
            hotSpotMission[1].showTime = showDuration + UnityEngine.Random.value * showDurationRandom;

            hotSpotStarted = true; //no need to start again
        }

        UpdateHotSpotUI(0); //update UI
        UpdateHotSpotUI(1);
        UpdateTitle();
    }

    //updates UI elements
    public static void UpdateUI()
    {
        foreach (var mission in missions.Values)
        {
            mission.uiElement.SetActive(mission.completed);
        }
    }

    //Find proper mission to be shown at left corner
    public static MissionData FindNewHotSpotMission()
    {
        var unhotMissions = new List<MissionData>();
        var unhotIncompleteMissions = new List<MissionData>();
        foreach(var mission in missions.Values)
        {
            var isHot = false;
            foreach (var hotMission in hotSpotMission)
            {
                if (hotMission == null || mission.tag != hotMission.tag) continue;
                isHot = true;
            }
            if (isHot) continue;
            unhotMissions.Add(mission);
            if (mission.completed) continue;
            unhotIncompleteMissions.Add(mission);
        }

        if (unhotIncompleteMissions.Count > 0)
        {
            return unhotIncompleteMissions[UnityEngine.Random.Range(0, unhotIncompleteMissions.Count)];
        }
        if (unhotMissions.Count > 0)
        {
            return unhotMissions[(UnityEngine.Random.Range(0, unhotMissions.Count))];
        }
        return missions.ElementAt(UnityEngine.Random.Range(0, missions.Count)).Value;
    }

    //add new mission
    public static void AddMission(string name, string tag, int index)
    {
        if (!missions.ContainsKey(tag))
        {
            missions[tag] = new MissionData(
                name,
                tag,
                GetChild(uiAchievements[index], "Checkmark"),
                index,
                false,
                0
            );
            GetChild(uiAchievements[index], "Text").GetComponent<Text>().text = name; //update the text
        }
        else //otherwise update UI
        {
            missions[tag].uiElement = GetChild(uiAchievements[index], "Checkmark");
            GetChild(uiAchievements[index], "Text").GetComponent<Text>().text = name;
        }
    }

    //sets mission completed
    public static void SetCompleted(string tag)
    {
        if (!missions.ContainsKey(tag)) return; 
        var mission = missions[tag];
        var row = 0;
        if (mission.showTime == 0) //if not showing at hotspot
        {
            hotSpotMission[row].showTime = 0; //hide old one
            hotSpotMission[row] = mission; //set just completed to hotspot
        }

        //if not completed before
        if (!mission.completed)
        {
            AchievementDoneUI.SetDone(mission.name);
        }

        mission.completed = true; //mark completed
        mission.showTime = showCompletedDuration; //show the completed mission a couple of seconds

        row = GetHotSpotRow(mission); //update the correct UI's row
        GetChild(uiHotSpotLine[row], "Text").GetComponent<IncreaseFontScale>().Increase();  //highlight completed mission
        UpdateHotSpotUI(row); //update UI's text

        UpdateTitle(); //update title procents
    }

    static void UpdateTitle()
    {
        uiHotSpotTitle.GetComponent<Text>().text = title + ((int)(GetCompletionPercentage()*100)) + "%";
        uiScoreMultiplier.GetComponent<Text>().text = string.Format("Kerroin: x{0:0.0}", 1 + GetCompletionPercentage());
    }


    //return the amount of completed mission
    static int GetCompletedAmount()
    {
        int amount = 0;
        foreach (var mission in missions.Values)
        {
            if (!mission.completed) continue;
            amount++;
        }
        return amount;
    }

    static int GetMissionAmount()
    {
        return missions.Count;
    }

    public static float GetCompletionPercentage()
    {
        return (float) GetCompletedAmount() / GetMissionAmount();
    }

    //finds in which row the mission is shown
    static int GetHotSpotRow(MissionData m)
    {
        for (int i = 0; i < hotSpotLineAmount; i++)
            if (hotSpotMission[i].tag == m.tag)
                return i;
        return 0;
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

    static List<GameObject> GetChildObjects(GameObject o)
    {

        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < o.transform.childCount; i++)
        {
            childs.Add(o.transform.GetChild(i).gameObject);
        }

        return childs;
    }
}

public class MissionData
{
    public string name; //description of the mission
    public string tag; //missions tah
    public GameObject uiElement; // UI element
    public float index; //how hard it is
    public bool completed; //if completed
    public float showTime; //how long have been showed

    public MissionData(string name, string tag, GameObject uiElement, float index, bool completed, float showTime)
    {
        this.name = name;
        this.tag = tag;
        this.uiElement = uiElement;
        this.index = index;
        this.completed = completed;
        this.showTime = showTime;
    }

}
