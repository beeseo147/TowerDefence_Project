using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    //����� �����ð� ������ 1��~5��
    public float minTime = 1;
    public float maxTime = 5;
    float createTime; //���� �ð� ����
    float currentTime; //���� �ð�
    float elapsedStageTime = 0f;
    public Transform[] spawnPoints; //����� ���� ����Ʈ
    public GameObject[] droneFactory; //��� ������
    public StageManager stageManager; //�������� ���� ��ũ��Ʈ
    public int droneCount = 0; //������ ��� ����
    public int maxDroneCount = 10; //�ִ� ��� ����

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
        if (currentTime > createTime) //1��~5�� ���� �����ϰ� ��� ����
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
            //������ ����� 4���� ��������Ʈ �� �ϳ��� ��ġ ����
            drone.transform.position = spawnPoints[index].position;
            currentTime = 0; //��� �ð� �ʱ�ȭ
            createTime = Random.Range(minTime, maxTime); //�����ð� ���Ҵ�
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
