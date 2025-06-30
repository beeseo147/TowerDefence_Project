using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 박세영
// 드론 매니저 클래스
// 기능 : 드론 생성, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정, 드론 생성 간격 설정
public class DroneManager : MonoBehaviour
{
    //드론 생성 간격 최소값
    public float minTime = 1;
    //드론 생성 간격 최대값
    public float maxTime = 5;
    float createTime; //드론 생성 간격
    float currentTime; //드론 생성 간격

    public Transform[] spawnPoints; //드론 생성 위치
    public GameObject[] droneFactory; //드론 생성 프리팹
    public StageManager stageManager; //스테이지 매니저

    private List<int> currentDroneIndex = new List<int>();
    //드론 생성 간격 맵
    private Dictionary<int, int[]> stageDroneMap = new Dictionary<int, int[]>()
    {
        { 1, new int[] { 0 } },              // 드론
        { 2, new int[] { 0, 1 } },               // 드론 + 힐러 드론
        { 3, new int[] { 0, 2 } },               // 드론 + 보스 드론
        { 4, new int[] { 0, 1, 2 } },            // 드론 + 힐러 드론 + 보스 드론
        { 5, new int[] { 2, 3 } },               // 힐러 드론 + 보스 드론 (드론 생성 간격 최대값)
        { 6, new int[] { 1, 2, 3 } },            // 힐러 드론 + 보스 드론 + 힐러 드론 (드론 생성 간격 최대값)
        { 7, new int[] { 0, 3 } },               // 드론 + 보스 드론 (드론 생성 간격 최대값)
        { 8, new int[] { 1, 3 } },               // 힐러 드론 + 보스 드론 (드론 생성 간격 최대값)
        { 9, new int[] { 0, 1, 2, 3 } },         // 드론 + 힐러 드론 + 보스 드론 + 힐러 드론 (드론 생성 간격 최대값)
        { 10, new int[] { } },                   // 드론 생성 간격 최대값
    };
    //드론 생성 간격 초기화
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.onStageChange += OnStageChanged;

        OnStageChanged(stageManager.stage);
        createTime = Random.Range(minTime, maxTime);
    }
    //드론 생성 간격 업데이트
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
    //스테이지 변경 시 드론 생성 간격 업데이트
    private void OnStageChanged(int stage)
    {
        if (stageDroneMap.ContainsKey(stage))
        {
            currentDroneIndex = new List<int>(stageDroneMap[stage]);
        }
    }
}
