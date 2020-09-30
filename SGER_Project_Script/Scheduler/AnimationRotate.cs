using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationRotate : MonoBehaviour {

    /*
* date 2019.04.02
* author GS
* desc
* 정적으로 생성된 "Rotate" 애니메이션 버튼을 다루는 스크립트.
* 따로 애니메이션이 없기 때문에 스크립트로써 구분.
*/
    [Header("Animation Information")]
    public GameObject _locatePrefab;
    public ClickedItemControl _clickedItemControl;
    public ItemListControl _itemListControl;

    [Header("Animation Create Information")]
    public GameObject _bigAniBarParent; //상세스케줄러 부모
    public GameObject _smallAniBarParent; //요약스케줄러 부모

    /* 동적인 부분이기 때문에 GameObject.Find사용*/

    private void Start()
    {
        /* 위치 프리팹 초기화 */
        _locatePrefab = null;
    }

    private void Update()
    {
        /* 이동시킬 오브젝트가 존재 시 */
        if(_locatePrefab != null)
        {
            /* Ray 사용 */
            UIController._isMouseUsed = true;
            MoveCurrentPlacableObjectToMouse();
        }
    }

    /* 해당 버튼을 클릭하면 */
    public void OnclickAnimationButton()
    {
        Move();
    }

    /* 움직이는 함수 */
    private void Move()
    {
        /* 설정할 위치를 보여줄, Prefab 불러오기 */
        GameObject _prefab = Resources.Load<GameObject>("Animation/LocatePrefab/Locate");

        /* 불러온 Prefab을 복사하기 */
        _locatePrefab = Instantiate(_prefab) as GameObject;

        ///* 이동할 지점을 선택할 때 줌 아웃 해주기 -> 시야를 넓게 보여주기 위해 */
        //Static.STATIC.cameraMoveAroun.CameraZoomOut();
    }

    /* 마우스로 위치를 조정하는 작업 */
    private void MoveCurrentPlacableObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo))
        {
            /* 해당 위치에 Human 또는 Item 있을 경우 그 위치로 조정 못하도록 */
            if (hitinfo.collider.tag != "Plane") return;

            Vector3 point = new Vector3(hitinfo.point.x, hitinfo.point.y, hitinfo.point.z);
            _locatePrefab.transform.position = point;
            _locatePrefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);

            /* 왼쪽 마우스 버튼을 누르면 */
            if (Input.GetMouseButton(0))
            {
                /* 위치 Debug */
                Debug.Log("AnimationRotate.cs 73줄 / Rotate Position : " + point);

                CreateAniBar(true);

                /* 마우스 작업상태 -> false 로 전환해준다! */
                UIController._isMouseUsed = false;

                /*위치 조정을 마쳤으면 이제 null 및 Destory 선언으로 끊어준다.*/
                Destroy(_locatePrefab.gameObject);
                _locatePrefab = null;
            }
        }
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
        _bigAniBar.gameObject.transform.GetChild(0).GetComponent<Text>().text = str;

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
        else
        {
            _bigAniBarParent = _bigAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(5).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(4).gameObject;
        }

        /*빅애니바, 스몰애니바의 부모설정*/
        _bigAniBar.transform.SetParent(_bigAniBarParent.transform, false);
        _bigAniBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정
        _smallAniBar.transform.SetParent(_smallAniBarParent.transform, false);
        _smallAniBar.transform.localPosition = new Vector3(0, 0, 0); //부모를 지정해둔 뒤 위치를 새로 지정

        _bigAniBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAniBar;
        _smallAniBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _bigAniBar;


        _itemListControl.AddActionDB(_bigAniBar.transform.GetComponent<BigAniBar>(), _smallAniBar.transform.GetComponent<SmallAniBar>());
    }

    public void SmallAnimationInfoSave(SmallAniBar _info, bool _moveCheck) //_animationInfo에 정보를 저장하는 함수
    {
        _info._layerNumber = 0;
        _info._actionOrFace = true; // 전체 행동이므로 true cf> false면 페이스
        _info._item = _clickedItemControl._clickedItem; //인물객체 저장
        _info._animator = _clickedItemControl._clickedItem.item3d.GetComponent<Animator>();
        _info._animationName = this.gameObject.name; //애니메이션 이름 저장
        _info._moveCheck = _moveCheck; //true면 이동하는 것, false면 이동하지않는 것
        _info._rotation = true;
        if (_moveCheck)
        {
            _info._arriveLocation = _locatePrefab.transform.position;
        }
    }
    public void BigAnimationInfoSave(BigAniBar _info)
    {
        _info._animationName = this.gameObject.name;
    }
}
