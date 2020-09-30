using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneControl : MonoBehaviour
{
    [Header("Component")]
    public Transform GantryCrane; //겐트리 트레인
    public Transform _platformParent; //움직이는 플랫폼
    public Animator _cargoHookAnimator; //카고훅 애니메이터
    public Transform _cargoHookBody;
    public Transform[] RubberTires = new Transform[4];  //바퀴
    public Transform _containerSet;

    [Header("CargoHook_operation")]
    public string _containerName;   //카고훅 자식 컨테이너 이름
    public Collider _other;

    [Header("PlatformParent_operation")]
    public GameObject _leftMaxPosition;
    public GameObject _rightMaxPosition;
    public float _platformParentSpeed;
    public float _platformParentAccelerationSpeed;
    public float _platformParentMaxSpeed;

    [Header("Rubber_operation")]
    public float _rubberY;
    public int _freeRubber;
    public bool _rotation_R;
    public float _rubberSpeed;
    public float _rubberAccelerationSpeed;
    public float _rubberMaxSpeed;

    [Header("JoyStick")]
    public Transform _joyStickL;
    public Transform _joyStickR;
    bool _aKey, _dKey, _wKey, _sKey;
    bool _upKey, _downKey, _leftKey, _rightKey;
    float _rU, _rUR, _rUL;
    float _rD, _rDR, _rDL;
    float _rR;
    float _rL;
    float _rStop;
    float _lU, _lUR, _lUL;
    float _lD, _lDR, _lDL;
    float _lR;
    float _lL;
    float _lStop;

    [Header("튜토리얼용")]
    bool isTutorial;
    public bool isCatching;

    [Header("Audio")]
    public AudioSource _audioSource_move1;
    public AudioSource _audioSource_move2;
    public AudioSource _audioSource_line;

    public List<Vector3> containerInfo = new List<Vector3>();
    public Vector3 gantryCraneInfo = new Vector3();
    public float _rubberRotate;
    float _percent;
    public bool _cargohookMove_Down;
    public bool _cargohookMove_Drop;
    public bool _cargohookPause;

    [Header("Script")]
    public VRInput VRInputScript;

    private void Start()
    {
        Debug.Log("W : 플랫폼앞? S : 플랫폼뒤? A : 겐트리크레인 몸통 앞? D : 겐트리크레인 몸통 뒤?");
        Debug.Log("왼방향키 : 컨테이너 잡고 오른방향키 : 컨테이너 놓고 앞방향키 : 내려감 뒷방향키 : 올라감");

        RubberTires[0] = transform.Find("rubberTires_1");
        RubberTires[1] = transform.Find("rubberTires_2");
        RubberTires[2] = transform.Find("rubberTires_3");
        RubberTires[3] = transform.Find("rubberTires_4");
        //24.3 바퀴 대략 1.6

        _rubberY = 0f;
        _freeRubber = 1;
        _rubberSpeed = 0f;
        _rubberAccelerationSpeed = 0.045f;
        _rubberMaxSpeed = 1.8f;
        
        //왼쪽 조이스틱
        _aKey = false;
        _dKey = false;
        _wKey = false;
        _sKey = false;

        //오른쪽 조이스틱
        _upKey = false;
        _downKey = false;
        _leftKey = false;
        _rightKey = false;
        _rU = _rUR = _rUL = 0f;
        _rD = _rDR = _rDL = 0f;
        _rR = 0f;
        _rL = 0f;
        _rStop = 0f;
        _lU = _lUR = _lUL = 0f;
        _lD = _lDR = _lDL = 0f;
        _lR = 0f;
        _lL = 0f;
        _lStop = 0f;

        _cargohookMove_Down = true;
        _cargohookMove_Drop = false;
        _cargohookPause = false;

        //컨테이너 크기 대략 2.3
        _platformParentSpeed = 0f;
        _platformParentAccelerationSpeed = 0.04f;
        _platformParentMaxSpeed = 1.6f;

        //카고훅에 있는 애니메이터를 가져옴
        _cargoHookAnimator = _platformParent.transform.Find("cargoHook").GetComponent<Animator>();

        //카고훅이 옆으로 쫙 펼치진 상태로 시작
        _cargoHookAnimator.SetFloat("Extension", 1);

        for (int i = 0; i < _containerSet.childCount; i++)
        {
            containerInfo.Add(_containerSet.GetChild(i).position);
        }
        gantryCraneInfo = GantryCrane.position;

    }
    void Update()
    {
        if (!_cargohookPause)
        {
            _aKey = Input.GetKey(Static._Akey) || VRInputScript.LeftJoystick.x < -0.8f;
            _dKey = Input.GetKey(Static._Dkey) || VRInputScript.LeftJoystick.x > 0.8f;
            _wKey = Input.GetKey(Static._Wkey) || VRInputScript.LeftJoystick.y > 0.8f;
            _sKey = Input.GetKey(Static._Skey) || VRInputScript.LeftJoystick.y < -0.8f;
            _upKey = Input.GetKey(Static._Upkey) || VRInputScript.RightJoystick.y > 0.8f;
            _downKey = Input.GetKey(Static._Downkey) || VRInputScript.RightJoystick.y < -0.8f;
            _leftKey = Input.GetKey(Static._Leftkey) || VRInputScript.RightJoystick.x < -0.8f;
            _rightKey = Input.GetKey(Static._Rightkey) || VRInputScript.RightJoystick.x > 0.8f;
        }

        _rubberRotate = RubberTires[0].eulerAngles.y;
        _percent = (_rubberRotate % 90f) / 90f;

        if (Static._play)
        {
            MovingPlatformParentMovement();

            CargoHook_Operation();
            JoyStickRotation();
            Rubber_Operation();
            SoundPlaying();
        }
    }
    //왼쪽 레버 움직임
    //오른쪽 레버 카고훅

    public void Rubber_Operation()
    {
        //360 ~ 270 ~ 360 도로 로테이션 제한 둠.
        //_freeRubber == 0  -> 동작 중
        //_freeRubber == 1  -> 동작 안함
        //_freeRubber == 2  -> 자유조작 가능
        if (Input.GetKeyDown(KeyCode.R) && _freeRubber == 1)
        {
            if (_rubberRotate > 269.54f)
            {
                _rotation_R = true;
                _freeRubber = 0;
                _rubberY = _rubberY % 360;
            }
        }
        else if (Input.GetKeyDown(KeyCode.L) && _freeRubber == 1)
        {
            if (_rubberRotate < 0.46f || _rubberRotate > 270.46f)
            {
                _rotation_R = false;
                _freeRubber = 0;
                _rubberY = _rubberY % 360;
            }
        }
        else if (Input.GetKey(KeyCode.R) && _freeRubber == 2)
        {
            if (_rubberRotate > 269.54f)
            {
                _rubberY += 0.4500f;
                _rubberY = _rubberY % 360;
                RotationRubber();
            }
        }
        else if (Input.GetKey(KeyCode.L) && _freeRubber == 2)
        {
            if (_rubberRotate < 0.46f || _rubberRotate > 270.46f)
            {
                _rubberY -= 0.4500f;
                _rubberY = _rubberY % 360;
                RotationRubber();
            }
        }

        if (_freeRubber == 0)
        {
            if (_rotation_R)
                _rubberY += 0.4500f;
            else
                _rubberY -= 0.4500f;

            RotationRubber();

            if (Mathf.Abs(_rubberY % 90f) > 89.55f && !_rotation_R)
                _freeRubber = 1;
            else if (Mathf.Abs(_rubberY % 90f) < 0.44f && _rotation_R)
                _freeRubber = 1;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (_freeRubber == 1 || _freeRubber == 0)
                _freeRubber = 2;

            else
                _freeRubber = 1;

            _rotation_R = false;
        }

        //A와 D로 겐트리크레인 이동
        if (_aKey)
        {
            _rubberSpeed += _rubberAccelerationSpeed;
            _rubberSpeed = Mathf.Clamp(_rubberSpeed, -_rubberMaxSpeed, _rubberMaxSpeed);
            if (_rubberRotate < 0.45f)
                GantryCrane.Translate(Time.deltaTime * _rubberSpeed * _percent, 0f, -Time.deltaTime * _rubberSpeed * (1 - _percent));
            else if (_rubberRotate > 269.55f && _rubberRotate < 360.2f)
                GantryCrane.Translate(Time.deltaTime * _rubberSpeed * (1 - _percent), 0f, -Time.deltaTime * _rubberSpeed * _percent);
        }
        else if (_dKey)
        {
            _rubberSpeed -= _rubberAccelerationSpeed;
            _rubberSpeed = Mathf.Clamp(_rubberSpeed, -_rubberMaxSpeed, _rubberMaxSpeed);
            if (_rubberRotate < 0.45f)
                GantryCrane.Translate(Time.deltaTime * _rubberSpeed * _percent, 0f, -Time.deltaTime * _rubberSpeed * (1 - _percent));
            else if (_rubberRotate > 269.55f && _rubberRotate < 360.2f)
                GantryCrane.Translate(Time.deltaTime * _rubberSpeed * (1 - _percent), 0f, -Time.deltaTime * _rubberSpeed * _percent);
        }
        else
        {
            if (_rubberSpeed > 0.045f)
            {
                _rubberSpeed -= _rubberAccelerationSpeed;
                if (_rubberRotate < 0.45f)
                    GantryCrane.Translate(Time.deltaTime * _rubberSpeed * _percent, 0f, -Time.deltaTime * _rubberSpeed * (1 - _percent));
                else if (_rubberRotate > 269.55f && _rubberRotate < 360.2f)
                    GantryCrane.Translate(Time.deltaTime * _rubberSpeed * (1 - _percent), 0f, -Time.deltaTime * _rubberSpeed * _percent);
            }
            else if (_rubberSpeed < -0.045f)
            {
                _rubberSpeed += _rubberAccelerationSpeed;
                if (_rubberRotate < 0.45f)
                    GantryCrane.Translate(Time.deltaTime * _rubberSpeed * _percent, 0f, -Time.deltaTime * _rubberSpeed * (1 - _percent));
                else if (_rubberRotate > 269.55f && _rubberRotate < 360.2f)
                    GantryCrane.Translate(Time.deltaTime * _rubberSpeed * (1 - _percent), 0f, -Time.deltaTime * _rubberSpeed * _percent);
            }
            else
            {
                _rubberSpeed = 0f;
            }
        }
    }

    //Rubber를 돌려주는 함수
    public void RotationRubber()
    { 
        RubberTires[0].rotation = Quaternion.Euler(RubberTires[0].rotation.x, _rubberY, RubberTires[0].rotation.z);
        RubberTires[1].rotation = Quaternion.Euler(RubberTires[0].rotation.x, _rubberY, RubberTires[0].rotation.z);
        RubberTires[2].rotation = Quaternion.Euler(RubberTires[0].rotation.x, 180f + _rubberY, RubberTires[0].rotation.z);
        RubberTires[3].rotation = Quaternion.Euler(RubberTires[0].rotation.x, 180f + _rubberY, RubberTires[0].rotation.z);
    }

    //platformParent 움직이는함수
    public void MovingPlatformParentMovement() 
    {
        if (_sKey)
        {
            _platformParentSpeed += _platformParentAccelerationSpeed;
            _platformParentSpeed = Mathf.Clamp(_platformParentSpeed, -_platformParentMaxSpeed, _platformParentMaxSpeed);
            if (_platformParent.position.x < (_rightMaxPosition.transform.position.x - 4.35f)&& _platformParent.position.x >= (_leftMaxPosition.transform.position.x + 4.35f))
                _platformParent.Translate(Time.deltaTime * _platformParentSpeed, 0f, 0f);          
            else if(_platformParent.position.x < (_leftMaxPosition.transform.position.x + 4.35f))
            {
                //반대방향 이동 후 제한된 거리를 초과하였을 때 속도를 대략 0으로 만든 후 위치 수정
                if (_platformParentSpeed < 0f)
                    _platformParentSpeed += _platformParentAccelerationSpeed;
                else
                    _platformParent.position = new Vector3(_leftMaxPosition.transform.position.x + 4.35f, _platformParent.position.y, _platformParent.position.z);
            
            }
        }
        else if (_wKey)
        {
            _platformParentSpeed -= _platformParentAccelerationSpeed;
            _platformParentSpeed = Mathf.Clamp(_platformParentSpeed, -_platformParentMaxSpeed, _platformParentMaxSpeed);
            if ((_platformParent.position.x > (_leftMaxPosition.transform.position.x + 4.35f)) && (_platformParent.position.x <= (_rightMaxPosition.transform.position.x - 4.35f)))
                _platformParent.Translate(Time.deltaTime * _platformParentSpeed, 0f, 0f);
            else if (_platformParent.position.x > (_rightMaxPosition.transform.position.x - 4.35f))
            {
                //반대방향 이동 후 제한된 거리를 초과하였을 때 속도를 대략 0으로 만든 후 위치 수정
                if (_platformParentSpeed > 0f)
                    _platformParentSpeed -= _platformParentAccelerationSpeed;
                else               
                    _platformParent.position = new Vector3(_rightMaxPosition.transform.position.x - 4.35f, _platformParent.position.y, _platformParent.position.z);               
            }
        }
        else
        {
            //W , S 를 누르지 않을 땐 (멈추는)속도 1.5배
            if (_platformParentSpeed > 0.04f)
            {            
                _platformParentSpeed -= _platformParentAccelerationSpeed * 1.5f;
                if (_platformParent.position.x < (_rightMaxPosition.transform.position.x - 4.35f))
                    _platformParent.Translate(Time.deltaTime * _platformParentSpeed, 0f, 0f);
            }
            else if (_platformParentSpeed < -0.04f)
            {
                _platformParentSpeed += _platformParentAccelerationSpeed * 1.5f;
                if (_platformParent.position.x > (_leftMaxPosition.transform.position.x + 4.35f))
                    _platformParent.Translate(Time.deltaTime * _platformParentSpeed, 0f, 0f);
            }
            else
            {
                _platformParentSpeed = 0f;
            }
        }
    }
    //업애로우 다운애로우
    public void CargoHook_Operation() 
    {
        //카고훅 위아래 조작
        if (_upKey) //윗 방향키 누르면 훅 올라감
        {
            _cargoHookAnimator.SetFloat("Level", Mathf.Clamp(_cargoHookAnimator.GetFloat("Level") - Time.deltaTime * 0.1f, 0f, 0.9f));
        }
        else if (_downKey && _cargohookMove_Down) //아래 방향키 누르면 훅 내려감
        {
            _cargoHookAnimator.SetFloat("Level", Mathf.Clamp(_cargoHookAnimator.GetFloat("Level") + Time.deltaTime * 0.1f, 0f, 0.9f));
        }
        if (_rightKey && _cargohookMove_Drop)  //오른쪽 방향키 컨테이너 놓음
        {
            try
            {
                if (_other != null && isCatching)
                {                   
                    _cargoHookBody.transform.Find(_other.gameObject.name).SetParent(_containerSet.transform);
                    _other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    _other.gameObject.tag = "container";
                    _other = null;
                    isCatching = false;
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e);
            }

        }
        else if (_leftKey)   //왼쪽 방향키 컨테이너 잡음
        {
            if (_other != null && !isCatching)
            {
                if (_cargoHookBody.position.x + 0.05f >= _other.transform.position.x &&
                    _cargoHookBody.position.x - 0.05f <= _other.transform.position.x &&
                    _cargoHookBody.position.z + 0.45f >= _other.transform.position.z &&
                    _cargoHookBody.position.z - 0.05f <= _other.transform.position.z)
                {
                    _other.transform.parent = _cargoHookBody;
                    _other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    _other.gameObject.tag = "Untagged";
                    isCatching = true;
                    _cargohookPause = true;
                    StartCoroutine(StopCargohook());
                }

            }
        }
        
    }
    //잡을 때 아무동작 못하게 막음
    public IEnumerator StopCargohook()
    {
        yield return new WaitForSeconds(2f);
        _cargohookPause = false;
    }

    public void setTutorial(bool swi)
    {
        isTutorial = swi;
    }

    public bool getCatching()
    {
        return isCatching;
    }

    public void JoyStickRotation()
    {
        if (_upKey || _downKey || _rightKey || _leftKey)
        {
            _rStop = 0f;

            if (_upKey && _rightKey)
            {
                _rUR = Mathf.Clamp(_rUR + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(50, 0, 50), _rUR);
                _rU = _rUL = 0f;
                _rD = _rDR = _rDL = 0f;
                _rR = 0f; _rL = 0f;
            }
            else if(_upKey && _leftKey)
            {
                _rUL = Mathf.Clamp(_rUR + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(-50, 0, 50), _rUL);
                _rU = _rUR = 0f;
                _rD = _rDR = _rDL = 0f;
                _rR = 0f; _rL = 0f;
            }
            if (_upKey)
            {
                _rU = Mathf.Clamp(_rU + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(0, 0, 45), _rU);
                _rUR = _rUL = 0f;
                _rD = _rDR = _rDL = 0f;
                _rR = 0f; _rL = 0f;
            }
            else if (_downKey)
            {
                _rD = Mathf.Clamp(_rD + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(0, 0, -45), _rD);
                _rU = _rUL = _rUR = 0f;
                _rDR = _rDL = 0f;
                _rR = 0f; _rL = 0f;
            }
            if (_rightKey)
            {
                _rR = Mathf.Clamp(_rR + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(45, 0, 0), _rR);
                _rU = _rUL = _rUR = 0f;
                _rD = _rDR = _rDL = 0f;
                _rL = 0f;
            }
            else if (_leftKey)
            {
                _rL = Mathf.Clamp(_rL + Time.deltaTime, 0f, 1f);
                _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(-45, 0, 0), _rL);
                _rU = _rUL = _rUR = 0f;
                _rD = _rDR = _rDL = 0f;
                _rR = 0f;
            }
        }
        else
        {
            _rStop = Mathf.Clamp(_rStop + Time.deltaTime * 1.5f, 0f, 1f);
            _joyStickR.rotation = Quaternion.Slerp(_joyStickR.rotation, Quaternion.Euler(0, 0, 0), _rStop);
            _rU = _rUL = _rUR = 0f;
            _rD = _rDR = _rDL = 0f;
            _rR = 0f; _rL = 0f;
        }

        if (_wKey || _sKey || _aKey || _dKey)
        {
            _lStop = 0f;

            if (_wKey && _dKey)
            {
                _lUR = Mathf.Clamp(_lUR + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(50, 0, 50), _lUR);
                _lU = _lUL = 0f;
                _lD = _lDR = _lDL = 0f;
                _lR = 0f; _lL = 0f;
            }
            else if (_wKey && _aKey)
            {
                _lUL = Mathf.Clamp(_lUL + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(-50, 0, 50), _lUL);
                _lU = _lUR = 0f;
                _lD = _lDR = _lDL = 0f;
                _lR = 0f; _lL = 0f;
            }
            if (_wKey)
            {
                _lU = Mathf.Clamp(_lU + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(0, 0, 45), _lU);
                _lUL = _lUR = 0f;
                _lD = _lDR = _lDL = 0f;
                _lR = 0f; _lL = 0f;
            }
            else if (_sKey)
            {
                _lD = Mathf.Clamp(_lD + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(0, 0, -45), _lD);
                _lU = _lUL = _lUR = 0f;
                _lDR = _lDL = 0f;
                _lR = 0f; _lL = 0f;
            }
            if (_dKey)
            {
                _lR = Mathf.Clamp(_lR + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(45, 0, 0), _lR);
                _lU = _lUL = _lUR = 0f;
                _lD = _lDR = _lDL = 0f;
                _lL = 0f;
            }
            else if (_aKey)
            {
                _lL = Mathf.Clamp(_lL + Time.deltaTime, 0f, 1f);
                _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(-45, 0, 0), _lL);
                _lU = _lUL = _lUR = 0f;
                _lD = _lDR = _lDL = 0f;
                _lR = 0f;
            }
        }
        else
        {
            _lStop = Mathf.Clamp(_lStop + Time.deltaTime * 1.5f, 0f, 1f);
            _joyStickL.rotation = Quaternion.Slerp(_joyStickL.rotation, Quaternion.Euler(0, 0, 0), _lStop);
            _lU = _lUL = _lUR = 0f;
            _lD = _lDR = _lDL = 0f;
            _lR = 0f; _lL = 0f;
        }

    }

    private void SoundPlaying()
    {
        if (_freeRubber == 0 || _rubberSpeed != 0)
        {
            if(!_audioSource_move1.isPlaying)
            _audioSource_move1.Play();
        }
        else _audioSource_move1.Pause();


        setSoundClip(_platformParentSpeed, _audioSource_move2);
        setSoundClip(_upKey, _downKey, _audioSource_line);
    }

    private void setSoundClip(float speed, AudioSource source)
    {
        if (speed != 0)
        {
            if (!source.isPlaying) source.Play();
        }
        else
        {
            if (source.isPlaying) source.Pause();
        }
    }

    private void setSoundClip(bool key1, bool key2 , AudioSource source)
    {
        if (key1 || key2)
        {
            if (!source.isPlaying) source.Play();
        }
        else
        {
            if (source.isPlaying) source.Pause();
        }
    }
}