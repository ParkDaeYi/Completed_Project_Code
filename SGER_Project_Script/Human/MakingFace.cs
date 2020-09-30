using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @date 2019.01.10
 * @author 원준석
 * @desc 각 표정을 클릭했을때의 정보

 * @history
 *  + 
 */

public class MakingFace : MonoBehaviour
{
    [Header("BlendShapes")]
    public float[] _face = new float[4]; // 컴포넌트 위에서 부터 0~3

    [Header("Animation Create Information")]
    public GameObject _aniBarParent;
    public GameObject _bigAniBarParent; //상세스케줄러 부모
    public GameObject _smallAniBarParent; //요약스케줄러 부모

    [Header("Scirpts")]
    public ClickedItemControl _clickedItemControl;
    public ItemListControl _itemListControl;

    /* 객체의 표정 정보가 저장된 데이터 컴포넌트 */
    public SkinnedMeshRenderer _skinnedMesh;
    public GameObject _3dObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        Debug.Log("MakingFace.cs 50번째 줄 : 표정 3");
        /* 버튼 클릭시, ClickedItem에 저장된 객체를 가져온다. */
        _3dObject = _clickedItemControl._clickedItem.item3d;

        _aniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/BigScheduler"); //빅바스케줄러를 찾아감
        _smallAniBarParent = GameObject.Find("Canvas/ItemMenuCanvas/SchedulerMenu/Scroll View/Viewport/Content/SmallScheduler"); //스몰스케줄러를 찾아감

        GameObject _animationBar = Instantiate(Resources.Load("Prefab/Big")) as GameObject;
        _animationBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
        _animationBar.transform.GetChild(1).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
        _animationBar.transform.GetChild(2).GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);

        GameObject _smallAnimationBar = Instantiate(Resources.Load("Prefab/Small")) as GameObject; //스몰 애니메이션 바를 생성
        _smallAnimationBar.GetComponent<Image>().color = new Color((float)212 / 255, (float)238 / 255, (float)204 / 255);
        if (_clickedItemControl._clickedItem._originNumber == 2001)
        {
            _aniBarParent = _aniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Man" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(1).gameObject;
        }
        else if (_clickedItemControl._clickedItem._originNumber == 2000)
        {
            _aniBarParent = _aniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Daughter" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(1).gameObject;
        }
        else
        {
            _aniBarParent = _aniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(2).gameObject;
            _smallAniBarParent = _smallAniBarParent.transform.Find("Woman" + (_clickedItemControl._clickedItem._objectNumber)).transform.GetChild(1).gameObject;
        }
        Debug.Log("MakingFace.cs 66줄 애니바 부모 이름 = " + _aniBarParent.name);
        _animationBar.transform.SetParent(_aniBarParent.transform, false);
        _smallAnimationBar.transform.SetParent(_smallAniBarParent.transform, false);
        _animationBar.transform.localPosition = new Vector3(0, 0, 0); //위치 지정
        _smallAnimationBar.transform.localPosition = new Vector3(0, 0, 0); //새로운 부모에서의 로컬 위치 지정
        _animationBar.transform.GetComponent<BigAniBar>()._smallAniBar = _smallAnimationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._bigAniBar = _animationBar;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._layerNumber = 5;
        _animationBar.transform.GetChild(0).GetComponent<Text>().text = this.name;

        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._item = _clickedItemControl._clickedItem;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationName = this.gameObject.name;
        _smallAnimationBar.transform.GetComponent<SmallAniBar>()._animationBar = _animationBar; //smallAnimationBar는 animationBar에 의해서 움직이니 해당 애니메이션바를 저장

        for (int i = 0; i < 4; i++)
        {
            _smallAnimationBar.transform.GetComponent<SmallAniBar>()._face[i] = _face[i];
        }
        _itemListControl.AddActionDB(_animationBar.transform.GetComponent<BigAniBar>(), _smallAnimationBar.transform.GetComponent<SmallAniBar>());
        //HistoryController.pushAniBarCreateHist(_animationBar, _smallAnimationBar, this.gameObject.name, _clickedItemControl._clickedItem._objectNumber,1);
    }
}
