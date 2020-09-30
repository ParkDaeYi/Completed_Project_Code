using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanItem
{
    /**
* date 2018.11.16
* author INHO
* desc
* 필수적으로 필요한 아이템의 정보값은 Item.cs 에 저장.
* 해당 스크립트는 Human 에 적용될 추가적인 정보를 저장함.
* 따라서, 필수적인 정보 (Item.cs) 을 상속받고,
* 추가적인 옷, 모션 Key 값 등 정보를 저장해주기 위한 클래스.
* 
*/

    /* 사람 객체마다 적용 가능한 애니메이션 효과를 String 형식으로 Key 값 적용. */
    public string _status; // Action Key 값.
    public string _face;
    public string _arm;
    public string _leg;
    public string _voice;
    public int _humanNumber;
    public Vector3 _originPos;

    /* 사람 객체일 때의 Dress 정보 */
    public GameObject _shirt;
    public string _shirtName = null;
    public float _shirt_R;
    public float _shirt_G;
    public float _shirt_B;

    public GameObject _pant;
    public string _pantName = null;
    public float _pant_R;
    public float _pant_G;
    public float _pant_B;

    public GameObject _shoes;
    public string _shoesName = null;
    public float _shoes_R;
    public float _shoes_G;
    public float _shoes_B;

    /* handItem */
    public Item _leftHandItem = null;
    public Item _rightHandItem = null;

    /* Human 클래스의 생성자에서 Human 고유의 정보를 호출시켜 주어야 한다! */
    public HumanItem(string _status, int _humanNumber)
    {
        this._status = _status;
        this._humanNumber = _humanNumber;
    }

}