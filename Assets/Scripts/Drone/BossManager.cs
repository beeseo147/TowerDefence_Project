using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 박세영
// 보스 매니저 클래스
// 기능 : 보스 생성, 보스 처치 시 보스 생성, 보스 처치 시 보스 처치 횟수 증가, 보스 처치 시 보스 처치 횟수 저장
public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool bossSpawned = false;

    // 싱글톤 인스턴스 초기화
    private void Awake()
    {
        Instance = this;
    }

    // 스테이지 변경 시 보스 생성
    private void Start()
    {
        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.onStageChange += OnStageChanged;
    }

    // 스테이지 변경 시 보스 생성
    private void OnStageChanged(int stage)
    {
        if (stage == 4 && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    // 보스 생성
    public void SpawnBoss()
    {
        bossSpawned = true;

        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        Debug.Log("Boss Spawned");
    }

    // 게임 재시작 시 보스 상태 초기화
    public void ResetGame()
    {
        Debug.Log("BossManager: 게임 데이터 초기화");
        
        bossSpawned = false;
        
        Debug.Log("BossManager: 게임 데이터 초기화 완료");
    }
}
