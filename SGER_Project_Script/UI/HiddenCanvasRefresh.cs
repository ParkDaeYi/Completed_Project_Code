using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenCanvasRefresh : MonoBehaviour {

    /* 원인불명으로 특정 UI Active해도 안 보일때 root Canvas false <-> true로 새로고침 해주기! */
    public void HiddenCanvasRefreshing()
    {
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }
}
