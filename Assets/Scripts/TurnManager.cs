using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    #region TurnManager_Singleton

    public static TurnManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private int _turnCount = 0;


    public void ResetGame()
    {
        LastActionIndicator.instance.ShowEmpty();
        _turnCount = 0;
    }

    public void NextTurn()
    {
        _turnCount++;
        LastActionIndicator.instance.ShowEmpty();
        BroadcastMessage("TurnPass");
    }

    public int GetTurn()
    {
        return _turnCount;
    }
}
