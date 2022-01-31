using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    private Game _game;
    private AttackMenu _menu;

    private bool _activeGameState = false;
    private bool _isControllingPlayer = true;


    #region PlayerController_Singleton

    public static PlayerController instance;


    private void Awake()
    {
        instance = this;
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
       
        if (_activeGameState && _ControlledObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Assuming both equal false on start, this can assume making it equal not itself will work adequetly.
                _game.ToggleAbilitiesUI();
                _isControllingPlayer = !_isControllingPlayer;
                LastActionIndicator.instance.ResetAbilityOutput();
                _CurrentAbility = null; //Allows the user to cancel their choice.

                if (_AbilityTarget != (null, null))
                    Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();

                _AbilityTarget = (null, null);

            }

            if (_isControllingPlayer)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    _ControlledObject.ControllerUpdate((0, -1));
                    LastActionIndicator.instance.MovementDirection(LastActionIndicator.directions.north);
                }
                else
                if (Input.GetKeyDown(KeyCode.D))
                {
                    _ControlledObject.ControllerUpdate((1, 1));
                    LastActionIndicator.instance.MovementDirection(LastActionIndicator.directions.east);
                }
                else
                if (Input.GetKeyDown(KeyCode.S))
                {
                    _ControlledObject.ControllerUpdate((0, 1));
                    LastActionIndicator.instance.MovementDirection(LastActionIndicator.directions.south);
                }
                else
                if (Input.GetKeyDown(KeyCode.A))
                {
                    _ControlledObject.ControllerUpdate((1, -1));
                    LastActionIndicator.instance.MovementDirection(LastActionIndicator.directions.west);
                }
            }
            else
            {

            }
        }
    }


    //The below should rarely be used so they can be done without excessive optimization.

    //As the controller manages what the player can do, it should know when the game is inactive.
    public void UpdateGameState()
    {
        _activeGameState = _game.GetGameState();
    }

    public override void SetActiveAbility(BaseAbility ability)
    {
        base.SetActiveAbility(ability);

        _game.ToggleAbilitiesUI();
        _isControllingPlayer = !_isControllingPlayer;
    }

    public bool IsPlayerControlled()
    {
        return _isControllingPlayer;
    }

    public bool IsAbilityLocationRequired()
    {
        return (_CurrentAbility != null);
    }




    public void LinkGame(Game game)
    {//This should never need to change. Disable the chance of it being called again.
        if (_game == null) _game = game;
    }
}
