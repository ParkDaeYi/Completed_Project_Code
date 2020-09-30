using System;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HiddenMenuControl : MonoBehaviour
{
    /**
* date 2018.07.17
* author Lugub
* desc
*  Hidden Menu Controller
*  히든메뉴의 모든 패널을 관리하는 스크립트
*  ESC를 누르면 HiddenMenu를 켜거나 끄거나 하는 작업과
*  또한 ESC를 눌러서 메뉴가 활성화되면 CameraAroun.cs의 움직임을 제한하고
*  반대의 역할도 수행함
*  
*/
    [Header("Hidden Menu")]
    public GameObject _menuPanel;
    public GameObject _savePanel;
    public GameObject _loadPanel;
    public GameObject _scrollView; //Load 와 Save는 하나의 ScrollView로 통합.
    public GameObject _coverDBPanel; //덮어 씌울건지 확인하는 패널
    public HiddenCanvasRefresh _hiddenCanvasRefresh; // 원인 불명으로 canvas active 해도 안나타는 경우 새로고침용

    [Header("Camera Move Around Switch")]
    public CameraMoveAroun _cameraMoveAroundSwi;
    public ClickedItemControl _clickedItemControl;

    [Header("Save")]
    public GameObject _saveParentObject;
    public InputField _saveText;
    public InputField _managerText;
    public ItemListControl _itemListControl;
    public GameObject _saveSamepleButton;
    private int contentNum;
    private bool newBtn;
    private string _date;
    private bool flag_createDB;
    private bool _isCoverDB;
    private string _copyText;

    [Header("Load")]
    public string _filePath = "";
    public FileLoad _fileLoad;
    public GameObject _loadParentObject;
    public GameObject _sampleButton;
    StartDBController _StartDBController;

    [Header("Dialog")]
    public GameObject _exitDialog;
    public GameObject _deleteDialog;
    public GameObject _deleteFile;
    public string _deleteFileName;

    [Header("LocateItem")]
    public LocateItem _locateItem;

    [Header("Variable")]
    public string _dataBaseFilepath;

    [Header("Dress")]
    public Material _dressMaterial;

    [Header("House")]
    public GameObject _inDoor;

    string dir_path;

    public IEnumerator Hidden_Start()
    {
        string folderPath = dir_path + "/Database/";
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        bool TT = false;
        /*
         *  TotalTable.sqlite 파일이 있는지 검사
         */
        foreach (FileInfo file in dir.GetFiles())
        {

            if (file.Extension.ToLower().Equals(".sqlite"))
            {
                string[] fileName = file.Name.Split('.');

                if (fileName[0].Equals("TotalTable"))
                {
                    TT = true;
                    break;
                }
            }

        }
        /*
         *  TotalTable.sqlite 파일이 없다면
         *  Total_Empty 를 통해 생성 및 DataBase 폴더에 있는 .sqlite 파일들을
         *  TotalTable에 추가시킴.
         */
        if (!TT)
        {
            VRDBController.Total_Init();

            foreach (FileInfo file in dir.GetFiles())
            {

                if (file.Extension.ToLower().Equals(".sqlite"))
                {
                    string[] fileName = file.Name.Split('.');
                    if (fileName[0].Equals("TotalTable") || fileName[0].Equals("VoiceDirectory") || fileName[0].Equals("VoiceFile"))
                        continue;

                    string tmp = file.LastWriteTime.ToString();
                    string[] lastWrite = tmp.Split(' ');
                    VRDBController.Total_add(fileName[0], lastWrite[0]);
                }

            }

#if UNITY_EDITOR
            ImportAsset.NewImportAsset_File("Assets/DataBase/TotalTable.sqlite");
#endif       
        }
        /*
         *  TotalTable.sqlite 파일이 있다면
         *  연결을 시켜줌
         */
        else
        {
            VRDBController.ConIn(dir_path + "/DataBase/TotalTable");
        }
        /*
         *  GameDBController에 있는 TotalStart를 실행    
         */
        VRDBController.Total_GetKey_Start();

        yield return null;
    }

    private void Start()
    {
        dir_path = Static.STATIC.dir_path;
        _dataBaseFilepath = dir_path + "/DataBase";
        _StartDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }


    void Update()
    {

        /*ESC키를 누르면 히든메뉴가 나오고, 한번더 누르면 꺼짐*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuCheck();
        }
    }

    public void MenuCheck()
    {
        if (_clickedItemControl._clickedItem == null)
        {
            /* 모든 히든메뉴가 꺼져있을 경우 */
            if (!_menuPanel.activeSelf && !_savePanel.activeSelf && !_loadPanel.activeSelf)
            {
                _menuPanel.SetActive(true);
                _cameraMoveAroundSwi._cameraAroun = false;
            }
            /* 하나라도 켜져있을 경우 전부 꺼지게함*/
            else
            {
                AllFalse();
            }
        }
        else
        {
            _clickedItemControl.OnclickClickedItemMenuCancel();
            MenuCheck();
        }
        _hiddenCanvasRefresh.HiddenCanvasRefreshing();
    }

    /* 모든 패널을 끄는 함수 */
    public void AllFalse()
    {
        _menuPanel.SetActive(false);
        _savePanel.SetActive(false);
        _loadPanel.SetActive(false);
        _scrollView.SetActive(false);
        _exitDialog.SetActive(false);
        _deleteDialog.SetActive(false);

        _cameraMoveAroundSwi._cameraAroun = true;
    }


    /* Load Part*/
    /* Load에 해당하는 버튼은 동적으로 생성되기 때문에 이에 들어갈 스크립트는
       동적으로 넣어주기 위해서 다른 LoadButton.cs에 넣어준다.
       LoadButton.cs에는 눌렀을 때 그 데이터의 filepath를 이스크립트의 _filepath라는 변수에다가 넣어주고
       OnclickLoadLoad를 눌렀을 시 _filepath에 있는 string을 토대로 db를 가져온다.*/
    public void OnclickLoadButton()
    {
        _menuPanel.SetActive(!_menuPanel.activeSelf);
        _loadPanel.SetActive(!_loadPanel.activeSelf);
        _scrollView.SetActive(!_scrollView.activeSelf);

        /* Save와 Load의 scrollView 는 공용으로 사용되지만, 보이는 위치는 달라야 함. */
        _scrollView.transform.localPosition = new Vector3(0, 50, 0);

        /* Load 버튼을 눌렀을 때 Asset/DataBase 경로의 ".db"파일을 전부 가져옴*/
        //_fileLoad.FileLoadControll(_dataBaseFilepath, ".db", _loadParentObject, _sampleButton, 0);
    }

    /* LoadPanel의 Cancel버튼을 눌렀을 때 */
    public void OnclickLoadCancel()
    {
        AllFalse();
    }

    /* LoadPanel의 Load버튼을 눌렀을 때 */
    public void OnclickLoadLoad()

    {
        /*Load Sequence*/
        /* 파일을 아무것도 선택하지 않았을 경우*/
        if (_filePath == "")
        {
            Debug.Log("파일 경로 선택 안함");
        }
        /* 파일을 선택했을 경우*/
        else
        {

            GameObject.Find("HistoryController").transform.GetComponent<HistoryController>().Clear();

            GameObject.Find("Controllers/MiniMenuController").transform.GetComponent<MenuMiniOption>().OnClickSchedulerMenu();
            //로드 버튼을 클릭하면 현재 클릭되어있던 창을 다끄고 클릭된 아이템을null로 만들고 로드해야함
            StartCoroutine(LoadCoroutine());
            //LoadObjectFromDataBase();
            //LoadHumanFromDataBase();
            //LoadAnimationFromDataBase();
            //LoadVoiceFromDataBase();
            //LoadPlayed();


            _loadPanel.SetActive(!_loadPanel);
            _scrollView.SetActive(!_scrollView);
            _loadPanel.SetActive(!_loadPanel);
            _scrollView.SetActive(!_scrollView);
        }

    }

    /**

* date 2018.07.24

* author Lugub

* desc

*  아이템을 Load함

*  _itemListControl.cs에서 선언한 _itemTable은 1차년도의 ConstTable의 역할을 한다.

*  또한 Dictionary형태를 가지고 있기 때문에 숫자값을 넣으면 Item값을 return해준다.

*  이를 이용해서 Item의 정보를 받아옴

*/

    IEnumerator LoadCoroutine()
    {
        yield return StartCoroutine(LoadObjectFromDataBase());
        yield return StartCoroutine(LoadWallFromDataBase());
        yield return StartCoroutine(LoadHouseInfoFromDataBase());
        yield return StartCoroutine(LoadHumanFromDataBase());
        yield return StartCoroutine(LoadHandItemFromDataBase());
        yield return StartCoroutine(LoadDressFromDataBase());
        yield return StartCoroutine(LoadAnimationFromDataBase());
        yield return StartCoroutine(LoadVoiceFromDataBase());
        yield return StartCoroutine(LoadPlayed());

        _filePath = "";
    }

    IEnumerator LoadObjectFromDataBase()
    {
        _itemListControl.DeleteAllObjectAndAniBar(); //전체 초기화
        /*  */
        VRDBController.ConIn(_filePath);// 초기화에서 디비연결을 활용

        /* 디비에서 테이블을 가져와 _dataBaseItem에 테이블 저장*/
        DataTable _dataBaseItem = VRDBController.getItemInfo();

        //2.db에 맞게 객체랑 연결하고 생성(생성되는 과정에서 itemDataBase.cs에 자동저장
        foreach (DataRow dr in _dataBaseItem.Rows)
        {

            int _objectNumber = (int)Convert.ChangeType(dr["ObjectNumber"], typeof(int));
            int _originNumber = (int)Convert.ChangeType(dr["OriginNumber"], typeof(int));
            string _objectName = (string)Convert.ChangeType(dr["ObjectName"], typeof(string));
            float _positionX = (float)Convert.ChangeType(dr["positionX"], typeof(float));
            float _positionY = (float)Convert.ChangeType(dr["positionY"], typeof(float));
            float _positionZ = (float)Convert.ChangeType(dr["positionZ"], typeof(float));
            float _rotationX = (float)Convert.ChangeType(dr["rotationX"], typeof(float));
            float _rotationY = (float)Convert.ChangeType(dr["rotationY"], typeof(float));
            float _rotationZ = (float)Convert.ChangeType(dr["rotationZ"], typeof(float));
            float _scaleX = (float)Convert.ChangeType(dr["scaleX"], typeof(float));
            float _scaleY = (float)Convert.ChangeType(dr["scaleY"], typeof(float));
            float _scaleZ = (float)Convert.ChangeType(dr["scaleZ"], typeof(float));

            Vector3 _currentPosition = new Vector3(_positionX, _positionY, _positionZ);
            Vector3 _currentRotate = new Vector3(_rotationX, _rotationY, _rotationZ);
            Vector3 _currentScale = new Vector3(_scaleX, _scaleY, _scaleZ);

            /* _itemListControl.cs의 _itemTable은 Dictionary형태로써 <(int)itemOriginNumber, (Item) item>의 형태로 이루어져있다.
             * 그래서 DB에서 가져온 값중 obj_grp_id는 아이템의 originnumber이고 이걸 
             * dictionary에 넣으면 Item을 호출한다. 그래서 이렇게 사용함 */

            _locateItem.LoadItem(_originNumber, _itemListControl.GetItem(1, _originNumber), _objectName, _currentPosition, _currentRotate, _currentScale);

        }

        yield return null;

    }

    IEnumerator LoadWallFromDataBase()
    {
        /* 디비에서 테이블을 가져와 _dataBaseItem에 테이블 저장*/
        DataTable _dataBaseItem = VRDBController.getWallInfo();

        //2.db에 맞게 객체랑 연결하고 생성(생성되는 과정에서 itemDataBase.cs에 자동저장
        foreach (DataRow dr in _dataBaseItem.Rows)
        {

            int _objectNumber = (int)Convert.ChangeType(dr["ObjectNumber"], typeof(int));
            int _originNumber = (int)Convert.ChangeType(dr["OriginNumber"], typeof(int));
            string _objectName = (string)Convert.ChangeType(dr["ObjectName"], typeof(string));
            float _positionX = (float)Convert.ChangeType(dr["positionX"], typeof(float));
            float _positionY = (float)Convert.ChangeType(dr["positionY"], typeof(float));
            float _positionZ = (float)Convert.ChangeType(dr["positionZ"], typeof(float));
            float _rotationX = (float)Convert.ChangeType(dr["rotationX"], typeof(float));
            float _rotationY = (float)Convert.ChangeType(dr["rotationY"], typeof(float));
            float _rotationZ = (float)Convert.ChangeType(dr["rotationZ"], typeof(float));
            float _scaleX = (float)Convert.ChangeType(dr["scaleX"], typeof(float));
            float _scaleY = (float)Convert.ChangeType(dr["scaleY"], typeof(float));
            float _scaleZ = (float)Convert.ChangeType(dr["scaleZ"], typeof(float));
            int _placeNumber = (int)Convert.ChangeType(dr["PlaceNumber"], typeof(int));
            float _tilingX = (float)Convert.ChangeType(dr["TilingX"], typeof(float));
            float _tilingY = (float)Convert.ChangeType(dr["TilingY"], typeof(float));

            Vector3 _currentPosition = new Vector3(_positionX, _positionY, _positionZ);
            Vector3 _currentRotate = new Vector3(_rotationX, _rotationY, _rotationZ);
            Vector3 _currentScale = new Vector3(_scaleX, _scaleY, _scaleZ);

            /* _itemListControl.cs의 _itemTable은 Dictionary형태로써 <(int)itemOriginNumber, (Item) item>의 형태로 이루어져있다.
             * 그래서 DB에서 가져온 값중 obj_grp_id는 아이템의 originnumber이고 이걸 
             * dictionary에 넣으면 Item을 호출한다. 그래서 이렇게 사용함 */

            _locateItem.LoadWall(_originNumber, _itemListControl.GetItem(4, _originNumber), _objectName, _currentPosition, _currentRotate, _currentScale, _placeNumber, _tilingX, _tilingY);

        }

        yield return null;

    }

    IEnumerator LoadHouseInfoFromDataBase()
    {
        DataTable _dataBaseItem = VRDBController.getHouseInfo();
        DataRow dr = _dataBaseItem.Rows[0];

        int _isSimpleHouse = (int)Convert.ChangeType(dr["isSimpleHouse"], typeof(int));
        int _isHouse = (int)Convert.ChangeType(dr["isHouse"], typeof(int));
        int _isCar = (int)Convert.ChangeType(dr["isCar"], typeof(int));

        if (_isSimpleHouse == 1)
        {
            _inDoor.transform.Find("SimpleHouse").gameObject.SetActive(true);
            Static.STATIC._isSimpleHouse = 1;
            Static.STATIC._isHouse = 0;
            Static.STATIC._isCar = 0;
        }
        else if (_isHouse == 1)
        {
            _inDoor.transform.Find("House").gameObject.SetActive(true);
            Static.STATIC._isSimpleHouse = 0;
            Static.STATIC._isHouse = 1;
            Static.STATIC._isCar = 0;
        }
        else if (_isCar == 1)
        {
            _inDoor.transform.Find("Car").gameObject.SetActive(true);
            Static.STATIC._isSimpleHouse = 0;
            Static.STATIC._isHouse = 0;
            Static.STATIC._isCar = 1;
        }

        yield return null;
    }

    IEnumerator LoadHumanFromDataBase()
    {
        /*  */
        //VRDBController.Init(_filePath);// 초기화에서 디비연결을 활용

        /* 디비에서 테이블을 가져와 _dataBaseItem에 테이블 저장*/
        DataTable _dataBaseItem = VRDBController.getHumanInfo();

        //2.db에 맞게 객체랑 연결하고 생성(생성되는 과정에서 itemDataBase.cs에 자동저장
        foreach (DataRow dr in _dataBaseItem.Rows)
        {

            int _objectNumber = (int)Convert.ChangeType(dr["ObjectNumber"], typeof(int));
            int _originNumber = (int)Convert.ChangeType(dr["OriginNumber"], typeof(int));
            string _objectName = (string)Convert.ChangeType(dr["ObjectName"], typeof(string));
            float _positionX = (float)Convert.ChangeType(dr["positionX"], typeof(float));
            float _positionY = (float)Convert.ChangeType(dr["positionY"], typeof(float));
            float _positionZ = (float)Convert.ChangeType(dr["positionZ"], typeof(float));
            float _rotationX = (float)Convert.ChangeType(dr["rotationX"], typeof(float));
            float _rotationY = (float)Convert.ChangeType(dr["rotationY"], typeof(float));
            float _rotationZ = (float)Convert.ChangeType(dr["rotationZ"], typeof(float));
            float _scaleX = (float)Convert.ChangeType(dr["scaleX"], typeof(float));
            float _scaleY = (float)Convert.ChangeType(dr["scaleY"], typeof(float));
            float _scaleZ = (float)Convert.ChangeType(dr["scaleZ"], typeof(float));
            int _humanNumber = (int)Convert.ChangeType(dr["HumanNumber"], typeof(int));

            Vector3 _currentPosition = new Vector3(_positionX, _positionY, _positionZ);
            Vector3 _currentRotate = new Vector3(_rotationX, _rotationY, _rotationZ);
            Vector3 _currentScale = new Vector3(_scaleX, _scaleY, _scaleZ);

            /* _itemListControl.cs의 _itemTable은 Dictionary형태로써 <(int)itemOriginNumber, (Item) item>의 형태로 이루어져있다.
             * 그래서 DB에서 가져온 값중 obj_grp_id는 아이템의 originnumber이고 이걸 
             * dictionary에 넣으면 Item을 호출한다. 그래서 이렇게 사용함 */

            _locateItem.LoadHuman(_originNumber, _itemListControl.GetItem(2, _originNumber), _objectName, _currentPosition, _currentRotate, _currentScale, _humanNumber);

        }

        yield return null;
    }

    IEnumerator LoadHandItemFromDataBase()
    {

        /* 디비에서 테이블을 가져와 _dataBaseItem에 테이블 저장*/
        DataTable _dataBaseItem = VRDBController.getHandItemInfo();

        //2.db에 맞게 객체랑 연결하고 생성(생성되는 과정에서 itemDataBase.cs에 자동저장
        foreach (DataRow dr in _dataBaseItem.Rows)
        {
            int _originNumber = (int)Convert.ChangeType(dr["OriginNumber"], typeof(int));
            int _humanNumber = (int)Convert.ChangeType(dr["HumanNumber"], typeof(int));
            int _isLeft = (int)Convert.ChangeType(dr["isLeft"], typeof(int));

            Item _item = null;
            Transform _hand = null;
            foreach (Item A in _itemListControl._dataBaseHuman)
            {
                if (A._loadHumanNumber == _humanNumber)
                {
                    _item = A;
                    break;
                }
            }

            Debug.Log(_item.itemName);
            Item _handItem = _itemListControl.GetItem(5, _originNumber);
            HumanItem _humanItem = _item.item3d.GetComponent<ItemObject>()._thisHuman;
            Debug.Log(_handItem.itemName);
            Debug.Log(_humanItem._humanNumber);

            if (_isLeft == 1)
            {
                _hand = _item.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                _humanItem._leftHandItem = _handItem;
            }
            else
            {
                _hand = _item.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
                _humanItem._rightHandItem = _handItem;
            }

            GameObject _clone = Instantiate(_handItem.item3d);
            _clone.transform.SetParent(_hand);
            _clone.name = _handItem.itemName;
            _clone.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Vector3 _localPos;

            //각 객체에 따라 객체의 위치 조정
            if (_item._originNumber == 2001) //Man
            {
                _localPos = _clone.transform.GetComponentInChildren<HandItem>()._manRightPos;
            }
            else if (_item._originNumber == 2002) //Woman
            {
                _localPos = _clone.transform.GetComponentInChildren<HandItem>()._womanRightPos;
            }
            else if (_item._originNumber == 2000) //Daughter
            {
                _localPos = _clone.transform.GetComponentInChildren<HandItem>()._daughterRightPos;
            }
            else if (_item._originNumber == 2003) //Woongin
            {
                _localPos = _clone.transform.GetComponentInChildren<HandItem>()._woonginPos;
            }
            else
            {
                _localPos = _hand.transform.GetChild(2).transform.position; //세번째 손가락 위치로 지정
            }

            _clone.transform.localPosition = _localPos;

            if (_isLeft == 1)
            {
                //왼손일 경우 좌우 반전
                Vector3 _tmp = _clone.transform.localScale;
                _tmp.z *= -1;
                _clone.transform.localScale = _tmp;

                //앞 뒤 반전
                _tmp = _clone.transform.localPosition;
                _tmp.z = -_tmp.z;
                _clone.transform.localPosition = _tmp;
            }

        }
        yield return null;
    }

    IEnumerator LoadDressFromDataBase()
    {

        /* 디비에서 테이블을 가져와 _dataBaseItem에 테이블 저장*/
        DataTable _dataBaseDress = VRDBController.getDressInfo();

        //2.db에 맞게 객체랑 연결하고 생성(생성되는 과정에서 itemDataBase.cs에 자동저장
        foreach (DataRow dr in _dataBaseDress.Rows)
        {
            int _humanNumber = (int)Convert.ChangeType(dr["HumanNumber"], typeof(int));
            string _shirtName = (string)Convert.ChangeType(dr["ShirtName"], typeof(string));
            float _shirt_R = (float)Convert.ChangeType(dr["Shirt_R"], typeof(float));
            float _shirt_G = (float)Convert.ChangeType(dr["Shirt_G"], typeof(float));
            float _shirt_B = (float)Convert.ChangeType(dr["Shirt_B"], typeof(float));
            string _pantName = (string)Convert.ChangeType(dr["PantName"], typeof(string));
            float _pant_R = (float)Convert.ChangeType(dr["Pant_R"], typeof(float));
            float _pant_G = (float)Convert.ChangeType(dr["Pant_G"], typeof(float));
            float _pant_B = (float)Convert.ChangeType(dr["Pant_B"], typeof(float));
            string _shoesName = (string)Convert.ChangeType(dr["ShoesName"], typeof(string));
            float _shoes_R = (float)Convert.ChangeType(dr["Shoes_R"], typeof(float));
            float _shoes_G = (float)Convert.ChangeType(dr["Shoes_G"], typeof(float));
            float _shoes_B = (float)Convert.ChangeType(dr["Shoes_B"], typeof(float));

            Item _item = null;
            foreach (Item A in _itemListControl._dataBaseHuman)
            {
                if (A._loadHumanNumber == _humanNumber)
                {
                    _item = A;
                    break;
                }
            }

            HumanItem _humanItem = _item.item3d.GetComponent<ItemObject>()._thisHuman;

            if (!_shirtName.Equals(""))
            {
                Transform _shirt_parent = _item.item3d.transform.Find("shirt");
                int childCnt = _shirt_parent.childCount;
                for (int i = 0; i < childCnt; i++)
                {
                    _shirt_parent.GetChild(i).gameObject.SetActive(false);
                }
                GameObject _shirt = _item.item3d.transform.Find("shirt").transform.Find(_shirtName).gameObject;
                _humanItem._shirt = _shirt;
                //이건 아직 필요할진 의문
                _humanItem._shirt_R = _shirt_R;
                _humanItem._shirt_G = _shirt_G;
                _humanItem._shirt_B = _shirt_B;

                _shirt.SetActive(true);
                GameObject _tmp = Instantiate(_shirt);
                Transform _colourization;

                _tmp.name = "Colourization"; //이름 변경
                _tmp.transform.SetParent(_shirt.transform);
                _colourization = _tmp.transform; //색상화 객체 추가

                /* 내부 Material을 교체하는 작업 */

                SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
                Material _copyMaterial = Instantiate(_dressMaterial); //메터리얼 복사
                _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

                for (int i = 0; i < _colourizationSkin.materials.Length; i++)
                {
                    Destroy(_colourizationSkin.materials[i]);
                }

                _colourizationSkin.material = _copyMaterial;
                //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
                _colourizationSkin.material.color = new Color(_shirt_R / 255f, _shirt_G / 255f, _shirt_B / 255f, 40f / 255f);
            }
            if (!_pantName.Equals(""))
            {
                Transform _pant_parent = _item.item3d.transform.Find("pant");
                int childCnt = _pant_parent.childCount;
                for (int i = 0; i < childCnt; i++)
                {
                    _pant_parent.GetChild(i).gameObject.SetActive(false);
                }
                GameObject _pant = _item.item3d.transform.Find("pant").transform.Find(_pantName).gameObject;
                _humanItem._pant = _pant;
                //이건 아직 필요할진 의문
                _humanItem._pant_R = _pant_R;
                _humanItem._pant_G = _pant_G;
                _humanItem._pant_B = _pant_B;

                _pant.SetActive(true);
                GameObject _tmp = Instantiate(_pant);
                Transform _colourization;

                _tmp.name = "Colourization"; //이름 변경
                _tmp.transform.SetParent(_pant.transform);
                _colourization = _tmp.transform; //색상화 객체 추가

                /* 내부 Material을 교체하는 작업 */

                SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
                Material _copyMaterial = Instantiate(_dressMaterial); //메터리얼 복사
                _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

                for (int i = 0; i < _colourizationSkin.materials.Length; i++)
                {
                    Destroy(_colourizationSkin.materials[i]);
                }

                _colourizationSkin.material = _copyMaterial;
                //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
                _colourizationSkin.material.color = new Color(_pant_R / 255f, _pant_G / 255f, _pant_B / 255f, 40f / 255f);
            }
            if (!_shoesName.Equals(""))
            {
                Transform _shoes_parent = _item.item3d.transform.Find("shoes");
                int childCnt = _shoes_parent.childCount;
                for (int i = 0; i < childCnt; i++)
                {
                    _shoes_parent.GetChild(i).gameObject.SetActive(false);
                }
                Vector3 _pos = _item.item3d.transform.parent.transform.position;

                string[] _sname = _shoesName.Split('_');
                GameObject _shoes = null;
                if (_sname[1].Equals("normal"))
                {
                    _item.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                    _shoes = _item.item3d.transform.Find("shoes").transform.GetChild(0).transform.Find(_shoesName).gameObject;
                    _pos.y = _item.item3d.GetComponent<ItemObject>()._humanInitPosition.y;
                }
                else if (_sname[1].Equals("abnormal"))
                {
                    _item.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(true);
                    _shoes = _item.item3d.transform.Find("shoes").transform.GetChild(1).transform.Find(_shoesName).gameObject;
                    _pos.y = _item.item3d.GetComponent<ItemObject>()._humanInitPosition.y + 1f;
                }
                _item.item3d.transform.parent.transform.position = _pos;

                _humanItem._shoes = _shoes;
                //이건 아직 필요할진 의문
                _humanItem._shoes_R = _shoes_R;
                _humanItem._shoes_G = _shoes_G;
                _humanItem._shoes_B = _shoes_B;

                GameObject _tmp = Instantiate(_shoes);
                Transform _colourization;

                _shoes.SetActive(true);
                _tmp.name = "Colourization"; //이름 변경
                _tmp.transform.SetParent(_shoes.transform);
                _tmp.SetActive(true);
                _colourization = _tmp.transform; //색상화 객체 추가

                /* 내부 Material을 교체하는 작업 */

                SkinnedMeshRenderer _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
                Material _copyMaterial = Instantiate(_dressMaterial); //메터리얼 복사
                _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

                for (int i = 0; i < _colourizationSkin.materials.Length; i++)
                {
                    Destroy(_colourizationSkin.materials[i]);
                }

                _colourizationSkin.material = _copyMaterial;
                //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
                _colourizationSkin.material.color = new Color(_shoes_R / 255f, _shoes_G / 255f, _shoes_B / 255f, 40f / 255f);
            }

        }
        yield return null;
    }

    IEnumerator LoadAnimationFromDataBase()
    {
        /*빅애니메이션 테이블 정보를 받아옴*/
        DataTable _dataBaseItem1 = VRDBController.getBigAnimationInfo();

        /*스몰애니메이션 테이블 정보를 받아옴*/
        DataTable _dataBaseItem2 = VRDBController.getSmallAnimationInfo();

        for (int i = 0; i < _dataBaseItem1.Rows.Count; i++)
        {
            /*빅애니메이션 정보 받아오기*/
            DataRow dr = _dataBaseItem1.Rows[i];
            int _number = (int)Convert.ChangeType(dr["Number"], typeof(int));
            float _barX = (float)Convert.ChangeType(dr["BarX"], typeof(float));
            float _barWidth = (float)Convert.ChangeType(dr["BarWidth"], typeof(float));
            string _animationName = (string)Convert.ChangeType(dr["AnimationName"], typeof(string));
            string _anibarName = (string)Convert.ChangeType(dr["AnibarName"], typeof(string));
            int _humanNumber = (int)Convert.ChangeType(dr["HumanNumber"], typeof(int));
            string _animationText = (string)Convert.ChangeType(dr["AnimationText"], typeof(string));

            /*빅애니메이션 생성*/
            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _animationName;
            _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _anibarName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _animationText;

            /*스몰애니메이션 정보 받아오기*/
            DataRow dr1 = _dataBaseItem2.Rows[i];
            int _number1 = (int)Convert.ChangeType(dr1["Number"], typeof(int));
            int _humanNumber1 = (int)Convert.ChangeType(dr1["HumanNumber"], typeof(int));
            int _moveOrState = (int)Convert.ChangeType(dr1["MoveOrState"], typeof(int));
            int _actionOrFace = (int)Convert.ChangeType(dr1["ActionOrFace"], typeof(int));
            int _layerNumber = (int)Convert.ChangeType(dr1["LayerNumber"], typeof(int));
            float _barX1 = (float)Convert.ChangeType(dr1["BarX"], typeof(float));
            float _barWidth1 = (float)Convert.ChangeType(dr1["BarWidth"], typeof(float));
            float _arriveX = (float)Convert.ChangeType(dr1["ArriveX"], typeof(float));
            float _arriveY = (float)Convert.ChangeType(dr1["ArriveY"], typeof(float));
            float _arriveZ = (float)Convert.ChangeType(dr1["ArriveZ"], typeof(float));
            int _originNumber = (int)Convert.ChangeType(dr1["OriginNumber"], typeof(int));
            int _rotation = (int)Convert.ChangeType(dr1["Rotation"], typeof(int));

            Vector3 _arriveLocation = new Vector3(_arriveX, _arriveY, _arriveZ);

            /*스몰애니바를 생성*/
            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
            _bigAniBar.gameObject.name = _smallAniBar.gameObject.name = _anibarName;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._layerNumber = _layerNumber;

            if (_actionOrFace == 0)
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._actionOrFace = false;
            }
            else
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._actionOrFace = true;
            }
            if (_rotation == 0)
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._rotation = false;
            }
            else
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._rotation = true;
            }

            /*찾아야하는 정보*/
            int _objectNumber = 0;
            Item _item = null;
            Animator _animator = null;
            foreach (Item A in _itemListControl._dataBaseHuman)
            {
                if (A._loadHumanNumber == _humanNumber1)
                {
                    _objectNumber = A._objectNumber;
                    _item = A;
                    _animator = A.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _animationName;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _anibarName;

            if (_moveOrState == 0)
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._moveCheck = false;
            }
            else
            {
                _smallAniBar.transform.GetComponent<SmallAniBar>()._moveCheck = true;
                _smallAniBar.transform.GetComponent<SmallAniBar>()._arriveLocation = _arriveLocation;
            }

            //수정해야하는 파트
            int _parentNumber = 0;
            if (_layerNumber == 0 || _layerNumber == 1) //액션
                _parentNumber = 4;
            else if (_layerNumber == 5) //페이스
                _parentNumber = 1;
            else if (_layerNumber == 4 || _layerNumber == 2) // 핸드
                _parentNumber = 2;
            else if (_layerNumber == 3) //레그
                _parentNumber = 3;

            /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
            if (_originNumber == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2002)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            
            /*빅애니바, 스몰애니바의 부모설정*/
            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX1, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;

            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _bigAniBar.transform.GetChild(1).transform.localPosition = new Vector3(-_barWidth / 2, 0, 0);
            _bigAniBar.transform.GetChild(2).transform.localPosition = new Vector3(_barWidth / 2, 0, 0);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth1, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

            if (_layerNumber == 0)
            {
                _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            }
            else if (_layerNumber == 3)
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }
            else if (_layerNumber == 2)
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }
            else if (_layerNumber == 5)
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }
            else if (_layerNumber == 1)
            {
                _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            }
            else if (_layerNumber == 4)
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }

            _itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        }

        yield return null;
    }

    IEnumerator LoadVoiceFromDataBase()
    {
        /*빅애니메이션 테이블 정보를 받아옴*/
        DataTable _dataBaseItem1 = VRDBController.getBigVoiceInfo();

        /*스몰애니메이션 테이블 정보를 받아옴*/
        DataTable _dataBaseItem2 = VRDBController.getSmallVoiceInfo();

        for (int i = 0; i < _dataBaseItem1.Rows.Count; i++)
        {
            /*빅애니메이션 정보 받아오기*/
            DataRow dr = _dataBaseItem1.Rows[i];
            int _number = (int)Convert.ChangeType(dr["Number"], typeof(int));
            float _barX = (float)Convert.ChangeType(dr["BarX"], typeof(float));
            float _barWidth = (float)Convert.ChangeType(dr["BarWidth"], typeof(float));
            string _voiceName = (string)Convert.ChangeType(dr["VoiceName"], typeof(string));
            int _humanNumber = (int)Convert.ChangeType(dr["HumanNumber"], typeof(int));
            string _voiceText = (string)Convert.ChangeType(dr["VoiceText"], typeof(string));

            Debug.Log("2020-04-05 HiddenMenuControl 920번줄 _voiceText : " + _voiceText);

            /*빅애니메이션 생성*/
            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _voiceName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _voiceText;
            _bigAniBar.name = _voiceText;

            /*스몰애니메이션 정보 받아오기*/
            DataRow dr1 = _dataBaseItem2.Rows[i];
            int _number1 = (int)Convert.ChangeType(dr1["Number"], typeof(int));
            int _humanNumber1 = (int)Convert.ChangeType(dr1["HumanNumber"], typeof(int));
            int _dir_key = (int)Convert.ChangeType(dr1["Key"], typeof(int));
            string _voiceName1 = (string)Convert.ChangeType(dr1["VoiceName"], typeof(string));
            float _barX1 = (float)Convert.ChangeType(dr1["BarX"], typeof(float));
            float _barWidth1 = (float)Convert.ChangeType(dr1["BarWidth"], typeof(float));
            int _originNumber = (int)Convert.ChangeType(dr1["OriginNumber"], typeof(int));

            AudioClip _audioClip = _StartDBController.audioInfo[_dir_key][_voiceName1];
            /////////////////////////////////////////////////////////////////////////////////
            //if (_voiceGender == 1)
            //{
            //    _audioClip = Resources.Load<AudioClip>("Voice/son/" + _voiceName1);
            //}
            //else
            //{
            //    _audioClip = Resources.Load<AudioClip>("Voice/yuinna/" + _voiceName1);
            //}
            /////////////////////////////////////////////////////////////////////////////////

            /*스몰애니바를 생성*/
            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
            //_smallAniBar.transform.GetComponent<SmallAniBar>()._voiceGender = _voiceGender;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._dir_key = _dir_key;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._voice = true;
            _smallAniBar.name = _voiceText;

            /*찾아야하는 정보*/
            int _objectNumber = 0;
            Item _item = null;
            Animator _animator = null;
            foreach (Item A in _itemListControl._dataBaseHuman)
            {
                if (A._loadHumanNumber == _humanNumber1)
                {
                    _objectNumber = A._objectNumber;
                    _item = A;
                    _animator = A.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _voiceName;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _voiceName;

            int _parentNumber = 5;

            /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
            if (_originNumber == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }
            else if (_originNumber == 2002)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + _objectNumber).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + _objectNumber).transform.GetChild(_parentNumber).gameObject;
            }

            /*빅애니바, 스몰애니바의 부모설정*/
            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정   
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX1, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;

            _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

            float _aniBarWidth = _bigAniBar.transform.GetComponent<RectTransform>().rect.width;
            float _width = _aniBarWidth * _audioClip.length / 11.73f;
            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

            _smallAniBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;

            _itemListControl.AddVoiceDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        }

        yield return null;
    }

    IEnumerator LoadPlayed()
    {
        VRDBController.plusPlayed(_filePath);
        yield return null;
    }

    /* Save Part */
    /* HiddenMenu에서 Save버튼을 눌렀을 경우 */
    public void OnclickSaveButton()
    {
        _menuPanel.SetActive(!_menuPanel.activeSelf);
        _savePanel.SetActive(!_savePanel.activeSelf);
        _scrollView.SetActive(!_scrollView.activeSelf);

        /* Save와 Load의 scrollView 는 공용으로 사용되지만, 보이는 위치는 달라야 함. */
        _scrollView.transform.localPosition = new Vector3(-160, 50, 0);

        /* Save버튼 눌렀을 때 SavePanel 켜지면서 동시에 Asset/DataBase에 있는 ".db"파일을 로드 */
        //_fileLoad.FileLoadControll(_dataBaseFilepath, ".db", _saveParentObject, _sampleButton, 0);

        //save 를 했어도 Static 에 _saveClickButton 을 null로 바꿔줘야함
        Static.STATIC._saveClickButton = null;
        _saveText.text = "";
    }

    /* Save패널에서 Cancel버튼을 눌렀을 경우 */
    public void OnclickSaveCancel()
    {
        AllFalse();
    }

    /**
* date 2018.07.23
* author Lugub
* desc
*  ItemListContrl.cs에서 가지고 있던 생성된 객체의 List를 가져와서 이 List를 바탕으로 DataBase에 저장
*  현재 객체만 해놓은 상태.
*/

    /* Save패널에서 Save버튼을 눌렀을 경우*/
    public void OnclickSaveSave()
    {
        /*
         * InputField가 활성화 되면 _saveClickButton 은 null 이 됨
         * _saveClickButton이 null이면 새로운 버튼 생성
         * 아니면 클릭된 버튼을 삭제하고 새로운 버튼 생성
         */
        _isCoverDB = false;
        if (_saveText.text != "")
        {
            string filePath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/Database", _saveText.text + ".sqlite");
            FileInfo fInfo = new FileInfo(filePath);

            if (Static.STATIC._saveClickButton == null)
            {
                flag_createDB = false;

                if (fInfo.Exists)
                {
                    newBtn = false;
                    flag_createDB = true;
                    /* 덮어 씌울건지 질의하는 창 띄움 */
                    _coverDBPanel.SetActive(true);
                }
                else
                {
                    newBtn = true;
                    StartCoroutine(CreateDB());
                }
            }
            else
            {
                _isCoverDB = true;
                flag_createDB = true;

                if (fInfo.Exists)
                {
                    flag_createDB = false;
                    /* 덮어 씌울건지 질의하는 창 띄움 */
                    _coverDBPanel.SetActive(true);
                }

                else 
                {
                    fInfo = new FileInfo(_filePath + ".sqlite");

                    if (fInfo.Exists)
                    {
                        fInfo.Delete();
                    }
                    newBtn = false;
                    StartCoroutine(CreateDB());
                }       
            }
#if UNITY_EDITOR
            ImportAsset.NewImportAsset_File("Assets/DataBase/" + _saveText.text + ".sqlite");
#endif
        }
        else
        {
            if (Static.STATIC._saveClickButton != null)
            {
                string filePath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/Database", Static.STATIC._saveClickButton.name + ".sqlite");
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Exists)
                {
                    fInfo.Delete();
                }
                flag_createDB = true;
                newBtn = false;
                StartCoroutine(CreateDB());
            }
            else Debug.Log("Text를 입력하세요");
        }
        //파일 이름만 변경 시 사용할 코드
        //string filePath = string.Format("{0}/{1}", Static.STATIC.dir_path + "/Database", _saveText.text + ".sqlite");
        //FileInfo fInfo = new FileInfo(filePath);
        //if (fInfo.Exists)
        //{
        //    newBtn = false;
        //    _isCreateDB = false;
        //    _coverDBPanel.SetActive(true);
        //}
        //Static.STATIC._saveClickButton.name = _saveText.text;
        //string tmp = Static.STATIC._saveClickButton.transform.GetChild(0).GetComponent<Text>().text;
        //string[] line = tmp.Split('.');
        //string[] line2 = line[1].Split(':');
        //line2[0] = "</color>" + _saveText.text;
        //Static.STATIC._saveClickButton.transform.GetChild(0).GetComponent<Text>().text = line[0] + ". " + line2[0] + " :" + line2[1];
    }

    //CreateDB를 실행하기 전 TotalTable을 먼저 방문==>.sqlite 가 겹치면 안됨. 
    public IEnumerator Before_CreateDB()
    {
        _date = DateTime.Now.ToString("yyyy-MM-dd");
        //먼저 TotalTable 에 _saveText.text 를 삽입
        VRDBController.ConIn(dir_path + "/DataBase/TotalTable");
        if (Static.STATIC._saveClickButton == null)
        {
            if (flag_createDB)
            {
                Static.STATIC._saveClickButton = _saveParentObject.transform.Find(_saveText.text).gameObject;
                VRDBController.Total_Delete(_saveText.text);
            }       
        }
        else
        {
            if (!flag_createDB)
                VRDBController.Total_Delete(_saveText.text);
            VRDBController.Total_Delete(Static.STATIC._saveClickButton.name);
        }

        _copyText = "";
        if (_saveText.text == "")
        {
            _copyText = Static.STATIC._saveClickButton.name;
        }
        else
        {
            _copyText = _saveText.text;
        }
        VRDBController.Total_add(_copyText, _date);
        contentNum = VRDBController.Total_GetKey(_copyText);
        yield return contentNum;

        //새로운 버튼 생성??
        if (newBtn)
        {
            GameObject tmp = Instantiate(_saveSamepleButton);
            tmp.name = _copyText;
            tmp.transform.GetChild(0).GetComponent<Text>().text = "<color=#0000ff>" + contentNum + ". </color>" + _copyText + " : <color=#ff0000>0</color>";
            tmp.transform.GetChild(2).GetComponent<Text>().text = _date;
            tmp.transform.SetParent(GameObject.Find("SaveContent").transform);
            tmp.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        }
        else
        {
            Static.STATIC._saveClickButton.name = _copyText;
            Static.STATIC._saveClickButton.transform.GetChild(0).GetComponent<Text>().text = "<color=#0000ff>" + contentNum + ". </color>" + _copyText + " : <color=#ff0000>0</color>";
            Static.STATIC._saveClickButton.transform.GetChild(2).GetComponent<Text>().text = _date;
        }
    }

    public IEnumerator CreateDB()
    {
        yield return StartCoroutine(Before_CreateDB());

        VRDBController.Init(dir_path + "/DataBase/" + _copyText);

        int _humanNumber = 0;

        _savePanel.SetActive(!_savePanel);
        _scrollView.SetActive(!_scrollView);
        _coverDBPanel.SetActive(false);

        /*Save Sequence*/
        /* _saveText 에 있는 이름대로 저장을 하되, 이름이 중복될 경우 처리와 새로 저장하는 경우를 처리 */
        VRDBController.addHouseInfo(Static.STATIC._isSimpleHouse, Static.STATIC._isHouse, Static.STATIC._isCar);
        for (int i = 0; i < _itemListControl._wallDBIndex; i++)
        {
            try
            {
                Item _tmp = _itemListControl._dataBaseWall[i];
                Vector3 _tmpPosition = _tmp.item3d.transform.parent.transform.position;
                Vector3 _tmpRotation = _tmp.item3d.transform.parent.transform.eulerAngles;
                Vector3 _tmpScale = _tmp.item3d.transform.parent.transform.localScale;
                int _placeNumber = _tmp.item3d.GetComponent<ItemObject>()._placeNumber;
                float _tilingX = _tmp.item3d.GetComponent<ItemObject>()._tilingX;
                float _tilingY = _tmp.item3d.GetComponent<ItemObject>()._tilingY;

                VRDBController.addWallInfo(_tmp._objectNumber, _tmp._originNumber, _tmp.itemName, _tmpPosition.x, _tmpPosition.y, _tmpPosition.z, _tmpRotation.x, _tmpRotation.y, _tmpRotation.z,
                    _tmpScale.x, _tmpScale.y, _tmpScale.z, _placeNumber, _tilingX, _tilingY);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        /* 아이템 리스트 저장 */ //사물객체만 활용
        for (int i = 0; i < _itemListControl._itemDBIndex; i++)
        {
            /* try-catch를 사용하는 이유*/
            /* itemDBIndex는 아이템이 늘어나면서 1씩 증가하지만 생성된 객체를 삭제할 경우는 처리를 하지않았기 때문에
             * 인덱스 오류가 발생해서 이를 방지하기위해 */
            try
            {
                Item _tmp = _itemListControl._dataBaseItem[i];
                Vector3 _tmpPosition = _tmp.item3d.transform.parent.transform.position;
                Vector3 _tmpRotation = _tmp.item3d.transform.parent.transform.eulerAngles;
                Vector3 _tmpScale = _tmp.item3d.transform.parent.transform.localScale;
                int _tmpOriginNumber = _tmp._originNumber;


                Debug.Log(_tmp.itemName);
                /* DataBase에 하나씩 추가 */
                VRDBController.addItemInfo(_tmp._objectNumber, _tmp._originNumber, _tmp.itemName, _tmpPosition.x, _tmpPosition.y, _tmpPosition.z,
                    _tmpRotation.x, _tmpRotation.y, _tmpRotation.z, _tmpScale.x, _tmpScale.y, _tmpScale.z);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        for (int i = 0; i < _itemListControl._dataBaseHuman.Count; i++)
        {
            try
            {
                Item _tmp = _itemListControl._dataBaseHuman[i];
                //HumanItem _hTmp = _itemListControl._dataBaseHuman[i].item3d.GetComponent<ItemObject>()._thisHuman;
                Vector3 _tmpPosition = _tmp.item3d.GetComponent<ItemObject>()._humanInitPosition;
                Vector3 _tmpRotation = _tmp.item3d.GetComponent<ItemObject>()._humanInitRotation;
                Vector3 _tmpScale = _tmp.item3d.transform.parent.transform.localScale;
                int _tmpOriginNumber = _tmp._originNumber;
                string _objectName = _tmp.itemName;


                /* DataBase에 하나씩 추가 */
                VRDBController.addHumanInfo(_tmp._objectNumber, _tmp._originNumber, _objectName, _tmpPosition.x, _tmpPosition.y, _tmpPosition.z,
                    _tmpRotation.x, _tmpRotation.y, _tmpRotation.z, _tmpScale.x, _tmpScale.y, _tmpScale.z, _humanNumber);
                _tmp._loadHumanNumber = _humanNumber;
                _humanNumber++;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        /*HandItem 저장*/
        for (int i = 0; i < _itemListControl._dataBaseHuman.Count; i++)
        {
            try
            {
                Item _tmp = _itemListControl._dataBaseHuman[i];
                HumanItem _hTmp = _tmp.item3d.GetComponent<ItemObject>()._thisHuman;

                if (_hTmp._leftHandItem != null)
                {
                    int _tmpHumanNumber = _tmp._loadHumanNumber;
                    int _tmpOriginNumber = _hTmp._leftHandItem._originNumber;

                    VRDBController.addHandItemInfo(_tmpOriginNumber, _tmpHumanNumber, 1);
                }
                if (_hTmp._rightHandItem != null)
                {
                    int _tmpHumanNumber = _tmp._loadHumanNumber;
                    int _tmpOriginNumber = _hTmp._rightHandItem._originNumber;

                    VRDBController.addHandItemInfo(_tmpOriginNumber, _tmpHumanNumber, 0);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        /*Dress저장*/
        for (int i = 0; i < _itemListControl._dataBaseHuman.Count; i++)
        {
            try
            {
                Item _tmp = _itemListControl._dataBaseHuman[i];
                HumanItem _hTmp = _tmp.item3d.GetComponent<ItemObject>()._thisHuman;

                if (_hTmp._shirt != null || _hTmp._pant != null || _hTmp._shoes != null)
                {
                    int _tmpHumanNumber = _tmp._loadHumanNumber;

                    string _shirtName = "";
                    float _shirt_R = 0f;
                    float _shirt_G = 0f;
                    float _shirt_B = 0f;
                    if (_hTmp._shirt != null)
                    {
                        _shirtName = _hTmp._shirt.name;
                        _shirt_R = _hTmp._shirt_R;
                        _shirt_G = _hTmp._shirt_G;
                        _shirt_B = _hTmp._shirt_B;
                    }
                    string _pantName = "";
                    float _pant_R = 0f;
                    float _pant_G = 0f;
                    float _pant_B = 0f;
                    if (_hTmp._pant != null)
                    {
                        _pantName = _hTmp._pant.name;
                        _pant_R = _hTmp._pant_R;
                        _pant_G = _hTmp._pant_G;
                        _pant_B = _hTmp._pant_B;
                    }
                    string _shoesName = "";
                    float _shoes_R = 0f;
                    float _shoes_G = 0f;
                    float _shoes_B = 0f;
                    if (_hTmp._shoes != null)
                    {
                        _shoesName = _hTmp._shoes.name;
                        _shoes_R = _hTmp._shoes_R;
                        _shoes_G = _hTmp._shoes_G;
                        _shoes_B = _hTmp._shoes_B;
                    }

                    VRDBController.addDressInfo(_tmpHumanNumber, _shirtName, _shirt_R, _shirt_G, _shirt_B, _pantName, _pant_R, _pant_G, _pant_B, _shoesName, _shoes_R, _shoes_G, _shoes_B);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        for (int i = 0; i < _itemListControl._dataBaseSmallAnimation.Count; i++)
        {
            try
            {
                BigAniBar _tmp = _itemListControl._dataBaseBigAnimation[i];
                SmallAniBar _tmp1 = _itemListControl._dataBaseSmallAnimation[i];

                float _barX = _tmp.gameObject.transform.localPosition.x;
                Debug.Log(_barX);
                float _barWidth = _tmp1._aniBarWidth;
                string _animationName = _tmp1._animationName;
                string _animationText = _tmp.transform.GetChild(0).GetComponent<Text>().text;
                string _anibarName = _tmp._anibarName;
                int _actionHumanNumber = _tmp1._item._loadHumanNumber;
                int _moveOrState;
                int _actionOrFace;
                int _rotation;
                if (_tmp1._moveCheck)
                    _moveOrState = 1;
                else
                    _moveOrState = 0;
                if (_tmp1._actionOrFace)
                    _actionOrFace = 1;
                else
                    _actionOrFace = 0;
                if (_tmp1._rotation)
                    _rotation = 1;
                else
                    _rotation = 0;

                int _layerNumber = _tmp1._layerNumber;
                float _arriveX = _tmp1._arriveLocation.x;
                float _arriveY = _tmp1._arriveLocation.y;
                float _arriveZ = _tmp1._arriveLocation.z;

                int _originNumber = _tmp1._item._originNumber;


                VRDBController.addBigActionInfo(i, _barX, _barWidth, _animationName, _anibarName, _actionHumanNumber, _animationText);
                VRDBController.addSmallActionInfo(i, _actionHumanNumber, _moveOrState, _actionOrFace, _layerNumber, _barX, _barWidth, _arriveX, _arriveY, _arriveZ, _originNumber, _rotation);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        for (int i = 0; i < _itemListControl._voiceDBIndex; i++)
        {
            try
            {
                BigAniBar _tmp = _itemListControl._dataBaseBigVoice[i];
                SmallAniBar _tmp1 = _itemListControl._dataBaseSmallVoice[i];

                float _barX = _tmp.gameObject.transform.localPosition.x;
                float _barWidth = _tmp._aniBarWidth;
                string _voiceName = _tmp1._audio.name;
                string _voiceText = _tmp.transform.GetChild(0).GetComponent<Text>().text;
                int _actionHumanNumber = _tmp1._item._loadHumanNumber;
                int _dir_key = _tmp1._dir_key;
                int _originNumber = _tmp1._item._originNumber;

                Debug.Log("2020-04-05 HiddenMenuControl 1436번줄 _voiceText : " + _voiceText);

                VRDBController.AddBigVoiceInfo(i, _barX, _barWidth, _voiceName, _actionHumanNumber, _voiceText);
                VRDBController.AddSmallVoiceInfo(i, _actionHumanNumber, _dir_key, _voiceName, _barX, _barWidth, _originNumber);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        ///* 애니메이션 리스트 저장 */
        //UnityEditor.AssetDatabase.Refresh(); //에셋리로드
    }

    public void CoverDBPanel_Yes()
    {
        string filePath = string.Format("{0}/{1}", dir_path + "/Database", _saveText.text + ".sqlite");
        FileInfo fInfo = new FileInfo(filePath);
        _coverDBPanel.SetActive(false);
        fInfo.Delete();
        if (flag_createDB)
            StartCoroutine(CreateDB());
        else if (_isCoverDB)
        {
            if (_saveText.text.Equals(Static.STATIC._saveClickButton.name) == false)
            {
                fInfo = new FileInfo(_filePath + ".sqlite");

                if (fInfo.Exists)
                {
                    fInfo.Delete();
                }
                Destroy(_saveParentObject.transform.Find(_saveText.text).gameObject);
            }
            newBtn = false;
            StartCoroutine(CreateDB());
        }
    }

    /* 덮어 씌울건지 질의하는 패널 닫는 함수 */
    public void CoverDBPanel_No()
    {
        _isCoverDB = false;
        _coverDBPanel.SetActive(false);
    }
    /**
* date 2018.08.10
* author INHO
* desc
* 본래는, EXIT 를 누르면 바로 탈출했으나, Delete 및 EXIT 를 누르면,
* 종료를 확인하는지 물어보는 창이 뜨도록 만듦.
*/

    /* Dialog Part */
    public void OnClickExitDialog()
    {
        _menuPanel.SetActive(false);
        _exitDialog.SetActive(true);
    }

    public void OnClickDeleteDialog()
    {
        _deleteDialog.SetActive(true);
    }

    public void OnClickDelete()
    {
        /* 삭제를 누른다면, 해당 Save파일은 삭제! */
        Destroy(_deleteFile);

        StartCoroutine(DropFile());
    }

    IEnumerator DropFile()
    {
        /* 세이브 파일 삭제시, 해당 스크립트를 가져와 Resource 파일을 삭제하도록 만들어야됨. */
        VRDBController.Drop(_deleteFileName);

        yield return StartCoroutine(DeleteFile_Total());

        _deleteFile = null;
        _deleteDialog.SetActive(false);
    }

    IEnumerator DeleteFile_Total()
    {
        VRDBController.Total_Delete(_deleteFileName);
        yield return null;
    }

    public void OnClickCancelDialog()
    {
        _deleteDialog.SetActive(false);
        _exitDialog.SetActive(false);
    }

    /* Exit Part*/
    public void OnclickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }
}