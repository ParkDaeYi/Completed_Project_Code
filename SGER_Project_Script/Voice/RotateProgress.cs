using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateProgress : MonoBehaviour {
    /**
* date 2019.03.23
* author INHO
* desc
* 음성 합성 시, 적지 않은 시간이 걸리므로,
* 프로그레스 바(로딩) 이미지 활성 시 회전을 나타내는 스크립트.
*/

    Transform _progressTransform;
    // Use this for initialization
    void Start () {
        _progressTransform = this.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
        /* 매 프레임마다 회전한다! */
        _progressTransform.Rotate(0, 0, 2);
	}
}
