using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocateItem : MonoBehaviour
{

    /**
* date 2018.07.20
* author Lugub
* desc
*   기존 프로젝트에서 slot.cs에 있는 아이템 위치를 놓아주는 코드가 
*   아이템 위치를 재조정하는 코드와 겹치기 때문에 이를 중복하지 않기 위해서
*   새로운 스크립트를 만들어서 여기서 두가지를 다 처리하게끔 만들었다.
*/

    [Header("Locating Item")]
    public Item _placableItem;
    public GameObject _placableGameObject;
    public GameObject _SpriteObject;

    [Header("Script")]
    public GameObject _smallScheduler;
    public DressController _dressController;

    [Header("!Setting! Plane Size")]
    public Vector3 _vec3;

    private float _mouseWheelRotation;
    private ItemListControl _itemListControl;

    bool _reLocateSwi = false;

    public Vector3 _itemPosition;     /* 해당 객체의 위치를 저장 */
    public Vector3 _clickedItemSize;     /* 해당 크기의 사이즈를 저장 */

    private Vector3 point;

    [Header("Place")]
    public GameObject _inDoor;

    private void Start()
    {
        _itemListControl = GameObject.Find("ItemController").GetComponent<ItemListControl>();
    }

    private void Update()
    {
        if (_placableGameObject)
        {
            /* 물체를 생성해 줄 때, Sprite 이미지는 안보이도록 */
            _SpriteObject.SetActive(false);

            UIController._isMouseUsed = true;
            MoveCurrentPlacableObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
            //Debug.Log("LocateItem : " + _placableGameObject.name);
        }
    }

    /* 마우스로 위치를 조정하는 작업 */
    private void MoveCurrentPlacableObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo))
        {

            /* 인물 객체 위에 다른 객체들이 올리지 못하도록! */
            if (hitinfo.collider.tag == "Human") return;

            if (hitinfo.collider.tag == "Building") return;

            /* 벽걸이 물체 (tag=="HangItem") 이 아니면 벽에 걸지 못한다! */
            if (_placableGameObject.tag != "HangItem" && hitinfo.collider.tag == "Wall") return;

            point = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z);
            //Vector3 point = new Vector3(hitinfo.point.x, hitinfo.point.y + (_placableGameObject.transform.localScale.y/2), hitinfo.point.z);

            _placableGameObject.transform.position = point;
            //_placableGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);

        }
    }

    /* 마우스 휠로 물체의 회전값을 조정하는 작업*/
    private void RotateFromMouseWheel()
    {
        _mouseWheelRotation += Input.mouseScrollDelta.y;

        //Debug.Log("LocateItem.cs 휠로 돌리는 아이템 이름 = " + _placableGameObject);
        /*휠로 돌릴때 사람의 경우에는 해당 본 객체를 돌려줘야해서 _item의 첫자식을 회전*/
        if (_placableItem._originNumber >= 2000 && _placableItem._originNumber <= 2005)
        {
            _placableGameObject.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, _mouseWheelRotation * 15f, 0);
        }
        else
        {
            _placableGameObject.transform.localRotation = Quaternion.Euler(0, _mouseWheelRotation * 15f, 0);
        }
    }

    /* 놓은 장소에 물체 생성하는 과정*/
    private void ReleaseIfClicked()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            /* 마우스 작업상태 -> false 로 전환해준다! */
            UIController._isMouseUsed = false;

            /* 벽 밖에 포인터를 갖다대면, 그 위치로 조정 못하도록 */
            /* 내부에서만 */
            if (_inDoor.activeSelf == true)
            {
                _clickedItemSize = _placableGameObject.transform.GetChild(0).GetComponent<Collider>().bounds.size;
                if (point.x + _clickedItemSize.x / 2 > _vec3.x) { _placableGameObject.transform.position = new Vector3(_vec3.x - _clickedItemSize.x / 2, _placableGameObject.transform.position.y, _placableGameObject.transform.position.z); }
                if (point.x - _clickedItemSize.x / 2 < -_vec3.x) { _placableGameObject.transform.position = new Vector3(-_vec3.x + _clickedItemSize.x / 2, _placableGameObject.transform.position.y, _placableGameObject.transform.position.z); }
                if (point.z + _clickedItemSize.z / 2 > _vec3.z) { _placableGameObject.transform.position = new Vector3(_placableGameObject.transform.position.x, _placableGameObject.transform.position.y, _vec3.z - _clickedItemSize.z / 2); }
            }

            /* 본체의 위치 - 중심점을 만들기 위하여 사용한 객체가 아니라 실제 객체*/
            _placableGameObject = _placableGameObject.transform.GetChild(0).gameObject;

            /* 설치한 오브젝트의 layer를 Default로 변경, 이걸 해야 클릭을 하든 뭘 하든 할 수가 있음 */
            _placableGameObject.layer = LayerMask.NameToLayer("Default");

            /* 재조정하는 물체인지 아닌지 판별 */
            /* 새로 위치를 잡는 아이템인 경우 */
            if (!_reLocateSwi)
            {
                /*생성된 오브젝트에 ItemObject라는 스크립트 추가*/
                _placableGameObject.AddComponent<ItemObject>();

                /* 빠른 연결을 위한 캐싱 작업 */
                ItemObject _tmp = _placableGameObject.GetComponent<ItemObject>();

                /* 만들어진 객체가 사람 일때 -> HumanItem.cs 추가 생성 */
                if (_placableItem._originNumber >= 2000 && _placableItem._originNumber < 3000)
                {
                    /* _tmpItem을 새로 선언해 주고 연결해준다. */
                    HumanItem _tmpHuman = new HumanItem("Idle", _itemListControl._humanDBIndex);

                    /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
                    _tmp._thisHuman = _tmpHuman;

                    /* 생성한 사람 객체를 ItemListControl.cs에 있는 HumanList에 저장. ItemListControl.cs 의 _dataBaseHumanItem 변수와 관련 */
                    _itemListControl.AddHumanDB(_tmpHuman);


                }

                /* _tmpItem을 새로 선언해 주고 연결해준다. */
                /*사람객체를 생성할때는 오브젝트 넘버를 사람넘버에서 따와서 넣어준다.*/
                Item _tmpItem;
                if (_placableItem._originNumber >= 2000 && _placableItem._originNumber < 2005)
                {
                    _tmpItem = new Item(_placableItem.itemName, _itemListControl._humanDBIndex, _placableItem._originNumber, _placableGameObject);
                }
                else
                {
                    _tmpItem = new Item(_placableItem.itemName, _itemListControl._itemDBIndex, _placableItem._originNumber, _placableGameObject);
                }


                /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
                _tmp._thisItem = _tmpItem;

                if (_placableItem._originNumber < 2000 || _placableItem._originNumber > 2005) //사람이 아닐때
                {
                    /* 생성한 객체를 ItemListControl.cs에 있는 List에 저장. ItemListControl.cs 의 _dataBaseItem 변수와 관련 */
                    _itemListControl.AddDB(_tmpItem);
                }
                else
                {
                    _itemListControl.AddHuman(_tmpItem);

                    /*
                    *   History
                    *   date   : 2020-02-10
                    *   author : skyde47
                    *   desc : HumanItem 갱신
                    */

                    HumanItem _tmpHuman = _tmp._thisHuman;
                    Transform _quick = _tmpItem.item3d.transform.Find("shirt");
                    //shirt의 자식 객체를 탐색하며 활성화되어있는 객체 저장
                    for (int i = 0; i < _quick.childCount; i++)
                    {
                        if (_quick.GetChild(i).gameObject.activeSelf)
                        {
                            _tmpHuman._shirt = _quick.GetChild(i).gameObject;
                            _tmpHuman._shirtName = _tmpHuman._shirt.name;
                        }
                    }
                    //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
                    _quick = _tmpItem.item3d.transform.Find("pant");
                    for (int i = 0; i < _quick.childCount; i++)
                    {
                        if (_quick.GetChild(i).gameObject.activeSelf)
                        {
                            _tmpHuman._pant = _quick.GetChild(i).gameObject;
                            _tmpHuman._pantName = _tmpHuman._pant.name;
                        }
                    }
                    //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
                    if (_tmpItem.item3d.transform.Find("shoes").GetChild(0).gameObject.activeSelf) //현재 발 상태가 normal
                    {
                        _quick = _tmpItem.item3d.transform.Find("shoes");
                        for (int i = 0; i < _quick.GetChild(0).childCount; i++)
                        {
                            //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                            if (_quick.GetChild(0).GetChild(i).gameObject.activeSelf)
                            {
                                _tmpHuman._shoes = _quick.GetChild(0).GetChild(i).gameObject;
                                _tmpHuman._shoesName = _tmpHuman._shoes.name;
                            }
                        }
                    }
                    else //abnormal
                    {
                        _quick = _tmpItem.item3d.transform.Find("shoes");
                        for (int i = 0; i < _quick.GetChild(1).childCount; i++)
                        {
                            //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                            if (_quick.GetChild(1).GetChild(i).gameObject.activeSelf)
                            {
                                _tmpHuman._shoes = _quick.GetChild(1).GetChild(i).gameObject;
                                _tmpHuman._shoesName = _tmpHuman._shoes.name;
                            }
                        }
                    }
                }


                /* Item의 생성번호를 부여 */
                if (_placableItem._originNumber >= 2000 && _placableItem._originNumber < 2005)
                {
                    _tmp.GetComponent<ItemObject>()._thisItem._objectNumber = _itemListControl._humanDBIndex;
                    HistoryController.pushObjectCreateHist(_placableGameObject, _placableItem._originNumber, _itemListControl._humanDBIndex);
                }
                else
                {
                    _tmp.GetComponent<ItemObject>()._thisItem._objectNumber = _itemListControl._itemDBIndex;
                    HistoryController.pushObjectCreateHist(_placableGameObject, _placableItem._originNumber, _itemListControl._itemDBIndex);
                }


                /* 윤곽선 안보이게 처리! */
                //_placableGameObject.GetComponent<Outline>().enabled = false;

                if (_placableItem._originNumber >= 2000 && _placableItem._originNumber < 2005)
                {
                    /* 스몰 스케줄러에 각 Item 값을 채워줌. */
                    Item temp = _tmp.GetComponent<ItemObject>()._thisItem;
                    Transform smallBar = _smallScheduler.transform.Find(temp.itemName + temp._objectNumber);
                    smallBar.GetComponent<SmallSchedulerBar>()._object = _tmpItem;


                }
            }
            /* 위치를 재조정하는 아이템인 경우 */
            else
            {
                _reLocateSwi = false;

                /* 윤곽선 보이게 처리! -> 위치 재조정 해도 클릭된 아이템은 여전하므로 */
                //_placableGameObject.GetComponent<Outline>().enabled = true;

                /* Sprite 이미지가 해당 위치로 이동하도록 설정 */
                _SpriteObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 30.0f, Input.mousePosition.z);

                _SpriteObject.SetActive(true);

                ///* 위치 설정 하는 객체가 사람일 경우 줌 인 필요! */
                //if (_placableItem._originNumber >= 2000 && _placableItem._originNumber < 3000)
                //{
                //    Static.STATIC.cameraMoveAroun.CameraZoomIn(_placableGameObject.transform.position);
                //}
            }

            _placableGameObject.GetComponent<ItemObject>()._humanInitPosition = point;
            _placableGameObject.GetComponent<ItemObject>()._humanInitRotation = _placableGameObject.transform.rotation.eulerAngles;

            /* 위치 조정을 마쳤으면 이제 null 선언으로 끊어준다. */
            _placableGameObject = null;
            _placableItem = null;

        }
        /*
     History
      date   : 2018-11-26
      author : Lugup
      내  용 : Item Relocate... 하는 스크립트였으나 
      실행시 : 클릭한 객체의 위치를 기존의 위치에서 새로운 위치로 조정함
      취소시 : 클릭했던 객체의 위치를 새로운 위치에서 기존의 위치로 조정

      */
    }

    /* clickedMenu에서 위치를 재조정하고자 할때 누르는 버튼 */
    public void RelocateItem(Item _clickedItem)
    {
        _placableItem = _clickedItem;
        _placableGameObject = _clickedItem.item3d.transform.parent.gameObject;
        _clickedItemSize = _clickedItem.item3d.GetComponent<Collider>().bounds.size;
        _itemPosition = _clickedItem.item3d.transform.position;

        /* 객체의 위치와, 발판의 위치가 다를 시 위치 조정 -> 인물 객체만 움직여 위치 변경 할 수 있으므로 if문 넣어줌. */
        if (_placableItem._originNumber >= 2000) _placableItem.item3d.transform.position = _placableGameObject.transform.position;
        _reLocateSwi = true;

        /*
    History
    date   : 2018-11-26
    author : Lugup
    내  용 : Item Relocate... 하는 스크립트였으나 
    실행시 : 클릭한 객체의 위치를 기존의 위치에서 새로운 위치로 조정함
    취소시 : 클릭했던 객체의 위치를 새로운 위치에서 기존의 위치로 조정

*/
    }


    /* <summary>
 Load 버튼을 누를 경우 SQLlite에 있는 db값을 바탕으로 아이템을 하나씩하나씩 소환하는 함수
 </summary>*/
    /// <param name="_loadItem">        로드할 객체    </param>
    /// <param name="_currentPosition"> 객체가 소환될 위치  </param>
    /// <param name="_currentRotate">   객체가 가질 회전 값 </param>

    public void LoadItem(int _originNumber, Item _loadItem, string _objectName, Vector3 _currentPosition, Vector3 _currentRotate, Vector3 _currentScale)
    {
        GameObject _loadObject = Instantiate(_loadItem.item3d) as GameObject;
        _loadObject.transform.SetParent(_inDoor.transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(_objectName, _itemListControl._itemDBIndex, _originNumber, _loadObject);

        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        _itemListControl.AddDB(_tmpItem);

        _tmp.GetComponent<ItemObject>()._thisItem._objectNumber = _itemListControl._itemDBIndex;


        // _loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        // _loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = _currentPosition;

        /* 회전값 설정 */
        _loadObject.transform.parent.Rotate(_currentRotate);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = _currentScale;
    }
    public void LoadWall(int _originNumber, Item _loadItem, string _objectName, Vector3 _currentPosition, Vector3 _currentRotate, Vector3 _currentScale, int _placeNumber, float _tilingX, float _tilingY)
    {
        /* 게임 오브젝트 생성 */
        GameObject _loadObject = Instantiate(_loadItem.item3d);

        /* 부모 지정 */
        _loadObject.transform.SetParent(_inDoor.transform.GetChild(0));

        /* 자식으로 변경 */
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        /* ItemObject 스크립트 추가 */
        ItemObject _tmp = _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        Item _tmpItem = new Item(_objectName, _itemListControl._wallDBIndex + 1, _originNumber, _loadObject);

        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;

        /* DB에 삽입 */
        _itemListControl.AddWall(_tmpItem);

        /* PlaceNumber 지정 */
        _tmp._placeNumber = _placeNumber;

        /* 이름 지정 */
        _loadObject.transform.parent.name = _loadObject.transform.parent.name.Substring(0, _loadObject.transform.parent.name.Length - 7);

        /* 위치 조정 */
        _loadObject.transform.parent.position = _currentPosition;

        /* 회전 조절*/
        _loadObject.transform.parent.Rotate(_currentRotate);

        /* 크기 지정*/
        _loadObject.transform.parent.localScale = _currentScale;

        /* Sprite 복사 */
        Sprite _tmpSprite = _itemListControl.GetImages(3, _placeNumber);

        /* Sprite가 존재하면 */
        if (_tmpSprite != null)
            /* Texture 지정 */
            _loadObject.GetComponent<MeshRenderer>().material.mainTexture = Instantiate(_tmpSprite).texture;

        /* Tiling 값 지정 */
        _loadObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(_tilingX, _tilingY);
    }

    public void LoadHuman(int _originNumber, Item _loadItem, string _objectName, Vector3 _currentPosition, Vector3 _currentRotate, Vector3 _currentScale, int _humanNumber)
    {
        GameObject _loadObject = Instantiate(_loadItem.item3d) as GameObject;
        _loadObject.transform.SetParent(_inDoor.transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(_objectName, _itemListControl._itemDBIndex, _originNumber, _loadObject);
        _tmpItem._loadHumanNumber = _humanNumber;
        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        _tmp._thisItem.itemName = _objectName;
        _tmp._humanInitPosition = _currentPosition;
        _tmp._humanInitRotation = _currentRotate;

        _itemListControl.AddHuman(_tmpItem);

        _tmp.GetComponent<ItemObject>()._thisItem._objectNumber = _itemListControl._humanDBIndex;
        _tmp.GetComponent<ItemObject>()._thisHuman = new HumanItem("Idle", _itemListControl._humanDBIndex - 1);

        // _loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        // _loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = _currentPosition;



        /* 회전값 설정 */
        _loadObject.transform.Rotate(_currentRotate);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = _currentScale;

        /* 스케줄러 생성 파트*/
        int _objectNumber = _itemListControl._humanDBIndex;
        //스몰바 생성 및 설정
        GameObject _smallBar = Instantiate(Resources.Load("Prefab/SmallScheduler_Sample")) as GameObject;
        if (_originNumber == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _smallBar.name = "Man" + (_objectNumber); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; //스몰바의 텍스트설정
        }
        else if (_originNumber == 2000)
        {
            _smallBar.name = "Daughter" + (_objectNumber); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //스몰바의 텍스트설정
        }
        else if (_originNumber == 2002)
        {
            _smallBar.name = "Woman" + (_objectNumber); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //스몰바의 텍스트설정
        }
        else if (_originNumber == 2003)
        {
            _smallBar.name = "Woongin" + (_objectNumber); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //스몰바의 텍스트설정
        }
        //content 설정
        GameObject _smallContent;
        _smallContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;

        _smallBar.transform.SetParent(_smallContent.transform); //small Bar의 부모설정
        _smallBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //small Bar의 정보
        _smallBar.GetComponent<SmallSchedulerBar>()._objectNumber = _objectNumber; //오브젝트넘버를 small bar에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._originNumber = _originNumber; //오리진 정보또한 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._humanNumber = _humanNumber;
        _smallBar.GetComponent<SmallSchedulerBar>()._object = _tmpItem;

        //빅바 생성 및 설정
        GameObject _bigBar = Instantiate(Resources.Load("Prefab/BigScheduler_Sample")) as GameObject;
        if (_originNumber == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _bigBar.name = "Man" + (_objectNumber); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //빅바의 텍스트설정
        }
        else if (_originNumber == 2000)
        {
            _bigBar.name = "Daughter" + (_objectNumber); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //빅바의 텍스트설정
        }
        else if (_originNumber == 2002)
        {
            _bigBar.name = "Woman" + (_objectNumber); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //빅바의 텍스트설정
        }
        else
        {
            _bigBar.name = "Woongin" + (_objectNumber); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + _objectName + _objectNumber; ; //빅바의 텍스트설정
        }
        //content 설정
        GameObject _bigContent;
        _bigContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        _bigBar.transform.SetParent(_bigContent.transform); //big Bar의 부모설정
        _bigBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //big Bar의 정보
        _bigBar.GetComponent<BigSchedulerBar>()._objectNumber = _objectNumber; //오브젝트넘버를 small bar에 저장
        _bigBar.GetComponent<BigSchedulerBar>()._originNumber = _originNumber; //오리진 정보또한 저장
        _bigBar.GetComponent<BigSchedulerBar>()._humanNumber = _humanNumber; //휴먼넘버를 저장
        //생성된 빅바를 스몰바에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._bigScheduler = _bigBar;
        //big Bar는 우선 안보이게 처리
        _bigBar.SetActive(false);
        _itemListControl._dataBaseBigBar.Add(_bigBar);
        _itemListControl._dataBaseSmallbar.Add(_smallBar);
    }
}
