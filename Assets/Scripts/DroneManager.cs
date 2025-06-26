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
    float elapsedStageTime = 0f;
    public Transform[] spawnPoints; //드론의 스폰 포인트
    public GameObject[] droneFactory; //드론 프리팹
    public StageManager stageManager; //스테이지 관리 스크립트
    public int droneCount = 0; //생성된 드론 갯수
    public int maxDroneCount = 10; //최대 드론 갯수

    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        createTime = Random.Range(minTime, maxTime);
        //stageManager.onStageChange += SetMaxDroneCount;
    }

    void Update()
    {
        elapsedStageTime += Time.deltaTime;
        currentTime += Time.deltaTime; 
        if (currentTime > createTime) //1초~5초 사이 랜덤하게 드론 생성
        {
            /*
            if(droneCount >= maxDroneCount)
            {
                return;
            }
            */

            GameObject prefabToSpawn = ChooseGhostByTime(elapsedStageTime);
            GameObject drone = Instantiate(prefabToSpawn);

            int index = Random.Range(0, spawnPoints.Length);
            //생성한 드론을 4개의 스폰포인트 중 하나에 위치 지정
            drone.transform.position = spawnPoints[index].position;
            currentTime = 0; //경과 시간 초기화
            createTime = Random.Range(minTime, maxTime); //생성시간 재할당
            droneCount++;
        }
    }
    GameObject ChooseGhostByTime(float time)
    {
        int maxIndex = 0;

        if (time < 20f)
        {
            maxIndex = 0;
        }
        else if (time < 40f)
        {
            maxIndex = 1;
        }
        else if (time < 60f)
        {
            maxIndex = 2;
        }
        else
        {
            maxIndex = 3;
        }

        int index = Random.Range(0, maxIndex + 1);
        return droneFactory[index];
    }

    /*
    public void SetMaxDroneCount(int stage)
    {
        maxDroneCount = stage * 10;
        droneCount = 0;
    }
    */
}
