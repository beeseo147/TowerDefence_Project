using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// 작성자 : 김동균
// 게임 결과 저장 및 랭크 계산 관리
[System.Serializable]
public class ScoreDataList
{
    public List<ScoreDTO> scores = new List<ScoreDTO>();
}

// 작성자 : 김동균
// 게임 결과 저장 및 랭크 계산 관리
public static class ResultSaver
{
    // 랭킹 데이터 저장 경로 반환
    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "rankings.json");
    }
    
    // 최신 결과 저장 경로 반환
    private static string GetLatestResultPath()
    {
        return Path.Combine(Application.persistentDataPath, "latest_result.json");
    }

    // 게임 결과 저장
    public static void SaveResult(ScoreDTO data)
    {
        // 1. 가장 최근 결과를 별도 파일로 저장 (ResultUI용)
        SaveLatestResult(data);
        
        // 2. 랭킹 데이터에 추가
        List<ScoreDTO> allScores = LoadAllResults();
        allScores.Add(data);
        
        // 킬 수 기준으로 내림차순 정렬
        allScores = allScores.OrderByDescending(score => score.kills).ToList();
        
        // 최대 10개까지만 저장 (상위 10위)
        if (allScores.Count > 10)
        {
            allScores = allScores.Take(10).ToList();
        }
        
        ScoreDataList scoreList = new ScoreDataList();
        scoreList.scores = allScores;
        
        string json = JsonUtility.ToJson(scoreList, true);
        string path = GetSavePath();
        File.WriteAllText(path, json);
        Debug.Log($"결과 저장됨: {path}, 총 {allScores.Count}개 기록");
    }
    
    // 최신 결과 저장
    private static void SaveLatestResult(ScoreDTO data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetLatestResultPath();
        File.WriteAllText(path, json);
        Debug.Log($"최신 결과 저장됨: {path}");
    }

    // 최신 결과 로드
    public static ScoreDTO LoadResult()
    {
        // 가장 최근에 플레이한 결과 반환 (ResultUI용)
        string path = GetLatestResultPath();
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                ScoreDTO result = JsonUtility.FromJson<ScoreDTO>(json);
                Debug.Log($"최신 결과 로드됨: {result.playerName}, 킬: {result.kills}");
                return result;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"최신 결과 로드 실패: {e.Message}");
            }
        }
        
        // 최신 결과가 없으면 랭킹 1위 반환 (백업)
        List<ScoreDTO> allScores = LoadAllResults();
        return allScores.Count > 0 ? allScores[0] : null;
    }
    
    // 모든 결과 로드
    public static List<ScoreDTO> LoadAllResults()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                ScoreDataList scoreList = JsonUtility.FromJson<ScoreDataList>(json);
                return scoreList?.scores ?? new List<ScoreDTO>();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"랭킹 데이터 로드 실패: {e.Message}");
                return new List<ScoreDTO>();
            }
        }
        return new List<ScoreDTO>();
    }
    
    // 상위 랭킹 조회
    public static List<ScoreDTO> GetTopRankings(int count = 10)
    {
        List<ScoreDTO> allScores = LoadAllResults();
        return allScores.OrderByDescending(score => score.kills).Take(count).ToList();
    }
} 