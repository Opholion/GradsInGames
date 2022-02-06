using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    #region TurnManager_Singleton

    public static TurnManager instance;
    [SerializeField] SoundtrackManager SoundTrackTurns;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private int _TurnCount = 0;
    private bool _CanTurnEnd = false;

    //I don't believe this is currently used as its original purpose, to disable turn progression until an action is taken, was thought to be not useful. 
    //This was retained, however, to allow animations or cinematics to be played - If this ever reaches that point. 
    public void EnableNextTurn()
    {
        _CanTurnEnd = true;
    }

    public void ResetGame()
    {
        _CanTurnEnd = false;
        LastActionIndicator.instance.ShowEmpty();
        _TurnCount = 0;
    }

    public void NextTurn()
    {
        if (!Board.instance.isGameActive()) return;
     //   if (_CanTurnEnd) return;
        _TurnCount++;
        LastActionIndicator.instance.ShowEmpty();
        BroadcastMessage("TurnPass");
        _CanTurnEnd = false;

        if (SoundTrackTurns)
        {
            SoundTrackTurns.UpdateTurns();
        }
    }

    public int GetTurn()
    {
        return _TurnCount;
    }
}
