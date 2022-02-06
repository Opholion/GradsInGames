using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using boxTransform = UnityEngine.Transform;


public class Board : MonoBehaviour
{
    public enum Event { ClickedPosition, ClickedTarget, ClickedNewRoom, Win };

    [SerializeField] Game gameRef;

    [SerializeField] private MapFile[] MapLoadouts;
    MapFile _ChosenMap;
    [SerializeField] private Box BoxPrefab;
    [SerializeField] private int _bWidth = 10; //Data now relies on map
    [SerializeField] private int _bHeight = 10;
    public const int WALL_COST = 4;
    //[SerializeField] private int NumberOfDangerousBoxes = 10;

    private Box[,] _grid;
    private boxTransform _rect;
    private Action<Event> _clickEvent;


    GameObject rowObj;
    boxTransform rowRect;
    boxTransform gridBoxTransform;

    bool WinConditionPass = false;

    #region board_Singleton

    public static Board instance;

    private void Awake()
    {

        instance = this;
        boxGeneration();
    }

    #endregion

    public bool isGameActive()
    {
        return gameRef.GetGameState();
    }

    public (int, int) GetBoardDimensions()
    {
        return (_bWidth, _bHeight);
    }

    public void Setup(Action<Event> onClickEvent)
    {
        _clickEvent = onClickEvent;
        Clear();
    }

    public void LoadMap()
    {
        if (_ChosenMap == null) _ChosenMap = MapLoadouts[UnityEngine.Random.Range(0, MapLoadouts.Length)];

        int[,] mapHolder = _ChosenMap.GetMap();
        for (int i = 0; i < _bWidth; ++i)
        {
            for (int j = 0; j < _bHeight; ++j)
            {
                //Hard code it so any "Unique" tile is a wall. Leaves room for future development but with this short deadline this is fine. 
                _grid[i, j].Charge((mapHolder[i, j] == (int)MapFile.MapIDs.door), (mapHolder[i, j] == (int)MapFile.MapIDs.wall), OnClickedBox, _ChosenMap.GetLinkedZModifier(MapFile.MapIDs.wall), _ChosenMap.GetLinkedTileColor(MapFile.MapIDs.wall), _ChosenMap.GetLinkedTileColor(mapHolder[i, j]), _ChosenMap.GetLinkedZModifier(mapHolder[i, j]));
            }
        }
    }
    public MapFile getMap()
    {
        return _ChosenMap;
    }

    public Box getBox(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _bWidth && y < _bHeight)
            return _grid[x, y];

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

    public bool IsBoxEmpty((int, int) index)
    {
        return !(_grid[index.Item1, index.Item2].IsHoldingUnit());
    }

    public bool IsBoxEmpty(int i, int j)
    {
        return !(_grid[i, j].IsHoldingUnit());
    }


    public void Clear()
    {
        for (int row = 0; row < _bHeight; ++row)
        {
            for (int column = 0; column < _bWidth; ++column)
            {
                _grid[row, column].StandDown();
            }
        }
    }

    public void RechargeBoxes()
    {
        //Originally this code was to be expanded but due to time limitations it was instead decided to load in maps.
        if (MapLoadouts.Length > 0)
        {
            LoadMap();
        }
        else //If there are no maps then generate an empty world. 
        {
            List<List<bool>> dangerList = new List<List<bool>>(_bHeight);

            for (int i = 0; i < _bWidth; ++i)
            {
                dangerList.Add(new List<bool>(_bWidth));
                for (int j = 0; j < _bHeight; ++j)
                    dangerList[i].Add(false);
            }

            //Random shuffle hsa been moved into map generation, to work as spawning. 
            // dangerList.RandomShuffle();

            for (int row = 0; row < _bHeight; ++row)
            {
                for (int column = 0; column < _bWidth; ++column)
                {
                    //CountDangerNearby(dangerList, row, column)b
                    _grid[row, column].Charge(false, false, OnClickedBox, 0.125f, Color.black);
                }
            }
        }
    }

    //Set up each box, and the map as a whole. 
    private void boxGeneration()
    {
        if (_ChosenMap == null) _ChosenMap = MapLoadouts[UnityEngine.Random.Range(0, MapLoadouts.Length)];
        _bWidth = _ChosenMap.GetWidthANDHeight().Item1;
        _bHeight = _ChosenMap.GetWidthANDHeight().Item2;

        _grid = new Box[_bWidth, _bHeight];
        _rect = transform as boxTransform;
        boxTransform boxRect = BoxPrefab.transform as boxTransform;

        _rect.lossyScale.Set(boxRect.lossyScale.x * _bWidth, boxRect.lossyScale.y * _bHeight, 1);
        Vector3 startPosition = _rect.localPosition - (_rect.lossyScale * 0.5f) + (boxRect.lossyScale * 0.5f);
        startPosition.y *= -1.0f;
        for (int row = 0; row < _bWidth; ++row)
        {
            rowObj = new GameObject(string.Format("Row{0}", row));
            rowRect = rowObj.transform as boxTransform;
            rowRect.SetParent(transform);
            rowRect.localPosition = new Vector2(_rect.localPosition.x, startPosition.y - (boxRect.lossyScale.y * row));
            rowRect.lossyScale.Set(boxRect.lossyScale.x * _bWidth, (boxRect.lossyScale.y) * _bHeight, 100);
            rowRect.localScale = Vector3.one;


            for (int column = 0; column < _bHeight; ++column)
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




    private void OnClickedBox(Box box)
    {
        if (PlayerController.instance.GetIsMoving() || PlayerController.instance.IsAbilityLocationRequired())
        {
            PlayerController.instance.SetTargetLocation((box.ID[0], box.ID[1]));

            Event clickEvent = Event.ClickedPosition;

            if (box.IsHoldingUnit())
            {
                clickEvent = Event.ClickedTarget;
            }
            else if (box.IsDoorway)
            {
                clickEvent = Event.ClickedNewRoom;
            }
            else
            if (CheckForWin())
            {
                clickEvent = Event.Win;
            }

            _clickEvent?.Invoke(clickEvent);
        }
    }

    private bool CheckForWin()
    {
        if (!gameRef)
            return false;

        return WinConditionPass;
    }

    public void DestroyCollectible()
    {
        WinConditionPass = gameRef.CollectedCollectible();
        if(CheckForWin())
            PlayerController.instance.ResetGame();
    }


    //Rewrote this function to no longer be recursive while adding another "IsScanned" variable to reduce the time required to process blocks. This makes it far more optimized when scanning larger chunks. 
    //Recursion is typically noted to be expensive so I attempted to unroll it.
    public void RecursiveClearBlanks()
    {
        //Allocate the max possible size for this so it will never need to find a new location for memory,
        List<Box> boxes = new List<Box>(_bWidth * _bHeight);
        boxes.Add(_grid[worldManager.instance.GetPlayer().GetGridPos().Item1, worldManager.instance.GetPlayer().GetGridPos().Item2]);
        int currPos = 0;

        if (boxes[0].IsScanned && !boxes[0].IsDoorway) return;

        boxes[0].IsScanned = true;

        while (boxes.Count > currPos)
        {

            boxes[currPos].Reveal();

            if (!boxes[currPos].GetIsDoor() ||
                boxes[currPos].ID[0] == worldManager.instance.GetPlayer().GetGridPos().Item1 &&
                boxes[currPos].ID[1] == worldManager.instance.GetPlayer().GetGridPos().Item2)
                //Want to get every 'box' within 1 box of the current box
                for (int x = -1; x <= 1; ++x)
                {
                    for (int y = -1; y <= 1; ++y)
                    {
                        int curX = boxes[currPos].ID[0] + x;
                        int curY = boxes[currPos].ID[1] + y;

                        if (curX >= 0 && curY >= 0 && curX < _bWidth && curY < _bHeight)
                        {

                            if (_grid[curX, curY].getTravelCost() == Board.WALL_COST && !_grid[curX, curY].IsHoldingUnit())
                            {
                                _grid[curX, curY].Reveal();
                            }
                            else
                                if (!_grid[curX, curY].IsScanned)
                            {
                                _grid[curX, curY].IsScanned = true;
                                boxes.Add(_grid[curX, curY]);
                            }


                        }
                    }
                }
            currPos++;
        }

    }
}


