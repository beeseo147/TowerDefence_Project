using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Action<int> onStageChange;

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
        StageText.text = $"Stage {stage}";
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
        // 시간 표시 00(분):00(초) 형식으로 표시 
        NowTimeText.text = $"{Mathf.FloorToInt(NowTime / 60):D2}:{Mathf.FloorToInt(NowTime % 60):D2}";
        
    }
    
    [ContextMenu("NextStage")]
    public void NextStage()
    {
        stage++;    
        if (stage > maxStage)
        {
            stage = maxStage;
        }
        StageText.text = $"STAGE {stage}";
        onStageChange?.Invoke(stage);
        Debug.Log($"<color=yellow>StageManager NextStage() : stage {stage}</color>");
    }
}
