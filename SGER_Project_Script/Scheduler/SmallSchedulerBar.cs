using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * content : 스몰스케줄러바의 정보를 담고 있음
 */

public class SmallSchedulerBar : MonoBehaviour
{

    public GameObject _bigScheduler;
    public int _objectNumber; //생성된 순서
    public int _originNumber; //생성된 캐릭의 형식
    public int _humanNumber; //사람 번호(로드에 활용)
    public Item _object;
    public GameObject _hideButton; //숨김버튼
    public bool _hideSwitch;
    ClickedItemControl _clickedItemControl;


    // Use this for initialization
    void Start()
    {
        _hideSwitch = false; //false면 눈에 보임, true면 눈에 안보임
        _clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void HideButtonClick()
    {
        if (_hideSwitch)
        {
            _hideButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI_Image/show_on");
            _hideSwitch = false;
        }
        else
        {
            _hideButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI_Image/show_off");
            _hideSwitch = true;
            this.gameObject.SetActive(false);
        }
    }

    public void SmallBarClick()
    {
        if (GameObject.Find("SchedulerController").GetComponent<SchedulerController>()._currentBigScheduler)
        {
            GameObject.Find("SchedulerController").GetComponent<SchedulerController>()._currentBigScheduler.SetActive(false);
        }
        GameObject.Find("SchedulerController").GetComponent<SchedulerController>()._currentBigScheduler = _bigScheduler;
        GameObject.Find("SchedulerController").GetComponent<SchedulerController>()._currentBigScheduler.SetActive(true);


        /* 19-04-01 인호 추가 */
        _clickedItemControl._clickedItem = _object;
        _clickedItemControl.ClickMenuActivate();
    }
}
