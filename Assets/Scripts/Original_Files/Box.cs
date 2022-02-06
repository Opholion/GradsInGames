using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Box : MonoBehaviour
{
    [SerializeField] protected Color[] DangerColors = new Color[8];
    public Unit? heldUnit;


    [SerializeField] protected Image HighlightedImage;
    [SerializeField] protected Image HLImageDecal;
    [SerializeField] protected Image BackgroundImage;
                
    [SerializeField] protected Sprite UserSelectedIcon;
    [SerializeField] protected Sprite FloorIcon;
    [SerializeField] protected Sprite WallIcon;


    [SerializeField] protected Sprite[] DecalList;

    private TMP_Text _textDisplay;
    private Button _button;
    private Action<Box> _changeCallback;
   
    //Values are X/Y  - Could not use a Vector2 as that uses a float.
    public int[] ID { get; private set; }

    public bool IsWall { get; private set; }
    public bool IsDoorway { get; private set; }
   // public bool IsDangerous { get; private set; }
    public bool IsScanned = false;

    private static float? _BaseZAxis;
    private static float _WallDisplacement;
    private float? _ExpectedDisplacement;
    private Color _BaseColor;
    private Color _TileTypeColor;
    int travelCost = 0;
    public int getTravelCost()
    {
        //Enemies target the player. Easier to allow them to path to player and then attack when within x range. 
        if (heldUnit != null && heldUnit != worldManager.instance.GetPlayer()) return Board.WALL_COST;
        return travelCost;
    }

    public bool GetIsDoor()
    {
        return IsDoorway;
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

    public void UpdateTileDIsplacement(float zDisplacement)
    {


        //Allows a reset to be done on creation
        if (_BaseZAxis != null)
            transform.position = new Vector3(transform.position.x, transform.position.y, (float)_BaseZAxis);

        _BaseZAxis = transform.position.z;

        if (zDisplacement > -0.00001 && zDisplacement < 0.00001)
        {
            zDisplacement = UnityEngine.Random.Range(-0.01f, 0.01f);
        }
        transform.position += new Vector3(0, 0, zDisplacement);
    }


    public void Charge(bool isDoor, bool isWall, Action<Box> onChange, float WallDisplacement, Color WallColor, Color? color = null, float zDisplacement = 0)
    {
        _WallDisplacement = WallDisplacement;

        //Store base displacement
        if(_BaseZAxis == null)
        _BaseZAxis = transform.position.z;


        //Store base color and assume wall.
        if(_BaseColor != null)
        _BaseColor = WallColor;
        BackgroundImage.color = (Color)_BaseColor;

        if (color != null)
            _TileTypeColor = (Color)color;


        //Calculate different potential displacements
        transform.position = new Vector3(transform.position.x, transform.position.y, (float)_BaseZAxis);

        _BaseZAxis = transform.position.z;

        //Add slight deviation
        if (zDisplacement > -0.00001 && zDisplacement < 0.00001)
            zDisplacement = UnityEngine.Random.Range(-0.015f, 0.015f);

        _ExpectedDisplacement = zDisplacement + UnityEngine.Random.Range(-0.0025f, 0.0025f);

        transform.position += new Vector3(0, 0, _WallDisplacement);


        _changeCallback = onChange;
        IsDoorway = isDoor;
        ResetState();

        IsWall = isWall;
        if (IsWall)
        {
            travelCost = Board.WALL_COST;
        }
        else
        {
            travelCost = 0;  
        }

        _button.interactable = false;

            BackgroundImage.color = _BaseColor;

        if (DecalList.Length > 0)
        {
            HLImageDecal.sprite = DecalList[UnityEngine.Random.Range(0, DecalList.Length)];
            HLImageDecal.transform.rotation = new Quaternion(0, 0,45 * UnityEngine.Random.RandomRange(0, 4), 1);
        }
    }
    const float COLOURRANDOMIZATION = 0.075f * (0.25f*0.75f);
    public void Reveal()
    {
        //Button is only interactable if seen. Easy return to avoid expense. 
        if (_button.interactable == true) return;
        _button.interactable = true;

        transform.position = new Vector3(transform.position.x, transform.position.y, (float)_BaseZAxis + (float)_ExpectedDisplacement);
       
        if (IsWall)
        {
            travelCost = Board.WALL_COST;
            BackgroundImage.sprite = WallIcon;
        }
        else
        {
            travelCost = 0;
            BackgroundImage.sprite = FloorIcon;
        }
        if (_TileTypeColor != null)
            BackgroundImage.color = (Color)_TileTypeColor + new Color(UnityEngine.Random.Range(-COLOURRANDOMIZATION, COLOURRANDOMIZATION), UnityEngine.Random.Range(-COLOURRANDOMIZATION, COLOURRANDOMIZATION), UnityEngine.Random.Range(-COLOURRANDOMIZATION, COLOURRANDOMIZATION));

        if (DecalList.Length > 0)
        {
            HLImageDecal.sprite = DecalList[UnityEngine.Random.Range(0, DecalList.Length)];
            HLImageDecal.transform.rotation = new Quaternion(0, 0, 45 * UnityEngine.Random.RandomRange(0, 4), 1);
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


        transform.position = new Vector3(transform.position.x, transform.position.y, ((_BaseZAxis != null) ? (float)_BaseZAxis : 0));

        _ExpectedDisplacement = null;
        _BaseZAxis = null;

        if (_BaseColor != null)
        {
            BackgroundImage.color = Color.white;
            travelCost = Board.WALL_COST;
            BackgroundImage.sprite = WallIcon;
        }
    }
    public void OnClick()
    {
        _changeCallback?.Invoke(this);
        //While this is likely to be inefficient, this should only happen rarely and so can be afforded the extra costs in time.

    }

    public bool AddUnit(Unit unit)
    {
        if (heldUnit == null)
        {
            heldUnit = unit;
            return true;
        }
        return false;
    }
    public void RemoveUnit()
    {
        heldUnit = null;
    }

    public bool IsHoldingUnit()
    {
        return (heldUnit != null);
    }
    private void Awake()
    {
        _textDisplay = GetComponentInChildren<TMP_Text>(true);
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnClick);

        ResetState();
        //transform.Rotate(0, 180, 0, Space.World);

        unHighlightBox();



       // Canvas canvasRef = gameObject.GetComponentInChildren<Canvas>();
       //
       //
       // //Boxes are set in world space. This is just to make sure it is set to the correct camera without extra fiddling. 
       // if (canvasRef != null)
       // {
       //     canvasRef.worldCamera = worldManager.instance.GetCamera();
       // }
    }

    private void ResetState()
    {
       // if (_BaseZAxis != null)
       //     transform.position = new Vector3(transform.position.x, transform.position.y, (float)_BaseZAxis);


        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //transform.position += new Vector3(0, 0, ID[0] + ID[1]/2);
        if (HighlightedImage != null)
        {
            HighlightedImage.enabled = false;
        }

        if (_textDisplay != null)
        {

            _textDisplay.text = string.Empty;
            _textDisplay.enabled = false;
        }

        if (_button != null && travelCost != Board.WALL_COST)
        {
            _button.targetGraphic.color = _button.targetGraphic.color * (1.0f/travelCost);
            _button.interactable = true;
        }


    }
}
