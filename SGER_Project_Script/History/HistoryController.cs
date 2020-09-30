using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HistoryController : MonoBehaviour
{
    /*
     *  date 2018.12.11
     *  author skyde47
     *  desc
     *  System 내의 동작들에 대한 히스토리 동작 기술
     */
    //static Stack<HistoryInfo> stck_hist = new Stack<HistoryInfo>();
    LocateItem locateItem;
    ItemListControl itemListControl;
    ClickedPlaceControl clickedPlaceControl;
    DressController dressController;
    ScaleControl scaleControl;
    SchedulerController schedulerController;
    ClickedItemControl clickedItemControl;
    StartDBController voiceFileController;
    static Stack<int> _stackHistZ = new Stack<int>();// 0 애니바P,W 1 오브젝트P,S
    static Stack<int> _stackHistY = new Stack<int>();
    static Stack<AniBarPosAndWidth> _aniBarHist = new Stack<AniBarPosAndWidth>();
    static Stack<AniBarPosAndWidth> _aniBarHistY = new Stack<AniBarPosAndWidth>();
    static Stack<ObjectPosAndScale> _objectHist = new Stack<ObjectPosAndScale>();
    static Stack<ObjectPosAndScale> _objectHistY = new Stack<ObjectPosAndScale>();
    static Stack<AniBarCreate> _aniBarCreateHist = new Stack<AniBarCreate>();
    static Stack<AniBarCreate> _aniBarCreateHistY = new Stack<AniBarCreate>();
    static Stack<ObjectCreate> _objectCreateHist = new Stack<ObjectCreate>();
    static Stack<ObjectCreate> _objectCreateHistY = new Stack<ObjectCreate>();
    static Stack<ObjectDelete> _objectDeleteHist = new Stack<ObjectDelete>();
    static Stack<ObjectDelete> _objectDeleteHistY = new Stack<ObjectDelete>();
    static Stack<HumanDelete> _humanDeleteHist = new Stack<HumanDelete>();
    static Stack<HumanDelete> _humanDeleteHistY = new Stack<HumanDelete>();
    static Stack<AniBarDelete> _aniBarDeleteHist = new Stack<AniBarDelete>();
    static Stack<AniBarDelete> _aniBarDeleteHistY = new Stack<AniBarDelete>();
    static Stack<AniBarVoiceDelete> _aniBarVoiceDeleteHist = new Stack<AniBarVoiceDelete>();
    static Stack<AniBarVoiceDelete> _aniBarVoiceDeleteHistY = new Stack<AniBarVoiceDelete>();
    static Stack<DressChange> _dressChangeHist = new Stack<DressChange>();
    static Stack<DressChange> _dressChangeHistY = new Stack<DressChange>();
    static Stack<TilingChange> _tilingHist = new Stack<TilingChange>();
    static Stack<TilingChange> _tilingHistY = new Stack<TilingChange>();
    static Stack<TileChange> _tileHist = new Stack<TileChange>();
    static Stack<TileChange> _tileHistY = new Stack<TileChange>();
    static Stack<HandChange> _handHist = new Stack<HandChange>();
    static Stack<HandChange> _handHistY = new Stack<HandChange>();
    static Stack<DressRGBChange> _dressRGBChangeHist = new Stack<DressRGBChange>();
    static Stack<DressRGBChange> _dressRGBChangeHistY = new Stack<DressRGBChange>();

    private void Awake()
    {
        locateItem = GameObject.Find("ItemController").GetComponent<LocateItem>();
        itemListControl = GameObject.Find("ItemController").GetComponent<ItemListControl>();
        clickedPlaceControl = GameObject.Find("ClickedPlaceCanvas").GetComponent<ClickedPlaceControl>();
        dressController = GameObject.Find("UIController").GetComponent<DressController>();
        scaleControl = GameObject.Find("Canvas").transform.GetChild(2).GetChild(5).GetChild(0).GetChild(1).GetComponent<ScaleControl>();
        schedulerController = GameObject.Find("SchedulerController").GetComponent<SchedulerController>();
        clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();
        voiceFileController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    void Update()
    {
        //Debug.Log("_stackHistZ : " + _stackHistZ.Count);
        //Debug.Log("_stackHistY : " + _stackHistY.Count);
        /**
         * 키보드 입력 이벤트
         */
        //if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        //if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Z))
        if (Input.GetKeyDown(KeyCode.Z))
        {
            popHistoryStack();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            popHistoryStackY();
        }
    }

    /**
     * 이력 저장 Z
     */
    #region
    static public void pushAniBarHist(GameObject go, Vector3 pos, float width, int objectNum, string animationName, bool voice)
    {
        _stackHistZ.Push(0);
        _aniBarHist.Push(new AniBarPosAndWidth(go, pos, width, objectNum, animationName, voice));
    }

    static public void pushObjectHist(GameObject go, Vector3 pos, Vector3 scale, int objectNum, int originNum, Quaternion rot)
    {
        _stackHistZ.Push(1);
        _objectHist.Push(new ObjectPosAndScale(go, pos, scale, objectNum, originNum, rot));
    }

    static public void pushAniBarCreateHist(GameObject go, GameObject go1, string str, string str1, int objectNum, int voice)
    {
        _stackHistZ.Push(2);
        _aniBarCreateHist.Push(new AniBarCreate(go, go1, str, str1, objectNum, voice));
    }

    static public void pushObjectCreateHist(GameObject go, int originNum, int objectNum)
    {
        _stackHistZ.Push(3);
        _objectCreateHist.Push(new ObjectCreate(go, originNum, objectNum));
    }

    static public void pushObjectDeleteHist(int objectNum, int originNum, string itemName, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        _stackHistZ.Push(4);
        _objectDeleteHist.Push(new ObjectDelete(objectNum, originNum, itemName, pos, rot, scale));
    }

    static public void pushHumanDeleteHist(int originNum, int objectNum, string humanName, Vector3 pos, Vector3 rot, Vector3 scale, List<AniBarDelete> A, List<AniBarVoiceDelete> AA, HumanItem hu)
    {
        _stackHistZ.Push(5);
        _humanDeleteHist.Push(new HumanDelete(originNum, objectNum, humanName, pos, rot, scale, A, AA, hu));
    }

    static public void pushAniBarDeleteHist(float barX, float barWidth, string animationName, string parentName, string animationText, int moveOrState, int actionOrFace, int layerNumber,
        float arriveX, float arriveY, float arriveZ, int originNumber, int rotation)
    {
        _stackHistZ.Push(6);
        _aniBarDeleteHist.Push(new AniBarDelete(barX, barWidth, animationName, parentName, animationText, moveOrState, actionOrFace, layerNumber, arriveX, arriveY, arriveZ, originNumber, rotation));
    }

    static public void pushAniBarVoiceDeleteHist(float barX, float barWidth, string voiceName, string parentName, int voiceGender, int originNumber)
    {
        _stackHistZ.Push(7);
        _aniBarVoiceDeleteHist.Push(new AniBarVoiceDelete(barX, barWidth, voiceName, parentName, voiceGender, originNumber));
    }

    static public void pushDressChangeHist(string[] name, string prev, string objectName, int objectNum, int originNum, Vector3 pos)
    {
        _stackHistZ.Push(8);
        _dressChangeHist.Push(new DressChange(name, prev, objectName, originNum, objectNum, pos));
    }

    static public void pushTilingHist(int ori, int obj, Vector3 scale, int tile)
    {
        _stackHistZ.Push(9);
        _tilingHist.Push(new TilingChange(ori, obj, scale, tile));
    }

    static public void pushTileHist(int ori, int obj, int bul)
    {
        _stackHistZ.Push(10);
        _tileHist.Push(new TileChange(ori, obj, bul));
    }

    static public void pushHandHist(int ori, int obj, bool left, string name, Item hand)
    {
        _stackHistZ.Push(11);
        _handHist.Push(new HandChange(ori, obj, left, name, hand));
    }

    static public void pushDressRGBHist(int ori, int obj, int what, Color color)
    {
        _stackHistZ.Push(12);
        _dressRGBChangeHist.Push(new DressRGBChange(ori, obj, what, color));
    }
    /**
     * 이력 저장 Y
     */

    static public void pushAniBarHistY(GameObject go, Vector3 pos, float width, int objectNum, string animationName, bool voice)
    {
        _stackHistY.Push(0);
        _aniBarHistY.Push(new AniBarPosAndWidth(go, pos, width, objectNum, animationName, voice));
    }

    static public void pushObjectHistY(GameObject go, Vector3 pos, Vector3 scale, int objectNum, int originNum, Quaternion rot)
    {
        _stackHistY.Push(1);
        _objectHistY.Push(new ObjectPosAndScale(go, pos, scale, objectNum, originNum, rot));
    }

    static public void pushAniBarCreateHistY(GameObject go, GameObject go1, string str, string str1, int objectNum, int voice)
    {
        _stackHistY.Push(2);
        _aniBarCreateHistY.Push(new AniBarCreate(go, go1, str, str1, objectNum, voice));
    }

    static public void pushObjectCreateHistY(GameObject go, int originNum, int objectNum)
    {
        _stackHistY.Push(3);
        _objectCreateHistY.Push(new ObjectCreate(go, originNum, objectNum));
    }

    static public void pushObjectDeleteHistY(int objectNum, int originNum, string itemName, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        _stackHistY.Push(4);
        _objectDeleteHistY.Push(new ObjectDelete(objectNum, originNum, itemName, pos, rot, scale));
    }

    static public void pushHumanDeleteHistY(int originNum, int objectNum, string humanName, Vector3 pos, Vector3 rot, Vector3 scale, List<AniBarDelete> A, List<AniBarVoiceDelete> AA, HumanItem hu)
    {
        _stackHistY.Push(5);
        _humanDeleteHistY.Push(new HumanDelete(originNum, objectNum, humanName, pos, rot, scale, A, AA, hu));
    }

    static public void pushAniBarDeleteHistY(float barX, float barWidth, string animationName, string parentName, string animationText, int moveOrState, int actionOrFace, int layerNumber,
        float arriveX, float arriveY, float arriveZ, int originNumber, int rotation)
    {
        _stackHistY.Push(6);
        _aniBarDeleteHistY.Push(new AniBarDelete(barX, barWidth, animationName, parentName, animationText, moveOrState, actionOrFace, layerNumber, arriveX, arriveY, arriveZ, originNumber, rotation));
    }

    static public void pushAniBarVoiceDeleteHistY(float barX, float barWidth, string voiceName, string parentName, int voiceGender, int originNumber)
    {
        _stackHistY.Push(7);
        _aniBarVoiceDeleteHistY.Push(new AniBarVoiceDelete(barX, barWidth, voiceName, parentName, voiceGender, originNumber));
    }

    static public void pushDressChangeHistY(string[] name, string prev, string objectName, int objectNum, int originNum, Vector3 pos)
    {
        _stackHistY.Push(8);
        _dressChangeHistY.Push(new DressChange(name, prev, objectName, originNum, objectNum, pos));
    }

    static public void pushTilingHistY(int ori, int obj, Vector3 scale, int tile)
    {
        _stackHistY.Push(9);
        _tilingHistY.Push(new TilingChange(ori, obj, scale, tile));
    }

    static public void pushTileHistY(int ori, int obj, int bul)
    {
        _stackHistY.Push(10);
        _tileHistY.Push(new TileChange(ori, obj, bul));
    }

    static public void pushHandHistY(int ori, int obj, bool left, string name, Item hand)
    {
        _stackHistY.Push(11);
        _handHistY.Push(new HandChange(ori, obj, left, name, hand));
    }

    static public void pushDressRGBHistY(int ori, int obj, int what, Color color)
    {
        _stackHistY.Push(12);
        _dressRGBChangeHistY.Push(new DressRGBChange(ori, obj, what, color));
    }
    #endregion
    /**
     * 이력 복귀 
     */
    #region
    private void popHistoryStack()
    {
        if (_stackHistZ.Count > 0)
        {
            int _what = _stackHistZ.Pop();
            if (_what == 0) PopAniBarPAW();
            else if (_what == 1) PopObjectPAS();
            else if (_what == 2) PopAniBarCreate();
            else if (_what == 3) PopObjectCreate();
            else if (_what == 4) PopObjectDelete();
            else if (_what == 5) PopHumanDelete();
            else if (_what == 6) PopAniBarDelete();
            else if (_what == 7) PopAniBarVoiceDelete();
            else if (_what == 8) PopDressChange();
            else if (_what == 9) PopTiling();
            else if (_what == 10) PopTile();
            else if (_what == 11) PopHand();
            else if (_what == 12) PopDressRGBChange();
        }
    }

    private void popHistoryStackY()
    {
        if (_stackHistY.Count > 0)
        {
            int _what = _stackHistY.Pop();
            if (_what == 0) PopAniBarPAWY();
            else if (_what == 1) PopObjectPASY();
            else if (_what == 2) PopAniBarCreateY();
            else if (_what == 3) PopObjectCreateY();
            else if (_what == 4) PopObjectDeleteY();
            else if (_what == 5) PopHumanDeleteY();
            else if (_what == 6) PopAniBarDeleteY();
            else if (_what == 7) PopAniBarVoiceDeleteY();
            else if (_what == 8) PopDressChangeY();
            else if (_what == 9) PopTilingY();
            else if (_what == 10) PopTileY();
            else if (_what == 11) PopHandY();
            else if (_what == 12) PopDressRGBChangeY();
        }
    }
    #endregion
    
    public void Clear()
    {
        while (_stackHistZ.Count > 0)
        {
            int _what = _stackHistZ.Pop();
            if (_what == 0) _aniBarHist.Pop();
            else if (_what == 1) _objectHist.Pop();
            else if (_what == 2) _aniBarCreateHist.Pop();
            else if (_what == 3) _objectCreateHist.Pop();
            else if (_what == 4) _objectDeleteHist.Pop();
            else if (_what == 5) _humanDeleteHist.Pop();
            else if (_what == 6) _aniBarDeleteHist.Pop();
            else if (_what == 7) _aniBarVoiceDeleteHist.Pop();
            else if (_what == 8) _dressChangeHist.Pop();
            else if (_what == 9) _tilingHist.Pop();
            else if (_what == 10) _tileHist.Pop();
            else if (_what == 11) _handHist.Pop();
            else if (_what == 12) _dressRGBChangeHist.Pop();
        }
        while (_stackHistY.Count > 0)
        {
            int _what = _stackHistY.Pop();
            if (_what == 0) _aniBarHistY.Pop();
            else if (_what == 1) _objectHistY.Pop();
            else if (_what == 2) _aniBarCreateHistY.Pop();
            else if (_what == 3) _objectCreateHistY.Pop();
            else if (_what == 4) _objectDeleteHistY.Pop();
            else if (_what == 5) _humanDeleteHistY.Pop();
            else if (_what == 6) _aniBarDeleteHistY.Pop();
            else if (_what == 7) _aniBarVoiceDeleteHistY.Pop();
            else if (_what == 8) _dressChangeHistY.Pop();
            else if (_what == 9) _tilingHistY.Pop();
            else if (_what == 10) _tileHistY.Pop();
            else if (_what == 11) _handHistY.Pop();
            else if (_what == 12) _dressRGBChangeHistY.Pop();
        }
    }

    /*
     * Z
     */
    #region
    private void PopAniBarPAW()
    {
        AniBarPosAndWidth apaw = _aniBarHist.Pop();
        GameObject _go = apaw.getHistoryTarget();
        Vector3 _pos = apaw.getHistoryPosition();
        Vector3 _posY = apaw.getHistoryPosition();
        float _width = apaw.getHistoryWidth();
        float _widthY = apaw.getHistoryWidth();
        int _objectNum = apaw.getObjectNum();
        string _animationName = apaw.getAnimationName();
        bool _voice = apaw.getVoice();

        Debug.Log("_width : " + _width);

        if (_voice)
        {
            foreach (SmallAniBar A in itemListControl._dataBaseSmallVoice)
            {
                if (A._item._objectNumber == _objectNum && A._animationName == _animationName)
                {
                    _go = A._bigAniBar;
                    _posY = _go.transform.localPosition;
                    _go.transform.localPosition = _pos;
                    A._thisAniBar.transform.localPosition = _pos;
                    _widthY = _go.GetComponent<RectTransform>().rect.width;
                    _go.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _go.GetComponent<RectTransform>().rect.height);
                    break;
                }
            }
        }
        else
        {
            foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
            {
                if (A._item._objectNumber == _objectNum && A._animationName == _animationName)
                {
                    _go = A._bigAniBar;
                    _posY = _go.transform.localPosition;
                    _go.transform.localPosition = _pos;
                    A._thisAniBar.transform.localPosition = _pos;
                    _widthY = _go.GetComponent<RectTransform>().rect.width;
                    _go.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _go.GetComponent<RectTransform>().rect.height);
                    break;
                }
            }
        }
        pushAniBarHistY(_go, _posY, _widthY, _objectNum, _animationName, _voice);
    }

    private void PopObjectPAS()
    {
        Debug.Log("alright");
        ObjectPosAndScale apaw = _objectHist.Pop();
        GameObject _go = apaw.getHistoryTarget();
        Vector3 _pos = apaw.getHistoryPosition();
        Vector3 _posY = apaw.getHistoryPosition();
        Vector3 _scale = apaw.getHistoryScale();
        Vector3 _scaleY = apaw.getHistoryScale();
        int _objectNum = apaw.getObjectNum();
        int _originNum = apaw.getOriginNum();
        Quaternion _rot = apaw.getRot();

        if (_originNum >= 2000)
        {
            if (_originNum >= 4000)
            {
                foreach (Item A in itemListControl._dataBaseWall)
                {
                    if (A._objectNumber == _objectNum)
                    {
                        _go = A.item3d.transform.parent.gameObject;
                        break;
                    }
                }
            }
            else
            {
                foreach (Item A in itemListControl._dataBaseHuman)
                {
                    if (A._objectNumber == _objectNum)
                    {
                        _go = A.item3d.transform.parent.gameObject;
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (Item A in itemListControl._dataBaseItem)
            {
                if (A._objectNumber == _objectNum)
                {
                    _go = A.item3d.transform.parent.gameObject;
                    break;
                }
            }
        }
        _posY = _go.transform.localPosition;
        _scaleY = _go.transform.localScale;
        _go.transform.localPosition = _pos;
        _go.transform.localScale = _scale;
        Quaternion _rotY;
        _rotY = _go.transform.localRotation;
        if (_originNum < 2000) _go.transform.rotation = _rot;
        else _go.transform.GetChild(0).transform.rotation = _rot;
        pushObjectHistY(_go, _posY, _scaleY, _objectNum, _originNum, _rotY);
        clickedPlaceControl.ResetForHistory();
        Debug.Log(_go);
        Debug.Log(scaleControl);
        scaleControl.ResetForHistory(_go);
    }

    private void PopAniBarCreate()
    {
        AniBarCreate abc = _aniBarCreateHist.Pop();
        GameObject biggo = abc.GetBigGameObject();
        GameObject smallgo = abc.GetSmallGameObject();
        string animationName = abc.GetAnimationName();
        string anibarName = abc.GetAnibarName();
        int objectNum = abc.GetObjectNum();
        int voice = abc.GetVoice();
        float _barX = 0;
        float _barWidth = 0;
        string _parentName = "";
        string _animationText = "";
        int _moveOrState = 0;
        int _actionOrFace = 0;
        int _layerNumber = 0;
        float _arriveX = 0;
        float _arriveY = 0;
        float _arriveZ = 0;
        int _originNum = 0;
        int _rotation = 0;
        string _voiceName = "";
        int _dir_key = 0;


        if (voice == 0)
        {
            foreach (BigAniBar A in itemListControl._dataBaseBigVoice)
            {
                if (A._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber == objectNum && A._smallAniBar.GetComponent<SmallAniBar>()._anibarName == anibarName)
                {
                    biggo = A._thisAniBar;
                    smallgo = A._smallAniBar;
                    itemListControl._dataBaseBigVoice.Remove(A);
                    break;
                }
            }
            Debug.Log(itemListControl._dataBaseSmallVoice.Count);
            foreach (SmallAniBar A in itemListControl._dataBaseSmallVoice)
            {
                Debug.Log(A._item._objectNumber);
                Debug.Log(objectNum);
                Debug.Log(A._anibarName);
                Debug.Log(anibarName);
                if (A._item._objectNumber == objectNum && A._anibarName == anibarName)
                {
                    Debug.Log("in");
                    _voiceName = A._animationName;
                    _dir_key = A._dir_key;
                    _barX = A.transform.localPosition.x;
                    _barWidth = A.GetComponent<RectTransform>().rect.width;
                    if (A._item._originNumber == 2000) _parentName = A.transform.parent.transform.parent.name.Substring(0, 8);
                    if (A._item._originNumber == 2001) _parentName = A.transform.parent.transform.parent.name.Substring(0, 3);
                    if (A._item._originNumber == 2002) _parentName = A.transform.parent.transform.parent.name.Substring(0, 5);
                    if (A._item._originNumber == 2003) _parentName = A.transform.parent.transform.parent.name.Substring(0, 7);
                    itemListControl._dataBaseSmallVoice.Remove(A);
                    itemListControl._voiceDBIndex--;
                    pushAniBarVoiceDeleteHistY(_barX, _barWidth, _voiceName, _parentName, _dir_key, A._item._originNumber);
                    break;
                }
            }
        }
        else
        {
            foreach (BigAniBar A in itemListControl._dataBaseBigAnimation)
            {
                if (A._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber == objectNum && A._smallAniBar.GetComponent<SmallAniBar>()._anibarName == anibarName)
                {
                    biggo = A._thisAniBar;
                    smallgo = A._smallAniBar;
                    itemListControl._dataBaseBigAnimation.Remove(A);
                    itemListControl._actionDBIndex--;
                    break;
                }
            }
            foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
            {
                if (A._item._objectNumber == objectNum && A._anibarName == anibarName)
                {
                    itemListControl._dataBaseSmallAnimation.Remove(A);
                    _barX = A.transform.localPosition.x;
                    _barWidth = A.GetComponent<RectTransform>().rect.width;
                    if (A._item._originNumber == 2000) _parentName = A.transform.parent.transform.parent.name.Substring(0, 8);
                    if (A._item._originNumber == 2001) _parentName = A.transform.parent.transform.parent.name.Substring(0, 3);
                    if (A._item._originNumber == 2002) _parentName = A.transform.parent.transform.parent.name.Substring(0, 5);
                    if (A._item._originNumber == 2003) _parentName = A.transform.parent.transform.parent.name.Substring(0, 7);
                    _animationText = A._bigAniBar.transform.GetChild(0).GetComponent<Text>().text;
                    _moveOrState = A._moveCheck == true ? 1 : 0;
                    _actionOrFace = A._actionOrFace == true ? 1 : 0;
                    _layerNumber = A._layerNumber;
                    _arriveX = A._arriveLocation.x;
                    _arriveY = A._arriveLocation.y;
                    _arriveZ = A._arriveLocation.z;
                    _originNum = A._item._originNumber;
                    _rotation = A._rotation == true ? 1 : 0;
                    pushAniBarDeleteHistY(_barX, _barWidth, animationName, _parentName, _animationText, _moveOrState, _actionOrFace, _layerNumber, _arriveX, _arriveY, _arriveZ, _originNum, _rotation);
                    break;
                }
            }
        }
        Destroy(biggo);
        Destroy(smallgo);
    }

    private void PopObjectCreate()
    {
        ObjectCreate oc = _objectCreateHist.Pop();
        int originNum = oc.GetOriginNum();
        int objectNum = oc.GetObjectNum();
        GameObject go = oc.GetGameObject();
        GameObject canvas = GameObject.Find("Canvas");
        GameObject big;
        GameObject small;
        /////////////////
        string _itemName = "";
        Vector3 _pos = new Vector3(0, 0, 0);
        Vector3 _rot = new Vector3(0, 0, 0);
        Vector3 _scale = new Vector3(0, 0, 0);
        List<AniBarDelete> abd = new List<AniBarDelete>();
        List<AniBarVoiceDelete> abvd = new List<AniBarVoiceDelete>();
        HumanItem _his = new HumanItem("Idle", objectNum);
        /////////////////

        if (originNum >= 2000 && originNum < 4000)
        {
            foreach (Item A in itemListControl._dataBaseHuman)
            {
                if (A._objectNumber == objectNum)
                {
                    _itemName = A.itemName;
                    _pos = A.item3d.transform.parent.position;
                    _rot = A.item3d.transform.eulerAngles;
                    _scale = A.item3d.transform.parent.localScale;
                    go = A.item3d.transform.parent.gameObject;
                    itemListControl._dataBaseHuman.Remove(A);
                    _his = A.item3d.GetComponent<ItemObject>()._thisHuman;
                    Debug.Log("ObjectCreate " + _rot);
                    break;
                }
            }

            big = canvas.transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
            small = canvas.transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;

            for (int i = 0; i < big.transform.childCount; i++)
            {
                if (big.transform.GetChild(i).GetComponent<BigSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
                    {
                        if (A._item._objectNumber == objectNum)
                        {
                            itemListControl._dataBaseSmallAnimation.Remove(A);
                            itemListControl._actionDBIndex--;
                        }
                    }
                    foreach (BigAniBar A in itemListControl._dataBaseBigAnimation)
                    {
                        if (A._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber == objectNum)
                        {
                            itemListControl._dataBaseBigAnimation.Remove(A);
                        }
                    }
                    pushHumanDeleteHistY(originNum, objectNum, _itemName, _pos, _rot, _scale, abd, abvd, _his);
                    Destroy(big.transform.GetChild(i).gameObject);
                    Destroy(small.transform.GetChild(i).gameObject);
                    break;
                }
            }

            foreach (GameObject A in itemListControl._dataBaseBigBar)
            {
                if (A.GetComponent<BigSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    itemListControl._dataBaseBigBar.Remove(A);
                    break;
                }
            }
            foreach (GameObject A in itemListControl._dataBaseSmallbar)
            {
                if (A.GetComponent<SmallSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    itemListControl._dataBaseSmallbar.Remove(A);
                    break;
                }
            }

            Destroy(go.gameObject);
        }
        else
        {
            if (originNum >= 4000)
            {
                foreach (Item A in itemListControl._dataBaseWall)
                {
                    if (A._objectNumber == objectNum && A._originNumber == originNum)
                    {
                        _itemName = A.itemName;
                        _pos = A.item3d.transform.parent.position;
                        _rot = A.item3d.transform.parent.eulerAngles;
                        _scale = A.item3d.transform.parent.localScale;
                        go = A.item3d.transform.parent.gameObject;
                        itemListControl._dataBaseWall.Remove(A);
                        pushObjectDeleteHistY(objectNum, originNum, _itemName, _pos, _rot, _scale);
                        itemListControl._wallDBIndex--;
                        break;
                    }
                }
            }
            else
            {
                foreach (Item A in itemListControl._dataBaseItem)
                {
                    if (A._objectNumber == objectNum)
                    {
                        _itemName = A.itemName;
                        _pos = A.item3d.transform.parent.position;
                        _rot = A.item3d.transform.parent.eulerAngles;
                        _scale = A.item3d.transform.parent.localScale;
                        go = A.item3d.transform.parent.gameObject;
                        itemListControl._dataBaseItem.Remove(A);
                        pushObjectDeleteHistY(objectNum, originNum, _itemName, _pos, _rot, _scale);
                        itemListControl._itemDBIndex--;
                        break;
                    }
                }
            }

            Destroy(go);
        }
        clickedItemControl.BasePanelReset();
        clickedItemControl.ResetClickMenu();
        clickedItemControl.ResetAnimationPanel();
    }//끝

    private void PopObjectDelete()
    {
        ObjectDelete od = _objectDeleteHist.Pop();

        int objectNum = od.GetObjectNum();
        int originNum = od.GetOriginNum();
        string itemName = od.GetItemName();
        Vector3 pos = od.GetPos();
        Vector3 rot = od.GetRot();
        Vector3 scale = od.GetScale();

        ///////////////////////////
        Debug.Log(originNum);
        int chk = originNum / 1000;
        Debug.Log(chk);
        Debug.Log(itemListControl.GetItem(chk, originNum));
        GameObject _loadObject = Instantiate(itemListControl.GetItem(chk, originNum).item3d) as GameObject;
        _loadObject.transform.SetParent(GameObject.Find("InDoor").transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(itemName, objectNum, originNum, _loadObject);

        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        if (originNum >= 4000) itemListControl.AddWall(_tmpItem);
        else itemListControl.AddDB(_tmpItem);

        _loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        _loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = pos;

        /* 회전값 설정 */
        Debug.Log(rot);
        if (originNum < 2000) _loadObject.transform.parent.Rotate(rot);
        else _loadObject.transform.Rotate(rot);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = scale;

        pushObjectCreateHistY(_loadObject, originNum, objectNum);

        /////////////////////////////
    }//끝

    private void PopHumanDelete()
    {
        HumanDelete hd = _humanDeleteHist.Pop();
        int originNum = hd.GetOriginNum();
        int objectNum = hd.GetObjectNum();
        string humanName = hd.GetHumanName();
        Vector3 pos = hd.GetPos();
        Vector3 rot = hd.GetRot();
        Vector3 scale = hd.GetScale();
        List<AniBarDelete> abd = hd.GetAniBarDelete();
        List<AniBarVoiceDelete> abvd = hd.GetAniBarVoiceDelete();
        HumanItem _tmpHuman = hd.GetHumanItem();

        /////////////////////////////////////////////////////////////////////

        GameObject _loadObject = Instantiate(itemListControl.GetItem(2, originNum).item3d) as GameObject;
        _loadObject.transform.SetParent(GameObject.Find("InDoor").transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(humanName, objectNum, originNum, _loadObject);
        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        _tmp._thisItem.itemName = humanName;
        _tmp._humanInitPosition = pos;

        //HumanItem _tmpHuman = new HumanItem("Idle", objectNum);
        _tmp._thisHuman = _tmpHuman;

        itemListControl.AddHuman(_tmpItem);

        //////////////////////////////////////////////////////
        ////////////// Dress부분 처리 ////////////////////////
        //////////////////////////////////////////////////////

        _tmpHuman = _tmp._thisHuman;

        //shirt의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        Transform _quick = _tmpItem.item3d.transform.Find("shirt");
        for (int i = 0; i < _quick.childCount; i++)
        {
            if (_quick.GetChild(i).name.CompareTo(_tmpHuman._shirtName) == 0)
            {
                _tmpHuman._shirt = _quick.GetChild(i).gameObject;
                _quick.GetChild(i).gameObject.SetActive(true);
            }
            else _quick.GetChild(i).gameObject.SetActive(false);
        }
        //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        _quick = _tmpItem.item3d.transform.Find("pant");
        for (int i = 0; i < _quick.childCount; i++)
        {
            if (_quick.GetChild(i).name.CompareTo(_tmpHuman._pantName) == 0)
            {
                _tmpHuman._pant = _quick.GetChild(i).gameObject;
                _quick.GetChild(i).gameObject.SetActive(true);
            }
            else _quick.GetChild(i).gameObject.SetActive(false);
        }
        //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        if (_tmpHuman._shoesName != null)
        {
            string[] _name = _tmpHuman._shoesName.Split('_');
            if (_name[1].Equals("normal")) //현재 발 상태가 normal
            {
                _quick = _tmpItem.item3d.transform.Find("shoes");
                for (int i = 0; i < _quick.GetChild(0).childCount; i++)
                {
                    //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                    if (_quick.GetChild(0).GetChild(i).name.CompareTo(_tmpHuman._shoesName) == 0)
                    {
                        _tmpHuman._shoes = _quick.GetChild(0).GetChild(i).gameObject;
                        _quick.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    }
                    else _quick.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }
            else if (_name[1].Equals("abnormal"))//abnormal
            {
                _quick = _tmpItem.item3d.transform.Find("shoes");
                _quick.GetChild(1).gameObject.SetActive(true);
                _quick.GetChild(0).gameObject.SetActive(false);
                for (int i = 0; i < _quick.GetChild(1).childCount; i++)
                {
                    //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                    if (_quick.GetChild(1).GetChild(i).name.CompareTo(_tmpHuman._shoesName) == 0)
                    {
                        _tmpHuman._shoes = _quick.GetChild(1).GetChild(i).gameObject;
                        _quick.GetChild(1).GetChild(i).gameObject.SetActive(true);
                    }
                    else _quick.GetChild(1).GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        GameObject _cloth;
        Transform _colourization = null;
        SkinnedMeshRenderer _colourizationSkin = null;
        Material _copyMaterial; //메터리얼 복사

        GameObject _clothes;
        if (_tmpHuman._shirt != null)
        {
            _cloth = _tmpHuman._shirt;
            _colourization = null;
            _colourizationSkin = null;

            _clothes = Instantiate(_cloth);

            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._shirt_R / 255f, _tmpHuman._shirt_G / 255f, _tmpHuman._shirt_B / 255f, 40f / 255f);
        }
        if (_tmpHuman._pant != null)
        {
            _cloth = _tmpHuman._pant;
            _clothes = Instantiate(_cloth);
            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._pant_R / 255f, _tmpHuman._pant_G / 255f, _tmpHuman._pant_B / 255f, 40f / 255f);
        }
        if (_tmpHuman._shoes != null)
        {
            _cloth = _tmpHuman._shoes;
            _clothes = Instantiate(_cloth);
            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._shoes_R / 255f, _tmpHuman._shoes_G / 255f, _tmpHuman._shoes_B / 255f, 40f / 255f);
        }
        //////////////////////////////////////////////////////
        ////////////// Dress부분 처리 ////////////////////////
        //////////////////////////////////////////////////////

        //_loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        //_loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = pos;



        Debug.Log(rot);
        /* 회전값 설정 */
        _loadObject.transform.Rotate(rot);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = scale;

        /* 스케줄러 생성 파트*/
        //스몰바 생성 및 설정
        GameObject _smallBar = Instantiate(Resources.Load("Prefab/SmallScheduler_Sample")) as GameObject;
        if (originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _smallBar.name = "Man" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        else if (originNum == 2000)
        {
            _smallBar.name = "Daughter" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        else if (originNum == 2002)
        {
            _smallBar.name = "Woman" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        //content 설정
        GameObject _smallContent;
        _smallContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;

        _smallBar.transform.SetParent(_smallContent.transform); //small Bar의 부모설정
        _smallBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //small Bar의 정보
        _smallBar.GetComponent<SmallSchedulerBar>()._objectNumber = objectNum - 1; //오브젝트넘버를 small bar에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._originNumber = originNum; //오리진 정보또한 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._humanNumber = 0;
        _smallBar.GetComponent<SmallSchedulerBar>()._object = _tmpItem;

        //빅바 생성 및 설정
        GameObject _bigBar = Instantiate(Resources.Load("Prefab/BigScheduler_Sample")) as GameObject;
        if (originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _bigBar.name = "Man" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        else if (originNum == 2000)
        {
            _bigBar.name = "Daughter" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        else
        {
            _bigBar.name = "Woman" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        //content 설정
        GameObject _bigContent;
        _bigContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        _bigBar.transform.SetParent(_bigContent.transform); //big Bar의 부모설정
        _bigBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //big Bar의 정보
        _bigBar.GetComponent<BigSchedulerBar>()._objectNumber = objectNum - 1; //오브젝트넘버를 small bar에 저장
        _bigBar.GetComponent<BigSchedulerBar>()._originNumber = originNum; //오리진 정보또한 저장
        _bigBar.GetComponent<BigSchedulerBar>()._humanNumber = 0; //휴먼넘버를 저장
        //생성된 빅바를 스몰바에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._bigScheduler = _bigBar;
        //big Bar는 우선 안보이게 처리
        _bigBar.SetActive(false);
        itemListControl._dataBaseBigBar.Add(_bigBar);
        itemListControl._dataBaseSmallbar.Add(_smallBar);

        ////////////////////////////////////////////////////////////////////

        foreach (AniBarDelete A in abd)
        {
            float _barX = A.GetBarX();
            float _barWidth = A.GetBarWidth();
            string _animationName = A.GetAnimationName();
            string _parentName = A.GetParentName();
            string _animationText = A.GetAnimationText();
            int _moveOrState = A.GetMoveOrState();
            int _actionOrFace = A.GetActionOrFace();
            int _layerNumber = A.GetLayerNumber();
            float _arriveX = A.GetArriveX();
            float _arriveY = A.GetArriveY();
            float _arriveZ = A.GetArriveZ();
            int originNumber = A.GetOriginNumber();
            int _rotation = A.GetRotation();
            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _animationName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _animationText;

            Vector3 _arriveLocation = new Vector3(_arriveX, _arriveY, _arriveZ);

            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
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

            Item _item = null;
            Animator _animator = null;
            foreach (Item B in itemListControl._dataBaseHuman)
            {
                if (B._objectNumber == objectNum)
                {
                    _item = B;
                    _animator = B.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _animationName;
            _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _animationText;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _animationText;
            _smallAniBar.name = _animationText;
            _bigAniBar.name = _animationText;
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
            if (originNum == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }

            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
            _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;

            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _bigAniBar.transform.GetChild(1).transform.localPosition = new Vector3(-_barWidth / 2, 0, 0);
            _bigAniBar.transform.GetChild(2).transform.localPosition = new Vector3(_barWidth / 2, 0, 0);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);


            if (_layerNumber < 2) // 1임
            {
                _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            }
            else // 3, 5, 4 ,2
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }
            //if (_layerNumber == 0)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            //}
            //else if (_layerNumber == 3)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

            //}
            //else if (_layerNumber == 2)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            //}
            //else
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
            //}

            itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        }
        foreach (AniBarVoiceDelete AA in abvd)
        {
            float _barX = AA.GetBarX();
            float _barWidth = AA.GetBarWidth();
            string _voiceName = AA.GetVoiceName();
            string _parentName = AA.GetParentName();
            int _dir_key = AA.GetDirkey();
            int originNumber = AA.GetOriginNumber();

            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _voiceName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _voiceName;

            AudioClip _audioClip = voiceFileController.audioInfo[_dir_key][_voiceName];

            /*스몰애니바를 생성*/
            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._dir_key = _dir_key;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._voice = true;

            /*찾아야하는 정보*/
            Item _item = null;
            Animator _animator = null;
            foreach (Item B in itemListControl._dataBaseHuman)
            {
                if (B.itemName == _parentName)
                {
                    objectNum = B._objectNumber;
                    _item = B;
                    _animator = B.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _voiceName;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _voiceName;

            int _parentNumber = 5;

            /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
            if (originNum == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }

            /*빅애니바, 스몰애니바의 부모설정*/
            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
            _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;


            _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

            float _aniBarWidth = _bigAniBar.transform.GetComponent<RectTransform>().rect.width;
            float _width = _aniBarWidth * _audioClip.length / 11.73f;
            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

            _smallAniBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;

            itemListControl.AddVoiceDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());

        }


        //shirt의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _tmpItem.item3d.transform.Find("shirt").childCount; i++)
        {
            if (_tmpItem.item3d.transform.Find("shirt").GetChild(i).gameObject.activeSelf)
            {
                _tmpHuman._shirt = _tmpItem.item3d.transform.Find("shirt").GetChild(i).gameObject;
                break;
            }
        }
        //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _tmpItem.item3d.transform.Find("pant").childCount; i++)
        {
            if (_tmpItem.item3d.transform.Find("pant").GetChild(i).gameObject.activeSelf)
            {
                _tmpHuman._pant = _tmpItem.item3d.transform.Find("pant").GetChild(i).gameObject;
                break;
            }
        }
        //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        if (_tmpItem.item3d.transform.Find("shoes").GetChild(0).gameObject.activeSelf) //현재 발 상태가 normal
        {
            for (int i = 0; i < _tmpItem.item3d.transform.Find("shoes").GetChild(0).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_tmpItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject.activeSelf)
                {
                    _tmpHuman._shoes = _tmpItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject;
                    break;
                }
            }
        }
        else //abnormal
        {
            for (int i = 0; i < _tmpItem.item3d.transform.Find("shoes").GetChild(1).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_tmpItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject.activeSelf)
                {
                    _tmpHuman._shoes = _tmpItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject;
                    break;
                }
            }
        }

        pushObjectCreateHistY(_loadObject, originNum, objectNum);

    }//끝

    private void PopAniBarDelete()
    {
        AniBarDelete A = _aniBarDeleteHist.Pop();
        float _barX = A.GetBarX();
        float _barWidth = A.GetBarWidth();
        string _animationName = A.GetAnimationName();
        string _parentName = A.GetParentName();
        string _animationText = A.GetAnimationText();
        int _moveOrState = A.GetMoveOrState();
        int _actionOrFace = A.GetActionOrFace();
        int _layerNumber = A.GetLayerNumber();
        float _arriveX = A.GetArriveX();
        float _arriveY = A.GetArriveY();
        float _arriveZ = A.GetArriveZ();
        int originNum = A.GetOriginNumber();
        int _rotation = A.GetRotation();
        GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
        _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _animationName;
        _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _animationText;


        Vector3 _arriveLocation = new Vector3(_arriveX, _arriveY, _arriveZ);

        GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
        GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
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

        int objectNum = 0;
        Item _item = null;
        Animator _animator = null;
        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B.itemName == _parentName)
            {
                objectNum = B._objectNumber;
                _item = B;
                _animator = B.item3d.GetComponent<Animator>();
                break;
            }
        }

        _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item; //사람객체 리스트를 돌면서 확인해야함
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator; //사람객체 리스트를 돌면서 확인해야함
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _animationName;
        _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _animationText;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _animationText;
        _smallAniBar.name = _animationText;
        _bigAniBar.name = _animationText;
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
        if (originNum == 2001)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2000)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2003)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }

        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
        _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;

        _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
        _bigAniBar.transform.GetChild(1).transform.localPosition = new Vector3(-_barWidth / 2, 0, 0);
        _bigAniBar.transform.GetChild(2).transform.localPosition = new Vector3(_barWidth / 2, 0, 0);
        _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

        //if (_layerNumber == 0)
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
        //}
        //else if (_layerNumber == 3)
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        //}
        //else if (_layerNumber == 2)
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        //}
        //else
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
        //}

        if (_layerNumber < 2) // 1임
        {
            _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
            _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
            _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
            _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
        }
        else // 3, 5, 4 ,2
        {
            _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        }

        itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        pushAniBarCreateHistY(_bigAniBar, _smallAniBar, _animationName, _animationText, objectNum, _smallAniBar.GetComponent<SmallAniBar>()._voice == true ? 0 : 1);
    }//끝

    private void PopAniBarVoiceDelete()
    {
        AniBarVoiceDelete A = _aniBarVoiceDeleteHist.Pop();
        float _barX = A.GetBarX();
        float _barWidth = A.GetBarWidth();
        string _voiceName = A.GetVoiceName();
        string _parentName = A.GetParentName();
        int _dir_key= A.GetDirkey();
        int originNum = A.GetOriginNumber();

        GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject;
        _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _voiceName;
        _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _voiceName;
        
        AudioClip _audioClip = voiceFileController.audioInfo[_dir_key][_voiceName];

        /*스몰애니바를 생성*/
        GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
        GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._dir_key = _dir_key;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._voice = true;

        /*찾아야하는 정보*/
        int objectNum = 0;
        Item _item = null;
        Animator _animator = null;
        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B.itemName == _parentName)
            {
                objectNum = B._objectNumber;
                _item = B;
                _animator = B.item3d.GetComponent<Animator>();
                break;
            }
        }

        _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _voiceName;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _voiceName;
        _bigAniBar.name = _voiceName;
        _smallAniBar.name = _voiceName;

        int _parentNumber = 5;

        /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
        if (originNum == 2001)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2000)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2003)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
        _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;


        _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        float _aniBarWidth = _bigAniBar.transform.GetComponent<RectTransform>().rect.width;
        float _width = _aniBarWidth * _audioClip.length / 11.73f;
        _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
        _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

        _smallAniBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;

        itemListControl.AddVoiceDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        pushAniBarCreateHistY(_bigAniBar, _smallAniBar, _voiceName, _voiceName, objectNum, _smallAniBar.GetComponent<SmallAniBar>()._voice == true ? 0 : 1);

    }//끝

    private void PopDressChange()
    {
        DressChange A = _dressChangeHist.Pop();
        string[] _name = A.GetName();
        string prev = A.GetPrev();
        string objectName = A.GetObjectName();
        int objectNum = A.GetObjectNum();
        int originNum = A.GetOriginNum();
        Vector3 _pos = A.GetPos();
        HumanItem _clickedHumanItem;
        Item _clickedItem;
        Vector3 _originPos;

        GameObject _newCloth = null;
        string _prev = "null";
        int _changeNum = 0;

        Debug.Log(_name[1]);
        Debug.Log(prev);

        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B._objectNumber == objectNum && B._originNumber == originNum)
            {
                _newCloth = B.item3d;
            }
        }
        _clickedHumanItem = _newCloth.GetComponent<ItemObject>()._thisHuman;
        _clickedItem = _newCloth.GetComponent<ItemObject>()._thisItem;
        _originPos = _clickedItem.item3d.transform.localPosition;
        if (_name[0] == "shirt")
        {
            _changeNum = 1;
            if (_clickedHumanItem._shirt != null)
            {
                _prev = _clickedHumanItem._shirt.name;
                _clickedHumanItem._shirt.SetActive(false);
            }
            if (prev.CompareTo("null") == 0) _clickedHumanItem._shirt = null;
            else if (prev.CompareTo("null") != 0)
            {
                _newCloth = _newCloth.transform.Find(_name[0] + "/" + prev).gameObject;
                _clickedHumanItem._shirt = _newCloth;
                _clickedHumanItem._shirtName = _newCloth.name;
            }
            pushDressChangeHistY(_name, _prev, objectName, objectNum, originNum, _pos);
        }
        else if (_name[0] == "pant")
        {
            _changeNum = 2;
            if (_clickedHumanItem._pant != null)
            {
                _prev = _clickedHumanItem._pant.name;
                _clickedHumanItem._pant.SetActive(false);
            }
            if (prev.CompareTo("null") == 0) _clickedHumanItem._pant = null;
            else if (prev.CompareTo("null") != 0)
            {
                _newCloth = _newCloth.transform.Find(_name[0] + "/" + prev).gameObject;
                _clickedHumanItem._pant = _newCloth;
                _clickedHumanItem._pantName = _newCloth.name;
            }
            pushDressChangeHistY(_name, _prev, objectName, objectNum, originNum, _pos);
        }
        else
        {
            _changeNum = 3;
            if (_clickedHumanItem._shoes != null)
            {
                _prev = _clickedHumanItem._shoes.name;
                _clickedHumanItem._shoes.SetActive(false);
            }
            else _prev = "null";
            GameObject _new = null;
            if (prev.CompareTo("null") == 0)
            {
                _clickedHumanItem._shoes = null;
                _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (prev.CompareTo("null") != 0)
            {
                String[] _change = prev.Split('_');
                if (_change[1] == "normal")
                {
                    _new = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).transform.Find(prev).gameObject;
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    _new = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).transform.Find(prev).gameObject;
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(false);
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(true);
                    _pos += new Vector3(0, 0.05f, 0);
                }
                _clickedHumanItem._shoes = _new;
                _clickedHumanItem._shoesName = _new.name;
            }
            pushDressChangeHistY(_name, _prev, objectName, objectNum, originNum, _originPos);
            _clickedItem.item3d.transform.localPosition = _pos;
            if (_new != null) _new.SetActive(true);
        }
        if (prev.CompareTo("null") != 0) _newCloth.SetActive(true);
        dressController.ForHistoryCheckUI(_clickedHumanItem, _changeNum);
    }

    private void PopTiling()
    {
        TilingChange _ti = _tilingHist.Pop();
        int _originNum = _ti.GetOriginNum();
        int _objectNum = _ti.GetObjectNum();
        int _tileOriginNum = _ti.GetTileOriginNum();
        Vector3 _scale = _ti.GetScale();
        Vector3 _scaleY;
        GameObject _go = null;

        Debug.Log(_objectNum);
        Debug.Log(_originNum);

        foreach (Item B in itemListControl._dataBaseWall)
        {
            if (B._originNumber == _originNum && B._objectNumber == _objectNum)
            {
                _go = B.item3d;
                break;
            }
        }
        MeshRenderer _mesh = _go.GetComponent<MeshRenderer>();

        if (_tileOriginNum == -1)
        {
            _scaleY = _mesh.material.mainTextureScale;
            _mesh.material.mainTextureScale = _scale;
            pushTilingHistY(_originNum, _objectNum, _scaleY, -1);
        }
        else
        {
            Sprite tmp = Instantiate(itemListControl.GetImages(3, _tileOriginNum));
            if (tmp != null) _mesh.material.mainTexture = tmp.texture;
            else _mesh.material.mainTexture = null;
            _go.GetComponent<ItemObject>()._placeNumber = _tileOriginNum;
            pushTilingHistY(_originNum, _objectNum, _scale, -1);
        }
        clickedPlaceControl.ResetForHistory();
    }

    private void PopTile()
    {
        TileChange A = _tileHist.Pop();
        int _originNumber = A.GetOriginNum();
        int _objectNumber = A.GetObjectNum();
        int _buildingNumber = A.GetBuildingOriginNum();
        GameObject _target = null;

        foreach (Item B in itemListControl._dataBaseWall)
        {
            if (B._originNumber == _originNumber && B._objectNumber == _objectNumber)
            {
                _target = B.item3d;
            }
        }
        Debug.Log(_target);

        /* 클릭된 Place 객체의 MeshRenderer 컴포넌트를 담음 */
        MeshRenderer _clickedMeshRenderer = _target.GetComponent<MeshRenderer>();

        /* Slot의 OriginNumber에 해당하는 Sprite 정보를 가져옴 */
        Sprite _tmp = itemListControl.GetImages(3, _buildingNumber);

        /* 존재하지 않는 Sprite이면 */
        if (_tmp == null)
        {
            /* 건물의 OriginNumber를 저장 */
            int _buildingOriginNumber = _target.GetComponent<ItemObject>()._thisItem._originNumber;

            /* 건물들의 3DObject가 들어있는 디렉터리 경로 지정 */
            System.IO.DirectoryInfo _dir = new System.IO.DirectoryInfo("Assets/Resources/Item/Wall/Build");

            /* 디렉터리 탐색 */
            foreach (System.IO.FileInfo _file in _dir.GetFiles())
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
        Debug.Log("YCheck");
        pushTileHistY(_originNumber, _objectNumber, _target.GetComponent<ItemObject>()._placeNumber);
        _target.GetComponent<ItemObject>()._placeNumber = _buildingNumber;
    }

    private void PopHand()
    {
        HandChange hg = _handHist.Pop();
        int _originNum = hg.GetOriginNum();
        int _objectNum = hg.GetObjectNum();
        bool _isLeft = hg.GetIsLeft();
        string _objectName = hg.GetObjectName();
        Item _thisItem = hg.GetItem();
        Item _thisHuman = null;
        HumanItem _thisHumanItem = null;
        GameObject _handObjectClone = null;
        Transform _hand;
        string _itemName = "null";

        Debug.Log("HandY " + _objectName);
        if (_thisItem != null) Debug.Log("HandY " + _thisItem.itemName);

        foreach (Item A in itemListControl._dataBaseHuman)
        {
            if (A._originNumber == _originNum && A._objectNumber == _objectNum)
            {
                _thisHuman = A;
                _thisHumanItem = A.item3d.GetComponent<ItemObject>()._thisHuman;
                break;
            }
        }

        Vector3 _pos = new Vector3(1, 0, 0);


        if (_isLeft)
        {
            _hand = _thisHuman.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
            if (_thisHumanItem._leftHandItem != null)
            {
                _itemName = _thisHumanItem._leftHandItem.itemName;
                HistoryController.pushHandHistY(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._leftHandItem);
                if (!_objectName.Equals("null") && !_thisHumanItem._leftHandItem.itemName.Equals(_thisItem.itemName))
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
                HistoryController.pushHandHistY(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._leftHandItem);
                _thisHumanItem._leftHandItem = _thisItem;
            }
        }
        else
        {
            _hand = _thisHuman.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            if (_thisHumanItem._rightHandItem != null)
            {
                _itemName = _thisHumanItem._rightHandItem.itemName;
                HistoryController.pushHandHistY(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._rightHandItem);
                if (!_objectName.Equals("null") && !_thisHumanItem._rightHandItem.itemName.Equals(_thisItem.itemName))
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
                HistoryController.pushHandHistY(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._rightHandItem);
                _thisHumanItem._rightHandItem = _thisItem;
            }
        }

        Vector3 _localPos;

        _handObjectClone = Instantiate(_thisItem.item3d); //객체 생성
        _handObjectClone.name = _thisItem.itemName; //이름 지정
        _handObjectClone.transform.SetParent(_hand); //손을 부모로 설정
        _handObjectClone.transform.localRotation = Quaternion.Euler(Vector3.zero); //로컬 회전값을 손 회전값과 동일하게 변경

        //각 객체에 따라 객체의 위치 조정
        if (_thisHuman._originNumber == 2001) //Man
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._manRightPos;
        }
        else if (_thisHuman._originNumber == 2002) //Woman
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._womanRightPos;
        }
        else if (_thisHuman._originNumber == 2000) //Daughter
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._daughterRightPos;
        }
        else if (_thisHuman._originNumber == 2003) //Woongin
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

    }

    private void PopDressRGBChange()
    {
        DressRGBChange A = _dressRGBChangeHist.Pop();
        int objectNum = A.GetObjectNum();
        int originNum = A.GetOriginNum();
        int what = A.GetWhatDress();
        Color color = A.GetChangeColor();
        HumanItem _clickedHumanItem;

        GameObject _changeCloth = null;

        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B._objectNumber == objectNum && B._originNumber == originNum)
            {
                _changeCloth = B.item3d;
            }
        }
        _clickedHumanItem = _changeCloth.GetComponent<ItemObject>()._thisHuman;

        GameObject _cloth = null;
        Transform _colourization = null;
        SkinnedMeshRenderer _colourizationSkin = null;

        if (what == 0 && _clickedHumanItem._shirt != null)
        {
            _cloth = _clickedHumanItem._shirt;
            _colourization = _clickedHumanItem._shirt.transform.Find("Colourization");
        }
        else if (what == 1 && _clickedHumanItem._pant != null)
        {
            _cloth = _clickedHumanItem._pant;
            _colourization = _clickedHumanItem._pant.transform.Find("Colourization");
        }
        else if (what == 2 && _clickedHumanItem._shoes != null)
        {
            _cloth = _clickedHumanItem._shoes;
            _colourization = _clickedHumanItem._shoes.transform.Find("Colourization");
        }


        if (_colourization == null) //색상화 객체가 없을 경우
        {
            GameObject _clothes = Instantiate(_cloth);

            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            Material _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(0f, 0f, 0f, 0f);
        }

        if (what == 0)//상의
        {
            _colourizationSkin = _clickedHumanItem._shirt.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHistY(originNum, objectNum, 0, new Color(_clickedHumanItem._shirt_R / 255f, _clickedHumanItem._shirt_G / 255f, _clickedHumanItem._shirt_B / 255f, 40f / 255f));
            _clickedHumanItem._shirt_R = color.r * 255f;
            _clickedHumanItem._shirt_G = color.g * 255f;
            _clickedHumanItem._shirt_B = color.b * 255f;
        }
        else if (what == 1)//하의
        {
            _colourizationSkin = _clickedHumanItem._pant.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHistY(originNum, objectNum, 1, new Color(_clickedHumanItem._pant_R / 255f, _clickedHumanItem._pant_G / 255f, _clickedHumanItem._pant_B / 255f, 40f / 255f));
            _clickedHumanItem._pant_R = color.r * 255f;
            _clickedHumanItem._pant_G = color.g * 255f;
            _clickedHumanItem._pant_B = color.b * 255f;
        }
        else if (what == 2)//신발
        {
            _colourizationSkin = _clickedHumanItem._shoes.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHistY(originNum, objectNum, 2, new Color(_clickedHumanItem._shoes_R / 255f, _clickedHumanItem._shoes_G / 255f, _clickedHumanItem._shoes_B / 255f, 40f / 255f));
            _clickedHumanItem._shoes_R = color.r * 255f;
            _clickedHumanItem._shoes_G = color.g * 255f;
            _clickedHumanItem._shoes_B = color.b * 255f;
        }
        Debug.Log(_colourizationSkin.material.color);
        _colourizationSkin.material.color = color;
        Debug.Log(_colourizationSkin.material.color);
    }
    #endregion
    /*
     * Y
     */
    #region
    private void PopAniBarPAWY()
    {
        AniBarPosAndWidth apaw = _aniBarHistY.Pop();
        GameObject _go = apaw.getHistoryTarget();
        Vector3 _pos = apaw.getHistoryPosition();
        Vector3 _posY = apaw.getHistoryPosition();
        float _width = apaw.getHistoryWidth();
        float _widthY = apaw.getHistoryWidth();
        int _objectNum = apaw.getObjectNum();
        string _animationName = apaw.getAnimationName();
        bool _voice = apaw.getVoice();

        Debug.Log("_width : " + _width);

        if (_voice)
        {
            foreach (SmallAniBar A in itemListControl._dataBaseSmallVoice)
            {
                if (A._item._objectNumber == _objectNum && A._animationName == _animationName)
                {
                    _go = A._bigAniBar;
                    _posY = _go.transform.localPosition;
                    _go.transform.localPosition = _pos;
                    A._thisAniBar.transform.localPosition = _pos;
                    _widthY = _go.GetComponent<RectTransform>().rect.width;
                    _go.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _go.GetComponent<RectTransform>().rect.height);
                    break;
                }
            }
        }
        else
        {
            foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
            {
                if (A._item._objectNumber == _objectNum && A._animationName == _animationName)
                {
                    _go = A._bigAniBar;
                    _posY = _go.transform.localPosition;
                    _go.transform.localPosition = _pos;
                    A._thisAniBar.transform.localPosition = _pos;
                    _widthY = _go.GetComponent<RectTransform>().rect.width;
                    _go.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _go.GetComponent<RectTransform>().rect.height);
                    break;
                }
            }
        }
        pushAniBarHist(_go, _posY, _widthY, _objectNum, _animationName, _voice);
    }

    private void PopObjectPASY()
    {
        ObjectPosAndScale apaw = _objectHistY.Pop();
        GameObject _go = apaw.getHistoryTarget();
        Vector3 _pos = apaw.getHistoryPosition();
        Vector3 _posY = apaw.getHistoryPosition();
        Vector3 _scale = apaw.getHistoryScale();
        Vector3 _scaleY = apaw.getHistoryScale();
        int _objectNum = apaw.getObjectNum();
        int _originNum = apaw.getOriginNum();
        Quaternion _rot = apaw.getRot();

        if (_originNum >= 2000)
        {
            if (_originNum >= 4000)
            {
                foreach (Item A in itemListControl._dataBaseWall)
                {
                    if (A._objectNumber == _objectNum)
                    {
                        _go = A.item3d.transform.parent.gameObject;
                        break;
                    }
                }
            }
            else
            {
                foreach (Item A in itemListControl._dataBaseHuman)
                {
                    if (A._objectNumber == _objectNum)
                    {
                        _go = A.item3d.transform.parent.gameObject;
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (Item A in itemListControl._dataBaseItem)
            {
                if (A._objectNumber == _objectNum)
                {
                    _go = A.item3d.transform.parent.gameObject;
                    break;
                }
            }
        }
        _posY = _go.transform.localPosition;
        _scaleY = _go.transform.localScale;
        _go.transform.localPosition = _pos;
        _go.transform.localScale = _scale;
        Quaternion _rotZ = _go.transform.rotation;
        _go.transform.rotation = _rot;
        pushObjectHist(_go, _posY, _scaleY, _objectNum, _originNum, _rotZ);
        clickedPlaceControl.ResetForHistory();
        scaleControl.ResetForHistory(_go);
    }

    private void PopAniBarCreateY()
    {
        AniBarCreate abc = _aniBarCreateHistY.Pop();
        GameObject biggo = abc.GetBigGameObject();
        GameObject smallgo = abc.GetSmallGameObject();
        string animationName = abc.GetAnimationName();
        string anibarName = abc.GetAnibarName();
        int objectNum = abc.GetObjectNum();
        int voice = abc.GetVoice();
        float _barX = 0;
        float _barWidth = 0;
        string _parentName = "";
        string _animationText = "";
        int _moveOrState = 0;
        int _actionOrFace = 0;
        int _layerNumber = 0;
        float _arriveX = 0;
        float _arriveY = 0;
        float _arriveZ = 0;
        int _originNum = 0;
        int _rotation = 0;
        string _voiceName = "";
        int _dir_key = 0;



        if (voice == 0)
        {
            foreach (BigAniBar A in itemListControl._dataBaseBigVoice)
            {
                Debug.Log("a = " + A);
                if (A._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber == objectNum && A._smallAniBar.GetComponent<SmallAniBar>()._anibarName == anibarName)
                {
                    biggo = A._thisAniBar;
                    smallgo = A._smallAniBar;
                    itemListControl._dataBaseBigVoice.Remove(A);
                    break;
                }
            }
            foreach (SmallAniBar A in itemListControl._dataBaseSmallVoice)
            {
                if (A._item._objectNumber == objectNum && A._anibarName == anibarName)
                {
                    _voiceName = A._animationName;
                    _dir_key = A._dir_key;
                    _barX = A.transform.localPosition.x;
                    _barWidth = A.GetComponent<RectTransform>().rect.width;
                    if (A._item._originNumber == 2000) _parentName = A.transform.parent.transform.parent.name.Substring(0, 8);
                    if (A._item._originNumber == 2001) _parentName = A.transform.parent.transform.parent.name.Substring(0, 3);
                    if (A._item._originNumber == 2002) _parentName = A.transform.parent.transform.parent.name.Substring(0, 5);
                    if (A._item._originNumber == 2003) _parentName = A.transform.parent.transform.parent.name.Substring(0, 7);
                    itemListControl._dataBaseSmallVoice.Remove(A);
                    itemListControl._voiceDBIndex--;
                    pushAniBarVoiceDeleteHist(_barX, _barWidth, _voiceName, _parentName, _dir_key, A._item._originNumber);
                    break;
                }
            }
        }
        else
        {
            foreach (BigAniBar A in itemListControl._dataBaseBigAnimation)
            {
                Debug.Log("a = " + A);
                if (A._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber == objectNum && A._smallAniBar.GetComponent<SmallAniBar>()._anibarName == anibarName)
                {
                    biggo = A._thisAniBar;
                    smallgo = A._smallAniBar;
                    itemListControl._dataBaseBigAnimation.Remove(A);
                    itemListControl._actionDBIndex--;
                    break;
                }
            }
            foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
            {
                if (A._item._objectNumber == objectNum && A._anibarName == anibarName)
                {
                    itemListControl._dataBaseSmallAnimation.Remove(A);
                    _barX = A.transform.localPosition.x;
                    _barWidth = A.GetComponent<RectTransform>().rect.width;
                    if (A._item._originNumber == 2000) _parentName = A.transform.parent.transform.parent.name.Substring(0, 8);
                    if (A._item._originNumber == 2001) _parentName = A.transform.parent.transform.parent.name.Substring(0, 3);
                    if (A._item._originNumber == 2002) _parentName = A.transform.parent.transform.parent.name.Substring(0, 5);
                    if (A._item._originNumber == 2003) _parentName = A.transform.parent.transform.parent.name.Substring(0, 7);
                    _animationText = A._bigAniBar.transform.GetChild(0).GetComponent<Text>().text;
                    _moveOrState = A._moveCheck == true ? 1 : 0;
                    _actionOrFace = A._actionOrFace == true ? 1 : 0;
                    _layerNumber = A._layerNumber;
                    _arriveX = A._arriveLocation.x;
                    _arriveY = A._arriveLocation.y;
                    _arriveZ = A._arriveLocation.z;
                    _originNum = A._item._originNumber;
                    _rotation = A._rotation == true ? 1 : 0;
                    pushAniBarDeleteHist(_barX, _barWidth, animationName, _parentName, _animationText, _moveOrState, _actionOrFace, _layerNumber, _arriveX, _arriveY, _arriveZ, _originNum, _rotation);
                    break;
                }
            }
        }
        Destroy(biggo);
        Destroy(smallgo);
    }

    private void PopObjectCreateY()
    {
        ObjectCreate oc = _objectCreateHistY.Pop();
        int originNum = oc.GetOriginNum();
        int objectNum = oc.GetObjectNum();
        GameObject go = oc.GetGameObject();
        GameObject canvas = GameObject.Find("Canvas");
        GameObject big;
        GameObject small;
        /////////////////
        string _itemName = "";
        Vector3 _pos = new Vector3(0, 0, 0);
        Vector3 _rot = new Vector3(0, 0, 0);
        Vector3 _scale = new Vector3(0, 0, 0);
        List<AniBarDelete> abd = new List<AniBarDelete>();
        List<AniBarVoiceDelete> abvd = new List<AniBarVoiceDelete>();
        HumanItem _history = new HumanItem("Idle", objectNum);
        /////////////////


        if (originNum >= 2000 && originNum < 4000)
        {
            foreach (Item A in itemListControl._dataBaseHuman)
            {
                if (A._objectNumber == objectNum)
                {
                    _itemName = A.itemName;
                    _pos = A.item3d.transform.parent.position;
                    _rot = A.item3d.transform.eulerAngles;
                    _scale = A.item3d.transform.parent.localScale;
                    go = A.item3d.transform.parent.gameObject;
                    itemListControl._dataBaseHuman.Remove(A);
                    _history = A.item3d.GetComponent<ItemObject>()._thisHuman;
                    break;
                }
            }

            big = canvas.transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
            small = canvas.transform.GetChild(2).GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;

            schedulerController.GetComponent<SchedulerController>()._deleteObjectName = _itemName + objectNum;
            schedulerController.GetComponent<SchedulerController>()._deleteObjectNmber = objectNum;


            for (int i = 0; i < big.transform.childCount; i++)
            {
                if (big.transform.GetChild(i).GetComponent<BigSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    foreach (SmallAniBar A in itemListControl._dataBaseSmallAnimation)
                    {
                        if (A._item._objectNumber == objectNum)
                        {
                            float _barX = A.gameObject.transform.localPosition.x;
                            float _barWidth = A._aniBarWidth;
                            string _animationName = A._animationName;
                            string _animationText = A._anibarName;
                            string _parentName = A.transform.parent.transform.parent.name;
                            int _moveOrState;
                            int _actionOrFace;
                            int _rotation;
                            if (A._moveCheck)
                                _moveOrState = 1;
                            else
                                _moveOrState = 0;
                            if (A._actionOrFace)
                                _actionOrFace = 1;
                            else
                                _actionOrFace = 0;
                            if (A._rotation)
                                _rotation = 1;
                            else
                                _rotation = 0;

                            int _layerNumber = A._layerNumber;
                            float _arriveX = A._arriveLocation.x;
                            float _arriveY = A._arriveLocation.y;
                            float _arriveZ = A._arriveLocation.z;

                            int _originNumber = A._item._originNumber;
                            abd.Add(new AniBarDelete(_barX, _barWidth, _animationName, _parentName, _animationText, _moveOrState, _actionOrFace, _layerNumber, _arriveX, _arriveY, _arriveZ, _originNumber, _rotation));
                        }
                    }
                    for (int j = 0; j < itemListControl._dataBaseSmallVoice.Count; j++)
                    {
                        SmallAniBar _tmp = itemListControl._dataBaseSmallVoice[j];
                        if (_tmp._item._objectNumber != objectNum) continue;
                        float _barX = _tmp.gameObject.transform.localPosition.x;
                        float _barWidth = _tmp._aniBarWidth;
                        string _voiceName = _tmp.transform.GetChild(0).GetComponent<Text>().text;
                        int _dir_key = _tmp._dir_key;
                        int _originNumber = _tmp._item._originNumber;
                        string _parentName = "";
                        if (_originNumber == 2000) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 8);
                        if (_originNumber == 2001) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 3);
                        if (_originNumber == 2002) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 5);
                        if (_originNumber == 2003) _parentName = _tmp.transform.parent.transform.parent.name.Substring(0, 7);

                        abvd.Add(new AniBarVoiceDelete(_barX, _barWidth, _voiceName, _parentName, _dir_key, _originNumber));
                    }
                    foreach (GameObject A in itemListControl._dataBaseSmallbar)
                    {
                        if (A.GetComponent<SmallSchedulerBar>()._objectNumber == objectNum - 1)
                        {
                            itemListControl._dataBaseSmallbar.Remove(A);
                            break;
                        }
                    }
                    pushHumanDeleteHist(originNum, objectNum, _itemName, _pos, _rot, _scale, abd, abvd, _history);
                    schedulerController.SchedulerDelete();
                    Destroy(big.transform.GetChild(i).gameObject);
                    Destroy(small.transform.GetChild(i).gameObject);
                    break;
                }
            }

            foreach (GameObject A in itemListControl._dataBaseBigBar)
            {
                if (A.GetComponent<BigSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    itemListControl._dataBaseBigBar.Remove(A);
                    break;
                }
            }
            foreach (GameObject A in itemListControl._dataBaseSmallbar)
            {
                if (A.GetComponent<SmallSchedulerBar>()._objectNumber == objectNum - 1)
                {
                    itemListControl._dataBaseSmallbar.Remove(A);
                    break;
                }
            }
            Destroy(go.gameObject);
        }
        else
        {
            if (originNum >= 4000)
            {
                foreach (Item A in itemListControl._dataBaseWall)
                {
                    if (A._objectNumber == objectNum && A._originNumber == originNum)
                    {
                        _itemName = A.itemName;
                        _pos = A.item3d.transform.parent.position;
                        _rot = A.item3d.transform.parent.eulerAngles;
                        _scale = A.item3d.transform.parent.localScale;
                        go = A.item3d.transform.parent.gameObject;
                        itemListControl._dataBaseWall.Remove(A);
                        pushObjectDeleteHist(objectNum, originNum, _itemName, _pos, _rot, _scale);
                        itemListControl._wallDBIndex--;
                        break;
                    }
                }
            }
            else
            {
                foreach (Item A in itemListControl._dataBaseItem)
                {
                    if (A._objectNumber == objectNum)
                    {
                        _itemName = A.itemName;
                        _pos = A.item3d.transform.parent.position;
                        _rot = A.item3d.transform.parent.eulerAngles;
                        _scale = A.item3d.transform.parent.localScale;
                        go = A.item3d.transform.parent.gameObject;
                        itemListControl._dataBaseItem.Remove(A);
                        pushObjectDeleteHist(objectNum, originNum, _itemName, _pos, _rot, _scale);
                        itemListControl._itemDBIndex--;
                        break;
                    }
                }
            }

            Destroy(go);
        }
    }//끝

    private void PopObjectDeleteY()
    {
        ObjectDelete od = _objectDeleteHistY.Pop();

        int objectNum = od.GetObjectNum();
        int originNum = od.GetOriginNum();
        string itemName = od.GetItemName();
        Vector3 pos = od.GetPos();
        Vector3 rot = od.GetRot();
        Vector3 scale = od.GetScale();

        ///////////////////////////
        GameObject _loadObject = null;
        if (originNum >= 4000) _loadObject = Instantiate(itemListControl.GetItem(4, originNum).item3d) as GameObject;
        else _loadObject = Instantiate(itemListControl.GetItem(1, originNum).item3d) as GameObject;

        _loadObject.transform.SetParent(GameObject.Find("InDoor").transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(itemName, objectNum, originNum, _loadObject);

        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        if (originNum >= 4000) itemListControl.AddWall(_tmpItem);
        else itemListControl.AddDB(_tmpItem);

        _loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        _loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = pos;

        /* 회전값 설정 */
        _loadObject.transform.parent.Rotate(rot);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = scale;

        pushObjectCreateHist(_loadObject, originNum, objectNum);

        /////////////////////////////
    }//끝

    private void PopHumanDeleteY()
    {
        HumanDelete hd = _humanDeleteHistY.Pop();
        int originNum = hd.GetOriginNum();
        int objectNum = hd.GetObjectNum();
        string humanName = hd.GetHumanName();
        Vector3 pos = hd.GetPos();
        Vector3 rot = hd.GetRot();
        Vector3 scale = hd.GetScale();
        List<AniBarDelete> abd = hd.GetAniBarDelete();
        List<AniBarVoiceDelete> abvd = hd.GetAniBarVoiceDelete();
        HumanItem _tmpHuman = hd.GetHumanItem();

        /////////////////////////////////////////////////////////////////////

        GameObject _loadObject = Instantiate(itemListControl.GetItem(2, originNum).item3d) as GameObject;
        _loadObject.transform.SetParent(GameObject.Find("InDoor").transform);
        _loadObject = _loadObject.transform.GetChild(0).gameObject;

        _loadObject.layer = LayerMask.NameToLayer("Default");
        _loadObject.AddComponent<ItemObject>();

        /* 빠른 연결을 위한 캐싱 작업 */
        ItemObject _tmp = _loadObject.GetComponent<ItemObject>();

        Item _tmpItem;
        _tmpItem = new Item(humanName, objectNum, originNum, _loadObject);
        /* 추가한 ItemObject에 현재 Item의 정보를 담음 */
        _tmp._thisItem = _tmpItem;
        _tmp._thisItem.itemName = humanName;
        _tmp._humanInitPosition = pos;

        //HumanItem _tmpHuman = new HumanItem("Idle", objectNum);
        _tmp._thisHuman = _tmpHuman;

        itemListControl.AddHuman(_tmpItem);

        //////////////////////////////////////////////////////
        ////////////// Dress부분 처리 ////////////////////////
        //////////////////////////////////////////////////////

        _tmpHuman = _tmp._thisHuman;

        Transform _quick = _tmpItem.item3d.transform.Find("shirt");
        for (int i = 0; i < _quick.childCount; i++)
        {
            if (_quick.GetChild(i).name.CompareTo(_tmpHuman._shirtName) == 0)
            {
                _tmpHuman._shirt = _quick.GetChild(i).gameObject;
                _quick.GetChild(i).gameObject.SetActive(true);
            }
            else _quick.GetChild(i).gameObject.SetActive(false);
        }
        //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        _quick = _tmpItem.item3d.transform.Find("pant");
        for (int i = 0; i < _quick.childCount; i++)
        {
            if (_quick.GetChild(i).name.CompareTo(_tmpHuman._pantName) == 0)
            {
                _tmpHuman._pant = _quick.GetChild(i).gameObject;
                _quick.GetChild(i).gameObject.SetActive(true);
            }
            else _quick.GetChild(i).gameObject.SetActive(false);
        }
        //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        if (_tmpHuman._shoesName != null)
        {
            string[] _name = _tmpHuman._shoesName.Split('_');
            if (_name[1].Equals("normal")) //현재 발 상태가 normal
            {
                _quick = _tmpItem.item3d.transform.Find("shoes");
                for (int i = 0; i < _quick.GetChild(0).childCount; i++)
                {
                    //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                    if (_quick.GetChild(0).GetChild(i).name.CompareTo(_tmpHuman._shoesName) == 0)
                    {
                        _tmpHuman._shoes = _quick.GetChild(0).GetChild(i).gameObject;
                        _quick.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    }
                    else _quick.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }
            else if (_name[1].Equals("abnormal"))//abnormal
            {
                _quick = _tmpItem.item3d.transform.Find("shoes");
                _quick.GetChild(1).gameObject.SetActive(true);
                _quick.GetChild(0).gameObject.SetActive(false);
                for (int i = 0; i < _quick.GetChild(1).childCount; i++)
                {
                    //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                    if (_quick.GetChild(1).GetChild(i).name.CompareTo(_tmpHuman._shoesName) == 0)
                    {
                        _tmpHuman._shoes = _quick.GetChild(1).GetChild(i).gameObject;
                        _quick.GetChild(1).GetChild(i).gameObject.SetActive(true);
                    }
                    else _quick.GetChild(1).GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        GameObject _cloth = null;
        Transform _colourization = null;
        SkinnedMeshRenderer _colourizationSkin = null;
        Material _copyMaterial = null;
        GameObject _clothes = null;

        if (_tmpHuman._shirt)
        {
            _cloth = _tmpHuman._shirt;

            _clothes = Instantiate(_cloth);

            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._shirt_R / 255f, _tmpHuman._shirt_G / 255f, _tmpHuman._shirt_B / 255f, 40f / 255f);
        }
        if (_tmpHuman._pant != null)
        {
            _cloth = _tmpHuman._pant;
            _clothes = Instantiate(_cloth);
            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._pant_R / 255f, _tmpHuman._pant_G / 255f, _tmpHuman._pant_B / 255f, 40f / 255f);
        }
        if (_tmpHuman._shoes != null)
        {
            _cloth = _tmpHuman._shoes;
            _clothes = Instantiate(_cloth);
            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(_tmpHuman._shoes_R / 255f, _tmpHuman._shoes_G / 255f, _tmpHuman._shoes_B / 255f, 40f / 255f);
        }

        //////////////////////////////////////////////////////
        ////////////// Dress부분 처리 ////////////////////////
        //////////////////////////////////////////////////////

        //_loadObject.AddComponent<Outline>();
        ///* 윤곽선 안보이게 처리! */
        //_loadObject.GetComponent<Outline>().enabled = false;

        /* 위치 설정 */
        _loadObject.transform.parent.position = pos;



        /* 회전값 설정 */
        _loadObject.transform.Rotate(rot);

        /* 크기값 설정*/
        _loadObject.transform.parent.localScale = scale;

        /* 스케줄러 생성 파트*/
        //스몰바 생성 및 설정
        GameObject _smallBar = Instantiate(Resources.Load("Prefab/SmallScheduler_Sample")) as GameObject;
        if (originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _smallBar.name = "Man" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        else if (originNum == 2000)
        {
            _smallBar.name = "Daughter" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        else if (originNum == 2002)
        {
            _smallBar.name = "Woman" + (objectNum); //스몰바의 이름 설정
            _smallBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //스몰바의 텍스트설정
        }
        //content 설정
        GameObject _smallContent;
        _smallContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;

        _smallBar.transform.SetParent(_smallContent.transform); //small Bar의 부모설정
        _smallBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //small Bar의 정보
        _smallBar.GetComponent<SmallSchedulerBar>()._objectNumber = objectNum - 1; //오브젝트넘버를 small bar에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._originNumber = originNum; //오리진 정보또한 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._humanNumber = 0;
        _smallBar.GetComponent<SmallSchedulerBar>()._object = _tmpItem;

        //빅바 생성 및 설정
        GameObject _bigBar = Instantiate(Resources.Load("Prefab/BigScheduler_Sample")) as GameObject;
        if (originNum == 2001) //클릭했을때의 오브젝트의 오리진 넘버로 구별
        {
            _bigBar.name = "Man" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        else if (originNum == 2000)
        {
            _bigBar.name = "Daughter" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        else
        {
            _bigBar.name = "Woman" + (objectNum); //빅바의 이름 설정
            _bigBar.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "" + humanName + objectNum; //빅바의 텍스트설정
        }
        //content 설정
        GameObject _bigContent;
        _bigContent = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        _bigBar.transform.SetParent(_bigContent.transform); //big Bar의 부모설정
        _bigBar.transform.localScale = new Vector3(1, 1, 1); //크기 지정
        //big Bar의 정보
        _bigBar.GetComponent<BigSchedulerBar>()._objectNumber = objectNum - 1; //오브젝트넘버를 small bar에 저장
        _bigBar.GetComponent<BigSchedulerBar>()._originNumber = originNum; //오리진 정보또한 저장
        _bigBar.GetComponent<BigSchedulerBar>()._humanNumber = 0; //휴먼넘버를 저장
        //생성된 빅바를 스몰바에 저장
        _smallBar.GetComponent<SmallSchedulerBar>()._bigScheduler = _bigBar;
        //big Bar는 우선 안보이게 처리
        _bigBar.SetActive(false);
        itemListControl._dataBaseBigBar.Add(_bigBar);
        itemListControl._dataBaseSmallbar.Add(_smallBar);

        ////////////////////////////////////////////////////////////////////

        foreach (AniBarDelete A in abd)
        {
            float _barX = A.GetBarX();
            float _barWidth = A.GetBarWidth();
            string _animationName = A.GetAnimationName();
            string _parentName = A.GetParentName();
            string _animationText = A.GetAnimationText();
            int _moveOrState = A.GetMoveOrState();
            int _actionOrFace = A.GetActionOrFace();
            int _layerNumber = A.GetLayerNumber();
            float _arriveX = A.GetArriveX();
            float _arriveY = A.GetArriveY();
            float _arriveZ = A.GetArriveZ();
            int originNumber = A.GetOriginNumber();
            int _rotation = A.GetRotation();
            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _animationName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _animationText;

            Vector3 _arriveLocation = new Vector3(_arriveX, _arriveY, _arriveZ);

            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
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

            Item _item = null;
            Animator _animator = null;
            foreach (Item B in itemListControl._dataBaseHuman)
            {
                if (B._objectNumber == objectNum)
                {
                    _item = B;
                    _animator = B.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator; //사람객체 리스트를 돌면서 확인해야함
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _animationName;
            _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _animationText;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _animationText;
            _smallAniBar.name = _animationText;
            _bigAniBar.name = _animationText;
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
            if (originNum == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }

            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
            _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;

            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _bigAniBar.transform.GetChild(1).transform.localPosition = new Vector3(-_barWidth / 2, 0, 0);
            _bigAniBar.transform.GetChild(2).transform.localPosition = new Vector3(_barWidth / 2, 0, 0);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

            if (_layerNumber < 2) // 1임
            {
                _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            }
            else // 3, 5, 4 ,2
            {
                _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
                _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
                _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
                _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            }

            //if (_layerNumber == 0)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
            //}
            //else if (_layerNumber == 3)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

            //}
            //else if (_layerNumber == 2)
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
            //}
            //else
            //{
            //    _bigAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //빅애니바의 색상을 변경
            //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //왼쪽드래그바의 색상 변경
            //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //오른쪽 드래그바의 색상 변경
            //    _smallAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
            //}

            itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        }
        foreach (AniBarVoiceDelete AA in abvd)
        {
            float _barX = AA.GetBarX();
            float _barWidth = AA.GetBarWidth();
            string _voiceName = AA.GetVoiceName();
            string _parentName = AA.GetParentName();
            int _dir_key = AA.GetDirkey();
            int originNumber = AA.GetOriginNumber();

            GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
            GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject;
            _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _voiceName;
            _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _voiceName;

            AudioClip _audioClip = voiceFileController.audioInfo[_dir_key][_voiceName];

            /*스몰애니바를 생성*/
            GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
            GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._dir_key = _dir_key;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._voice = true;

            /*찾아야하는 정보*/
            Item _item = null;
            Animator _animator = null;
            foreach (Item B in itemListControl._dataBaseHuman)
            {
                if (B.itemName == _parentName)
                {
                    objectNum = B._objectNumber;
                    _item = B;
                    _animator = B.item3d.GetComponent<Animator>();
                    break;
                }
            }

            _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _voiceName;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _voiceName;

            int _parentNumber = 5;

            /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
            if (originNum == 2001)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2000)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else if (originNum == 2003)
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }
            else
            {
                _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
                _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
            }

            /*빅애니바, 스몰애니바의 부모설정*/
            _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
            _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
            _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
            _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

            _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
            _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
            _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;


            _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

            float _aniBarWidth = _bigAniBar.transform.GetComponent<RectTransform>().rect.width;
            float _width = _aniBarWidth * _audioClip.length / 11.73f;
            _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
            _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

            _smallAniBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;

            itemListControl.AddVoiceDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());

        }

        //shirt의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _tmpItem.item3d.transform.Find("shirt").childCount; i++)
        {
            if (_tmpItem.item3d.transform.Find("shirt").GetChild(i).gameObject.activeSelf)
            {
                _tmpHuman._shirt = _tmpItem.item3d.transform.Find("shirt").GetChild(i).gameObject;
                break;
            }
        }
        //pant의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        for (int i = 0; i < _tmpItem.item3d.transform.Find("pant").childCount; i++)
        {
            if (_tmpItem.item3d.transform.Find("pant").GetChild(i).gameObject.activeSelf)
            {
                _tmpHuman._pant = _tmpItem.item3d.transform.Find("pant").GetChild(i).gameObject;
                break;
            }
        }
        //shoes의 자식 객체를 탐색하며 활성화되어있는 객체 저장
        if (_tmpItem.item3d.transform.Find("shoes").GetChild(0).gameObject.activeSelf) //현재 발 상태가 normal
        {
            for (int i = 0; i < _tmpItem.item3d.transform.Find("shoes").GetChild(0).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_tmpItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject.activeSelf)
                {
                    _tmpHuman._shoes = _tmpItem.item3d.transform.Find("shoes").GetChild(0).GetChild(i).gameObject;
                    break;
                }
            }
        }
        else //abnormal
        {
            for (int i = 0; i < _tmpItem.item3d.transform.Find("shoes").GetChild(1).childCount; i++)
            {
                //normal의 자식객체를 탐색하며 활성화된 객체의 경우 저장
                if (_tmpItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject.activeSelf)
                {
                    _tmpHuman._shoes = _tmpItem.item3d.transform.Find("shoes").GetChild(1).GetChild(i).gameObject;
                    break;
                }
            }
        }


        pushObjectCreateHist(_loadObject, originNum, objectNum);

    }//끝

    private void PopAniBarDeleteY()
    {
        AniBarDelete A = _aniBarDeleteHistY.Pop();
        float _barX = A.GetBarX();
        float _barWidth = A.GetBarWidth();
        string _animationName = A.GetAnimationName();
        string _parentName = A.GetParentName();
        string _animationText = A.GetAnimationText();
        int _moveOrState = A.GetMoveOrState();
        int _actionOrFace = A.GetActionOrFace();
        int _layerNumber = A.GetLayerNumber();
        float _arriveX = A.GetArriveX();
        float _arriveY = A.GetArriveY();
        float _arriveZ = A.GetArriveZ();
        int originNum = A.GetOriginNumber();
        int _rotation = A.GetRotation();
        GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
        _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _animationName;
        _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _animationText;

        Vector3 _arriveLocation = new Vector3(_arriveX, _arriveY, _arriveZ);

        GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
        GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
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

        int objectNum = 0;
        Item _item = null;
        Animator _animator = null;
        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B.itemName == _parentName)
            {
                objectNum = B._objectNumber;
                _item = B;
                _animator = B.item3d.GetComponent<Animator>();
                break;
            }
        }

        _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item; //사람객체 리스트를 돌면서 확인해야함
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator; //사람객체 리스트를 돌면서 확인해야함
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _animationName;
        _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _animationText;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _animationText;
        _smallAniBar.name = _animationText;
        _bigAniBar.name = _animationText;

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
        if (originNum == 2001)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2000)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2003)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }

        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;

        _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
        _bigAniBar.transform.GetChild(1).transform.localPosition = new Vector3(-_barWidth / 2, 0, 0);
        _bigAniBar.transform.GetChild(2).transform.localPosition = new Vector3(_barWidth / 2, 0, 0);
        _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_barWidth, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

        //if (_layerNumber == 0) // 1임
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
        //}
        //else if (_layerNumber == 3) // 3, 5, 4 ,2
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        //}
        //else if (_layerNumber == 2)
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        //}
        //else
        //{
        //    _bigAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //빅애니바의 색상을 변경
        //    _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //왼쪽드래그바의 색상 변경
        //    _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255); //오른쪽 드래그바의 색상 변경
        //    _smallAniBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
        //}

        if (_layerNumber < 2) // 1임
        {
            _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
            _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
            _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
            _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f);
        }
        else // 3, 5, 4 ,2
        {
            _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
            _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //왼쪽드래그바의 색상 변경
            _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //오른쪽 드래그바의 색상 변경
            _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        }

        itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        pushAniBarCreateHist(_bigAniBar, _smallAniBar, _animationName, _animationText, objectNum, _smallAniBar.GetComponent<SmallAniBar>()._voice == true ? 0 : 1);
    }//끝

    private void PopAniBarVoiceDeleteY()
    {
        AniBarVoiceDelete A = _aniBarVoiceDeleteHistY.Pop();
        float _barX = A.GetBarX();
        float _barWidth = A.GetBarWidth();
        string _voiceName = A.GetVoiceName();
        string _parentName = A.GetParentName();
        int _dir_key = A.GetDirkey();
        int originNum = A.GetOriginNumber();


        GameObject _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject;
        _bigAniBar.transform.GetComponent<BigAniBar>()._animationName = _voiceName;
        _bigAniBar.transform.GetChild(0).GetComponent<Text>().text = _voiceName;
        
        AudioClip _audioClip = voiceFileController.audioInfo[_dir_key][_voiceName];

        /*스몰애니바를 생성*/
        GameObject _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감
        GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._dir_key = _dir_key;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._voice = true;

        /*찾아야하는 정보*/
        int objectNum = 0;
        Item _item = null;
        Animator _animator = null;
        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B.itemName == _parentName)
            {
                objectNum = B._objectNumber;
                _item = B;
                _animator = B.item3d.GetComponent<Animator>();
                break;
            }
        }

        _smallAniBar.transform.GetComponent<SmallAniBar>()._item = _item;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animator = _animator;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName = _voiceName;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _voiceName;
        _bigAniBar.name = _voiceName;
        _smallAniBar.name = _voiceName;

        int _parentNumber = 5;

        /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
        if (originNum == 2001)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2000)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else if (originNum == 2003)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber + 1).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + objectNum).transform.GetChild(_parentNumber).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(_barX, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
        _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;


        _bigAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255); //빅애니바의 색상을 변경
        _smallAniBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        float _aniBarWidth = _bigAniBar.transform.GetComponent<RectTransform>().rect.width;
        float _width = _aniBarWidth * _audioClip.length / 11.73f;
        _bigAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _bigAniBar.transform.GetComponent<RectTransform>().rect.height);
        _smallAniBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAniBar.transform.GetComponent<RectTransform>().rect.height);

        _smallAniBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;

        itemListControl.AddVoiceDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        pushAniBarCreateHist(_bigAniBar, _smallAniBar, _voiceName, _voiceName, objectNum, 0);

    }//끝

    private void PopDressChangeY()
    {
        DressChange A = _dressChangeHistY.Pop();
        string[] _name = A.GetName();
        string prev = A.GetPrev();
        string objectName = A.GetObjectName();
        int objectNum = A.GetObjectNum();
        int originNum = A.GetOriginNum();
        Vector3 _pos = A.GetPos();
        HumanItem _clickedHumanItem;
        Item _clickedItem;
        Vector3 _originPos;

        GameObject _newCloth = null;
        string _prev = "null";
        int _changeNum = 0;

        Debug.Log("Come In Y");
        Debug.Log(_name[0] + _name[1]);
        Debug.Log(prev);
        Debug.Log(objectNum);
        Debug.Log(originNum);

        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B._objectNumber == objectNum && B._originNumber == originNum)
            {
                _newCloth = B.item3d;
            }
        }
        _clickedHumanItem = _newCloth.GetComponent<ItemObject>()._thisHuman;
        _clickedItem = _newCloth.GetComponent<ItemObject>()._thisItem;
        _originPos = _clickedItem.item3d.transform.localPosition;
        if (_name[0] == "shirt")
        {
            _changeNum = 1;
            if (_clickedHumanItem._shirt != null)
            {
                _prev = _clickedHumanItem._shirt.name;
                _clickedHumanItem._shirt.SetActive(false);
            }
            if (prev.CompareTo("null") == 0) _clickedHumanItem._shirt = null;
            else if (prev.CompareTo("null") != 0)
            {
                _newCloth = _newCloth.transform.Find(_name[0] + "/" + prev).gameObject;
                _clickedHumanItem._shirt = _newCloth;
                _clickedHumanItem._shirtName = _newCloth.name;
            }
            pushDressChangeHist(_name, _prev, objectName, objectNum, originNum, _pos);
        }
        else if (_name[0] == "pant")
        {
            _changeNum = 2;
            if (_clickedHumanItem._pant != null)
            {
                _prev = _clickedHumanItem._pant.name;
                _clickedHumanItem._pant.SetActive(false);
            }
            if (prev.CompareTo("null") == 0) _clickedHumanItem._pant = null;
            else if (prev.CompareTo("null") != 0)
            {
                _newCloth = _newCloth.transform.Find(_name[0] + "/" + prev).gameObject;
                _clickedHumanItem._pant = _newCloth;
                _clickedHumanItem._pantName = _newCloth.name;
            }
            pushDressChangeHist(_name, _prev, objectName, objectNum, originNum, _pos);
        }
        else
        {
            _changeNum = 3;
            if (_clickedHumanItem._shoes != null)
            {
                _prev = _clickedHumanItem._shoes.name;
                _clickedHumanItem._shoes.SetActive(false);
            }
            else _prev = "null";
            GameObject _new = null;
            Debug.Log(prev);
            if (prev.CompareTo("null") == 0)
            {
                _clickedHumanItem._shoes = null;
                _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (prev.CompareTo("null") != 0)
            {
                String[] _change = prev.Split('_');
                Debug.Log(_change[1]);
                if (_change[1] == "normal")
                {
                    _new = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).transform.Find(prev).gameObject;
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(true);
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    _new = _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).transform.Find(prev).gameObject;
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(0).gameObject.SetActive(false);
                    _clickedItem.item3d.transform.Find("shoes").transform.GetChild(1).gameObject.SetActive(true);
                    _pos += new Vector3(0, 0.05f, 0);
                }
                _clickedHumanItem._shoes = _new;
                _clickedHumanItem._shoesName = _new.name;
            }
            pushDressChangeHist(_name, _prev, objectName, objectNum, originNum, _originPos);
            _clickedItem.item3d.transform.localPosition = _pos;
            if (_new != null) _new.SetActive(true);
        }
        if (prev.CompareTo("null") != 0) _newCloth.SetActive(true);
        dressController.ForHistoryCheckUI(_clickedHumanItem, _changeNum);
    }
    private void PopTilingY()
    {
        TilingChange _ti = _tilingHistY.Pop();
        int _originNum = _ti.GetOriginNum();
        int _objectNum = _ti.GetObjectNum();
        int _tileOriginNum = _ti.GetTileOriginNum();
        Vector3 _scale = _ti.GetScale();
        Vector3 _scaleZ;
        GameObject _go = null;

        foreach (Item B in itemListControl._dataBaseWall)
        {
            if (B._originNumber == _originNum && B._objectNumber == _objectNum)
            {
                _go = B.item3d;
                break;
            }
        }
        MeshRenderer _mesh = _go.GetComponent<MeshRenderer>();

        if (_tileOriginNum == -1)
        {
            _scaleZ = _mesh.material.mainTextureScale;
            _mesh.material.mainTextureScale = _scale;
            pushTilingHistY(_originNum, _objectNum, _scaleZ, -1);
        }
        else
        {
            Sprite tmp = Instantiate(itemListControl.GetImages(3, _tileOriginNum));
            if (tmp != null) _mesh.material.mainTexture = tmp.texture;
            else _mesh.material.mainTexture = null;
            _go.GetComponent<ItemObject>()._placeNumber = _tileOriginNum;
            pushTilingHistY(_originNum, _objectNum, _scale, -1);
        }
        clickedPlaceControl.ResetForHistory();
    }

    private void PopTileY()
    {
        TileChange A = _tileHistY.Pop();
        int _originNumber = A.GetOriginNum();
        int _objectNumber = A.GetObjectNum();
        int _buildingNumber = A.GetBuildingOriginNum();
        GameObject _target = null;

        foreach (Item B in itemListControl._dataBaseWall)
        {
            if (B._originNumber == _originNumber && B._objectNumber == _objectNumber)
            {
                _target = B.item3d;
            }
        }
        Debug.Log(_target);

        /* 클릭된 Place 객체의 MeshRenderer 컴포넌트를 담음 */
        MeshRenderer _clickedMeshRenderer = _target.GetComponent<MeshRenderer>();

        /* Slot의 OriginNumber에 해당하는 Sprite 정보를 가져옴 */
        Sprite _tmp = itemListControl.GetImages(3, _buildingNumber);

        /* 존재하지 않는 Sprite이면 */
        if (_tmp == null)
        {
            /* 건물의 OriginNumber를 저장 */
            int _buildingOriginNumber = _target.GetComponent<ItemObject>()._thisItem._originNumber;

            /* 건물들의 3DObject가 들어있는 디렉터리 경로 지정 */
            System.IO.DirectoryInfo _dir = new System.IO.DirectoryInfo("Assets/Resources/Item/Wall/Build");

            /* 디렉터리 탐색 */
            foreach (System.IO.FileInfo _file in _dir.GetFiles())
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
        pushTileHist(_originNumber, _objectNumber, _target.GetComponent<ItemObject>()._placeNumber);
        _target.GetComponent<ItemObject>()._placeNumber = _buildingNumber;
    }

    private void PopHandY()
    {
        HandChange hg = _handHistY.Pop();
        int _originNum = hg.GetOriginNum();
        int _objectNum = hg.GetObjectNum();
        bool _isLeft = hg.GetIsLeft();
        string _objectName = hg.GetObjectName();
        Item _thisItem = hg.GetItem();
        Item _thisHuman = null;
        HumanItem _thisHumanItem = null;
        GameObject _handObjectClone = null;
        Transform _hand;
        string _itemName = "null";

        Debug.Log("HandY " + _objectName);
        if (_thisItem != null) Debug.Log("HandY " + _thisItem.itemName);

        foreach (Item A in itemListControl._dataBaseHuman)
        {
            if (A._originNumber == _originNum && A._objectNumber == _objectNum)
            {
                _thisHuman = A;
                _thisHumanItem = A.item3d.GetComponent<ItemObject>()._thisHuman;
                break;
            }
        }

        Debug.Log(_thisHuman.item3d.GetComponent<ItemObject>()._thisItem.itemName);
        Debug.Log(_thisHuman.item3d.GetComponent<ItemObject>()._thisHuman._humanNumber);
        Debug.Log(_thisHumanItem);

        Vector3 _pos = new Vector3(1, 0, 0);

        if (_isLeft)
        {
            _hand = _thisHuman.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
            if (_thisHumanItem._leftHandItem != null)
            {
                _itemName = _thisHumanItem._leftHandItem.itemName;
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._leftHandItem);
                if (!_objectName.Equals("null") && !_thisHumanItem._leftHandItem.itemName.Equals(_thisItem.itemName))
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

        else
        {
            _hand = _thisHuman.item3d.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            if (_thisHumanItem._rightHandItem != null)
            {
                _itemName = _thisHumanItem._rightHandItem.itemName;
                HistoryController.pushHandHist(_thisHuman._originNumber, _thisHuman._objectNumber, _isLeft, _itemName, _thisHumanItem._rightHandItem);
                if (!_objectName.Equals("null") && !_thisHumanItem._rightHandItem.itemName.Equals(_thisItem.itemName))
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

        Vector3 _localPos;

        _handObjectClone = Instantiate(_thisItem.item3d); //객체 생성
        _handObjectClone.name = _thisItem.itemName; //이름 지정
        _handObjectClone.transform.SetParent(_hand); //손을 부모로 설정
        _handObjectClone.transform.localRotation = Quaternion.Euler(Vector3.zero); //로컬 회전값을 손 회전값과 동일하게 변경

        //각 객체에 따라 객체의 위치 조정
        if (_thisHuman._originNumber == 2001) //Man
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._manRightPos;
        }
        else if (_thisHuman._originNumber == 2002) //Woman
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._womanRightPos;
        }
        else if (_thisHuman._originNumber == 2000) //Daughter
        {
            _localPos = _handObjectClone.transform.GetComponentInChildren<HandItem>()._daughterRightPos;
        }
        else if (_thisHuman._originNumber == 2003) //Woongin
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

    }

    private void PopDressRGBChangeY()
    {
        DressRGBChange A = _dressRGBChangeHistY.Pop();
        int objectNum = A.GetObjectNum();
        int originNum = A.GetOriginNum();
        int what = A.GetWhatDress();
        Color color = A.GetChangeColor();
        HumanItem _clickedHumanItem;

        GameObject _changeCloth = null;

        foreach (Item B in itemListControl._dataBaseHuman)
        {
            if (B._objectNumber == objectNum && B._originNumber == originNum)
            {
                _changeCloth = B.item3d;
            }
        }
        _clickedHumanItem = _changeCloth.GetComponent<ItemObject>()._thisHuman;


        GameObject _cloth = null;
        Transform _colourization = null;
        SkinnedMeshRenderer _colourizationSkin = null;
        if (what == 0 && _clickedHumanItem._shirt != null)
        {
            _cloth = _clickedHumanItem._shirt;
            _colourization = _clickedHumanItem._shirt.transform.Find("Colourization");
        }
        else if (what == 1 && _clickedHumanItem._pant != null)
        {
            _cloth = _clickedHumanItem._pant;
            _colourization = _clickedHumanItem._pant.transform.Find("Colourization");
        }
        else if (what == 2 && _clickedHumanItem._shoes != null)
        {
            _cloth = _clickedHumanItem._shoes;
            _colourization = _clickedHumanItem._shoes.transform.Find("Colourization");
        }


        if (_colourization == null) //색상화 객체가 없을 경우
        {
            GameObject _clothes = Instantiate(_cloth);

            _clothes.name = "Colourization"; //이름 변경
            _clothes.transform.SetParent(_cloth.transform);
            _colourization = _clothes.transform; //색상화 객체 추가

            /* 내부 Material을 교체하는 작업 */

            _colourizationSkin = _colourization.GetComponent<SkinnedMeshRenderer>();
            Material _copyMaterial = Instantiate(dressController._dressMaterial); //메터리얼 복사
            _copyMaterial.name = "DressMaterial"; //메터리얼 이름 지정

            for (int i = 0; i < _colourizationSkin.materials.Length; i++)
            {
                Destroy(_colourizationSkin.materials[i]);
            }

            _colourizationSkin.material = _copyMaterial;
            //_colourizationSkin.material.color.r,g,b,a 첨에 1로 초기화 되어 있어서 0으로 초기화 해줌
            _colourizationSkin.material.color = new Color(0f, 0f, 0f, 0f);
        }

        if (what == 0)//상의
        {
            _colourizationSkin = _clickedHumanItem._shirt.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHist(originNum, objectNum, 0, new Color(_clickedHumanItem._shirt_R / 255f, _clickedHumanItem._shirt_G / 255f, _clickedHumanItem._shirt_B / 255f, 40f / 255f));
            _clickedHumanItem._shirt_R = color.r * 255f;
            _clickedHumanItem._shirt_G = color.g * 255f;
            _clickedHumanItem._shirt_B = color.b * 255f;
        }
        else if (what == 1)//하의
        {
            _colourizationSkin = _clickedHumanItem._pant.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHist(originNum, objectNum, 1, new Color(_clickedHumanItem._pant_R / 255f, _clickedHumanItem._pant_G / 255f, _clickedHumanItem._pant_B / 255f, 40f / 255f));
            _clickedHumanItem._pant_R = color.r * 255f;
            _clickedHumanItem._pant_G = color.g * 255f;
            _clickedHumanItem._pant_B = color.b * 255f;
        }
        else if (what == 2)//신발
        {
            _colourizationSkin = _clickedHumanItem._shoes.transform.Find("Colourization").GetComponent<SkinnedMeshRenderer>();
            HistoryController.pushDressRGBHist(originNum, objectNum, 2, new Color(_clickedHumanItem._shoes_R / 255f, _clickedHumanItem._shoes_G / 255f, _clickedHumanItem._shoes_B / 255f, 40f / 255f));
            _clickedHumanItem._shoes_R = color.r * 255f;
            _clickedHumanItem._shoes_G = color.g * 255f;
            _clickedHumanItem._shoes_B = color.b * 255f;
        }
        _colourizationSkin.material.color = color;
    }
    #endregion  Y
}
