using System;
// 작성자 : 김동균
// 점수 정보를 저장하는 클래스
[Serializable]
public class ScoreDTO
{
    public string playerName;
    public int kills;
    public int stage;
    public float time;
    public int itemCollectCount;
    public string boldnessRank;
    public string timeTakenRank;
    public string itemCollectedRank;
    public string totalRank;
    public int starCount;
    public string dateUtc;
} 