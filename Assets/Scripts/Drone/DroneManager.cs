using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    //드론의 생성시간 간격을 1초~5초
    public float minTime = 1;
    public float maxTime = 5;
    float createTime; //생성 시간 간격
    float currentTime; //현재 시간

    public Transform[] spawnPoints; //드론의 스폰 포인트
    public GameObject[] droneFactory; //드론 프리팹
    public StageManager stageManager; //스테이지 관리 스크립트

    private List<int> currentDroneIndex = new List<int>();

    private Dictionary<int, int[]> stageDroneMap = new Dictionary<int, int[]>()
    {
        { 1, new int[] { 0 } },              // 근거리만
        { 2, new int[] { 0, 1 } },               // 근거리 + 러너
        { 3, new int[] { 0, 2 } },               // 근거리 + 원거리
        { 4, new int[] { 0, 1, 2 } },            // 기본 조합
        { 5, new int[] { 2, 3 } },               // 원거리 + 힐러 (근거리 없음 → 방어 중심)
        { 6, new int[] { 1, 2, 3 } },            // 러너 중심, 압박 계열
        { 7, new int[] { 0, 3 } },               // 근거리 + 힐러만 (회복 보호 우선 판단 유도)
        { 8, new int[] { 1, 3 } },               // 러너 + 힐러 (회복과 속도의 조합)
        { 9, new int[] { 0, 1, 2, 3 } },         // 전 조합 (보스전 직전)
        { 10, new int[] { } },                   // 보스 스테이지
    };

    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.onStageChange += OnStageChanged;

        OnStageChanged(stageManager.stage);
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        if (stageManager.stage >= 10) return;

        currentTime += Time.deltaTime;
        if (currentTime > createTime) 
        {
            currentTime = 0;
            createTime = Random.Range(minTime, maxTime);

            if (currentDroneIndex.Count == 0) return;

            int prefabIndex = currentDroneIndex[Random.Range(0, currentDroneIndex.Count)];
            GameObject drone = Instantiate(droneFactory[prefabIndex]);

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            drone.transform.position = spawnPoints[spawnIndex].position;
        }
    }
    private void OnStageChanged(int stage)
    {
        if (stageDroneMap.ContainsKey(stage))
        {
            currentDroneIndex = new List<int>(stageDroneMap[stage]);
        }
    }
}
