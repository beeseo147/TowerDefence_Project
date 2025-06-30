using System.Collections;
using UnityEngine;

// 작성자 : 김동균
// 게임 초기화 관리
// 기능 : 게임 시작 시 모든 매니저들 초기화
public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // 게임 시작 시 모든 매니저들 초기화
        InitializeAllManagers();
    }
    
    // 모든 Instance 초기화
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