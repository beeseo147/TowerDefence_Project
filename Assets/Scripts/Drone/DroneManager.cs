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
    public GameObject[] droneFactory; //��� ������
    public StageManager stageManager; //�������� ���� ��ũ��Ʈ

    private List<int> currentDroneIndex = new List<int>();

    private Dictionary<int, int[]> stageDroneMap = new Dictionary<int, int[]>()
    {
        { 1, new int[] { 0 } },              // �ٰŸ���
        { 2, new int[] { 0, 1 } },               // �ٰŸ� + ����
        { 3, new int[] { 0, 2 } },               // �ٰŸ� + ���Ÿ�
        { 4, new int[] { 0, 1, 2 } },            // �⺻ ����
        { 5, new int[] { 2, 3 } },               // ���Ÿ� + ���� (�ٰŸ� ���� �� ��� �߽�)
        { 6, new int[] { 1, 2, 3 } },            // ���� �߽�, �й� �迭
        { 7, new int[] { 0, 3 } },               // �ٰŸ� + ������ (ȸ�� ��ȣ �켱 �Ǵ� ����)
        { 8, new int[] { 1, 3 } },               // ���� + ���� (ȸ���� �ӵ��� ����)
        { 9, new int[] { 0, 1, 2, 3 } },         // �� ���� (������ ����)
        { 10, new int[] { } },                   // ���� ��������
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
