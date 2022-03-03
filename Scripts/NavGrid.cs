using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour
{

    public float xDim = 3000f;
    public float zDim = 3000f;
    public float xOffset = -1500f;
    public float zOffset = -1500f;
    public float yTop = 200f;
    public float yBottom = 100f;
    public float squareSize = 30f;
    bool[ , ] grid;
    float yMid;
    Vector3 cellDim, halfCellDim;
    int gridX, gridZ;
    int layerMask = 1 << 7;

    // Start is called before the first frame update
    void Start()
    {
        gridX = (int)(xDim / squareSize);
        gridZ = (int)(zDim / squareSize);
        grid = new bool[(gridX), (gridZ)];

        yMid = yBottom + ((yTop - yBottom) / 2);
        cellDim = new Vector3(squareSize, (yTop - yBottom), squareSize);
        halfCellDim = cellDim / 2;
        //check each cell area for terrain colliders
        for (int i = 0; i < (xDim / squareSize); i++)
        {
            for (int j = 0; j < (zDim / squareSize); j++)
            {
                //if there is a navigation obstacle in this cell, set it'
                grid[i, j] = (Physics.CheckBox((new Vector3((i * squareSize) + xOffset, yMid, (j * squareSize) + zOffset)), halfCellDim, Quaternion.identity, layerMask));
            }

        }

        //record true in the corresponding cell if there is an obstacle
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            List<(int, int)> path = GeneratePath(3,3,5,7);
            foreach((int, int) t in path)
            {
                Debug.Log(t.Item1 + ", " + t.Item2);
            }
        }
    }

    List<(int, int)> GeneratePath(int startX, int startZ, int goalX, int goalZ)
    {

        List<(int, int)> bestPath = new List<(int, int)>();
        //hashsets for use in A* search
        //open format: ((int x, int z), (float f value, float g cost, (int parent_x, int parent_z)));
        Hashtable open = new Hashtable();
        //closed format ((int x, int z), (float f_value, (int parent_x, int parent_z)));
        Hashtable closed = new Hashtable();
        
        bool pathing = true;
        (int, int) tileXZ = (startX, startZ);
        (int, int) goalXZ = (goalX, goalZ);
        float tileF;
        float tileG = 0f;
        //-1s here meant to signify no parent / start of path
        (int, int) parentXZ = (-1, -1);
        //add start tile to open list
        open.Add((startX, startZ), (0f, 0f, parentXZ));
        pathing = true;
        while (pathing)
        {
            tileF = Mathf.Infinity;
            //set current node to lowest F cost in open
            foreach ((int, int) k in open.Keys)
            {

                (float, float, (int, int)) val = ((float, float, (int, int)))open[k];
                if (val.Item1 < tileF)
                {
                    tileXZ = k;
                    tileF = val.Item1;
                    tileG = val.Item2;
                    parentXZ = val.Item3;
                }
            }
            //remove current from open
            open.Remove(tileXZ);
            //add current to closed
            closed.Add((tileXZ), (tileF, parentXZ));

            if(tileXZ == goalXZ)
            {
                //eureka
                Debug.Log("made it");
                //make path
                (int, int) current = goalXZ;
                //until we're back at the start
                while (current.Item1 != startX && current.Item2 != startZ)
                {
                    //add current to path
                    bestPath.Add(current);
                    //set current to parent of current
                    (float, (int, int)) currentVal = ((float, (int, int)))closed[current];
                    current = currentVal.Item2;
                }
                return bestPath;
            }


            //Check neighbours and add or update them in open:
            //for each of the current tile's neighbours
            for(int nX = (tileXZ.Item1)-1; nX <= (tileXZ.Item1 + 1); nX++)
            {
                for (int nZ = (tileXZ.Item2) - 1; nZ <= (tileXZ.Item2 + 1); nZ++)
                {
                    //out of bounds / "is this me?" check
                    if(nX >= 0 && nZ >= 0 && nX < gridX && nZ < gridZ && nX != tileXZ.Item1 && nZ != tileXZ.Item2)
                    {
                        //if they are an obstacle or are in closed, skip to next
                        if (!grid[nX, nZ] && !closed.ContainsKey((nX, nZ)))
                        {
                            //calculate G, H and F for new
                            //G is squared distance travelled (because root calculations are computationally nasty)
                            float nGVal = tileG + (((nX - tileXZ.Item1) ^ 2) + ((nZ - tileXZ.Item2) ^ 2));
                            //H is squared distance to goal
                            float nHVal = (((goalX - nX) ^ 2) + ((goalZ - nZ) ^ 2));
                            //F is total cost, G + H costs
                            float nFcost = nGVal + nHVal;

                            //if neighbour is in open and its new F value is lower than its old one
                            if (open.ContainsKey((nX, nZ)))
                            {
                                (float, float, (int, int)) prevVal = ((float, float, (int, int)))open[(nX, nZ)];
                                if (prevVal.Item1 < nFcost)
                                {
                                    //update its f and g values and set current tile as parent
                                    open[(nX, nZ)] = (nFcost, nGVal, tileXZ);
                                }
                            }
                            else
                            {
                                //add it to open
                                open.Add((nX, nZ), (nFcost, nGVal, tileXZ));
                            }
                        }
                        
                    }

                }

            }
            //what is "parent"? I may need to change closed so it contains a parent as well as an f value
            //how is path determined?

        }


        //make path
        (int, int) current2 = goalXZ;
        //until we're back at the start
        while (current2.Item1 != startX && current2.Item2 != startZ)
        {
            //add current to path
            bestPath.Add(current2);
            //set current to parent of current
            (float, (int, int)) currentVal = ((float, (int, int)))closed[current2];
            current2 = currentVal.Item2;
        }


        return bestPath;
    }




    void OnDrawGizmos()
    {
        //draw grid squares as mesh boxes


        if (Application.isPlaying)
        {
            for (int i = 0; i < (xDim / squareSize); i++)
            {
                for (int j = 0; j < (zDim / squareSize); j++)
                {

                    if (grid[i, j])
                    {
                        Gizmos.color = Color.red;
                    }
                    
                    Gizmos.DrawWireCube((new Vector3((i*squareSize) + xOffset, yMid, (j*squareSize) + zOffset)),(cellDim));
                    Gizmos.color = Color.white;
                }
            
            }
        }
    }

}
