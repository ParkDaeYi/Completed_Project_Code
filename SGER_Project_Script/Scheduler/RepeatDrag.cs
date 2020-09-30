using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepeatDrag : MonoBehaviour, IDragHandler
{
    [Header("UI")]
    public GameObject _minLine;
    public GameObject _maxLine;
    public GameObject _cursor;

    [Header("Script")]
    public TimeController _timeScript; //시간변수가 존재하는 스크립트

    void Start()
    {
        _minLine = GameObject.Find("MinDivisionLine");
        _maxLine = GameObject.Find("MaxDivisionLine");
        _cursor = GameObject.Find("Cursor");

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
            /* 마우스의 X 위치가 왼쪽 바 X 위치보다 왼쪽에 있으면 */
            if (Input.mousePosition.x < Static.STATIC._repeatleft.transform.position.x)
            {
                /* 오른쪽 바의 위치는 왼쪽 바와 동일하게 변경 */
                transform.position = new Vector3(Static.STATIC._repeatleft.transform.position.x, transform.position.y, transform.position.z);
            }
            /* 마우스의 X 위치가 커서의 X 위치보다 오른쪽에 있으면 */
            else if (Input.mousePosition.x > _cursor.transform.position.x)
            {
                /* 오른쪽 바의 위치는 커서로 지정 */
                transform.position = new Vector3(_cursor.transform.position.x, transform.position.y, transform.position.z);
            }
            /* 마우스의 X 위치가 왼쪽 바 X 위치와 스케줄러 최대 X 위치 사이에 있으면 */
            else
            {
                /* 오른쪽 바의 위치는 마우스의 위치로 지정 */
                transform.position = new Vector3(Input.mousePosition.x, transform.position.y, transform.position.z);
            }
        }
    }
}
