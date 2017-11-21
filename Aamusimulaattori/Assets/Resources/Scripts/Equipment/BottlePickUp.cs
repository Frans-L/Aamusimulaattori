using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlePickUp : MonoBehaviour {

    public float triggerDistance = 1f;

    GameObject player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {


        if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance)
        {
            GameObject equipment = GameObject.FindWithTag("EquipmentBottle");
            equipment.SetActive(true);
            equipment.GetComponent<EquipmentBottle>().SetActive();

            Destroy(gameObject);
        }


    }

}
