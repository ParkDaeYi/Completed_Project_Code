using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class readTextFiles : MonoBehaviour {
    /**
* date 2018.09.28
* author INHO
* desc
* 각 객체의 아이콘마다 기본적인 부가 설명(.txt) 파일을 불러오기 및
* 화면에 띄우는 Scripts.
*/

    /**
* date 2018.10.01
* author LUGUP
* desc
*   설명에 들어갈 객체 소환 및 Slot.cs와의 연동을 통해서 설명칸의 효과 추가
*   BackGroundService() 추가
*/

    [Header("Text")]
    public GameObject _textObject;
    public string _printName;

    [Header("Status")]
    public bool isItem;
    public bool isHuman;
    public bool isPlace;
    public bool _isON = false;

    [Header("BackGroundService")]
    public GameObject _objectPlace;

    private string _itemPath;
    private string _humanPath;
    private string _placePath;

    private string _path;
    private string _content;
    private string dir_path;

    private float _buttonPosition;
    GameObject _exampleItem;

    public void Start()
    {
        /* static 선언으로, Static.cs를 통해 다른 Scripts 에서 응용 가능하도록 설정. */
        Static.STATIC.readTextFiles = this;

        dir_path = Static.STATIC.dir_path;
        _itemPath = dir_path + "/Resources/Item/Items/Text";
        _humanPath = dir_path + "/Resources/Item/Human/Text";
        _placePath = dir_path + "/Resources/Item/Place/Text";
    }

    void Update()
    {
        if (!_isON) {
            _textObject.SetActive(false);
            return;
        }

        /* 아이템에 대한 설명일 경우. */
        if (isItem) { _path = _itemPath; }
        /* 사람 객체에 대한 설명일 경우. */
        else if (isHuman) { _path = _humanPath; }
        /* Place에 대한 설명일 경우. */
        else { _path = _placePath; }

        /* 텍스트 파일에 있는 내용 채우기 */
        _content = System.IO.File.ReadAllText(_path + "/" + _printName+".txt", Encoding.UTF8);

        /* _content 변수에 저장되어 있는 내용 화면에 출력하기. */
        _textObject.transform.GetChild(1).GetComponent<Text>().text = _content;

        /* 화면에 보이게 Active 시키기. */
        _textObject.SetActive(true);
    }

    public void BackGroundService(GameObject _object)
    {
        _exampleItem = Instantiate(_object) as GameObject;
        string[] part = _printName.Split('_');
        if (part[1] == "object") //object 객체인 경우 별도의 크기 조정 필요
        {
            _exampleItem.transform.localScale = new Vector3(5, 5, 5) * 3;
        }
        _exampleItem.transform.SetParent(_objectPlace.transform);
        _exampleItem.transform.position = _objectPlace.transform.position;
    }

    public void BackGroundServiceOff()
    {
        Destroy(_exampleItem);
    }

    /* Slot.cs의 OnpointerEnter 와 연동해서 위치를 조정*/
    public void GetButtonPosition(float x)
    {
        _buttonPosition = x + 40;
        _textObject.transform.position = new Vector3(_buttonPosition, _textObject.transform.position.y, _textObject.transform.position.z);
    }



}
