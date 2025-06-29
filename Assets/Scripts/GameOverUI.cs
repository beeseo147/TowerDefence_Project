using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverUI;
    // Start is called before the first frame update
    void Start()
    {
        gameOverUI.SetActive(false);
        float z = Camera.main.nearClipPlane + 3.0f;
        gameOverUI.transform.parent = Camera.main.transform;
        gameOverUI.transform.localPosition = new Vector3(0, 0, z); //UI 위치 카메라 근접 평면 위에 위치
        Tower.Instance.onTowerDestroy += GameOver;
    }
    
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        

        
        // ScoreManager에 결과 저장 (실시간으로 추적된 아이템 수집 횟수 사용)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveResult();
        }
        // 게임 오버창 띄운 후 일정 시간 경과 후 결과창 로드
        StartCoroutine(LoadResultAfterDelay());
    }
    
    private IEnumerator LoadResultAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        LoadingManager.Instance.LoadSceneViaLoading("Result");
        ItemObjectPool.Instance.ClearAllPools();
        ItemObjectPool.Instance.DestroyAllItems();
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("UI")))
        {
            // UI에 닿았을 때
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Camera.main.WorldToScreenPoint(hit.point)
            };

            // 클릭 등 이벤트 처리
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger)) // VR 컨트롤러 입력에 맞게 수정
            {
                ExecuteEvents.Execute(hit.collider.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
