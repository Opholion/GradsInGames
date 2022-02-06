using UnityEngine;

public class Game : MonoBehaviour
{

    private Board _board;
    private UI _ui;
    private double _gameStartTime;
    private bool _gameInProgress;
    private bool isShowingAbilities;

    int CollectibleCount;
    [SerializeField] int CollectibleLimit;

    public bool GetGameState()
    {
        return (_gameInProgress);
    }
    public void OnClickedNewGame()
    {
        SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.Gameplay);

        isShowingAbilities = false;
        if (_board != null)
        {
            _board.RechargeBoxes();
        }

        if (_ui != null)
        {
            _ui.HideMenu();
            _ui.ShowGame();
        }
        _gameInProgress = true;
        PlayerController.instance.UpdateGameState();
        _gameStartTime = Time.realtimeSinceStartupAsDouble;



        //Entity spawning
        Unit unitRef = null;


        for (int i = 0; i < 5 && unitRef == null; ++i)
        unitRef = worldManager.instance.trySpawnPlayer(_board.getMap().GetUnitSpawnPos());
        if (unitRef == null)
            OnClickedExit();

        int indexLimit = _board.getMap().GetEnemySpawnCount();

        unitRef = null;
        for (int i = 0; i < indexLimit; ++i)
        {
            //Chance units may spawn in same location and not spawn, resulting this. This is an attempt to force it. 
            for (int j = 0; j < 5 && unitRef == null; ++j)
            unitRef = worldManager.instance.trySpawnRandomEnemy(_board.getMap().GetUnitSpawnPos());
            unitRef = null;
        }

        //Should consider putting this in a function; Pass in function to repeat until not null. 
        for (int i = 0; i < CollectibleLimit; ++i)
        {
            for (int j = 0; j < 5 && unitRef == null; ++j)
            unitRef =  worldManager.instance.trySpawnRandomCollectible(_board.getMap().GetUnitSpawnPos());
            unitRef = null;
        }
        _ui.ShowCollectibles(0, CollectibleLimit);
    }

    public void OnClickedExit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    public void OnClickedReset()
    {
        SoundtrackManager.instance.SetState(SoundtrackManager.SoundtrackTypes.MainMenu);
        
        if (_board != null)
        {
            _board.Clear();
        }

        if (_ui != null)
        {
            _ui.HideResult();
            _ui.ShowMenu();
        }
        _gameInProgress = false;
        PlayerController.instance.UpdateGameState();
    }
    public void ToggleAbilitiesUI()
    {
        if (_gameInProgress)
        {
            if (!isShowingAbilities)
                _ui.ShowAbilities();
            else
                _ui.HideAbilities();

            isShowingAbilities = !isShowingAbilities;
        }
    }

    //From my understanding, the game is the overall manager and should not directly be interfaced or linked to any component outside of the manangers. 
    //This means the board needs to notify this; as another class solely dedicated to counting collectibles seems to be unneeded and expensive. 
    public bool CollectedCollectible()
    {
        //If true; you ahve won. Gj
        ++CollectibleCount;
        _ui.ShowCollectibles( CollectibleCount, CollectibleLimit);
        if (CollectibleCount >= CollectibleLimit) 
            return true;
        return false;
    }

    private void Start()
    {
        _ui = transform.parent.GetComponentInChildren<UI>();
        _board = worldManager.instance.GetBoard();
        PlayerController.instance.LinkGame(this);
        _gameInProgress = false;
        setupUI();
    }

    private void setupUI()
    {
        if (_board != null)
        {
            _board.Setup(BoardEvent);
        }
        else
        {
            _board = worldManager.instance.GetBoard();
            _board.Setup(BoardEvent);
        }

        if (_ui != null)
        {
            _ui.ShowMenu();
        }
        else
        {
            _ui = transform.parent.GetComponentInChildren<UI>();
            _ui.ShowMenu();
        }
    }
    private void Update()
    {
        if (_ui != null)
        {
            _ui.UpdateTimer(_gameInProgress ? Time.realtimeSinceStartupAsDouble - _gameStartTime : 0.0);
        }
        else
        {
            setupUI();
        }
    }

    private void BoardEvent(Board.Event eventType)
    {
        if(eventType == Board.Event.ClickedTarget || eventType == Board.Event.ClickedPosition)
        TurnManager.instance.EnableNextTurn();        
    }
}
