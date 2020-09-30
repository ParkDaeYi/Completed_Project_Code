using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEnterMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /*
* date 2018.08.17
* author INHO
* desc
* 만약 마우스를 Enter(통과) 시키고 있을때, 해당 메뉴를 UI 안으로 보여주고,
* 마우스가 밖으로 나갔을때, 해당 메뉴를 UI 밖으로 보내버리는 스크립트.
*/
    [Header("Panel")]
    public GameObject _panel;

    [Header("StopPositionSetting")]
    public float _enter;
    public float _exit;
    public bool _isHorizental;
    private Coroutine _coroutine; //코루틴 함수용 변수

    /* 마우스가 들어 왔을 때 */
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_coroutine != null) StopCoroutine(_coroutine); //코루틴 함수가 실행 중이면 종료
        _coroutine = StartCoroutine(EnterPanel()); //Panel이 안으로 들어올 수 있도록 함
    }

    /* 마우스가 나갈 때 */
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_coroutine != null) StopCoroutine(_coroutine); //코루틴 함수가 실행 중이면 종료
        _coroutine = StartCoroutine(ExitPanel()); //Panel이 밖으로 나갈 수 있도록 함
    }

    /*
* date 2019.12.30
* author GS
* desc
* MenuCanvas가 안팎으로 움직이는 것을 코루틴 함수로 변경(최적화).
*/
    public IEnumerator EnterPanel() //Panel을 안으로 움직여주는 코루틴 함수
    {
        /* 가로로 이동해야 될 경우 */
        if (_isHorizental)
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.x > _enter)
            {
                yield return new WaitForFixedUpdate();
                /* Horizental 이면.. -> 가로로 이동! */
                _panel.transform.Translate(new Vector3(-500, 0, 0) * Time.deltaTime);
            }
            /* x값 보정 */
            _panel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_enter, _panel.transform.GetComponent<RectTransform>().anchoredPosition.y);
        }

        /* 세로로 이동해야 될 경우 */
        else
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.y > _enter)
            {
                yield return new WaitForFixedUpdate();
                /* Horizental이 아니면.. -> 세로로 이동! */
                _panel.transform.Translate(new Vector3(0, 500, 0) * Time.deltaTime);
            }
            /* y값 보정 */
            _panel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_panel.transform.GetComponent<RectTransform>().anchoredPosition.x, _enter);
        }
    }

    public IEnumerator ExitPanel() //Panel을 밖으로 움직여주는 코루틴 함수
    {
        /* 가로로 이동해야 될 경우 */
        if (_isHorizental)
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.x < _exit)
            {
                yield return new WaitForFixedUpdate();
                _panel.transform.Translate(new Vector3(500, 0, 0) * Time.deltaTime);
            }
            /* x값 보정 */
            _panel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_exit, _panel.transform.GetComponent<RectTransform>().anchoredPosition.y);
        }

        /* 세로로 이동해야 될 경우 */
        else
        {
            while (_panel.transform.GetComponent<RectTransform>().anchoredPosition.y < _exit)
            {
                yield return new WaitForFixedUpdate();
                _panel.transform.Translate(new Vector3(0, -500, 0) * Time.deltaTime);
            }
            /* y값 보정 */
            _panel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_panel.transform.GetComponent<RectTransform>().anchoredPosition.x, _exit);
        }
    }
}