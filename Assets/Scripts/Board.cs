using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using boxTransform = UnityEngine.Transform;


public class Board : MonoBehaviour
{
    public enum Event { ClickedBlank, ClickedNearDanger, ClickedDanger, Win };

    [SerializeField] private Box BoxPrefab;
    [SerializeField] public static int bWIDTH = 5;
    [SerializeField] public static int bHEIGHT = 5;
    public const int WALL_COST = 4;
    [SerializeField] private int NumberOfDangerousBoxes = 10;

    private Box[,] _grid;
    private boxTransform _rect;
    private Action<Event> _clickEvent;

    private Thread[] gridCreatorMesh;

    GameObject rowObj;
    boxTransform rowRect;
    boxTransform gridBoxTransform;


    #region board_Singleton

    public static Board instance;

    private void Awake()
    {
        instance = this;
        boxGeneration();
    }

    #endregion

    public void Setup(Action<Event> onClickEvent)
    {
        _clickEvent = onClickEvent;
        Clear();
    }

    public Box getBox(int x, int y)
    {
        if(x >= 0 && y >= 0 && x < bWIDTH && y < bHEIGHT)
        return _grid[x,y];

        //Out of bounds. Remove.
        return null;
    }
    public Box getBox((int, int) xy)
    {
        return getBox(xy.Item1, xy.Item2);
    }
    public Box getBox((int?, int?) xy)
    {
        if (xy.Item1 == null)
        {
            return getBox(0, 0);
        }
        return getBox((int)xy.Item1, (int)xy.Item2);
    }



    public void Clear()
    {
        for (int row = 0; row < bHEIGHT; ++row)
        {
            for (int column = 0; column < bWIDTH; ++column)
            {
                _grid[row, column].StandDown();
            }
        }
    }

    public void RechargeBoxes()
    {

        List<List<bool>> dangerList = new List<List<bool>>(bHEIGHT);

        for (int i = 0; i < bWIDTH; ++i)
        {
            dangerList.Add(new List<bool>(bWIDTH));
            for (int j = 0; j < bHEIGHT; ++j)
                dangerList[i].Add((i*bWIDTH)+j < NumberOfDangerousBoxes);
        }


       // dangerList.RandomShuffle();

        for (int row = 0; row < bHEIGHT; ++row)
        {
            for (int column = 0; column < bWIDTH; ++column)
            {
                _grid[row, column].Charge(CountDangerNearby(dangerList, row, column), dangerList[row][column], OnClickedBox);
            }
        }
    }

    //Noted to be implemented as a thread in the future.
    private void boxGeneration()
    {
        _grid = new Box[bWIDTH, bHEIGHT];
        _rect = transform as boxTransform;
        boxTransform boxRect = BoxPrefab.transform as boxTransform;

            _rect.lossyScale.Set(boxRect.lossyScale.x * bWIDTH, boxRect.lossyScale.y * bHEIGHT, 1);
        Vector3 startPosition = _rect.localPosition - (_rect.lossyScale * 0.5f) + (boxRect.lossyScale * 0.5f);
        startPosition.y *= -1.0f;
        for (int row = 0; row < bWIDTH; ++row)
        {
            rowObj = new GameObject(string.Format("Row{0}", row), typeof(boxTransform));
            rowRect = rowObj.transform as boxTransform;
            rowRect.SetParent(transform);
            rowRect.localPosition = new Vector2(_rect.localPosition.x, startPosition.y - (boxRect.lossyScale.y * row));
            rowRect.lossyScale.Set(boxRect.lossyScale.x * bWIDTH, (boxRect.lossyScale.y)* bHEIGHT, 100);
            rowRect.localScale = Vector3.one;

            
            for (int column = 0; column < bHEIGHT; ++column)
            {
                _grid[row, column] = Instantiate(BoxPrefab, rowObj.transform);
                _grid[row, column].Setup(row, column);
                 gridBoxTransform = _grid[row, column].transform as boxTransform;
                _grid[row, column].name = string.Format("Row{0}, Column{1}", row, column);
                //_grid[row, column].unHighlightBox();

                gridBoxTransform.localPosition = new Vector3(startPosition.x + (boxRect.lossyScale.x * column), 0.0f, .0f);
            }
        }
     
    }


    private int CountDangerNearby(List<List<bool>> danger, int xIndex, int yIndex)
    {
        int result = 0;


        if (!danger[xIndex][yIndex])
        {
            //Want to get every 'box' within 1 box of the current box
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    //If it is equal to the current box then ignore it.
                    if (!(x == 0 && y == 0))
                    {
                        int curX = _grid[xIndex,yIndex].ID[0] + x;
                        int curY = _grid[xIndex, yIndex].ID[1] + y;
                        bool active = curX >= 0 && curY >= 0 && curX < bWIDTH && curY < bHEIGHT;

                        if (active)
                        {
                            if(danger[curX][curY])
                            ++result;
                        }
                    }
                }
            }
        }

        return result;
    }

    private void OnClickedBox(Box box)
    {
        if (true)
            worldManager.instance.GetPlayer().GetComponent<PlayerUnit>().SetDestination((box.ID[0], box.ID[1]));
        else
        {
            Event clickEvent = Event.ClickedBlank;

            if (box.IsDangerous)
            {
                clickEvent = Event.ClickedDanger;
            }
            else if (box.DangerNearby > 0)
            {
                clickEvent = Event.ClickedNearDanger;
            }
            else
            {
                ClearNearbyBlanks(box);
            }

            if (false && CheckForWin())
            {
                clickEvent = Event.Win;
            }

            _clickEvent?.Invoke(clickEvent);
        }
    }

    private bool CheckForWin()
    {
        for (int row = 0; row < bWIDTH; ++row)
            for (int column = 0; column < bHEIGHT; ++column)
                if (!_grid[row, column].IsDangerous && _grid[row, column].IsActive)
                {
                    return false;
                }
        return true;
    }

    private void ClearNearbyBlanks(Box box)
    {
        RecursiveClearBlanks(box);
    }


    //Rewrote this function to no longer be recursive while adding another "IsScanned" variable to reduce the time required to process blocks. This makes it far more optimized when scanning larger chunks. 
    //Recursion is typically noted to be expensive so I attempted to unroll it.
    private void RecursiveClearBlanks(Box box)
    {
        //Allocate the max possible size for this so it will never need to find a new location for memory,
        List<Box> boxes = new List<Box>(bWIDTH*bHEIGHT);
        boxes.Add(box);
        int currPos = 0;
        boxes[0].IsScanned = true;

        while (boxes.Count > currPos)
        {
            if (!boxes[currPos].IsDangerous)
            {
                boxes[currPos].Reveal();

                if (boxes[currPos].DangerNearby == 0)
                {
                    //Want to get every 'box' within 1 box of the current box
                    for (int x = -1; x <= 1; ++x)
                    {
                        for (int y = -1; y <= 1; ++y)
                        {
                            int curX = boxes[currPos].ID[0] + x;
                            int curY = boxes[currPos].ID[1] + y;

                            
                            //It will make it slightly faster.
                            if (curX >= 0 && curY >= 0 && curX < bWIDTH && curY < bHEIGHT)
                            {
                                if (_grid[curX, curY].IsActive && !_grid[curX, curY].IsScanned)
                                {
                                    _grid[curX, curY].IsScanned = true;
                                    boxes.Add(_grid[curX, curY]);
                                }

                            }
                        }
                    }
                }
            }
            currPos++;
        }

        //Reset the list
        for (int i = 0; i < currPos; ++i)
        {
            boxes[i].IsScanned = false;
        }
    }
}
