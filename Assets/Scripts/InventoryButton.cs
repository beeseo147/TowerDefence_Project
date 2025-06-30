using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//인벤토리 내에서 Scene을 전환 하거나 종료하거나 HowtoPlay 창을 띄우는 버튼
// 작성자 : 김동균
// 인벤토리 버튼 클래스
// 기능 : 인벤토리 메인 메뉴 클릭, 인벤토리 플레이 클릭, 인벤토리 종료 클릭, 인벤토리 플레이 활성화, 인벤토리 플레이 비활성화
public class InventoryButton : MonoBehaviour
{
    public GameObject howtoPlayUI;
    
    // 인벤토리 메인 메뉴 클릭
    public void OnClickMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Lobby");
    }
    // 인벤토리 플레이 클릭
    public void OnClickHowtoPlay()
    {
        StageManager.Instance.ClearUIReferences();
        ActiveHowtoPlay();
    }
    // 인벤토리 종료 클릭
    public void OnClickExit()
    {
        Application.Quit();
    }
    // 인벤토리 플레이 활성화
    void ActiveHowtoPlay()
    {
        print("HowtoPlayOpen");
        howtoPlayUI.SetActive(true);
    }
    // 인벤토리 플레이 비활성화
    void DeactiveHowtoPlay()
    {
        howtoPlayUI.SetActive(false);
    }
    // 인벤토리 버튼 시작
    void Start()
    {
        
    }

    // 인벤토리 UI가 꺼질 때 HowToPlay UI도 함께 끄기
    // 인벤토리 버튼 비활성화
    void OnDisable()
    {
        if (howtoPlayUI != null && howtoPlayUI.activeSelf)
        {
            DeactiveHowtoPlay();
        }
    }

    // 인벤토리 버튼 업데이트
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(howtoPlayUI.activeSelf)
            {
                DeactiveHowtoPlay();
            }
        }
    }
}
