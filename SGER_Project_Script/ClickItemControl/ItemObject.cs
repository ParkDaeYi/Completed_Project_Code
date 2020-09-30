using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemObject : MonoBehaviour
{
    /**
* date 2018.07.18
* author Lugub
* desc
*  동적으로 생성된 객체에 추가되는 스크립트.
*  생성된 객체가 Item정보를 가지는 역할을 한다.
*  
*  또한 객체를 클릭했을 경우 그 객체가 ClickedItemCanvas의 ClickeditemControl.cs 의 _clickedItem변수와
*  연결시켜서 아이템을 수정 및 삭제 등의 작업을 할 수 있게 함
*  
*/
    /* 이 스크립트는 동적생성으로 만들어진 객체에 AddComponent() 하기 때문에
     * clickedItemCanvas와 연결해 주기 위해서 GameObject.Find()를 사용한다. */
    [Header("ClickedItemCanvas")]
    private ClickedItemControl _clickedItemControl;

    [Header("This Item")]
    public Item _thisItem;
    public HumanItem _thisHuman;
    public Animator _animator;

    [Header("CameraMove")]
    public CameraMoveAroun _cameraMoveAron;

    [Header("WallInfo")]
    public int _placeNumber;
    public float _tilingX;
    public float _tilingY;

    [Header("HumanInfo")]
    public Vector3 _humanInitPosition; //사람일 경우 초기위치 -> 정지버튼을 눌렀을때 해당 위치로 이동
    public Vector3 _humanInitRotation; //사람일 경우 초기방향 -> 정지버튼을 눌렀을때 해당 방향으로 쳐다봄

    void Start()
    {
        _clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();

        /* 사람 객체일 경우, Animator Controller 변수를 할당 해 주도록! */
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000)
        {
            _animator = _thisItem.item3d.GetComponent<Animator>();
            //사람객체의 초기위치를 기억시켜놓음 //초기위치는 _item의 포지션 
            //_humanInitPosition = this.gameObject.transform.parent.transform.localPosition;
            //사람객체의 초기방향을 기억시켜놓음 //초기방향은 _item.item3d의 로테이션
            //_humanInitRotation = this.gameObject.transform.rotation;
        }
        _cameraMoveAron = GameObject.Find("CameraController").GetComponent<CameraMoveAroun>();


        if (_thisItem._originNumber == 2001) //남
        {
            this.gameObject.transform.parent.name = "Man" + _thisItem._objectNumber;
        }
        else if (_thisItem._originNumber == 2000) //테스트
        {
            this.gameObject.transform.parent.name = "Daughter" + _thisItem._objectNumber;
        }
        else if (_thisItem._originNumber == 2002) //여
        {
            this.gameObject.transform.parent.name = "Woman" + _thisItem._objectNumber;
        }
        else if(_thisItem._originNumber == 2003)
        {
            this.gameObject.transform.parent.name = "Woongin" + _thisItem._objectNumber;
        }

    }

    private void Update()
    {
        //RoofAnimation();
    }

    /* 외곽선이 나타나게 하는 부분 */
    /* 이 스크립트가 가지고 있는 _thisitem의 _originNumber를 사용해서
 * 사물일 때와 사람일 때를 구분하여 외곽선이 다르게 나타나게 해야함
 * 배치할 사물일 때는 1000자리대의 _originNumber를 가지고.
*  그리고 사람 객체는 2000자리대의 _originNumber를 가진다. */

    /* 객체를 클릭했을 경우 */
    public void OnMouseDown()
    {
        //Debug.Log("ItemObject.cs 75줄 : " + _thisItem.itemName + "Click");

        /* 마우스가 Raycast 등의 작업 중이면, 클릭이 안되도록! */
        if (UIController._isMouseUsed) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (tag == "Floor" || tag == "Wall" || tag == "Door") return; //바닥이나 벽이면 이 함수 실행 안함

        if (_clickedItemControl._clickedItem != _thisItem)
        {

            ///* 외곽선 처리 - 이전에 클릭됬었던 객체는 안보이게 */
            //if (_clickedItemControl._clickedItem != null)
            //{
            //    _clickedItemControl._clickedItem.item3d.GetComponent<Outline>().enabled = false;

            //    if (_clickedItemControl._clickedItem._originNumber >= 2000 && _thisItem._originNumber >= 2000)
            //    {
            //        _cameraMoveAron.CameraZoomChange(this.transform.position);

            //    }
            //    else if (_clickedItemControl._clickedItem._originNumber >= 2000 && _thisItem._originNumber < 2000)
            //    {
            //        _cameraMoveAron.CameraZoomOut();
            //    }
            //    else if (_clickedItemControl._clickedItem._originNumber < 2000 && _thisItem._originNumber >= 2000)
            //    {
            //        _cameraMoveAron.CameraZoomIn(this.transform.position);
            //    }

            //}
            //else
            //{
            //    if (_thisItem._originNumber >= 2000)
            //    {
            //        _cameraMoveAron.CameraZoomIn(this.transform.position);
            //    }
            //}
            _clickedItemControl._clickedItem = _thisItem;
            _clickedItemControl._clickedHumanItem = _thisHuman;
            _clickedItemControl.ClickMenuActivate();
        }
    }

    /* 사람 객체가 동작을 가지면, 그 동작 반복해 주도록 설정! */
    private void RoofAnimation()
    {
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000)
        {
            //_animator.Play(_thisHuman._status);
        }
    }
}