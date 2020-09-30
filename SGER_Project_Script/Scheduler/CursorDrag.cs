using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * content : 스케줄러의 커서 드래그 조절
 */

public class CursorDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float _currentXPos; //// 현재 x좌표 저장하는 변수
    public GameObject _minLine;
    public GameObject _maxLine;

    public Text _timeText;
    float _cursorMinPos;
    float _cursorMaxPos;
    public bool _timeCheck; //현재 시간이 흘러가는지 체크
    public float _distanse;
    public bool _canDrag = true;
    public bool _signal = false;
    public GameObject _canvas;


    // Use this for initialization
    void Start()
    {
        _canvas = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu");
        _timeCheck = false;
        _distanse = (_maxLine.transform.position.x - _minLine.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //드래그시작할때
    public void OnBeginDrag(PointerEventData eventData)
    {
        _currentXPos = Input.mousePosition.x;   // x축만 이동할 것이기 때문에 x축만 사용
        TimeController.timeController._drag = true;
    }

    //드래그중일때
    public void OnDrag(PointerEventData eventData)
    {
        if (!_timeCheck && _canDrag)
        {
            float _moveX = Input.mousePosition.x - _currentXPos;
            this.transform.Translate(new Vector3(_moveX, 0, 0)); // 마우스의 이동만큼 드래그바이동
            //드래그바를 이동하는데 이동의 방향에 따라서 _mode 변경

            _currentXPos = Input.mousePosition.x; // 다음 드래그때 이동 할 거리를 알기 위해 다시 현재 위치를 구한다.

            if (!Static.STATIC._repeat)
            {
                //최대 최소에서 벗어나면 최대 최소로 회귀시킴
                if (this.transform.position.x < _minLine.transform.position.x)
                {
                    this.transform.position = new Vector3(_minLine.transform.position.x, this.transform.position.y, this.transform.position.z);
                    if (_timeText != null)
                    {
                        _timeText.text = "00:00.00";
                        TimeController.timeController._time = 0;
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _h = _timei / 3600; //시 계산
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경\
                    }
                }
                else if (this.transform.position.x > _maxLine.transform.position.x)
                {
                    this.transform.position = new Vector3(_maxLine.transform.position.x, this.transform.position.y, this.transform.position.z);
                    if (_timeText != null)
                    {
                        _timeText.text = "03:30.00";
                        TimeController.timeController._time = 210;
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _h = _timei / 3600; //시 계산
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
                    }
                }
                else
                {
                    TimeController.timeController._time = TimeController.timeController._maxMoveTime * (this.transform.position.x - _minLine.transform.position.x) / _distanse;
                    if (_timeText != null)
                    {
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _h = _timei / 3600; //시 계산
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
                    }
                }
            }
            else
            {
                if (this.transform.position.x < Static.STATIC._repeatleft.transform.position.x)
                {
                    this.transform.position = new Vector3(Static.STATIC._repeatleft.transform.position.x, this.transform.position.y, this.transform.position.z);
                    if (_timeText != null)
                    {
                        TimeController.timeController._time = TimeController.timeController._maxMoveTime * (this.transform.position.x - _minLine.transform.position.x) / _distanse;
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        string _StartStr = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1];

                        _timeText.text = _StartStr;
                    }
                }
                else if (this.transform.position.x > Static.STATIC._repeatright.transform.position.x)
                {
                    this.transform.position = new Vector3(Static.STATIC._repeatright.transform.position.x, this.transform.position.y, this.transform.position.z);
                    if (_timeText != null)
                    {
                        TimeController.timeController._time = TimeController.timeController._maxMoveTime * (this.transform.position.x - _minLine.transform.position.x) / _distanse;
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        string _finishStr = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1];

                        _timeText.text = _finishStr;
                    }
                }
                else
                {
                    TimeController.timeController._time = TimeController.timeController._maxMoveTime * (this.transform.position.x - _minLine.transform.position.x) / _distanse;
                    if (_timeText != null)
                    {
                        string _timestr;
                        _timestr = TimeController.timeController._time.ToString("00.00"); //00.00형식으로 변환해서 저장
                        string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                        int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                        int _h = _timei / 3600; //시 계산
                        int _m = (_timei % 3600) / 60; //분 계산
                        int _s = (_timei % 3600) % 60; //초 계산
                        _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
                    }
                }
            }
        }
    }

    //드래그종료할때
    public void OnEndDrag(PointerEventData eventData)
    {
        TimeController.timeController._drag = false;
    }
}
