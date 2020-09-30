using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Item
{
    /**
* date 2018.07.12
* author Lugub
* desc
*  아이템의 정보값들을 다룸
*  itemName = 아이템의 이름
*  item3d   = 아이템의 객체
*  item2d   = 아이템의 메뉴 그림
*  objectNumber = 아이템의 생성 번호
*  OriginNumber = 아이템의 고유 번호
*  float x,y,z  = 아이템의 x,y,z 벡터값
*  float rot_x,y,z = 아이템의 회전값
*  
*  
*/

    public string itemName = null;
    public GameObject item3d; // 3d 객체
                              /**
                          * date 2018.07.18
                          * author Lugub
                          * desc
                          *  flot x,y,z rot_x,y,z를 전부 Vector3 으로 변환하고 
                          *  그와 관련된 모든 함수를 전부 Vector3 으로 변환 했음
*/

    /**
* date 2018.07.23
* author Lugub
* desc
*   Vector3 값인 _currentPlace변수와 _rotateValue를 삭제
*   삭제이유 3d객체가 있기 때문에 이 객체의 Transform값을 활용
*   이렇게 하면 위치를 수정했을 때 List의 값을 변경할 필요가 사라짐
*   
* date 2018.11.15
* author INHO
* desc
* Item.cs 는 사물 및 장소용 객체의 정보를 저장하는 Class
* HumanItem.cs 는 인물 객체의 정보를 저장하는 Class 로 구분.
*/
    public int _objectNumber;
    public int _originNumber;
    public int _loadHumanNumber; //로드 할때 스케줄러와 연결을 위한 값
    public string _status; // 사람 객체일때의 행동 모션을 나타내기 위한 Key값
    

    //fin
    /* 아이템 생성메뉴에서 호출 */
    /* ItemListControl.cs에서 호출 */
    public Item(string _itemName, GameObject _item3d, int _OriginNumber)
    {
        itemName = _itemName;
        item3d = _item3d;
        _originNumber = _OriginNumber;
    }



    /* 객체를 생성했을 때 호출 */
    /* LocateItem.cs와 연결 */
    public Item(string _itemName, int _objectNumber, int _originNumber, GameObject _item3d)
    {
        this.itemName = _itemName;
        this._objectNumber = _objectNumber;
        this._originNumber = _originNumber;
        this.item3d = _item3d;

        //string _path = "";
        //switch (_originNumber / 1000)
        //{
        //    case 1: _path = "Item/Items/Images/"; break;
        //    case 2: _path = "Item/Human/Images/"; break;
        //    case 3: _path = "Item/Place/Images/"; break;
        //}

        //item2d = Resources.Load<Sprite>(_path + _itemName);
    }

    public Item(Item _item)
    {
        this.itemName = _item.itemName;
        this._objectNumber = _item._objectNumber;
        this._originNumber = _item._originNumber;
        this.item3d = _item.item3d;
    }

}

