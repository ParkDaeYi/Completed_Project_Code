using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    /*
* date 2019.11.10
* author GS
* desc
* 동적으로 생성된 문 오브젝트의 여닫이가 가능하도록 하는 스크립트.
*/

    [Header("Raycast")]
    public RaycastHit[] _hits; //Ray와 충돌한 물체들
    public Vector3 _rayCastCenter; //레이캐스트 발사하는 위치
    public float _maxDistance; //최대 거리

    [Header("Component")]
    public Transform _doorParentTransform; //문의 부모 Transform
    public Transform _doorChildTransform; //문의 자식 Transform

    [Header("Variable")]
    public Transform _hitTransform; //부딪힌 물체의 Transform
    public float _rotationDegree; //회전해야하는 각도

    public void Start()
    {
        _maxDistance = 20f; //최대 거리 지정

        _doorParentTransform = GetComponent<Transform>(); //부모의 Transform 담음
        _doorChildTransform = GetComponent<Transform>().GetChild(0); //자식의 Transform 담음
    }

    public void Update()
    {
        _rayCastCenter = new Vector3(_doorParentTransform.position.x, 10f * _doorParentTransform.localScale.y, _doorParentTransform.position.z); //레이캐스트 발사 위치 갱신
        RotateDoor(); //문 회전

        Vector3 _boxSize = new Vector3(_doorParentTransform.localScale.x * 5f, _doorParentTransform.localScale.y * 5f, _doorParentTransform.localScale.z * 0.5f); //레이캐스트를 위한 박스 크기 지정
        _hits = Physics.BoxCastAll(_rayCastCenter, _boxSize, _doorParentTransform.forward, _doorParentTransform.rotation, _maxDistance); //앞 쪽에서 부딪힌 물체들의 정보를 담음
        for (int i = 0; !_hitTransform && i < _hits.Length; i += 1) //부딪힌 물체들을 탐색
        {
            if (_hits[i].transform.tag != "Door") //문이 아니면
            {
                _hitTransform = _hits[i].transform; //닿은 물체의 Transform 정보를 담음
                _rotationDegree = 90f; //회전 각도 지정
                break;
            }
        }
        _hits = Physics.BoxCastAll(_rayCastCenter, _boxSize, -_doorParentTransform.forward, _doorParentTransform.rotation, _maxDistance); //뒷 쪽에서 부딪힌 물체들의 정보를 담음
        for (int i = 0; !_hitTransform && i < _hits.Length; i += 1) //부딪힌 물체들을 탐색
        {
            if (_hits[i].transform.tag != "Door") //문이 아니면
            {
                _hitTransform = _hits[i].transform; //닿은 물체의 Transform 정보를 담음
                _rotationDegree = -90f; //회전 각도 지정
                break;
            }
        }

        if (_hitTransform) //닿은 물체가 있으면
        {
            if (Vector3.Distance(_hitTransform.position, _doorParentTransform.position) > _maxDistance) //거리가 멀어지면
            {
                _hitTransform = null; //닿은 물체의 Transform 제거
                _rotationDegree = 0f; //회전 각도 초기화

            }
        }
    }

    public void RotateDoor() //문을 회전하는 함수
    {
        float _x = _doorChildTransform.localEulerAngles.x; //X축 회전 각도
        float _z = _doorChildTransform.localEulerAngles.z; //Z축 회전 각도
        _doorChildTransform.rotation = Quaternion.Slerp(_doorChildTransform.rotation, Quaternion.Euler(_x, _doorParentTransform.localEulerAngles.y + _rotationDegree, _z), 2f * Time.deltaTime); //회전
    }
}