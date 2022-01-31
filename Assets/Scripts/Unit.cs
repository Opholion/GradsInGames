using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] protected Transform _VisualRadius;

    GameObject boxRef;
    int boardLimit;

    protected Board boardRef;
    protected int[] boardPos = new int[2];

    protected int ID;
    [SerializeField] protected int Health;
    [SerializeField] protected int MoveSpeed = 1; //How many blocks the unit can travel per turn. 

    protected const float Z_SIZE = -0.45f;

    protected BaseAbility _activeAbility;

    protected BaseController _controller;
    protected MovementComponent pathing;
    protected List<(int, int)> pathList;
    const float PATH_TIME_DELAY = .33f;
    protected float currentDelay;

    protected int? _gameTurn = null;


    protected (int?, int?) _lastActionVariables;
    protected System.Action<int,int> _lastAction;


    public void TurnPass()
    {

        //Preparing for real time adjustments to this.
        if(!(_VisualRadius.localScale.x > MoveSpeed + MoveSpeed + 1 - 0.01 &&_VisualRadius.localScale.x < MoveSpeed + MoveSpeed + 1 + 0.01))
        _VisualRadius.localScale =new Vector3(MoveSpeed+MoveSpeed+1, MoveSpeed + MoveSpeed + 1, 1);

        if (_lastAction  != null && !_controller.NextTurn())
        {
            //If the player has not attempted to force a new position then go to the last stored pathfinder position, if it exists.
            if (pathList != null && pathList.Count > 0)
            {
                _lastAction.Invoke(0, pathList[0].Item1 - boardPos[0]);
                _lastAction.Invoke(1, pathList[0].Item2 - boardPos[1]);
                
                Board.instance.getBox(pathList[0].Item1, pathList[0].Item2).unHighlightBox();
                pathList.RemoveAt(0);
            }
            else if (_lastActionVariables.Item1 != null && _lastActionVariables.Item2 != null)
            {
                if (pathList != null)
                    pathList.Clear();

                if(_lastAction != null)
                _lastAction.Invoke((int)_lastActionVariables.Item1, (int)_lastActionVariables.Item2);
                _lastActionVariables = (null, null);
                _lastAction = null;
            }
        }


        if (_gameTurn == null)
        {
            _gameTurn = TurnManager.instance.GetTurn();
        }
        ++_gameTurn;
    }
    public void SetupUnit(int id, int boardPosX, int boardPosY, int health, int damage, int moveSpeed, Board reference)
    {
        //In theory, the entity manager will instantly call this, giving nothing else time to abuse this function. 
        if (ID != null)
        {
            ID = id;
            boardPos[0] = boardPosX;
            boardPos[1] = boardPosY;
            Health = health;
            MoveSpeed = moveSpeed;
            boardRef = reference;
            pathing = new MovementComponent();
            currentDelay = PATH_TIME_DELAY;
        }
    }

    public BaseController GetController()
    {
        return _controller;
    }

    public void SetController(BaseController Controller)
    {
        if (_controller == null) _controller = Controller;
    }
    public void ControllerUpdate((int, int) input)
    {
     _lastActionVariables = input;
        _lastAction =  MoveUnit;
    }

    public (int, int) GetGridPos()
    {
        return (boardPos[0], boardPos[1]);
    }

    public void SetDestination((int, int) target)
    {
        if (target.Item1<= boardPos[0] + MoveSpeed && target.Item2 <= boardPos[1] + MoveSpeed)
        {
            if (pathList != null && pathList.Count > 0)
                for(int i =0; i < pathList.Count; ++i)
                    Board.instance.getBox(pathList[i].Item1, pathList[i].Item2).unHighlightBox();


            pathList = pathing.getPath((boardPos[0], boardPos[1]), target);
            pathList.RemoveAt(0); //Holds the current position. Useful in set scenarios but in this case it would delay movement by a turn. 
            for (int i = 0; i < pathList.Count; ++i)
            {
                Board.instance.getBox(pathList[i].Item1, pathList[i].Item2).highlightBox(true);
            }
            _lastAction = MoveUnit;
        }
    }
    //Used for children i.e. the player will end the game on death or remove a life. Enemies may explode.
    protected virtual void OnDeath() { }
    protected virtual void OnDamage() { }

    protected virtual (Unit, int) GetTargetAndDistance()
    {
        return (null,0-1);
    }
    protected void DamageUnit(int damage, int padding)
    {
        Health -= damage;
        if (Health <= 0)
        {
            worldManager.instance.makeEntityAvailable(ID);
            OnDeath();
            Destroy(this);
        }
        else
        {
            OnDamage();
        }
    }


    //Virtual purely to give the ability to the PlayerUnit override the capacity for unique interactinos, i.e. Camera updating on each movement. 
    protected virtual void MoveUnit(int xy, int increment)
    {
        if (xy == 0)
        {
            boardLimit = Board.bWIDTH;
        }
        else boardLimit = Board.bHEIGHT;

        if (boardPos[xy] + increment < boardLimit && increment + boardPos[xy] >= 0)
        {
            boardPos[xy] += increment;
            boxRef = boardRef.getBox(boardPos[0], boardPos[1]).gameObject;

            this.transform.position =
                boxRef.gameObject.transform.position + new Vector3(0, 0, Z_SIZE * transform.localScale.z * 1.5f); //Z scaling is not touched so it is not needed for calculations.
        }
    }

    protected void Update()
    {

    }
    /*
             if (pathList != null) //Once this is true it should always be true and so it shouldn't effect performance.
        {
            currentDelay += Time.deltaTime;
            if (pathList.Count > 0 && currentDelay > PATH_TIME_DELAY)
            {
                currentDelay = 0;


                boardPos[0] = pathList[0].Item1;
                boardPos[1] = pathList[0].Item2;
                Board.instance.getBox(pathList[0].Item1, pathList[0].Item2).unHighlightBox();

                pathList.RemoveAt(0);
                boxRef = boardRef.getBox(boardPos[0], boardPos[1]).gameObject;
                transform.position =
                    boxRef.gameObject.transform.position + new Vector3(0, 0, Z_SIZE * transform.localScale.z * 1.5f); //Z scaling is not touched so it is not needed for calculations.

            }
        }




     * */

    public BaseAbility GetActiveAbility()
    {
        return _activeAbility;
    }
    public void SetActiveAbility(BaseAbility ability)
    {
        _activeAbility = ability;
    }
}
