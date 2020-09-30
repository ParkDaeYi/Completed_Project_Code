using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionObject : MonoBehaviour {
    /*
* date 2019.11.10
* author GS
* desc
* 좌표 축 오브젝트를 카메라 회전에 맞추어 회전 시켜주는 스크립트.
*/
    public Transform _mainCamera; //메인 카메라
    public Transform _dimenstionObject; //3차원 축 오브젝트
	public void Update () {
        _dimenstionObject.rotation = Quaternion.Inverse(_mainCamera.rotation); //카메라와 회전 각도를 일치하도록 변경
	}
}