using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool bossSpawned = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.onStageChange += OnStageChanged;
    }

    private void OnStageChanged(int stage)
    {
        if (stage == 10 && !bossSpawned)
        {
            SpawnBoss();
        }
    }

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
