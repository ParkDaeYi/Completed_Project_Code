using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ClickedItemControl : MonoBehaviour
{
    /**
 * date 2018.07.12
 * author Lugub
 * desc
 *  클릭한 객체의 삭제, 이동, 크기변환, 옷 바꾸기, 애니메이션, 보이스 기능을
 *  이 스크립트 내에서 다룰거임
 *  
 *  OnclickDeleteItem()
 *  OnclickRelocateItem()
 *  OnclickRescaleItem()
 *  OnclickAnimationInsert()
 *  OnclickVoiceInsert()
 *  OnclickDressChange()
 */
    [Header("Clicked Item")]
    public Item _clickedItem;
    public HumanItem _clickedHumanItem;
    public GameObject _clickedItemPanel;

    [Header("ClickedItemUI")]
    public GameObject _animationButton;
    public GameObject _voiceButton;
    public GameObject _dressChange;
    public GameObject _downPanel;
    public GameObject _SpriteObject;
    public GameObject _inputTextHolder;
    public GameObject _inputText;
    public InputField _inputField;

    [Header("ClickedItemSubMenu")]
    public ScrollRect _settingAnimationPanel; // 해당 애니메이션 View를 수정할 수 있도록
    public ScrollRect _settingHeadPanel; // 해당 헤드 View를 수정할 수 있도록
    public ScrollRect _settingHandPanel; // 해당 핸드 View를 수정할 수 있도록
    public ScrollRect _settingLegPanel; // 해당 레그 View를 수정할 수 있도록
    public GameObject _basePanel;
    public GameObject _reScaleCanvas;
    public GameObject _voiceCanvas;
    public GameObject _scrollView;
    public GameObject _dressCanvas;
    public GameObject _sampleButton;
    public GameObject _colorChangeCanvas;

    [Header("ItemListControl for DataBase")]
    public ItemListControl _itemListControl;

    [Header("SubMenuScript")]
    public LocateItem _reLocate;
    public ScaleControl _reScale;
    public FileLoad _fileLoad;
    public DressController _dressController;
    public MenuMiniOption _menuMiniOption;
    public MenuMiniOption _menuMiniOption2;
    public SearchInputField _searchInputField;
    public StartDBController _startDBController;

    [Header("CameraMove")]
    public CameraMoveAroun _cameraMoveAron;

    [Header("AnimationPanel")]
    public GameObject _actionAnimationPanel;
    public GameObject _headAnimationPanel;
    public GameObject _handAnimationPanel;
    public GameObject _legAnimationPanel;
    public GameObject _voiceAnimationPanel;

    [Header("Voice Relevant")]
    public GameObject _voiceMadeCanvas;
    public GameObject _manVoiceMenu;
    public GameObject _womanVoiceMenu;
    public GameObject _manVoiceButtons;
    public GameObject _womanVoiceButtons;
    public GameObject _manMenuTag;
    public GameObject _womanMenuTag;
    public GameObject _manTypeTag;
    public GameObject _womanTypeTag;
    public GameObject _manTypeMenu;
    public GameObject _womanTypeMenu;

    [Header("DressPanel")]
    public GameObject _sampleDressButton;
    public GameObject _shirtButtons;
    public GameObject _pantButtons;
    public GameObject _shoesButtons;
    /* 인물의 갯수만큼 옷 Panel은 동적으로 생성필요 */
    List<GameObject> _humanPanel = new List<GameObject>();
    public Vector3 _originPos;

    string dir_path;
    int idx = 0;

    private void Start()
    {
        dir_path = Static.STATIC.dir_path;
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    public IEnumerator ClickedItem_Start()
    {
        _clickedItemPanel.SetActive(false);
        _basePanel.SetActive(false);
        _clickedItem = null;

        string _path = dir_path + "/Database/";
        DirectoryInfo dir = new DirectoryInfo(_path);
        bool _directory = false, _file = false;
        foreach (FileInfo file in dir.GetFiles())
        {

            if (file.Extension.ToLower().Equals(".sqlite"))
            {
                string[] fileName = file.Name.Split('.');

                if (fileName[0].Equals("VoiceDirectory"))
                {
                    _directory = true;
                }
                else if (fileName[0].Equals("VoiceFile"))
                {
                    _file = true;
                }
                if (_directory && _file) break;
            }
        }

        _path = dir_path + "Resources/Voice/";    
        dir = new DirectoryInfo(_path);

        if (!_directory)
        {
            VRDBController.VoiceDirectory_Init();
            string tmp = dir.LastWriteTime.ToString();
            string[] lastWrite = tmp.Split(' ');
            VRDBController.VoiceDirectory_add(dir.Name, lastWrite[0], 0, 0);
            yield return StartCoroutine(GetVoiceDirectory(dir_path + "/Resources/Voice/", "", 0));
#if UNITY_EDITOR
            ImportAsset.NewImportAsset_File("Assets/DataBase/VoiceDirectory.sqlite");
#endif  
        }
        else
        {
            VRDBController.ConIn(dir_path + "/DataBase/VoiceDirectory");
        }

        VRDBController.VoiceDirectory_Start();

        if (!_file)
        {
            VRDBController.VoiceFile_Init();
            yield return StartCoroutine(GetVoiceFile(dir_path + "/Resources/Voice/", "Voice/", ""));
#if UNITY_EDITOR
            ImportAsset.NewImportAsset_File("Assets/DataBase/VoiceFile.sqlite");
#endif       
        }
        else
        {
            VRDBController.ConIn(dir_path + "/DataBase/VoiceFile");
        }

        VRDBController.VoiceFile_Start();
        VRDBController.VoiceAudio_Start();

        yield return null;
    }

    private IEnumerator GetVoiceFile(string cur_path, string l_path, string nxt_path)
    {
        string _path = cur_path + nxt_path;
        string _path2 = l_path + nxt_path;
        DirectoryInfo dir = new DirectoryInfo(_path);
        int cur_idx;
        if (!_startDBController.directoryString.ContainsKey(dir.Name)) cur_idx = 0;
        else cur_idx = _startDBController.directoryString[dir.Name];

        foreach (FileInfo f in dir.GetFiles())
        {
            if (f.Extension.ToLower().Equals(".meta")) continue;
            else if (f.Extension.Equals(".mp3")|| f.Extension.Equals(".wav")|| f.Extension.Equals(".aac")|| f.Extension.Equals(".flac"))
            {
                string[] fname = f.Name.Split('.');
                string tmp = f.LastWriteTime.ToString();
                string[] lastWrite = tmp.Split(' ');

                VRDBController.VoiceFile_add(f.Name, lastWrite[0], cur_idx);
                VRDBController.VoiceAudio_add(fname[0], _path2, cur_idx);
            }
        }

        foreach(DirectoryInfo d in dir.GetDirectories())
        {
            StartCoroutine(GetVoiceFile(_path, _path2, d.Name + "/"));
        }

        yield return null;
    }

    private IEnumerator GetVoiceDirectory(string cur_path, string nxt_path, int cur)
    {
        string _path = cur_path + nxt_path;
        DirectoryInfo dir = new DirectoryInfo(_path);
        int cur_idx = cur;
        foreach (DirectoryInfo d in dir.GetDirectories())
        {
            string tmp = d.LastWriteTime.ToString();
            string[] lastWrite = tmp.Split(' ');
            VRDBController.VoiceDirectory_add(d.Name, lastWrite[0], ++idx, cur_idx);
            StartCoroutine(GetVoiceDirectory(_path, d.Name + "/", idx));
        }
        yield return null;
    }

    /* FileLoading Method 구문은, 시작 할 때 딱 1번만 불러오면 되므로, ItemListControll.cs 으로 옮김. */

    /* ClickedMenu가 활성화 되었을 때 text값을 바뀌어서 무엇을 클릭했는지 표시 */
    public void ClickMenuActivate()
    {
        _clickedItemPanel.SetActive(true);

        /* 객체 Name Text 가 제대로 표시 되도록! */
        _inputTextHolder.GetComponent<Text>().enabled = true;
        _inputText.GetComponent<Text>().enabled = false;
        _inputField.text = "";

        /* 클릭메뉴 캔버스의 이름 변경 */
        _inputTextHolder.GetComponent<Text>().text = _clickedItem.itemName + _clickedItem._objectNumber;

        /* 외곽선 처리 - 보이게 */
        //_clickedItem.item3d.GetComponent<Outline>().enabled = true;

        /* Sprite 이미지 - 보이게 */
        _SpriteObject.SetActive(true);

        /* 해당 크기의 사이즈를 저장 */
        Vector3 _clickedItemSize = _clickedItem.item3d.GetComponent<Collider>().bounds.size;

        /* 이전 클릭의 기능 메뉴 안보이게*/
        BasePanelReset();

        /* 클릭한 객체의 _originNumber를 보고 필요한 버튼만 활성화
         *  아이템은 1000자리대
         *  사람은   2000자리대*/
        if (_clickedItem._originNumber < 2000)
        {
            /* 클릭된 사물 객체에 상세 Animation UI가 활성되어 있으면, 메뉴를 닫음. */
            if (_menuMiniOption._uiStatus) { _menuMiniOption.OnClickMinimumButton(); _menuMiniOption2.OnClickMinimumButton(); }

            _downPanel.SetActive(false);
        }
        else
        {
            _downPanel.SetActive(true);

            /* 클릭된 객체가 남자객체 일시.. -> 남성용 Animation 창 띄움. */
            if (_clickedItem._originNumber == 2001)
            {
                setAnimationPanel(_settingAnimationPanel, 1);
                setAnimationPanel(_settingHandPanel, 1);
                setAnimationPanel(_settingHeadPanel, 1);
                setAnimationPanel(_settingLegPanel, 1);
            }

            /* 클릭된 객체가 여자객체 일시.. -> 여성용 Animation 창 띄움. */
            if (_clickedItem._originNumber == 2000)
            {
                setAnimationPanel(_settingAnimationPanel, 2);
                setAnimationPanel(_settingHandPanel, 2);
                setAnimationPanel(_settingHeadPanel, 2);
                setAnimationPanel(_settingLegPanel, 2);
            }

            /* 클릭된 객체가 아이(현 : Daughter)객체 일시.. -> 아이용 Animation 창 띄움. */
            if (_clickedItem._originNumber == 2002)
            {
                setAnimationPanel(_settingAnimationPanel, 0);
                setAnimationPanel(_settingHandPanel, 0);
                setAnimationPanel(_settingHeadPanel, 0);
                setAnimationPanel(_settingLegPanel, 0);
            }

            /* 클릭된 객체가 3차년도 남자 객체(Woongin) 일시.. -> Woongin용 Animation 창 띄움 */
            if (_clickedItem._originNumber == 2003)
            {
                setAnimationPanel(_settingAnimationPanel, 3);
                setAnimationPanel(_settingHandPanel, 3);
                setAnimationPanel(_settingHeadPanel, 3);
                setAnimationPanel(_settingLegPanel, 3);
            }
        }
        /* 사람 객체 클릭 시 dressPanel을 조작할 수 있는 파츠의 버튼을 받아오도록 한다 */
        if (2000 <= _clickedItem._originNumber && _clickedItem._originNumber < 3000)
        {
            DressButtonUpdate();
        }
    }

    /* ClickedItemMenu 우측상단의 빨간색버튼을 눌렀을 때 메뉴를 닫음 */
    public void OnclickClickedItemMenuCancel()
    {
        /* 외곽선 처리 - 안보이게 */
        //if (_clickedItem.item3d != null)
        //    _clickedItem.item3d.GetComponent<Outline>().enabled = false;

        /* 메뉴 리셋 */
        BasePanelReset();
        ResetClickMenu();
        ResetAnimationPanel();

        ///* 카메라 제자리로 */
        //_cameraMoveAron.CameraZoomOut();
    }

    /* 클릭된 객체의 애니메이션 패널 활성/비활성 관리 */
    private void setAnimationPanel(ScrollRect panelSet, int index)
    {
        GameObject setViewport = panelSet.transform.GetChild(0).GetChild(0).gameObject;
        panelSet.content = setViewport.transform.GetChild(index).GetComponent<RectTransform>();

        for (int i = 0; i < setViewport.transform.childCount; i++)
        {
            if (i == index) setViewport.transform.GetChild(i).gameObject.SetActive(true);
            else setViewport.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /* ClickedItemMenu의 DeleteButton 눌렀을 경우*/
    public void OnclickDeleteItem()
    {

        GameObject _schedulerController = GameObject.Find("Controllers").transform.GetChild(0).gameObject;

        if (_clickedItem._originNumber >= 2000 && _clickedItem._originNumber <= 2005) //즉 사람이면 스케줄러 삭제 실행
        {
            HumanItem _history;
            _history = _clickedItem.item3d.GetComponent<ItemObject>()._thisHuman;
            if(_history._shirt!=null)_history._shirtName = _history._shirt.name;
            if(_history._pant!=null)_history._pantName = _history._pant.name;
            if (_history._shoes != null) _history._shoesName = _history._shoes.name;

            _schedulerController.GetComponent<SchedulerController>()._deleteObjectName = _clickedItem.item3d.transform.parent.gameObject.name;
            _schedulerController.GetComponent<SchedulerController>()._deleteObjectNmber = _clickedItem._objectNumber;

            Debug.Log("OnclickDeleteItem : " + _clickedItem.item3d.transform.parent.gameObject.name);
            GameObject _deleteSmallScheduler = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.Find(_clickedItem.item3d.transform.parent.gameObject.name).gameObject;
            GameObject _deleteBigScheduler = _deleteSmallScheduler.GetComponent<SmallSchedulerBar>()._bigScheduler;

            List<AniBarDelete> abd = new List<AniBarDelete>();
            List<AniBarVoiceDelete> abvd = new List<AniBarVoiceDelete>();

            for (int i = 0; i < _itemListControl._dataBaseSmallAnimation.Count; i++)
            {
                try
                {
                    BigAniBar _tmp = _itemListControl._dataBaseBigAnimation[i];
                    SmallAniBar _tmp1 = _itemListControl._dataBaseSmallAnimation[i];
                    if (_tmp.transform.parent.transform.parent.name != _clickedItem.item3d.transform.parent.gameObject.name) continue;
                    float _barX = _tmp.gameObject.transform.localPosition.x;
                    float _barWidth = _tmp1._aniBarWidth;
                    string _animationName = _tmp1._animationName;
                    string _animationText = _tmp.transform.GetChild(0).GetComponent<Text>().text;
                    string _parentName = _tmp.transform.parent.transform.parent.name;
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

                    abd.Add(new AniBarDelete(_barX, _barWidth, _animationName, _parentName, _animationText, _moveOrState, _actionOrFace, _layerNumber, _arriveX, _arriveY, _arriveZ, _originNumber, _rotation));
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            for (int i = 0; i < _itemListControl._dataBaseBigVoice.Count; i++)
            {
                try
                {
                    BigAniBar _tmp = _itemListControl._dataBaseBigVoice[i];
                    SmallAniBar _tmp1 = _itemListControl._dataBaseSmallVoice[i];
                    if (_tmp.transform.parent.transform.parent.name != _clickedItem.item3d.transform.parent.gameObject.name) continue;
                    float _barX = _tmp.gameObject.transform.localPosition.x;
                    float _barWidth = _tmp1._aniBarWidth;
                    string _voiceName = _tmp.transform.GetChild(0).GetComponent<Text>().text;
                    int _dir_key = _tmp1._dir_key;
                    int _originNumber = _tmp1._item._originNumber;
                    string _parentName = "";
                    if (_originNumber == 2000) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 8);
                    if (_originNumber == 2001) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 3);
                    if (_originNumber == 2002) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 5);
                    if (_originNumber == 2003) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 7);

                    abvd.Add(new AniBarVoiceDelete(_barX, _barWidth, _voiceName, _parentName, _dir_key, _originNumber));
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            /* DB에서 삭제해야 하는 부분 */
            Debug.Log(abvd.Count);
            HistoryController.pushHumanDeleteHist(_clickedItem._originNumber, _clickedItem._objectNumber, _clickedItem.itemName, _clickedItem.item3d.transform.parent.transform.position, _clickedItem.item3d.transform.eulerAngles, _clickedItem.item3d.transform.parent.transform.localScale, abd, abvd, _history);
            _itemListControl.DeleteDBHuman(_clickedItem._objectNumber);
            _schedulerController.GetComponent<SchedulerController>().SchedulerDelete();
            Debug.Log("사람 삭제");
        }
        else
        {
            Debug.Log(_clickedItem.item3d.transform.parent.transform.eulerAngles);
            /* DB에서 삭제해야 하는 부분 */
            HistoryController.pushObjectDeleteHist(_clickedItem._objectNumber, _clickedItem._originNumber, _clickedItem.itemName, _clickedItem.item3d.transform.parent.transform.position, _clickedItem.item3d.transform.parent.transform.eulerAngles, _clickedItem.item3d.transform.parent.transform.localScale);
            _itemListControl.DeleteDBitem(_clickedItem._objectNumber);

            Debug.Log("아이템 삭제");
        }

        /* 아이템 파괴 */
        Destroy(_clickedItem.item3d.transform.parent.gameObject);

        /* 메뉴 리셋 */
        BasePanelReset();
        ResetClickMenu();
        ResetAnimationPanel();

        ///* 카메라 제자리로 */
        //_cameraMoveAron.CameraZoomOut();

        /*
      History
       date   : 2018-11-26
       author : Lugup
       내  용 : Item Deleted
       실행시 : 객체를 제거함 - 객체와 연결된 애니메이션, 스케쥴러의 내용 전부 삭제
       취소시 : 제거했던 객체를 다시 불러옴, 객체와 연결됬던 애니메이션, 스케쥴러의 내용 전부 복구하는 작업.
      
       */


    }

    /* 메뉴 초기화 */
    public void ResetClickMenu()
    {
        /* Sprite 이미지 - 안 보이게 */
        _SpriteObject.SetActive(false);

        _clickedItem = null;
        _downPanel.SetActive(true);
        _clickedItemPanel.SetActive(false);
    }

    /**
* date 2018.07.20
* author Lugub
* desc
*  클릭메뉴에서 나온 6가지 버튼을 눌렀을 때 행동할 함수들이 밑에 있다.
*/

    /* 위치 재설정 */
    public void OnclickRelocateItem()
    {
        //if(_clickedItem._originNumber<2000) HistoryController.pushObjectHist(_clickedItem.item3d.transform.parent.gameObject, _clickedItem.item3d.transform.parent.localPosition, _clickedItem.item3d.transform.parent.localScale, _clickedItem._objectNumber, _clickedItem._originNumber, _clickedItem.item3d.transform.transform.rotation);
        //else 
        HistoryController.pushObjectHist(_clickedItem.item3d.transform.parent.gameObject, _clickedItem.item3d.transform.parent.localPosition, _clickedItem.item3d.transform.parent.localScale, _clickedItem._objectNumber, _clickedItem._originNumber, _clickedItem.item3d.transform.parent.transform.rotation);
        _clickedItem.item3d.layer = LayerMask.NameToLayer("Ignore Raycast");
        _reLocate.RelocateItem(_clickedItem);

        /* 위치 재설정 하는 객체가 인물일 경우, 인물 상태 초기화 및 줌 아웃 필요! */
        if (_clickedItem._originNumber >= 2000 && _clickedItem._originNumber < 3000)
        {
            //_clickedHumanItem._status = "Idle";

            //_cameraMoveAron.CameraZoomOut();
        }

        /*
     History
      date   : 2018-1q1-26
      author : Lugup
      내  용 : Relocate Item
      실행시 : 객체의 위치를 기존의 위치에서 새로운 위치로 바꿈
      취소시 : 객체의 위치를 새로운 위치에서 기존의 위치로 바꿈.

      */

    }

    /* 크기 조정 */
    public void OnclickRescaleItem()
    {

        if (_reScaleCanvas.activeSelf) //ReScaleCanvas가 활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화
        }
        else //ReScaleCanvas가 비활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화 후
            _basePanel.SetActive(true); //BasePanel 활성화
            _reScaleCanvas.SetActive(true); //ReScaleCanvas 활성화
        }

        /* ScaleControl.cs의 ScaleStart() */
        _reScale.ScaleStart(_clickedItem.item3d.transform.parent.gameObject);

        /*
     History
      date   : 2018-11-26
      author : Lugup
      내  용 : Item Resized
      실행시 : 객체의 사이즈를 기존의 사이즈에서 새로운 사이즈로 설정함.
      취소시 : 객체의 사이즈를 새로운 사이즈에서 기존의 사이즈로 설정함.

      */
    }
    /**
    * date 2018.08.01
* author INHO
* desc
* 각 버튼을 누를때, 팝업창 띄우기는 구현되나,
* 실행할 함수 구현 필요 (음성 누르면 음성추가나.. 애니메이션 누르면 추가된다거나..)
*/

    public void OnclickAnimationInsert()
    {
        BasePanelReset();
        //_basePanel.SetActive(true);

        /* 애니메이션 넣는 부분 */
    }

    public void OnclickVoiceInsert()
    {
        if (_voiceCanvas.activeSelf) //VoiceCanvas가 활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화
            _voiceCanvas.SetActive(false);
            _scrollView.SetActive(false);
        }
        else //VoiceCanvas가 비활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화 후
            _voiceCanvas.SetActive(true); //VoiceCanvas 활성화
            _scrollView.SetActive(true);     
        }
    }

    /* VoiceMadeButton을 눌렀을 때 작동하는 함수 */
    public void OnClickVoiceMadeButton()
    {
        /* VoiceCanvas 비활성화 */
        _voiceCanvas.SetActive(false);

        /* VoiceMadeCanvas 활성화 */
        _voiceMadeCanvas.SetActive(true);
    }

    /* VoiceCanvas의 ManMenuTag를 클릭 했을 때*/
    public void OnClickVoiceManMenuTag()
    {
        /* ManVoiceMenu 활성화 */
        _manVoiceMenu.SetActive(true);

        /* ManMenuTag의 Text를 흰 색상으로 변경 */
        _manMenuTag.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);

        /* WomanVoiceMenu 비활성화 */
        _womanVoiceMenu.SetActive(false);

        /* WomanMenuTag의 Text를 검은 색상으로 변경 */
        _womanMenuTag.transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);

        /* ManMenuTag의 Button을 검은 색상으로 변경 */
        ColorBlock _cb = _manMenuTag.GetComponent<Button>().colors;
        _cb.normalColor = _cb.highlightedColor;
        _manMenuTag.GetComponent<Button>().colors = _cb;

        /* WomanMenuTag의 Button을 흰 색상으로 변경 */
        ColorBlock _cb2 = _womanMenuTag.GetComponent<Button>().colors;
        _cb2.normalColor = new Color(255f, 255f, 255f);
        _womanMenuTag.GetComponent<Button>().colors = _cb2;
        Static.STATIC._voiceGender = true;
    }

    /* VoiceCanvas의 WomanMenuTag를 클릭 했을 때*/
    public void OnClickVoiceWomanMenuTag()
    {
        /* WomanVoiceMenu 활성화 */
        _womanVoiceMenu.SetActive(true);

        /* WomanMenuTag의 Text를 흰 색상으로 변경 */
        _womanMenuTag.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);

        /* ManVoiceMenu 비활성화 */
        _manVoiceMenu.SetActive(false);

        /* ManMenuTag의 Text를 검은 색상으로 변경 */
        _manMenuTag.transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);

        /* WomanMenuTag의 Button을 검은 색상으로 변경 */
        ColorBlock _cb = _womanMenuTag.GetComponent<Button>().colors;
        _cb.normalColor = _cb.highlightedColor;
        _womanMenuTag.GetComponent<Button>().colors = _cb;

        /* ManMenuTag의 Button을 흰 색상으로 변경 */
        ColorBlock _cb2 = _manMenuTag.GetComponent<Button>().colors;
        _cb2.normalColor = new Color(255f, 255f, 255f);
        _manMenuTag.GetComponent<Button>().colors = _cb2;
        Static.STATIC._voiceGender = false;
    }

    /* VoiceMadeCanvas의 ManTypeTag 버튼을 클릭했을 때 */
    public void OnClickVoiceManTypeTag()
    {
        /* ManTypeMenu 활성화 */
        _manTypeMenu.SetActive(true);

        /* ManTypeTag의 Text를 흰 색상으로 변경 */
        _manTypeTag.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);

        /* WomanTypeMenu 비활성화 */
        _womanTypeMenu.SetActive(false);

        /* WomanTypeTag의 Text를 검은 색상으로 변경 */
        _womanTypeTag.transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);

        /* ManTypeTag의 Button을 검은 색상으로 변경 */
        ColorBlock _cb = _manTypeTag.GetComponent<Button>().colors;
        _cb.normalColor = _cb.highlightedColor;
        _manTypeTag.GetComponent<Button>().colors = _cb;

        /* WomanTypeTag의 Button을 흰 색상으로 변경 */
        ColorBlock _cb2 = _womanTypeTag.GetComponent<Button>().colors;
        _cb2.normalColor = new Color(255f, 255f, 255f);
        _womanTypeTag.GetComponent<Button>().colors = _cb2;
    }

    /* VoiceMadeCanvas의 WomanTypeTag 버튼을 클릭했을 때 */
    public void OnClickVoiceWomanTypeTag()
    {
        /* WomanTypeMenu 활성화 */
        _womanTypeMenu.SetActive(true);

        /* WomanTypeTag의 Text를 흰 색상으로 변경 */
        _womanTypeTag.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);

        /* ManTypeMenu 비활성화 */
        _manTypeMenu.SetActive(false);

        /* ManTypeTag의 Text를 검은 색상으로 변경 */
        _manTypeTag.transform.GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);

        /* WomanTypeTag의 Button을 검은 색상으로 변경 */
        ColorBlock _cb = _womanTypeTag.GetComponent<Button>().colors;
        _cb.normalColor = _cb.highlightedColor;
        _womanTypeTag.GetComponent<Button>().colors = _cb;

        /* ManTypeTag의 Button을 흰 색상으로 변경 */
        ColorBlock _cb2 = _manTypeTag.GetComponent<Button>().colors;
        _cb2.normalColor = new Color(255f, 255f, 255f);
        _manTypeTag.GetComponent<Button>().colors = _cb2;
    }

    public void OnclickDressChange()
    {
        //_dressController.ResetColorToggle();
        if (_dressCanvas.activeSelf) //DressCanvas가 활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화
        }
        else //DressCanvas가 비활성화되어 있으면
        {
            BasePanelReset(); //BasePanel 초기화 후
            _dressController._shirtButtons.SetActive(false);
            _dressController._pantButtons.SetActive(false);
            _dressController._shoesButtons.SetActive(false);
            _basePanel.SetActive(true); //BasePanel 활성화
            _dressCanvas.SetActive(true); //DressCanvas 활성화
        }

        //기본적으로 rightHandItem 표시
        _dressController.OnClickRightHand();

        /* 초기 상태의 옷을 humanItem에 저장 */
        //shirt의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _clickedItem.item3d.transform.Find("shirt").childCount; i++)
        {
            if (_clickedItem.item3d.transform.Find("shirt").GetChild(i).gameObject.activeSelf)
            {
                _clickedHumanItem._shirt = _clickedItem.item3d.transform.Find("shirt").GetChild(i).gameObject;
                break;
            }
        }
        //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _clickedItem.item3d.transform.Find("pant").childCount; i++)
        {
            if (_clickedItem.item3d.transform.Find("pant").GetChild(i).gameObject.activeSelf)
            {
                _clickedHumanItem._pant = _clickedItem.item3d.transform.Find("pant").GetChild(i).gameObject;
                break;
            }
        }
        //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        if (_clickedItem.item3d.transform.Find("shoes").GetChild(0).gameObject.activeSelf) //현재 발 상태가 normal
        {
            for (int i = 0; i < _clickedItem.item3d.transform.Find("shoes").GetChild(0).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_clickedItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject.activeSelf)
                {
                    _clickedHumanItem._shoes = _clickedItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject;
                    break;
                }
            }
        }
        else //abnormal
        {
            for (int i = 0; i < _clickedItem.item3d.transform.Find("shoes").GetChild(1).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_clickedItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject.activeSelf)
                {
                    _clickedHumanItem._shoes = _clickedItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject;
                    break;
                }
            }
        }

        _dressController.CheckUI();
    }

    /* x버튼을 눌렀을 경우 */
    public void OnclickBasePanelCancel()
    {
        BasePanelReset();
    }

    public void BasePanelReset()
    {
        _basePanel.SetActive(false);
        _reScaleCanvas.SetActive(false);
        _dressCanvas.SetActive(false);
        _voiceMadeCanvas.SetActive(false);
        _colorChangeCanvas.SetActive(false);
    }

    void DressPanelReset()
    {
        for (int i = 1; i < _shirtButtons.transform.childCount; i++)
        {
            Destroy(_shirtButtons.transform.GetChild(i).gameObject);
        }
        for (int i = 1; i < _pantButtons.transform.childCount; i++)
        {
            Destroy(_pantButtons.transform.GetChild(i).gameObject);
        }
        for (int i = 1; i < _shoesButtons.transform.childCount; i++)
        {
            Destroy(_shoesButtons.transform.GetChild(i).gameObject);
        }
    }

    /**
* date 2019.03.22
* author GS
* desc
* 활성화 된 AnimationPanel을 비활성화 하는 함수
*/
    public void ResetAnimationPanel()
    {
        /* 활성화 되어 있으면 비활성화 */
        if (_actionAnimationPanel.activeSelf) _actionAnimationPanel.SetActive(false);
        if (_headAnimationPanel.activeSelf) _headAnimationPanel.SetActive(false);
        if (_handAnimationPanel.activeSelf) _handAnimationPanel.SetActive(false);
        if (_legAnimationPanel.activeSelf) _legAnimationPanel.SetActive(false);
        if (_voiceAnimationPanel.activeSelf) _voiceAnimationPanel.SetActive(false);
    }

    /* date 2019.09.18
     * author HR
     * desc
     * 사람 객체 클릭 시 dressButton을 생성하는 함수
     */

    void DressButtonUpdate()
    {
        DressPanelReset();
        /* shirt 버튼 생성 */
        GameObject _cloth = _clickedItem.item3d.transform.Find("shirt").gameObject; //클릭된 사람 객체의 shirt
        for (int i = 0; i < _cloth.transform.childCount; i++)
        {
            MakingDressButton(_shirtButtons, _sampleDressButton, _cloth.transform.GetChild(i).gameObject);
        }
        /* pant 버튼 생성 */
        _cloth = _clickedItem.item3d.transform.Find("pant").gameObject; //클릭된 사람 객체의 pant
        for (int i = 0; i < _cloth.transform.childCount; i++)
        {
            MakingDressButton(_pantButtons, _sampleDressButton, _cloth.transform.GetChild(i).gameObject);
        }
        /* shoes 버튼 생성 */
        _cloth = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject; //normal shoes
        for (int i = 0; i < _cloth.transform.childCount; i++)
        {
            MakingDressButton(_shoesButtons, _sampleDressButton, _cloth.transform.GetChild(i).gameObject);
        }
        _cloth = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject; //abnormal shoes
        for (int i = 0; i < _cloth.transform.childCount; i++)
        {
            MakingDressButton(_shoesButtons, _sampleDressButton, _cloth.transform.GetChild(i).gameObject);
        }
    }

    void MakingDressButton(GameObject _parentObject, GameObject _sampleButton, GameObject _clothObject)
    {
        GameObject _instantiateSample = Instantiate(_sampleButton) as GameObject; //샘플 버튼 생성
        _instantiateSample.SetActive(true); //샘플 버튼은 비활성화 되어 있으므로 활성화
        _instantiateSample.name = _clothObject.name; //샘플 버튼의 이름 설정
        _instantiateSample.transform.GetChild(0).GetComponent<Text>().text = _clothObject.name; //샘플 버튼의 텍스트 설정
        _instantiateSample.transform.SetParent(_parentObject.transform); //샘플 버튼의 부모 설정
        _instantiateSample.transform.localScale = new Vector3(1, 1, 1);
        _instantiateSample.GetComponent<DressSlot>()._newCloth = _clothObject; //생성하면서 _newCloth 할당
    }
}