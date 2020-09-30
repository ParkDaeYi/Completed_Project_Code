using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : MonoBehaviour
{
    /**
    * date 2018.11.16
    * author INHO
    * desc
    * 코드 상에 존재하는 Static 을 가진 스크립트형 자료형을 관리 및 선언 해주는 공간.
*/
    public static Static STATIC;

    /* 여기에 선언된 자료형은 static 으로 사용될 스크립트 형식! */
    [Header("Static")]
    public readTextFiles readTextFiles;
    public CameraMoveAroun cameraMoveAroun;
    public List<GameObject> _humanArray;
    public GameObject _saveClickButton;
    public GameObject _clickAniBar;
    public bool _voiceGender = true;//true == man, false == woman
    public string _voicename;
    public bool _repeat = false;
    public GameObject _repeatleft;
    public GameObject _repeatright;
    public List<GameObject> _repeatHuman;
    public ArrayList _repeatAniBar = new ArrayList();
    public ArrayList _repeatHumanPos = new ArrayList();
    public ArrayList _repeatHumanRotate = new ArrayList();
    public ArrayList _repeatState = new ArrayList();
    public int _isSimpleHouse = 1;
    public int _isHouse = 0;
    public int _isCar = 0;
    public string dir_path;
    public int _dir_key;

    /* 타 스크립트의 Start() 보다 먼저 실행되어야 할 필요성이 있는 경우, Awake() 를 통해 선언해준다. */
    public void Awake()
    {
        /* Static 스크립트를 사용하려면, 해당 스크립트에 저장된 내용물을 먼저 채워줘야 한다! */
        Static.STATIC = this;
        _voiceGender = true;

        if (Application.platform == RuntimePlatform.Android)
        {
            dir_path = Application.persistentDataPath;
            Debug.Log("안드로이드 빌드!" + dir_path);
            // Application.persistentDataPath -> /storage/emulated/0/Android/data/com.VRTest.AndroidTest/files
        }
        else
        {
            dir_path = Application.dataPath;
            Debug.Log("안드로이드 빌드 아님!" + dir_path);
            // dir_path -> D:/Unity/androidTest(해당 프로젝트 절대경로)/Assets
        }

    }
}
