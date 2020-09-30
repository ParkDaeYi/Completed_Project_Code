using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemMenuDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float _currentXPos; //// 현재 x좌표 저장하는 변수
    private float _currentYPos; //// 현재 x좌표 저장하는 변수
    public GameObject ItemCanvas;

    //드래그시작할때
    public void OnBeginDrag(PointerEventData eventData)
    {
        _currentXPos = Input.mousePosition.x;   // x축 이동할 것이기 때문에
        _currentYPos = Input.mousePosition.y;   // y축 이동할 것이기 때문에
        //throw new System.NotImplementedException();
    }

    //드래그중일때
    public void OnDrag(PointerEventData eventData)
    {
        float _moveX = Input.mousePosition.x - _currentXPos;
        float _moveY = Input.mousePosition.y - _currentYPos;
        ItemCanvas.transform.Translate(new Vector3(_moveX, _moveY, this.transform.position.z));
        //this.transform.Translate(new Vector3(_moveX, _moveY, 0)); //// 마우스의 이동만큼 드래그바이동
        // 다음 드래그때 이동 할 거리를 알기 위해 다시 현재 위치를 구한다.
        _currentXPos = Input.mousePosition.x;
        _currentYPos = Input.mousePosition.y;
    }

    //드래그종료할때
    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
