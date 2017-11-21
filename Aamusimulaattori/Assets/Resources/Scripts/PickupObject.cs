using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour {

    GameObject mainCamera;
    GameObject uiCrosshair;
    List<GameObject> carriedObjects = new List<GameObject>();

    public float holdDistance = 3.5f; //hold distance
    public float pickUpDistance = 8f; //hold distance

    public float carriedObjectGap = 0.2f; //when multiple objects are carried the distance between objects
    float pickUpAreaRadius = 0.1f;

    public float rotationForceMultipler = 0.3f;
    public float carriedObjectspeed = 6; //the higher, the faster the object follows the camera

    public float maxCarryWeight = 10;
    float currentCarryWeight = 0;

    bool pickingUp = false;

    LayerMask rayMask;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindWithTag("MainCamera");
        uiCrosshair = GameObject.FindWithTag("Crosshair");

        gameObject.layer = LayerMask.NameToLayer("RayCastIgnore"); //make sure that ray doesn't hit player
        rayMask = ~(1 << LayerMask.NameToLayer("RayCastIgnore")); //create layermask that ignores raycastigonre
    }
	
	// Update is called once per frame
	void Update () {

        Pickup();

        if (carriedObjects.Count > 0)
        {


            for (int i = 0; i<carriedObjects.Count; i++)
            {

                //in case of the object is somehow deleted
                if(carriedObjects[i] != null && carriedObjects[i].GetComponent<Rigidbody>() != null)
                {
                    //calculate position of the carried object
                    float offsetX = 0, offsetY = 0;
                    if (i > 0)
                    {
                        float dir = Mathf.Min(Mathf.Max((i % 4) - 1, 0), 1) * 2 - 1;
                        offsetX = ((i + 1) % 2) * carriedObjectGap * dir * Mathf.Ceil(i / 4.0f);
                        offsetY = (i % 2) * carriedObjectGap * dir * Mathf.Ceil(i / 4.0f);
                    }

                    Carry(carriedObjects[i], offsetX, offsetY);
                }       
            }


            if(!pickingUp)
                CheckDrop();

        }
        
    }

    // keep the object in front of the player
    void Carry(GameObject o, float offsetX, float offsetY)
    {
        Vector3 currentPos = o.transform.position;
        Vector3 nextPos = mainCamera.transform.position
            + mainCamera.transform.forward * holdDistance * Mathf.Max(Mathf.Cos(mainCamera.transform.localEulerAngles.x * Mathf.PI / 180f), 0.5f)
            + mainCamera.transform.right * offsetX 
            + mainCamera.transform.up * offsetY; //each carried object has their own position
        o.GetComponent<Rigidbody>().velocity = (nextPos - currentPos) * carriedObjectspeed;

        o.GetComponent<Rigidbody>().AddTorque(-o.GetComponent<Rigidbody>().angularVelocity*0.2f); //stop the rotation
    }

    //check if the object is picked
    void Pickup()
    {
        if (Input.GetButtonDown("PickUp") && carriedObjects.Count == 0)
            pickingUp = true;
        if (Input.GetButtonUp("PickUp"))
            pickingUp = false;


        //always calculates if the object can be picked -> so the crosshair size can be updated
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.SphereCast(ray, pickUpAreaRadius, out hit, pickUpDistance, rayMask))
        {
            PickupAble p = hit.collider.GetComponent<PickupAble>();

            //if can be carried
            if (p != null && p.IsCarried == false && p.ForceDrop == false && p.carryWeight + currentCarryWeight <= maxCarryWeight && p.isActiveAndEnabled)
            {

                uiCrosshair.GetComponent<CrosshairSize>().ToActionSize();

                if (pickingUp) //if player wants to pick it
                {
                    p.IsCarried = true;
                    p.gameObject.layer = LayerMask.NameToLayer("RayCastIgnore"); //make sure raycast goes through the object
                    p.gameObject.GetComponent<Rigidbody>().useGravity = false; //gravity would couse trouble
                    currentCarryWeight += p.carryWeight;
                    carriedObjects.Add(p.gameObject);

                }
            }
        }

    }

    //check if the player wants to drop the object
    void CheckDrop()
    {
        if (Input.GetButtonDown("PickUp") || IsForceDrop())
        {
            foreach(GameObject o in carriedObjects)
            {
                if(o != null)
                    Drop(o);
            }
            carriedObjects.Clear(); //make sure that nothing is carried
            currentCarryWeight = 0; //reset weight
        } 
    }

    //drop action
    void Drop(GameObject o)
    {
        o.GetComponent<PickupAble>().IsCarried = false;
        o.layer = LayerMask.NameToLayer("Normal"); //back to normal raycast target
        o.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    //check if the object has to be dropped
    bool IsForceDrop()
    {
        foreach(GameObject o in carriedObjects)
        {
            if (o != null && o.GetComponent<PickupAble>().ForceDrop == true) //jos joku esine pakko pudottaa
                return true;
        }

        return false;
    }

    //return the object that is being carried by the player
    public List<GameObject> CarriedObject()
    {
        return carriedObjects;
    }
}
