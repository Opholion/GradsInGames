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

    public virtual void SetTargetLocation((int, int) locationInput)
    {
        if (_AbilityTarget.Item1 != null && _AbilityTarget.Item2 != null)
        {
            if(_CurrentAbility == null)
                _ControlledObject.SetDestination((locationInput.Item1, locationInput.Item2));
        }
        _AbilityTarget = locationInput;
    }


    public bool IsControlledObject(Unit unit)
    {
        return (_ControlledObject == unit);
    }
    public void LinkToUnit()
    {
        _ControlledObject = gameObject.GetComponent<Unit>();
        
    }
    public virtual void LinkToUnit(Unit Unit)
    {//This needs to update on a regular basis, assuming a new player can be created.

        if (Unit.SetController(this))
        {
            _ControlledObject = Unit;
        }
    }

    public (int, int) GetCurrentGridPos()
    {
        return _ControlledObject.GetGridPos();
    }


    //If an ability is selected then check for an activated box. If it's valid then create a particle effect
    //At that point. 
    public virtual bool NextTurn()
    {
        if (_CurrentAbility != null)
        {
            if (_AbilityTarget.Item1 != null)
            {
                if (_CurrentAbility.AttemptActiveAbility(_AbilityTarget))
                {
                    _CurrentAbility = null;
                }
                _AbilityTarget.Item1 = null;
                _AbilityTarget.Item2 = null;
                return true;
            }
        }
        return false;
    }


}
