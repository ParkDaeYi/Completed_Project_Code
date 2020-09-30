using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchBackButton : MonoBehaviour {

    public StartDBController _startDBController;
    public CurrentSearch _currentSearch;
    public AllSearch _allSearch;
    public ScrollRect _scrollRect;

    private void Start()
    {
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
        _scrollRect = GameObject.Find("Canvas/ItemMenuCanvas/ClickedItemCanvas/VoiceCanvas/Scroll View").GetComponent<ScrollRect>();
    }

    public void OnClickCurrentBtn()
    {
        _currentSearch._current_content.gameObject.SetActive(false);
        _startDBController.contentInfo[_currentSearch._key].gameObject.SetActive(true);
        _scrollRect.content= _startDBController.contentInfo[_currentSearch._key].GetComponent<RectTransform>();
        _startDBController._pathText.text = _startDBController._filePath = _startDBController.directoryPath[_startDBController._now];
        _currentSearch._inputField.text = "";
    }

    public void OnClickAllBtn()
    {
        _allSearch._all_content.gameObject.SetActive(false);
        _startDBController.contentInfo[_allSearch._key].gameObject.SetActive(true);
        _scrollRect.content = _startDBController.contentInfo[_allSearch._key].GetComponent<RectTransform>();
        _startDBController._pathText.text = _startDBController._filePath = _startDBController.directoryPath[_startDBController._now];
        _allSearch._inputField.text = "";
    }


}
