using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DressSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    /*
     * date 2019.07.24
     * author HR
     * desc
     * dress viewport에 동적으로 생성되는 버튼의 onclick 함수
     */

    [Header("Controller")]
    public ClickedItemControl _clickItemControl;
    public ItemListControl _itemListControl;
    public DressController _uiController;
    public GameObject _DressContent;

    public string[] _name; //착용될 부위_의상명 || shoes_normal또는 abnormal_신발명
    public GameObject _newCloth; //ClickedItemControl.cs 에서 버튼 갱신 시 삽입
    public Vector3 _originPos; //신발 적용 시 높이 변경

    public string _prevName; //기존에 작용하고 있던 의상의 name
    public GameObject _prevObject;
    private bool _isTook=false; //선택완료여부
    public GameObject _prevButton;
    public Sprite _img;

    private void Start()
    {
        _name = this.name.Split('_');
        
        if (_name[0].Equals("shoes"))
        {
            this.GetComponentInChildren<Text>().text = _name[2];
        }
        else
        {
            this.GetComponentInChildren<Text>().text = _name[1];
        }

        if (_name[0].Equals("shirt")){
            _DressContent = _DressContent.transform.GetChild(1).gameObject;
        }
        else if (_name[0].Equals("pant"))
        {
            _DressContent = _DressContent.transform.GetChild(2).gameObject;
        }
        else
        {
            _DressContent = _DressContent.transform.GetChild(3).gameObject;
        }

    }
    public void dressChange(GameObject _new)
    {
        Vector3 _pos = _clickItemControl._clickedItem.item3d.transform.localPosition;
        string[] _name = _new.name.Split('_');
        /*
         * date 2019.09.02
         * author skyde47
         * desc
         * history에 담기 위한 정보 추가
         */
        Item _hist = _clickItemControl._clickedItem;
        string _prev = null;

        //파츠 분리
        if (_name[0].Equals("shirt"))
        {

            if (_clickItemControl._clickedHumanItem._shirt != null)
            {
                _prev = _clickItemControl._clickedHumanItem._shirt.name;
                _clickItemControl._clickedHumanItem._shirtName = _new.name;
                _clickItemControl._clickedHumanItem._shirt.SetActive(false);
                //기존에 선택된 버튼의 이미지 변경
                _prevButton = _DressContent.transform.GetChild(0).GetChild(0).transform.Find(_prev).gameObject;
            }
            else _prev = "null";
            if (_isTook) //착용한 경우에만 history에 삽입
            {
                _clickItemControl._clickedHumanItem._shirt = _new;
                _clickItemControl._clickedHumanItem._shirtName = _new.name;
                HistoryController.pushDressChangeHist(_name, _prev, this.name, _hist._objectNumber, _hist._originNumber,_pos);
            }
        }
        else if (_name[0].Equals("pant"))
        {
            if (_clickItemControl._clickedHumanItem._pant != null)
            {
                _prev = _clickItemControl._clickedHumanItem._pant.name;
                _clickItemControl._clickedHumanItem._pantName = _new.name;
                _clickItemControl._clickedHumanItem._pant.SetActive(false);
                _prevButton = _DressContent.transform.GetChild(0).GetChild(0).transform.Find(_prev).gameObject;
            }
            else _prev = "null";
            
            if (_isTook) //착용한 경우에만 history에 삽입
            {
                _clickItemControl._clickedHumanItem._pant = _new;
                _clickItemControl._clickedHumanItem._pantName = _new.name;
                HistoryController.pushDressChangeHist(_name, _prev, this.name, _hist._objectNumber, _hist._originNumber, _pos);
            }
        }
        else //shoes
        {
            if (_clickItemControl._clickedHumanItem._shoes != null)
            {
                _prev = _clickItemControl._clickedHumanItem._shoes.name;
                _clickItemControl._clickedHumanItem._shoes.SetActive(false);
                _prevButton = _DressContent .transform.GetChild(0).GetChild(0).transform.Find(_clickItemControl._clickedHumanItem._shoes.name).gameObject;
            }
            else _prev = "null";
            if (_name[1] == "normal")
            {
                _clickItemControl._clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                _clickItemControl._clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(false);
                Vector3 _tmp = _clickItemControl._clickedItem.item3d.transform.parent.transform.position;
                _tmp.y = _clickItemControl._clickedItem.item3d.GetComponent<ItemObject>()._humanInitPosition.y;
                _clickItemControl._clickedItem.item3d.transform.parent.transform.position = _tmp;

            }
            else
            {
                _clickItemControl._clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(false);
                _clickItemControl._clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(true);
                Vector3 _tmp= _clickItemControl._clickedItem.item3d.transform.parent.transform.position;
                _tmp.y = _clickItemControl._clickedItem.item3d.GetComponent<ItemObject>()._humanInitPosition.y + 1f;
                _clickItemControl._clickedItem.item3d.transform.parent.transform.position = _tmp;

            }
            if (_isTook)
            {
                _clickItemControl._clickedHumanItem._shoes = _new;
                _clickItemControl._clickedHumanItem._shoesName = _new.name;
                HistoryController.pushDressChangeHist(_name, _prev, this.name, _hist._objectNumber, _hist._originNumber, _originPos);
            }
        }
        _new.SetActive(true);
        _uiController._cloth = _new;
        _uiController.OnClickColorToggle();
        if (_isTook && _prevButton!=null) {
            _prevButton.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }
    }

    /*
     * date 2019.11.01
     * desc
     * 이벤트를 이용한 dress preview 기능 구현
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        //기존에 입고있던 object 저장
        if (_name[0].Equals("shirt"))
        {
            if (_clickItemControl._clickedHumanItem._shirt != null)
            {
                _prevName = _clickItemControl._clickedHumanItem._shirt.name;
            }
            else _prevName = "null";
        }
        else if (_name[0].Equals("pant"))
        {
            if (_clickItemControl._clickedHumanItem._pant != null)
            {
                _prevName = _clickItemControl._clickedHumanItem._pant.name;
            }
            else _prevName = "null";
        }
        else //shoes
        {
            if (_clickItemControl._clickedHumanItem._shoes != null)
            {
                _prevName = _clickItemControl._clickedHumanItem._shoes.name;
                string []_temp = _prevName.Split('_');
                _prevName = _temp[1] + "/" + _prevName;
            }
            else _prevName = "null";
        }
        if (!_prevName.Equals("null"))
        {
            _prevObject = _clickItemControl._clickedItem.item3d.transform.Find(_name[0]+"/"+ _prevName).gameObject;
        }
        dressChange(_newCloth);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isTook) { //임의로 착용한 옷을 해제하고 기존에 입고있던 옷 적용
            //임의로 착용한 옷 해제
            if (_name[0].Equals("shirt"))
            {
                _clickItemControl._clickedItem.item3d.transform.Find("shirt/" + this.name).gameObject.SetActive(false);
            }
            else if (_name[0].Equals("pant"))
            {
                _clickItemControl._clickedItem.item3d.transform.Find("pant/" + this.name).gameObject.SetActive(false);
            }
            else //shoes
            {
                _clickItemControl._clickedItem.item3d.transform.Find("shoes/" +_name[1]+"/"+ this.name).gameObject.SetActive(false);
            }
            //기존에 입고있던 옷 재적용
            if (!_prevName.Equals("null"))
            {
                dressChange(_prevObject);
            }
            else
            {
                if (_name[0].Equals("shoes")) _uiController.onShoseTookOff();
            }
        }
        //원래대로 되돌린다.
        _isTook = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isTook = true;
        this.transform.GetChild(1).GetComponent<Image>().sprite = _img;
        dressChange(_newCloth);
        _uiController.CheckUI();
    }
}