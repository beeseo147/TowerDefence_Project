using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//인벤토리 내에서 Scene을 전환 하거나 종료하거나 HowtoPlay 창을 띄우는 버튼
public class InventoryButton : MonoBehaviour
{
    public GameObject howtoPlayUI;
    
    public void OnClickMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Lobby");
    }
    public void OnClickHowtoPlay()
    {
        StageManager.Instance.ClearUIReferences();
        ActiveHowtoPlay();
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
    void ActiveHowtoPlay()
    {
        print("HowtoPlayOpen");
        howtoPlayUI.SetActive(true);
    }
    void DeactiveHowtoPlay()
    {
        howtoPlayUI.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // 인벤토리 UI가 꺼질 때 HowToPlay UI도 함께 끄기
    void OnDisable()
    {
        if (howtoPlayUI != null && howtoPlayUI.activeSelf)
        {
            DeactiveHowtoPlay();
        }
    }

    // Update is called once per frame
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
