using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStart : MonoBehaviour {
    /*
     * date 2018.10.28
     * author geun
     * desc
     * 시간이 되었을때 해당 애니메이션을 실행시키는 스크립트
    */

    [Header("Clicked Item Canvas")]
    private ClickedItemControl _clickedItemControl;

    [Header("This Item")]
    public Item _thisItem;
    public HumanItem _thisHuman;
    public Animator _animator;

    [Header("CameraMove")]
    public CameraMoveAroun _cameraMoveAron;


    // Use this for initialization
    void Start () {
	    _clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();


        /* 사람 객체일 경우, Animator Controller 변수를 할당 해 주도록! */
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000)
        {
            _animator = _thisItem.item3d.GetComponent<Animator>();
            _cameraMoveAron = GameObject.Find("CameraController").GetComponent<CameraMoveAroun>();
        }
    }
	// Update is called once per frame
	void Update () {
        //시작시간 <= 현재시간 <= 종료 시간 : 해당 애니메이션을 반복적으로 실행
        //현재시간 < 시작시간 || 종료시간 < 현재시간 : 가만히 있는 애니메이션을 반복적으로 실행
        RoofAnimation();

    }
    /* 사람 객체가 동작을 가지면, 그 동작 반복해 주도록 설정! */
    private void RoofAnimation()
    {
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000)
        {
            _animator.Play(_thisHuman._status);
        }
    }

}
