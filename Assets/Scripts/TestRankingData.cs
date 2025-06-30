using UnityEngine;

// 작성자 : 김동균
// 테스트 랭킹 데이터 클래스
// 기능 : 테스트 랭킹 데이터 생성, 현재 게임 결과를 랭킹에 추가, 저장 경로 확인, 랭킹 데이터 초기화
public class TestRankingData : MonoBehaviour
{
    [Header("테스트 설정")]
    public bool generateTestDataOnStart = false;
    public int testDataCount = 10;
    
    void Start()
    {
        if (generateTestDataOnStart)
        {
            GenerateTestRankingData();
        }
    }
    
    [ContextMenu("테스트 랭킹 데이터 생성")]
    public void GenerateTestRankingData()
    {
        string[] testNames = {
            "Player1", "Warrior", "Sniper", "Hero", "Champion",
            "Legend", "Master", "Elite", "Pro", "Ace",
            "Dragon", "Phoenix", "Storm", "Blade", "Shadow"
        };
        
        for (int i = 0; i < testDataCount; i++)
        {
            ScoreDTO testScore = new ScoreDTO
            {
                playerName = testNames[Random.Range(0, testNames.Length)],
                kills = Random.Range(5, 100), // 5~99 킬
                time = Random.Range(120f, 600f), // 2~10분
                stage = Random.Range(1, 10), // 1~9 스테이지
                itemCollectCount = Random.Range(0, 20),
                boldnessRank = "A",
                timeTakenRank = "B", 
                itemCollectedRank = "C",
                totalRank = "B",
                starCount = 3,
                dateUtc = System.DateTime.UtcNow.AddDays(-Random.Range(0, 30)).ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            ResultSaver.SaveResult(testScore);
        }
        
        Debug.Log($"{testDataCount}개의 테스트 랭킹 데이터가 생성되었습니다.");
    }
    
    [ContextMenu("현재 게임 결과를 랭킹에 추가")]
    public void AddCurrentResultToRanking()
    {
        // latest_result.json에서 현재 게임 결과 로드
        string latestPath = System.IO.Path.Combine(Application.persistentDataPath, "latest_result.json");
        if (System.IO.File.Exists(latestPath))
        {
            string json = System.IO.File.ReadAllText(latestPath);
            ScoreDTO latestResult = JsonUtility.FromJson<ScoreDTO>(json);
            
            if (latestResult != null)
            {
                // 현재 랭킹 데이터 로드
                var currentRankings = ResultSaver.LoadAllResults();
                
                // 이미 동일한 결과가 있는지 확인 (중복 방지)
                bool alreadyExists = false;
                foreach (var ranking in currentRankings)
                {
                    if (ranking.playerName == latestResult.playerName && 
                        ranking.kills == latestResult.kills && 
                        ranking.dateUtc == latestResult.dateUtc)
                    {
                        alreadyExists = true;
                        break;
                    }
                }
                
                if (!alreadyExists)
                {
                    // 랭킹에 추가
                    ResultSaver.SaveResult(latestResult);
                    Debug.Log($"게임 결과를 랭킹에 추가했습니다: {latestResult.playerName}, 킬: {latestResult.kills}");
                }
                else
                {
                    Debug.Log("이미 랭킹에 추가된 결과입니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning("저장된 게임 결과가 없습니다.");
        }
    }
    
    [ContextMenu("저장 경로 확인")]
    public void ShowSavePath()
    {
        string path = Application.persistentDataPath;
        Debug.Log($"저장 경로: {path}");
        
        // 윈도우에서 탐색기로 폴더 열기
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", path.Replace('/', '\\'));
        #endif
    }
    
    [ContextMenu("랭킹 데이터 초기화")]
    public void ClearRankingData()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "rankings.json");
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("랭킹 데이터가 초기화되었습니다.");
        }
    }
} 