using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMenuController : MonoBehaviour {
    /*
* date 2018.08.20
* author INHO
* desc
* UI상에 나타나는 Menu들을 관리 (컨트롤) 해주는 스크립트.
* 주로 활성화 <-> 비활성화를 하기위해 만들어진 스크립트.
*/

    [Header("Scripts")]
    public MenuMiniOption _menuMiniOption;
    public MenuMiniOption _menuMiniOption2;
    
    private bool _isMouseStatus=false;

	void Update () {

        /* 마우스 동작 상태 바뀜! */
        if (UIController._isMouseUsed != _isMouseStatus)
        {
            OnController();
            _isMouseStatus = UIController._isMouseUsed;
        }

	}

    /* 해당 Method를 통해 메뉴창을 위, 아래로 이동시키는 함수 호출(전부 전용) */
    public void OnController()
    {
        if (_menuMiniOption) _menuMiniOption.OnClickMinimumButton();
        if (_menuMiniOption2) _menuMiniOption2.OnClickMinimumButton();
    }

    /* 클릭시 해당 UI의 ON/OFF 반전. */
    public void ActiveChange()
    {
        /* UI ON/OFF 기능 */
        if (_menuMiniOption) _menuMiniOption.gameObject.SetActive(!_menuMiniOption.gameObject.activeSelf);
        if (_menuMiniOption2) _menuMiniOption2.gameObject.SetActive(!_menuMiniOption2.gameObject.activeSelf);
    }
}
