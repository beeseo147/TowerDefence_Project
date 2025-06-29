using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneBoss : DroneAI
{
    [Header("�п� ��� ������")]
    [SerializeField] private GameObject[] splitDronePrefabs;
    [SerializeField] private GameObject spawnEffectPrefab;

    [Header("���� �̵�")]
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float rotateRadius = 10f;

    [Header("�ð� ����� ���� ��ȯ ����")]
    [SerializeField] private float spawnIncreaseInterval = 15f; // n�ʸ��� ��ȯ �� +1
    [SerializeField] private int maxSpawnCount = 5;

    private int spawnCount = 1;
    private float spawnTimer = 0f;
    private float startTime;
    private float angle;
    private bool isRotating = false;

    new void Start()
    {
        base.Start();

        startTime = Time.time;
        HpUI.SetActive(true);
    }

    protected override void Move()
    {
        float targetRadius = rotateRadius;
        Vector3 directionToTower = (tower.position - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, tower.position);
        
        if(!isRotating)
        {
            if (currentDistance > targetRadius)
            {
                agent.SetDestination(tower.position);
            }
            else
            {
                isRotating = true;
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;

                angle = Mathf.Atan2(transform.position.z - tower.position.z, transform.position.x - tower.position.x) * Mathf.Rad2Deg;
            }
        }

        else
        {
            RotateAroundTower();
            Attack(attackPower);
        }
    }

    private void RotateAroundTower()
    {
        angle += rotateSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float rad = angle * Mathf.Deg2Rad;

        // ���ο� ��ġ ��� (Ÿ�� �߽����� �� �˵�)
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * rotateRadius;
        Vector3 orbitPos = tower.position + offset;
        transform.position = orbitPos;

        // �׻� Ÿ���� �ٶ󺸵��� ����
        Vector3 lookDir = (tower.position - transform.position).normalized;
        lookDir.y = 0f;
        if(lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }
    protected override void Attack(int attackPower)
    {
        currentTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // �ð� ����� ���� ��ȯ �� ����
        float elapsed = Time.time - startTime;
        spawnCount = Mathf.Min(1 + Mathf.FloorToInt(elapsed / spawnIncreaseInterval), maxSpawnCount);

        if (currentTime > attackDelayTime)
        {
            currentTime = 0;
            SpawnRandomSplitDrones(spawnCount);
        }
    }

    private void SpawnRandomSplitDrones(int count)
    {
        if (splitDronePrefabs == null || splitDronePrefabs.Length == 0) return;

        float radius = 5f;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
            Vector3 spawnPos = transform.position + offset;

            // ����Ʈ ���� ����
            if (spawnEffectPrefab != null)
            {
                GameObject fx = Instantiate(spawnEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(fx, 1f); 
            }

            int index = Random.Range(0, splitDronePrefabs.Length);
            GameObject selectedPrefab = splitDronePrefabs[index];

            // ��� ����
            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }
    }

    public override void OnDamageProcess(int damage)
    {
        currentHp -= damage;
        if (currentHp > 0)
        {
            //state = DroneState.Damage;
            HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
            StopAllCoroutines(); //����ǰ� �ִ� �ڷ�ƾ �Լ��� �ִٸ� ������Ŵ
            StartCoroutine(Damage());

        }
        else //�׾��ٸ� ���� ����Ʈ ���, ��� �ı�
        {
            Die();
        }
    }

    protected override IEnumerator Damage()
    {
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color; //���� �� ����
        mat.color = Color.black;  //������ ���� ���������� ����
        //     GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        //     GetComponentInChildren<MeshRenderer>().enabled = true;
        mat.color = originalColor;
    }//���� ������ ����

    protected override void Die()
    {
        agent.enabled = false;

        //����ȿ�� ��ġ ����
        explosion.position = transform.position;
        //����Ʈ ���
        expEffect.Play();
        expAudio.Play(); //이펙트 사운드 재생
        
        // 보스가 죽으면 게임 클리어 (승리)
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.GameOver();
        }
        
        Destroy(gameObject); //보스 없애기
    }
}
