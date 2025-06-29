using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    // Stage에 따라 드론 생성 갯수 증가를 관리하기 위해
    // DroneManager 에게 현재 스테이지를 알려줌
    //스테이지별 최대 드론 갯수
    //일정 시간이 지나면 스테이지 변경
    public int stage = 1;
    public int maxStage = 10;
    public float stageTime = 20;
    public float currentTime = 0;
    public float NowTime = 0.0f;
    public Text NowTimeText;
    public Text StageText;
    public Text StageTextInInventory;
    public Action<int> onStageChange;
    
    // UI 업데이트를 위한 이벤트 추가
    public Action<int> onStageTextChanged;
    public Action<float> onTimeChanged;
    
    // 씬 전환 시 UI 컴포넌트 정리
    public void ClearUIReferences()
    {
        NowTimeText = null;
        StageText = null;
        StageTextInInventory = null;
    }
    
    // 씬 로드 이벤트 핸들러
    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"StageManager: 씬 로드됨 - {scene.name}");
        
        // 메인 게임 씬일 때만 UI 컴포넌트 찾기
        if (scene.name == "MainScenes")
        {
            StartCoroutine(FindUIComponentsAfterDelay());
        }
    }
    
    // 씬 로드 후 잠시 기다린 다음 UI 컴포넌트들 찾기
    private IEnumerator FindUIComponentsAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        FindUIComponents();
    }
    
    // UI 컴포넌트들을 이름으로 찾아서 할당
    private void FindUIComponents()
    {
        // NowTimeText 찾기
        if (NowTimeText == null)
        {
            GameObject timeObj = GameObject.Find("NowTimeText");
            if (timeObj != null)
            {
                NowTimeText = timeObj.GetComponent<Text>();
                Debug.Log("StageManager: NowTimeText 찾음");
            }
            else
            {
                Debug.LogWarning("StageManager: NowTimeText를 찾을 수 없습니다.");
            }
        }
        
        // StageText 찾기
        if (StageText == null)
        {
            GameObject stageObj = GameObject.Find("StageText");
            if (stageObj != null)
            {
                StageText = stageObj.GetComponent<Text>();
                Debug.Log("StageManager: StageText 찾음");
            }
            else
            {
                Debug.LogWarning("StageManager: StageText를 찾을 수 없습니다.");
            }
        }
        
        // StageTextInInventory 찾기
        if (StageTextInInventory == null)
        {
            GameObject stageInvObj = GameObject.Find("StageTextInInventory");
            if (stageInvObj != null)
            {
                StageTextInInventory = stageInvObj.GetComponent<Text>();
                Debug.Log("StageManager: StageTextInInventory 찾음");
            }
            else
            {
                Debug.LogWarning("StageManager: StageTextInInventory를 찾을 수 없습니다.");
            }
        }
        
        // UI 업데이트
        UpdateUITexts();
    }
    
    // UI 텍스트들 업데이트
    private void UpdateUITexts()
    {
        if (StageText != null)
            StageText.text = $"STAGE {stage}";
        if (StageTextInInventory != null)
            StageTextInInventory.text = $"STAGE {stage}";
    }
    
    // 게임 재시작 시 모든 데이터 초기화
    public void ResetGame()
    {
        Debug.Log("StageManager: 게임 데이터 초기화");
        
        // 스테이지 데이터 초기화
        stage = 1;
        currentTime = 0;
        NowTime = 0.0f;
        
        // UI 업데이트
        UpdateUITexts();
            
        Debug.Log("StageManager: 게임 데이터 초기화 완료");
    }

    void Awake()
    {
        // Singleton
        if (null != Instance && this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoadedEvent;
        
        // Text 컴포넌트들이 있을 때만 설정
        if (StageText != null)
            StageText.text = $"STAGE {stage}";
        if (StageTextInInventory != null)
            StageTextInInventory.text = $"STAGE {stage}";
    }
    
    void OnDestroy()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= stageTime)
        {
            NextStage();
            currentTime = 0;
        }
        NowTime = currentTime;
        
        // 이벤트로 시간 변경 알림 (전체 경과 시간)
        float totalElapsedTime = ScoreManager.Instance != null ? ScoreManager.Instance.ElapsedTime : 0f;
        onTimeChanged?.Invoke(totalElapsedTime);
        
        // Text 컴포넌트가 있을 때만 업데이트
        if (NowTimeText != null)
        {
            // 게임 전체 경과 시간을 표시 (ScoreManager의 ElapsedTime 사용)
            totalElapsedTime = ScoreManager.Instance != null ? ScoreManager.Instance.ElapsedTime : 0f;
            NowTimeText.text = $"{Mathf.FloorToInt(totalElapsedTime / 60):D2}:{Mathf.FloorToInt(totalElapsedTime % 60):D2}";
        }
    }
    
    [ContextMenu("NextStage")]
    public void NextStage()
    {
        stage++;    
        if (stage > maxStage)
        {
            stage = maxStage;
        }
        
        // 이벤트로 스테이지 변경 알림
        onStageTextChanged?.Invoke(stage);
        
        // UI 업데이트
        UpdateUITexts();
            
        onStageChange?.Invoke(stage);
        Debug.Log($"<color=yellow>StageManager NextStage() : stage {stage}</color>");
    }
}
