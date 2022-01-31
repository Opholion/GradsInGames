using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LastActionIndicator : MonoBehaviour
{
    #region VisualCuesController_Singleton

    public static LastActionIndicator instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion


    [SerializeField] Image _visualOutput;
    [SerializeField] Sprite _MovementIndication;
    [SerializeField] Sprite _FireIndication;
    [SerializeField] Sprite _IceIndication;
    [SerializeField] Sprite _BuffIndication;
    [SerializeField] Sprite _DebuffIndication;

    public enum directions
    {
        north, east, west, south
    }

    public enum abilityTypes
    {
        Fire, Ice, Buff, Debuff
    }

    // Start is called before the first frame update
    void Start()
    {
        //If this is true then the class is unusable. No matter the circumstance. 
        if (_visualOutput == null) Destroy(this);
    }

    //A bit repetitive but by doing this is makes it far more readable. 
    public void ShowAbility(abilityTypes ability)
    {
        //Simply reusing the below code to avoid inflating the function. 
        switch (ability)
        {
            case abilityTypes.Fire:
                ShowFireAbility();
                break;
            case abilityTypes.Ice:
                ShowIceAbility();
                break;
            case abilityTypes.Buff:
                ShowBuffAbility();
                break;
            case abilityTypes.Debuff:
                ShowDebuffAbility();
                break;
        }
    }

    public void ResetAbilityOutput()
    {
        if (isAbility(_visualOutput.sprite))
        {
            _visualOutput.sprite = null;
        }
    }

    public void ShowEmpty()
    {
        _visualOutput.sprite = null; 
    }
    protected bool isAbility(Sprite input)
    {
        return (
               input == _BuffIndication ||
               input == _DebuffIndication ||
               input == _FireIndication ||
               input == _IceIndication
               );  
    }

    public void ShowBuffAbility()
    {
        //Set the rotation in the scenario where the controller might point in a set direction beforehand.
        _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 0, 1);
        _visualOutput.sprite = _BuffIndication;
    }

    public void ShowFireAbility()
    {
        _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 0, 1);
        _visualOutput.sprite = _FireIndication;
    }

    public void ShowIceAbility()
    {
        _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 0, 1);
        _visualOutput.sprite = _IceIndication;
    }

    public void ShowDebuffAbility()
    {
        _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 0, 1);
        _visualOutput.sprite = _DebuffIndication;
    }

    public void MovementDirection(directions facingDirection)
    {
        switch (facingDirection)
        {
            //Arrow used is pointing left. These have been manually adjusted to account for that. 
            case directions.east:
                _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 0, 1);
                break;
            case directions.south:
                _visualOutput.rectTransform.rotation =  Quaternion.Euler(0, 0, -90);
                break;
            case directions.north:
                _visualOutput.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case directions.west:
                _visualOutput.rectTransform.rotation = new Quaternion(0, 0, 180, 1);
                break;
        }

        _visualOutput.sprite = _MovementIndication;
    }
}
