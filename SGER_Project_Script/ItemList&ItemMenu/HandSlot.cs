using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSlot : MonoBehaviour
{
    /**
* date 2019.07.11
* author GS
* desc
*  HandItem 슬롯에 동적으로 추가되는 스크립트
*  Slot이나 HumanSlot 스크립트와는 다르게 방안에 배치하는 것이 아니라, 손에 들려지는 것이기 때문에 따로 구분.
*/
    public ClickedItemControl _clickedItemControl;
    public Item _thisItem;
    public bool _isLeft; //왼손이면 true, 오른손이면 false
    private Item _thisHuman;
    private HumanItem _thisHumanItem;
    private GameObject _handObjectClone;
    public Transform _hand; //_leftHand와 _rightHand의 역할 수행
    string _itemName; // 히스토리를 위해 핸드아이템의 이름을 받아옴

    private Vector3 _localPos;

    /* 슬롯이 클릭되었을 때 */
    public void OnClickSlot()
    {
        _thisHuman = _clickedItemControl._clickedItem;
        _thisHumanItem = _clickedItemControl._clickedHumanItem;

        if (_isLeft) //왼손이면
        {
            _hand = _clickedItemControl._clickedItem.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);

            if (_thisHumanItem._leftHandItem != null) //이미 왼손에 handitem을 들고있는 경우
            {
                _itemName = _thisHumanItem._leftHandItem.itemName;
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._leftHandItem);
                if (!_thisHumanItem._leftHandItem.itemName.Equals(_thisItem.itemName)) //이미 들고있는 물건과 착용하려는 물건이 같지않은 경우
                {
                    GameObject dObj = _hand.Find(_thisHumanItem._leftHandItem.itemName).gameObject;
                    Destroy(dObj);
                    _thisHumanItem._leftHandItem = _thisItem;
                }
                else
                {
                    GameObject dObj = _hand.Find(_thisHumanItem._leftHandItem.itemName).gameObject;
                    Destroy(dObj);
                    _thisHumanItem._leftHandItem = null;
                    return;
                }
            }
            else
            {
                _itemName = "null";
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._leftHandItem);
                _thisHumanItem._leftHandItem = _thisItem;
            }

        }
        else //오른손이면
        {
            _hand = _clickedItemControl._clickedItem.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);

            if (_thisHumanItem._rightHandItem != null) //이미 왼손에 handitem을 들고있는 경우
            {
                _itemName = _thisHumanItem._rightHandItem.itemName;
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._rightHandItem);
                if (!_thisHumanItem._rightHandItem.itemName.Equals(_thisItem.itemName))
                {
                    GameObject dObj = _hand.Find(_thisHumanItem._rightHandItem.itemName).gameObject;
                    Destroy(dObj);
                    _thisHumanItem._rightHandItem = _thisItem;
                }
                else
                {
                    GameObject dObj = _hand.Find(_thisHumanItem._rightHandItem.itemName).gameObject;
                    Destroy(dObj);
                    _thisHumanItem._rightHandItem = null;
                    return;
                }
            }
            else
            {
                _itemName = "null";
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._rightHandItem);
                _thisHumanItem._rightHandItem = _thisItem;
            }
        }

        /* 실제로 handitem을 생성하는 과정 */

        GameObject _handObjectClone = Instantiate(_thisItem.item3d); //객체 생성
        _handObjectClone.name = _thisItem.itemName; //이름 지정
        _handObjectClone.transform.SetParent(_hand); //손을 부모로 설정
        _handObjectClone.transform.localRotation = Quaternion.Euler(Vector3.zero); //로컬 회전값을 손 회전값과 동일하게 변경

        //각 객체에 따라 객체의 위치 조정
        if (_clickedItemControl._clickedItem._originNumber == 2001) //Man
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._manRightPos;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2002) //Woman
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._womanRightPos;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2000) //Daughter
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._daughterRightPos;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2003) //Woongin
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._woonginPos;
        }
        else
        {
            _localPos = _hand.transform.GetChild(2).transform.position; //세번째 손가락 위치로 지정
        }

        _handObjectClone.transform.localPosition = _localPos;

        if (_isLeft)
        {
            //왼손일 경우 좌우 반전
            Vector3 _tmp = _handObjectClone.transform.localScale;
            _tmp.z *= -1;
            _handObjectClone.transform.localScale = _tmp;

            //앞 뒤 반전
            _tmp = _handObjectClone.transform.localPosition;
            _tmp.z = -_tmp.z;
            _handObjectClone.transform.localPosition = _tmp;
        }

        //_handObjectClone.transform.localPosition = _localPos;

        //_handObjectClone.transform.localRotation = Quaternion.Euler(_handObjectClone.transform.localRotation * (_clickedItemControl._clickedItem.item3d.transform.forward.normalized * (-1)));

        //Debug.Log("HandSlot 96 : " + (_hand.transform.position - _clickedItemControl._clickedItem.item3d.transform.position).normalized);
        //Debug.Log("HandSlot 93 : "+_clickedItemControl._clickedItem.item3d.transform.forward.normalized*(-1));
    }

}
