using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Movement : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Image _visualOutput;
    [SerializeField] Sprite _defaultImage;

    #region VisualMovementCuesController_Singleton

    public static UI_Movement instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public void LinkButton(UnityAction action)
    {
        if (_button != null)
            _button.onClick.AddListener(action);
    }

    // Start is called before the first frame update
    void Start()
    {
        _visualOutput.sprite = _defaultImage;
    }

}
