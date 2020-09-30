using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class ConnectProcess : MonoBehaviour
{
    /**
* date 2019.03.22
* author INHO
* desc
* 원하는 음성 텍스트를 입력후, 버튼을 누르면
* 자동적으로 음성합성이 진행되도록 구성.
*/

    [Header("Script")]
    public VoiceController _voiceController; // 음성 타입에 따라 모델이 바뀌도록!!
    public StartDBController _startDBController;

    Process cmd = new Process();
    public InputField _title;
    public InputField _sentence;
    public GameObject _progressImage;
    public GameObject _voiceButton;

    bool _isMakingVoice; // 음성을 만드는 중인지?
    int _countFiles; // 현재 폴더 파일 갯수 구하기.
    int currentCountFiles; // 실시간 폴더 파일 갯수 구하기. -> 갯수 달라짐으로 음성 만들어 졌는지 판단!

    string _dirPath;
    string title_str;

    void Start()
    {
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
        _dirPath = Static.STATIC.dir_path + "/DeepLeaning/samples";

        /* 현재 폴더에 존재하는 파일 갯수를 구한다. */
        _countFiles = getCountFiles();

        /* 외부 프로세서 실행시 보이지 않도록 설정. */
        //cmd.StartInfo.CreateNoWindow = true;
        //cmd.StartInfo.UseShellExecute = false;

        /* 외부프로세서 (cmd)를 지정하며, 하위 디렉토리를 설정한다. */
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.WorkingDirectory = Static.STATIC.dir_path + "/DeepLeaning";
        UnityEngine.Debug.Log(cmd.StartInfo.WorkingDirectory);
    }

    void Update()
    {
        /* 외부 프로세서가 겹치지 않도록 체크해줌. */
        CheckProcess();
    }

    /* 외부 프로세서를 이용해 동적으로 문장 만들어주는 함수. */
    public void MakingVoice()
    {

        if (_isMakingVoice) // 만들고 있을 때 누름 -> 취소해야 된다
        {
            CancelClick();
            return;
        }

        UnityEngine.Debug.Log(_title.text);

        StartDBController _sd = _startDBController;
        string dirPath = _sd.directoryPath[_sd._now].Substring(1, _sd.directoryPath[_sd._now].Length - 1);
        string model = dirPath.Contains("Woman") || dirPath.Contains("woman") ? "yuinna" : "son";

        /* 현재 존재하는 파일 갯수를 가져온다. */
        _countFiles = getCountFiles();

        /* 만들 텍스트를 입력하여 python 코드를 실행. -> 동적으로 생성됨. */
        cmd.StartInfo.Arguments = "/K python synthesizer.py --load_path logs/" + model + " --text \"" + _sentence.text + "\"";
        //cmd.StartInfo.Arguments = "/K dir";

        /* try-catch 문으로 잡아준 이유 : 최초 cmd 만들어 줄 시 해당 if 문이 아닌 오류에 걸리는 현상 발생! */
        try
        {
            /* cmd가 있을시! */
            if (!cmd.HasExited)
            {
                //UnityEngine.Debug.Log("음성 파일 생성중이니 조금만 기다려 주세요!");
            }
            /* cmd가 없을시! (2번째 음성 합성 이후 여기 걸림) */
            else
            {
                cmd.Start();
                _isMakingVoice = true;
            }
        }
        catch (System.Exception e)
        {
            /* 최초 cmd 생성 할때, 해당 구문에 걸린다! */
            cmd.Start();
            _isMakingVoice = true;
        }

        /* 음성합성이 끝나면, 텍스트 내용 초기화! */
        title_str = _title.text;
        _sentence.text = "";
        _title.text = "";

        /* Cancel <-> Make 버튼이 바뀌어야 한다! */
        ChangeButton();
    }

    public void CheckProcess()
    {

        if (_isMakingVoice)
        {
            /* 실시간으로 파일 갯수를 구한다. => 파일 갯수가 달라지면, 음성 파일이 만들어 졌음을 의미! */
            currentCountFiles = getCountFiles();
            //UnityEngine.Debug.Log("현재 음성 합성파일 생성 중!");
            //UnityEngine.Debug.Log("파일 갯수 : " + currentCountFiles);

            /* 프로그레스 (로딩) 이미지 활성화 */
            _progressImage.SetActive(true);

            /* 음성 합성시 그래프 + 음성파일 ==> 2개가 만들어 진다! */
            if (_countFiles + 2 <= currentCountFiles)
            {
                /* 외부 프로세서(cmd)를 종료시킨다. */
                cmd.CloseMainWindow();

                /* 프로그레스 (로딩) 이미지 비활성화 */
                _progressImage.SetActive(false);
                _isMakingVoice = false;

                /* Cancel <-> Make 버튼이 바뀌어야 한다! */
                ChangeButton();

                // 여기 수정
                /* 해당 생성된 파일들을 디렉터리 정리 및 동적 생성 */
                _voiceController.MoveVoiceFile(_dirPath, title_str);
            }

        }

    }

    /* 현재 폴더의 파일 갯수를 구하는 함수 */
    public int getCountFiles()
    {
        DirectoryInfo dir = new DirectoryInfo(_dirPath);
        FileInfo[] fileInfos = dir.GetFiles();
        return fileInfos.Length;
    }

    /* 취소 버튼을 눌렀을 시..! */
    public void CancelClick()
    {
        /* Python 프로그램은 cmd창 종료해도 지속되므로, 이름을 찾아가 종료 시켜야 한다! */
        Process[] processes = Process.GetProcessesByName("Python");
        foreach (Process pro in processes) { pro.CloseMainWindow(); }

        /* 외부 프로세서(cmd)를 종료시킨다. */
        cmd.CloseMainWindow();

        /* 프로그레스 (로딩) 이미지 비활성화 */
        _progressImage.SetActive(false);
        _isMakingVoice = false;

        /* Cancel <-> Make 버튼이 바뀌어야 한다! */
        ChangeButton();
    }

    /* Cancel <-> Make 버튼이 바뀌도록 구성. */
    public void ChangeButton()
    {
        Text text = this.transform.GetChild(0).gameObject.GetComponent<Text>();
        
        if (_isMakingVoice)
        {
            /* 버튼의 기능(함수)이 Make -> Cancel 로 바뀐다! */
            text.text = "Cancel";
        }
        else
        {
            /* 버튼의 기능(함수)이 Cancel -> Make 로 바뀐다!*/
            text.text = "Make";
        }

    }
}
