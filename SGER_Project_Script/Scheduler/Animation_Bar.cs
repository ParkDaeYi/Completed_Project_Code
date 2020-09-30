using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * content : 각 사람객체의 애니메이션 시작을 담당
 */

public class Animation_Bar : MonoBehaviour {
    [Header("Animation Time")]
    public double _startTime; //해당 애니메이션이 실행되는 시간
    public double _finishTime; //해당 애니메이션이 종료되는 시간

    [Header("ClickedItemCanvas")]
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
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000) _animator = _thisItem.item3d.GetComponent<Animator>();
        _cameraMoveAron = GameObject.Find("CameraController").GetComponent<CameraMoveAroun>();


        _startTime = 0;
        _finishTime = 10;
    }
	
	// Update is called once per frame
	void Update () {
        //RoofAnimation();
    }

    private void RoofAnimation()
    {
        if (_thisItem._originNumber >= 2000 && _thisItem._originNumber < 3000)
        {
            _animator.Play(_thisHuman._status);
        }
    }
}
