using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//calculates in which terrain player is
public class MirrorOptimiser : MonoBehaviour {

    public string groundTag = "Ground"; //object with the Grounddata component
    GroundData ground;

    float mirrorDistMinZ; //when further -> this object is mirrored
    float mirrorDistMinX;

    public int playerGroundIndex;

    MirrorTable[] mirrorTables;

	// Use this for initialization
	void Start () {

        //initialize variables
        ground = GameObject.FindWithTag(groundTag).GetComponent<GroundData>();

        mirrorDistMinX = ground.mapBlockLengthX / 2;
        mirrorDistMinZ = ground.mapBlockLengthZ / 2;

        CreateMirrorRules(); //to 3x3 map
        playerGroundIndex = GetGroundIndex(transform.position); //update player ground index

    }
	
	// Update is called once per frame
	void Update () {

        playerGroundIndex = GetGroundIndex(transform.position); //update player ground index
    }

    //returns playergroundindex
    public int GetPlayerGroundIndex()
    {
        return playerGroundIndex;
    }


    //return all mirror rules for given position
    public MirrorTable GetMirrorRules(Vector3 pos)
    {
        return mirrorTables[GetGroundIndex(pos)];
    }


    //return the ground plate index of 'pos'
    //MAP =
    //|789|
    //|456|
    //|123|
    public int GetGroundIndex(Vector3 pos)
    {
        int[] cord = new int[3] { -1, 1, 5 }; //-1 = left, 1 = middle, 5 = right + leeway
        int index = 0;

        //loops all tiles 
        foreach (int z in cord)
        {
            if(pos.z <= mirrorDistMinZ * z)
            {
                foreach (int x in cord) //check x-row
                {
                    index++;
                    if (pos.x <= mirrorDistMinX * x) //if the object is in this ground plate
                    {
                        return index; //plate found    
                    }
                }
                break;
            }
            index += 3;
        }

        return 0; //object isn't on any ground plate
    }


    //creates mirror rules for 3x3 map8
    //MAP =
    //|789|
    //|456|
    //|123|
    void CreateMirrorRules()
    {
        const int size = 3; //3*3 = 9
        mirrorTables = new MirrorTable[size*size+1];

        //rule for left-down tile
        //when the object is in the south, mirror object when the player is in the north
        mirrorTables[1] = new MirrorTable(size, size);
        mirrorTables[1].rule[2, 0] = new MirrorRule(1, 0); //right x-axis
        mirrorTables[1].rule[2, 1] = new MirrorRule(1, 0);
        mirrorTables[1].rule[0, 2] = new MirrorRule(0, 1); //up z-axis
        mirrorTables[1].rule[1, 2] = new MirrorRule(0, 1);
        mirrorTables[1].rule[2, 2] = new MirrorRule(1, 1); //right-up x&z-axis

        //rule for middle-down tile
        //when the object is in the south, mirror object when the player is in the north
        mirrorTables[2] = new MirrorTable(size, size);
        mirrorTables[2].rule[0,2] = new MirrorRule(0, 1);
        mirrorTables[2].rule[1,2] = new MirrorRule(0, 1);
        mirrorTables[2].rule[2,2] = new MirrorRule(0, 1);

        //create rules for other tiles by rotating older rules
        mirrorTables[4] = mirrorTables[2].RotateClcokWise();
        mirrorTables[8] = mirrorTables[4].RotateClcokWise();
        mirrorTables[6] = mirrorTables[8].RotateClcokWise();
        mirrorTables[7] = mirrorTables[1].RotateClcokWise();
        mirrorTables[9] = mirrorTables[7].RotateClcokWise();
        mirrorTables[3] = mirrorTables[9].RotateClcokWise();

        //middle one without rules
        mirrorTables[5] = new MirrorTable(size, size);
    }

}




//table with rules
//tells where the object should be mirrored depending on
//player position
public class MirrorTable
{
    public MirrorRule[,] rule; //index as player's position as ground index

    public MirrorTable(int sizeX, int sizeZ)
    {
        rule = new MirrorRule[sizeX,sizeZ];

        //create empty mirror table
        for (int x=0; x < sizeX; x++) 
            for (int z = 0; z < sizeZ; z++)
                rule[x,z] = new MirrorRule(0, 0);
    }

    //return rotates table clockwise
    //only works if square
    public MirrorTable RotateClcokWise()
    {
        int size = rule.GetLength(0);
        MirrorTable rotated = new MirrorTable(size, size);

        for (int x=0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                rotated.rule[x, z] = rule[size - z - 1, x].RotateClockkWise();
            }
        }

        return rotated;
    }

    //returns mirrorrule
    public MirrorRule GetRule(int groundIndex)
    {
        groundIndex--; //indexing start at 0

        if (groundIndex >= 0 && groundIndex <= rule.Length)
            return rule[groundIndex % 3, Mathf.FloorToInt(groundIndex / 3.0f)];
        else
            return new MirrorRule(0, 0);
    }
}

//direction where the object should be mirrored
public class MirrorRule
{
    public sbyte directionX;
    public sbyte directionZ;

    public MirrorRule(sbyte directionX, sbyte directionZ)
    {
        this.directionX = directionX;
        this.directionZ = directionZ;
    }

    //rotates rules clockwise
    public MirrorRule RotateClockkWise()
    {
        sbyte rotatedX = directionZ;
        sbyte rotatedZ = (sbyte) - directionX;

        return new MirrorRule(rotatedX, rotatedZ);
    }
}