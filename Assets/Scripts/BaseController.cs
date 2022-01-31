using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Unit _ControlledObject;
    protected BaseAbility _CurrentAbility;
    protected (int?, int?) _AbilityTarget;
    public virtual void SetActiveAbility(BaseAbility ability)
    {
        _CurrentAbility = ability;
    }

    public void SetTargetLocation((int, int) locationInput)
    {
        Board.instance.getBox(locationInput.Item1, locationInput.Item2).highlightBox(false);

        if (_AbilityTarget.Item1 != null && _AbilityTarget.Item2 != null)
        Board.instance.getBox((int)_AbilityTarget.Item1, (int)_AbilityTarget.Item2).unHighlightBox();
        _AbilityTarget = locationInput;
    }

    public virtual void LinkPlayer(Unit Unit)
    {//This needs to update on a regular basis, assuming a new player can be created.
        if (Unit != null)
            _ControlledObject = Unit;
        else
            LinkPlayer();

        _ControlledObject.SetController(this);
    }

    public void LinkPlayer()
    {
        _ControlledObject = gameObject.GetComponent<Unit>();
        
    }
    public (int, int) GetCurrentGridPos()
    {
        return _ControlledObject.GetGridPos();
    }

    //If an ability is selected then check for an activated box. If it's valid then create a particle effect
    //At that point. 
    public bool NextTurn()
    {
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
}
