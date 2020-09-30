using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /**
* date 2018.07.12
* author Lugub
* desc
*  아이템창의 아이템칸을 눌렀을 때 그 아이템이 나오게 끔 만들고
*  또한 아이템의 정보를 저장
*/

    /**
* date 2018.11.15
* author INHO
* desc
* Slot.cs은 사물 객체를 생성할 때 쓰는 스크립트.
* HumanSlot.cs 은 사람 객체를 생성할 때 쓰는 스크립트로 나눔.
*/
    [Header("Item Information")]
    public Item _thisItem;
    private GameObject _currentPlacableObject;
    public bool _isItem;
    public bool _isHouse;
    public bool _isBuild;
    public bool _isPlace;

    [Header("Place Information")]
    public PlaceController _placeController;

    [Header("Scripts")]
    public ClickedItemControl _clickedItemControl;
    public LocateItem _locateItem;
    public readTextFiles _readTextFiles;
    public ItemListControl _itemListControl;

    [Header("Place")]
    public GameObject _inDoor;
    public GameObject _simpleHouse;
    public CameraMoveAroun _cameraMoveAroun;

    string dir_path;
    /**
* date 2018.07.18
* author Lugub
* desc
*   코드상 복잡한 부분 - float x,y,z 와 rot_x,y,z를 전부 벡터로써 변환하는 작업을 함
*/
    /**
* date 2018.07.19
* author Lugub
* desc
*   코드상 복잡한 부분 - float x,y,z 와 rot_x,y,z를 전부 벡터로써 변환하는 작업을 함
*/

    /**
    * date 2018.07.20
    * author Lugub
    * desc
    *   물체의 위치를 옮기는 스크립트 LocateItem.cs를 하나 만들어서
    *   물체의 위치를 옮기는 작업 - ReLocate, Locate 를 동시에 그 스크립트에서 처리하게끔함
    *   
*/

    /* 또한 동적인 부분이기 때문에 GameObject.Find사용*/
    public void Start()
    {
        _locateItem = GameObject.Find("ItemController").GetComponent<LocateItem>();
        _readTextFiles = Static.STATIC.readTextFiles;
        dir_path = Static.STATIC.dir_path;
    }

    /*이 슬롯을 클릭 했을 경우*/
    public void OnclickThisSlot()
    {
        _currentPlacableObject = Instantiate(_thisItem.item3d) as GameObject;

        /* 내부 씬의 자식으로 설정 */
        _currentPlacableObject.transform.SetParent(_inDoor.transform);

        _locateItem._placableItem = _thisItem;
        _locateItem._placableGameObject = _currentPlacableObject;
        _locateItem._clickedItemSize = _currentPlacableObject.transform.GetChild(0).GetComponent<Collider>().bounds.size;

        /* 외곽선 Scripts 추가하기 */
        //_currentPlacableObject.transform.GetChild(0).gameObject.AddComponent<Outline>();

        /* 외곽선 처리 - 이전에 클릭됬었던 객체는 안보이게 */
        //if (_clickedItemControl._clickedItem != null) _clickedItemControl._clickedItem.item3d.GetComponent<Outline>().enabled = false;

        /* 메뉴 리셋 */
        _clickedItemControl.BasePanelReset();
        _clickedItemControl.ResetClickMenu();

        /*
        History
        date   : 2018-11-26
        author : Lugup
        내  용 : Item created
        실행시 : 객체 생성
        취소시 : 객체 제거

        */
    }

    /*이 장소를 클릭 했을 경우*/
    public void OnClickThisPlace()
    {
        if (_placeController._clickedPlace != null)
        {
            /* 클릭된 Place 객체의 MeshRenderer 컴포넌트를 담음 */
            MeshRenderer _clickedMeshRenderer = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>();

            /* Slot의 OriginNumber에 해당하는 Sprite 정보를 가져옴 */
            Sprite _tmp = _itemListControl.GetImages(3, _thisItem._originNumber);

            /* 존재하지 않는 Sprite이면 */
            if(_tmp == null)
            {
                /* 건물의 OriginNumber를 저장 */
                int _buildingOriginNumber = _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber;

                /* 건물들의 3DObject가 들어있는 디렉터리 경로 지정 */
                System.IO.DirectoryInfo _dir = new System.IO.DirectoryInfo(dir_path + "/Resources/Item/Wall/Build");

                /* 디렉터리 탐색 */
                foreach(System.IO.FileInfo _file in _dir.GetFiles())
                {
                    /* 파일의 OriginNumber와 일치하면 */
                    if (_file.Name.Split('_')[0] == _buildingOriginNumber.ToString() && _file.Extension.ToLower() == ".prefab")
                    {
                        /* Resource를 통해 건물 정보 저장 */
                        GameObject _building = Resources.Load<GameObject>("Item/Wall/Build/" + _file.Name.Substring(0, _file.Name.Length - 7));

                        /* 머터리얼을 복제해서 클릭된 건물의 머터리얼로 지정 */
                        _clickedMeshRenderer.material = Instantiate(_building.GetComponent<Transform>().GetChild(0).GetComponent<MeshRenderer>().sharedMaterial);

                        break; //반복문 빠져나옴
                    }
                }
            }

            /* 존재하는 Sprite이면 */
            else
            {
                /* Sprite 복제 */
                Sprite _tmpClone = Instantiate(_tmp);

                /* Texture로 적용 */
                _clickedMeshRenderer.material.mainTexture = _tmpClone.texture;
            }

            /* 클릭된 건물의 ItemObject에 PlaceNumber 지정 */
            HistoryController.pushTileHist(_placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._thisItem._originNumber, _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._thisItem._objectNumber, _placeController._clickedPlace.transform.GetChild(0).GetComponent<ItemObject>()._placeNumber);
            _placeController._clickedPlace.GetComponent<Transform>().GetChild(0).GetComponent<ItemObject>()._placeNumber = _thisItem._originNumber;
        }
    }

    /* Build 버튼이 클릭되면 실행되는 함수 */
    public void OnClickBuildButton()
    {
        /* 이동할 건물 생성(지정) */
        _currentPlacableObject = Instantiate(_thisItem.item3d) as GameObject;

        /* 이름 지정 */
        _currentPlacableObject.name = _currentPlacableObject.name.Substring(0, _currentPlacableObject.name.Length - 7);

        /* 부모 지정 */
        _currentPlacableObject.GetComponent<Transform>().SetParent(_simpleHouse.GetComponent<Transform>());

        /* RayCast 무시 */
        _currentPlacableObject.GetComponent<Transform>().GetChild(0).gameObject.layer = 2;

        /* 카메라 움직일 수 없음 */
        _cameraMoveAroun._cameraAroun = false;

        /* 기준 Plane 활성화 */
        _placeController._measurePlane.SetActive(true);

        /* 현재 건물을 배치 */
        _placeController._locatedBuilding = _currentPlacableObject;

        /* Item 정보를 넘김 */
        _placeController._locatedBuildingItem = _thisItem;

        /* 새로운 건물을 배치한다고 알림 */
        _placeController._isRelocated = false;
    }

    /* Place 버튼이 클릭되면 실행되는 함수 */
    public void OnClickPlaceButton()
    {
        /* 장소 개수만큼 반복 */
        for (int i = 0; i < _inDoor.transform.childCount; i++)
        {
            if (_inDoor.transform.GetChild(i).name.Equals(this.name))
            {
                _inDoor.transform.GetChild(i).gameObject.SetActive(true);
                if (this.name.Equals("SimpleHouse"))
                {
                    Static.STATIC._isSimpleHouse = 1;
                    Static.STATIC._isHouse = 0;
                    Static.STATIC._isCar = 0;
                }
                else if (this.name.Equals("House"))
                {
                    Static.STATIC._isSimpleHouse = 0;
                    Static.STATIC._isHouse = 1;
                    Static.STATIC._isCar = 0;
                }
                else if (this.name.Equals("Car"))
                {
                    Static.STATIC._isSimpleHouse = 0;
                    Static.STATIC._isHouse = 0;
                    Static.STATIC._isCar = 1;
                }
            }
            else
            {
                _inDoor.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isBuild || _isHouse) return; //집이나 건물 제작 버튼일 경우 무시
        /* 해당 name (key값) 에 대한 설명을 띄워주도록 넘겨준다. */
        _readTextFiles._printName = _thisItem.item3d.name;
        _readTextFiles._isON = true;
        _readTextFiles.isItem = _isItem;
        _readTextFiles.isPlace = _isPlace;
        _readTextFiles.isHuman = false;
        _readTextFiles.GetButtonPosition(this.transform.position.x);
        if (_thisItem.item3d != null)
            _readTextFiles.BackGroundService(_thisItem.item3d);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isBuild || _isHouse) return; //집이나 건물 제작 버튼일 경우 무시
        /* 마우스 포인터 밖으로 나가면, 초기화 시켜주기 */
        _readTextFiles._printName = "";
        _readTextFiles._isON = false;
        _readTextFiles.BackGroundServiceOff();
    }
}