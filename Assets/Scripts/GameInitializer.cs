using System.Collections;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // 게임 시작 시 모든 매니저들 초기화
        InitializeAllManagers();
    }
    
    private void InitializeAllManagers()
    {
        Debug.Log("GameInitializer: 모든 매니저 초기화 시작");
        
        // ScoreManager 초기화
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetGame();
        }
        
        // StageManager 초기화
        if (StageManager.Instance != null)
        {
            StageManager.Instance.ResetGame();
        }
        
        Debug.Log("GameInitializer: 모든 매니저 초기화 완료");
    }
} 