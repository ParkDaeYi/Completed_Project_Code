using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VoiceDirectoryButton : MonoBehaviour
{
    public StartDBController _startDBController;
    public CurrentSearch _currentSearch;
    public AllSearch _allSearch;

    public ScrollRect _scrollRect;
    public GameObject _renameField;
    public bool inputField_flag;
    public int _key = 0;
    public int _prev_key = 0;
    public bool _change_btn = true;

    private void Start()
    {
        _allSearch = GameObject.Find("Canvas/ItemMenuCanvas/ClickedItemCanvas/VoiceCanvas/InsertAllPath/InputAllPath").GetComponent<AllSearch>();
        _currentSearch = GameObject.Find("Canvas/ItemMenuCanvas/ClickedItemCanvas/VoiceCanvas/InsertCurrentPath/InputCurrentPath").GetComponent<CurrentSearch>();
        _scrollRect = GameObject.Find("Canvas/ItemMenuCanvas/ClickedItemCanvas/VoiceCanvas/Scroll View").GetComponent<ScrollRect>();
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    public void OnClickBtn()
    {
        _currentSearch._current_content.gameObject.SetActive(false);
        _currentSearch._inputField.text = "";
        _allSearch._all_content.gameObject.SetActive(false);
        _allSearch._inputField.text = "";
        if (_change_btn)
        {
            _startDBController.contentInfo[_key].gameObject.SetActive(false);
            _startDBController.contentInfo[_prev_key].gameObject.SetActive(true);
            _startDBController._pathText.text = _startDBController._filePath = _startDBController.directoryPath[_prev_key];
            _startDBController._now = _prev_key;
            _scrollRect.content = _startDBController.contentInfo[_prev_key].GetComponent<RectTransform>();
        }
        else
        {
            _scrollRect.content = _startDBController.contentInfo[_key].GetComponent<RectTransform>();
            _startDBController.contentInfo[_key].gameObject.SetActive(true);
            _startDBController.contentInfo[_prev_key].gameObject.SetActive(false);
            _startDBController._pathText.text = _startDBController._filePath = _startDBController.directoryPath[_key];
            _startDBController._now = _key;
        }
    }

    public void OnClickRename()
    {
        _renameField.SetActive(!inputField_flag);
        inputField_flag = !inputField_flag;
    }

    /* 삭제 버튼을 클릭하면 실행되는 함수 */
    public void OnClickDelete()
    {
        string _voiceDirectoryPath;

        if (_startDBController._filePath.Length > 1) //경로 깊이가 1 이상일 때는
            _voiceDirectoryPath = _startDBController._filePath.Substring(1, _startDBController._filePath.Length - 1); //. 없애기
        else
            _voiceDirectoryPath = ""; //경로 없음

        string _folderPath = @"" + Static.STATIC.dir_path + "/Resources/Voice" + _voiceDirectoryPath + "/" + gameObject.name; //폴더 경로 담기

        DirectoryInfo _dir = new DirectoryInfo(_folderPath); //폴더 정보 담기
        if (_dir.Exists) //파일이 존재하면
        {
            DeleteDirectory(_folderPath); //하위 폴더 읽기 전용 해제
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh(); //Asset 갱신
        Destroy(gameObject); //현재 Content 삭제
#endif
        _startDBController.directoryPathIdx[_startDBController._now].Remove(_key);
        _startDBController.DeleteDirectory(_key);
    }

    /* 하위 폴더 읽기 전용 해제하는 함수 */
    private void DeleteDirectory(string _folderPath)
    {
        File.SetAttributes(_folderPath, FileAttributes.Normal); //폴더 읽기 전용 해제

        foreach (string _folder in Directory.GetDirectories(_folderPath)) //폴더 탐색
        {
            DeleteDirectory(_folder); //재귀 호출
        }

        foreach (string _file in Directory.GetFiles(_folderPath)) //파일 탐색
        {
            File.SetAttributes(_file, FileAttributes.Normal); //파일 읽기 전용 해제
            File.Delete(_file); //파일 삭제
        }

        Directory.Delete(_folderPath); //폴더 삭제
    }
}