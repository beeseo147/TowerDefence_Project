using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIManager : MonoBehaviour
{
    [Header("랭킹 UI 요소들")]
    public Transform rankingParent; // 랭킹 항목들이 들어갈 부모 Transform
    public GameObject rankingItemPrefab; // 랭킹 항목 프리팹
    
    [Header("랭킹 항목별 UI (프리팹이 없을 경우)")]
    public Text[] rankingNameTexts; // NAME 텍스트들
    public Text[] rankingKillsTexts; // KILLS 텍스트들  
    public Text[] rankingStageTexts; // STAGE 텍스트들
    
    [Header("버튼")]
    public Button homeButton;
    
    void Start()
    {
        LoadAndDisplayRankings();
        
        // 홈 버튼 이벤트 연결
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnClickHomeButton);
        }
    }
    
    void LoadAndDisplayRankings()
    {
        List<ScoreDTO> rankings = ResultSaver.GetTopRankings(10);
        
        if (rankings == null || rankings.Count == 0)
        {
            Debug.Log("저장된 랭킹 데이터가 없습니다.");
            DisplayEmptyRankings();
            return;
        }
        
        Debug.Log($"랭킹 데이터 로드됨: {rankings.Count}개");
        
        // 프리팹을 사용하는 경우
        if (rankingItemPrefab != null && rankingParent != null)
        {
            DisplayRankingsWithPrefab(rankings);
        }
        // 고정된 UI 요소를 사용하는 경우
        else if (rankingNameTexts != null && rankingKillsTexts != null && rankingStageTexts != null)
        {
            DisplayRankingsWithFixedUI(rankings);
        }
        else
        {
            Debug.LogError("랭킹 UI 요소들이 설정되지 않았습니다!");
        }
    }
    
    void DisplayRankingsWithPrefab(List<ScoreDTO> rankings)
    {
        // 기존 랭킹 항목들 제거
        foreach (Transform child in rankingParent)
        {
            Destroy(child.gameObject);
        }
        
        // 새로운 랭킹 항목들 생성
        for (int i = 0; i < rankings.Count; i++)
        {
            GameObject rankingItem = Instantiate(rankingItemPrefab, rankingParent);
            ScoreDTO score = rankings[i];
            
            // 랭킹 항목 UI 설정 (프리팹 내부 구조에 따라 수정 필요)
            Text nameText = rankingItem.transform.Find("NameText")?.GetComponent<Text>();
            Text killsText = rankingItem.transform.Find("KillsText")?.GetComponent<Text>();
            Text stageText = rankingItem.transform.Find("StageText")?.GetComponent<Text>();
            
            if (nameText != null) nameText.text = score.playerName;
            if (killsText != null) killsText.text = score.kills.ToString();
            if (stageText != null) stageText.text = score.stage.ToString();
        }
    }
    
    void DisplayRankingsWithFixedUI(List<ScoreDTO> rankings)
    {
        int maxDisplay = Mathf.Min(rankings.Count, rankingNameTexts.Length);
        
        // 랭킹 데이터 표시
        for (int i = 0; i < maxDisplay; i++)
        {
            ScoreDTO score = rankings[i];
            
            if (i < rankingNameTexts.Length && rankingNameTexts[i] != null)
                rankingNameTexts[i].text = score.playerName;
                
            if (i < rankingKillsTexts.Length && rankingKillsTexts[i] != null)
                rankingKillsTexts[i].text = score.kills.ToString();
                
            if (i < rankingStageTexts.Length && rankingStageTexts[i] != null)
                rankingStageTexts[i].text = score.stage.ToString();
        }
        
        // 나머지 빈 슬롯들 처리
        for (int i = maxDisplay; i < rankingNameTexts.Length; i++)
        {
            if (i < rankingNameTexts.Length && rankingNameTexts[i] != null)
                rankingNameTexts[i].text = "---";
                
            if (i < rankingKillsTexts.Length && rankingKillsTexts[i] != null)
                rankingKillsTexts[i].text = "0";
                
            if (i < rankingStageTexts.Length && rankingStageTexts[i] != null)
                rankingStageTexts[i].text = "0";
        }
    }
    
    void DisplayEmptyRankings()
    {
        if (rankingNameTexts != null)
        {
            for (int i = 0; i < rankingNameTexts.Length; i++)
            {
                if (rankingNameTexts[i] != null)
                    rankingNameTexts[i].text = "---";
                    
                if (i < rankingKillsTexts.Length && rankingKillsTexts[i] != null)
                    rankingKillsTexts[i].text = "0";
                    
                if (i < rankingStageTexts.Length && rankingStageTexts[i] != null)
                    rankingStageTexts[i].text = "0";
            }
        }
    }
    
    public void OnClickHomeButton()
    {
        LoadingManager.Instance.LoadSceneViaLoading("Lobby");
    }
    
    // 랭킹 새로고침 (게임 결과 저장 후 호출 가능)
    public void RefreshRankings()
    {
        LoadAndDisplayRankings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
