using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    protected AbilityData _AbilityData; //Database for prefab info
    protected Unit _LinkedUnit;
    protected BaseController _controllerOwner; //Reference to current Units controller
    protected bool _controllerIsPlayer = false;

    protected (int?, int?) _tempOwnerPos; //Last inputted target location.
    protected ParticleComponent particleComponent;
    //This will also need to be overwritten - It allows abiliities to have modifed ranges.

    [SerializeField] protected int _AttackRange;
    [SerializeField] protected int _AttackDamage;
    [SerializeField] protected LastActionIndicator.abilityTypes _abilityType;

    protected void Awake()
    {
        particleComponent = gameObject.GetComponent<ParticleComponent>();
        if (particleComponent == null)
            particleComponent = gameObject.AddComponent<ParticleComponent>();
    }


    public int GetRange()
    {
        return _AttackRange;
    }
    public void Setup(AbilityData dataRef, Unit ownerData, int AttackRange = 0, int AttackDamage = 0)
    {
        if (!IsSetup())
        {
            //Should only be set once,
            if(_LinkedUnit == null && dataRef.GetButtonRef() != null)
                dataRef.GetButtonRef().onClick.AddListener(ButtonActivate);


            _AbilityData = dataRef;
            _LinkedUnit = ownerData;
            _controllerOwner = ownerData.GetController();
            _controllerIsPlayer = (ownerData ==worldManager.instance.GetPlayer()); 
            //Should be more efficient to just make a boolean to check if t's the player rather than casting and seeing if null.

            if (AttackRange > 0)
            {
                _AttackRange = AttackRange;
            }
            if (AttackDamage > 0)
            {
                _AttackDamage = AttackDamage;
            }
            
        }
    }

    //Generic bool to make sure that the ability is linked to its database of info.
    protected bool IsSetup()
    {
        return (_AbilityData != null && _controllerOwner != null);
    }

    //This is the interface for the button, to start the process of informing the 
    //Controller that a target should be selected.
    public void ButtonActivate()
    {

        if (!Board.instance.isGameActive()) return;
        //If this is true then no linked unit has been made so this cannot be used.
        if (_LinkedUnit == null) return;

        //Needs to interact with the AI.
        LastActionIndicator.instance.ShowAbility(_abilityType);

        //Safety check inside function
         Setup(_AbilityData, _LinkedUnit);
       

            if (_controllerIsPlayer)
                ((PlayerController)_controllerOwner).SetActiveAbility(this);
            else
                _controllerOwner.SetActiveAbility(this);
    }


    public bool AttemptActiveAbility((int?, int?) targetPos)
    {



        _tempOwnerPos = _controllerOwner.GetCurrentGridPos();
        if (IsSetup() &&
            _tempOwnerPos.Item1 + _AttackRange >= targetPos.Item1 &&
            _tempOwnerPos.Item1 - _AttackRange <= targetPos.Item1 &&
            _tempOwnerPos.Item2 + _AttackRange >= targetPos.Item2 &&
            _tempOwnerPos.Item2 - _AttackRange <= targetPos.Item2)
        {
            _tempOwnerPos = targetPos;
            
            
            if(Board.instance.getBox(targetPos).heldUnit!= null) Board.instance.getBox(targetPos).heldUnit.DamageUnit(_AttackDamage);
            ActivateAbility();
            return true;
        }
        else
        {
            _tempOwnerPos = (null, null);
            return false;
        }
    }


    //This is the ability output and what will be overwritten when new abilities are made.
    protected virtual void ActivateAbility()
    {

    }

}
