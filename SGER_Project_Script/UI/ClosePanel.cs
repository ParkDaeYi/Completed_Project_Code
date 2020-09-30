using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanel : MonoBehaviour {
    /**
* date 2018.10.27
* author INHO
* desc
* Object Panel, Scheduler Panel이 아닌 추가적인 패널에 따른
* close 버튼은 이 스크립트를 이용해 닫는다!
* (ex. 상세 동작 설정할 때 나오는 팝업창)
*/

    [Header("닫을 팝업창을 연결시켜 주세요.")]
    public GameObject _closeTarget; // 닫기 버튼을 클릭할 때, 닫을 Object.
	
    /* 닫기 버튼을 누를 때 실행되는 Method */
    public void OnCloseClick()
    {
        _closeTarget.SetActive(false);
    }
}
