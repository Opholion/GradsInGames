using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//This is A* - research that to learn more. 
public class MovementComponent : MonoBehaviour
{
    struct Tile
    {
        public (int, int) Position;
        public int Cost;

        public Tile((int,int) pos, int cost)
        {
            Position = pos;
            Cost = cost;
        }
    }
    struct Node
    {
        public List<Tile> previous;
        public Tile current;

        public Node(List<Tile> history, (int, int) position, int cost)
        {
            previous = history;
            current = new Tile(position, cost);
        }
    }
    List<(int, int)> getPathOutput;
    List<Node> openList;
    List<Node> closedList;


    int tempValue;
    Node tempNode;
    Node currentNodeInList;
    (int, int) tempTuple;
    public List<(int, int)> getPath((int, int) start, (int, int) destination)
    {
        if(openList == null)
            openList = new List<Node>();
        if (closedList == null)
            closedList = new List<Node>();

        openList.Clear();
     
        closedList.Clear();
        getPathOutput = new List<(int, int)>();


        //In theory this should path everything from the starting position, without repeating a taken path. Eventually it will run out of options
        openList.Add(new Node(new List<Tile>(), start, 0));
        while (closedList.Count < 1)
        {
            //Cannot check the "Back" of the stack as new objects will be added within the loop. 
            currentNodeInList = openList[0];
            openList.RemoveAt(0);

            //Check each neighbour as a valid path. -1.-1 is the top left. 1,1 is the bottom right. 0,0 is the current position.
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (!(y == 0 && x == y)) //Don't add current position. Unnecesary expense. 
                    {
                        tempTuple = currentNodeInList.current.Position;
                        tempTuple.Item1 += x;
                        tempTuple.Item2 += y;

                        tempValue = isValid(tempTuple);

                        //If the nearbye node is not invalid/a_node, attempt to add it to the openlist.
                        if (tempValue < Board.WALL_COST)
                        {
                            //Create a new node with the new position and cost.
                                tempNode = new Node(new List<Tile>(), (currentNodeInList.current.Position.Item1 + x, currentNodeInList.current.Position.Item2 + y), currentNodeInList.current.Cost + 2 + tempValue);

                            for (int node =0; currentNodeInList.previous.Count > node; ++node)
                                tempNode.previous.Add(currentNodeInList.previous[node]);
                            tempNode.previous.Add(currentNodeInList.current);

                            if (Mathf.Abs(tempNode.current.Position.Item1 - destination.Item1) <
                                Mathf.Abs(currentNodeInList.current.Position.Item1 - destination.Item1) )
                                tempNode.current.Cost -= 2;

                            if(Mathf.Abs(tempNode.current.Position.Item2 - destination.Item2) < Mathf.Abs(currentNodeInList.current.Position.Item2 - destination.Item2))
                                tempNode.current.Cost -= 2;

                            if (x == 0) tempNode.current.Cost -= 1;
                            else if(y== 0) tempNode.current.Cost -= 1; 

                            //If it doesn't go back on itself and hasn't repeated this path somewhere. 
                            //if (tempNode.previous.Count == 0 || tempNode.previous[tempNode.previous.Count-1].Position != tempNode.current.Position)
                            {



                                if (tempNode.previous.Count <= 1  || tempNode.current.Position != tempNode.previous[tempNode.previous.Count-2].Position)
                                {
                                    if (tempNode.current.Position == destination)
                                    {
                                        tempNode.previous.Add(tempNode.current);
                                        closedList.Add(tempNode);
                                    }
                                    else
                                    {
                                       // Board.instance.getBox(tempNode.current.Position.Item1, tempNode.current.Position.Item2).alert();

                                        
                    
                                        openList.Add(tempNode);

                                        int i = openList.Count - 1;
                                        while (i != 0 && openList[i].current.Cost < openList[i - 1].current.Cost)
                                        {
                                            tempNode = openList[i - 1];
                                            openList[i - 1] = openList[i];
                                            openList[i] = tempNode;
                                            --i;
                                        }
                                    }
                                }
                                else if (tempNode.previous.Count > 0)
                                    tempNode.previous.Add(new Tile(tempTuple, tempValue));
                            }
                        }
                    }
                }
            }

            if (openList.Count > 5000)
            {
                break;
            }
        }

    
        foreach (Tile tile in closedList[0].previous)
        {
            getPathOutput.Add(tile.Position);
            
        }


        return getPathOutput;
    }


    int isValid((int,int) position)
    {
        if (position.Item1 >= 0 && position.Item2 >= 0 &&
            position.Item1 < Board.bWIDTH && position.Item2 < Board.bHEIGHT)
        {
           return Board.instance.getBox(position.Item1, position.Item2).getTravelCost();
        }
        return Board.WALL_COST;
    }

    void isInList(Tile comparison)
    {
        //Check if the tile found has been reached before and if the previous method was more optimal.
        int i = 0;
        foreach (Node element in openList)
        {
            foreach (Tile tile in element.previous)
            {
                if (tile.Position == comparison.Position && tile.Cost > comparison.Cost) openList.RemoveAt(i);
            }
            ++i;
        }
    }
}
