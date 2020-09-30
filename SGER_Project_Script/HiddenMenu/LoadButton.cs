using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{/*
   * date 2018.07.17
   * author Lugub
   * desc
   *  OnclickDBbtn.cs 참조
   *  
   *  동적생성된 DB버튼에 자동 참조되어있는 스크립트 
   *  동적이기 때문에 GameObject.Find 사용
   *  
   *  HiddenMenuControl.cs 동적으로 연결함
   */
    HiddenMenuControl _hiddenMenuControl;
    public GameObject _renameField;
    string dir_path;
    public bool inputField_flag;

    void Start()
    {
        dir_path = Static.STATIC.dir_path;
        _hiddenMenuControl = GameObject.Find("HiddenMenu").GetComponent<HiddenMenuControl>();
    }

    /* 버튼 클릭 했을 경우 HiddenMenuControl.cs의 _filePath변수를 경로 + 버튼의 이름으로 만들어줌 */
    public void OnclickLoad()
    {
        //_hiddenMenuControl._filePath = "Assets/DataBase/" + this.gameObject.name + ".db";
        //_hiddenMenuControl._filePath = "Assets/DataBase/" + this.gameObject.transform.GetChild(0).GetComponent<Text>().text;
        _hiddenMenuControl._filePath = dir_path + "/DataBase/" + this.gameObject.transform.name;
    }

    public void OnclickSave()
    {
        //_hiddenMenuControl._filePath = "Assets/DataBase/" + this.gameObject.name + ".db";
        //_hiddenMenuControl._filePath = "Assets/DataBase/" + this.gameObject.transform.GetChild(0).GetComponent<Text>().text;
        //_hiddenMenuControl._saveText.GetComponent<Text>().text = this.gameObject.transform.GetChild(0).GetComponent<Text>().text;
        _hiddenMenuControl._filePath = dir_path + "/DataBase/" + this.gameObject.transform.name;
        Static.STATIC._saveClickButton = this.gameObject;
    }

    public void OnValueChanged()
    {
        Static.STATIC._saveClickButton = null;
    }

    public void OnClickDelete()
    {
        _hiddenMenuControl._deleteFile = this.gameObject;
        _hiddenMenuControl._deleteFileName = this.gameObject.name;
        _hiddenMenuControl.OnClickDeleteDialog();
        Static.STATIC._saveClickButton = null;
    }
    public void OnClickRename()
    {
        _renameField.SetActive(!inputField_flag);
        inputField_flag = !inputField_flag;
    }
}
