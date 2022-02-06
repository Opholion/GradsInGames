using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{



    void Start()
    {
        UnitStart();
        PlayerController.instance.LinkToUnit(this);

    }

    protected override void OnDeath()
    {



        PlayerController.instance.ResetGame();

        base.OnDeath();
    }

    protected override void MoveUnit(int xy, int increment)
    {
        //Camera is linked to this unit so it needs to move in real-time.
      

        base.MoveUnit(xy, increment);

        if (pathList != null)
            for (int i = 0; i < pathList.Count; ++i)
            {
                Board.instance.getBox(pathList[i].Item1, pathList[i].Item2).highlightBox(true);
            }

        Board.instance.RecursiveClearBlanks();
        CameraMovement.instance.forcePositionUpdate();
    }


    public override void SetDestination((int, int) target)
    {
        base.SetDestination(target);

        if (pathList != null)
            for (int i = 0; i < pathList.Count; ++i)
            {
                Board.instance.getBox(pathList[i].Item1, pathList[i].Item2).highlightBox(true);
            }
    }

}


/*    
 *    Original purpose of this function was to only activate sections within a set distance but it did not help efficiency
 *    
 *    
 *    //Find current place in array and calculate the next row in the given direction.
    private void activateRow(bool isActive, int xy, int increment)
    {
        for (int i = boardPos[xy] - worldManager.GRID_EDGE_DIST; i <= boardPos[xy] + worldManager.GRID_EDGE_DIST; ++i)
            if (xy == 0)
            {
                //x value
                Box box = boardRef.getBox(i, boardPos[1] + (worldManager.GRID_EDGE_DIST * increment));
                if (box != null)
                    box.gameObject.SetActive(isActive);
            }
            else
            {
                //y value
                Box box = boardRef.getBox(boardPos[0] + (worldManager.GRID_EDGE_DIST * increment), i);
                if (box != null)
                    box.gameObject.SetActive(isActive);
            }
    }
 */