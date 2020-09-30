using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

public class DownloadDB : MonoBehaviour {

    [Header("입력값")]
    public string _downURL;


    public GameObject _progress;
    public Text totalText; // 전체 파일 진행도
    public Text currentText; // 현재 파일 진행도

    FileInfo _fileinfo; // 파일 존재 유무 확인용
    List<string> _files; // 파일 항목들

    int downloadCnt;
    string dir_path;
    // Use this for initialization
    void Start () {
        dir_path = Static.STATIC.dir_path;
        _files = new List<string>();
        checkFileList();
        DownloadFile();
    }
	
    public void checkFileList() // 해당 url에 존재하는 하위 디렉토리 가져옴 (-> load 를 여러개 구현하기 위해)
    {
        Uri uri = new Uri(_downURL);
        WebRequest webRequest = WebRequest.Create(uri);
        WebResponse webResponse = webRequest.GetResponse();

        StreamReader reader = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.Default); // 한글도 지원해주기 위해!
        string strData = reader.ReadToEnd();
        string[] dir = strData.Split('\"'); // " 기준으로 split 수행

        for(int i = 5; i < dir.Length; i+=2)
        {
            _files.Add(dir[i]); // 파일 이름 있는 string 값만 정제하기
        }
        webResponse.Close();
    }

    public void DownloadFile() // 파일 다운로드 하기
    {
        Uri uri = null;
        try
        {
            foreach (string file in _files)
            {

                _fileinfo = new FileInfo(dir_path + "/Database/" + file); // 하위 디렉토리로부터 하나씩 정보 가져옴
                if (_fileinfo.Exists) // 이미 존재하는 파일이면 덮어쓰기
                {
                    _fileinfo.Delete();
                    Debug.Log("이미 있는 파일! -> 최신화 하자 : " + _fileinfo.Name);
                }
                uri = new Uri(_downURL + file);
                WebClient webClient = new WebClient(); // 파일 1개당 하나의 객체가 담당함

                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                webClient.DownloadFileAsync(uri, dir_path + "/DataBase/" + file); // 프로그레스를 설정하기 위해 비동기 방식 채택
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompletedCallback);
            }
        }
        catch (Exception e)
        {
            Debug.Log("오류발생! " + e);
        }

    }

    void DownloadProgressCallback(System.Object sender, DownloadProgressChangedEventArgs e)
    {
        Debug.Log("프로그레스 : " + (string)e.UserState+"/"+ e.BytesReceived + "/" + e.TotalBytesToReceive + "/" + e.ProgressPercentage);
        currentText.text = "현재 진행도 : " + e.ProgressPercentage + " %";
    }

    void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        downloadCnt++;
        totalText.text = "전체 진행도 : " + downloadCnt + " / " + _files.Count;

        if (downloadCnt == _files.Count)
        {
            setActiveProgress(false);
        }
    }

    public void setActiveProgress(bool swi)
    {
        _progress.SetActive(swi);
        totalText.gameObject.SetActive(swi);
        currentText.gameObject.SetActive(swi);
    }
}
