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
}
