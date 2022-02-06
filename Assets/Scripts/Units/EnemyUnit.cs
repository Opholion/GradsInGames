using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    protected bool IsDamaged = false;

    //Class is not particularly needed but it allows for potential future development while segmenting the
    //classes nicely while organizing the controller. 
    void Start()
    {
        UnitStart();
        _controller = GetComponent<EnemyController>();
        ShowMoveRadius();
        //If there's a custom enemy controller use it. Otherwise, make a basic one.
        if (_controller == null)
        {
            _controller = new EnemyController();
        }
    }

    protected override void OnDamage()
    {
        IsDamaged = true;
        base.OnDamage();
    }

    protected override void MoveUnit(int xy, int increment)
    {
        //Apply stun to enemy.
        if(!IsDamaged)
        base.MoveUnit(xy, increment);

        IsDamaged = false;
    }

    protected override void OnDeath()
    {
        if(Health < MaxHealth-(Unit.MIN_ADMIN_DAMAGE*1.15f))
        //always keep up enemy count. Killing enemies should not be rewarded beyond giving a temporary reprieve.
        worldManager.instance.trySpawnRandomEnemy(boardRef.getMap().GetUnitSpawnPos());
        base.OnDeath();
    }
}
