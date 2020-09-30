using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMenuClick : MonoBehaviour
{
    /*
* date 2018.08.11
* author INHO
* desc
* Animation 창에서, Action 버튼을 클릭하거나
* Head, Body, Leg, Hand 버튼을 클릭할 때, 각 버튼에 맞는 Animation 메뉴가
* ScrollView 에 표시게되 해주는 Script
*/

    [Header("Menu Buttons")]
    public GameObject _actionButton;
    public GameObject _headButton;
    public GameObject _voiceButton;
    public GameObject _legButton;
    public GameObject _handButton;

    [Header("Menu Buttons")]
    public GameObject _actionScrollView;
    public GameObject _headScrollView;
    public GameObject _VoiceScrollView;
    public GameObject _legScrollView;
    public GameObject _handScrollView;

    /* 동적으로 생성되는 Script 이므로, Find함수를 이용해 연결시켜줌! */
    void Start()
    {
        string _path = "AnimationPanel";
        GameObject _animationPanelSet = GameObject.Find("Canvas").transform.Find("AnimationPanelSet").gameObject;

        _actionScrollView = _animationPanelSet.transform.Find("Action" + _path).gameObject;
        _headScrollView = _animationPanelSet.transform.Find("Head" + _path).gameObject;
        _VoiceScrollView = _animationPanelSet.transform.Find("Voice" + _path).gameObject;
        _legScrollView = _animationPanelSet.transform.Find("Leg" + _path).gameObject;
        _handScrollView = _animationPanelSet.transform.Find("Hand" + _path).gameObject;
    }

    /* 인물 객체에서 Action Button을 클릭 했을 경우 */
    public void OnClickActionButton()
    {
        AlmostFalse(_actionScrollView);
    }

    /* 인물 객체에서 Head Button을 클릭 했을 경우 */
    public void OnClickHeadButton()
    {
        AlmostFalse(_headScrollView);
    }

    /* 인물 객체에서 Voice Button을 클릭 했을 경우 */
    public void OnClickVoiceButton()
    {
        AlmostFalse(_VoiceScrollView);
    }

    /* 인물 객체에서 Leg Button을 클릭 했을 경우 */
    public void OnClickLegButton()
    {
        AlmostFalse(_legScrollView);
    }

    /* 인물 객체에서 Hand Button을 클릭 했을 경우 */
    public void OnClickHandButton()
    {
        AlmostFalse(_handScrollView);
    }

    /* 대부분 ScrollView가 보이지 않게 하기! */
    public void AlmostFalse(GameObject ActiveView)
    {
        //Debug.Log("AnimationMenuClick.cs 75줄 / " + Input.mousePosition);

        if (ActiveView == _actionScrollView) _actionScrollView.SetActive(!_actionScrollView.activeSelf);
        else _actionScrollView.SetActive(false);

        if (ActiveView == _headScrollView) _headScrollView.SetActive(!_headScrollView.activeSelf);
        else _headScrollView.SetActive(false);

        if (ActiveView == _VoiceScrollView) _VoiceScrollView.SetActive(!_VoiceScrollView.activeSelf);
        else _VoiceScrollView.SetActive(false);

        if (ActiveView == _legScrollView) _legScrollView.SetActive(!_legScrollView.activeSelf);
        else _legScrollView.SetActive(false);

        if (ActiveView == _handScrollView) _handScrollView.SetActive(!_handScrollView.activeSelf);
        else _handScrollView.SetActive(false);
    }
}
