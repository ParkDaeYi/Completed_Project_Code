using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI")]
    public Text _speedometer; //속도계
    public bool _droneSightActive = true;
    public GameObject _droneSight; //드론 카메라
    public GameObject M; //M 글자
    public GameObject _keyM; //M UI
    public GameObject _resetButton; //초기화 버튼
    public RectTransform _resetButtonRectTransform;
    public Text _typeText; //타입 텍스트
    public Text _altitudeStandardText; //고도 텍스트
    public Transform _altitudeStandardTransform; //표준 고도

    [Header("Script")]
    public Drone _drone; //드론 스크립트

    private void Update()
    {
        DroneCameraButton();
    }
    private void FixedUpdate()
    {
        SpeedUpdate();
        TypeUpdate();
        AltitudeUpdate();
    }

    private void SpeedUpdate() //속도 갱신 함수
    {

        if (_drone._StartUp) //드론이 작동중이면
        {
            _speedometer.text = string.Format("{0:0.0}", System.Math.Round(_drone._Speedmeter, 1)) + " <color=#78EFAD>km/s</color>"; //속도 갱신
        }
        else //드론이 작동중이지 않으면
        {
            _speedometer.text = "";
        }
    }

    private void DroneCameraButton() //드론 카메라 버튼 입력 함수
    {
        if (Input.GetKeyDown(KeyCode.M)) //M키를 누르면
        {
            if (_droneSightActive) //드론 카메라 활성화 중이면
            {
                _droneSight.SetActive(false); //드론 카메라 비활성화
                M.SetActive(false); //M 이미지 비활성화
                _keyM.SetActive(true); //M UI 활성화
                _resetButtonRectTransform.anchoredPosition = new Vector2(_resetButtonRectTransform.anchoredPosition.x + 375f, _resetButtonRectTransform.anchoredPosition.y);
            }
            else //드론 카메라 비활성화 중이면
            {
                _droneSight.SetActive(true); //드론 카메라 활성화
                M.SetActive(true); //M 이미지 비활성화
                _keyM.SetActive(false); //M UI 비활성화
                _resetButtonRectTransform.anchoredPosition = new Vector2(_resetButtonRectTransform.anchoredPosition.x - 375f, _resetButtonRectTransform.anchoredPosition.y);

            }
            _droneSightActive = !_droneSightActive; //드론 카메라 활성화 반대로
        }
    }
    private void TypeUpdate() //타입 변경 UI
    {
        if (_drone._StartUp)
        {
            if (_drone._Type == 1)
            {
                _typeText.text = "Mode 1";
            }
            else if (_drone._Type == 2)
            {
                _typeText.text = "Mode 2";
            }
            else if (_drone._Type == 3)
            {
                _typeText.text = "Mode 3";
            }
        }
        else
        {
            _typeText.text = "";
        }
    }

    private void AltitudeUpdate() //고도 갱신
    {
        if (_drone._StartUp)
        {
            _altitudeStandardText.text = string.Format("{0:0.0}", System.Math.Round((_drone.transform.position.y - _altitudeStandardTransform.position.y) * (7.5f / 10f), 1)) + " <color=#78EFAD>m</color>";
        }
        else
        {
            _altitudeStandardText.text = "";
        }

    }

}
