using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftBarDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float _currentXPos; //// 현재 x좌표 저장하는 변수
    public GameObject _rightBar; //오른쪽 바
    public GameObject _leftBar; //왼쪽 바
    public GameObject _drag; //가운데 애니메이션
    public GameObject _minDivision;

    public Texture2D _cursor; //변경할 마우스 포인터
    public Vector2 _hotSpot;

    public RectTransform _rect;
    const float _rightMax = 939.3f;
    const float _leftMax = 159.5f;
    const float _timerBar = 1880.6f;
    const float _minWidth = 10;
    public float _loadWidth;

    public GameObject _detailTime;
    public Text _detailTimeText;

    void Awake()
    {
        _detailTime = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/DragTime") as GameObject;
        _detailTimeText = _detailTime.transform.GetChild(0).GetComponent<Text>();

        _leftBar = this.gameObject;
        _rightBar = this.gameObject.transform.parent.GetChild(2).gameObject;
        _drag = this.gameObject.transform.parent.gameObject;
        _rect = _drag.transform.GetComponent<RectTransform>();
        _minDivision = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/MinDivisionLine");
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {/*
        float _width = _rightBar.transform.localPosition.x - _leftBar.transform.localPosition.x;
        if (_width < 20) _width = 20.0f;
        Vector2 v2 = new Vector2(_width, _drag.transform.GetComponent<RectTransform>().rect.height);
        _drag.transform.GetComponent<RectTransform>().sizeDelta = v2; //사이즈 변경*/
        //Debug.Log(_drag.GetComponent<RectTransform>().rect.width);
        float _dragwidth = _drag.GetComponent<RectTransform>().rect.width;
        float _leftwidth = _leftBar.GetComponent<RectTransform>().rect.width;
        Vector3 _leftP = _leftBar.transform.localPosition;
        Vector3 _rightP = _rightBar.transform.localPosition;
        _leftP.x = _leftwidth / 2 - _dragwidth / 2;
        _rightP.x = -_leftwidth / 2 + _dragwidth / 2;
        _leftBar.transform.localPosition = _leftP;
        _rightBar.transform.localPosition = _rightP;
    }

    public void OnMouseEnter()
    {
        //Debug.Log("마우스 온");
        _hotSpot.x = _cursor.width / 2;
        _hotSpot.y = _cursor.height / 2;
        Cursor.SetCursor(_cursor, _hotSpot, CursorMode.Auto);
    }

    public void OnMouseExit()
    {
        //Debug.Log("마우스 아웃");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    //드래그시작할때
    public void OnBeginDrag(PointerEventData eventData)
    {
        HistoryController.pushAniBarHist(_drag.gameObject, _drag.transform.localPosition, _drag.GetComponent<RectTransform>().rect.width, _drag.GetComponent<BigAniBar>()._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber, _drag.GetComponent<BigAniBar>()._smallAniBar.GetComponent<SmallAniBar>()._animationName, _drag.GetComponent<BigAniBar>()._smallAniBar.GetComponent<SmallAniBar>()._voice);
        _detailTime.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 40);
        Debug.Log("RightBarDrag.cs 29번줄 드래그시작");
        _currentXPos = Input.mousePosition.x;   // x축만 이동할 것이기 때문에 x축만 사용
        _detailTime.SetActive(true);
        DetailContentUpdate();
    }

    //드래그중일때
    public void OnDrag(PointerEventData eventData)
    {
        _detailTime.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 40);
        DetailContentUpdate();

        float _moveX = Input.mousePosition.x - _currentXPos;
        Vector3 _pos = _drag.transform.localPosition;
        _pos.x += _moveX / 2;
        _drag.transform.localPosition = _pos;
        float _width = _drag.GetComponent<RectTransform>().rect.width - _moveX;
        if (_width < 20) _width = 20.0f;
        Vector2 v2 = new Vector2(_width, _drag.transform.GetComponent<RectTransform>().rect.height);
        _drag.transform.GetComponent<RectTransform>().sizeDelta = v2; //사이즈 변경
        _currentXPos = Input.mousePosition.x; // 다음 드래그때 이동 할 거리를 알기 위해 다시 현재 위치를 구한다.
        _drag.transform.GetComponent<BigAniBar>().InfoUpdate();

    }

    //드래그종료할때
    public void OnEndDrag(PointerEventData eventData)
    {
        _detailTime.SetActive(false);

    }

    public void DetailContentUpdate()
    {
        string _timestr;
        double _time = _drag.GetComponent<BigAniBar>()._startTime;
        _timestr = _time.ToString("00.00"); //00.00형식으로 변환해서 저장
        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
        double _m = (_timei % 3600) / 60; //분 계산
        double _s = (_timei % 3600) % 60; //초 계산
        _detailTimeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
    }

}
