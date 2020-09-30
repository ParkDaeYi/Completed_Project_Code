using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenuControl : MonoBehaviour
{

    [Header("NULL이 있어도 무방함.")]
    [Header("ItemMenu")]
    public GameObject _button1;
    public GameObject _button2;
    public GameObject _button3;
    public GameObject _button4;
    public GameObject _button5;
    public GameObject _button6;
    public GameObject _button1Bar;
    public GameObject _button2Bar;
    public GameObject _button3Bar;
    public GameObject _button4Bar;
    public GameObject _button5Bar;
    public int _status;

    [Header("SearchMenu")]
    public InputField _inputField;
    public SearchInputField _searchInputField;

    /*
     * flag는 SearchInputField 클릭 시 
     * _categorie가 꺼지는 것을 방지하기 위해
     * 있는 bool 타입 변수이다.*
     */
    public bool _flag = false;
    public GameObject _categorie;
    public bool _switch = false;

    /**
* date 2018.07.17
* author Lugub
* desc
*  아이템 메뉴의 각 메뉴 버튼을 클릭했을 시
*  나오는 아이템 창이 변경되어야하기 때문에 이를 조정하기위한 함수들
*  
*  date 2018.11.09
*  author INHO
*  카테고리 항목 메뉴도 같은 기능을 하므로, 해당 스크립트에 추가
*  단, 카테고리 갯수가 비교적으로 많기 때문에 변수가 많아짐.
*/

    public void OnclickButton1()
    {
        ItemMenuClick(1);
    }

    public void OnclickButton2()
    {
        ItemMenuClick(2);
    }

    public void OnclickButton3()
    {
        ItemMenuClick(3);
    }

    public void OnclickButton4()
    {
        ItemMenuClick(4);
    }
    public void OnclickButton5()
    {
        ItemMenuClick(5);
    }
    public void OnclickButton6()
    {
        ItemMenuClick_InputField();
    }

    void ItemMenuClick_InputField()
    {
        AllFalse_InputField();
        _button6.SetActive(true);
        _inputField.gameObject.SetActive(true);
    }

    void ItemMenuClick(int swi)
    {
        /* 아이템 창이 변경되면 search 하는 Input Text를 초기화 시켜주기 */
        _inputField.text = "";

        AllFalse();

        /* 현재 어떤 UI 를 보여주는지 알 수 있도록 */
        _status = swi;
        _inputField.gameObject.SetActive(true);

        if (_categorie != null && _flag)
        {
            _categorie.SetActive(true);
        }
        switch (swi)
        {
            case 1:
                _button1.SetActive(true);
                _button1Bar.SetActive(true);
                break;
            case 2:
                _button2.SetActive(true);
                _button2Bar.SetActive(true);
                break;
            case 3:
                _button3.SetActive(true);
                _button3Bar.SetActive(true);
                break;
            case 4:
                _button4.SetActive(true);
                _button4Bar.SetActive(true);
                break;
            case 5:
                _button5.SetActive(true);
                _button5Bar.SetActive(true);
                break;
            default:
                break;
        }
    }
    //InputField의 경우
    void AllFalse_InputField()
    {
        _switch = false;
        if (_button1 != null)
        {
            _button1.SetActive(false);
        }

        if (_button2 != null)
        {
            _button2.SetActive(false);
        }
        if (_button3 != null)
        {
            _button3.SetActive(false);
        }
        if (_button4 != null)
        {
            _button4.SetActive(false);
        }
        if (_button5 != null)
        {
            _button5.SetActive(false);
        }
    }

    /* 전부 .SetActive(false) 하고 눌러진 것만 true 로 변경*/
    void AllFalse()
    {
        _switch = true;
        if (_button1 != null)
        {
            _button1.SetActive(false);
            _button1Bar.SetActive(false);
        }

        if (_button2 != null)
        {
            _button2.SetActive(false);
            _button2Bar.SetActive(false);

        }
        if (_button3 != null)
        {
            _button3.SetActive(false);
            _button3Bar.SetActive(false);
        }
        if (_button4 != null)
        {
            _button4.SetActive(false);
            _button4Bar.SetActive(false);
        }
        if (_button5 != null)
        {
            _button5.SetActive(false);
            _button5Bar.SetActive(false);
        }
        if (_button6 != null)
        {
            _button6.SetActive(false);
        }
    }//AllFalse Method End
}