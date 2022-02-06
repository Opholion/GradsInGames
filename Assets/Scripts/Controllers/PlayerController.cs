using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    private Game _game;
    private AttackMenu _menu;


    private bool _activeGameState = false;
    private bool _isControllingPlayer = true;
    private bool _isMoving = false;

    private List<BaseController> CombatEnemyCount;


    #region PlayerController_Singleton

    public static PlayerController instance;


    private void Awake()
    {
        instance = this;
    }

    #endregion

    private void Start()
    {
        LastActionIndicator.instance.LinkButton(ToggleAbilityShow);
        UI_Movement.instance.LinkButton(ToggleMovementRadiusShow);
        CombatEnemyCount = new List<BaseController>();
    }
    // Update is called once per frame
    public void EnterCombat(BaseController enemy)
    {
        foreach (BaseController target in CombatEnemyCount)
        {
            if (enemy == target) return;
        }
        CombatEnemyCount.Add(enemy);

        if (CombatEnemyCount != null && CombatEnemyCount.Count > 0)
            SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.Combat);
        else
            SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.Gameplay);
    }

    public void LeaveCombat(BaseController enemy)
    {
        for (int i = 0; i < CombatEnemyCount.Count; ++i)
            if (enemy == CombatEnemyCount[i])
            {
                CombatEnemyCount.RemoveAt(i);
                break;
            }

        if (CombatEnemyCount != null && CombatEnemyCount.Count > 0)
            SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.Combat);
        else
            SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.Gameplay);
    }
    //Linked to whatver button is bound to movement
    private void ToggleMovementRadiusShow()
    {
        _ControlledObject.ControllerMovementUpdate();

        //Any grouped values are set together. If one is null, both are. 
        if (_AbilityTarget.Item1 != null)
        {
            //Remove ability so movement can be used.
            _CurrentAbility = null;
            Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();
        }

        //There is a toggle fu4nction alternative but this hard codes it to always show when "IsMoving" is true; Allowing safety checks to be placed
        if (_isMoving)
            _ControlledObject.HideMoveRadius();
        else
            _ControlledObject.ShowMoveRadius();

        _isMoving = !_isMoving;
    }

    //Linked to whatever button is bound to abilities
    private void ToggleAbilityShow()
    {
        _isMoving = false;
        //Assuming both equal false on start, this can assume making it equal not itself will work adequetly.
        _game.ToggleAbilitiesUI(); //Show UI
        _isControllingPlayer = !_isControllingPlayer; //Disable player input

        LastActionIndicator.instance.ResetAbilityOutput(); //Visual output

        _ControlledObject.HideMoveRadius(); //Hide radius unless movement or ability selected.
        _CurrentAbility = null; //Resets ability if unset. 

        if (_AbilityTarget != (null, null))
            Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();

        _AbilityTarget = (null, null);

    }
    //The below should rarely be used so they can be done without excessive optimization.

    //As the controller manages what the player can do, it should know when the game is inactive.
    public void UpdateGameState()
    {
        _activeGameState = _game.GetGameState();
    }
    public void  ResetGame()
    {
        if (!_activeGameState) return;

        worldManager.instance.RemoveUnits();
        worldManager.instance.EndGame();
        _activeGameState = false;
            _game.OnClickedReset();
    }

    public override void SetActiveAbility(BaseAbility ability)
    {
        base.SetActiveAbility(ability);
        _ControlledObject.ShowMoveRadius();
        _game.ToggleAbilitiesUI();
        _isControllingPlayer = !_isControllingPlayer;


        Transform RadiusRef = _ControlledObject.GetMoveRadius();
        //Preparing for real time adjustments to the radius.
        if (!(RadiusRef.localScale.x > _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1 - 0.01 && RadiusRef.localScale.x < _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1 + 0.01))
            RadiusRef.localScale = new Vector3(_CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1, _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1, 1);
    }

    public bool IsPlayerControlled()
    {
        return _isControllingPlayer;
    }

    public bool IsAbilityLocationRequired()
    {
        return (_CurrentAbility != null);
    }

    public bool GetIsMoving()
    {
        return _isMoving;
    }

    public void LinkGame(Game game)
    {//This should never need to change. Disable the chance of it being called again.
        if (_game == null) _game = game;
    }

    public override bool NextTurn()
    {
        _isMoving = false;
        _ControlledObject.HideMoveRadius();



        if (_CurrentAbility != null)
        {
            if (_AbilityTarget.Item1 != null)
            {
                if (_CurrentAbility.AttemptActiveAbility(_AbilityTarget))
                {
                    _CurrentAbility = null;
                    Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();
                }
                _AbilityTarget.Item1 = null;
                _AbilityTarget.Item2 = null;
                return true;
            }
        }
        return false;
    }

    public override void SetTargetLocation((int, int) locationInput)
    {
        Board.instance.getBox(locationInput.Item1, locationInput.Item2).highlightBox(false);

        if (_AbilityTarget.Item1 != null && _AbilityTarget.Item2 != null)
        {
            Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();

            if (_CurrentAbility == null)
                worldManager.instance.GetPlayer().GetComponent<PlayerUnit>().SetDestination((locationInput.Item1, locationInput.Item2));
        }
        _AbilityTarget = locationInput;
    }

}
