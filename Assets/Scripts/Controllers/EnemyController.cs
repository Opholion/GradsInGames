using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{





    private PlayerUnit _PlayerRef; //Player is always targetted. Store reference.
    private List<BaseAbility> AbilityClasses;
    GameObject _currentObjectReference;
    AbilityData _abilityDataRef;
    BaseAbility _currentComponent;    // Start is called before the first frame update

    [SerializeField] protected int[] AbilityRange;
    [SerializeField] protected int[] AbilityDamage;
    [SerializeField] protected List<GameObject> _UnitAbilities;

    // Start is called before the first frame updatew
    void Start()
    {
        AbilityClasses = new List<BaseAbility>();
        _ControlledObject = GetComponent<Unit>();
        int i = 0; 
        if(_ControlledObject.SetController(this))
         foreach (GameObject ability in _UnitAbilities)
        {
            _currentObjectReference = Instantiate(ability, gameObject.transform);
            _abilityDataRef = _currentObjectReference.GetComponent<AbilityData>();

            _currentComponent = _currentObjectReference.GetComponent<BaseAbility>();
            if (_currentComponent == null)
                _currentComponent = _currentObjectReference.AddComponent<BaseAbility>();


            //There are custom stat values to input for teh ability. This allows for only 1 to be applied, or both.
            if(AbilityRange.Length > i && AbilityDamage.Length > i)
            _currentComponent.Setup(_abilityDataRef, _ControlledObject, AbilityRange[i], AbilityDamage[i]);

                else if (AbilityRange.Length > i)
                    _currentComponent.Setup(_abilityDataRef, _ControlledObject, AbilityRange[i]);

                else if (AbilityDamage.Length > i)
                    _currentComponent.Setup(_abilityDataRef, _ControlledObject,0, AbilityDamage[i]);

                else
                    _currentComponent.Setup(_abilityDataRef, _ControlledObject);


                AbilityClasses.Add(_currentComponent as BaseAbility);
            _currentObjectReference.GetComponent<CanvasGroup>().alpha = 0;
                ++i;
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_PlayerRef == null)
        {
            _PlayerRef = (PlayerUnit)worldManager.instance.GetPlayer();
        }
        if (_ControlledObject == null)
        {
            _ControlledObject = GetComponent<Unit>();
        }

    }



    //Players will expect a "Next turn" to be slow so it gives the chance to do something expensive
    public override bool NextTurn()
    {



        if (_PlayerRef != null)
        {
            //Assuming it should be fast as it is called multiple times in a short section.
            if (_AbilityTarget != _PlayerRef.GetGridPos())
            {
                _AbilityTarget = _PlayerRef.GetGridPos();

                //Position doesn't need updating if it hasn't moved.
                _ControlledObject.SetDestination(_PlayerRef.GetGridPos());
            }

            //If the target is within range and the unit holds an ability; attempt to attack the target.
            if (_UnitAbilities.Count > 0 && _AbilityTarget.Item1 != null)
            {
                _CurrentAbility = AbilityClasses[Random.Range(0, AbilityClasses.Count)].GetComponent<BaseAbility>();

                Transform RadiusRef = _ControlledObject.GetMoveRadius();

                //Preparing for real time adjustments to the radius.
                if (!(RadiusRef.localScale.x > _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1 - 0.01 && RadiusRef.localScale.x < _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1 + 0.01))
                    RadiusRef.localScale = new Vector3(_CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1, _CurrentAbility.GetRange() + _CurrentAbility.GetRange() + 1, 1);


                if (_ControlledObject.GetDistToTarget() <= _CurrentAbility.GetRange() )
                {
                   
                    if (_CurrentAbility.AttemptActiveAbility(_AbilityTarget))
                    {
                        PlayerController.instance.EnterCombat(this);
                        _CurrentAbility = null;
                    }


                    return true;
                }
                _CurrentAbility = null;
            }
            else
            {
                _ControlledObject.ControllerMovementUpdate();
            }
        }



        PlayerController.instance.LeaveCombat(this);
        return false;
    }
}
