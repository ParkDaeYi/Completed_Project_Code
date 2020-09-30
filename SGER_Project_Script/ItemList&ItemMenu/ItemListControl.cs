using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class ItemListControl : MonoBehaviour
{
    /**
* date 2018.07.12
* author Lugub
* desc
*  생성할 아이템 리스트
*  생성한 아이템 리스트
*  두가지를 다루는 스크립트.
*  만들기 전의 Slot, Inventory, Itemdb 등이 여기에 포함됨
*  
*  그리고 혹시몰라서 _itemTable생성
*  
*/
    //public List<Item> _itemForMenu = new List<Item>();
    //public List<Item> _itemForPlace = new List<Item>();
    //public List<Item> _itemForHuman = new List<Item>();

    /* 인물의 갯수만큼 Dress창도 있어야 되므로, 동적생성 방식 선택 
    [Header("Dress")]
    public List<GameObject> _dressForHuman = new List<GameObject>();
    public GameObject _sampleDressPanel;
    public GameObject _dressParent;
    */

    /* 손에 든 물건도, 해당 스크립트에서 생성. */
    [Header("Dress")]
    public GameObject _rightHandPanel;
    public GameObject _leftHandPanel;
    public GameObject _rightButtonSample;
    public GameObject _leftButtonSample;

    [Header("DataBase")]
    public List<Item> _dataBaseItem = new List<Item>();
    public List<Item> _dataBaseWall = new List<Item>();
    public List<Item> _dataBaseHuman = new List<Item>();
    public List<BigAniBar> _dataBaseBigAnimation = new List<BigAniBar>();
    public List<SmallAniBar> _dataBaseSmallAnimation = new List<SmallAniBar>();
    public List<GameObject> _dataBaseBigBar = new List<GameObject>(); //큰스케줄러
    public List<GameObject> _dataBaseSmallbar = new List<GameObject>(); //작은스케줄러
    public List<BigAniBar> _dataBaseBigVoice = new List<BigAniBar>(); //큰 보이스
    public List<SmallAniBar> _dataBaseSmallVoice = new List<SmallAniBar>(); //작은 보이스 

    /* 구성 <int, Item> 으로 되어있어서 _originNumber로 검색하면 그 아이템이 나오게 만들었다 */
    /* 이 Dictionary는 아이템을 Load할때 사용하고, 1차년도의 ConstTable을 대신한다. */
    public Dictionary<int, Item> _itemTable = new Dictionary<int, Item>();

    public int _itemDBIndex = 0; // 사물용 동적 생성 번호 //사람을 생성해도 값이 올라감
    public int _humanDBIndex = 0; // 사람용 동적 생성 번호
    public int _wallDBIndex = 0;
    public int _actionDBIndex = 0;
    public int _voiceDBIndex = 0;

    [Header("Inventory")]
    public GameObject _sampleButton; //객체 클릭 -> 생성용
    public GameObject _humanSampleButton; //사람 객체 클릭 -> 생성용
    public GameObject _placeSampleButton; //장소 클릭 -> 이동용
    public GameObject _dressSampleButton; //드레스 객체 버튼
    public GameObject _humanInstantiatePlace;
    public GameObject _placeInstantiatePlace;
    public GameObject _inputFieldInstantiatePlace;

    [Header("ItemCategorieViewPort")]
    public GameObject _chairInstantiatePlace;
    public GameObject _ShelfInstantiatePlace;
    public GameObject _TVInstantiatePlace;
    public GameObject _tableInstantiatePlace;
    public GameObject _objectInstantiatePlace;

    [Header("PlaceCategorieViewPort")]
    public GameObject _planeInstantiatePlace;
    public GameObject _carInstantiatePlace;

    [Header("Using FileLoad - 드래그앤드랍으로 연결 필요(UI)")]
    public GameObject _actionViewPort_woman;
    public GameObject _actionViewPort_man;
    public GameObject _actionViewPort_baby;
    public GameObject _actionViewPort_woongin;
    public GameObject _headViewPort_woman;
    public GameObject _headViewPort_man;
    public GameObject _headViewPort_baby;
    public GameObject _headViewPort_woongin;
    public GameObject _handViewPort_woman;
    public GameObject _handViewPort_man;
    public GameObject _handViewPort_baby;
    public GameObject _handViewPort_woongin;
    public GameObject _legViewPort_woman;
    public GameObject _legViewPort_man;
    public GameObject _legViewPort_baby;
    public GameObject _legViewPort_woongin;
    public GameObject _voiceViewPort;

    [Header("Animation Sample Button")]
    public GameObject _actionSampleButton;
    public GameObject _lowerBodySampleButton;
    public GameObject _UpperBodySampleButton;
    public GameObject _legAnimationSampleButton;
    public GameObject _handAnimationSampleButton;
    public GameObject _faceAnimationSampleButton;

    [Header("Slot에 대한 동적 생성용")]
    public GameObject _backWall;
    public RenderTexture _backGroundTexture;
    public GameObject _spriteOfSet;

    [Header("SubMenuScript")]
    public FileLoad _fileLoad;
    public SearchInputField _searchInputField;

    /* Dress 창을 생성해 줄때 위치를 잡아주기 위해 Vector3 생성 */
    Vector3 _dressPanelVec;

    //    public List<ActionItems> ActionItem = new List<ActionItems>();

    /**
* date 2019.08.19
* author Day
* desc
*  List 는 이제 사용하지 않을 거임.
*  => List를 쓰면 Primary key 를 구분하기가 힘들어짐.
*  그래서 Dictionary를 사용
*  _itemsTable 을 이전에 사용하고 있던 List 처럼 human, item, place 를 저장
*  _itemsImages 는 이제 Item 에 Sprite 를 저장하지 않기 때문에 새로 만들었음.
*/
    static private Dictionary<string, Dictionary<int, Item>> _itemsTable;
    static private Dictionary<string, Dictionary<int, Sprite>> _itemsImages;
    static private Dictionary<string, Dictionary<int, GameObject>> _textureDic;

    public GameObject[] _house;
    public GameObject _inDoor;
    public GameObject _houseSampleButton;
    public GameObject _houseInstantiatePlace;
    public GameObject _buildSampleButton;
    public GameObject _buildInstantiatePlace;
    List<string> _slotName;

    string _url;
    string dir_path;

    private void Awake()
    {
        _itemsTable = new Dictionary<string, Dictionary<int, Item>>();
        _itemsImages = new Dictionary<string, Dictionary<int, Sprite>>();
        _textureDic = new Dictionary<string, Dictionary<int, GameObject>>();
        _searchInputField._slotTable = new Dictionary<string, GameObject>();
        _slotName = new List<string>();
        _url = "file://D:/Ass/";

    }

    /**
* date 2018.07.12
* author Lugub
* desc
*  오브젝트를 파일에서 가져옴
*  가져온 오브젝트로 데이터베이스 구성
*  데이터베이스 구성 후 슬롯 만듬
*/
    /**
* date 2019.08.19
* author Day
* desc
*  void Start() 는 AssetBundle을 사용하지 않을 때 사용
*  => Resources/Item 에 3dObject 와 Images 폴더 존재
*  즉, 작업용임.
*  IEnumerator Start() 는 AssetBundle 을 사용할 때
*  => Resources/Item 에 3dObject 와 Images 폴더 없음
*  서버에서 로컬로 받아와 사용
*/
   IEnumerator Start()
    {
        /* Dress 창을 생성해 줄때 위치 설정을 위해 Vector3 생성 */
        //_dressPanelVec = _sampleDressPanel.transform.localPosition;
        dir_path = Static.STATIC.dir_path;
        GetObjectFromFolder();
        yield return StartCoroutine(MakingSlot());
        yield return StartCoroutine(MakingSlot_House());
        Making_Tire();
        FileLoading();
    }
    //IEnumerator Start()
    //{
    //    yield return StartCoroutine(AssetBundlePatch.Instance.Patch(_url));
    //    //컴퓨터의 임의의 Cache폴더에 용량이 쌓이는 것을 방지하기 위해
    //    //테스트시에는 Caching.CleanCache()를 써주지만
    //    //서버에서 바로 불러오는 것이 아닌 로컬에 저장하여 사용하므로
    //    //지워주는게 맞을 듯,,, 문제 시 삭제
    //    Caching.ClearCache();

    //    yield return StartCoroutine(GetObjectFromFolder_Bundle());
    //    /*
    //     * AssetBundle에 담긴 내용은 _itemsTable에 저장 하였으므로
    //     * AssetBundleManager.cs 에 있는 Dictionary에 AssetBundle을 저장해 둘
    //     * 필요가 없음
    //     * => 메모리만 차지함.
    //     */
    //    AssetBundlePatch.Instance.RemoveAllAssetBundles();
    //    MakingSlot();
    //    MakingSlot_House();
    //    FileLoading();
    //}

    /**
* date 2018.07.12
* author Lugub
* desc
*  오브젝트를 파일에서 가져옴
*  => 나중에는 오브젝트 DB에서 가져올거임 일단은
*  1차년도때 했던 방식으로 가져옴
*  지금은 getObjectFromFolder()함수를 쓰지만 
*  
*  ※나중에는 getObjectFromDB()로 바꿔야함
*  
*  기본적인 가구(Furniture)라던가 우리가 배치할때 사용할 것들은
*  1000자리대의 OriginNumber를 가지게끔 만들었다.
*  그리고 사람 객체는 2000자리대
*  장소는 3000자리대의 OriginNumber를 가지게 했다.
*  
* date 2018.08.08
* author INHO
* desc
* 추가적으로, HandItem 의 경우도, 한 번 폴더에서 가져오면 끝이기 때문에,
* 같은 역할을 하는 스크립트에 같이 넣음.
*/
    /**
* date 2019.08.19
* author Day
* desc
*  기존의 틀은 유지하되 대부분 갈아 엎음.
*  dicKey를 사용하여 Dictionary 처음 키를 결정
*  Dictionary[dicKey] 의 키는 giving_OriginNumber 로 결정
*  giving_OriginNumber 는 각 파일에 있는 넘버로 지정해줌.
*/
    void GetObjectFromFolder()
    {
        int giving_OriginNumber;
        string folderpath = "";
        string dicKey = "";
        string resourcesPath = "";

        for (int i = 1; i <= 5; i++)
        {
            switch (i)
            {
                case 1: folderpath = dir_path + "/Resources/Item/Items/3dObject"; dicKey = "items"; resourcesPath = "Item/Items/3dObject"; break;
                case 2: folderpath = dir_path + "/Resources/Item/Human/3dObject"; dicKey = "human"; resourcesPath = "Item/Human/3dObject"; break;
                case 3: folderpath = dir_path + "/Resources/Item/Place/3dObject"; dicKey = "place"; resourcesPath = "Item/Place/3dObject"; break;
                case 4: folderpath = dir_path + "/Resources/Item/Wall/Build"; dicKey = "wall"; resourcesPath = "Item/Wall/Build"; break;
                case 5: folderpath = dir_path + "/Resources/Item/HandItem/3dObject"; dicKey = "handitem"; resourcesPath = "Item/HandItem/3dObject"; break;
            }
            DirectoryInfo dir = new DirectoryInfo(folderpath);

            foreach (FileInfo file in dir.GetFiles())
            {
                if (!file.Extension.ToLower().Equals(".meta"))
                {

                    string[] fileName = file.Name.Split('.');
                    string[] objNnum = fileName[0].Split('_');

                    giving_OriginNumber = Convert.ToInt32(objNnum[0]);

                    if (fileName[1].Equals("prefab"))
                    {
                        Item tmp = null;
                        GameObject tmpobj = Resources.Load<GameObject>(string.Format("{0}/{1}", resourcesPath, fileName[0]));

                        if (!_itemsTable.ContainsKey(dicKey))
                        {
                            Dictionary<int, Item> _ii = new Dictionary<int, Item>();

                            _itemsTable.Add(dicKey, _ii);
                        }
                        if (!_itemsTable[dicKey].ContainsKey(giving_OriginNumber))
                        {
                            if (tmpobj != null)
                            {
                                switch (i)
                                {
                                    case 1: tmp = AddItem(fileName[0], giving_OriginNumber, tmpobj); break;
                                    case 2: tmp = AddHuman(fileName[0], giving_OriginNumber, tmpobj); break;
                                    case 3: tmp = AddPlace(fileName[0], giving_OriginNumber, tmpobj); break;
                                    case 4: tmp = AddWall(fileName[0], giving_OriginNumber, tmpobj); break;
                                    case 5: tmp = AddHandItem(fileName[0], giving_OriginNumber, tmpobj); break;
                                }

                                _itemsTable[dicKey].Add(giving_OriginNumber, tmp);
                            }
                        }

                    }
                }
            }

            switch (i)
            {
                case 1: folderpath = dir_path + "/Resources/Item/Items/Images"; dicKey = "items"; resourcesPath = "Item/Items/Images"; break;
                case 2: folderpath = dir_path + "/Resources/Item/Human/Images"; dicKey = "human"; resourcesPath = "Item/Human/Images"; break;
                case 3: folderpath = dir_path + "/Resources/Item/Place/Images"; dicKey = "place"; resourcesPath = "Item/Place/Images"; break;
                case 4: continue;
                case 5: folderpath = dir_path + "/Resources/Item/HandItem/Images"; dicKey = "handitem"; resourcesPath = "Item/HandItem/Images"; break;
            }
            dir = new DirectoryInfo(folderpath);

            foreach (FileInfo file in dir.GetFiles())
            {
                if (!file.Extension.ToLower().Equals(".meta"))
                {

                    string[] fileName = file.Name.Split('.');
                    string[] objNnum = fileName[0].Split('_');

                    giving_OriginNumber = Convert.ToInt32(objNnum[0]);

                    if (fileName[1].Equals("png"))
                    {
                        Sprite tmpspr = Resources.Load<Sprite>(string.Format("{0}/{1}", resourcesPath, fileName[0]));

                        if (!_itemsImages.ContainsKey(dicKey))
                        {
                            Dictionary<int, Sprite> _is = new Dictionary<int, Sprite>();

                            _itemsImages.Add(dicKey, _is);
                        }
                        if (!_itemsImages[dicKey].ContainsKey(giving_OriginNumber))
                        {
                            if (tmpspr != null)
                            {
                                _itemsImages[dicKey].Add(giving_OriginNumber, tmpspr);
                            }
                        }
                    }
                }
            }

        }//for 문 end

        /* 손에 들 아이템을 폴더에서 가져온다. */
        //List<int> _tmp = new List<int>(_itemsImages["handitem"].Keys);
        //_fileLoad.FileLoadControll(_tmp, _rightHandPanel, _rightButtonSample, 1);
        //_fileLoad.FileLoadControll(_tmp, _leftHandPanel, _leftButtonSample, 1);
        //_fileLoad.FileLoadControll("Assets/Resources/HandItem/Image/", ".png", _rightHandPanel, _rightButtonSample, 1);
        //_fileLoad.FileLoadControll("Assets/Resources/HandItem/Image/", ".png", _leftHandPanel, _leftButtonSample, 1);
    }

    /**
* date 2019.08.19
* author Day
* desc
*  위에 GetObjectFromFolder 형식을 맞춤
*  GetAssetBundle 에서 AssetBundle 을 가져옴
*  AssetBundleManifest 에 저장 되어 있는 경로를 이용해 객체를 가져오는 방식
*/
    IEnumerator GetObjectFromFolder_Bundle()
    {
        string localPath = dir_path + "/AssetBundles/";

        for (int i = 1; i <= 5; i++)
        {

            string bundleName = "";

            switch (i)
            {
                case 1: bundleName = "items"; break;
                case 2: bundleName = "human"; break;
                case 3: bundleName = "place"; break;
                case 4: bundleName = "wall"; break;
                case 5: bundleName = "handitem"; break;
            }

            //- Assets/Resources/Item/Human/3dObject/Daughter.prefab

            yield return StartCoroutine(AssetBundlePatch.Instance.LoadAssetBundle(localPath, bundleName + ".unity3d", false));

            AssetBundle ab = AssetBundlePatch.Instance.GetAssetBundle(localPath, bundleName + ".unity3d");

            string[] bundlePath = ab.GetAllAssetNames();

            int pathLength;
            int nameLength;

            foreach (string path in bundlePath)
            {
                string[] extension = path.Split('.');
                //Debug.Log(extension[0] + " + " + extension[1]);

                string[] obj = extension[0].Split('/');
                //Debug.Log(obj[0] + " + " + obj[1] + " + " + obj[2] + " + " + obj[3] + " + " + obj[4] + " + " + obj[5]);

                string[] objName = obj[5].Split('_');

                pathLength = obj.Length;
                nameLength = objName.Length;

                if (i == 2 || i == 5)
                {
                    nameLength += 1;
                }

                if ((pathLength == 6) && (nameLength == 3))
                {
                    Debug.Log(bundleName + " : " + obj[5]);
                    int giving_OriginNumber = Convert.ToInt32(objName[0]);

                    if (extension[1].Equals("prefab"))
                    {
                        Item tmp = null;
                        GameObject tmpobj = ab.LoadAsset<GameObject>(obj[5]);

                        if (!_itemsTable.ContainsKey(bundleName))
                        {
                            Dictionary<int, Item> _ii = new Dictionary<int, Item>();

                            _itemsTable.Add(bundleName, _ii);
                        }
                        if (!_itemsTable[bundleName].ContainsKey(giving_OriginNumber))
                        {
                            if (tmpobj != null)
                            {
                                switch (i)
                                {
                                    case 1: tmp = AddItem(obj[5], giving_OriginNumber, tmpobj); break;
                                    case 2: tmp = AddHuman(obj[5], giving_OriginNumber, tmpobj); break;
                                    case 3: tmp = AddPlace(obj[5], giving_OriginNumber, tmpobj); break;
                                    case 4: tmp = AddWall(obj[5], giving_OriginNumber, tmpobj); break;
                                    case 5: tmp = AddHandItem(obj[5], giving_OriginNumber, tmpobj); break;
                                }

                                _itemsTable[bundleName].Add(giving_OriginNumber, tmp);
                            }
                        }
                    }
                    else if (extension[1].Equals("png"))
                    {
                        Sprite tmpspr = ab.LoadAsset<Sprite>(obj[5]);

                        if (!_itemsImages.ContainsKey(bundleName))
                        {
                            Dictionary<int, Sprite> _is = new Dictionary<int, Sprite>();

                            _itemsImages.Add(bundleName, _is);
                        }
                        if (!_itemsImages[bundleName].ContainsKey(giving_OriginNumber))
                        {
                            if (tmpspr != null)
                            {
                                _itemsImages[bundleName].Add(giving_OriginNumber, tmpspr);
                                Debug.Log("Images_save_check");
                            }
                        }
                    }

                }
            }
        }

    }

    //나중에 void getObjectFromFolder()가 삭제되고 밑의 getObjectFromDB()가 활성화되어야함
    void GetObjectFromDB()
    {

    }

    /**
* date 2018.07.17
* author Lugub
* desc
*  각 메뉴별로 슬롯을 만드는 작업
*  _sampleButton을 통해서 만들었기 때문에 1차년도랑은 다르게 세부적인 크기조정은 생략했음.
*  
*/
    IEnumerator MakingSlot_House()
    {
        _house = new GameObject[_inDoor.transform.childCount];

        for (int i = 0; i < _house.Length; i++)
        {
            _house[i] = _inDoor.transform.GetChild(i).gameObject;

            GameObject _button = Instantiate(_houseSampleButton);

            _button.SetActive(true);

            _button.transform.SetParent(_houseInstantiatePlace.transform);

            _button.transform.localScale = new Vector3(1f, 1f, 1f);

            _button.name = _house[i].name;

            _button.transform.GetChild(0).GetComponent<Text>().text = _house[i].name;

            yield return null;
            //버튼에 OnClickPlaceButton() 함수 실행...버튼함수는 slot에 넣자
        }

        //foreach (int k in _placeImages)
        //{
        //    GameObject _parent = _placeInstantiatePlace;
        //    GameObject _slot = Instantiate(_placeSampleButton) as GameObject;
        //    _slot.SetActive(true);

        //    if (_slot.GetComponent<Slot>() != null)
        //    {
        //        _slot.GetComponent<Slot>().Start();
        //    }

        //    if (_itemsImages["place"].ContainsKey(k))
        //    {
        //        _slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemsImages["place"][k];

        //    }
        //    _slot.name = "" + k;
        //    _slot.GetComponent<Transform>().SetParent(_parent.transform);
        //    _slot.GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);

        //}

    }

    IEnumerator MakingSlot()
    {

        for (int i = 1; i <= 6; i++)
        {
            List<int> _tmpKey = null;
            string _dicKey = null;
            GameObject _parent = null;
            RectTransform _rect = null;
            RenderTexture _texture = null;
            float _buttonSampleSize = 0;
            int _wallSampleSize = 0;

            int _menusize = 0;

            switch (i)
            {
                /* case 1 (만들어질 슬롯 : 물체)의 경우, 카테고리에 따라 만들어질 parent가 다르므로, 밑의 소스에서 결정. */
                //List 에 해당 키(_dicKey)에 있는 키(originNumber) 값을 저장함.
                case 1: { _dicKey = "items"; _tmpKey = new List<int>(_itemsTable[_dicKey].Keys); _parent = null; break; }
                case 2:
                    {
                        _dicKey = "human";
                        _tmpKey = new List<int>(_itemsTable[_dicKey].Keys);
                        _parent = _humanInstantiatePlace;
                        _sampleButton = _humanSampleButton;
                        break;
                    }
                case 3:
                    {
                        _dicKey = "wall"; _tmpKey = new List<int>(_itemsTable[_dicKey].Keys); _parent = _buildInstantiatePlace;
                        _sampleButton = _buildSampleButton; break;
                    }
                case 4:
                    {
                        _dicKey = "place"; _tmpKey = new List<int>(_itemsTable[_dicKey].Keys); _parent = _placeInstantiatePlace;
                        _sampleButton = _placeSampleButton; break;
                    }
                case 5:
                    {
                        _dicKey = "handitem"; _tmpKey = new List<int>(_itemsTable[_dicKey].Keys); _parent = _rightHandPanel;
                        _sampleButton = _rightButtonSample; break;
                    }
                case 6:
                    {
                        _dicKey = "handitem"; _tmpKey = new List<int>(_itemsTable[_dicKey].Keys); _parent = _leftHandPanel;
                        _sampleButton = _leftButtonSample; break;
                    }
            }

            foreach (int k in _tmpKey)
            {
                /* 슬롯을 생성 - 새로운 버튼 추가 */
                GameObject _slot = Instantiate(_sampleButton) as GameObject;
                _slot.SetActive(true);
                Item tmp = null;
                if (_itemsTable[_dicKey].ContainsKey(k))
                {
                    tmp = _itemsTable[_dicKey][k];
                }
                /* 물건/장소 객체용 버튼일 때 Slot.cs 을 이용! */
                if (_slot.GetComponent<Slot>() != null)
                {
                    if (tmp != null)
                    {
                        _slot.GetComponent<Slot>().Start();
                        _slot.GetComponent<Slot>()._thisItem = tmp;
                    }
                }
                else if (_slot.GetComponent<HandSlot>() != null)
                {
                    if (tmp != null)
                    {
                        _slot.GetComponent<HandSlot>()._thisItem = tmp;
                    }
                }
                /* 사람 객체용 버튼일 때 HumanSlot.cs 을 이용! */
                else
                {
                    if (tmp != null)
                    {
                        _slot.GetComponent<HumanSlot>().Start();
                        _slot.GetComponent<HumanSlot>()._thisItem = tmp;
                    }
                }


                /* 현재 만들고자 하는것이 아이템 슬롯인가? */
                if (_dicKey.Equals("items"))
                {

                    /* 언더바(_) 를 기준으로 왼쪽은 카테고리 항목이름, 오른쪽은 물체 이름이 저장됨. */
                    string[] line = tmp.itemName.Split('_');
                    string lineName = line[2].Substring(0, 1).ToUpper() + line[2].Substring(1);

                    /* 아이템 슬롯을 만들때.. 카테고리 항목에 따라 만들어질 ViewPort가 다르므로 설정해준다! */
                    //AssetBundle로 저장 시 모든 문자는 소문자로 저장됨.
                    tmp.itemName = lineName;
                    //tmp.itemName = line[2];

                    if (line[1] == "chair") { _parent = _chairInstantiatePlace; }
                    else if (line[1] == "shelf") { _parent = _ShelfInstantiatePlace; }
                    else if (line[1] == "tv") { _parent = _TVInstantiatePlace; }
                    else if (line[1] == "table") { _parent = _tableInstantiatePlace; }
                    else if (line[1] == "object") { _parent = _objectInstantiatePlace; }

                    _texture = new RenderTexture(_backGroundTexture);
                    /* 2D 스프라이트 이미지를 가져오도록 객체를 미리 만들어 설정 */
                    Making2DSpriteImg(tmp.item3d, _wallSampleSize, _texture,line[1]);

                    _wallSampleSize += 50;

                    /* 물체에 관한 슬롯의 이미지는 동적 생성된 카메라를 통해 이미지 (스프라이트) 설정 */
                    _slot.transform.GetChild(1).GetComponent<RawImage>().texture = _texture;
                }

                /* 현재 만들고자 하는것이 장소 슬롯인가? */
                else if (_dicKey.Equals("place"))
                {
                    string[] line = tmp.itemName.Split('_');
                    string lineName = line[2].Substring(0, 1).ToUpper() + line[2].Substring(1);

                    tmp.itemName = lineName;

                    if (line[1] == "place") { _parent = _placeInstantiatePlace; }
                    else if (line[1] == "car") { _parent = _carInstantiatePlace; }

                    if (_itemsImages["place"].ContainsKey(tmp._originNumber))
                        _slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemsImages["place"][k];
                }
                else if (_dicKey.Equals("wall"))
                {
                    /* 언더바(_) 를 기준으로 왼쪽은 카테고리 항목이름, 오른쪽은 물체 이름이 저장됨. */
                    string[] line = tmp.itemName.Split('_');
                    string lineName = line[2].Substring(0, 1).ToUpper() + line[2].Substring(1);

                    /* 아이템 슬롯을 만들때.. 카테고리 항목에 따라 만들어질 ViewPort가 다르므로 설정해준다! */
                    tmp.itemName = lineName;
                    //tmp.itemName = line[2];
                    if (line[1] == "wall") { _parent = _buildInstantiatePlace; }
                    //else if (line[1] == "car") { _parent = _carInstantiatePlace; }

                    /* (물체를 제외한) 슬롯의 이미지 (스프라이트) 설정 */
                    //if (_itemsImages["place"].ContainsKey(tmp._originNumber))
                    //    _slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemsImages["place"][k];
                    GameObject texture = tmp.item3d.transform.GetChild(0).gameObject;

                    if (texture != null)
                    {
                        if (!_textureDic.ContainsKey(_dicKey))
                        {
                            Dictionary<int, GameObject> it = new Dictionary<int, GameObject>();

                            _textureDic.Add(_dicKey, it);
                        }
                        if (!_textureDic[_dicKey].ContainsKey(tmp._originNumber))
                        {
                            _textureDic[_dicKey].Add(tmp._originNumber, texture);
                        }
                    }
                }

                else if (_dicKey.Equals("handitem"))
                {
                    if (i == 5)
                    {
                        string[] line = tmp.itemName.Split('_');
                        string lineName = line[1].Substring(0, 1).ToUpper() + line[1].Substring(1);

                        tmp.itemName = lineName;
                    }
                    _slot.SetActive(true);

                    if (_itemsImages["handitem"].ContainsKey(tmp._originNumber))
                        _slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemsImages[_dicKey][k];
                }

                /* 현재 만들고자 하는것이 사람 슬롯인가? */
                else
                {
                    /* (물체를 제외한) 슬롯의 이미지 (스프라이트) 설정 */
                    if (_itemsImages["human"].ContainsKey(tmp._originNumber))
                        _slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemsImages["human"][k];
                }

                _rect = _parent.GetComponent<RectTransform>();

                _buttonSampleSize = _sampleButton.GetComponent<RectTransform>().rect.width;

                /*슬롯의 이름(유니티 툴 안에서의 이름) 설정*/
                _slot.name = tmp.itemName;
                _slot.transform.GetChild(0).GetComponent<Text>().text = tmp.itemName;

                /*슬롯의 부모객체 설정*/
                _slot.transform.SetParent(_parent.transform);

                /*슬롯의 크기 설정*/
                _slot.transform.localScale = new Vector3(1, 1, 1);

                if (!_dicKey.Equals("handitem"))
                {
                    GameObject _tmpSlot = Instantiate(_slot) as GameObject;
                    _tmpSlot.SetActive(false);
                    _tmpSlot.transform.SetParent(_inputFieldInstantiatePlace.transform);
                    _tmpSlot.transform.localScale = new Vector3(1, 1, 1);
                    //SearchInputField의 slotTable에 아이템네임과 버튼을 추가
                    if (!_searchInputField._slotTable.ContainsKey(tmp.itemName.ToLower()))
                        _searchInputField._slotTable.Add(tmp.itemName.ToLower(), _tmpSlot);
                    _slotName.Add(tmp.itemName.ToLower() + '\0');
                }
                /*버튼의 크기를 딱맞게 나오게
                 * 하기 위해서*/
                _menusize++;
            }

            /* 각 Viewport의 크기 수정*/
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _menusize * _buttonSampleSize + 30);
        }
        yield return null;
    }

    void Making_Tire()
    {
        _searchInputField.root = new SearchInputField.Trie();
        foreach(string str in _slotName)
        {
            _searchInputField.root.insert(str, 0);
        }
    }

    /* 각 Slot에 적용될 스프라이트 이미지를 3D 객체로부터 가져온다. */
    public void Making2DSpriteImg(GameObject _3dObject, int _instantiateLocation, RenderTexture _tex,string part)
    {
        /* 슬롯에 대한 BackGround 및 3D 객체 추가 */
        GameObject _backGround = Instantiate(_backWall) as GameObject;
        GameObject _exampleItem = Instantiate(_3dObject) as GameObject;

        /* 이름설정 및 부모 그룹 지정. */
        _backGround.name = _3dObject.name;
        _backGround.transform.SetParent(_spriteOfSet.transform);

        /* 생성된 Wall에 대한 위치 설정. */
        _backGround.transform.position = new Vector3(_instantiateLocation, -250, 0);

        _backGround.SetActive(true);

        /* 생성된 3D 객체 위치 설정. */
        _exampleItem.transform.position = _backGround.transform.position;

        /* 객체 크기에 따라 카메라 위치 조정 */
        if (part == "object")
        {
            GameObject _cam = _backGround.transform.GetChild(3).gameObject;
            Vector3 _pos = _exampleItem.transform.GetChild(0).transform.localPosition;
            _pos -= new Vector3(0, 0, 5) * _exampleItem.transform.GetComponentInChildren<Collider>().bounds.extents.z;
            _pos += new Vector3(0, 5, 0) * _exampleItem.transform.GetComponentInChildren<Collider>().bounds.extents.y;
            _cam.transform.localPosition = _pos;
            _cam.transform.LookAt(_exampleItem.transform);
            //Debug.Log(_exampleItem.name + "bound : " + _exampleItem.transform.GetComponentInChildren<Collider>().bounds);
        }

        /* 해당 parent(Wall)에 동적 생성된 객체를 붙여준다. */
        _exampleItem.transform.SetParent(_backGround.transform.GetChild(2));

        _backGround.transform.GetChild(3).gameObject.GetComponent<Camera>().targetTexture = _tex;

        /* 생성되는 물체의 collider 전체 off */
        Collider[] _col = _exampleItem.GetComponentsInChildren<Collider>();
        foreach (Collider _box in _col)
        {
            _box.enabled = false;
        }
    }//Making2DSpriteImg Method end

    /**
* date 2018.07.22
* author Lugub
* desc
*  시작할 때 폴더에서 파일을 가져오는 역할을 함.
*  일단 정지시켜둠. 새로운 애니메이션이 어떤 형식으로 있는지를 모르고 음성, 드레스도 마찬가지이기 때문에
*  나중에 이런 형식이 정해지면 활성화.
*  FileLoad.cs의 FileLoadControll()함수 사용
*  
* data 2018.08.11
* author INHO
* desc
* FileLoading Method 는 시작할 때 1번만 불러오고, 해당 부분 Animation(머리,몸,다리 등)의
* 버튼을 클릭하면 보이는 Panel이 달라지는 방식으로 구현.
*/

    void FileLoading()
    {
        string _folderPath = dir_path + "/Resources/Animation/";
        string _extension = ".fbx";


        /* data 2019.03.15
         * author geun
         * desc 액션은 모든 객체에 대해 동일 하기 때문에 나눌 필요 없음 -> 휴머노이드 객체에 휴머노이드 애니메이션 적용
         * 현재 객체별 _actionViewPort가 각각 나누어져 있어 액션 폴더를 하나로 만들어도 어차피 소스는 3번 작성 -> _actionViewPort를 다른 VoewPort 처럼 하나로 만들면 한줄로 만들 수 있음 
         */
        /*
         action = 0 : Base Layer
         lowerbody = 1 : LowerBody
         upperbody = 2 : UpperBody
         leg = 3 : LegLayer
         hand = 4 : HandLayer
         face = 5 : FaceLayer
         */
        /*남자 */

        _fileLoad.FileLoadControll(_folderPath + "Action/Man/", _extension, _actionViewPort_man, _actionSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "LowerBody/Man/", _extension, _actionViewPort_man, _lowerBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "UpperBody/Man/", _extension, _handViewPort_man, _UpperBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Leg/Man/", _extension, _legViewPort_man, _legAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Hand/Man/", _extension, _handViewPort_man, _handAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Head/Man/", _extension, _headViewPort_man, _faceAnimationSampleButton, 1);

        /*여자*/
        _fileLoad.FileLoadControll(_folderPath + "Action/Woman/", _extension, _actionViewPort_woman, _actionSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "LowerBody/Woman/", _extension, _actionViewPort_woman, _lowerBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "UpperBody/Woman/", _extension, _handViewPort_woman, _UpperBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Leg/Woman/", _extension, _legViewPort_woman, _legAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Hand/Woman/", _extension, _handViewPort_woman, _handAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Head/Woman/", _extension, _headViewPort_woman, _faceAnimationSampleButton, 1);

        /*아이*/
        _fileLoad.FileLoadControll(_folderPath + "Action/Baby/", _extension, _actionViewPort_baby, _actionSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "LowerBody/Baby/", _extension, _actionViewPort_baby, _lowerBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "UpperBody/Baby/", _extension, _handViewPort_baby, _UpperBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Leg/Baby/", _extension, _legViewPort_baby, _legAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Hand/Baby/", _extension, _handViewPort_baby, _handAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Head/Baby/", _extension, _headViewPort_baby, _faceAnimationSampleButton, 1);

        /*3차년도 남자*/
        _fileLoad.FileLoadControll(_folderPath + "Action/Woongin/", _extension, _actionViewPort_woongin, _actionSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "LowerBody/Woongin/", _extension, _actionViewPort_woongin, _lowerBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "UpperBody/Woongin/", _extension, _handViewPort_woongin, _UpperBodySampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Leg/Woongin/", _extension, _legViewPort_woongin, _legAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Hand/Woongin/", _extension, _handViewPort_woongin, _handAnimationSampleButton, 1);
        _fileLoad.FileLoadControll(_folderPath + "Head/Woongin/", _extension, _headViewPort_woongin, _faceAnimationSampleButton, 1);

        _folderPath = dir_path + "/Resources/";

        /* Voice */
        //_fileLoad.FileLoadControll(_folderPath + "Voice", _extension, _voiceViewPort, _faceAnimationSampleButton, 1);

        /* Dress */
        /* Dress 소스는 , AddHuman Method에 병합! -> Human의 종류(남, 여)에 따라 옷(남, 여 전용) 있어야 되므로 */
    }

    /*
 * date 2019.08.19
 * author Day
 * desc
 * _itemsTable 은 static private 형식이므로 
 * itemListControl.cs 에서 Item을 반환해줘야함.
 * _itemsImages 도 마찬가지
 */
    public Item GetItem(int num, int key)
    {
        string _dicKey = "";
        switch (num)
        {
            case 1: _dicKey = "items"; break;
            case 2: _dicKey = "human"; break;
            case 3: _dicKey = "place"; break;
            case 4: _dicKey = "wall"; break;
            case 5: _dicKey = "handitem"; break;
        }
        if (_itemsTable[_dicKey].ContainsKey(key))
            return _itemsTable[_dicKey][key];
        else
            return null;
    }

    public Sprite GetImages(int num, int key)
    {
        string _dicKey = "";
        switch (num)
        {
            case 1: _dicKey = "items"; break;
            case 2: _dicKey = "human"; break;
            case 3: _dicKey = "place"; break;
            case 5: _dicKey = "handitem"; break;
        }
        if (_itemsImages[_dicKey].ContainsKey(key))
            return _itemsImages[_dicKey][key];
        else
            return null;
    }

    public GameObject GetTexture(int num, int key)
    {
        string _dicKey = "";
        switch (num)
        {

            case 4: _dicKey = "wall"; break;
        }

        if (_textureDic[_dicKey].ContainsKey(key))
            return _textureDic[_dicKey][key];
        else
            return null;
    }

    /**
* date 2018.07.12
* author Lugub
* desc
*      public List<Item> _itemForMenu = new List<Item>();
*      public List<Item> _itemForPlace = new List<Item>();
*      public List<Item>_itemForHuman = new List<Item>();
*      위에서 만든 각 리스트들안에 아이템을 추가해주는 것
*      나중에 이 리스트를 사용해서 각 3개의 Inventory를 구성함
*      
*/
    /**
* date 2019.08.19
* author Day
* desc
*  기존에 List 를 만들어 또 저장을 해주었지만
*  그런 불필요한 방식은 없앰.
*/
    Item AddItem(string _itemName, int _originNumber, GameObject gObj)
    {
        Item _item = new Item(_itemName
            , gObj
            , _originNumber);

        return _item;
    }

    Item AddPlace(string _placeName, int _originNumber, GameObject gObj)
    {
        Item _item = new Item(_placeName
            , gObj
            , _originNumber);

        return _item;
    }

    Item AddHandItem(string _handItem, int _originNumber, GameObject gObj)
    {
        Item _item = new Item(_handItem
            , gObj
            , _originNumber);
        return _item;
    }

    Item AddWall(string _wallName, int _originNumber, GameObject gObj)
    {
        Item _item = new Item(_wallName
        , gObj
         , _originNumber);

        return _item;
    }

    Item AddHuman(string _human, int _originNumber, GameObject gObj)
    {
        string[] _humanName = _human.Split('_');
        string _lineName = _humanName[1].Substring(0, 1).ToUpper() + _humanName[1].Substring(1);
        Item _item = new Item(_lineName
            , gObj
            , _originNumber);


        /* Human List가 생성될 때, Dress List도 같이 생성된다. */
        //GameObject _newPanel = Instantiate(_sampleDressPanel) as GameObject;

        /* Slot을 만들어줄 Parent 설정
        GameObject _sampleViewPort = _newPanel.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        GameObject _shirtViewPort = _newPanel.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        GameObject _pantViewPort = _newPanel.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
        GameObject _shoesViewPort = _newPanel.transform.GetChild(3).GetChild(0).GetChild(0).gameObject;
        */
        /* 해당 Human의 옷들의 목록 생성 */
        //_fileLoad.FileLoadControll("Assets/Resources/Dress/Image/" + _humanName + "/", ".png", _sampleViewPort, _dressSampleButton, 1);
        //_fileLoad.FileLoadControll("Assets/Resources/Dress/3dObject/" + _lineName + "/", ".prefab", _sampleViewPort, _dressSampleButton, 1);
        /*
        _fileLoad.FileLoadControll("Assets/Resources/Dress/3dObject/" + _lineName + "/shirt/", ".prefab", _shirtViewPort, _dressSampleButton, 1);
        _fileLoad.FileLoadControll("Assets/Resources/Dress/3dObject/" + _lineName + "/pant/", ".prefab", _pantViewPort, _dressSampleButton, 1);
        _fileLoad.FileLoadControll("Assets/Resources/Dress/3dObject/" + _lineName + "/shoes/", ".prefab", _shoesViewPort, _dressSampleButton, 1);
        */
        /*슬롯의 부모객체 설정*/
        //_newPanel.transform.SetParent(_dressParent.transform);

        /* List형식으로 추가 (ClickedItem 의 List만 SetActive(true)) 하기 위해서! */
        //_dressForHuman.Add(_newPanel);

        /* 슬롯의 이름(유니티 툴 안에서의 이름) 설정 */
        //_newPanel.name = _lineName + "Dress";

        /* 슬롯의 크기 및 위치 설정 */
        //_newPanel.transform.localScale = new Vector3(1, 1, 1);
        //_newPanel.transform.localPosition = _dressPanelVec;
        return _item;
    }

    /* Slot을 눌러서 객체를 하나씩 소환할때마다 DB에 저장하는 용도
     * LocateItem와 연결되어 있음 */
    public void AddDB(Item item)
    {
        _dataBaseItem.Add(item);
        _itemDBIndex++;
    }
    public void AddHuman(Item item)
    {
        _dataBaseHuman.Add(item);
        _humanDBIndex++;
    }

    public void AddWall(Item item)
    {
        _dataBaseWall.Add(item);
        _wallDBIndex++;
    }

    public void AddHumanDB(HumanItem item)
    {
        //_dateBaseHumanItem.Add(item);
    }

    /* Delete버튼을 통해서 객체를 삭제했을 때 */
    public void DeleteDBitem(int _objectNumber)
    {
        foreach (Item A in _dataBaseItem)
        {
            if (_objectNumber == A._objectNumber)
            {
                _dataBaseItem.Remove(A);
                break;
            }
        }
    }

    /* Delete버튼을 통해서 사람을 삭제했을 때 */
    public void DeleteDBHuman(int _humanNumber)
    {
        foreach (Item A in _dataBaseHuman)
        {
            if (_humanNumber == A._objectNumber)
            {
                _dataBaseHuman.Remove(A);
                break;
            }
        }
    }

    public void DeleteDBWall(int _wallNumber)
    {
        foreach (Item A in _dataBaseWall)
        {
            if (_wallNumber == A._objectNumber)
            {
                _dataBaseWall.Remove(A);
                break;
            }
        }
    }

    /**
* date 2018.07.23
* author Lugub
* desc
*     AnimationBar를 List만들어서 DataBase저장용의 List를 만듬
*     객체와는 다르게 선언이 "List<AnimationBar>" 이기 때문에 스크립트 단위로 저장
*     AddActionDB()     - AnimationBar가 하나 생성되었을 때 이걸 리스트에 저장함.
*     DeleteActionDB()  - AnimationBar를 삭제했을 경우 호출
*/


    public void AddActionDB(BigAniBar _bigAniBar, SmallAniBar _smallAniBar)
    {
        _dataBaseBigAnimation.Add(_bigAniBar);
        _dataBaseSmallAnimation.Add(_smallAniBar);
        _actionDBIndex++;
    }
    public void AddVoiceDB(BigAniBar _bigAniBar, SmallAniBar _smallAniBar)
    {
        _dataBaseBigVoice.Add(_bigAniBar);
        _dataBaseSmallVoice.Add(_smallAniBar);
        _voiceDBIndex++;
    }

    //액션 db삭제
    public void DeleteActionDB()
    {
        foreach (BigAniBar A in _dataBaseBigAnimation)
        {
            if (A._thisAniBar == null)
            {
                _dataBaseBigAnimation.Remove(A);
                _actionDBIndex--;
            }
        }
        foreach (SmallAniBar A in _dataBaseSmallAnimation)
        {
            if (A._thisAniBar == null)
            {
                _dataBaseSmallAnimation.Remove(A);
            }
        }
    }

    /* 초기화 */
    public void DeleteAllObjectAndAniBar()
    {
        /* 제거 Part - 현재 있는 객체 및 AnimationBar 전부 제거 후 로드 */
        /* 객체 제거*/
        foreach (Item A in _dataBaseItem)
        {
            /* 객체 삭제 */
            Destroy(A.item3d.transform.parent.gameObject);
        }
        /* 객체 DataBase 부분 제거*/
        _dataBaseItem.Clear();

        /* 스케쥴러의 AniBar 제거 */
        foreach (SmallAniBar A in _dataBaseSmallAnimation)
        {
            /* 객체 삭제 */
            Destroy(A);
        }
        foreach (BigAniBar A in _dataBaseBigAnimation)
        {
            Destroy(A);
        }
        ///* 스케쥴러 AnimationBar DataBase 부분 제거*/
        _dataBaseBigAnimation.Clear();
        _dataBaseSmallAnimation.Clear();

        /* 스케쥴러의 Voice 제거 */
        foreach (SmallAniBar A in _dataBaseSmallVoice)
        {
            /* 객체 삭제 */
            Destroy(A);
        }
        foreach (BigAniBar A in _dataBaseBigVoice)
        {
            Destroy(A);
        }

        _dataBaseBigVoice.Clear();
        _dataBaseSmallVoice.Clear();

        /* 사람 제거*/
        foreach (Item A in _dataBaseHuman)
        {
            /* 객체 삭제 */
            Debug.Log("Human 삭제");
            Destroy(A.item3d.transform.parent.gameObject);
        }
        /* 객체 DataBase 부분 제거*/
        _dataBaseHuman.Clear();

        /*간단 스케줄러 바 제거*/
        foreach (GameObject A in _dataBaseSmallbar)
        {
            Debug.Log("small 스케줄러 삭제");
            Destroy(A.gameObject);
        }
        _dataBaseSmallbar.Clear();

        /*상세 스케줄러 바 제거*/
        foreach (GameObject A in _dataBaseBigBar)
        {
            Debug.Log("big 스케줄러 삭제");
            Destroy(A.gameObject);
        }
        _dataBaseBigBar.Clear();


        _itemDBIndex = 0;
        _humanDBIndex = 0;
        _wallDBIndex = 0;
        _actionDBIndex = 0;
        _voiceDBIndex = 0;
    }
}
