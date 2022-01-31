using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Box : MonoBehaviour
{
    [SerializeField] protected Color[] DangerColors = new Color[8];

                 
    [SerializeField] protected Image HighlightedImage;
    [SerializeField] protected Image BackgroundImage;
                
    [SerializeField] protected Sprite UserSelectedIcon;
    [SerializeField] protected Sprite FloorIcon;
    [SerializeField] protected Sprite WallIcon;
                   
    [SerializeField] protected bool isOptimized = false;

    private TMP_Text _textDisplay;
    private Button _button;
    private Action<Box> _changeCallback;
   
    //Values are X/Y  - Could not use a Vector2 as that uses a float.
    public int[] ID { get; private set; }

    public int DangerNearby { get; private set; }
    public bool IsDangerous { get; private set; }
    public bool IsActive { get { return _button != null && _button.interactable; } }
    public bool IsScanned = false;

    int travelCost = 0;
    public int getTravelCost()
    {
        return travelCost;
    }

    //One thing I noticed when attempting to optimized my code was that Unity struggled with creating larger groups of entities. While I might have been doing this wrong, instead of researching it further, I decided to make my "Box.cs" act as a viewport manager
    //Essentially voxelizing my viewport. 
    public void setToWall()
    {
        travelCost = Board.WALL_COST;
        BackgroundImage.sprite = WallIcon;
    }
    public void setToFloor()
    {
        travelCost = 0;
        BackgroundImage.sprite = FloorIcon;
    }


    //This is mostly used to allow the player to hold a better understanding of what they're doing. Making the Turn by turn process much more fluent on the player side.  

    //Need to be able to disable the boxes after highlighting them.
    public Box highlightBox(bool isPathing)
    {
        HighlightedImage.enabled = true;

        HighlightedImage.sprite = UserSelectedIcon;

        if (isPathing)
            HighlightedImage.color = Color.cyan;
        else
            HighlightedImage.color = Color.magenta;

        return this;
    }

    public void unHighlightBox()
    {
        HighlightedImage.enabled = false;
    }

    public void Setup(int row, int column)
    {
        ID = new int[2] { row, column };
    }

    public void Charge(int dangerNearby, bool danger, Action<Box> onChange)
    {
        _changeCallback = onChange;
        DangerNearby = dangerNearby;
        IsDangerous = danger;
        ResetState();
    }               

    public void Reveal()
    {
        if (_button != null)
        {
            _button.interactable = false;
        }

        if (_textDisplay != null)
        {
            _textDisplay.enabled = true;
        }
    }

    public void StandDown()
    {
        if (_button != null)
        {
            _button.interactable = false;
        }

        if (HighlightedImage != null)
        {
            HighlightedImage.enabled = false;
        }

        if (_textDisplay != null)
        {
            _textDisplay.enabled = false;
        }
    }
  //  public void alert()
  //  {
  //      HighlightedImage.enabled = true; 
  //  }
    public void calm()
    {
        HighlightedImage.enabled = false; _button.interactable = true;
    }

    public void OnClick()
    {
        //While this is likely to be inefficient, this should only happen rarely and so can be afforded the extra costs in time.
        if (!PlayerController.instance.IsAbilityLocationRequired())
        {
            /*
            if (_button != null)
            {
                _button.interactable = false;
            }

            if (IsDangerous && HighlightedImage != null)
            {
                HighlightedImage.enabled = true;
            }
            else if (_textDisplay != null)
            {
                _textDisplay.enabled = true;
            }
            */
            PlayerController.instance.SetTargetLocation((ID[0], ID[1]));
            _changeCallback?.Invoke(this);
        }
        else
        {
            PlayerController.instance.SetTargetLocation((ID[0], ID[1]));
        }
    }

    private void Awake()
    {
        _textDisplay = GetComponentInChildren<TMP_Text>(true);
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnClick);

        ResetState();
        //transform.Rotate(0, 180, 0, Space.World);
      
        if(isOptimized)
        gameObject.SetActive(false);
        Canvas canvasRef = gameObject.GetComponentInChildren<Canvas>();


        //Boxes are set in world space. This is just to make sure it is set to the correct camera without extra fiddling. 
        if (canvasRef != null)
        {
            canvasRef.worldCamera = worldManager.instance.GetCamera();
        }
        unHighlightBox();
    }

    private void ResetState()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //transform.position += new Vector3(0, 0, ID[0] + ID[1]/2);
        if (HighlightedImage != null)
        {
            HighlightedImage.enabled = false;
        }

        if (_textDisplay != null)
        {

            if (DangerNearby > 0)
            {
                _textDisplay.text = DangerNearby.ToString("D");
                _textDisplay.color = DangerColors[DangerNearby];
            }
            else
            {
                _textDisplay.text = string.Empty;
            }

            _textDisplay.enabled = false;
        }

        if (_button != null && travelCost != Board.WALL_COST)
        {
            _button.targetGraphic.color = new Color(0,worldManager.instance.GetSeed(), transform.position.z, 1.0f);
            _button.interactable = true;
        }
    }
}
