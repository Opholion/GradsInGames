using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleUnit : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        UnitStart();

    }

    protected override void OnDeath() 
    {
        boardRef.DestroyCollectible();
        base.OnDeath();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    //Sicne this is a collectible, it doesn't actually do anything. 
    public override void TurnPass()
    {

    }
}
