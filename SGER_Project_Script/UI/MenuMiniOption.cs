using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuMiniOption : MonoBehaviour
{
    /*
* date 2018.08.17
* author INHO
* desc
* 최소화 창 or 물음표 창 or 닫기 버튼을 눌렀을 경우
* 실행되는 스크립트
*/
    [Header("Panel")]
    public GameObject _panel;
    public GameObject _objectMenu;
    public GameObject _scheulerMenu;
    public GameObject _minimumIcon;
    public GameObject _maximunIcon;

    [Header("StopPositionSetting(입력필요)")]
    public int _enter;
    public int _exit;
    public bool _isHorizental;

    public bool _uiStatus;
    /*
* date 2018.10.01
* author LUGUP
* desc
*  색상변경을 추가함.
*/
    [Header("Color Change")]
    public Text _itemMenuString;
    public Text _schedulerString;
    public Button _itemMenuButton;
    public Button _schedulerButton;

    void Start()
    {
        try
        {
            _itemMenuButton.Select();
            OnClickItemMenu();
        }
        catch (Exception e) { }
    }

    /* 메뉴 창이 위 아래로 이동될 수 있도록 */
    public void OnClickMinimumButton()
    {
        _uiStatus = !_uiStatus; //UI 상태 변경

        if (_uiStatus) //UI가 안으로 들어와야 하면
        {
            StartCoroutine(MoveMiniMenuTrue());
        }
        else //UI가 밖으로 나가야 하면
        {
            StartCoroutine(MoveMiniMenuFalse());
        }
    }

    /*
    * Author : GS
    * Date : 2019.12.30
    * Desc :
    * Mini Menu가 안 팎으로 움직이는 것을 코루틴 함수로 수정 (최적화).
    */
    public IEnumerator MoveMiniMenuTrue() //UI 안으로 들어오는 코루틴 함수
    {
        /* 아이콘이 있으면, 해당 지정 아이콘으로 변경되도록! */
        if (_minimumIcon != null && _maximunIcon != null)
        {
            _minimumIcon.SetActive(true);
            _maximunIcon.SetActive(false);
        }
        /* 가로로 이동해야 될 경우 */
        if (_isHorizental)
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.x - (600 * Time.deltaTime) > _enter)
            {
                yield return new WaitForFixedUpdate();
                /* Horizental 이면.. -> 가로로 이동! */
                _panel.transform.Translate(new Vector3(-600, 0, 0) * Time.deltaTime);
            }
        }

        /* 세로로 이동해야 될 경우 */
        else
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.y + (600 * Time.deltaTime) < _enter)
            {
                yield return new WaitForFixedUpdate();
                /* Horizental이 아니면.. -> 세로로 이동! */
                _panel.transform.Translate(new Vector3(0, 600, 0) * Time.deltaTime);
            }
        }
    }

    public IEnumerator MoveMiniMenuFalse() //UI 밖으로 나가는 코루틴 함수
    {
        /* 아이콘이 있으면, 해당 지정 아이콘으로 변경되도록! */
        if (_minimumIcon != null && _maximunIcon != null)
        {
            _minimumIcon.SetActive(false);
            _maximunIcon.SetActive(true);
        }

        /* 가로로 이동해야 될 경우 */
        if (_isHorizental)
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.x + (600 * Time.deltaTime) < _exit)
            {
                yield return new WaitForFixedUpdate();
                _panel.transform.Translate(new Vector3(600, 0, 0) * Time.deltaTime);
            }
        }

        /* 세로로 이동해야 될 경우 */
        else
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.y - (600 * Time.deltaTime) > _exit)
            {
                yield return new WaitForFixedUpdate();
                _panel.transform.Translate(new Vector3(0, -600, 0) * Time.deltaTime);
            }
        }
    }

    /*
     * Author : geun
     * Date : 2018.08.18
     * Desc :
     *  - ItemMenu를 클릭하면 하단 뷰에서 ItemMenu를 띄움
     *  - Scheduler를 클릭하면 하단 뷰에서 Scheduler를 띄움
     */
    public void OnClickItemMenu()
    {
        /* 각 메뉴 활성화/비활성화 */
        _scheulerMenu.SetActive(false);
        _objectMenu.SetActive(true);

        /* 글자에 색상 넣기 */
        _itemMenuString.text = "<color=#ffffff>" + "ItemMenu" + "</color>";
        _schedulerString.text = "<color=#000000>" + "Scheduler" + "</color>";

        /*눌렀을 때 버튼 색깔 회색으로, 흰색으로 */

        ButtonColorChange(1);
    }

    public void OnClickSchedulerMenu()
    {
        /* 각 메뉴 활성화/비활성화 */
        _objectMenu.SetActive(false);
        _scheulerMenu.SetActive(true);

        /* 색상 넣기 */
        _itemMenuString.text = "<color=#000000>" + "ItemMenu" + "</color>";
        _schedulerString.text = "<color=#ffffff>" + "Scheduler" + "</color>";

        ButtonColorChange(0);
    }

    void ButtonColorChange(int i)
    {
        ColorBlock _cb = _itemMenuButton.colors;
        ColorBlock _cb2 = _schedulerButton.colors;
        if (i == 0)
        {
            _cb.normalColor = Color.white;
            _cb2.normalColor = _cb2.highlightedColor;

        }
        else
        {
            _cb2.normalColor = Color.white;
            _cb.normalColor = _cb.highlightedColor;
        }
        _itemMenuButton.colors = _cb;
        _schedulerButton.colors = _cb2;
    }
}