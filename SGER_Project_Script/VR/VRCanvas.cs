using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCanvas : MonoBehaviour
{
    /*
    * date 2020.04.26
    * author GS
    * desc
    * VR을  환경에서 볼 수 있는 Canvas를 다루는 스크립트
    */

    [Header("UI")]
    public Text _dataBaseFileNameText; //데이터베이스 파일 이름 Text 컴포넌트

    [Header("DataBase")]
    public List<string> _saveFileName = new List<string>(); //Save 파일의 이름들을 담는 리스트
    public int _saveFileNameIndex; //현재 Save File의 인덱스 번호를 담는 변수

    [Header("Script")]
    public ButtonController _buttonController;
    public HiddenMenuControl _hiddenMenuControl;

    public IEnumerator Init() //초기화 함수
    {
        if(_saveFileName.Count > 0)
        {
            _saveFileNameIndex = 0; //인덱스 초기화
            _dataBaseFileNameText.text = _saveFileName[_saveFileNameIndex]; //첫 번째 파일 이름으로 지정
        }
        else
        {
            _saveFileNameIndex = -1; //인덱스 초기화
            _dataBaseFileNameText.text = "None"; //None 표시
        }

        yield return null;
    }

    public void OnClickPlayButton() //Play 버튼을 클릭하면 실행되는 함수
    {
        _buttonController.PlayButtonClick();
    }

    public void OnClickPreviousFileButton() //이전 파일 버튼을 클릭하면 실행되는 함수
    {
        _saveFileNameIndex = (_saveFileNameIndex + 1) % _saveFileName.Count; //인덱스 증가
        _dataBaseFileNameText.text = _saveFileName[_saveFileNameIndex]; //현재 인덱스의 파일 이름으로 텍스트 변경
    }

    public void OnClickNextFileButton() //다음 파일 버튼을 클릭하면 실행되는 함수
    {
        _saveFileNameIndex = _saveFileNameIndex == 0 ? _saveFileName.Count - 1 : _saveFileNameIndex - 1; //인덱스 감소
        _dataBaseFileNameText.text = _saveFileName[_saveFileNameIndex]; //현재 인덱스의 파일 이름으로 텍스트 변경
    }

    public void OnClickLoadFileButton() //파일을 불러오는 버튼을 클릭하면 실행되는 함수
    {
        if(_saveFileNameIndex != -1) //데이터베이스 파일이 존재하면
        {
            _hiddenMenuControl._filePath = Static.STATIC.dir_path + "/DataBase/" + _saveFileName[_saveFileNameIndex]; //데이터베이스 파일 경로 지정
            _hiddenMenuControl.OnclickLoadLoad(); //파일 로드
            transform.GetChild(1).gameObject.SetActive(false); // 데이터베이스 UI 안보이도록!
        }
    }
}