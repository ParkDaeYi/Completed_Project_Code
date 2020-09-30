using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpperAnimation : MonoBehaviour
{

    /**
* date 2018.08.21
* author INHO
* desc
* 중첩 애니메이션 등 다양한 동작을 동시 다발적으로 하기 위해
* 만든 스크립트로, 현재 테스트 단계로 추후 구현해야 된다!!
*  */

    [Header("Script")]
    public Animator _animator;
    public ClickedItemControl _clickedItemControl;
    public ItemListControl _itemListControl;

    [Header("Animation Create Information")]
    public GameObject _aniBarParent;
    public GameObject _bigAniBarParent; //상세스케줄러 부모
    public GameObject _smallAniBarParent; //요약스케줄러 부모

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /* 클릭하면 중첩 애니메이션 <-> 단일 애니메이션 전환 */
    public void OnClickAction()
    {

        Debug.Log("MultiAnimation.cs 32번째 줄 : 부분 동작 2");
        _animator = _clickedItemControl._clickedItem.item3d.GetComponent<Animator>();

        _aniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감

        GameObject _animationBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject; //애니메이션 바를 생성
        _animationBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        _animationBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        _animationBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);
        //보이스 색상은 202 233 189 로 지정

        GameObject _smallAnimationBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject; //스몰 애니메이션 바를 생성
        _smallAnimationBar.GetComponent<Image>().color = new Color((float)252 / 255, (float)198 / 255, (float)247 / 255);

        if (_clickedItemControl._clickedItem._originNumber == 2001)
        {
            _aniBarParent = _aniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(3).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2000)
        {
            _aniBarParent = _aniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(3).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2002)
        {
            _aniBarParent = _aniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(3).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
        }
        else
        {
            _aniBarParent = _aniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(3).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _animationBar.transform.SetParent(_aniBarParent.transform, false);
        _animationBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAnimationBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAnimationBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _animationBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAnimationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _animationBar;
        _animationBar.transform.GetComponent<BigAniBar>()._thisAniBar = _animationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAnimationBar;
        SmallAnimationInfoSave(_smallAnimationBar.GetComponent<SmallAniBar>(), false);

        _itemListControl.AddActionDB(_animationBar.transform.GetComponent<BigAniBar>(), _smallAnimationBar.transform.GetComponent<SmallAniBar>());
        Debug.Log(_itemListControl._actionDBIndex);

        _animationBar.transform.GetComponent<BigAniBar>()._animationName = _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationName;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._anibarName = _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationName + _itemListControl._actionDBIndex.ToString();
        _animationBar.transform.GetComponent<BigAniBar>()._anibarName = _smallAnimationBar.transform.GetComponent<SmallAniBar>()._anibarName;
        _smallAnimationBar.gameObject.name = _smallAnimationBar.transform.GetComponent<SmallAniBar>()._anibarName;
        _animationBar.gameObject.name = _smallAnimationBar.gameObject.name;

        _animationBar.transform.GetChild(0).GetComponent<Text>().text = _smallAnimationBar.gameObject.name;
        Debug.Log("UpperAnimation.cs _anibarName : " + _smallAnimationBar.transform.GetComponent<SmallAniBar>()._anibarName);

        HistoryController.pushAniBarCreateHist(_animationBar, _smallAnimationBar, _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationName, _animationBar.gameObject.name,
            _clickedItemControl._clickedItem._objectNumber, 1);

        /*
 History
 date   : 2018-11-26
 author : Lugup
 내  용 : AnimationStatus change
 실행시 : 현재 상태의 애니메이션을 바꿈
 취소시 : 현재 상태의 애니메이션을 바꿈

*/

    }

    /*
     * date : 2019.01.18
     * author : geun
     * parameter : 애니메이션바의 정보를 담은 스크립트 (각 애니메이션 바 당 1개씩 존재)
     * desc :
     * 애니메이션 바에 필요한 전체 행동에 대한 정보
     * _actionOrFace  // true면 단일행동 false면 페이스
     * _item  // 인물객체의 본체
     * _animator // 인물객체의 컴포넌트로 주로 애니메이션 실행 쪽을 담당
     * _status // 애니메이션 이름
     * _basicStatus //기본 애니메이션 (행동이므로 Idle, cf> 페이스였으면 올0)
     * _moveCheck //이동하는건지
     * _startLocation //처음위치
     * _arriveLocation //도착위치 && 바라볼방향(도착할 위치가 있으면 해당 방향을 바라보고 걷던 뛰던 한다.)
    */


    public void SmallAnimationInfoSave(SmallAniBar _info, bool _moveCheck) //_animationInfo에 정보를 저장하는 함수
    {
        _info._layerNumber = 2;
        _info._actionOrFace = true; // 전체 행동이므로 true cf> false면 페이스
        _info._item = _clickedItemControl._clickedItem; //인물객체 저장
        _info._animator = _animator;
        _info._animationName = this.gameObject.name; //애니메이션 이름 저장
        _info._moveCheck = _moveCheck; //true면 이동하는 것, false면 이동하지않는 것
        _info._rotation = false;
    }
}
