using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public Transform _droneTransform; //드론 Transform

    [Header("AudioSource")]
    public AudioSource _flying; //나는 중
    public AudioSource _collide; //충돌
    public AudioSource _dragging; //끌리는 중
    public AudioSource _wind; //바람 소리

    [Header("Script")]
    public Drone _drone; //drone script

    [Header("Speed")]
    public Vector3 _prePosition; //이전 위치
    public float _moveDistancePerFrame; //프레임 당 이동거리

    private void Start()
    {
        _prePosition = _droneTransform.position; //드론의 이전 위치 초기화
    }

    private void FixedUpdate()
    {
        CalculateDroneMoveDistance();
        FlyingSound();
        DraggingSound();
        WindSound();
    }
    /* 드론이 프레임당 이동 거리를 계산하는 함수 */
    public void CalculateDroneMoveDistance()
    {
        _moveDistancePerFrame = Vector3.Distance(_droneTransform.position, _prePosition); //이동 거리 계산
        _prePosition = _droneTransform.position; //이전 위치 갱신
    }

    /* 나는 소리 */
    public void FlyingSound()
    {
        if (_drone._StartUp) //드론 시동이 걸리면
        {
            _flying.volume = 0.4f + _moveDistancePerFrame * (0.6f / 0.8f);
            if (!_flying.isPlaying) //소리가 실행 중이지 않으면
            {
                _flying.Play(); //드론 사운드 실행
            }
        }
        else //드론 시동이 꺼지면
        {
            _flying.Stop(); //드론 사운드 중지
        }
    }

    /* 크게 부딪혔을 때 */
    public void CollideEnterSound()
    {
        _collide.volume = _moveDistancePerFrame / 0.5f; //일정 이동 거리 이상이면 볼륨 1, 그 이하는 볼륨 점점 내려감
        _collide.Play(); //소리 재생
    }

    /* 끌리는 소리 */
    public void DraggingSound()
    {
        if (_drone._collide) //충돌 중이면
        {
            _dragging.volume = _moveDistancePerFrame / 0.5f; //일정 이동 거리 이상이면 볼륨 1, 그 이하는 볼륨 점점 내려감
            if (!_dragging.isPlaying) //소리 시작 안했으면
            {
                _dragging.Play(); //끌리는 소리 실행
            }
        }
        else
        {
            _dragging.Stop(); //끌리는 소리 중지
        }
    }

    /* 바람 소리 */
    public void WindSound()
    {
        _wind.volume = _moveDistancePerFrame / 2f; //일정 이동 거리 이상이면 볼륨 1, 그 이하는 볼륨 점점 내려감

        if (_moveDistancePerFrame < 0.2f) //프레임당 이동 거리가 0이면
        {
            _wind.Stop(); //바람 소리 정지
        }
        else //프레임당 이동 거리가 0이 아니면
        {
            if (!_wind.isPlaying) //바람 소리가 나지 않으면
            {
                _wind.Play(); //바람 소리 실행
            }
        }
    }
}
