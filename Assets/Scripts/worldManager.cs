using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class worldManager : MonoBehaviour
{
    #region PosRef_Singleton

    public static worldManager instance;

    private void Awake()
    {
        instance = this;
        InitiateValues();
        Random.seed = Random.Range(0, 10000);
        seed = Random.value;
    }
    #endregion



    private UI UserInterfaceRef;
    private GameObject unitParent; //Gives each unit a local position reference for the board.

    private UnityEngine.Camera cameraRef;
    private Board boardRef;
    private float seed;

    private Transform cameraPos;
    private int[] gridCentralPos;
    static int[] gridSize;
    static Vector2 gridScale;

    readonly float[] BLOCK_SIZE = new float[] { 1.0f, 1.0f };

    public const int GRID_EDGE_DIST = 5;

    //Setup
    void Start()
    {
        if(Board.bHEIGHT + Board.bWIDTH - 1 > GRID_EDGE_DIST + GRID_EDGE_DIST)
        for (int i = gridCentralPos[0] - (GRID_EDGE_DIST); i < gridCentralPos[0] + GRID_EDGE_DIST; ++i)
        {
            for (int j = gridCentralPos[1] - (GRID_EDGE_DIST); j < gridCentralPos[0] + GRID_EDGE_DIST; ++j)
            {
                boardRef.getBox(i, j).gameObject.SetActive(true);
            }
        }

        gridScale = boardRef.transform.localScale;
        trySpawnPlayer(gridCentralPos[0], gridCentralPos[1]);

       
    }
 


    void InitiateValues()
    {
        boardRef = GameObject.FindWithTag("Board").GetComponent<Board>();
        gridSize = new int[2] { Board.bHEIGHT, Board.bWIDTH };
        gridCentralPos = new int[2] { gridSize[0] / 2, gridSize[1] / 2 }; //Get the middle point
        cameraRef = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cameraPos = cameraRef.transform;
        unitParent = GameObject.FindWithTag("UnitParent");
        UserInterfaceRef = GameObject.FindGameObjectWithTag("UserInterface").GetComponent<UI>();

        openEntitySlots = new Stack<int>();
        for (int i = 0; i < entityLimit; ++i)
            openEntitySlots.Push(i);
    }

    public void ActivateMenu()
    {
        UserInterfaceRef.gameObject.SetActive(!UserInterfaceRef.gameObject.activeSelf);
    }


    //Getters
    public float GetSeed()
    {
        return seed;
    }
    public Board GetBoard()
    {
        return boardRef;
    }



    public UnityEngine.Camera GetCamera()
    {
        return cameraRef;
    }














































    //EntityMan
    public const int entityLimit = 15;
    private Unit[] entities = new Unit[entityLimit];
    private Stack<int> openEntitySlots;

    private int playerID = -1;

    //EntityMan temp variables (More efficient to pre-set.)
    int tempEntityID;
    GameObject tempEntityGameObject;
    Unit tempUnit;

    //EntityMan - Developer side variables
    [SerializeField] GameObject playerUnit;
    [SerializeField] GameObject[] spawnableEnemies;


    public Unit GetUnit(int id)
    {
        return entities[id];
    }

    public Unit GetPlayer()
    {
        if (playerID == -1) return null;
        return entities[playerID];
    }

    public int trySpawnRandomEnemy(int positionX, int positionY)
    {
        if (spawnableEnemies.Length == 0)
        {
            return -1;
        }

        return trySpawnUnit(spawnableEnemies[Random.Range(0, spawnableEnemies.Length)], positionX, positionY);
    }

    public int trySpawnPlayer(int positionX, int positionY)
    {
        if (playerID == -1)
        {
            playerID = trySpawnUnit(playerUnit, positionX, positionY);
            CameraMovement.instance.forcePositionUpdate();
            return playerID;
        }

        //This is to inform the 'caller' that this function has failed.
        return -1;
    }

    private int trySpawnUnit(GameObject unit, int positionX, int positionY)
    {

        tempEntityID = isEntityavailable();
        if (tempEntityID >= 0) //No way for an ID to be less than 0
        {
            tempEntityGameObject = Instantiate(playerUnit, unitParent.transform);
            tempEntityGameObject.TryGetComponent<Unit>(out tempUnit);

            tempEntityGameObject.transform.position = boardRef.getBox(positionX, positionY).gameObject.transform.position;
            //tempEntityGameObject.transform.localScale = new Vector3(gridScale.x,gridScale.y,1.0f);
            tempEntityGameObject.transform.localRotation = new Quaternion(90, 0, 0,1);
            if (tempUnit != null)
            {
                tempUnit.SetupUnit(tempEntityID, positionX, positionY, 100, 10,2, boardRef);
                entities[tempEntityID] = tempUnit;

                tempUnit = null;
                tempEntityGameObject = null;
                return tempEntityID;
            }
            else
            {
                makeEntityAvailable(tempEntityID);
            }
        }
        return -1;
    }

    private int isEntityavailable()
    {
        if (openEntitySlots.Count > 0)
        {
            int output = openEntitySlots.Peek();
            openEntitySlots.Pop();
            return output;
        }
        return -1;
    }

    public void makeEntityAvailable(int id)
    {
        //Assume this is only being used on deletion. Should not require safety checks. Passing additional parameters will take additional time. 
        openEntitySlots.Push(id);
    }





}
