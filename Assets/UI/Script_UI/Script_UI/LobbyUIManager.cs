using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public Button StartButton;
    public Button HowToPlayButton;
    public Button RankingButton;
    public Button ExitButton;

    public GameObject PlayerNameUI;
    public InputField playerNameInputField;
    public Button playerNameOKButton;
    public GameObject HowToPlayUI;


    public void OnClickPlayerNameOKButton()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        PlayerNameUI.SetActive(false);
    }
    public void OnClickStartButton()
    {
        LoadingManager.Instance.LoadSceneViaLoading("MainScenes");
    }

    public void OnClickHowToPlayButton()
    {
        HowToPlayUI.SetActive(true);
    }

    public void OnClickRankingButton()
    {
        LoadingManager.Instance.LoadScene("Ranking");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HowToPlayUI.SetActive(false);
        }
    }
}
