using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenameInputField : MonoBehaviour
{

    public CameraMoveAroun _cameraMoveAroundSwi;
    public InputField _inputField;
    public LoadButton _loadButton;

    // Use this for initialization
    void Start()
    {
        _cameraMoveAroundSwi = Static.STATIC.cameraMoveAroun;
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputField.isFocused)
        {
            _cameraMoveAroundSwi._cameraAroun = false;
        }

    }

    public void MouseOut()
    {
        _loadButton._renameField.SetActive(false);
        _loadButton.inputField_flag = false;
    }
}
