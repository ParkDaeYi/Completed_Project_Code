using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VoiceFileButton : MonoBehaviour
{
    [Header("UI")]
    public GameObject _playButton;
    public GameObject _renameField;
    public bool inputField_flag;

    [Header("Image")]
    public Sprite _playImage;
    public Sprite _stopImage;

    [Header("Script")]
    public VoiceController _voiceController;
    public StartDBController _startDBController;

    [Header("VoiceInfo")]
    public int _voiceType;
    public int _key;
    public string _file_name;

    private void Start()
    {
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    private void Update()
    {
        PlayButtonImageManagement();
    }

    /* PlayButton을 클릭했을 때 */
    public void OnClickPlayButton()
    {
        /* Audio Clip이 실행 중이면 */
        if (this.GetComponent<AudioSource>().isPlaying)
        {
            /* Audio Clip 종료 */
            this.GetComponent<AudioSource>().Stop();
        }
        /* Audio Clip이 실행 중이지 않으면 */
        else
        {
            /* Audio Clip 실행 */
            this.GetComponent<AudioSource>().Play();
        }
    }

    /* PlayButton 이미지 관리자 */
    public void PlayButtonImageManagement()
    {
        /* Audio Clip이 실행 중이면 */
        if (this.GetComponent<AudioSource>().isPlaying)
        {
            /* Play Image로 교체 */
            _playButton.GetComponent<Image>().sprite = _stopImage;
        }
        /* Audio Clip이 실행 중이지 않으면 */
        else
        {
            /* Stop Image로 교체 */
            _playButton.GetComponent<Image>().sprite = _playImage;
        }
    }

    public void OnClickVoiceBtn()
    {
        Static.STATIC._voicename = _file_name;
        Static.STATIC._dir_key = _key;
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

        FileInfo _file = new FileInfo(@"Assets/Resources/Voice" + _voiceDirectoryPath + "/" + gameObject.name); //본 파일 삭제

        if (_file.Exists) //파일 존재하면
        {
            _file.IsReadOnly = false; //읽기 전용 해제
            _file.Delete(); //삭제
        }

        _file = new FileInfo(@"Assets/Resources/Voice" + _voiceDirectoryPath + "/" + gameObject.name + ".meta"); //meta 파일 삭제

        if (_file.Exists) //파일 존재하면
        {
            _file.IsReadOnly = false; //읽기 전용 해제
            _file.Delete(); //삭제
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh(); //Asset 갱신
        Destroy(gameObject); //현재 Content 삭제
#endif
        _startDBController.DeleteFile(_key, gameObject.name);
    }
}