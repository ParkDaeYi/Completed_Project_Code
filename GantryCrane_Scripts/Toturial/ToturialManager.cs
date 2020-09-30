using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToturialManager : MonoBehaviour
{
    [Header("Connect Script")]
    public PlayableDirector Intro;
    public Text CurrentGoalText;
    public GameObject totalGoalObject;
    public CraneControl craneControl;
    public TruckController truckController;
    public GameObject AnimationObj;
    public Image leftHandImg;
    public Image rightHandImg;
    public GameObject exitDialog;
    public GameObject completeDialog;

    [Header("input 필요")]
    public string goKey;
    public string leftKey;
    public string rightKey;
    public string backKey;
    public string upKey;
    public string downKey;
    public string catchKey;
    public string missKey;

    KeyCode goKeyCode = Static._Wkey;
    KeyCode leftKeyCode = Static._Akey;
    KeyCode rightKeyCode = Static._Dkey;
    KeyCode backKeyCode = Static._Skey;
    KeyCode upKeyCode = Static._Upkey;
    KeyCode downKeyCode = Static._Downkey;
    KeyCode catchKeyCode = Static._Leftkey;
    KeyCode missKeyCode = Static._Rightkey;
    
    [Header("Resource")]
    public Sprite checkImg;


    [SerializeField()]
    int level;

    float stayTimer = 2.0f; // 꾸욱 눌러야 되는 시간
    float currentTimer; // 실제로 누른 시간
    void Start()
    {
        craneControl.setTutorial(true);
        Intro.Play();
        GoalSetting();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 버튼
        {
            exitDialog.SetActive(!exitDialog.activeSelf);
        }


        switch (level)
        {
            case 0:
                if (isWantkeyPush(leftKeyCode)) GoalSetting();
                break;
            case 1:
                if (isWantkeyPush(rightKeyCode)) GoalSetting();
                break;
            case 2:
                if (isWantkeyPush(goKeyCode)) GoalSetting();
                break;
            case 3:
                if (isWantkeyPush(backKeyCode)) GoalSetting();
                break;
            case 4:
                if (isWantkeyPush(downKeyCode)) GoalSetting();
                break;
            case 5:
                if (isWantkeyPush(upKeyCode)) GoalSetting();
                break;
            case 6:
                if (craneControl.getCatching()) GoalSetting();
                break;
            case 7:
                if (!craneControl.getCatching()) GoalSetting();
                break;
            case 8:
                if (truckController.TutorialComplete) GoalSetting();
                break;
        }
    }

    void setGoalText(string str)
    {
        CurrentGoalText.text = str;
    }

    void setGoalSpriteImg(string str,string str2)
    {
        if (str == null) str = "왼손";
        if (str2 == null) str2 = "오른손";

        leftHandImg.sprite = Resources.Load<Sprite>("Sprite/" + str);
        rightHandImg.sprite = Resources.Load<Sprite>("Sprite/" + str2);
    }

    void setGoalSpriteChecked(int position)
    {
        totalGoalObject.transform.GetChild(position).GetChild(1).GetComponent<Image>().sprite = checkImg;
    }

    void GoalSetting()
    {
        level++;
        switch (level)
        {
            case 0:
                setGoalText("'" + leftKey + "' 버튼을 이용해 갠트리 크레인을 왼쪽으로 움직이세요.");
                setGoalSpriteImg("왼손_왼", null);
                break;

            case 1:
                setGoalText("'" + rightKey + "' 버튼을 이용해 갠트리 크레인을 오른쪽으로 움직이세요.");
                setGoalSpriteImg("왼손_오른", null);
                break;

            case 2:
                setGoalText("'" + goKey + "' 버튼을 이용해 갠트리 크레인을 앞으로 움직이세요.");
                setGoalSpriteImg("왼손_위", null);
                break;

            case 3:
                setGoalText("'" + backKey + "' 버튼을 이용해 갠트리 크레인을 뒤로 움직이세요.");
                setGoalSpriteImg("왼손_아래", null);
                break;

            case 4:
                setGoalSpriteChecked(0);
                setGoalText("'" + downKey + "' 버튼을 이용해 갠트리 크레인 줄을 내리세요.");
                setGoalSpriteImg(null, "오른손_아래");
                break;

            case 5:
                setGoalText("'" + upKey + "' 버튼을 이용해 갠트리 크레인 줄을 올리세요.");
                setGoalSpriteImg(null, "오른손_위");
                break;

            case 6:
                setGoalSpriteChecked(1);
                setGoalText("갠트리 크레인과 줄을 움직여 '" + catchKey + "' 키를 이용해 컨테이너를 집으세요.");
                setGoalSpriteImg("왼손_전부", "오른손_왼");
                break;

            case 7:
                setGoalSpriteChecked(2);
                setGoalText("갠트리 크레인과 줄을 움직여 '" + missKey + "' 키를 이용해 컨테이너를 놓으세요.");
                setGoalSpriteImg(null, "오른손_오른");
                break;

            case 8:
                setGoalSpriteChecked(3);
                setGoalText("갠트리 크레인과 줄을 움직여 컨테이너를 컨테이너 트럭 위에 놓으세요.");
                setGoalSpriteImg("왼손_전부", "오른손_전부");
                break;
            case 9:
                setGoalSpriteChecked(4);
                setGoalText("");
                Complete();
                break;
        }
        if (level >= 1)
        {
            AnimationObj.SetActive(true);
            AnimationObj.GetComponent<UIAnimation>().StartAnimation("Spooting");
        }
    }

    bool isWantkeyPush(KeyCode keyCode)
    {
        bool isClicked = false;
        if (Input.GetKey(keyCode))
        {
            isClicked = true;
        }

        if (isClicked) currentTimer += Time.deltaTime;

        if (currentTimer > stayTimer)
        {
            currentTimer = 0f;
            return true;
        }
        else return false;
    }

    public void onClickExitScene()
    {
        SceneManager.LoadScene("Crane");
    }

    public void cancelDialog()
    {
        exitDialog.SetActive(false);
    }

    public void Complete()
    {
        completeDialog.SetActive(true);
    }


}
