using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class VoiceMaking : MonoBehaviour
{

    [Header("Script")]
    public Animator _animator;
    public ClickedItemControl _clickedItemControl;
    public StartDBController _startDBController;
    public ItemListControl _itemListControl;

    [Header("Animation Create Information")]
    public GameObject _aniBarParent;
    public GameObject _bigAniBarParent; //상세스케줄러 부모
    public GameObject _smallAniBarParent; //요약스케줄러 부모
    public GameObject _parentObject;

    [Header("VoicePanel")]
    public GameObject _basePanel;
    public GameObject _reScaleCanvas;
    public GameObject _dressCanvas;
    public GameObject _voiceMadeCanvas;
    public GameObject _ColorChangeCanvas;

    public AudioClip _audioClip;

    // Use this for initialization
    void Start()
    {
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    public void OnClickVoicePanel()
    {
        _dressCanvas.SetActive(false);
        _ColorChangeCanvas.SetActive(false);
        _reScaleCanvas.SetActive(false);

        if (_basePanel.activeSelf) // 켜져있다 -> 끄자!
        {
            _basePanel.SetActive(false);
            _voiceMadeCanvas.SetActive(false);
        }
        else // 꺼져있다 -> 켜자!
        {
            _basePanel.SetActive(true);
            _voiceMadeCanvas.SetActive(true);
        }
    }

    public void OnClickAction()
    {
        if (!_startDBController.audioInfo.ContainsKey(Static.STATIC._dir_key))
        {
            Debug.Log("VoiceMaking.cs : Static 변수 _dir_key 없음");
            return;
        }
        if (!_startDBController.audioInfo[Static.STATIC._dir_key].ContainsKey(Static.STATIC._voicename))
        {
            Debug.Log("VoiceMaking.cs : Static 변수 _voicename 없음");
            return;
        }

        _audioClip = _startDBController.audioInfo[Static.STATIC._dir_key][Static.STATIC._voicename];
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //if (Static.STATIC._voiceGender == true) _audioClip = Resources.Load<AudioClip>("Voice/son/" + Static.STATIC._voicename);
        //else _audioClip = Resources.Load<AudioClip>("Voice/yuinna/" + Static.STATIC._voicename);
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        _aniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감

        GameObject _animationBar = Instantiate(Resources.Load("Prefab/VoiceBig")) as GameObject; //애니메이션 바를 생성
        _animationBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        //보이스 색상은 202 233 189 로 지정

        GameObject _smallAnimationBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject; //스몰 애니메이션 바를 생성
        _smallAnimationBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        if (_clickedItemControl._clickedItem._originNumber == 2001)
        {
            _aniBarParent = _aniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(6).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2000)
        {
            _aniBarParent = _aniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(6).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2003)
        {
            _aniBarParent = _aniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(6).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
        }
        else
        {
            _aniBarParent = _aniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(6).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _animationBar.transform.SetParent(_aniBarParent.transform, false);
        _animationBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAnimationBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAnimationBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        float _aniBarWidth = _animationBar.transform.GetComponent<RectTransform>().rect.width;
        float _width = _aniBarWidth * _audioClip.length / 11.73f;
        _animationBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _animationBar.transform.GetComponent<RectTransform>().rect.height);
        _smallAnimationBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width, _smallAnimationBar.transform.GetComponent<RectTransform>().rect.height);
        _smallAnimationBar.GetComponent<SmallAniBar>()._voice = true;
        _animationBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAnimationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _animationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._audio = _audioClip;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._item = _clickedItemControl._clickedItem;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationName = Static.STATIC._voicename;
        _animationBar.transform.GetChild(0).GetComponent<Text>().text = Static.STATIC._voicename;
        _animationBar.name = Static.STATIC._voicename;
        _smallAnimationBar.name = Static.STATIC._voicename;
        _animationBar.GetComponent<BigAniBar>()._anibarName = _animationBar.name;
        _animationBar.GetComponent<BigAniBar>()._animationName = _animationBar.name;
        _animationBar.transform.GetComponent<BigAniBar>()._thisAniBar = _animationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAnimationBar;
        _smallAnimationBar.GetComponent<SmallAniBar>()._anibarName = _animationBar.name;
        _smallAnimationBar.GetComponent<SmallAniBar>()._dir_key = Static.STATIC._dir_key;


        _itemListControl.AddVoiceDB(_animationBar.transform.GetComponent<BigAniBar>(), _smallAnimationBar.transform.GetComponent<SmallAniBar>());
        HistoryController.pushAniBarCreateHist(_animationBar, _smallAnimationBar, Static.STATIC._voicename, Static.STATIC._voicename, _clickedItemControl._clickedItem._objectNumber, 0);
    }
}