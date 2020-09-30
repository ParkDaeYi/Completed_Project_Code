using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * content : 각 스케줄러의 관릴를 하는 스케줄러컨트롤러 스크립트
 */

public class SchedulerController : MonoBehaviour
{

    public GameObject _currentBigScheduler; //현재 오픈된 빅 스케줄러
    public ItemListControl _itemListControl;

    [Header("Hide Function")]
    public GameObject _smallContent;
    bool HideSwitch;
    [Header("DeleteObject")]
    public int _deleteOriginNumber;
    public int _deleteObjectNmber;
    public string _deleteObjectName;

    // Use this for initialization
    void Start()
    {
        HideSwitch = true; //처음시작할땐 숨김기능이 On
        _itemListControl = GameObject.Find("ItemController").GetComponent<ItemListControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AllHideButton()
    {
        int count = _smallContent.transform.childCount;
        Debug.Log(count);
        if (HideSwitch) //일시적으로 숨김기능 끄기
        {
            for (int i = 0; i < count; i++)
            {
                _smallContent.transform.GetChild(i).gameObject.SetActive(true);
            }
            HideSwitch = false;
        }
        else //숨김기능 사용
        {
            for (int i = 0; i < count; i++)
            {
                if (_smallContent.transform.GetChild(i).gameObject.GetComponent<SmallSchedulerBar>()._hideSwitch)
                {
                    _smallContent.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            HideSwitch = true;

        }
    }

    public void SchedulerDelete()
    {
        GameObject _deleteSmallScheduler = GameObject.Find("Canvas").transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.Find(_deleteObjectName).gameObject;
        GameObject _deleteBigScheduler = _deleteSmallScheduler.GetComponent<SmallSchedulerBar>()._bigScheduler;

        int idx = 0;
        while (idx < _itemListControl._dataBaseSmallAnimation.Count)
        {
            if (_deleteObjectNmber == _itemListControl._dataBaseSmallAnimation[idx]._item._objectNumber) _itemListControl._dataBaseSmallAnimation.RemoveAt(idx);
            else ++idx;
        }
        idx = 0;
        while (idx < _itemListControl._dataBaseSmallbar.Count)
        {
            if (_deleteSmallScheduler == _itemListControl._dataBaseSmallbar[idx]) _itemListControl._dataBaseSmallbar.RemoveAt(idx);
            else ++idx;
        }
        idx = 0;
        while (idx < _itemListControl._dataBaseBigBar.Count)
        {
            if (_deleteBigScheduler == _itemListControl._dataBaseBigBar[idx]) _itemListControl._dataBaseBigBar.RemoveAt(idx);
            else ++idx;
        }
        idx = 0;
        while (idx < _itemListControl._dataBaseBigAnimation.Count)
        {
            if (_deleteObjectNmber == _itemListControl._dataBaseBigAnimation[idx]._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber)
            {
                _itemListControl._dataBaseBigAnimation.RemoveAt(idx);
                _itemListControl._actionDBIndex--;
            }
            else ++idx;
        }
        idx = 0;
        while (idx < _itemListControl._dataBaseSmallVoice.Count)
        {
            if (_deleteObjectNmber == _itemListControl._dataBaseSmallVoice[idx]._item._objectNumber) _itemListControl._dataBaseSmallVoice.RemoveAt(idx);
            else ++idx;
        }
        idx = 0;
        while (idx < _itemListControl._dataBaseBigVoice.Count)
        {
            if (_deleteObjectNmber == _itemListControl._dataBaseBigVoice[idx]._smallAniBar.GetComponent<SmallAniBar>()._item._objectNumber)
            {
                _itemListControl._dataBaseBigVoice.RemoveAt(idx);
                _itemListControl._voiceDBIndex--;
            }
            else ++idx;
        }

        Destroy(_deleteSmallScheduler);
        Destroy(_deleteBigScheduler);
    }

}
