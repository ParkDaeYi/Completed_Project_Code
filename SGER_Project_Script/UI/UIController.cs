using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    /**
 * date 2018.07.12
 * author Lugub
 * desc
 *  모든 UI객체들을 여기에 저장시켜놓고 
 *  모든 UI On/Off를 여기서 하게끔 만들거임
 *  Canvas를 비롯해서 Scheduler까지
 *  
 *  대신 여기서 조정하는 모든 메뉴는 정적으로 사용되기 때문에
 *  GetComponent가 아니라 public을 사용한 Drag&Drop으로 구성할 예정
 *  더 나은 방식이 있을시 수정
 *  
 *  단 Hidden메뉴는 제외
 *  
 *  
 *  data 2018.08.20
 *  author INHO
 *  추가로, 마우스 상태를 나타내주는 기능 추가 (Raycast로 재배치 중이면 true , 아니면 false 반환)
 */
    [Header("UI")]
    public GameObject _itemMenuCanvas;
    public HiddenMenuControl _hiddenMenu;
    public GameObject _scheduler;
    public GameObject _objectMenu;
    public GameObject _teleport;

    [Header("ClickedItem")]
    public GameObject _clickedItemCanvas;

    [Header("MouseStatus")]
    public static bool _isMouseUsed=false; // 마우스 상태 표시 -> Raycast를 하고 있는지? 아닌지?

    [Header("Scripts")]
    public MenuMiniOption _menuMiniOption;

    public void OnclickItemMenu()

    {
        if (!_itemMenuCanvas.activeSelf) _itemMenuCanvas.SetActive(true);
        else if (_scheduler.activeSelf) _itemMenuCanvas.SetActive(true);
        else _itemMenuCanvas.SetActive(false);

        /* ItemMenuButton 텍스트 색상과 버튼 색상을 변경해주는 함수 */
        _menuMiniOption.OnClickItemMenu();
    }

    public void OnclickHiddenMenu()
    {
        _hiddenMenu.MenuCheck();
    }

    public void OnclickScheduler()
    {
        if (!_itemMenuCanvas.activeSelf) _itemMenuCanvas.SetActive(true);
        else if (_objectMenu.activeSelf) _itemMenuCanvas.SetActive(true);
        else _itemMenuCanvas.SetActive(false);

        /* SchedulerButton 텍스트 색상과 버튼 색상을 변경해주는 함수 */
        _menuMiniOption.OnClickSchedulerMenu();
    }

    public void TeleportONOFFButton()
    {
        _teleport.SetActive(!_teleport.activeSelf);
    }
}
