using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class LeaderMove : MonoBehaviour
{
    public NavMeshAgent leaderAgent;
    public List<GameObject> goal = new List<GameObject>();

    public int currentRP = 1;

    void Start()
    {
        leaderAgent.destination = goal[currentRP].transform.position;

        GetChild(gameObject, "Leader").GetComponent<Animator>().speed = 0.9f;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, leaderAgent.destination) <= 1.5f)
            NextGoal();
            leaderAgent.destination = goal[currentRP].transform.position;

    }


    //Return the next goal to go to.
    void NextGoal()
    {
        currentRP++;
        if(currentRP >= goal.Count)
            currentRP = 0;
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
