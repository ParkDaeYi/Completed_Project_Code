using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Categorie : MonoBehaviour {

    /**
*  date 2018.11.09
*  author INHO
*  카테고리 항목에 대한 아이콘 정렬 및 Item 버튼 클릭시
*  카테고리 항목 종류가 나오도록 설정해주는 스크립트.
*/
    [Header("Script")]
    public ItemMenuControl _itemMenuControl;

    [Header("Object")]
    public GameObject _ItemcategorieMenu;
    public GameObject _PlacecategorieMenu;

    [Header("변수값")]
    public int _status;

    void Start () {

	}
	
	void Update () {
        /* 보여질 ItemMenu중 Item(물건)에 관한 항목일때.. -> 카테고리 메뉴 표시! */
        _status = _itemMenuControl._status;

        /* 각 Menu를 누를때 마다, 해당 메뉴에 해당하는 카테고리가 보이도록 설정! */
        switch (_status) {
            case 1: // 아이템(Item) 에 대한 카테고리
                _ItemcategorieMenu.SetActive(true);
                _PlacecategorieMenu.SetActive(false);
                break;
            case 2: // 사람(Human) 은 카테고리 X -> 카테고리 메뉴 표시 X
                _PlacecategorieMenu.SetActive(false);
                _ItemcategorieMenu.SetActive(false);
                break;
            case 3: // 장소(Place) 에 대한 카테고리
                _PlacecategorieMenu.SetActive(true);
                _ItemcategorieMenu.SetActive(false);
                break;
        }//switch 

    }
}
