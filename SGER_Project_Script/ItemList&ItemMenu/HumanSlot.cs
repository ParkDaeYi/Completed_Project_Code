using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HumanSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /**
* date 2018.11.15
* author INHO
* desc
* Slot.cs은 사물 객체를 생성할 때 쓰는 스크립트.
* HumanSlot.cs 은 사람 객체를 생성할 때 쓰는 스크립트로 구분지음.
*/
    [Header("Item Information")]
    public Item _thisItem;
    private GameObject _currentPlacableObject;

    [Header("Scripts")]
    public ClickedItemControl _clickedItemControl;
    public ItemListControl _itemListControl;

    [Header("BigScheduler")]
    public GameObject _bigContent;
    [Header("SmallScheduler")]
    public GameObject _smallContent;

    [Header("SchdulerInfo")]
    public int _objectNum;
    public int _originNum;

    [Header("Place")]
    public GameObject _inDoor;

    private readTextFiles _readTextFiles;
    private LocateItem _locateItem;

    /* 또한 동적인 부분이기 때문에 GameObject.Find사용*/
    public void Awake()
    {
        /* static 방식을 이용해, Start 부문에서 동적으로 연결시킴. */
        _readTextFiles = Static.STATIC.readTextFiles;
    }

    public void Start()
    {
        _locateItem = GameObject.Find("ItemController").GetComponent<LocateItem>();
    }

    /* 이 슬롯을 클릭 했을 경우 -> 동적으로 3D 객체 및 정보를 생성해준다. */
    public void OnclickThisSlot()
    {
        _currentPlacableObject = Instantiate(_thisItem.item3d) as GameObject;

        /* 내부 씬의 자식으로 설정 */
        _currentPlacableObject.transform.SetParent(_inDoor.transform);

        _locateItem._placableItem = _thisItem;
        

        /* 사람 객체이므로, 사람의 정보값이 든 Item.cs 을 상속받은 HumanItem.cs를 추가시킨다. */

        _locateItem._placableGameObject = _currentPlacableObject;
        _locateItem._clickedItemSize = _currentPlacableObject.transform.GetChild(0).GetComponent<Collider>().bounds.size;

        /* 외곽선 Scripts 추가하기 */
        //_currentPlacableObject.transform.GetChild(0).gameObject.AddComponent<Outline>();

        /* 외곽선 처리 - 이전에 클릭됬었던 객체는 안보이게 */
        //if (_clickedItemControl._clickedItem != null) _clickedItemControl._clickedItem.item3d.GetComponent<Outline>().enabled = false;

        /* 메뉴 리셋 */
        _clickedItemControl.BasePanelReset();
        _clickedItemControl.ResetClickMenu();

        //생성한 사람객체의 정보를 Static의 사람객체 정보에 저장
        Static.STATIC._humanArray.Add(_currentPlacableObject);




        /*
     History
      date   : 2018-11-26
      author : Lugup
      내  용 : Human Item created
      실행시 : 사람 객체 생성
      취소시 : 생성된 사람 객체 제거

      */

        /*
         * date : 2018.12.11
         * author : geun
         * 내용 : 사람을 생성할 때 해당 사람에 대한 스케줄 바(스몰바, 빅바) 생성
         * History
         * 실행시 : 사람객체에 따른 스몰바, 빅바 생성
         * 취소시 : 생성된 스몰바, 빅바 제거
         */
        Debug.Log("HumanSlot.cs 84줄 / 스케줄바 생성 실행");
        // 오브젝트 넘버를 가져옴
        _objectNum = GameObject.Find("Controllers").transform.GetChild(2).transform.GetComponent<ItemListControl>()._humanDBIndex;
        _originNum = _thisItem._originNumber;
        Debug.Log("HumanSlot.cs 88줄 / 생성된 사람객체의 오리진넘버 = " + _originNum);

        //스몰바 생성 및 설정
        GameObject _smallBar = Instantiate(Resources.Load("Prefab/SmallScheduler_Sample")) as GameObject;
        if (_originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _smallBar.name = "Man" + (_objectNum + 1); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Man" + (_objectNum + 1); //스몰바의 텍스트설정
        }
        else if (_originNum == 2000)
        {
            _smallBar.name = "Daughter" + (_objectNum + 1); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Daughter" + (_objectNum + 1);
        }
        else if (_originNum == 2002)
        {
            _smallBar.name = "Woman" + (_objectNum + 1); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Woman" + (_objectNum + 1);
        }
        else if (_originNum == 2003)
        {
            _smallBar.name = "Woongin" + (_objectNum + 1); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Woongin" + (_objectNum + 1);
        }
        //content 설정
        _smallContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;

        _smallBar.transform.SetParent(_smallContent.transform); //small Bar의 부모설정
        _smallBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //small Bar의 정보
        _smallBar.GetComponent<SmallSchedulerBar>()._objectNumber = _objectNum; //오브젝트넘버를 small bar에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._originNumber = _originNum; //오리진 정보또한 저장

        //빅바 생성 및 설정
        GameObject _bigBar = Instantiate(Resources.Load("Prefab/BigScheduler_Sample")) as GameObject;
        if (_originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _bigBar.name = "Man" + (_objectNum + 1); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Man" + (_objectNum + 1); //빅바의 텍스트설정
        }
        else if (_originNum == 2000)
        {
            _bigBar.name = "Daughter" + (_objectNum + 1); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Daughter" + (_objectNum + 1);
        }
        else if(_originNum == 2002)
        {
            _bigBar.name = "Woman" + (_objectNum + 1); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Woman" + (_objectNum + 1);
        }
        else if (_originNum == 2003)
        {
            _bigBar.name = "Woongin" + (_objectNum + 1); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "Woongin" + (_objectNum + 1);
        }
        //content 설정
        _bigContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        _bigBar.transform.SetParent(_bigContent.transform); //big Bar의 부모설정
        _bigBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //big Bar의 정보
        _bigBar.GetComponent<BigSchedulerBar>()._objectNumber = _objectNum; //오브젝트넘버를 small bar에 저장
        _bigBar.GetComponent<BigSchedulerBar>()._originNumber = _originNum; //오리진 정보또한 저장
        //생성된 빅바를 스몰바에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._bigScheduler = _bigBar;
        //big Bar는 우선 안보이게 처리
        _bigBar.SetActive(false);

        _itemListControl._dataBaseBigBar.Add(_bigBar);
        _itemListControl._dataBaseSmallbar.Add(_smallBar);
    }
    /*이 휴대 물건을 클릭 했을 경우*/
    public void OnClickHandItem()
    {
        /* 추후 개발 코딩 필요 */
    }

    /* OnPointer 함수는, 마우스를 갖다 대면 설명창이 뜰 수 있도록 구현해주는 Methods. */
    public void OnPointerEnter(PointerEventData eventData)
    {
        /* 해당 name (key값) 에 대한 설명을 띄워주도록 넘겨준다. */
        _readTextFiles._printName = _thisItem.item3d.name;
        _readTextFiles._isON = true;
        _readTextFiles.isItem = false;
        _readTextFiles.isPlace = false;
        _readTextFiles.isHuman = true;
        _readTextFiles.GetButtonPosition(this.transform.position.x);
        _readTextFiles.BackGroundService(_thisItem.item3d);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        /* 마우스 포인터 밖으로 나가면, 초기화 시켜주기 */
        _readTextFiles._printName = "";
        _readTextFiles._isON = false;
        _readTextFiles.BackGroundServiceOff();
    }
}
