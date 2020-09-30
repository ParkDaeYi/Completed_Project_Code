using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; //GraphicRaycaster 사용하기 위한 지시문
using Valve.VR;

public class VRJoystick : BaseInputModule //BaseInputModule 클래스 상속
{
    /*
* date 2020.04.26
* author GS
* desc
* VRCanvas을 조작하는 컨트롤러 다루는 스크립트
* 대부분 VRCanvas에 이벤트를 전달하는 역할을 한다
*/
    [Header("GraphicRaycaster")]
    public GraphicRaycaster _graphicRaycaster; //Canvas에 있는 GraphicRaycaster
    private List<RaycastResult> _raycastResults; //Raycast로 충돌한 UI들을 담는 리스트
    private PointerEventData _pointerEventData; //Canvas 상의 포인터 위치 및 정보
    public Camera _target; //마우스 커서 역할을 대신할 카메라

    [Header("Laser Pointer")]
    public LineRenderer _lineRenderer; //레이저 포인터 컴포넌트
    public SteamVR_Action_Boolean _clickPad; //컨트롤러의 패드를 클릭하는지 여부

    [Header("VRCamera Transition")]
    public Transform _VRCameraTransform; //VRCamera의 Transform 정보
    public SteamVR_Action_Vector2 _joystickVector2; //조이스틱의 위치
    public SteamVR_Action_Single _triggerVector1; //트리거의 위치
    public Vector3 _centerAxis; //중심축

    protected override void Start()
    {
        _pointerEventData = new PointerEventData(null); //pointerEventData 초기화
        _pointerEventData.position = new Vector2(_target.pixelWidth / 2, _target.pixelHeight / 2); //카메라의 중앙으로 포인터 지정
        _raycastResults = new List<RaycastResult>(); //리스트 초기화
    }

    protected void Update()
    {
        Raycaster();
        VRCameraTransition();
        LaserPointer();
    }

    public void Raycaster() //Raycaster 발생시키는 함수
    {
        _graphicRaycaster.Raycast(_pointerEventData, _raycastResults); //포인터 위치로부터 Raycast 발생, 결과는 raycastResults에 담긴다
        if (_raycastResults.Count > 0) //충돌한 UI가 있으면
        {
            foreach (RaycastResult raycastResult in _raycastResults) //충돌한 UI 탐색
            {
                HandlePointerExitAndEnter(_pointerEventData, raycastResult.gameObject); //호버링 이벤트 전달
                if (_clickPad.GetStateDown(SteamVR_Input_Sources.RightHand)) //컨트롤러의 오른쪽 패드를 클릭하면
                    ExecuteEvents.Execute(raycastResult.gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler); //해당 UI에 클릭 이벤트 전달
            }
        }
        else //충돌한 UI가 없으면
        {
            HandlePointerExitAndEnter(_pointerEventData, null); //포인터 벗어남 → GameObject가 null이어야 호버링에서 벗어남
        }
        _raycastResults.Clear(); //Raycast 결과 리스트 초기화 → 필수
    }

    public void LaserPointer() //레이저 포인터 만드는 함수
    {
        _lineRenderer.SetPosition(0, _target.GetComponent<Transform>().position); //시작 지점 지정
        _lineRenderer.SetPosition(1, _target.GetComponent<Transform>().position + _target.GetComponent<Transform>().forward * 100f); //끝 지점 지정
    }

    public void VRCameraTransition() //VRCamera 오브젝트의 위치를 변경하는 함수
    {
        float _triggerAxis = _triggerVector1.GetAxis(SteamVR_Input_Sources.RightHand);

        if (_triggerAxis > 0.6f)
        {
            float _rotateSpeed = 100f;
            float _rotateDirection = _joystickVector2.GetAxis(SteamVR_Input_Sources.RightHand).x;
            _VRCameraTransform.LookAt(_centerAxis); //중심 축을 바라봄
            _VRCameraTransform.Translate(_rotateDirection * Time.deltaTime * _rotateSpeed, 0f, 0f); //카메라 이동
        }
        else
        {
            _centerAxis = _VRCameraTransform.position + _VRCameraTransform.forward * 50f; //중심 축 지정

            float _moveSpeed = 100f;
            float _moveDirection = _joystickVector2.GetAxis(SteamVR_Input_Sources.RightHand).y;
            if (Mathf.Abs(_moveDirection) > 0.3f)
                _VRCameraTransform.position += _target.GetComponent<Transform>().forward * Time.deltaTime * _moveSpeed * _moveDirection;

            float _rotateSpeed = 100f;
            float _rotateDirection = _joystickVector2.GetAxis(SteamVR_Input_Sources.RightHand).x;
            if (Mathf.Abs(_rotateDirection) > 0.3f)
                _VRCameraTransform.Rotate(0f, _rotateSpeed * _rotateDirection * Time.deltaTime, 0f);
        }
    }

    public override void Process() { } //상속받아야 에러 안뜸
    protected override void OnEnable() { } //상속받아야 에러 안뜸
    protected override void OnDisable() { } //상속받아야 에러 안뜸
}