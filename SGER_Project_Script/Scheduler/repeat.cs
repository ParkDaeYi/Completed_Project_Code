using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class repeat : MonoBehaviour
{
    [Header("UI")]
    public GameObject _canvas;
    public GameObject _repeatLine;

    [Header("Variable")]
    public bool _type = false;
    public RectTransform _repeatLineRectTransform; //RepeatLine의 캔버스 상의 Transform 자료형

    void Start()
    {
        _canvas = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu");
        _repeatLine = _canvas.transform.Find("RepeatLine").gameObject;
        _repeatLineRectTransform = _repeatLine.GetComponent<RectTransform>();
    }

    public void onclick()
    {
        if (!_type)
        {
            Debug.Log("1");
            GameObject _leftbar = Instantiate(_repeatLine);
            _leftbar.SetActive(true); //활성화
            _leftbar.name = "LeftRepeatBar"; //이름 지정
            _leftbar.AddComponent<RepeatLeftDrag>();
            _leftbar.GetComponent<RepeatLeftDrag>()._signal = true;
            _leftbar.transform.SetParent(_canvas.transform);

            /* 생성 위치를 지정 */
            _leftbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(_repeatLineRectTransform.anchoredPosition.x + 20f, _repeatLineRectTransform.anchoredPosition.y);

            /* 상대 크기 지정 */
            _leftbar.transform.localScale = new Vector3(1, 1, 1);

            _type = true;
            Static.STATIC._repeatleft = _leftbar;
        }
        else
        {
            Static.STATIC._repeat = false;
            Destroy(Static.STATIC._repeatright);
            Destroy(Static.STATIC._repeatleft);
            _type = false;
        }
    }
}
