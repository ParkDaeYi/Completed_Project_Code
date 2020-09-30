using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  date 2018.12.11
 *  author skyde47
 *  desc
 *  System 내의 동작들에 대한 정보를 담은 클래스
 */

public class ObjectPosAndScale
{
    private GameObject _go;
    private Vector3 _pos;
    private Vector3 _scale;
    private Quaternion _rot;
    private int _objectNum;
    private int _originNum;

    public ObjectPosAndScale(GameObject go, Vector3 pos, Vector3 scale, int objectNum, int originNum, Quaternion rot)
    {
        this._go = go;
        this._pos = new Vector3(pos.x, pos.y, pos.z);
        this._scale = new Vector3(scale.x, scale.y, scale.z);
        this._objectNum = objectNum;
        this._originNum = originNum;
        this._rot = rot;
    }

    public GameObject getHistoryTarget()
    {
        return _go;
    }

    public Vector3 getHistoryPosition()
    {
        return _pos;
    }

    public Vector3 getHistoryScale()
    {
        return _scale;
    }

    public int getObjectNum()
    {
        return _objectNum;
    }

    public int getOriginNum()
    {
        return _originNum;
    }

    public Quaternion getRot()
    {
        return _rot;
    }
}

public class AniBarPosAndWidth
{
    private GameObject _go;
    private Vector3 _pos;
    private float _width;
    private int _objectNum;
    private string _animationName;
    private bool _voice;

    public AniBarPosAndWidth(GameObject go, Vector3 pos, float width, int objectNum, string animationName, bool voice)
    {
        this._go = go;
        this._pos = new Vector3(pos.x, pos.y, pos.z);
        this._width = width;
        this._objectNum = objectNum;
        this._animationName = animationName;
        this._voice = voice;
    }

    public GameObject getHistoryTarget()
    {
        return _go;
    }

    public Vector3 getHistoryPosition()
    {
        return _pos;
    }

    public float getHistoryWidth()
    {
        return _width;
    }

    public int getObjectNum()
    {
        return _objectNum;
    }

    public string getAnimationName()
    {
        return _animationName;
    }

    public bool getVoice()
    {
        return _voice;
    }
}

public class AniBarCreate
{
    private GameObject _biggo;
    private GameObject _smallgo;
    private string _animationName;
    private string _anibarName;
    private int _objectNum;
    private int _voice;

    public AniBarCreate(GameObject go, GameObject go1, string str, string str1, int objectNum, int voice)
    {
        this._biggo = go;
        this._smallgo = go1;
        this._animationName = str;
        this._anibarName = str1;
        this._objectNum = objectNum;
        this._voice = voice;
    }

    public GameObject GetBigGameObject()
    {
        return _biggo;
    }

    public GameObject GetSmallGameObject()
    {
        return _smallgo;
    }

    public string GetAnimationName()
    {
        return _animationName;
    }

    public string GetAnibarName()
    {
        return _anibarName;
    }

    public int GetObjectNum()
    {
        return _objectNum;
    }

    public int GetVoice()
    {
        return _voice;
    }
}

public class ObjectCreate
{
    private GameObject _go;
    private int _originNum;
    private int _objectNum;

    public ObjectCreate(GameObject go, int originNum, int objectNum)
    {
        this._go = go;
        this._objectNum = objectNum;
        this._originNum = originNum;
    }

    public GameObject GetGameObject()
    {
        return _go;
    }

    public int GetObjectNum()
    {
        return _objectNum;
    }

    public int GetOriginNum()
    {
        return _originNum;
    }
}

public class ObjectDelete
{
    private int _objectNum;
    private int _originNum;
    private string _itemName;
    private Vector3 _pos;
    private Vector3 _rot;
    private Vector3 _scale;

    public ObjectDelete(int objectNum, int originNum, string itemName, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        this._objectNum = objectNum;
        this._originNum = originNum;
        this._itemName = itemName;
        this._pos = pos;
        this._rot = rot;
        this._scale = scale;
    }
    public int GetObjectNum()
    {
        return _objectNum;
    }

    public int GetOriginNum()
    {
        return _originNum;
    }

    public string GetItemName()
    {
        return _itemName;
    }

    public Vector3 GetPos()
    {
        return _pos;
    }

    public Vector3 GetRot()
    {
        return _rot;
    }

    public Vector3 GetScale()
    {
        return _scale;
    }
}

public class HumanDelete
{
    private int _originNum;
    private int _objectNum;
    private string _humanName;
    private Vector3 _pos;
    private Vector3 _rot;
    private Vector3 _scale;
    private List<AniBarDelete> aniBarDelete = new List<AniBarDelete>();
    private List<AniBarVoiceDelete> aniBarVoiceDelete = new List<AniBarVoiceDelete>();
    private HumanItem _humanItem;

    public HumanDelete(int originNum, int objectNum, string humanName, Vector3 pos, Vector3 rot, Vector3 scale, List<AniBarDelete> A, List<AniBarVoiceDelete> AA, HumanItem hu)
    {
        this._originNum = originNum;
        this._objectNum = objectNum;
        this._humanName = humanName;
        this._pos = pos;
        this._rot = rot;
        this._scale = scale;
        this.aniBarDelete = A;
        this.aniBarVoiceDelete = AA;
        this._humanItem = hu;
    }
    public int GetOriginNum()
    {
        return _originNum;
    }
    public int GetObjectNum()
    {
        return _objectNum;
    }

    public string GetHumanName()
    {
        return _humanName;
    }

    public Vector3 GetPos()
    {
        return _pos;
    }

    public Vector3 GetRot()
    {
        return _rot;
    }

    public Vector3 GetScale()
    {
        return _scale;
    }

    public List<AniBarDelete> GetAniBarDelete()
    {
        return aniBarDelete;
    }

    public List<AniBarVoiceDelete> GetAniBarVoiceDelete()
    {
        return aniBarVoiceDelete;
    }

    public HumanItem GetHumanItem()
    {
        return _humanItem;
    }
}

public class AniBarDelete
{
    private float _barX;
    private float _barWidth;
    private string _animationName;
    private string _parentName;
    private string _animationText;
    private int _moveOrState;
    private int _actionOrFace;
    private int _layerNumber;
    private float _arriveX;
    private float _arriveY;
    private float _arriveZ;
    private int _originNumber;
    private int _rotation;

    public AniBarDelete(float barX, float barWidth, string animationName, string parentName, string animationText, int moveOrState, int actionOrFace, int layerNumber,
        float arriveX, float arriveY, float arriveZ, int originNumber, int rotation)
    {
        _barX = barX;
        _barWidth = barWidth;
        _animationName = animationName;
        _parentName = parentName;
        _animationText = animationText;
        _moveOrState = moveOrState;
        _actionOrFace = actionOrFace;
        _layerNumber = layerNumber;
        _arriveX = arriveX;
        _arriveY = arriveY;
        _arriveZ = arriveZ;
        _originNumber = originNumber;
        _rotation = rotation;
    }

    public float GetBarX() { return _barX; }
    public float GetBarWidth() { return _barWidth; }
    public string GetAnimationName() { return _animationName; }
    public string GetParentName() { return _parentName; }
    public string GetAnimationText() { return _animationText; }
    public int GetMoveOrState() { return _moveOrState; }
    public int GetActionOrFace() { return _actionOrFace; }
    public int GetLayerNumber() { return _layerNumber; }
    public float GetArriveX() { return _arriveX; }
    public float GetArriveY() { return _arriveY; }
    public float GetArriveZ() { return _arriveZ; }
    public int GetOriginNumber() { return _originNumber; }
    public int GetRotation() { return _rotation; }
}

public class AniBarVoiceDelete
{
    private float _barX;
    private float _barWidth;
    private string _voiceName;
    private string _parentName;
    private int _dir_key;
    private int _originNumber;

    public AniBarVoiceDelete(float barX, float barWidth, string voiceName, string parentName, int dirkey, int originNumber)
    {
        _barX = barX;
        _barWidth = barWidth;
        _voiceName = voiceName;
        _parentName = parentName;
        _dir_key = dirkey;
        _originNumber = originNumber;
    }

    public float GetBarX() { return _barX; }
    public float GetBarWidth() { return _barWidth; }
    public string GetVoiceName() { return _voiceName; }
    public string GetParentName() { return _parentName; }
    public int GetDirkey() { return _dir_key; }
    public int GetOriginNumber() { return _originNumber; }
}

public class DressChange
{
    private string[] _name;
    private string _prev;
    private string _objectName;
    private int _originNum;
    private int _objectNum;
    private Vector3 _pos;

    public DressChange(string[] name, string prev, string objectName, int originNum, int objectNum, Vector3 pos)
    {
        _name = name;
        _prev = prev;
        _objectName = objectName;
        _objectNum = objectNum;
        _originNum = originNum;
        _pos = pos;
    }

    public string[] GetName() { return _name; }
    public string GetPrev() { return _prev; }
    public string GetObjectName() { return _objectName; }
    public int GetOriginNum() { return _originNum; }
    public int GetObjectNum() { return _objectNum; }
    public Vector3 GetPos() { return _pos; }
}

public class TilingChange
{
    private int _originNum;
    private int _objectNum;
    private Vector3 _scale;
    private int _tileoriginNum;

    public TilingChange(int ori, int obj, Vector3 scale, int tile)
    {
        _originNum = ori;
        _objectNum = obj;
        _scale = scale;
        _tileoriginNum = tile;
    }

    public int GetOriginNum() { return _originNum; }
    public int GetObjectNum() { return _objectNum; }
    public Vector3 GetScale() { return _scale; }
    public int GetTileOriginNum() { return _tileoriginNum; }
}

public class TileChange
{
    private int _originNum;
    private int _objectNum;
    private int _buildingOriginNumber;

    public TileChange(int ori, int obj, int bul)
    {
        _originNum = ori;
        _objectNum = obj;
        _buildingOriginNumber = bul;
    }

    public int GetOriginNum() { return _originNum; }
    public int GetObjectNum() { return _objectNum; }
    public int GetBuildingOriginNum() { return _buildingOriginNumber; }
}

public class HandChange
{
    private int _originNum;
    private int _objectNum;
    private bool _isleft;
    private string _objectName;
    private Item _handItem;

    public HandChange(int ori, int obj, bool left, string name, Item hand)
    {
        _originNum = ori;
        _objectNum = obj;
        _isleft = left;
        _objectName = name;
        _handItem = hand;
    }

    public int GetOriginNum() { return _originNum; }
    public int GetObjectNum() { return _objectNum; }
    public bool GetIsLeft() { return _isleft; }
    public string GetObjectName() { return _objectName; }
    public Item GetItem() { return _handItem; }
}

public class DressRGBChange
{
    private int _originNum;
    private int _objectNum;
    private int _whatDress;
    private Color _changeColor;

    public DressRGBChange(int ori, int obj, int what, Color color)
    {
        _originNum = ori;
        _objectNum = obj;
        _whatDress = what;
        _changeColor = color;
    }

    public int GetOriginNum() { return _originNum; }
    public int GetObjectNum() { return _objectNum; }
    public int GetWhatDress() { return _whatDress; }
    public Color GetChangeColor() { return _changeColor; }
}