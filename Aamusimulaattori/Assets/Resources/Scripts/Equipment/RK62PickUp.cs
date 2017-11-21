using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//copy paste koodattu AxePickUp:ista kiireen vuoksi
public class RK62PickUp : MonoBehaviour
{

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
            GameObject equipment = GameObject.FindWithTag("EquipmentRK62");
            equipment.SetActive(true);
            equipment.GetComponent<EquipmentRK62>().SetActive();

            Destroy(gameObject);
        }


    }
}
