using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    protected AbilityData _AbilityData; //Database for prefab info
    protected BaseController _controllerOwner; //Reference to current Units controller
    protected bool _controllerIsPlayer = false;

    protected (int?, int?) _tempOwnerPos; //Last inputted target location.
    protected ParticleComponent particleComponent;
    //This will also need to be overwritten - It allows abiliities to have modifed ranges.
    [SerializeField] protected int _AttackRange = 0;
    [SerializeField] protected float _AttackDamage = 0;
    [SerializeField] protected int _AttackDuration = 0;
    protected int _RemainingAttackDuration = 0;
    [SerializeField] protected LastActionIndicator.abilityTypes _abilityType;

    protected void Awake()
    {
        particleComponent = gameObject.GetComponent<ParticleComponent>();
        if (particleComponent == null)
            particleComponent = gameObject.AddComponent<ParticleComponent>();
    }

    private void FixedUpdate()
    {
        if (TurnManager.instance.GetTurn() > _AttackDuration)
        {

        }
    }
    public void Setup(AbilityData dataRef, BaseController ownerData)
    {
        if (!IsSetup())
        {
            _AbilityData = dataRef;
            _controllerOwner = ownerData;
            _controllerIsPlayer = ((PlayerController)_controllerOwner != null); //Should be more efficient to just make a boolean to check if t's the player rather than casting and seeing if null.

                dataRef.GetButtonRef().onClick.AddListener(ButtonActivate);         
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
        //Needs to interact with the AI.
        LastActionIndicator.instance.ShowAbility(_abilityType);

        if(_controllerIsPlayer)
            ((PlayerController)_controllerOwner).SetActiveAbility(this);
        else
            _controllerOwner.SetActiveAbility(this);
    }


    public bool AttemptActiveAbility((int?, int?) targetPos)
    {
        _tempOwnerPos = _controllerOwner.GetCurrentGridPos();
        if (IsSetup() &&
            _tempOwnerPos.Item1 + _AttackRange > targetPos.Item1 &&
            _tempOwnerPos.Item1 - _AttackRange < targetPos.Item1 &&
            _tempOwnerPos.Item2 + _AttackRange > targetPos.Item2 &&
            _tempOwnerPos.Item2 - _AttackRange < targetPos.Item2)
        {
            _tempOwnerPos = targetPos;
            _RemainingAttackDuration = _AttackDuration;
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
