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
    }
    #endregion

    private UI UserInterfaceRef;
    private GameObject unitParent; //Gives each unit a local position reference for the board.

    private UnityEngine.Camera cameraRef;
    private Board boardRef;



    //Setup
    void Start()
    {
        _SpawnedEnitites = new Unit[EntityLimit];
        _OpenIDs = new List<int>();
        for (int i = 0; i < EntityLimit; ++i)
            _OpenIDs.Add(i);
    }
    void InitiateValues()
    {
        boardRef = GameObject.FindWithTag("Board").GetComponent<Board>();
        cameraRef = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        unitParent = GameObject.FindWithTag("UnitParent");
        UserInterfaceRef = GameObject.FindGameObjectWithTag("UserInterface").GetComponent<UI>();
    }

    
    public void EndGame()
    {
        UserInterfaceRef.gameObject.SetActive(true);
        
    }
    public Board GetBoard()
    {
        return boardRef;
    }

    public UnityEngine.Camera GetCamera()
    {
        return cameraRef;
    }


/////////////////////////////////////
///Basic Entity manager
///
[SerializeField] int EntityLimit =500;
    private Unit playerRef;

    //EntityMan temp variables (More efficient to pre-set.)
    int tempEntityID;
    GameObject tempEntityGameObject;
    Unit tempUnit;
    Unit[] _SpawnedEnitites ;
    List<int> _OpenIDs;

    //EntityMan - Developer side variables
    [SerializeField] GameObject playerUnit;
    [SerializeField] GameObject[] spawnableEnemies;
    [SerializeField] GameObject[] spawnableCollectibles;

    public void RemoveUnits()
    {
        //If something is immor
        //iftal then this is going to cause issues
        foreach (Unit unit in _SpawnedEnitites)
        {
            if(unit != null && unit.getHealthPercentage() > 0.0f)
            unit.DamageUnit(250);
        }
    }

    public Unit GetPlayer()
    {
        if (playerRef == null && boardRef.isGameActive())
            PlayerController.instance.ResetGame();

        return playerRef;
    }

    public Unit trySpawnRandomCollectible((int, int) position)
    {
        //Really not good to do this but it happens rarely, in small amounts and this function is more for utility (overloading it) in its design. 
       return trySpawnRandomCollectible(position.Item1, position.Item2);
    }

    public Unit trySpawnRandomCollectible(int positionX, int positionY)
    {
        if (spawnableCollectibles.Length == 0)
        {
            return null;
        }

        return trySpawnUnit(spawnableCollectibles[Random.Range(0, spawnableCollectibles.Length)], positionX, positionY);
    }

    public Unit trySpawnRandomEnemy((int,int) position)
    {
       return trySpawnRandomEnemy( position.Item1,  position.Item2);
    }

    public Unit trySpawnRandomEnemy(int positionX, int positionY)
    {
        if (spawnableEnemies.Length == 0)
        {
            return null;
        }

        return trySpawnUnit(spawnableEnemies[Random.Range(0, spawnableEnemies.Length)], positionX, positionY);
    }


    public Unit trySpawnPlayer((int, int) position)
    {
     
        return trySpawnPlayer(position.Item1, position.Item2);
        return null;
    }
    public Unit trySpawnPlayer(int positionX, int positionY)
    {
        if (playerRef == null)
        {
            playerRef = trySpawnUnit(playerUnit, positionX, positionY);
            CameraMovement.instance.forcePositionUpdate();
        }
        //Should either hold the player or null. 
        return playerRef;
    }

    private Unit trySpawnUnit(GameObject unit, int positionX, int positionY)
    {
        //Forcefully stop any stacked entities on spawn.
        if (!Board.instance.IsBoxEmpty((positionX, positionY))) return null;

            tempEntityGameObject = Instantiate(unit, unitParent.transform);
            tempEntityGameObject.TryGetComponent<Unit>(out tempUnit);

            tempEntityGameObject.transform.position = boardRef.getBox(positionX, positionY).gameObject.transform.position;
            //tempEntityGameObject.transform.localScale = new Vector3(gridScale.x,gridScale.y,1.0f);
            tempEntityGameObject.transform.localRotation = new Quaternion(90, 0, 0,1);
            if (tempUnit != null)
            {
                tempUnit.SetupUnit(_OpenIDs[0], positionX, positionY, boardRef);
                _SpawnedEnitites[_OpenIDs[0]] = tempUnit;
            _OpenIDs.RemoveAt(0);
                return tempUnit;
            }
        return null;
    }


    public void DeleteEntity(Unit target, int ID)
    {
        //Small safety check
        if (_SpawnedEnitites[ID] == target)
        {
            _SpawnedEnitites[ID] = null;

            _OpenIDs.Add(ID);
        }
    }



}
