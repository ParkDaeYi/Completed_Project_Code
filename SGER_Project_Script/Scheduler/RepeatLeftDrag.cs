using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepeatLeftDrag : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [Header("UI")]
    public GameObject _canvas;

    [Header("Variable")]
    public GameObject _minLine;
    public GameObject _maxLine;
    public GameObject _cursor;

    [Header("Script")]
    public TimeController _timeScript; //시간변수가 존재하는 스크립트

    public bool _signal = true;

    void Start()
    {
        _minLine = GameObject.Find("MinDivisionLine");
        _maxLine = GameObject.Find("MaxDivisionLine");
        _cursor = GameObject.Find("Cursor");
        _canvas = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu");

        _timeScript = GameObject.Find("Controllers").transform.Find("SchedulerController").GetComponent<TimeController>();
    }

    void Update()
    {
        /* 정지 중이면 RepeatLine의 버튼이 표시됨! */
        transform.GetChild(0).gameObject.SetActive(!_timeScript._sw);
    }

    public void OnDrag(PointerEventData eventData)
    {
        /* 정지 중이면 */
        if (!_timeScript._sw)
        {
            /* 마우스의 위치가 최소 바보다 왼쪽에 있으면 */
            if (Input.mousePosition.x < _minLine.transform.position.x)
            {
                /* 최소 바 위치로 변경 */
                transform.position = new Vector3(_minLine.transform.position.x, transform.position.y, transform.position.z);
            }
            /* 마우스의 위치가 커서 또는 오른쪽 바 보다 오른쪽에 있으면 */
            else if (Input.mousePosition.x > _cursor.transform.position.x || (Static.STATIC._repeatright && Input.mousePosition.x > Static.STATIC._repeatright.transform.position.x))
            {
                /* 오른쪽 바가 없으면 */
                if (!Static.STATIC._repeatright)
                {
                    /* 커서의 위치로 변경 */
                    transform.position = new Vector3(_cursor.transform.position.x, transform.position.y, transform.position.z);
                }
                /* 오른쪽 바가 있으면 */
                else
                {
                    /* 커서의 위치가 더 왼쪽에 있으면 */
                    if (_cursor.transform.position.x < Static.STATIC._repeatright.transform.position.x)
                    {
                        /* 커서의 위치로 변경 */
                        transform.position = new Vector3(_cursor.transform.position.x, transform.position.y, transform.position.z);
                    }
                    /* 오른쪽 바의 위치가 더 왼쪽에 있으면 */
                    else
                    {
                        /* 오른쪽 바의 위치로 변경 */
                        transform.position = new Vector3(Static.STATIC._repeatright.transform.position.x, transform.position.y, transform.position.z);
                    }
                }
            }
            /* 사이에 있으면*/
            else
            {
                /* 마우스 위치로 변경 */
                transform.position = new Vector3(Input.mousePosition.x, transform.position.y, transform.position.z);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Static.STATIC._repeatHuman.Count > 0)
        {
            Debug.Log("Garbage Clearing...");
            Static.STATIC._repeatHuman.Clear();
            Static.STATIC._repeatAniBar.Clear();
            Static.STATIC._repeatHumanPos.Clear();
            Static.STATIC._repeatHumanRotate.Clear();
            Static.STATIC._repeatState.Clear();
            Debug.Log("Garbage Clear...!");
        }
        if (_signal)
        {
            Debug.Log("2");
            GameObject _rightbar = Instantiate(_canvas.transform.Find("RepeatLine").gameObject);
            _rightbar.SetActive(true); //활성화
            _rightbar.name = "RightRepeatLine"; //이름 지정
            _rightbar.AddComponent<RepeatDrag>();
            _rightbar.transform.SetParent(_canvas.transform);
            _rightbar.GetComponent<RepeatDrag>()._minLine = GameObject.Find("MinDivisionLine");
            _rightbar.GetComponent<RepeatDrag>()._maxLine = GameObject.Find("MaxDivisionLine");

            /* 상대 크기 지정 */
            _rightbar.transform.localScale = new Vector3(1, 1, 1);

            /* 생성 위치를 지정 */
            _rightbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x + 30f, GetComponent<RectTransform>().anchoredPosition.y);

            _signal = false;
            Static.STATIC._repeat = true;
            Static.STATIC._repeatright = _rightbar;
        }
        _timeScript._repeatCal = true;
    }
}
