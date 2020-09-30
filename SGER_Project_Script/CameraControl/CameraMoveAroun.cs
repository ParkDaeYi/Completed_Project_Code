using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMoveAroun : MonoBehaviour
{
    /**
   * date 2018.07.12
   * author Lugub
   * desc
   *  키보드 "S" "W" 를 누르면 각각
   *  카메라가 뒤로, 앞으로 움직이고
   *  키보드 "A" "D" 를 누르면 각각
   *  카메라가 왼쪽, 오른쪽으로 회전하고
   *  "Q" "E"를 누르면 각각 위로 아래로 움직인다.
   */

    [Header("Component")]
    public Transform _cameraTransform; //카메라 Transform

    [Header("Variable")]
    public bool _cameraAroun = true;
    public float _mouseSensitivity; //카메라 민감도
    public float _mouseRotateSpeed; //카메라 회전 속도

    [Header("CameraView")]
    /* 카메라 시점 변환용 */
    public GameObject _smallSchedulerParent;
    int _cameraViewIndex; // -1 : 전체 시점 , 0 ~ 사람수 : 해당 인원의 시점

    /* 카메라 시점 변환할 인덱스 설정 */
    public int _disableIndex;
    public int _enableIndex = -1;

    [Header("ClickedItemCanvas")]
    public ClickedItemControl _clickedItemControl;

    public void Awake()
    {
        _cameraTransform = GetComponent<Transform>(); //현재 Transform 담음
        Static.STATIC.cameraMoveAroun = this;
        _mouseSensitivity = 45f;
        _mouseRotateSpeed = 30f;
    }

    public void Update()
    {
        if (_cameraAroun)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                EditMouseControl();
            }

            /* 특정 키 (현 : Keypad 0)를 누르면 카메라 시점이 변환되도록! */
            if (!InputFieldFocused()) //InputField에 Focus되어 있지 않은 경우
            {
                if (Input.GetKeyDown(KeyCode.Space)) ChangeCameraNumber();
                if (Input.GetKeyDown(KeyCode.Keypad0)) ChangeCameraNumber(0);
                if (Input.GetKeyDown(KeyCode.Keypad1)) ChangeCameraNumber(1);
                if (Input.GetKeyDown(KeyCode.Keypad2)) ChangeCameraNumber(2);
                if (Input.GetKeyDown(KeyCode.Keypad3)) ChangeCameraNumber(3);
                if (Input.GetKeyDown(KeyCode.Keypad4)) ChangeCameraNumber(4);
                if (Input.GetKeyDown(KeyCode.Keypad5)) ChangeCameraNumber(5);
                if (Input.GetKeyDown(KeyCode.Keypad6)) ChangeCameraNumber(6);
                if (Input.GetKeyDown(KeyCode.Keypad7)) ChangeCameraNumber(7);
                if (Input.GetKeyDown(KeyCode.Keypad8)) ChangeCameraNumber(8);
                if (Input.GetKeyDown(KeyCode.Keypad9)) ChangeCameraNumber(9);
            }
        }
        /* 마우스 클릭을 통해 화면을 빠져나오면, 카메라 이동 할 수 있도록 */
        else { if (Input.GetMouseButtonDown(0)) { _cameraAroun = true; } }

        /* 객체 클릭시 생기는 Sprite 이미지가 카메라 이동시 따라가도록! */
        MoveSprite();
    }

    void MoveSprite()
    {

        if (_clickedItemControl._clickedItem != null && _clickedItemControl._clickedItem.item3d != null)
        {
            /* 해당 객체의 위치를 저장 */
            Vector3 _itemPosition = _clickedItemControl._clickedItem.item3d.transform.position;

            /* 인물 객체인 경우, 스프라이트 이미지가 사람 머리 위에 존재하도록 위치 수정. */
            if (_clickedItemControl._clickedItem._originNumber >= 2000 && _clickedItemControl._clickedItem._originNumber < 3000)
            {
                _itemPosition.y += 20f;
            }

            /* 스프라이트 이미지가, 카메라에 따라 이동하도록 Update! */
            _clickedItemControl._SpriteObject.transform.position = Camera.main.WorldToScreenPoint(_itemPosition);
        }
    }

    public void EditMouseControl()
    {
        float x = Input.GetAxis("Mouse X"); //마우스의 X축 값 담음
        float y = Input.GetAxis("Mouse Y"); //마우스의 Y축 값 담음
        float scroll = Input.GetAxis("Mouse ScrollWheel"); //마우스의 휠 값을 담음

        /* 왼쪽 마우스 클릭하면서 UI 상에 마우스가 올려져있지 않으면 */
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            /* 상하좌우 이동 */
            _cameraTransform.Translate(-x * Time.deltaTime * _mouseSensitivity, -y * Time.deltaTime * _mouseSensitivity, 0f);
        }
        /* 오른쪽 마우스 클릭하면 */
        if (Input.GetMouseButton(1))
        {
            /* 화면 회전 */
            _cameraTransform.rotation = Quaternion.Euler(_cameraTransform.eulerAngles.x - y * Time.deltaTime * _mouseRotateSpeed, _cameraTransform.eulerAngles.y + x * Time.deltaTime * _mouseRotateSpeed, 0f);
        }
        /* 앞 뒤로 이동 */
        _cameraTransform.Translate(0f, 0f, scroll * Time.deltaTime * _mouseSensitivity * 10f);
    }

    /**
   * date 2019.05.06
   * author INHO
   * 특정 버튼을 누름에 따라 카메라 시점이 변환 될 수 있도록!
     * */
    public void ChangeCameraNumber()
    {
        int _peopleCnt = _smallSchedulerParent.transform.childCount;
        if (_peopleCnt == 0) return; // 0으로 나눌수 없도록 예외처리

        /* -1 : 전체시점 , 0 ~ PeopleCnt : 해당 인원 시점 */
        _disableIndex = (_cameraViewIndex++ % (_peopleCnt + 1)) - 1;
        _enableIndex = (_cameraViewIndex % (_peopleCnt + 1)) - 1;

        OnEnableCamera(_enableIndex);
        OnDisableCamera(_disableIndex);
    }

    /* 키패드 입력으로, 시점이 변환될 수 있도록 -> 오버로딩 형식 */
    public void ChangeCameraNumber(int _keypad)
    {
        if (_smallSchedulerParent.transform.childCount < _keypad) return;
        _keypad--;

        OnDisableCamera(_enableIndex);
        OnEnableCamera(_keypad);

        _enableIndex = _keypad;
    }

    /* 어떤 카메라를 Enable 시킬지? */
    public void OnEnableCamera(int _index)
    {
        if (_index == -1) this.transform.GetChild(0).transform.GetComponent<Camera>().enabled = true;
        else
        {
            GameObject _ableObject = _smallSchedulerParent.transform.GetChild(_index).GetComponent<SmallSchedulerBar>()._object.item3d;
            ChangeLayersRecursively(_ableObject.transform, 8, true); // layer : CurrentHuman (해당 객체는 1인칭으로 안보여야 됨)
        }
    }

    /* 어떤 카메라를 Disable 시킬지? */
    public void OnDisableCamera(int _index)
    {
        if (_index == -1) this.transform.GetChild(0).transform.GetComponent<Camera>().enabled = false;
        else
        {
            GameObject _disableObject = _smallSchedulerParent.transform.GetChild(_index).GetComponent<SmallSchedulerBar>()._object.item3d;
            ChangeLayersRecursively(_disableObject.transform, 2, false); // layer : Ignore Laycast (해당 객체는 보여야 됨)
        }
    }

    /* 재귀함수를 통해 모든 Transform layer 바꾸기 및 Camera 찾아서 이벤트 동작 */
    public void ChangeLayersRecursively(Transform _trans, int _idx, bool _swi)
    {
        _trans.gameObject.layer = _idx;
        if (_trans.Find("Camera"))
        {
            _trans.Find("Camera").GetComponent<Camera>().enabled = _swi;
        }
        foreach (Transform child in _trans)
        {
            ChangeLayersRecursively(child, _idx, _swi);
        }
    }

    /* InputField가 Focused 되어 있는지 반환하는 함수 */
    public bool InputFieldFocused()
    {
        GameObject _selectedGameObject = EventSystem.current.currentSelectedGameObject; //선택된 오브젝트를 얻음
        if (_selectedGameObject) //선택된 오브젝트가 있으면
        {
            InputField _selectedInputField = _selectedGameObject.GetComponent<InputField>(); //선택된 UI 오브젝트로부터 InputFied 컴포넌트 얻음
            if (_selectedInputField) return _selectedInputField.isFocused; //해당 오브젝트가 InputField가 있으면 Focus 여부에 따라 반환 결정
            else return false; //해당 오브젝트가 InputField가 없으면 false 반환
        }
        else return false; //선택된 오브젝트가 없으면
    }
}