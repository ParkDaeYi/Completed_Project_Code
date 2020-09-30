using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    /*
* date 2019.11.25
* author GS
* desc
* Main Camera Panel을 관리하는 스크립트.
*/

    [Header("Script")]
    public CameraMoveAroun _cameraMoveAroun; //카메라 스크립트

    [Header("UI")]
    public GameObject _basePanel; //기반 Panel

    public Slider _moveSensitiveSlider; //이동 민감도 Slider
    public Slider _rotateSensitiveSlider; //회전 민감도 Slider
    public Transform _cameraButton; //Camera 버튼 Transform

    public Text _moveSensitiveMinText; //이동 민감도 최소
    public Text _moveSensitiveMiddleText; //이동 민감도 중간
    public Text _moveSensitiveMaxText; //이동 민감도 최대

    public Text _rotateSensitiveMinText; //회전 민감도 최소
    public Text _rotateSensitiveMiddleText; //회전 민감도 중간
    public Text _rotateSensitiveMaxText; //회전 민감도 최대

    public void Start()
    {
        _moveSensitiveMinText.text = (int)_moveSensitiveSlider.minValue + "";
        _moveSensitiveMiddleText.text = (int)((_moveSensitiveSlider.minValue + _moveSensitiveSlider.maxValue) * 0.5f) + "";
        _moveSensitiveMaxText.text = (int)_moveSensitiveSlider.maxValue + "";

        _rotateSensitiveMinText.text = (int)_rotateSensitiveSlider.minValue + "";
        _rotateSensitiveMiddleText.text = (int)((_rotateSensitiveSlider.minValue + _rotateSensitiveSlider.maxValue) * 0.5f) + "";
        _rotateSensitiveMaxText.text = (int)_rotateSensitiveSlider.maxValue + "";
    }

    public void OnClickCameraButton() //카메라 버튼을 클릭하면 실행되는 함수
    {
        _basePanel.SetActive(!_basePanel.activeSelf); //활성화 / 비활성화
        if (_basePanel.activeSelf) //활성화되면
        {
            _moveSensitiveSlider.value = _cameraMoveAroun._mouseSensitivity; //이동 민감도 초기화
            _rotateSensitiveSlider.value = _cameraMoveAroun._mouseRotateSpeed; //회전 민감도 초기화
        }
    }

    public void OnClickBasePanelCancel() //기반 Panel의 나가기 버튼을 클릭했을 때 실행되는 함수
    {
        _basePanel.SetActive(false); //기반 Panel 비활성화
    }

    public void OnMoveSensitiveSlider() //이동 민감도 슬라이더의 값이 변경되면 실행되는 함수
    {
        _cameraMoveAroun._mouseSensitivity = _moveSensitiveSlider.value; //민감도 변경
    }

    public void OnRotateSensitiveSlider() //회전 민감도 슬라이더의 값이 변경되면 실행되는 함수
    {
        _cameraMoveAroun._mouseRotateSpeed = _rotateSensitiveSlider.value; //민감도 변경
    }

    public void OnDownCameraButton() //카메라 버튼이 눌렸을 떄 실행되는 함수
    {
        _cameraButton = EventSystem.current.currentSelectedGameObject.transform;
        StartCoroutine(MoveCameraButton()); //코루틴 실행
    }

    public IEnumerator MoveCameraButton() //카메라 버튼 이동하는 코루틴
    {
        while (Input.GetMouseButton(0))
        {
            _cameraButton.position = Input.mousePosition; //Camera 버튼의 위치를 마우스 위치로 변경
            yield return new WaitForEndOfFrame();
        }
    }
}