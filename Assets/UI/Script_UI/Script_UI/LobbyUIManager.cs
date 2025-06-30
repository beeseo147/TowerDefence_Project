using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 작성자 : 김동균
// 로비 UI 매니저 클래스
// 기능 : 로비 UI 매니저 설정, 로비 UI 매니저 시작, 로비 UI 매니저 정지, 로비 UI 매니저 일시정지/재개, 로비 UI 매니저 최소값 설정, 로비 UI 매니저 최대값 설정, 로비 UI 매니저 변화 속도 설정, 로비 UI 매니저 현재 값 반환, 로비 UI 매니저 활성화 상태 반환, 로비 UI 매니저 바 이미지 설정, 볼륨 연동 기능 메서드들
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

    // 플레이어 이름 확인 버튼 클릭 시 플레이어 이름 저장
    public void OnClickPlayerNameOKButton()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        PlayerNameUI.SetActive(false);
    }
    // 시작 버튼 클릭 시 메인 씬 로드
    public void OnClickStartButton()
    {
        LoadingManager.Instance.LoadSceneViaLoading("MainScenes");
    }

    // 플레이어 이름 확인 버튼 클릭 시 플레이어 이름 저장
    public void OnClickHowToPlayButton()
    {
        HowToPlayUI.SetActive(true);
    }

    // 랭킹 버튼 클릭 시 랭킹 씬 로드
    public void OnClickRankingButton()
    {
        LoadingManager.Instance.LoadScene("Ranking");
    }

    // 종료 버튼 클릭 시 애플리케이션 종료
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    // 업데이트
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HowToPlayUI.SetActive(false);
        }
    }
}
