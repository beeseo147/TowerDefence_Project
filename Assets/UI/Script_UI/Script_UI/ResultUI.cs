using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Text playerNameText;
    public Text killCountText;
    public Text stageText;
    public Text lasTime;
    public GameObject[] totalRankText;
    public Text boldnessText;
    public Text timeTakenText;
    public Text itemCollectedText;
    public Text rankText;


    public void OnClickRankingButton()
    {
        LoadingManager.Instance.LoadSceneViaLoading("Ranking");
    }
    public void OnClickHomeButton()
    {
        LoadingManager.Instance.LoadSceneViaLoading("Lobby");
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        ScoreDTO data = ResultSaver.LoadResult();
        if (data != null)
        {
            playerNameText.text = data.playerName;
            killCountText.text = "ENEMY KILL : " + data.kills.ToString();
            stageText.text = "STAGE : " + data.stage.ToString();
            
            // 시간을 분:초 형식으로 변환
            int minutes = Mathf.FloorToInt(data.time / 60);
            int seconds = Mathf.FloorToInt(data.time % 60);
            lasTime.text = "TIME : " +$"{minutes:D2}:{seconds:D2}";
            
            boldnessText.text = "BOLDNESS : " + data.boldnessRank;
            timeTakenText.text = "TIME TAKEN : " + data.timeTakenRank;
            itemCollectedText.text = "UPGRADE : " + data.itemCollectedRank;
            rankText.text = new string('★', data.starCount); // 별 개수로 표시
            
            // TotalRank에 따라 해당 랭크 오브젝트 활성화
            SetTotalRankDisplay(data.totalRank);
        }
        else
        {
            Debug.LogWarning("저장된 결과 데이터가 없습니다.");
        }
    }
    
    private void SetTotalRankDisplay(string totalRank)
    {
        // 모든 랭크 오브젝트를 비활성화
        for (int i = 0; i < totalRankText.Length; i++)
        {
            if (totalRankText[i] != null)
            {
                totalRankText[i].SetActive(false);
            }
        }
        
        // totalRank에 따라 해당 오브젝트 활성화
        // 배열 순서: 0=S, 1=A, 2=B, 3=C, 4=D 등으로 가정
        int rankIndex = GetRankIndex(totalRank);
        if (rankIndex >= 0 && rankIndex < totalRankText.Length && totalRankText[rankIndex] != null)
        {
            totalRankText[rankIndex].SetActive(true);
        }
    }
    
    private int GetRankIndex(string rank)
    {
        switch (rank.ToUpper())
        {
            case "S": return 0;
            case "A": return 1;
            case "B": return 2;
            case "C": return 3;
            case "D": return 4;
            default: return -1; // 알 수 없는 랭크
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
