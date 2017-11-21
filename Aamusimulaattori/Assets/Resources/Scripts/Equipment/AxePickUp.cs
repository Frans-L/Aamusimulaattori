using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxePickUp : MonoBehaviour {

    public float triggerDistance = 0.5f;

    GameObject player;

    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
		

        if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance)
        {
            GameObject equipment = GameObject.FindWithTag("EquipmentAxe");
            equipment.SetActive(true);
            equipment.GetComponent<EquipmentAxe>().SetActive();

            Destroy(gameObject);
        }


    }
}
