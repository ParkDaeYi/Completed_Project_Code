using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBar : MonoBehaviour {

    /**
* date 2018.07.23
* author Lugub
* desc
*   Animation을 만들면 Bar가 생성되는데 그 Bar에 들어가는 함수
*   스케쥴러부분은 추후 만들예정
*/


    public GameObject _thisAniBar;

	// Use this for initialization
	void Start () {
        _thisAniBar = this.gameObject;	
	}
	
}
