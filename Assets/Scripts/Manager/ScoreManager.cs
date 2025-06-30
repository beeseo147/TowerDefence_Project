using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 작성자 : 김동균 윤여진
// 게임 결과 저장 및 랭크 계산 관리
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int   KillCount     { get; private set; } = 0;
    public int LastItemCount = 0;
    public float ElapsedTime   { get; private set; } // 게임 시간
    public int   CurrentStage  { get; private set; }
    public Text KillCountText;
    public int LastItemCollectCount { get; private set; }

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
        
        // UI 텍스트 초기화
        RefreshKillCountUI();
    }
    
    void OnDestroy()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }
    
    // 씬 로드 이벤트 핸들러
    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 로드됨: {scene.name}");
        
        // 메인 게임 씬일 때만 게임 데이터 초기화
        if (scene.name == "MainScenes")
        {
            ResetGame();
        }
        
        OnSceneLoaded();
    }

    void Update()
    {
        ElapsedTime += Time.deltaTime;
    }

    // KillCountText를 찾아서 할당하는 메서드
    // Scene이 변경됨에 따라 제거되는 UI를 찾아서 할당하는 메서드
    private void FindKillCountText()
    {
        if (KillCountText == null)
        {
            // 1. 정확한 이름으로 찾기
            GameObject killCountObj = GameObject.Find("Enemykill");
            if (killCountObj != null)
            {
                KillCountText = killCountObj.GetComponent<Text>();
                if (KillCountText != null)
                {
                    Debug.Log($"KillCountText 정확한 이름으로 할당: {killCountObj.name}");
                    return;
                }
            }
            
            // 2. 이름에 키워드가 포함된 것 찾기
            Text[] allTexts = FindObjectsOfType<Text>();
            foreach (Text textComponent in allTexts)
            {
                string objName = textComponent.gameObject.name.ToLower();
                if (objName.Contains("enemykill") || objName.Contains("killcount") || 
                    objName.Contains("enemy") && objName.Contains("kill"))
                {
                    KillCountText = textComponent;
                    Debug.Log($"KillCountText 키워드로 할당: {textComponent.gameObject.name}");
                    return;
                }
            }
            
            Debug.LogWarning("KillCountText를 찾을 수 없습니다. GameObject 이름을 'EnemyKillCountText'로 설정하거나 'EnemyKill' 키워드를 포함하도록 해주세요.");
        }
    }

    // UI 텍스트 업데이트 메서드
    // 적 처치 횟수를 표시하는 UI를 업데이트하는 메서드
    private void RefreshKillCountUI()
    {
        // KillCountText가 null이면 찾아서 할당
        if (KillCountText == null)
        {
            FindKillCountText();
        }
        
        // 여전히 null이 아니면 업데이트
        if (KillCountText != null)
        {
            KillCountText.text = "ENEMY KILLED: " + KillCount.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreManager: KillCountText를 찾을 수 없습니다.");
        }
    }

    // 적 처치 횟수를 증가시키는 메서드
    public void AddKill()
    {
        KillCount++;
        RefreshKillCountUI();
    }
    
    // 아이템 수집 횟수를 증가시키는 메서드
    public void AddItemCollection()
    {
        LastItemCollectCount++;
        Debug.Log($"아이템 수집 횟수 증가: {LastItemCollectCount}");
    }
    
    // 스테이지 번호를 설정하는 메서드
    public void SetStage(int num)
    {
        CurrentStage = num;
    }

    // 씬이 로드될 때 호출될 수 있는 메서드
    public void OnSceneLoaded()
    {
        KillCountText = null; // 기존 참조 초기화
        RefreshKillCountUI(); // 새로운 UI 찾아서 할당
    }

    // 아이템 수집 횟수를 저장하는 메서드
    // SaveResult() 메서드를 호출하여 결과를 저장
    public void SaveResult(int itemCollectCount)
    {
        LastItemCollectCount = itemCollectCount;
        SaveResult();
    }

    // 게임 결과를 저장하는 메서드
    // 플레이어명, 킬, 시간, 스테이지, 아이템 수집 횟수, 랭크, 별 개수, 날짜를 저장
    public void SaveResult()
    {
        // PlayerRuntimeStat에서 플레이어명 가져오기
        string playerName = PlayerPrefs.GetString("PlayerName", "Unknown Player");
        
        // 랭크 계산
        string boldnessRank = ResultCalculator.GetBoldnessRank(KillCount);
        string timeTakenRank = ResultCalculator.GetTimeTakenRank(ElapsedTime);
        string itemCollectedRank = ResultCalculator.GetItemCollectedRank(LastItemCollectCount);
        string totalRank = ResultCalculator.GetTotalRank(boldnessRank, timeTakenRank, itemCollectedRank);
        int starCount = ResultCalculator.GetStarCount(totalRank);

        var scoreData = new ScoreDTO
        {
            playerName = playerName,
            kills = KillCount,
            time = ElapsedTime, // 초 단위로 저장 (ResultUI에서 분:초로 변환)
            stage = CurrentStage,
            itemCollectCount = LastItemCollectCount,
            boldnessRank = boldnessRank,
            timeTakenRank = timeTakenRank,
            itemCollectedRank = itemCollectedRank,
            totalRank = totalRank,
            starCount = starCount,
            dateUtc = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
        
        ResultSaver.SaveResult(scoreData);
        
        // 분:초 형식으로 로그 출력
        int minutes = Mathf.FloorToInt(ElapsedTime / 60);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60);
        Debug.Log($"게임 결과 저장 완료 - 플레이어: {playerName}, 킬: {KillCount}, 시간: {minutes:D2}:{seconds:D2}, 총 랭크: {totalRank}");
    }

    // 게임 재시작 시 모든 데이터 초기화
    public void ResetGame()
    {
        Debug.Log("ScoreManager: 게임 데이터 초기화");
        
        // 모든 게임 데이터 초기화
        KillCount = 0;
        LastItemCount = 0;
        ElapsedTime = 0f;
        CurrentStage = 1;
        LastItemCollectCount = 0;
        
        // UI 업데이트
        RefreshKillCountUI();
        
        Debug.Log("ScoreManager: 게임 데이터 초기화 완료");
    }
}
