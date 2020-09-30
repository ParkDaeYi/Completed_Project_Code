using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PlaceController : MonoBehaviour
{
    /**
* date 2019.07.29
* author GS
* desc
* 집 오브젝트를 변경하고 클릭된 Plane 오브젝트의 Sprite 이미지를 변경하는 역할을 하는 스크립트.
* Slot이 클릭되면 _clickedPlace의 텍스쳐가 Slot의 _thisItem의 Sprite 이미지로 변경.
* SimpleHouse 오브젝트를 에디팅 하는 데에도 사용된다.
*/

    [Header("Variable")]
    //public GameObject[] _places; //장소들
    public GameObject _clickedPlace; //클릭된 Place 오브젝트
    public GameObject _locatedBuilding; //이동 중인 건물
    public bool _isRelocated; //최초 건물 배치인지 재배치인지 구분하는 변수
    public Item _locatedBuildingItem; //최초로 배치되는 건물의 Item 정보

    [Header("Object")]
    //public GameObject _inDoor; //내부
    public Camera _mainCamera; //카메라 오브젝트
    public GameObject _simpleHouse; //SimpleHouse 오브젝트
    public GameObject _measurePlane; //바닥의 기준이 되는 Plane

    [Header("UI")]
    //public Transform _placeItemContent; //부모로 지정할 PlaceItemContent
    //public Transform _buildItemContent; //부모로 지정할 BuildItemContent
    public Button _planeButton; //텍스처를 변경하는 Plane 버튼
    public Button _buildButton; //건물을 생성하는 build 버튼

    //[Header("Sample")]
    //public GameObject _placeButtonSample; //Place 버튼 샘플
    //public GameObject _buildButtonSample; //Build 버튼 샘플

    [Header("Script")]
    public CameraMoveAroun _cameraMoveAroun; //카메라 관련 스크립트
    public ClickedPlaceControl _clickedPlaceControl; //클릭된 장소를 관리하는 스크립트
    public ItemListControl _itemListControl; //아이템을 총괄하는 스크립트

    public void Start()
    {
        //ResetPlaceButtons();
        //ResetBuildButtons();
        _measurePlane.SetActive(false); //기준 Plane 초기 비활성화
    }

    ///* Place Button들을 초기화하는 함수 */
    //private void ResetPlaceButtons()
    //{
    //    /* 장소 개수만큼 배열 크기 조정 */
    //    _places = new GameObject[_inDoor.transform.childCount];

    //    /* 장소 개수만큼 반복 */
    //    for (int i = 0; i < _places.Length; i++)
    //    {
    //        /* 장소들을 차례대로 변수에 담음 */
    //        _places[i] = _inDoor.transform.GetChild(i).gameObject;

    //        /* 버튼 생성*/
    //        GameObject _button = Instantiate(_placeButtonSample);

    //        /* 버튼 활성화 */
    //        _button.SetActive(true);

    //        /* 버튼 부모 지정 */
    //        _button.transform.SetParent(_placeItemContent);

    //        /* 버튼 크기 지정 */
    //        _button.transform.localScale = new Vector3(1f, 1f, 1f);

    //        /* 버튼 이름 지정 */
    //        _button.name = _places[i].name;

    //        /* 버튼 텍스트 지정 */
    //        _button.transform.GetChild(0).GetComponent<Text>().text = _places[i].name;

    //        /* 해당 버튼이 클릭되면 이름을 인자 값으로 넘기면서 OnClickPlaceButton 함수 실행 */
    //        _button.GetComponent<Button>().onClick.AddListener(() => OnClickPlaceButton(_button.name));
    //    }
    //}

    ///* Build 버튼들을 초기화하는 함수 */
    //private void ResetBuildButtons()
    //{
    //    /* Build 폴더의 정보를 가져옴 */
    //    DirectoryInfo _buildFolder = new DirectoryInfo("Assets/Resources/Item/Place/Build");

    //    /* 폴더 탐색 */
    //    foreach (FileInfo _file in _buildFolder.GetFiles())
    //    {
    //        /* 프리팹 확장자이면 */
    //        if (_file.Extension.ToLower() == ".prefab")
    //        {
    //            /* '_' 기준으로 이름 분리 */
    //            string[] _name = _file.Name.Split('_');

    //            /* 복제된 버튼 */
    //            GameObject _cloneButton = Instantiate(_buildButtonSample);

    //            /* 활성화 */
    //            _cloneButton.SetActive(true);

    //            /* 부모 변경 */
    //            _cloneButton.GetComponent<Transform>().SetParent(_buildItemContent);

    //            /* 크기 조절 */
    //            _cloneButton.GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);

    //            /* 이름 변경 */
    //            _cloneButton.name = _name[2].Substring(0, _name[2].Length - 7) + "Button";

    //            /* 내부 Text 변경 */
    //            _cloneButton.GetComponent<Transform>().GetChild(0).GetComponent<Text>().text = _name[2].Substring(0, _name[2].Length - 7);

    //            /* 해당 버튼이 클릭되면 이름을 인자 값으로 넘기면서 OnClickBuildButton 함수 실행 */
    //            _cloneButton.GetComponent<Button>().onClick.AddListener(() => OnClickBuildButton(_file.Name));
    //        }
    //    }
    //}

    public void Update()
    {
        ReplaceClickedItem();
        ButtonInteractableControl();
        LocateBuilding();
    }

    ///* Place 버튼이 클릭되면 실행되는 함수 */
    //public void OnClickPlaceButton(string _name)
    //{
    //    /* 장소 개수만큼 반복 */
    //    for (int i = 0; i < _places.Length; i++)
    //    {
    //        /* 해당하는 이름의 장소 활성화 */
    //        if (_places[i].name == _name)
    //            _places[i].SetActive(true);

    //        /* 해당하지 않으면 비활성화 */
    //        else
    //            _places[i].SetActive(false);
    //    }
    //}

    ///* Build 버튼이 클릭되면 실행되는 함수 */
    //public void OnClickBuildButton(string _name)
    //{
    //    /* 이동할 건물 생성(지정) */
    //    _locatedBuilding = Instantiate(Resources.Load("Item/Place/Build/" + _name.Substring(0, _name.Length - 7)) as GameObject);

    //    /* 이름 지정 */
    //    _locatedBuilding.name = _name.Substring(0, _name.Length - 7);

    //    /* 부모 지정 */
    //    _locatedBuilding.GetComponent<Transform>().SetParent(_simpleHouse.GetComponent<Transform>());

    //    /* RayCast 무시 */
    //    _locatedBuilding.GetComponent<Transform>().GetChild(0).gameObject.layer = 2;

    //    /* 카메라 움직일 수 없음 */
    //    _cameraMoveAroun._cameraAroun = false;

    //    /* 기준 Plane 활성화 */
    //    _measurePlane.SetActive(true);
    //}

    /* 클릭된 Place를 교체하는 함수 */
    public void ReplaceClickedItem()
    {
        /* 왼쪽 마우스를 클릭하는 동시에 UI 상에 마우스가 올라와 있지 않으면서 SimpleHouse 오브젝트가 활성화일 시 */
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && _simpleHouse.activeSelf)
        {
            /* RayCast 발동 */
            RaycastHit _hit;
            Ray _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            /* RayCast가 닿으면 */
            if (Physics.Raycast(_ray, out _hit))
            {
                GameObject _hitObject = _hit.transform.gameObject;

                /* 타격한 오브젝트의 태그를 확인 */
                if (_hitObject.tag == "Wall" || _hitObject.tag == "Floor" || _hitObject.tag == "Door")
                {
                    /* 클릭된 오브젝트의 부모로 교체(피봇) */
                    _clickedPlace = _hitObject.GetComponent<Transform>().parent.gameObject;

                    /* ClickedPlaceCanvas의 크기 Slider 초기화 */
                    _clickedPlaceControl.ResetScaleSlider();

                    /* ClickedPlaceCanvas의 Tiling InputField를 갱신 */
                    _clickedPlaceControl.ResetTilingInputField();

                    /* ClickedPlaceCanvas의 위치 InputField를 갱신 */
                    _clickedPlaceControl.ResetPositionInputField();
                }
            }
        }
    }

    /* 버튼 Interactable 조절 함수 */
    public void ButtonInteractableControl()
    {
        _planeButton.interactable = _simpleHouse.activeSelf; //SimpleHouse가 활성화되어 있으면 Plane 버튼 활성화
        _buildButton.interactable = _simpleHouse.activeSelf; //SimpleHouse가 활성화되어 있으면 Build 버튼 활성화
    }

    /* 건물을 이동하는 함수 */
    void LocateBuilding()
    {
        /* 설치해야할 건물이 있으면 */
        if (_locatedBuilding)
        {
            Ray _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit _rayHit;

            /* 레이캐스트 충돌 시 */
            if (Physics.Raycast(_ray, out _rayHit))
            {
                /* 도킹 기능이 활성화 중이면 */
                if (_clickedPlaceControl._isDocking && (_rayHit.transform.tag == "Floor" || _rayHit.transform.tag == "Wall"))
                {
                    if (_rayHit.transform.tag == "Floor") //바닥에 설치하는 경우
                    {
                        Transform _rayHitTransform = _rayHit.transform;

                        /* 바닥의 네 개의 모퉁이의 좌표를 구함 */
                        Vector3 _xMinusCorner = _rayHitTransform.position - _rayHitTransform.right * ((_rayHitTransform.localScale.x * _rayHitTransform.parent.localScale.x) * 0.5f);
                        Vector3 _xPlusCorner = _rayHitTransform.position + _rayHitTransform.right * ((_rayHitTransform.localScale.x * _rayHitTransform.parent.localScale.x) * 0.5f);
                        Vector3 _zMinusCorner = _rayHitTransform.position - _rayHitTransform.forward * ((_rayHitTransform.localScale.z * _rayHitTransform.parent.localScale.z) * 0.5f);
                        Vector3 _zPlusCorner = _rayHitTransform.position + _rayHitTransform.forward * ((_rayHitTransform.localScale.z * _rayHitTransform.parent.localScale.z) * 0.5f);

                        /* 왼쪽 모퉁이와 가까워지면 */
                        if (Vector3.Distance(_xMinusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_xMinusCorner, _rayHit.point))
                        {
                            /* 왼쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _xMinusCorner;
                        }
                        /* 오른쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_xPlusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_xPlusCorner, _rayHit.point))
                        {
                            /* 오른쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _xPlusCorner;
                        }
                        /* 아래쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_zMinusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_zMinusCorner, _rayHit.point))
                        {
                            /* 아래쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _zMinusCorner;
                        }
                        /* 위쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_zPlusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_zPlusCorner, _rayHit.point))
                        {
                            /* 위쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _zPlusCorner;
                        }
                        /* 아무런 모퉁이와 가깝지 않으면 */
                        else
                        {
                            /* 닿은 위치로 건물 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _rayHit.point;
                        }
                    }
                    else if (_rayHit.transform.tag == "Wall") //벽에 설치하는 경우
                    {
                        Transform _rayHitTransform = _rayHit.transform;

                        /* 벽 네 개의 모퉁이 좌표를 구함*/
                        Vector3 _yPlusCorner = _rayHitTransform.position + _rayHitTransform.up * ((_rayHitTransform.localScale.y * _rayHitTransform.parent.localScale.y) * 0.5f);
                        Vector3 _yMinusCorner = _rayHitTransform.position - _rayHitTransform.up * ((_rayHitTransform.localScale.y * _rayHitTransform.parent.localScale.y) * 0.5f);
                        Vector3 _xMinusCorner = _rayHitTransform.position - _rayHitTransform.right * ((_rayHitTransform.localScale.x * _rayHitTransform.parent.localScale.x) * 0.5f);
                        Vector3 _xPlusCorner = _rayHitTransform.position + _rayHitTransform.right * ((_rayHitTransform.localScale.x * _rayHitTransform.parent.localScale.x) * 0.5f);

                        Debug.Log(_xMinusCorner);
                        /* 위 모퉁이와 가까워지면 */
                        if (Vector3.Distance(_yPlusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_yPlusCorner, _rayHit.point))
                        {
                            /* 위쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _yPlusCorner;
                        }
                        /* 아래쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_yMinusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_yMinusCorner, _rayHit.point))
                        {
                            /* 아래쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _yMinusCorner;
                        }
                        /* 왼쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_xMinusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_xMinusCorner, _rayHit.point))
                        {
                            /* 왼쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _xMinusCorner;
                        }
                        /* 오른쪽 모퉁이와 가까워지면 */
                        else if (Vector3.Distance(_xPlusCorner, _rayHitTransform.position) / 2f > Vector3.Distance(_xPlusCorner, _rayHit.point))
                        {
                            /* 오른쪽 모퉁이로 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _xPlusCorner;
                        }
                        /* 아무런 모퉁이와 가깝지 않으면 */
                        else
                        {
                            /* 닿은 위치로 건물 이동 */
                            _locatedBuilding.GetComponent<Transform>().position = _rayHit.point;
                        }
                    }
                }
                /* 도킹 기능이 비활성화 중이면 */
                else
                {
                    /* 건물 위치 변경 */
                    _locatedBuilding.GetComponent<Transform>().position = _rayHit.point;
                }
            }

            /* 마우스 휠을 회전하면 건물 회전 */
            float _wheel = Input.GetAxis("Mouse ScrollWheel");

            if (_wheel > 0f)
            {
                _locatedBuilding.GetComponent<Transform>().Rotate(0, 15f, 0, Space.World);
            }
            else if (_wheel < 0f)
            {
                _locatedBuilding.GetComponent<Transform>().Rotate(0, -15f, 0, Space.World);
            }

            /* 마우스 왼쪽 버튼을 누르면 */
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isRelocated) //최초의 건물 배치이면
                {
                    /* 배치한 ItemObject라는 스크립트 추가 */
                    ItemObject _tmpItemObject = _locatedBuilding.GetComponent<Transform>().GetChild(0).gameObject.AddComponent<ItemObject>();

                    /* Item Component 할당 */
                    Item _tmpItem = new Item(_locatedBuildingItem.itemName, _itemListControl._wallDBIndex + 1, _locatedBuildingItem._originNumber, _locatedBuilding.GetComponent<Transform>().GetChild(0).gameObject);

                    /* DB에 Item Component 삽입 */
                    _itemListControl.AddWall(_tmpItem);
                    HistoryController.pushObjectCreateHist(_locatedBuildingItem.item3d, _tmpItem._originNumber, _tmpItem._objectNumber);

                    /* Item Component 삽입 */
                    _tmpItemObject._thisItem = _tmpItem;

                    /* PlaceNumber 초기화 */
                    _tmpItemObject._placeNumber = 3000;

                    /* Tiling 값 초기화 */
                    _tmpItemObject._tilingX = 1f;
                    _tmpItemObject._tilingY = 1f;
                }

                /* RayCast 적용 */
                _locatedBuilding.GetComponent<Transform>().GetChild(0).gameObject.layer = 0;

                /* 이동시킬 건물 없음 */
                _locatedBuilding = null;

                /* 기준 Plane 비활성화 */
                _measurePlane.SetActive(false);

                /* 카메라 움직일 수 있음 */
                _cameraMoveAroun._cameraAroun = true;

                /* ClickedPlaceCanvas의 위치 InputField를 갱신 */
                _clickedPlaceControl.ResetPositionInputField();
            }
        }
    }
}