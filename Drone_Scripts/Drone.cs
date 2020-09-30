using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 1. 드론 이동 구현함(속도,호버링,로터회전)
 2. 한 방향으로 이동하다가 멈췄을 경우 반대 방향으로 힘을 주게 보이게끔 구현
 3. 대각으로 이동할 경우 반대 방향으로 힘을 주게 보이게끔 구현 도중 예외가 많아서
    몇 가지 함수 및 소스 추가
 4. 대각 이동 도중 멈췄을 때 반대 모션 구현 완료

해야할 일:  제자리 회전을 하는데 이 때에도 드론이 기울여져야 하므로 그에 맞게끔 구현 예정
            velocity가 절대좌표여서 제자리 회전 이후 이동이 현 포지션에 맞게 움직이도록 구현 예정
            플립 구현 예정(어려울 듯)
            드론 하강 중 대략 바닥과 2m 정도 남았을 때 하강 속도를 조절해줘야함
            >>Raycast?
            그리고 부딪혔을 때 드론이 떨어지는 모션이랑 그와 동시에 바닥으로 꺼지는 현상이 없도록
            etc.
 */

public class Drone : MonoBehaviour
{
    public static Drone _Drone;

    [Header("Drone")]
    public Rigidbody _droneModel;
    public Transform[] CW = new Transform[2];
    public Transform[] CCW = new Transform[2];

    public float _effectiveHeight;

    public Vector3 _velocity = Vector3.zero;

    private float _percent;
    public float _XTemp;
    public float _ZTemp;
    public float _XTemp2;
    public float _ZTemp2;
    public float _Q;

    [Header("DroneForce")]
    public float _ForwardForce;
    public float _LeftForce;
    public float _YForce;
    public float _W, _D;

    [Header("DroneSpeed")]
    public static float maxSpeed = 40f;//60f;
    public static float minSpeed = -40f;//-60f;
    public static float upMax = 13f;//16f;
    public static float downMax = -9.5f;//-11f;

    [Header("RotorRotateTime")]
    //FBDrag()함수에서 사용
    private float _XRotateTime_A;
    private float _XRotateTime_D;
    private float _XMaxVel;
    private float _XMinVel_A;
    private float _XMinVel_D;
    private float _XZeroTime_A;
    private float _XZeroTime_D;
    private float _XMaxZero;
    private float _XMinZero_A;
    private float _XMinZero_D;
    //LRDrag() 함수에서 사용
    private float _ZRotateTime_W;
    private float _ZRotateTime_S;
    private float _ZZeroTime_W;
    private float _ZZeroTime_S;
    private float _ZMaxVel;
    private float _ZMinVel_W;
    private float _ZMinVel_S;
    private float _ZMaxZero;
    private float _ZMinZero_W;
    private float _ZMinZero_S;
    //AllDrag에 쓰일 변수들(그냥 새로 해줌)
    private float _ART;
    private float _AZT;

    [Header("RotorTorque")]
    public float _Rotor_1;
    public float _Rotor_2;
    public float _Rotor_3;
    public float _Rotor_4;
    public float _MinRotor;
    public float _MaxRotor;

    [Header("MotionTirgger")]
    public bool _SingleMotion;
    public bool _DoubleMotion;
    public bool _StartUp;
    public bool _collide = false; //충돌 반정

    [Header("Speed")]
    public float _Speedmeter;

    [Header("Mode")]
    public int _Type;

    [Header("Static")]
    public static float _x;
    public static float _y;
    public static float _z;

    [Header("Script")]
    public AudioController _audioController; //오디오 컨트롤러

    [Header("Input")]
    public bool _wKey;
    public bool _sKey;
    public bool _aKey;
    public bool _dKey;
    public bool _upKey;
    public bool _downKey;
    public bool _leftKey;
    public bool _rightKey;
    public bool _cKey;
    public bool _vKey;
    public bool _zKey;

    private float _startUpDelay; //시동 걸리는 딜레이

    /* 충돌체 판정이 있을 때 반응하는 함수 */
    private void OnCollisionEnter(Collision _col)
    {
        _audioController.CollideEnterSound(); //사운드 실행
        if (_Speedmeter > 10.0f)
        {
            _StartUp = false; //드론 시동 비활성화
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _collide = true; //충돌 중
    }

    private void OnCollisionExit(Collision collision)
    {
        _collide = false; //충돌 안함
    }

    private void Start()
    {
        //Rotor1 우측상단 ,Rotor2 우측하단, Rotor3 좌측하단, Rotor4 좌측상단
        //CW 시계방향
        CW[0] = transform.Find("Rotor2");
        CW[1] = transform.Find("Rotor4");

        //CCW 반시계방향
        CCW[0] = transform.Find("Rotor1");
        CCW[1] = transform.Find("Rotor3");

        //드론이 최대 날 수 있는 고도
        //일단 임의 값
        _effectiveHeight = 100f;

        _ForwardForce = 0.0f;
        _LeftForce = 0.0f;

        _XRotateTime_A = 0.0f;
        _XRotateTime_D = 0.0f;
        _XMaxVel = 0.0f;
        _XMinVel_A = 0.0f;
        _XMinVel_D = 0.0f;
        _XZeroTime_A = 0.0f;
        _XZeroTime_D = 0.0f;
        _XMaxZero = 0.0f;
        _XMinZero_A = 0.0f;
        _XMinZero_D = 0.0f;

        _ZRotateTime_W = 0.0f;
        _ZRotateTime_S = 0.0f;
        _ZMaxVel = 0.0f;
        _ZMinVel_W = 0.0f;
        _ZMinVel_S = 0.0f;
        _ZZeroTime_W = 0.0f;
        _ZZeroTime_S = 0.0f;
        _ZMaxZero = 0.0f;
        _ZMinZero_W = 0.0f;
        _ZMinZero_S = 0.0f;

        //velocity는 가속도임.
        _velocity = _droneModel.velocity;

        _MinRotor = 60f;
        _MaxRotor = 100f;

        _SingleMotion = true;
        _DoubleMotion = false;

        //속도계
        _Speedmeter = 0.0f;

        _Q = 0.0f;

        _Type = 1;

        maxSpeed = 40f;
        minSpeed = -40f;
        upMax = 13f;
        downMax = -9.5f;
    }

    private void Update()
    {
        _cKey = Input.GetKey(KeyCode.C) || OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        _vKey = Input.GetKey(KeyCode.V) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
        _zKey = Input.GetKeyDown(KeyCode.Z) || OVRInput.GetDown(OVRInput.Button.Two);

        _startUpDelay += Time.deltaTime; //시동 딜레이 계산

        if (_startUpDelay > 1f)
        {
            if (_cKey && _vKey)
            {
                _StartUp = !_StartUp;
                _startUpDelay = 0f; //딜레이 초기화
            }
        }

        if (_StartUp)
        {
            if (_zKey)
            {
                if (_Type == 3)
                {
                    _Type = 0;
                }
                _Type += 1;

                /*
                _Type == 1 은 max min 속도 40 -40 그리고 AllDrag 적용
                _Type == 2 는 max min 속도 20 -20 그리고 AllDrag 적용
                _Type == 3 은 max min 속도 40 -40 AllDrag는 적용 x
                */
            }
        }
    }

    void FixedUpdate()
    {
        _wKey = Input.GetKey(KeyCode.W) || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0.4f;
        _sKey = Input.GetKey(KeyCode.S) || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < -0.4f;
        _aKey = Input.GetKey(KeyCode.A) || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.4f;
        _dKey = Input.GetKey(KeyCode.D) || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.4f;
        _upKey = Input.GetKey(KeyCode.UpArrow) || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.4f;
        _downKey = Input.GetKey(KeyCode.DownArrow) || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.4f;
        _leftKey = Input.GetKey(KeyCode.LeftArrow) || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.4f;
        _rightKey = Input.GetKey(KeyCode.RightArrow) || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.4f;

        _x = _droneModel.transform.eulerAngles.x;
        _y = _YForce;
        _z = _droneModel.transform.eulerAngles.z;

        //속도계 만들어두긴함 아마 이거 쓸거 같음
        _Speedmeter = Mathf.Clamp(
            Mathf.Max(Mathf.Abs(_W) + Mathf.Abs(_velocity.y), Mathf.Abs(_velocity.y), Mathf.Abs(_D) + Mathf.Abs(_velocity.y)), 0f, 66f);

        if (_StartUp)
        {
            if (_Type == 1)
            {
                _MaxRotor = 100f;
                maxSpeed = 40f;
                minSpeed = -40f;
                upMax = 13f;
                downMax = -9.5f;

                MoveButton();
            }
            else if (_Type == 2)
            {
                _MaxRotor = 88f;
                maxSpeed = 20f;
                minSpeed = -20f;
                upMax = 10f;
                downMax = -6f;

                MoveButton();
            }
            else if (_Type == 3)
            {
                _MaxRotor = 100f;
                maxSpeed = 40f;
                minSpeed = -40f;
                upMax = 13f;
                downMax = -9.5f;

                MoveButton_3();
            }

            /*지정된 방향키(이동관련) 눌렀을 경우 로터 값 수시로 변경
            -> 로터 값으로 기울기 구현해서 로터 값 변경해줘야함*/
            RotorTorque();

            //_droneModel.velocity에 값을 바로 넣어주려 했으나 안되서 _velocity라는 변수 값을 넣어줌
            _droneModel.velocity = _velocity;
            LimitForce();

        }
        else if (!_StartUp)
        {
            _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
            _W = 0.0f; _D = 0.0f;

            _velocity.x = 0.0f;
            _velocity.z = 0.0f;
        }
    }

    void FBValueClear()
    {
        //FBDrag() 에서 쓰는 변수
        _XRotateTime_A = 0f;
        _XRotateTime_D = 0f;
        _XZeroTime_A = 0f;
        _XZeroTime_D = 0f;
        _XMinVel_A = 0f;
        _XMinVel_D = 0f;
        _XMinZero_A = 0f;
        _XMinZero_D = 0f;
        _XMaxVel = 0f;
        _XMaxZero = 0f;
    }

    void LRValueClear()
    {
        //LRDrag() 에서 쓰는 변수
        _ZRotateTime_W = 0f;
        _ZRotateTime_S = 0f;
        _ZZeroTime_W = 0f;
        _ZZeroTime_S = 0f;
        _ZMinVel_W = 0f;
        _ZMinVel_S = 0f;
        _ZMinZero_W = 0f;
        _ZMinZero_S = 0f;
        _ZMaxVel = 0f;
        _ZMaxZero = 0f;
    }

    void MoveButton_3()
    {
        if (!_leftKey && !_rightKey)
        {
            SingleForce();

            if ((_wKey || _sKey) && (_aKey || _dKey))
            {
                //이동 키 두개 누르고 있으므로 _DoubleMoion=true
                _SingleMotion = false;
                _DoubleMotion = true;

                //로터 값에 따라 기울기 변경
                if (_upKey)
                {//DJI에서 이동 중 Space(상승)를 누르면 기울기가 변해서 추가
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * 0.5f), Time.fixedDeltaTime * 2.5f);
                }

                else if (!_upKey)
                {
                    //하강 중엔 기울기에 영향을 안줘서 원래 수치를 넣어둠
                    //*0.75f는 값이 max값에 인접해 졌을 경우 기울기가 조금 올라감? DJI에서 그랬음

                    if (Mathf.Abs(_W) >= maxSpeed - 1f && Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_W) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }
                }

            }
            else if ((_wKey || _sKey) || (_aKey || _dKey))
            {
                //이동 키 두개 누르고 있으므로 _DoubleMoion=true
                _SingleMotion = true;
                _DoubleMotion = false;

                //로터 값에 따라 기울기 변경
                if (_upKey)
                {//DJI에서 이동 중 Space(상승)를 누르면 기울기가 변해서 추가
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * 0.5f), Time.fixedDeltaTime * 2.5f);
                }

                else if (!_upKey)
                {
                    //하강 중엔 기울기에 영향을 안줘서 원래 수치를 넣어둠
                    //*0.75f는 값이 max값에 인접해 졌을 경우 기울기가 조금 올라감? DJI에서 그랬음

                    if (Mathf.Abs(_W) >= maxSpeed - 1f && Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_W) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }
                }
            }
            else
            {
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.0f), Time.fixedDeltaTime * 2.5f);
            }
        }

        else if (_leftKey || _rightKey)
        {

            //_Q는 AllDrag() 함수에서 가속도 줄여주는 비율..?
            _Q = 0.8f;

            SingleForce();

            if ((_wKey || _sKey) && (_aKey || _dKey))
            {
                _SingleMotion = false;
                _DoubleMotion = true;

                if (_W < 1f && _W > -1f && _D < 1f && _D > -1f)
                {

                    _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
                    _W = 0.0f; _D = 0.0f;

                    _velocity.x = 0.0f;
                    _velocity.z = 0.0f;

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);

                    if ((Mathf.Abs(_droneModel.transform.eulerAngles.x) >= Mathf.Abs(_ForwardForce * 0.3f)) || (Mathf.Abs(_droneModel.transform.eulerAngles.z) >= Mathf.Abs(_LeftForce * 0.3f)))
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);
                    }

                }

            }
            else if (_wKey || _sKey || _aKey || _dKey)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                if (_W < 1f && _W > -1f && _D < 1f && _D > -1f)
                {

                    _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
                    _W = 0.0f; _D = 0.0f;

                    _velocity.x = 0.0f;
                    _velocity.z = 0.0f;

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);

                    if ((Mathf.Abs(_droneModel.transform.eulerAngles.x) >= Mathf.Abs(_ForwardForce * 0.3f)) || (Mathf.Abs(_droneModel.transform.eulerAngles.z) >= Mathf.Abs(_LeftForce * 0.3f)))
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);
                    }

                }

            }
            else
            {
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.0f), Time.fixedDeltaTime * 2.5f);
            }

        }
    }

    void MoveButton()
    {

        if (!_leftKey && !_rightKey)
        {

            SingleForce();

            if ((_wKey || _sKey) && (_aKey || _dKey))
            {
                //이동 키 두개 누르고 있으므로 _DoubleMoion=true
                _SingleMotion = false;
                _DoubleMotion = true;

                FBValueClear();

                LRValueClear();

                //AllDrag() 에서 쓰는 변수
                _ART = 0f;
                _AZT = 0f;

                //로터 값에 따라 기울기 변경
                if (_upKey)
                {//DJI에서 이동 중 Space(상승)를 누르면 기울기가 변해서 추가
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * 0.5f), Time.fixedDeltaTime * 2.5f);
                }

                else if (!_upKey)
                {
                    //하강 중엔 기울기에 영향을 안줘서 원래 수치를 넣어둠
                    //*0.75f는 값이 max값에 인접해 졌을 경우 기울기가 조금 올라감? DJI에서 그랬음

                    if (Mathf.Abs(_W) >= maxSpeed - 1f && Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_W) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }

                    else if (Mathf.Abs(_D) >= maxSpeed - 1f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.75f), Time.fixedDeltaTime * 2.5f);
                    }

                    else
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);
                    }
                }

            }

            else if ((!_aKey && !_dKey) && ((_wKey || _sKey)))
            {
                FBValueClear();

                //AllDrag() 에서 쓰는 변수
                _ART = 0f;
                _AZT = 0f;

                //LRDrag는 a와 d를 누르지 않았을 때 드론이 좌우측 힘을 역으로 주게 해줌
                LRDrag();

            }

            else if ((!_wKey && !_sKey) && ((_aKey || _dKey)))
            {
                LRValueClear();

                //AllDrag() 에서 쓰는 변수
                _ART = 0f;
                _AZT = 0f;

                //FBDrag는 w와 s를 누르지 않았을 때 드론이 좌우측 힘을 역으로 주게 해줌
                FBDrag();

            }

            else if ((!_wKey && !_sKey && !_aKey && !_dKey) || (_upKey || _downKey))
            {
                FBValueClear();

                LRValueClear();

                /*AllDrag함수는 wasd를 누르지 않았을 때 좌우앞뒤에 남아있는 속도를 줄여줘야하기 때문에
                 대각선으로 기울여 줘야함*/
                AllDrag();
            }
        }

        else if (_leftKey || _rightKey)
        {
            FBValueClear();

            LRValueClear();


            //_Q는 AllDrag() 함수에서 가속도 줄여주는 비율..?
            //_Q = 0.8f;

            SingleForce();

            if ((_wKey || _sKey) && (_aKey || _dKey))
            {
                _SingleMotion = false;
                _DoubleMotion = true;

                if (_W < 1f && _W > -1f && _D < 1f && _D > -1f)
                {

                    _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
                    _W = 0.0f; _D = 0.0f;

                    _velocity.x = 0.0f;
                    _velocity.z = 0.0f;

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);

                    if ((Mathf.Abs(_droneModel.transform.eulerAngles.x) >= Mathf.Abs(_ForwardForce * 0.3f)) || (Mathf.Abs(_droneModel.transform.eulerAngles.z) >= Mathf.Abs(_LeftForce * 0.3f)))
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);
                    }

                }
                else
                {
                    AllDrag_Q();
                }
                _ART = 0f;
                _AZT = 0f;
            }
            else if (_wKey || _sKey || _aKey || _dKey)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                if (_W < 1f && _W > -1f && _D < 1f && _D > -1f)
                {

                    _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
                    _W = 0.0f; _D = 0.0f;

                    _velocity.x = 0.0f;
                    _velocity.z = 0.0f;

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);

                    if ((Mathf.Abs(_droneModel.transform.eulerAngles.x) >= Mathf.Abs(_ForwardForce * 0.3f)) || (Mathf.Abs(_droneModel.transform.eulerAngles.z) >= Mathf.Abs(_LeftForce * 0.3f)))
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.3f, _YForce, _LeftForce * 0.3f), Time.fixedDeltaTime * 2f);
                    }

                }
                else
                {
                    AllDrag_Q();
                }

                _ART = 0f;
                _AZT = 0f;
            }
            if (!_wKey || !_sKey || !_aKey || !_dKey)
            {

                AllDrag();

            }

        }
    }

    //w와 s를 누르지 않았을 때 적용되는 관성? 반대 방향으로 힘을 주는 모션
    void FBDrag()
    {
        if (_W < -1.0f)
        {//뒤에 힘이 남아있을 때
            _XZeroTime_A = 0f;
            _XZeroTime_D = 0f;
            _XMinZero_A = 0f;
            _XMinZero_D = 0f;
            _XMaxZero = 0f;

            if (_aKey)
            {
                _Rotor_2 = 100f;
                _Rotor_3 = 60f;
            }
            else if (_dKey)
            {
                _Rotor_2 = 60f;
                _Rotor_3 = 100f;
            }

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _W = _ZTemp * -1f + _XTemp * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _W = _ZTemp - _XTemp;
            }

            if (_upKey)
            {
                _XMaxVel = 0.0f;
                _XMinVel_A = 0.0f;
                _XMinVel_D = 0.0f;
                if (_aKey)
                {
                    _XRotateTime_D = 0.0f;
                    _XRotateTime_A = Mathf.Clamp(_XRotateTime_A + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.5f), _XRotateTime_A);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * 0.5f), _XRotateTime_A);
                    }
                }
                else if (_dKey)
                {
                    _XRotateTime_A = 0.0f;
                    _XRotateTime_D = Mathf.Clamp(_XRotateTime_D + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.5f), _XRotateTime_D);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * 0.5f), _XRotateTime_D);
                    }
                }
            }
            else if (Mathf.Abs(_D) >= maxSpeed - 1f)
            {
                _XRotateTime_A = 0.0f;
                _XRotateTime_D = 0.0f;
                _XMinVel_A = 0.0f;
                _XMinVel_D = 0.0f;
                _XMaxVel = Mathf.Clamp(_XMaxVel + Time.fixedDeltaTime, 0, 1);

                if (_ForwardForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, (_LeftForce * 0.75f)), _XMaxVel);
                }
                else if (_ForwardForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, (_LeftForce * 0.75f)), _XMaxVel);
                }

            }
            else if (Mathf.Abs(_D) < maxSpeed - 1f)
            {
                _XRotateTime_A = 0.0f;
                _XRotateTime_D = 0.0f;
                _XMaxVel = 0.0f;
                if (_aKey)
                {
                    _XMinVel_D = 0.0f;
                    _XMinVel_A = Mathf.Clamp(_XMinVel_A + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _XMinVel_A);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _XMinVel_A);
                    }
                }
                else if (_dKey)
                {
                    _XMinVel_A = 0.0f;
                    _XMinVel_D = Mathf.Clamp(_XMinVel_D + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _XMinVel_D);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _XMinVel_D);
                    }
                }
            }
        }
        else if (_W > 1.0f)
        {//앞에 힘이 남아있을 때
            _XZeroTime_A = 0f;
            _XZeroTime_D = 0f;
            _XMinZero_A = 0f;
            _XMinZero_D = 0f;
            _XMaxZero = 0f;

            if (_aKey)
            {
                _Rotor_1 = 100f;
                _Rotor_4 = 60f;
            }
            else if (_dKey)
            {
                _Rotor_1 = 60f;
                _Rotor_4 = 100f;
            }

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _W = _ZTemp * -1f + _XTemp * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _W = _ZTemp - _XTemp;
            }

            if (_upKey)
            {
                _XMaxVel = 0.0f;
                _XMinVel_A = 0.0f;
                _XMinVel_D = 0.0f;

                if (_aKey)
                {
                    _XRotateTime_D = 0.0f;
                    _XRotateTime_A = Mathf.Clamp(_XRotateTime_A + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * 0.5f), _XRotateTime_A);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.5f), _XRotateTime_A);
                    }
                }
                else if (_dKey)
                {
                    _XRotateTime_A = 0.0f;
                    _XRotateTime_D = Mathf.Clamp(_XRotateTime_D + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * 0.5f), _XRotateTime_D);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.5f), _XRotateTime_D);
                    }
                }
            }
            else if (Mathf.Abs(_D) >= maxSpeed - 1f)
            {
                _XRotateTime_A = 0.0f;
                _XRotateTime_D = 0.0f;
                _XMinVel_A = 0.0f;
                _XMinVel_D = 0.0f;
                _XMaxVel = Mathf.Clamp(_XMaxVel + Time.fixedDeltaTime, 0, 1);

                if (_ForwardForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * 0.75f), _XMaxVel);
                }
                else if (_ForwardForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.75f), _XMaxVel);
                }
            }
            else if (Mathf.Abs(_D) < maxSpeed - 1f)
            {
                _XRotateTime_A = 0.0f;
                _XRotateTime_D = 0.0f;
                _XMaxVel = 0.0f;

                if (_aKey)
                {
                    _XMinVel_D = 0.0f;
                    _XMinVel_A = Mathf.Clamp(_XMinVel_A + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _XMinVel_A);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _XMinVel_A);
                    }
                }
                else if (_dKey)
                {
                    _XMinVel_A = 0.0f;
                    _XMinVel_D = Mathf.Clamp(_XMinVel_D + Time.fixedDeltaTime, 0, 1);

                    if (_ForwardForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _XMinVel_D);
                    }
                    else if (_ForwardForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _XMinVel_D);
                    }
                }
            }
        }
        else
        {//_velocity.z 값이 -1과 1 사이에 있을 때
            if (_XTemp != 0.0f || _ZTemp != 0.0f)
            {
                if (_W > 0f)
                {
                    if (_YForce >= 0 && _YForce < 90)
                    {
                        _velocity.x -= _XTemp;
                        _velocity.z -= _ZTemp;
                    }
                    else if (_YForce >= 90 && _YForce < 180)
                    {
                        _velocity.x -= _XTemp;
                        _velocity.z += _ZTemp;
                    }
                    else if (_YForce >= 180 && _YForce < 270)
                    {
                        _velocity.x += _XTemp;
                        _velocity.z += _ZTemp;
                    }
                    else if (_YForce >= 270 && _YForce < 360)
                    {
                        _velocity.x += _XTemp;
                        _velocity.z -= _ZTemp;

                    }
                }
                else if (_W < 0f)
                {
                    if (_YForce >= 0 && _YForce < 90)
                    {
                        _velocity.x += _XTemp;
                        _velocity.z += _ZTemp;
                    }
                    else if (_YForce >= 90 && _YForce < 180)
                    {
                        _velocity.x += _XTemp;
                        _velocity.z -= _ZTemp;
                    }
                    else if (_YForce >= 180 && _YForce < 270)
                    {
                        _velocity.x -= _XTemp;
                        _velocity.z -= _ZTemp;
                    }
                    else if (_YForce >= 270 && _YForce < 360)
                    {
                        _velocity.x -= _XTemp;
                        _velocity.z += _ZTemp;

                    }
                }
                _W = 0.0f;
                _XTemp = 0.0f; _ZTemp = 0.0f;
                _SingleMotion = true;
                _DoubleMotion = false;
            }

            if (_upKey)
            {
                _XMaxZero = 0.0f;
                _XMinZero_A = 0.0f;
                _XMinZero_D = 0.0f;

                if (_aKey)
                {
                    _XZeroTime_D = 0.0f;
                    _XZeroTime_A = Mathf.Clamp(_XZeroTime_A + Time.fixedDeltaTime, 0, 1);
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.5f), _XZeroTime_A);

                }
                else if (_dKey)
                {
                    _XZeroTime_A = 0.0f;
                    _XZeroTime_D = Mathf.Clamp(_XZeroTime_D + Time.fixedDeltaTime, 0, 1);
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.5f), _XZeroTime_D);

                }

            }

            else if (Mathf.Abs(_D) >= maxSpeed - 1f)
            {
                _XZeroTime_A = 0.0f;
                _XZeroTime_D = 0.0f;
                _XMinZero_A = 0.0f;
                _XMinZero_D = 0.0f;
                _XMaxZero = Mathf.Clamp(_XMaxZero + Time.fixedDeltaTime, 0, 1);
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.75f), _XMaxZero);
            }

            else if (Mathf.Abs(_D) < maxSpeed - 1f)
            {
                _XZeroTime_A = 0.0f;
                _XZeroTime_D = 0.0f;
                _XMaxZero = 0.0f;

                if (_aKey)
                {
                    _XMinZero_D = 0.0f;
                    _XMinZero_A = Mathf.Clamp(_XMinZero_A + Time.fixedDeltaTime, 0, 1);
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce), _XMinZero_A);

                }
                else if (_dKey)
                {
                    _XMinZero_A = 0.0f;
                    _XMinZero_D = Mathf.Clamp(_XMinZero_D + Time.fixedDeltaTime, 0, 1);
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce), _XMinZero_D);

                }

            }

            if (_XZeroTime_A == 1 || _XMaxZero == 1 || _XMinZero_A == 1)
            {
                FBValueClear();

            }
        }

    }

    //a와 d를 누르지 않았을 때 관성/ 반대 방향으로 힘을 주는 모션
    void LRDrag()
    {
        if (_D < -1.0f)
        {
            _ZZeroTime_W = 0f;
            _ZZeroTime_S = 0f;
            _ZMaxZero = 0f;
            _ZMinZero_W = 0f;
            _ZMinZero_S = 0f;

            if (_wKey)
            {
                _Rotor_3 = 100f;
                _Rotor_4 = 60f;
            }
            else if (_sKey)
            {
                _Rotor_3 = 60f;
                _Rotor_4 = 100f;
            }

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _D = _ZTemp2 * -1f + _XTemp2 * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _D = _ZTemp2 + _XTemp2;

            }

            if (_upKey)
            {
                _ZMaxVel = 0.0f;
                _ZMinVel_W = 0.0f;
                _ZMinVel_S = 0.0f;

                if (_wKey)
                {
                    _ZRotateTime_S = 0.0f;
                    _ZRotateTime_W = Mathf.Clamp(_ZRotateTime_W + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * -1f), _ZRotateTime_W);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce), _ZRotateTime_W);
                    }
                }
                else if (_sKey)
                {
                    _ZRotateTime_W = 0.0f;
                    _ZRotateTime_S = Mathf.Clamp(_ZRotateTime_S + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * -1f), _ZRotateTime_S);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce), _ZRotateTime_S);
                    }
                }

            }

            //하강 시 드론의 기울기는 같았음.
            else if (Mathf.Abs(_W) >= maxSpeed - 1f)
            {
                _ZRotateTime_W = 0.0f;
                _ZRotateTime_S = 0.0f;
                _ZMinVel_W = 0.0f;
                _ZMinVel_S = 0.0f;
                _ZMaxVel = Mathf.Clamp(_ZMaxVel + Time.fixedDeltaTime, 0, 1);

                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * -1f), _ZMaxVel);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce), _ZMaxVel);
                }
            }

            else if (Mathf.Abs(_W) < maxSpeed - 1f)
            {
                _ZRotateTime_W = 0.0f;
                _ZRotateTime_S = 0.0f;
                _ZMaxVel = 0.0f;

                if (_wKey)
                {
                    _ZMinVel_S = 0.0f;
                    _ZMinVel_W = Mathf.Clamp(_ZMinVel_W + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ZMinVel_W);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ZMinVel_W);
                    }
                }
                else if (_sKey)
                {
                    _ZMinVel_W = 0.0f;
                    _ZMinVel_S = Mathf.Clamp(_ZMinVel_S + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ZMinVel_S);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ZMinVel_S);
                    }
                }

            }
        }

        else if (_D > 1.0f)
        {
            _ZZeroTime_W = 0f;
            _ZZeroTime_S = 0f;
            _ZMaxZero = 0f;
            _ZMinZero_W = 0f;
            _ZMinZero_S = 0f;

            if (_wKey)
            {
                _Rotor_1 = 60f;
                _Rotor_2 = 100f;
            }
            else if (_sKey)
            {
                _Rotor_1 = 100f;
                _Rotor_2 = 60f;
            }

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _D = _ZTemp2 * -1f + _XTemp2 * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _D = _ZTemp2 + _XTemp2;

            }

            if (_upKey)
            {
                _ZMaxVel = 0.0f;
                _ZMinVel_W = 0.0f;
                _ZMinVel_S = 0.0f;

                if (_wKey)
                {
                    _ZRotateTime_S = 0.0f;
                    _ZRotateTime_W = Mathf.Clamp(_ZRotateTime_W + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce), _ZRotateTime_W);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * -1f), _ZRotateTime_W);
                    }
                }
                else if (_sKey)
                {
                    _ZRotateTime_W = 0.0f;
                    _ZRotateTime_S = Mathf.Clamp(_ZRotateTime_S + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce), _ZRotateTime_S);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * -1f), _ZRotateTime_S);
                    }
                }

            }
            else if (Mathf.Abs(_W) >= maxSpeed - 1f)
            {
                _ZRotateTime_W = 0.0f;
                _ZRotateTime_S = 0.0f;
                _ZMinVel_W = 0.0f;
                _ZMinVel_S = 0.0f;
                _ZMaxVel = Mathf.Clamp(_ZMaxVel + Time.fixedDeltaTime, 0, 1);

                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce), _ZMaxVel);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * -1f), _ZMaxVel);
                }
            }
            else if (Mathf.Abs(_W) < maxSpeed - 1f)
            {
                _ZRotateTime_W = 0.0f;
                _ZRotateTime_S = 0.0f;
                _ZMaxVel = 0.0f;

                if (_wKey)
                {
                    _ZMinVel_S = 0.0f;
                    _ZMinVel_W = Mathf.Clamp(_ZMinVel_W + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ZMinVel_W);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ZMinVel_W);
                    }
                }
                else if (_sKey)
                {
                    _ZMinVel_W = 0.0f;
                    _ZMinVel_S = Mathf.Clamp(_ZMinVel_S + Time.fixedDeltaTime, 0, 1);

                    if (_LeftForce > 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ZMinVel_S);
                    }
                    else if (_LeftForce < 0f)
                    {
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ZMinVel_S);
                    }
                }

            }
        }

        else
        {
            if (_XTemp2 != 0.0f || _ZTemp2 != 0.0f)
            {
                if (_D > 0f)
                {
                    if (_YForce >= 0 && _YForce < 90)
                    {
                        _velocity.x -= _XTemp2;
                        _velocity.z += _ZTemp2;
                    }
                    else if (_YForce >= 90 && _YForce < 180)
                    {
                        _velocity.x += _XTemp2;
                        _velocity.z += _ZTemp2;
                    }
                    else if (_YForce >= 180 && _YForce < 270)
                    {
                        _velocity.x += _XTemp2;
                        _velocity.z -= _ZTemp2;
                    }
                    else if (_YForce >= 270 && _YForce < 360)
                    {
                        _velocity.x -= _XTemp2;
                        _velocity.z -= _ZTemp2;

                    }
                }
                else if (_D < 0f)
                {
                    if (_YForce >= 0 && _YForce < 90)
                    {
                        _velocity.x += _XTemp2;
                        _velocity.z -= _ZTemp2;
                    }
                    else if (_YForce >= 90 && _YForce < 180)
                    {
                        _velocity.x -= _XTemp2;
                        _velocity.z -= _ZTemp2;
                    }
                    else if (_YForce >= 180 && _YForce < 270)
                    {
                        _velocity.x -= _XTemp2;
                        _velocity.z += _ZTemp2;
                    }
                    else if (_YForce >= 270 && _YForce < 360)
                    {
                        _velocity.x += _XTemp2;
                        _velocity.z += _ZTemp2;

                    }
                }
                _D = 0.0f;
                _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
                _SingleMotion = true;
                _DoubleMotion = false;
            }

            if (_upKey)
            {
                _ZMinZero_W = 0.0f;
                _ZMinZero_S = 0.0f;
                _ZMaxZero = 0.0f;

                if (_wKey)
                {
                    _ZZeroTime_S = 0.0f;
                    _ZZeroTime_W = Mathf.Clamp(_ZZeroTime_W + Time.fixedDeltaTime, 0, 1);

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * 0.0f), _ZZeroTime_W);

                }

                else if (_sKey)
                {
                    _ZZeroTime_W = 0.0f;
                    _ZZeroTime_S = Mathf.Clamp(_ZZeroTime_S + Time.fixedDeltaTime, 0, 1);

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.5f, _YForce, _LeftForce * 0.0f), _ZZeroTime_S);

                }

            }

            else if (Mathf.Abs(_W) >= maxSpeed - 1f)
            {
                _ZMinZero_W = 0.0f;
                _ZMinZero_S = 0.0f;
                _ZZeroTime_W = 0.0f;
                _ZZeroTime_S = 0.0f;

                _ZMaxZero = Mathf.Clamp(_ZMaxZero + Time.fixedDeltaTime, 0, 1);
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.75f, _YForce, _LeftForce * 0.0f), _ZMaxZero);

            }

            else if (Mathf.Abs(_W) < maxSpeed - 1f)
            {

                _ZZeroTime_W = 0.0f;
                _ZZeroTime_S = 0.0f;
                _ZMaxZero = 0.0f;

                if (_wKey)
                {
                    _ZMinZero_S = 0.0f;
                    _ZMinZero_W = Mathf.Clamp(_ZMinZero_W + Time.fixedDeltaTime, 0, 1);

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.0f), _ZMinZero_W);

                }
                else if (_sKey)
                {
                    _ZMinZero_W = 0.0f;
                    _ZMinZero_S = Mathf.Clamp(_ZMinZero_S + Time.fixedDeltaTime, 0, 1);

                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * 0.0f), _ZMinZero_S);

                }

            }


            if (_ZZeroTime_W == 1 || _ZMaxZero == 1 || _ZMinZero_W == 1)
            {

                LRValueClear();

            }
        }
    }

    //방향키 아무 것도 안누르고 있을 때 적용 시켜야하는 함수
    //큰 값을 기준으로 되도록이면 _Speedometer와 연동 시키는게 좋음(지금은 잘 모르겠음)
    void AllDrag()
    {

        _ART = Mathf.Clamp(_ART + Time.fixedDeltaTime, 0, 1);

        if (_W < -1.0f && _D < -1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent);

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent);

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z += 0.8f;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_ForwardForce > 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
                }
            }
            else if (_ForwardForce < 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);
                }
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W < -1.0f && _D > 1.0f)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z += 0.8f;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent);

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent);

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_ForwardForce > 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);
                }
            }
            else if (_ForwardForce < 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);
                }
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W > 1.0f && _D < -1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent);

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent);

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_ForwardForce > 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);
                }
            }
            else if (_ForwardForce < 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
                }
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W > 1.0f && _D > 1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent);

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent);

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent);
                _velocity.z -= 0.8f;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_ForwardForce > 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);
                }
            }
            else if (_ForwardForce < 0f)
            {
                if (_LeftForce > 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
                }
                else if (_LeftForce < 0f)
                {
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);
                }
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W < -1.0f && _D < 1.0f && _D > -1.0f)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _W = (_ZTemp + _XTemp) * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _W = _ZTemp - _XTemp;
            }

            //_velocity.x<0일때 방향 반대
            if (_D > 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                }
            }
            //_velocity.x >0 일 때 방향 반대
            else if (_D < 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                }
            }
            else
            {
                if (_ForwardForce > 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                else if (_ForwardForce < 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W > 1.0f && _D < 1.0f && _D > -1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent;
                _ZTemp -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent);
                _ZTemp += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent;
                _ZTemp += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _W = _ZTemp * -1f + _XTemp * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent);
                _ZTemp -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _W = _ZTemp - _XTemp;
            }

            //_velocity.x 0으로 초기화 하지 말고 해야할 듯
            //_velocity.x<0일때 방향 반대
            if (_D > 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                }
            }
            //_velocity.x >0 일 때 방향 반대
            else if (_D < 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                }
            }
            else
            {
                if (_ForwardForce > 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                else if (_ForwardForce < 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W < 1.0f && _W > -1.0f && _D < -1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _D = _ZTemp2 + _XTemp2;

            }

            //_velocity.z<0일때 방향 반대
            if (_W > 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                }
            }
            //_velocity.z >0 일 때 방향 반대
            else if (_W < 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                }
            }

            else
            {
                if (_LeftForce > 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                else if (_LeftForce < 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else if (_W < 1.0f && _W > -1.0f && _D > 1.0f)
        {

            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent);
                _ZTemp2 += 0.8f * _percent;

                _velocity.x -= 0.8f * (1.0f - _percent);
                _velocity.z += 0.8f * _percent;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 += 0.8f * _percent;
                _ZTemp2 += 0.8f * (1.0f - _percent);

                _velocity.x += 0.8f * _percent;
                _velocity.z += 0.8f * (1.0f - _percent);

                _D = _ZTemp2 * -1f + _XTemp2 * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 += 0.8f * (1.0f - _percent);
                _ZTemp2 -= 0.8f * _percent;

                _velocity.x += 0.8f * (1.0f - _percent);
                _velocity.z -= 0.8f * _percent;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 -= 0.8f * _percent;
                _ZTemp2 -= 0.8f * (1.0f - _percent);

                _velocity.x -= 0.8f * _percent;
                _velocity.z -= 0.8f * (1.0f - _percent);

                _D = _ZTemp2 + _XTemp2;

            }

            //_velocity.z < 0일때 방향 반대
            if (_W > 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                }
            }
            //_velocity.z >0 일 때 방향 반대
            else if (_W < 0.0f)
            {
                if (_ForwardForce > 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);

                }
                else if (_ForwardForce < 0f)
                {
                    if (_LeftForce > 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), _ART);

                    else if (_LeftForce < 0f)
                        _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce * -1f), _ART);

                }
            }

            else
            {
                if (_LeftForce > 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), _ART);

                else if (_LeftForce < 0f)
                    _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), _ART);
            }

            if (_ART == 1)
            {
                _SingleMotion = true;
                _DoubleMotion = false;

                _ART = 0f;
                _AZT = 0f;

            }

        }

        else
        {
            _SingleMotion = true;
            _DoubleMotion = false;

            _AZT = Mathf.Clamp(_AZT + Time.fixedDeltaTime * 2f, 0, 1);

            _XTemp = 0.0f; _ZTemp = 0.0f; _XTemp2 = 0.0f; _ZTemp2 = 0.0f;
            _W = 0.0f; _D = 0.0f;

            _velocity.x = 0.0f;
            _velocity.z = 0.0f;

            _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * 0.0f, _YForce, _LeftForce * 0.0f), _AZT);

            if (_AZT == 1)
            {

                _ART = 0f;
                _AZT = 0f;

                _Rotor_1 = _Rotor_2 = _Rotor_3 = _Rotor_4 = 60f;

            }
        }

    }

    void AllDrag_Q()
    {
        if (_sKey && _aKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * _Q;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * _Q;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z += 0.8f * _Q;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_leftKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);

            else if (_rightKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

        }

        else if (_sKey && _dKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z += 0.8f * _Q;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * _Q;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _Q;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_leftKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

            else if (_rightKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);

        }

        else if (_wKey && _aKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _Q;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _Q;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_leftKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

            else if (_rightKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);
        }

        else if (_wKey && _dKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * _Q;
                _velocity.z -= 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = _ZTemp + _XTemp;
                _D = _XTemp2 - _ZTemp2;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = _XTemp - _ZTemp;
                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * _Q;
                _velocity.z += 0.8f * (1.0f - 2.0f * _percent) * _Q;

                _W = (_ZTemp + _XTemp) * -1f;
                _D = _ZTemp2 - _XTemp2;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * (1.0f - 2.0f * _percent) * _Q;
                _velocity.z -= 0.8f * _Q;

                _W = _ZTemp - _XTemp;
                _D = _ZTemp2 + _XTemp2;
            }

            if (_leftKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);

            else if (_rightKey)
                _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

        }

        else if (_sKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * _percent * _Q;
                _velocity.z += 0.8f * (1.0f - _percent) * _Q;

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - _percent) * _Q;
                _velocity.z -= 0.8f * _percent * _Q;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _percent * _Q;
                _velocity.z -= 0.8f * (1.0f - _percent) * _Q;

                _W = (_ZTemp + _XTemp) * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - _percent) * _Q;
                _velocity.z += 0.8f * _percent * _Q;

                _W = _ZTemp - _XTemp;
            }

            _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);

        }

        else if (_wKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp -= 0.8f * _percent * _Q;
                _ZTemp -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _percent * _Q;
                _velocity.z -= 0.8f * (1.0f - _percent) * _Q;

                _W = _ZTemp + _XTemp;
            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - _percent) * _Q;
                _velocity.z += 0.8f * _percent * _Q;

                _W = _XTemp - _ZTemp;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp += 0.8f * _percent * _Q;
                _ZTemp += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * _percent * _Q;
                _velocity.z += 0.8f * (1.0f - _percent) * _Q;

                _W = _ZTemp * -1f + _XTemp * -1f;
            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - _percent) * _Q;
                _velocity.z -= 0.8f * _percent * _Q;

                _W = _ZTemp - _XTemp;
            }

            _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce, _YForce, _LeftForce * -1f), Time.fixedDeltaTime * 2.5f);

        }

        else if (_aKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - _percent) * _Q;
                _velocity.z -= 0.8f * _percent * _Q;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _percent * _Q;
                _velocity.z -= 0.8f * (1.0f - _percent) * _Q;

                _D = (_ZTemp2 + _XTemp2) * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - _percent) * _Q;
                _velocity.z += 0.8f * _percent * _Q;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * _percent * _Q;
                _velocity.z += 0.8f * (1.0f - _percent) * _Q;

                _D = _ZTemp2 + _XTemp2;

            }

            _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

        }

        else if (_dKey)
        {
            if (_YForce >= 0 && _YForce < 90)
            {
                _XTemp2 -= 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 += 0.8f * _percent * _Q;

                _velocity.x -= 0.8f * (1.0f - _percent) * _Q;
                _velocity.z += 0.8f * _percent * _Q;

                _D = _XTemp2 - _ZTemp2;

            }
            else if (_YForce >= 90 && _YForce < 180)
            {
                _XTemp2 += 0.8f * _percent * _Q;
                _ZTemp2 += 0.8f * (1.0f - _percent) * _Q;

                _velocity.x += 0.8f * _percent * _Q;
                _velocity.z += 0.8f * (1.0f - _percent) * _Q;

                _D = _ZTemp2 * -1f + _XTemp2 * -1f;
            }
            else if (_YForce >= 180 && _YForce < 270)
            {
                _XTemp2 += 0.8f * (1.0f - _percent) * _Q;
                _ZTemp2 -= 0.8f * _percent * _Q;

                _velocity.x += 0.8f * (1.0f - _percent) * _Q;
                _velocity.z -= 0.8f * _percent * _Q;

                _D = _ZTemp2 - _XTemp2;

            }
            else if (_YForce >= 270 && _YForce < 360)
            {
                _XTemp2 -= 0.8f * _percent * _Q;
                _ZTemp2 -= 0.8f * (1.0f - _percent) * _Q;

                _velocity.x -= 0.8f * _percent * _Q;
                _velocity.z -= 0.8f * (1.0f - _percent) * _Q;

                _D = _ZTemp2 + _XTemp2;

            }

            _droneModel.rotation = Quaternion.Slerp(_droneModel.rotation, Quaternion.Euler(_ForwardForce * -1f, _YForce, _LeftForce), Time.fixedDeltaTime * 2.5f);

        }

    }

    void LimitForce()
    {
        if (_velocity.y >= upMax)
            _velocity.y = upMax;

        else if (_velocity.y <= downMax)
            _velocity.y = downMax;

        if (_YForce >= 0 && _YForce < 90)
        {
            if (_W >= maxSpeed)
            {
                _W = _XTemp + _ZTemp;

                _ZTemp = maxSpeed * (1.0f - _percent);
                _XTemp = maxSpeed * _percent;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_W <= minSpeed)
            {
                _W = _XTemp + _ZTemp;

                _ZTemp = minSpeed * (1.0f - _percent);
                _XTemp = minSpeed * _percent;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }

            if (_D >= maxSpeed)
            {
                _D = _XTemp2 - _ZTemp2;

                _XTemp2 = maxSpeed * (1.0f - _percent);
                _ZTemp2 = (maxSpeed * _percent) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_D <= minSpeed)
            {
                _D = _XTemp2 - _ZTemp2;

                _XTemp2 = (minSpeed * (1.0f - _percent));
                _ZTemp2 = (minSpeed * _percent) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }

        }
        else if (_YForce >= 180 && _YForce < 270)
        {
            if (_W >= maxSpeed)
            {
                _W = -1f * (_XTemp + _ZTemp);

                _ZTemp = (maxSpeed * (1.0f - _percent)) * -1f;
                _XTemp = (maxSpeed * _percent) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_W <= minSpeed)
            {
                _W = -1f * (_XTemp + _ZTemp);

                _ZTemp = (minSpeed * (1.0f - _percent)) * -1f;
                _XTemp = (minSpeed * _percent) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;
            }

            if (_D >= maxSpeed)
            {
                _D = _ZTemp2 - _XTemp2;

                _XTemp2 = (maxSpeed * (1.0f - _percent)) * -1f;
                _ZTemp2 = (maxSpeed * _percent);

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_D <= minSpeed)
            {
                _D = _ZTemp2 - _XTemp2;

                _XTemp2 = (minSpeed * (1.0f - _percent)) * -1f;
                _ZTemp2 = (minSpeed * _percent);

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
        }
        else if (_YForce >= 90 && _YForce < 180)
        {
            if (_W >= maxSpeed)
            {
                _W = (_XTemp - _ZTemp);

                _ZTemp = (maxSpeed * _percent) * -1f;
                _XTemp = (maxSpeed * (1.0f - _percent));

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_W <= minSpeed)
            {
                _W = (_XTemp - _ZTemp);

                _ZTemp = (minSpeed * _percent) * -1f;
                _XTemp = (minSpeed * (1.0f - _percent));

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }

            if (_D >= maxSpeed)
            {
                _D = (_XTemp2 + _ZTemp2) * -1f;

                _XTemp2 = (maxSpeed * _percent) * -1f;
                _ZTemp2 = (maxSpeed * (1.0f - _percent)) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_D <= minSpeed)
            {
                _D = (_XTemp2 + _ZTemp2) * -1f;

                _XTemp2 = (minSpeed * _percent) * -1f;
                _ZTemp2 = (minSpeed * (1.0f - _percent)) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
        }
        else if (_YForce >= 270 && _YForce < 360)
        {
            if (_W >= maxSpeed)
            {
                _W = (_ZTemp - _XTemp);

                _ZTemp = (maxSpeed * _percent);
                _XTemp = (maxSpeed * (1.0f - _percent)) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_W <= minSpeed)
            {
                _W = (_ZTemp - _XTemp);

                _ZTemp = (minSpeed * _percent);
                _XTemp = (minSpeed * (1.0f - _percent)) * -1f;

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }

            if (_D >= maxSpeed)
            {
                _D = _XTemp2 + _ZTemp2;

                _XTemp2 = (maxSpeed * _percent);
                _ZTemp2 = (maxSpeed * (1.0f - _percent));

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
            else if (_D <= minSpeed)
            {
                _D = _XTemp2 + _ZTemp2;

                _XTemp2 = (minSpeed * _percent);
                _ZTemp2 = (minSpeed * (1.0f - _percent));

                _velocity.z = _ZTemp + _ZTemp2;
                _velocity.x = _XTemp + _XTemp2;

            }
        }

    }

    void SingleForce()
    {

        _YForce = _droneModel.transform.eulerAngles.y;
        _YForce = _YForce % 360;
        _percent = (_YForce % 90) / 90.0f;
        /*Rotate에 따라 _velocity값 조정 해줘야함(x랑 z)
         현재 cos 이랑 sin 으로 할까 생각 중이긴 하나
         이상하다면 Math함수로 값을 정해줄 예정*/
        //위/아래 (가)속도
        if (_upKey)
        {
            _velocity.y += 0.4f;

        }
        else if (_downKey)
            _velocity.y -= 0.45f;

        else
        {
            if (_velocity.y > 0.4f)
                _velocity.y -= 0.6f;
            //중력으로 인해 Space키를 뗏을 경우 속도 감소가 더 큼
            else if (_velocity.y < -0.45f)
                _velocity.y += 0.4f;

            else
                _velocity.y = Random.Range(0.0f, 0.5f);
            //Hovering을 위해선 0.5f정도 가속도가 유지되어야 내려가지 않음.
            //일단 그냥 랜덤 값으로 해둠
        }

        if (_leftKey || _rightKey)
        {
            if (_rightKey)
            {
                _droneModel.maxAngularVelocity = 7.0f;
                _droneModel.AddTorque(Vector3.up * 7.0f);

            }
            else if (_leftKey)
            {
                _droneModel.maxAngularVelocity = 7.0f;
                _droneModel.AddTorque(Vector3.down * 7.0f);

            }

        }

        else if (!_leftKey && !_rightKey)
        {
            _droneModel.maxAngularVelocity = 0.0f;

            if (_YForce >= 0 && _YForce < 90)
            {

                if (_wKey)
                {
                    _velocity.z += (0.4f * (1.0f - _percent));
                    _velocity.x += (0.4f * (_percent));

                    _ZTemp += (0.4f * (1.0f - _percent));
                    _XTemp += (0.4f * (_percent));

                    _W = _ZTemp + _XTemp;

                }
                else if (_sKey)
                {
                    _velocity.z -= (0.4f * (1.0f - _percent));
                    _velocity.x -= (0.4f * (_percent));

                    _ZTemp -= (0.4f * (1.0f - _percent));
                    _XTemp -= (0.4f * (_percent));

                    _W = _ZTemp + _XTemp;

                }

                if (_dKey)
                {
                    _velocity.x += (0.4f * (1.0f - _percent));
                    _velocity.z -= (0.4f * (_percent));

                    _XTemp2 += (0.4f * (1.0f - _percent));
                    _ZTemp2 -= (0.4f * (_percent));

                    _D = _XTemp2 - _ZTemp2;

                }
                else if (_aKey)
                {
                    _velocity.x -= (0.4f * (1.0f - _percent));
                    _velocity.z += (0.4f * (_percent));

                    _XTemp2 -= (0.4f * (1.0f - _percent));
                    _ZTemp2 += (0.4f * (_percent));

                    _D = _XTemp2 - _ZTemp2;

                }

            }
            else if (_YForce >= 90 && _YForce < 180)
            {

                if (_wKey)
                {
                    _velocity.x += (0.4f * (1.0f - _percent));
                    _velocity.z -= (0.4f * (_percent));

                    _XTemp += (0.4f * (1.0f - _percent));
                    _ZTemp -= (0.4f * (_percent));

                    _W = _XTemp - _ZTemp;

                }
                else if (_sKey)
                {
                    _velocity.x -= (0.4f * (1.0f - _percent));
                    _velocity.z += (0.4f * (_percent));

                    _XTemp -= (0.4f * (1.0f - _percent));
                    _ZTemp += (0.4f * (_percent));

                    _W = _XTemp - _ZTemp;

                }

                if (_dKey)
                {
                    _velocity.z -= (0.4f * (1.0f - _percent));
                    _velocity.x -= (0.4f * (_percent));

                    _ZTemp2 -= (0.4f * (1.0f - _percent));
                    _XTemp2 -= (0.4f * (_percent));

                    _D = (_XTemp2 + _ZTemp2) * -1f;

                }
                else if (_aKey)
                {
                    _velocity.z += (0.4f * (1.0f - _percent));
                    _velocity.x += (0.4f * (_percent));

                    _ZTemp2 += (0.4f * (1.0f - _percent));
                    _XTemp2 += (0.4f * (_percent));

                    _D = (_XTemp2 + _ZTemp2) * -1f;

                }

            }
            else if (_YForce >= 180 && _YForce < 270)
            {

                if (_wKey)
                {
                    _velocity.z -= (0.4f * (1.0f - _percent));
                    _velocity.x -= (0.4f * (_percent));

                    _ZTemp -= (0.4f * (1.0f - _percent));
                    _XTemp -= (0.4f * (_percent));

                    _W = _ZTemp * -1f + _XTemp * -1f;

                }
                else if (_sKey)
                {
                    _velocity.z += (0.4f * (1.0f - _percent));
                    _velocity.x += (0.4f * (_percent));

                    _ZTemp += (0.4f * (1.0f - _percent));
                    _XTemp += (0.4f * (_percent));

                    _W = _ZTemp * -1f + _XTemp * -1f;

                }

                if (_dKey)
                {
                    _velocity.x -= (0.4f * (1.0f - _percent));
                    _velocity.z += (0.4f * (_percent));

                    _XTemp2 -= (0.4f * (1.0f - _percent));
                    _ZTemp2 += (0.4f * (_percent));

                    _D = _ZTemp2 - _XTemp2;

                }
                else if (_aKey)
                {
                    _velocity.x += (0.4f * (1.0f - _percent));
                    _velocity.z -= (0.4f * (_percent));

                    _XTemp2 += (0.4f * (1.0f - _percent));
                    _ZTemp2 -= (0.4f * (_percent));

                    _D = _ZTemp2 - _XTemp2;

                }


            }
            else if (_YForce >= 270 && _YForce < 360)
            {

                if (_wKey)
                {
                    _velocity.x -= (0.4f * (1.0f - _percent));
                    _velocity.z += (0.4f * (_percent));

                    _XTemp -= (0.4f * (1.0f - _percent));
                    _ZTemp += (0.4f * (_percent));

                    _W = _ZTemp - _XTemp;

                }
                else if (_sKey)
                {
                    _velocity.x += (0.4f * (1.0f - _percent));
                    _velocity.z -= (0.4f * (_percent));

                    _XTemp += (0.4f * (1.0f - _percent));
                    _ZTemp -= (0.4f * (_percent));

                    _W = _ZTemp - _XTemp;

                }

                if (_dKey)
                {
                    _velocity.z += (0.4f * (1.0f - _percent));
                    _velocity.x += (0.4f * (_percent));

                    _ZTemp2 += (0.4f * (1.0f - _percent));
                    _XTemp2 += (0.4f * (_percent));

                    _D = _XTemp2 + _ZTemp2;

                }
                else if (_aKey)
                {
                    _velocity.z -= (0.4f * (1.0f - _percent));
                    _velocity.x -= (0.4f * (_percent));

                    _ZTemp2 -= (0.4f * (1.0f - _percent));
                    _XTemp2 -= (0.4f * (_percent));

                    _D = _XTemp2 + _ZTemp2;

                }

            }
        }

    }

    //Rotor1 우측상단 ,Rotor2 우측하단, Rotor3 좌측하단, Rotor4 좌측상단
    void RotorTorque()
    {
        //회전 시킬 경우에도 드론이 기울여지므로 그 부분도 생각을 해야함.......하...
        CW[0].Rotate(Vector3.forward * _Rotor_2 * 50f);
        CW[1].Rotate(Vector3.forward * _Rotor_4 * 50f);
        CCW[0].Rotate(Vector3.back * _Rotor_1 * 50f);
        CCW[1].Rotate(Vector3.back * _Rotor_3 * 50f);

        //Mathf.Lerp으로 잡아줄까....?
        //forwardForce /backForce
        //기울기를 30 -30 으로 범위를 정해 뒀기 때문에 0.75를 곱해 줌 그러면 30하고 -30 보다 크거나 작게 안나옴
        //일부러 그렇게 되도록 로터 값을 맞춰줬음
        _ForwardForce = Mathf.Clamp((Mathf.Min(_Rotor_2, _Rotor_3) - Mathf.Min(_Rotor_1, _Rotor_4)) * 0.75f, -30f, 30f);

        //LeftForce /RightForce
        _LeftForce = Mathf.Clamp((Mathf.Min(_Rotor_1, _Rotor_2) - Mathf.Min(_Rotor_3, _Rotor_4)) * 0.75f, -30f, 30f);

        //그냥 로터 값 노가다
        //Rotor1 우측상단 ,Rotor2 우측하단, Rotor3 좌측하단, Rotor4 좌측상단

        if (_DoubleMotion)
        {
            if (_wKey && _dKey)
            {
                _Rotor_1 = _MinRotor;
                _Rotor_2 = _MaxRotor;
                _Rotor_3 = _MaxRotor;
                _Rotor_4 = _MaxRotor;

            }
            else if (_wKey && _aKey)
            {
                _Rotor_1 = _MaxRotor;
                _Rotor_2 = _MaxRotor;
                _Rotor_3 = _MaxRotor;
                _Rotor_4 = _MinRotor;

            }
            else if (_sKey && _aKey)
            {
                _Rotor_1 = _MaxRotor;
                _Rotor_2 = _MaxRotor;
                _Rotor_3 = _MinRotor;
                _Rotor_4 = _MaxRotor;

            }
            else if (_sKey && _dKey)
            {
                _Rotor_1 = _MaxRotor;
                _Rotor_2 = _MinRotor;
                _Rotor_3 = _MaxRotor;
                _Rotor_4 = _MaxRotor;

            }

        }

        else if (_SingleMotion)
        {

            if (_wKey)
            {
                _Rotor_1 = _MinRotor;
                _Rotor_2 = _MaxRotor;
                _Rotor_3 = _MaxRotor;
                _Rotor_4 = _MinRotor;

            }
            else if (_sKey)
            {
                _Rotor_1 = _MaxRotor;
                _Rotor_2 = _MinRotor;
                _Rotor_3 = _MinRotor;
                _Rotor_4 = _MaxRotor;

            }
            else if (_aKey)
            {
                _Rotor_1 = _MaxRotor;
                _Rotor_2 = _MaxRotor;
                _Rotor_3 = _MinRotor;
                _Rotor_4 = _MinRotor;

            }
            else if (_dKey)
            {
                _Rotor_1 = _MinRotor;
                _Rotor_2 = _MinRotor;
                _Rotor_3 = _MaxRotor;
                _Rotor_4 = _MaxRotor;

            }
            if ((!_wKey && !_sKey && !_aKey && !_dKey)
                && ((_W < 1f && _W > -1f) && (_D < 1f && _D > -1f)))
            {
                if (_upKey)
                {
                    _Rotor_1 = 80f;
                    _Rotor_2 = 80f;
                    _Rotor_3 = 80f;
                    _Rotor_4 = 80f;

                }
                else if (_downKey)
                {
                    _Rotor_1 = 50f;
                    _Rotor_2 = 50f;
                    _Rotor_3 = 50f;
                    _Rotor_4 = 50f;

                }
            }
        }
    }
}