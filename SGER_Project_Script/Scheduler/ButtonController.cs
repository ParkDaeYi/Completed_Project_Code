using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * author : geun
 * content : 스케줄러에서 실행, 정지, 숨김등의 버튼을 컨트롤하는 스크립트
*/

public class ButtonController : MonoBehaviour
{
    [Header("메뉴바 버튼")]
    public GameObject _playButton; //플레이버튼
    public GameObject _resetButton; //리셋버튼
    bool _buttonSwitch; // false : 정지상태 true : 플레이상태

    [Header("Time")]
    public GameObject _cursor;
    public GameObject _minLine;
    TimeController _timeController; //TimeControl 변수 선언

    [Header("활성/비활성 UI")]
    public GameObject _clickedMenu;
    public GameObject _panelMenu;

    [Header("아이템 관리")]
    public ItemListControl _itemListControl;
    public ClickedItemControl _clickedItemControl;

    void Start()
    {
        _buttonSwitch = false;
        _timeController = GameObject.Find("SchedulerController").GetComponent<TimeController>();
    }

    public void PlayButtonClick()
    {
        _timeController._reset = false;
        if (_buttonSwitch) //현재 플레이상태 - 이미지 : play_btn로 바꿈
        {
            _playButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI_Image/Play");
            _timeController._sw = false;
            GameObject.Find("Cursor").GetComponent<CursorDrag>()._timeCheck = false;
        }
        else //현재 일시정지상태 - 이미지 : stop_btn로 바꿈
        {
            _playButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI_Image/Pause");
            _timeController._sw = true;
            GameObject.Find("Cursor").GetComponent<CursorDrag>()._timeCheck = true;
        }

        /**
       * @date 2019.01.15
       * @author INHO
       * @desc : 49~65줄 추가
       * Play 버튼을 누를때, UI 메뉴가 보이지 않도록 설정.
       * Stop 버튼을 누를때, UI 메뉴가 보이도록 설정.
       * */
        if (_buttonSwitch)
        {
            if (_clickedItemControl._clickedItem != null) //클릭된 아이템이 있으면
            {
                _clickedMenu.SetActive(true);
            }
            _panelMenu.SetActive(true);
        }
        else
        {
            _clickedMenu.SetActive(false);
            _panelMenu.SetActive(false);
            _clickedItemControl.BasePanelReset(); //Clicked 패널 Reset
        }

        _buttonSwitch = !_buttonSwitch;
    }

    public void ResetButtonOnClick()
    {
        _timeController._reset = true;
        _timeController._firstTime = false;
        GameObject.Find("Cursor").GetComponent<CursorDrag>()._timeCheck = false;
        _buttonSwitch = false;
        _timeController._sw = false; //시간 리셋버튼을 누르면 멈추고 초기화
        _timeController._time = 0;
        _playButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI_Image/Play");
        _timeController._timeText.text = "00:00.00"; //텍스트 초기화
        _cursor.transform.position = new Vector3(_minLine.transform.position.x, _cursor.transform.position.y, _cursor.transform.position.z);

        /* 모든 객체의 위치를 원래상태로 */
        foreach (Item A in _itemListControl._dataBaseHuman)
        {
            GameObject item = A.item3d.transform.parent.gameObject;
            //Debug.Log(item.gameObject.name);
            //Debug.Log(item.transform.GetChild(0).GetComponent<ItemObject>()._humanInitPosition);
            item.transform.position = item.transform.GetChild(0).GetComponent<ItemObject>()._humanInitPosition;
            item.transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 0f);
            //Debug.Log(item.transform.position);
            //Debug.Log(item.transform.GetChild(0).position);
            //Debug.Log(item.transform.GetChild(0).name);
            item.transform.GetChild(0).transform.localRotation = Quaternion.Euler(item.transform.GetChild(0).GetComponent<ItemObject>()._humanInitRotation);
        }

        /**
            * @date 2019.01.15
            * @author INHO
            * @desc : 89~97줄 추가
            * Reset 버튼을 누를 때, UI 메뉴가 보이도록 설정.
            * */
        if (_clickedItemControl._clickedItem != null)
        {
            _clickedMenu.SetActive(true);
            _panelMenu.SetActive(true);
        }
    }
}
