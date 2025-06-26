using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int   KillCount     { get; private set; }
    public float ElapsedTime   { get; private set; } // 경과시간
    public int   CurrentStage  { get; private set; }
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
    }

    void Update()
    {
        ElapsedTime += Time.deltaTime;
    }

    public void AddKill()
    {
        KillCount++;
    }
    public void SetStage(int num)
    {
        CurrentStage = num;
    }

    //public void SaveResult()
    //{
    //    var scoreData = new ScoreDTO
    //    {
    //        kills = KillCount,
    //        time = Mathf.RoundToInt(ElapsedTime),
    //        stage = CurrentStage,
    //        dateUtc = System.DateTime.UtcNow
    //    };
    //    RankingService.Save(scoreData);   // PlayerPrefs·JSON·서버 호출 등
    //}
}
