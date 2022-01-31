using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    private void Awake()
    {
        PlayerController.instance.LinkPlayer(this);
    }
    //Find current place in array and calculate the next row in the given direction.
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

    protected override void MoveUnit(int xy, int increment)
    {
        base.MoveUnit(xy, increment);
        CameraMovement.instance.forcePositionUpdate();
    }
    // Update is called once per frame



}
/*
 *for (int i = 0; i < Board.bHEIGHT; ++i)
                for (int j = 0; j < Board.bWIDTH; ++j)
                    Board.instance.getBox(i, j).calm();
*/
