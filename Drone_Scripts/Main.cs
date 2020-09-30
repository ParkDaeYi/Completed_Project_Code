using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    /* 메인 화면에서 시뮬레이션 화면으로 넘어가기 위한 스크립트 */
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Any)) //아무 키나 누르면
        {
            SceneManager.LoadScene(1); //첫 화면으로 이동
        }
    }
}
