using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Text;

public class AllSearch : MonoBehaviour
{
    /*
* date 2018.08.16
* author INHO
* desc
* Item Menu에서, Item 또는 Human의 Slot을 검색해 줄 수 있도록
* 지원해주는 Script !
*/

    public InputField _inputField;

    [Header("Script")]
    public CameraMoveAroun _cameraMoveAroundSwi;
    public StartDBController _startDBController;
    public CurrentSearch _currentSearch;

    public Dictionary<string, List<GameObject>> _fileTable;
    public int _key;
    public ScrollRect _scrollRect;
    public Transform _all_content;

    public int _flag;
    public bool _isReady = false;
    private const int child_size = 39;
    public static List<string> syn = new List<string>();

    private void Start()
    {
        _scrollRect = GameObject.Find("Canvas/ItemMenuCanvas/ClickedItemCanvas/VoiceCanvas/Scroll View").GetComponent<ScrollRect>();
        _cameraMoveAroundSwi = Static.STATIC.cameraMoveAroun;
        _startDBController = GameObject.Find("StartDBController").GetComponent<StartDBController>();
    }

    public class Trie
    {
        bool is_terminal;
        Trie[] children = new Trie[child_size];

        string alpha = "abcdefghijklmnopqrstuvwxyz";
        public Trie()
        {
            is_terminal = false;
            children.Initialize();
            //System.Array.Clear(children, '\0', child_size);
        }

        public void insert(string key, int idx)
        {
            if (key[idx] == '\0') is_terminal = true;
            else
            {
                int index;
                //알파벳
                if (97 <= key[idx] && key[idx] <= 122) index = key[idx] - 'a';
                else if (65 <= key[idx] && key[idx] <= 90) index = key[idx] - 'A';
                else if (key[idx] == '_') index = 36;
                else if (key[idx] == '.') index = 37;
                else if (key[idx] == ' ') index = 38;
                //숫자
                else index = key[idx] - '0' + 26;
                if (children[index] == null) children[index] = new Trie();

                children[index].insert(key, idx + 1);
            }
        }

        public void find(string key, int idx, string prev)
        {
            if (key[idx] == '\0')
            {
                depth(prev, "");
                return;
            }
            int index;
            //알파벳
            if (97 <= key[idx] && key[idx] <= 122) index = key[idx] - 'a';
            else if (65 <= key[idx] && key[idx] <= 90) index = key[idx] - 'A';
            //숫자
            else if (48 <= key[idx] && key[idx] <= 57) index = key[idx] - '0' + 26;
            else if (key[idx] == '_') index = 36;
            else if (key[idx] == '.') index = 37;
            else if (key[idx] == ' ') index = 38;
            //이외
            else return;
            if (children[index] == null) return;
            prev += key[idx];
            children[index].find(key, idx + 1, prev);
        }

        void depth(string head, string tail)
        {
            bool flag = false;

            for (int i = 0; i < child_size; ++i)
            {
                if (is_terminal && !flag)
                {
                    syn.Add(head + tail);
                    flag = true;
                }
                if (children[i] == null) continue;
                if (i < 26) tail += alpha[i];
                else if (i == 36) tail += '_';
                else if (i == 37) tail += '.';
                else if (i == 38) tail += ' ';
                else tail += (i - 26);
                children[i].depth(head, tail);
                tail = tail.Substring(0, tail.Length - 1);
            }
            return;
        }

        public void clear()
        {
            for (int i = 0; i < child_size; ++i)
            {
                if (children[i] == null) continue;
                children[i] = null;
            }
        }
    }

    public Trie root;

    // Update is called once per frame
    void Update()
    {

        /* Input Field 에 커서가 깜빡이면, W,A,S,D 를 눌러도 카메라 이동 안하도록 */
        if (_inputField.isFocused)
        {
            if (_currentSearch._inputField.text != "") _currentSearch._inputField.text = "";
            _cameraMoveAroundSwi._cameraAroun = false;
            _isReady = true;
        }
        if (_isReady && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            if (_currentSearch._current_content.gameObject.activeSelf) _currentSearch._current_content.gameObject.SetActive(false);
            _startDBController._pathText.text = "Search Results";
            _scrollRect.content = _all_content.GetComponent<RectTransform>();
            _all_content.gameObject.SetActive(true);
            _inputField.gameObject.SetActive(true);
            _key = _startDBController._now;
            Transform _content = _startDBController.contentInfo[_key];
            _content.gameObject.SetActive(false);

            foreach (string str in syn)
            {
                if (_fileTable.ContainsKey(str))
                {
                    foreach (GameObject g in _fileTable[str]) g.SetActive(false);
                }
            }
            syn.Clear();
            root.find(_inputField.text + '\0', 0, "");
            foreach (string str in syn)
            {
                Debug.Log(str);
                if (_fileTable.ContainsKey(str))
                {
                    foreach (GameObject g in _fileTable[str]) g.SetActive(true);
                }
            }
        }
        if (!_inputField.isFocused)
        {
            _isReady = false;
        }
    }

    public void ChangeText(Text txt)
    {
        txt.text = _inputField.text;
    }

}