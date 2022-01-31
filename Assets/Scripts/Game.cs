using UnityEngine;

public class Game : MonoBehaviour
{

    private Board _board;
    private UI _ui;
    private double _gameStartTime;
    private bool _gameInProgress;
    private bool isShowingAbilities;

  public bool GetGameState()
    {
        return (_gameInProgress);
    }
    public void OnClickedNewGame()
    {
        
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
    }

    public void OnClickedExit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    public void OnClickedReset()
    {
        
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
        if (false)
        {


            if (eventType == Board.Event.ClickedDanger && _ui != null)
            {
                _ui.HideGame();
                _ui.ShowResult(success: false);
            }

            if (eventType == Board.Event.Win && _ui != null)
            {
                _ui.HideGame();
                _ui.ShowResult(success: true);
            }
        }
        
    }
}
