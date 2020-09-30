using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeName : MonoBehaviour
{
    /*
* date 2018.09.29
* author INHO
* desc
* 각 인물 객체를 생성했을 때, 이름을 바꿀수 있도록
* 도와주는 스크립트.
*/

    public InputField _inputField;
    public GameObject _inputTextHolder;
    public GameObject _inputText;

    [Header("Script")]
    public CameraMoveAroun _cameraMoveAroundSwi;
    public ClickedItemControl _clickedItemControl;

    [Header("Schduler")]
    public GameObject _bigScheduler;
    public GameObject _smallScheduler;

    private void Start()
    {
        /* static 으로 선언된 스크립트를 연결시켜 준다. */
        _cameraMoveAroundSwi = Static.STATIC.cameraMoveAroun;

        _bigScheduler = GameObject.Find("Canvas").transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
        _smallScheduler = GameObject.Find("Canvas").transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;
    }

    void Update()
    {
        /* Input Field 에 커서가 깜빡이면, W,A,S,D 를 눌러도 카메라 이동 안하도록 */
        if (_inputField.isFocused)
        {
            _inputTextHolder.GetComponent<Text>().enabled = false;
            _inputText.GetComponent<Text>().enabled = true;
            _cameraMoveAroundSwi._cameraAroun = false;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            _clickedItemControl._clickedItem.itemName = _inputField.text;
            _cameraMoveAroundSwi._cameraAroun = true;

            if (_clickedItemControl._clickedItem._originNumber >= 2000 && _clickedItemControl._clickedItem._originNumber <= 2005)
            {
                //캐릭터의 이름을 변경후 스케줄러에서도 표시
                _bigScheduler.transform.Find(_clickedItemControl._clickedItem.item3d.transform.parent.name).GetChild(0).GetChild(0).GetComponent<Text>().text = _inputField.text;
                _smallScheduler.transform.Find(_clickedItemControl._clickedItem.item3d.transform.parent.name).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = _inputField.text;
                //_bigScheduler.transform.Find(_clickedItemControl._clickedItem.item3d.transform.parent.name).GetChild(0).GetChild(0).GetComponent<Text>().text = _inputField.text + _clickedItemControl._clickedItem._objectNumber;
                //_smallScheduler.transform.Find(_clickedItemControl._clickedItem.item3d.transform.parent.name).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = _inputField.text + _clickedItemControl._clickedItem._objectNumber;
            }

        }
    }

    /* 입력받은 Text를 Script에 저장해주는 Method */
    public void OnSearchSlotName(InputField _inputField)
    {

    }

}
