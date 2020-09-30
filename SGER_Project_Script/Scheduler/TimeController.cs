using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * content : 시간을 관리하는 스크립트
 *  -> 현재 발생하는 오류중에서 실행후 화면의 크기를 변화시키면 시간의 값이 맞지 않는 오류가 있음
 *  -> 화면 크기를 늘릴때 현재 시간을 변화시킨다 (1안)
 *  -> 화면 크기를 늘릴때 현재 시간에 맞게 커서를 움직인다 (2안)
 */


public class TimeController : MonoBehaviour
{

    public static TimeController timeController;

    [Header("시간 변수")]
    public double _time;
    int _h, _m, _s;
    public bool _sw;
    public bool _drag = false;

    [Header("스케줄러 시간텍스트")]
    public Text _timeText; // 시간 텍스트
    [Header("시간당 커서 이동")]
    public GameObject _cursor;
    public GameObject _minLine;
    public GameObject _maxLine;
    float _distance; //커서가 움직일수 있는 거리
    public float _maxMoveTime; //최대 이동 시간(초 단위)
    public float _repeatFinishTime;
    public float _repeatStartTime;
    public bool _repeatCal;//구간반복시 위치계산 ON하는 불
    public bool _reset = false;
    public bool _firstTime = false;

    // Use this for initialization
    void Start()
    {
        TimeController.timeController = this;
        _sw = false;
        _h = _m = _s = 0;
        _time = 0;

        //커서관련
        _maxMoveTime = 210; // 3분30초
        _distance = (_maxLine.transform.position.x - _minLine.transform.position.x) / _maxMoveTime; // 커서의 시간에 따른 이동 거리
    }

    // Update is called once per frame
    void Update()
    {
        if (_sw)
        {
            if (!Static.STATIC._repeat)
            {
                if (_cursor.transform.position.x >= _maxLine.transform.position.x)
                {
                    _cursor.transform.position = new Vector3(_maxLine.transform.position.x, _cursor.transform.position.y, _cursor.transform.position.z);
                    _timeText.text = "03:30.00";
                }
                else
                {
                    _time += Time.deltaTime; //지나간 시간계산
                    string _timestr;
                    _timestr = _time.ToString("00.00"); //00.00형식으로 변환해서 저장
                    string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                    int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                    _h = _timei / 3600; //시 계산
                    _m = (_timei % 3600) / 60; //분 계산
                    _s = (_timei % 3600) % 60; //초 계산

                    _cursor.transform.Translate(new Vector3(Time.deltaTime * _distance, 0, 0)); //시간이 움직일때 초당 움직임
                    _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
                }
            }
            else
            {
                if (_cursor.transform.position.x >= Static.STATIC._repeatright.transform.position.x)
                {
                    Debug.Log("Cursor < repeatLeft");
                    Debug.Log(Static.STATIC._repeatHuman.Count);
                    _cursor.transform.position = new Vector3(Static.STATIC._repeatleft.transform.localPosition.x - 1, _cursor.transform.position.y, _cursor.transform.position.z);
                }
                else if (_cursor.transform.position.x < Static.STATIC._repeatleft.transform.position.x)
                {
                    _cursor.transform.position = new Vector3(Static.STATIC._repeatleft.transform.position.x + 0.1f, _cursor.transform.position.y, _cursor.transform.position.z);
                    _repeatStartTime = (Static.STATIC._repeatleft.transform.localPosition.x - _minLine.transform.localPosition.x) * 210 / 1785f;
                    _time = _repeatStartTime;
                    string _timestr;
                    _timestr = _repeatStartTime.ToString("00.00"); //00.00형식으로 변환해서 저장
                    string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                    int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                    _m = (_timei % 3600) / 60; //분 계산
                    _s = (_timei % 3600) % 60; //초 계산
                    string _StartStr = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1];

                    _timeText.text = _StartStr;
                    int i = 0;
                    foreach (GameObject go in Static.STATIC._repeatHuman)
                    {
                        Debug.Log(Static.STATIC._repeatHumanRotate[i]);
                        go.transform.position = (Vector3)Static.STATIC._repeatHumanPos[i];
                        if ((int)Static.STATIC._repeatState[i] == 1) go.transform.GetChild(0).transform.localRotation = Quaternion.Euler((Vector3)Static.STATIC._repeatHumanRotate[i++]);
                        else
                        {
                            Vector3 v = (Vector3)Static.STATIC._repeatHumanRotate[i++];
                            Debug.Log(v);
                            go.transform.GetChild(0).transform.LookAt(v);
                        }
                    }
                }
                else
                {
                    _time += Time.deltaTime; //지나간 시간계산
                    string _timestr;
                    _timestr = _time.ToString("00.00"); //00.00형식으로 변환해서 저장
                    string[] _timearr = _timestr.Split('.'); // '.'을 기준으로 문자열 나눔
                    int _timei = System.Convert.ToInt32(_timearr[0]); // 1초이상의 숫자를 int로 변환
                    _h = _timei / 3600; //시 계산
                    _m = (_timei % 3600) / 60; //분 계산
                    _s = (_timei % 3600) % 60; //초 계산

                    _cursor.transform.Translate(new Vector3(Time.deltaTime * _distance, 0, 0)); //시간이 움직일때 초당 움직임
                    _timeText.text = _m.ToString("00") + ":" + _s.ToString("00") + "." + _timearr[1]; //시간 텍스트 변경
                }
            }
        }
    }
}

