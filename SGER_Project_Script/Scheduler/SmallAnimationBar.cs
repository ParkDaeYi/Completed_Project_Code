using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallAnimationBar : MonoBehaviour {
    [Header("Animation Info")]
    public int _layerNumber; //레이어 번호
    public bool _actionOrFace; //true면 단일행동 false면 페이스
    public Item _item; //해당 표정, 단일 행동을 하는 객체의본체
    public Animator _animator; //애니메이션 담당 하는 컴포넌트
    public string _animationName; //애니메이션 이름

    [Header("Animation Play Info")]
    public bool _moveCheck; //이동하는 애니메이션인가
    public Vector3 _arriveLocation; // 도착 위치 //바생성시 값 정해짐
    public double _time; //현재 시간
    public GameObject _thisAniBar; //현재 애니메이션 바
    public TimeController _timeScript; //시간변수가 존재하는 스크립트
    private float _aniBarWidth; //애니메이션 바의 가로 길이
    public double _startTime = 0; //시작 시간(초단위)
    public double _finishTime = 10; //끝나는 시간(초단위)
    public double _playTime; //총 플레이 시간(초단위) = 끝나는시간 - 시작시간
    public bool _playSw = false; //애니메이션 실행 여부
    public Vector3 _startLocation; //시작 위치
    private float _currentXPos; // 현재 x좌표 저장하는 변수

    [Header("Face Info")]
    public SkinnedMeshRenderer _skin;
    public float[] _face = new float[4]; // 컴포넌트 위에서 부터 0~3

    public GameObject _animationBar;
    public RectTransform _animationRectTransform;



	// Use this for initialization
	void Start () {
        _animationRectTransform = _animationBar.GetComponent<RectTransform>();

        _thisAniBar = this.gameObject;
        _timeScript = GameObject.Find("Controllers").transform.Find("SchedulerController").GetComponent<TimeController>();
        _time = _timeScript._time;

        Start_Update();
    }

    public void Start_Update()
    {
        _aniBarWidth = _thisAniBar.transform.GetComponent<RectTransform>().rect.width;
        _startTime = (_thisAniBar.transform.localPosition.x - _aniBarWidth / 2 - (-894.3f)) * 210 / 1785;
        _finishTime = (_thisAniBar.transform.localPosition.x + _aniBarWidth / 2 - (-894.3f)) * 210 / 1785;
        _playTime = _finishTime - _startTime;
        _time = _timeScript._time;
    }

    // Update is called once per frame
    void Update () {
        if (_animationBar.activeSelf)
        {
            this.gameObject.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_animationRectTransform.rect.width, this.gameObject.transform.GetComponent<RectTransform>().rect.height);
            this.gameObject.transform.position = new Vector2(_animationBar.transform.position.x, this.gameObject.transform.position.y);
        }

        Start_Update();
        if (_actionOrFace) //단일 행동
        {
            if (_playSw) //실행중 
            {
                if (_startTime >= _time || _finishTime <= _time) //중지시킴
                {
                    _playSw = false;
                    Debug.Log("layerNumber = " + _layerNumber);
                    _animator.SetBool(_animationName, false);
                    _animator.SetLayerWeight(_layerNumber, 0);
                }
                if (_moveCheck)
                    Character_Move_By_Timer();
            }
            else //실행중이 아님
            {
                /* 실행중이 아닐때 시작위치 재설정 */
                if (_startTime - _time < 0.02 && _startTime - _time > 0)
                {
                    _startLocation = _item.item3d.transform.position;
                }

                if (_startTime < _time && _finishTime > _time) //실행해야함
                {
                    if (_startTime - _time > 0)
                        _startLocation = _item.item3d.transform.position;
                    _playSw = true;
                    _animator.SetLayerWeight(_layerNumber, 1);
                    _animator.SetBool(_animationName, true);
                    //애니메이션을 시행할때 해당 방향을 쳐다보게 만듬
                    if (_moveCheck)
                        _item.item3d.transform.LookAt(new Vector3(_arriveLocation.x, _arriveLocation.y, _item.item3d.transform.localRotation.z)); //LookAt을 활용해 작성하니 단순 방향으로 트는게 아닌 해당 한점을 향해 기우는것 같음

                }
            }
        }
        else //얼굴 모핑
        {
            if (_playSw) //애니메이션이 실행중임
            {
                if (_startTime >= _time || _finishTime <= _time)
                {
                    _playSw = false;//실행시간이 아님 고로 종료
                    FaceInit();
                }
            }
            else //애니메이션이 실행중이지 않음
            {
                if (_startTime < _time && _finishTime > _time)
                {
                    _playSw = true;
                    FaceMove();
                }
            }
        }

    }

    void FaceMove()
    {
        for (int i = 0; i < 4; i++)
        {
            _item.item3d.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, _face[i]);
        }
    }

    void FaceInit()
    {
        for (int i = 0; i < 4; i++)
        {
            _item.item3d.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, 0);
        }
    }

    void Character_Move_By_Timer()
    {
        float val = (float)((_time - _startTime) / (_playTime));

        _item.item3d.transform.parent.transform.position =
            new Vector3(
                (val * _arriveLocation.x + (1 - val) * _startLocation.x)
                , 1
                , (val * _arriveLocation.z + (1 - val) * _startLocation.z));
    }
}
