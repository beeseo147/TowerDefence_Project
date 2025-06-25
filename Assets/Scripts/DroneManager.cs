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
    public Transform[] spawnPoints; //����� ���� ����Ʈ
    public GameObject droneFactory; //��� ������
    public StageManager stageManager; //�������� ���� ��ũ��Ʈ
    public int droneCount = 0; //������ ��� ����
    public int maxDroneCount = 10; //�ִ� ��� ����
    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        createTime = Random.Range(minTime, maxTime);
        stageManager.onStageChange += SetMaxDroneCount;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime; 
        if (currentTime > createTime) //1��~5�� ���� �����ϰ� ��� ����
        {
            if(droneCount >= maxDroneCount)
            {
                return;
            }
            GameObject drone = Instantiate(droneFactory);
            int index = Random.Range(0, spawnPoints.Length);
            //������ ����� 4���� ��������Ʈ �� �ϳ��� ��ġ ����
            drone.transform.position = spawnPoints[index].position;
            currentTime = 0; //��� �ð� �ʱ�ȭ
            createTime = Random.Range(minTime, maxTime); //�����ð� ���Ҵ�
            droneCount++;
        }
    }
    public void SetMaxDroneCount(int stage)
    {
        maxDroneCount = stage * 10;
        droneCount = 0;
    }
}
