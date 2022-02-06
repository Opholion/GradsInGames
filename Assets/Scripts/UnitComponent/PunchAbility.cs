using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAbility : BaseAbility
{
    private void Awake()
    {
        _AttackRange = 5;
        base.Awake();
    }

    protected override void ActivateAbility()
    {
        particleComponent.StartParticleEffect(Board.instance.getBox(_tempOwnerPos).transform, Board.instance.getBox(_controllerOwner.GetCurrentGridPos()).transform);
    }



}