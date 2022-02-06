using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFile : MonoBehaviour
{

    public enum MapIDs
    {
        floor = 0,
        wall = 1,
        door = 2
    }

    [SerializeField] private int _Width;
    [SerializeField] private int _Height;
    [SerializeField] int EnemySpawnCount;
    //Easier to copy and paste a file in here. Code was written with potential errors in mind and will only accept numbers. Copying and pasting the below, from the "Map = " to where the values of i and j are set should not incur any errors. 
    [SerializeField]
    private string map =
        "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1," +
        "1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,2,2,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1," +
        "1,0,0,0,0,0,0,1,0,0,0,0,0,2,0,0,0,0,0,0,0,2,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,1,1,0,0,0,0,0,2,0,0,0,0,0,0,0,1,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,2,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,2,2,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,2,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,1,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,1,1,1,1," +
        "1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,2,2,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,0,0,0,0,1,1,2,2,1,2,2,1,1," +
        "1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,2,0,0,0,1,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,2,0,0,0,1,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,0,1,0,0,0,2,0,0,0,1," +
        "1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,0,1,0,0,0,2,0,0,0,1," +
        "1,1,1,2,2,2,1,1,1,2,2,1,1,1,1,1,1,1,2,2,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1," +
        "1,0,0,0,0,0,0,0,1,0,0,1,1,1,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,2,0,0,2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,2,0,0,2,0,0,0,0,0,0,0,0,2,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,2,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,2,2,1,0,0,0,0,0,0,0,1," +
        "1,1,1,2,2,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,1,2,2,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1," +
        "1,0,0,0,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1," +
        "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,";


    //1 is always a wall.

    int[,] convertedMap;

    //Could automate adding additional types of spawn positions but, besides pickups and maybe obstacles (I.e. Locking off sections randomly to make the world more interesting) there isnt' much variation needed for this. 
    List<(int, int)> UnitSpawnPositions;

    [SerializeField] private Color[] _ColorTheme;
    [SerializeField] private float[] _ZAxisModifier;

    public int[,] GetMap()
    {
        //A converted map is needed to find unique tiles (i.e. player spawns)
        if (convertedMap != null) return convertedMap;

        if (Random.Range(0, 10) > 5)
            MapRandomization(0, 1);
        else
            MapRandomization(map.Length - 1, -1);

        return convertedMap;
    }



    //Further rnadomization is possible but in this scenario, writing a basic "Flip" seems to be fine.l 
    private void MapRandomization(int startVal,int iterator)
    {
        int[,] output = new int[_Width, _Height];

        int i, j;
        i = 0;
        j = 0;

        //Not sure on the best approach for this but attempting randomization.
        

        for (int Index = startVal; (Index < map.Length && Index > -1); Index += iterator)
        {
            if (Index < 0) break;

            if (char.IsNumber(map[Index]))
            {
                //Multiply "I" by the max value of "J" to get the amounted layers.
                //Assuming this cannot hold an invalid valueoutput[i, j] =
                output[i, j] = int.Parse(map[Index].ToString());
                ++i;
                if (i >= _Width)
                {
                    ++j;
                    i = 0;
                }
                if (j >= _Height) break; //Just rmeove the rest of the map; no need to break anything.
            }

        }
        convertedMap = output;
    }

    public (int, int) GetUnitSpawnPos()
    {
        int index;

        if (convertedMap == null) GetMap();
        if (UnitSpawnPositions != null)
        {
            if (UnitSpawnPositions.Count == 0) return (0, 0);
            index = Random.Range(0, UnitSpawnPositions.Count);
            return UnitSpawnPositions[index];
        }

        UnitSpawnPositions = new List<(int, int)>();



        int SpawnCount = UnityEngine.Random.Range(2, _Width * _Height / 10);
        if (SpawnCount < 2) SpawnCount = 2;

        (int, int) tempIndex;
        while (UnitSpawnPositions.Count < SpawnCount)
        {
            tempIndex.Item1 = UnityEngine.Random.Range(0, _Width);
            tempIndex.Item2 = UnityEngine.Random.Range(0, _Height);

            if (convertedMap[tempIndex.Item1, tempIndex.Item2] == (int)MapIDs.floor) UnitSpawnPositions.Add(tempIndex);
        }


        if (UnitSpawnPositions.Count == 0) return (0, 0);
        index = Random.Range(0, UnitSpawnPositions.Count);
        return UnitSpawnPositions[index];
    }


    public int GetEnemySpawnCount()
    {
        return EnemySpawnCount;
    }


    public Color GetLinkedTileColor(int index)
    {
        if (index < 0 || index >= _ColorTheme.Length)
            return _ColorTheme[0];
        return _ColorTheme[index];
    }
    public Color GetLinkedTileColor(MapIDs index)
    {
        return _ColorTheme[(int)index];
    }
    public float GetLinkedZModifier(int index)
    {
        if (index < 0 || index >= _ZAxisModifier.Length)
            return _ZAxisModifier[0];
        return _ZAxisModifier[index];
    }
    public float GetLinkedZModifier(MapIDs index)
    {
        return _ZAxisModifier[(int)index];
    }

    public (int, int) GetWidthANDHeight()
    {
        return (_Width, _Height);
    }
}
