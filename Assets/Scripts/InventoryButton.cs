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
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }
    public void OnClickHowtoPlay()
    {
        ActiveHowtoPlay();
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
    void ActiveHowtoPlay()
    {
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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || gameObject.activeSelf == true)
        {
            if(howtoPlayUI.activeSelf)
            {
                DeactiveHowtoPlay();
            }
        }
    }
}
