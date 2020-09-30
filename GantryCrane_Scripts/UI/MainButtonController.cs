using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.UI;
using UnityEngine.SceneManagement;

public class MainButtonController : MonoBehaviour
{

    public PlayableDirector Intro;
    public PlayableDirector TruckIn;
    public Canvas canvas;
    public GameObject _mainCanvas;
    public CraneControl _craneControl;

    public List<Vector3> containerInfo = new List<Vector3>();
    public Vector3 gantryCraneInfo = new Vector3();
    public int containerCount;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _mainCanvas = canvas.transform.GetChild(0).gameObject;
        _mainCanvas.transform.Find("Retry").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReYes").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReNo").gameObject.SetActive(false);
        Static._play = false;

        containerInfo = _craneControl.containerInfo;
        gantryCraneInfo = _craneControl.gantryCraneInfo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Static._play && Input.GetKeyDown(KeyCode.Escape))
        {
            _mainCanvas.SetActive(!_mainCanvas.activeSelf);
            Static._play = false;
        }
        else if(!Static._play && Input.GetKeyDown(KeyCode.Escape))
        {
            _mainCanvas.SetActive(!_mainCanvas.activeSelf);
            Static._play = true;
        }
        if (_craneControl._containerSet.childCount <= 0 && Static._play)
        {
            _mainCanvas.SetActive(true);
            _mainCanvas.transform.GetChild(0).gameObject.SetActive(true);
            _mainCanvas.transform.Find("Retry").transform.GetChild(0).gameObject.SetActive(true);
            _mainCanvas.transform.Find("Retry").transform.GetChild(1).gameObject.SetActive(true);
            _mainCanvas.transform.Find("Start").gameObject.SetActive(false);
            _mainCanvas.transform.Find("Tutorial").gameObject.SetActive(false);
            _mainCanvas.transform.Find("Continue").gameObject.SetActive(false);
            _mainCanvas.transform.Find("Restart").gameObject.SetActive(false);
        }
    }

    public void OnClickStartButton()
    {    
        _mainCanvas.transform.Find("Start").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Tutorial").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Continue").gameObject.SetActive(true);
        _mainCanvas.transform.Find("Restart").gameObject.SetActive(true);
        _mainCanvas.SetActive(false);
        Static._play = true;
        Intro.Play();
        TruckIn.Play();
    }

    public void OnClickContinue()
    {
        _mainCanvas.SetActive(false);
        Static._play = true;
    }

    public void OnClickTutorial()
    {
        Static._play = true;
        SceneManager.LoadScene("Toturial");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickReYes()
    {
        StartCoroutine(ReStart());
        containerCount = containerInfo.Count;
        _craneControl.GantryCrane.position = gantryCraneInfo;
        _mainCanvas.transform.Find("Retry").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReYes").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReNo").gameObject.SetActive(false);
        _mainCanvas.SetActive(false);
    }

    public void OnClickReNo()
    {
        _mainCanvas.transform.Find("Retry").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReYes").gameObject.SetActive(false);
        _mainCanvas.transform.Find("Retry").transform.Find("ReNo").gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public IEnumerator ReStart()
    {
        Intro.Play();
        TruckIn.Play();
        while (_craneControl._containerSet.childCount > 0) {
            Destroy(_craneControl._containerSet.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < containerCount; i++)
        {
            GameObject tmp = (GameObject)Instantiate(Resources.Load("Prefabs/container"));
            tmp.name = "container " + i;
            tmp.transform.position = containerInfo[i];
            tmp.transform.SetParent(_craneControl._containerSet);
        }
        yield return null;
    }
}
