using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    //Movement radius
    [SerializeField] protected Transform _VisualRadius;

    //Current linked box.
    GameObject boxRef;

    //Forward declared variable
    int boardLimit;

    //Board reference and position.
    protected Board boardRef;
    protected int[] boardPos = new int[2];

    //Stats and identifier
    protected int ID;
    [SerializeField] protected int Health;
    protected static int MaxHealth;
    [SerializeField] protected int HealthPerTurn;

    [SerializeField] protected int MoveSpeed = 1; //How many blocks the unit can travel per turn. 
    [SerializeField] protected Transform HealthBar;
    [SerializeField] RandomizedSoundComponent soundOutput;

    //Unit forward displacement
    protected const float Z_SIZE = -0.45f;

    //Controller
    protected BaseController _controller;


    //Pathfinding
    protected MovementComponent pathing;
    protected List<(int, int)> pathList;


    //Store actions for each turn
    protected (int?, int?) _lastActionVariables;
    protected System.Action<int, int> _lastAction;

    //Activated ability, for turn.
    protected BaseAbility _activeAbility;

    List<Vector3> targetPosition;

    protected const int MIN_ADMIN_DAMAGE = 100;
    private void Start()
    {
        UnitStart();
 
    }

    //Cannot use base.Start in inherited functions. 
    protected void UnitStart()
    {
        //Fix any potential issues with spawning.
        MoveUnit(0, 0);
        ControllerMovementUpdate();
        //Radius should never show on creation.
        HideMoveRadius();

        //Preparing for real time adjustments to the radius.
        if (!(_VisualRadius.localScale.x > MoveSpeed + MoveSpeed + 1 - 0.01 && _VisualRadius.localScale.x < MoveSpeed + MoveSpeed + 1 + 0.01))
            _VisualRadius.localScale = new Vector3(MoveSpeed + MoveSpeed + 1, MoveSpeed + MoveSpeed + 1, 1);

        MaxHealth = Health;
    }

    /////////////////
    //Called by the TurnManager at the end of each turn. 
    //ENtities are essentially any placeable unit. This "Turnpass" is for any 'living' entity which may move or attack in a turn. 
    public virtual void TurnPass()
    {
        DamageUnit(-HealthPerTurn);
        //Preparing for real time adjustments to the radius.
        if (!(_VisualRadius.localScale.x > MoveSpeed + MoveSpeed + 1 - 0.01 && _VisualRadius.localScale.x < MoveSpeed + MoveSpeed + 1 + 0.01))
            _VisualRadius.localScale = new Vector3(MoveSpeed + MoveSpeed + 1, MoveSpeed + MoveSpeed + 1, 1);

        if (!_controller.NextTurn() && _lastAction != null)
        {
            //Nextturn should check for ability usage. If it reaches this point then the unit is moving. 
            for(int i = 0; i < MoveSpeed; ++i)
            if (pathList != null && pathList.Count > 0)
            {
                _lastAction.Invoke(0, pathList[0].Item1 - boardPos[0]);
                _lastAction.Invoke(1, pathList[0].Item2 - boardPos[1]);

                Board.instance.getBox(pathList[0].Item1, pathList[0].Item2).unHighlightBox();
                pathList.RemoveAt(0);
            }

        }
    }


    void Update()
    {

        if (targetPosition.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition[0], (targetPosition.Count * 2.0f) * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, Z_SIZE * transform.localScale.z * 1.5f);

            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetPosition[0].x, targetPosition[0].y)) < .001f)
                targetPosition.RemoveAt(0);
        }



    }


    /////////////////
    //Allows the user to set up units in code. This gives potential for loading n from text files. 
    public void SetupUnit(int id, int boardPosX, int boardPosY, Board reference, int? health = null, int? moveSpeed = null, int? damage = null)
    {
        //In theory, the entity manager will instantly call this, giving nothing else time to abuse this function. 

            ID = id;
            boardPos[0] = boardPosX;
            boardPos[1] = boardPosY;
            boardRef = reference;
            pathing = new MovementComponent();
        targetPosition = new List<Vector3>();

        //If these are null then set to default.
        if (health != null)
                Health = (int)health;

            if (moveSpeed != null)
                MoveSpeed = (int)moveSpeed;

        if (!boardRef.getBox(boardPos[0], boardPos[1]).AddUnit(this))
            DamageUnit(99999);
    }

    /////////////////
    //Controller controls
    public BaseController GetController()
    {
        return _controller;
    }

    public bool SetController(BaseController Controller)
    {
        if (_controller == null || _controller == Controller)
        {

            _controller = Controller;
            return true;
        }
        return false;
    }
    public void ControllerMovementUpdate()
    {
        //Preparing for real time adjustments to the radius.
        if (!(_VisualRadius.localScale.x > MoveSpeed + MoveSpeed + 1 - 0.01 && _VisualRadius.localScale.x < MoveSpeed + MoveSpeed + 1 + 0.01))
            _VisualRadius.localScale = new Vector3(MoveSpeed + MoveSpeed + 1, MoveSpeed + MoveSpeed + 1, 1);

        _lastAction = MoveUnit;
    }


    /////////////////
    //Movement controls
    public (int, int) GetGridPos()
    {
        return (boardPos[0], boardPos[1]);
    }

    public virtual void SetDestination((int, int) target)
    {

        //Originally this code had a limitation on distance but with how the NextTurn has been written it will automatically reach the move limit.
        //What this does is it allows me to directly set the EnemyUnits target to the player

        // if (target.Item1 <= boardPos[0] + MoveSpeed && target.Item2 <= boardPos[1] + MoveSpeed&&
        //     target.Item1 >= boardPos[0] - MoveSpeed && target.Item2 >= boardPos[1] - MoveSpeed
        //     )
        //

        //Remove evidence of previous path. (If the user decides to change paths in a single turn, essentially)
        if (pathList != null && pathList.Count > 0)
            for (int i = 0; i < pathList.Count; ++i)
                Board.instance.getBox(pathList[i].Item1, pathList[i].Item2).unHighlightBox();


        pathList = pathing.getPath((boardPos[0], boardPos[1]), target);
        pathList.RemoveAt(0); //Holds the current position. Useful in set scenarios but in this case it would delay movement by a turn. 

        _lastAction = MoveUnit;

    }



    //Virtual purely to give the ability to the PlayerUnit override the capacity for unique interactinos, i.e. Camera updating on each movement. 
    //Potential usage: Leave fire trail
    protected virtual void MoveUnit(int xy, int increment)
    {
        if (xy == 0)
        {
            boardLimit = Board.instance.GetBoardDimensions().Item1;
        }
        else boardLimit = Board.instance.GetBoardDimensions().Item2;

        if (boardPos[xy] + increment < boardLimit && increment + boardPos[xy] >= 0)
        {
            boardRef.getBox(boardPos[0], boardPos[1]).RemoveUnit();
            boardPos[xy] += increment;

            if (!boardRef.getBox(boardPos[0], boardPos[1]).AddUnit(this))
            {
                boardPos[xy] -= increment;


                if (!boardRef.getBox(boardPos[0], boardPos[1]).AddUnit(this))
                {
                    //Kill the unit if something broke.
                    while (Health > 0)
                    {
                        DamageUnit(99999);
                    }
                }

                return;
            }

            boxRef = boardRef.getBox(boardPos[0], boardPos[1]).gameObject;


            soundOutput.PlaySound(0);

            targetPosition.Add(new Vector3(boxRef.gameObject.transform.position.x, boxRef.gameObject.transform.position.y, boxRef.gameObject.transform.position.z) + new Vector3(0, 0, Z_SIZE * transform.localScale.z * 1.5f));
                 //Z scaling is not touched so it is not needed for calculations.
        }
    }

    public int GetDistToTarget()
    {
        if(pathList != null)
        return pathList.Count;

        return 404; //Error not found
    }

    public int GetMaxDistTravel()
    {
        return MoveSpeed;
    }

    /////////////////
    //Ability interactions
    public BaseAbility GetActiveAbility()
    {
        return _activeAbility;
    }
    public void SetActiveAbility(BaseAbility ability)
    {
        _activeAbility = ability;
    }

    /////////////////
    
    //Visual radius control
    public void ToggleMoveRadius()
    {
        _VisualRadius.gameObject.SetActive(!_VisualRadius.gameObject.activeSelf);
    }

    public void ShowMoveRadius()
    {
        _VisualRadius.gameObject.SetActive(true);
    }

    public void HideMoveRadius()
    {
        _VisualRadius.gameObject.SetActive(false);
    }
    public Transform GetMoveRadius()
    {
     return _VisualRadius.transform;
    }



    /////////////////////////////////////////////////////////////////////////////
    //Used for children i.e. the player will end the game on death or remove a life. Enemies may explode.
    //Causes and reactions to damage. This gives the capacity for enemies that might, for example, explode on death or reflect damage. 

    //Return true if dead
    protected virtual void OnDeath() {
        worldManager.instance.DeleteEntity(this, ID);
        boardRef.getBox(boardPos[0], boardPos[1]).RemoveUnit();
        Destroy(gameObject);
    }
    protected virtual void OnDamage() { }
    public void DamageUnit(int damage)
    {
        if(soundOutput != null && damage > 0 && damage < MIN_ADMIN_DAMAGE)
        soundOutput.PlaySound(1);

        if (MaxHealth < Health) 
            MaxHealth = Health;

        //If damage is over 100 then it is probably a scripted death. If power creep reaches this point then it is likely this is being reused and rewritten anyway. 
        if(damage < MIN_ADMIN_DAMAGE)
        OnDamage();

        if (Health - damage < MaxHealth)
        Health -= damage;    

        setHealthPercentage((float)Health / (float)MaxHealth);

        if (Health <= 0)
        {
            OnDeath();
            
        }
    }

    public void setHealthPercentage(float input)
    {
        if (input > 1 || input < 0) return;
        if(HealthBar != null)
        HealthBar.localScale = new Vector3(input, transform.localScale.y, transform.localScale.z);
    }

    public float getHealthPercentage()
    {
        return (float)Health / (float)MaxHealth;

    }
    /////////////////////////////////////////////////////////////////////////////
}
