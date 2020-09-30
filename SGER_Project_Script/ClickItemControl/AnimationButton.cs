using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimationButton : MonoBehaviour
{
    /*
   * date 2018.08.13
   * author INHO
   * desc
   *  동적으로 생성된 Animation 버튼을 눌렀을 때 사용
   *  각 Animation 버튼을 누르면, 해당 Animation 이나, 이동 함수가 실행되도록 구현.
   *  
   *  + Add(2019.01.18)
   *  author geun
   *  desc 
   *   각Animation 버튼을 눌렀을때 현재 클릭된 인물의 상세 스케줄러에 해당 전체 애니메이션이 추가
   */

    [Header("Scripts")]
    public ClickedItemControl _clickedItemControl;
    public AnimationMenuClick _animationMenuClick;
    public ItemListControl _itemListControl;

    [Header("Animation Information")]
    public Animator _animator;
    public GameObject _locatePrefab;

    [Header("Animation Create Information")]
    public GameObject _bigAniBarParent; //상세스케줄러 부모
    public GameObject _smallAniBarParent; //요약스케줄러 부모

    /* 동적인 부분이기 때문에 GameObject.Find사용*/
    public void Start()
    {
        _clickedItemControl = GameObject.Find("ClickedItemCanvas").GetComponent<ClickedItemControl>();
    }

    void Update()
    {
        /* 위치를 설정 시켜야 하는 경우에 Laycast 함수로 위치 설정 */
        if (_locatePrefab != null)
        {
            UIController._isMouseUsed = true;
            MoveCurrentPlacableObjectToMouse();
            if (Input.GetMouseButton(0)) Clicked();
        }
    }

    public void OnclickAnimationButton()
    {
        Debug.Log("AnimationButton.cs 55번째 줄 : 전체 동작 1");
        _animator = _clickedItemControl._clickedItem.item3d.GetComponent<Animator>();

        /* 움직여야 되는 함수면, Animation 재생 X -> 움직일 위치를 설정해야 하므로! */
        if (IsMoveObject()) return; // 런과 워크는 여기서 걸러짐

        /* 클릭된 애니메이션은 무슨 status(상태) 를 나타내는지? -> Key값으로 사용!! */

        //움직이지 않는 애니메이션(크라우치, 아이들 등)은 아래에서 처리 
        /*
           History
           date   : 2019.03.10
           author: Skyde47
           내  용: InDoor,OutDoor의 Canvas 구분을 둠
       */

        //빅애니바, 스몰애니바 생성
        CreateAniBar(false);

    }

    public void CreateAniBar(bool check)
    {

        _bigAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감

        /*빅애니바를 생성*/
        GameObject _bigAniBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
        _bigAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
        _bigAniBar.transform.GetChild(1).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //왼쪽드래그바의 색상 변경
        _bigAniBar.transform.GetChild(2).GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //오른쪽 드래그바의 색상 변경
        // 빅애니바의 정보를 저장함
        BigAnimationInfoSave(_bigAniBar.transform.GetComponent<BigAniBar>());
        string str = this.name;

        /*스몰애니바를 생성*/
        GameObject _smallAniBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject;
        _smallAniBar.GetComponent<Image>().color = new Color(196 / 255.0f, 244 / 255.0f, 254 / 255.0f); //빅애니바의 색상을 변경
        // 스몰애니바의 정보를 저장함
        SmallAnimationInfoSave(_smallAniBar.transform.GetComponent<SmallAniBar>(), check);

        /*스케줄러에 도달했으니 해당 사람의 스케줄바를 찾음*/
        if (_clickedItemControl._clickedItem._originNumber == 2001)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(4).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2000)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(4).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2002)
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(4).gameObject;
        }
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woongin" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(4).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;
        _bigAniBar.transform.GetComponent<BigAniBar>()._thisAniBar = _bigAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._thisAniBar = _smallAniBar;

        _itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
        Debug.Log(_itemListControl._actionDBIndex);

        _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName = _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName + _itemListControl._actionDBIndex.ToString();
        _bigAniBar.transform.GetComponent<BigAniBar>()._anibarName = _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName;
        _smallAniBar.gameObject.name = _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName;
        _bigAniBar.gameObject.name = _smallAniBar.gameObject.name;

        _bigAniBar.gameObject.transform.GetChild(0).GetComponent<Text>().text = _smallAniBar.gameObject.name;
        Debug.Log("AnimationButton.cs _anibarName : " + _smallAniBar.transform.GetComponent<SmallAniBar>()._anibarName);
        
        HistoryController.pushAniBarCreateHist(_bigAniBar, _smallAniBar, _smallAniBar.transform.GetComponent<SmallAniBar>()._animationName, _smallAniBar.gameObject.name, _clickedItemControl._clickedItem._objectNumber, 1);
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
        _info._layerNumber = 0;
        _info._actionOrFace = true; // 전체 행동이므로 true cf> false면 페이스
        _info._item = _clickedItemControl._clickedItem; //인물객체 저장
        _info._animator = _animator;
        _info._animationName = this.gameObject.name; //애니메이션 이름 저장
        _info._moveCheck = _moveCheck; //true면 이동하는 것, false면 이동하지않는 것
        _info._rotation = false;
        if (_moveCheck)
        {
            _info._arriveLocation = _locatePrefab.transform.position;
        }
    }
    public void BigAnimationInfoSave(BigAniBar _info)
    {
        _info._animationName = this.gameObject.name;

    }

    /* 인물의 Walk 나 Run 등으로 움직여야 되는 Animation 일시 움직이는 함수 적용 */
    public bool IsMoveObject()
    {
        if (this.gameObject.name == "Walk") { Move(); return true; }
        if (this.gameObject.name == "Run") { Move(); return true; }
        else return false;
    }

    /* 움직이는 함수 */
    public void Move()
    {
        /* 설정할 위치를 보여줄, Prefab 불러오기 */
        GameObject _prefab = Resources.Load<GameObject>("Animation/LocatePrefab/Locate");

        /* 불러온 Prefab을 복사하기 */
        _locatePrefab = Instantiate(_prefab) as GameObject;

        /* 클릭된 객체의 SpriteObject 비활성화 */
        _clickedItemControl._SpriteObject.SetActive(false);

        ///* 이동할 지점을 선택할 때 줌 아웃 해주기 -> 시야를 넓게 보여주기 위해 */
        //Static.STATIC.cameraMoveAroun.CameraZoomOut();
    }

    /* 마우스 포인터의 위치를 확정 지을 때! -> 클릭으로 그 곳으로 이동시키고 싶을 때 */
    private void Clicked() //런과 워크 상태일때 실행
    {
        /* 마우스 작업상태 -> false 로 전환해준다! */
        UIController._isMouseUsed = false;

        /* 클릭된 객체의 SpriteObject 활성화 */
        _clickedItemControl._SpriteObject.SetActive(true);

        /*
        * Date : 2018.11.12 
        * desc
        * 액션을 클릭했으므로 해당 액션을 애니메이션 바로 저장
        * 
        * Date : 2018.11.13
        * desc
        * 액션을 클릭했을때 애니메이션 바를 생성
        */

        //빅애니바, 스몰애니바 생성
        CreateAniBar(true);


        /*위치 조정을 마쳤으면 이제 null 및 Destory 선언으로 끊어준다.*/
        Destroy(_locatePrefab.gameObject);
        _locatePrefab = null;

    }

    /* 마우스로 위치를 조정하는 작업 */
    private void MoveCurrentPlacableObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo))
        {
            /* 해당 위치에 Human 또는 Item 있을 경우 그 위치로 조정 못하도록 */
            if (hitinfo.collider.tag != "Floor") return;

            Vector3 point = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z);
            //Vector3 point = new Vector3(hitinfo.point.x, hitinfo.point.y + (_placableGameObject.transform.localScale.y/2), hitinfo.point.z);
            _locatePrefab.transform.position = point;
            _locatePrefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
        }
    }

    public void AnimationAdd()
    {

    }

}
