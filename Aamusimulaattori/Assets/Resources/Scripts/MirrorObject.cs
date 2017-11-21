using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Mirros the object
/// Allso teleports it to others side of the map, when needed
/// </summary>
public class MirrorObject : MonoBehaviour {

    public string groundTag = "Ground"; //object with the Grounddata component
    public bool staticObject = false; //if static, the mirror objects aren't updated

    GroundData ground;

    Vector3 previousPos; //previous frame pos
    
    float mirrorDistMinZ; //when further -> this object is mirrored
    float mirrorDistMinX;
    float mirrorDistMaxX; //when further -> this object isn't mirrored
    float mirrorDistMaxZ;
    float mirrorDistMaxY;

    List<GameObject> clones = new List<GameObject>(); // also known as mirrors

    bool configured = false; //if start method is already caled

    public bool optimised = true;
    public string playerTag = "Player";
    int previousGroundIndex = -1;

    MirrorOptimiser mirrorOptimiser; //contains 
    MirrorTable mirrorTable; //contains all mirror rules 

    int groundIndex;


	// Use this for initialization
	void Start () {

        if (!configured)
        {
            configured = true; //make sure that clones won't duplicate themselves

            //initialize variables
            ground = GameObject.FindWithTag(groundTag).GetComponent<GroundData>();

            mirrorDistMinX = ground.mapBlockLengthX / 2;
            mirrorDistMinZ = ground.mapBlockLengthZ / 2;
            mirrorDistMaxX = ground.mapLengthX / 2;
            mirrorDistMaxZ = ground.mapLengthZ / 2;

            mirrorOptimiser = GameObject.FindWithTag(playerTag).GetComponent<MirrorOptimiser>();
            mirrorOptimiser.GetPlayerGroundIndex();

            //to avoid mirror loops --> negative Max side belongs to mirror area when equal, positve side doesn't
            if ((gameObject.transform.position.x >= -mirrorDistMaxX && gameObject.transform.position.x < mirrorDistMaxX) &&
                (gameObject.transform.position.z >= -mirrorDistMaxZ && gameObject.transform.position.z < mirrorDistMaxZ))
            {
                CreateClones();
            }
        }

        //make sure that object won't be touched anymore
        else
        {
            staticObject = true; 
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!staticObject)
        {
            //optimised way
            if (optimised)
            {

                //update mirror rules when object moved
                if (previousPos != transform.position)
                {
                    mirrorTable = mirrorOptimiser.GetMirrorRules(transform.position);
                    previousPos = transform.position;
                    previousGroundIndex = -1; //make sure that clones are updated
                }

                //update mirroring when player is moved
                if (previousGroundIndex != mirrorOptimiser.GetPlayerGroundIndex())
                {
                    UpdateClonesOptimised();
                    previousGroundIndex = mirrorOptimiser.GetPlayerGroundIndex();
                }

                

            }
            
            //old way
            else
            {
                if (previousPos != gameObject.transform.position)
                {
                    UpdateClones();
                    previousPos = gameObject.transform.position;
                }
                    
            }

            UpdateLoop();
        }
	}

    // When the object is destoyed
    void OnDestroy()
    {
        DestroyClones();
    }

    // Teleports the object if needed
    void UpdateLoop()
    {
        if (gameObject.GetComponent<PickupAble>() == null || !gameObject.GetComponent<PickupAble>().IsCarried)
        {
            if (gameObject.transform.position.x < -mirrorDistMaxX || gameObject.transform.position.x >= mirrorDistMaxX)
                TeleportAxisX(gameObject); //teleport the object 
            if (gameObject.transform.position.z < -mirrorDistMaxZ || gameObject.transform.position.z >= mirrorDistMaxZ)
                TeleportAxisZ(gameObject);
        }
    }

    //updates the location of the clones
    void UpdateClones()
    {
        Vector3 pos = gameObject.transform.position; // position
        float directionX = Mathf.Abs(pos.x) / pos.x; //equals 1 or -1
        float directionZ = Mathf.Abs(pos.z) / pos.z;

        int i = 0;
        float newPosX = (pos - new Vector3(ground.mapLengthX * directionX, 0, 0)).x;
        float newPosZ = (pos - new Vector3(0, 0, ground.mapLengthZ * directionZ)).z;

        foreach (GameObject clone in clones)
        {
            // clone.SetActive(!(newPosX >= -mirrorDistMaxX && newPosX < mirrorDistMaxX &&
            //      newPosZ >= -mirrorDistMaxZ && newPosZ < mirrorDistMaxZ)); //if inside the playing area, deactive the clone

            if (i == 0)
            { //X
                clone.transform.position = new Vector3(newPosX, pos.y, pos.z);
            }
            else if (i == 1)
            { //Z
                clone.transform.position = new Vector3(pos.x, pos.y, newPosZ);
            }
            else if (i == 2)
            { //X && Z
                clone.transform.position = new Vector3(newPosX, pos.y, newPosZ);
            }

            clone.transform.rotation = gameObject.transform.rotation;

            i++;
        }
    }

    //mirror one clone in a different location depending on player position
    void UpdateClonesOptimised()
    {

        int groundIndex = mirrorOptimiser.GetPlayerGroundIndex();
        Vector3 pos = gameObject.transform.position; // position
        float newPosX = pos.x + ground.mapLengthX * mirrorTable.GetRule(groundIndex).directionX;
        float newPosZ = pos.z + ground.mapLengthZ * mirrorTable.GetRule(groundIndex).directionZ;

        if (newPosX == pos.x && newPosZ == pos.z) //no mirror rules
            clones[0].transform.position = new Vector3(newPosX, ground.mapMinY, newPosZ); //hide it
        else //there are mirror rules
            clones[0].transform.position = new Vector3(newPosX, pos.y, newPosZ);

        clones[0].transform.rotation = transform.rotation; //mirror rotation also
        clones[0].transform.localScale = transform.lossyScale; //mirror size

    }


    //teleports the object in X axis
    void TeleportAxisX(GameObject o)
    {
        Vector3 pos = o.transform.position;
        float direction = pos.x / Mathf.Abs(pos.x);
        o.transform.position = new Vector3(pos.x - ground.mapLengthX * direction, pos.y, pos.z); //teleport the player
    }

    //teleports the object in Z axis
    void TeleportAxisZ(GameObject o)
    {
        Vector3 pos = o.transform.position;
        float direction = pos.z / Mathf.Abs(pos.z);
        o.transform.position = new Vector3(pos.x, pos.y, pos.z - ground.mapLengthZ * direction); //teleport the player
    }


    // Creates mirrors / clones, if it's in the mirror area
    void CreateClones()
    {
        Vector3 pos = gameObject.transform.position; // position
        float directionX = Mathf.Abs(pos.x) / pos.x; //equals 1 or -1
        float directionZ = Mathf.Abs(pos.z) / pos.z;


        //if static, clones are only created if needed
        if (staticObject)
        {
            //positive Min side belongs to mirror area when equal, negative side doesn't --> opposite as Max
            if (pos.x >= mirrorDistMinX || pos.x < -mirrorDistMinX) //(X axis, horizontally)
            {
                clones.Add(Instantiate(gameObject, pos - new Vector3(ground.mapLengthX * directionX, 0, 0), gameObject.transform.rotation));
            }

            if (pos.z >= mirrorDistMinZ || pos.z < -mirrorDistMinZ) //(Z axis, vertically)
            {
                clones.Add(Instantiate(gameObject, pos - new Vector3(0, 0, ground.mapLengthZ * directionZ), gameObject.transform.rotation));
            }

            if ((pos.x >= mirrorDistMinX || pos.x < -mirrorDistMinX) && //(X and Z axis, diagonally)
                (pos.z >= mirrorDistMinZ || pos.z < -mirrorDistMinZ)) 
            {
                clones.Add(Instantiate(gameObject, pos - new Vector3(ground.mapLengthX * directionX, 0, ground.mapLengthZ * directionZ), gameObject.transform.rotation));
            }

            SimplifyClones();
        }

        //if not static
        else
        {
            if (optimised) //1 clone is made
            {
                mirrorTable = mirrorOptimiser.GetMirrorRules(pos); //get mirror rules
                clones.Add(Instantiate(gameObject, new Vector3(0, ground.mapMinY, 0), gameObject.transform.rotation));

                SimplifyClones(); 
                UpdateClonesOptimised();
            }
            else //if not, 3 clones are created immediately
            {
                for (int i = 0; i < 3; i++)
                    clones.Add(Instantiate(gameObject, new Vector3(0, ground.mapMinY, 0), gameObject.transform.rotation));

                SimplifyClones();
                UpdateClones();
            }

            previousPos = new Vector3(0,0,0);
            
        }
        
    }

    //destroys unwanted components from the clone
    void SimplifyClones()
    {
        foreach (GameObject clone in clones)
        {
            //delete worthless components
            if (clone.gameObject.GetComponent<Rigidbody>() != null)
                Destroy(clone.gameObject.GetComponent<Rigidbody>());

            if (clone.gameObject.GetComponent<MeshCollider>() != null)
                Destroy(clone.gameObject.GetComponent<MeshCollider>());
            if (clone.gameObject.GetComponent<SphereCollider>() != null)
                Destroy(clone.gameObject.GetComponent<SphereCollider>());
            if (clone.gameObject.GetComponent<CapsuleCollider>() != null)
                Destroy(clone.gameObject.GetComponent<CapsuleCollider>());
            if (clone.gameObject.GetComponent<AudioSource>() != null)
                Destroy(clone.gameObject.GetComponent<AudioSource>());
            if (clone.gameObject.GetComponent<BoxCollider>() != null)
                Destroy(clone.gameObject.GetComponent<BoxCollider>());
            if (clone.gameObject.GetComponent<NavMeshAgent>() != null)
                Destroy(clone.gameObject.GetComponent<NavMeshAgent>());

            //Delete all scripts
            foreach (MonoBehaviour script in clone.gameObject.GetComponents<MonoBehaviour>())
                Destroy(script);

            clone.gameObject.transform.parent = GameObject.FindWithTag("ClonesHolder").transform ; //set clones to main object childrens
            clone.transform.localScale = transform.lossyScale; //mirror size



        }
    }


    //Destroys all the clones / mirros
    void DestroyClones()
    {
        foreach (GameObject clone in clones)
        {
            Destroy(clone);
        }
        clones.Clear();
    }

    
}
