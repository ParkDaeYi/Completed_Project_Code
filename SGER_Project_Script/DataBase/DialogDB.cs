using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDB : MonoBehaviour {

    public DownloadDB _downloadDB;
    public GameObject _dialogObj;

    public void onClickDownloading()
    {
        _downloadDB.enabled = true;
        _downloadDB.setActiveProgress(true);
        _dialogObj.SetActive(false);
    }

    public void onClickNoDownloading()
    {
        this.gameObject.SetActive(false);
        _dialogObj.SetActive(false);
    }
}
