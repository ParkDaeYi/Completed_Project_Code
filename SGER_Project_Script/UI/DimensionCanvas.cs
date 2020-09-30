using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionCanvas : MonoBehaviour
{
    /*
* date 2019.11.17
* author GS
* desc
* 좌표 축을 나타내는 이미지를 관리하는 스크립트.
* 이미지를 클릭했을 때 원하는 방향으로 화면 회전이 가능하도록 함.
*/
    [Header("Variable")]
    public bool _isClick; //축 이미지가 클릭되었는지 확인하는 변수
    public float _moveSpeed; //움직이는 속도
    public float _rotateSpeed; //회전 속도
    public Vector3 _centerAxis; //중심 축

    [Header("Object")]
    public Transform _object; //움직일 오브젝트

    public void Update()
    {
        if (_isClick) //클릭 되었을 땐
        {
            _object.LookAt(_centerAxis); //중심 축을 바라봄
            _object.Translate(-Input.GetAxis("Mouse X") * Time.deltaTime * _moveSpeed, -Input.GetAxis("Mouse Y") * Time.deltaTime * _moveSpeed, 0f); //카메라 이동
        }
    }

    public void ActivateClick() //클릭 활성화하는 함수
    {
        _isClick = true; //클릭됨
        _centerAxis = _object.position + _object.forward * 100f; //중심 축 지정
    }

    public void DectivateClick() //클릭 비활성화하는 함수
    {
        _isClick = false; //클릭되지 않음
    }
}