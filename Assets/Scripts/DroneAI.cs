//using JetBrains.Annotations;
//using Oculus.Interaction.Editor.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DroneAI : MonoBehaviour//, IFrozenObject
{
    enum DroneState //����� ���� ��� ����
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }
    DroneState state = DroneState.Idle; //�ʱ� ���� ���´� Idle�� ����
    public float idleDelayTime = 2f; //��� ������ ���ӽð�
    float currentTime; //��� �ð�

    public float moveSpeed = 1; //���� �ӵ�
    public Transform tower; //Ÿ����ġ(Ÿ����ġ)
    NavMeshAgent agent; //����Ž� ������Ʈ ������Ʈ
    public float attackRange = 3; //Ÿ���� 3���� �Ÿ��� ���� ����
    public float attackDelayTime = 2; //���� ������ �ð�

    public GameObject HpUI;
    //private ������ ����Ƽ �����Ϳ��� ���̰� �ϴ� ��Ʈ����Ʈ
    [SerializeField] //private�Ӽ� ������ �����Ϳ� ������ �ȴ�.
    private int hp = 3;
    //����ȿ�� ������Ʈ
   
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;

    // Start is called before the first frame update
    void Start()
    {
        //Ÿ�� ������Ʈ�� ã�´�(������)
        tower = GameObject.Find("Tower").transform;
        explosion = GameObject.Find("Explosion").transform;
        agent = GetComponent<NavMeshAgent>(); 
        agent.enabled = false; //������̼��� �Ҵ� �ް� �ٷ� ��Ȱ��ȭ
        agent.speed = moveSpeed; // �����̴� �ӵ� 1

        //explosion ������Ʈ�� ��ƼŬ�� ����� ������Ʈ ������
        expEffect = explosion.GetComponent<ParticleSystem>();
        expAudio = explosion.GetComponent<AudioSource>();

        Tower.Instance.onTowerDestroy += GameOver;  
        HpUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case DroneState.Idle: Idle(); break;
            case DroneState.Move: Move(); break;
            case DroneState.Attack: Attack(); break;
            case DroneState.Damage: Damage(); break;
            case DroneState.Die: Die(); break;
        }
        //print(state);
    }
    void Idle() 
    {
        currentTime += Time.deltaTime; //�ð��� ���
        if (currentTime > idleDelayTime) //��� �ð��� ��� �ð��� �ʰ��ߴٸ�
        {
            state = DroneState.Move; //���¸� �̵����� ��ȯ
            agent.enabled = true; //agent Ȱ��ȭ
        }
    }
    void Move()
    {
        //������̼��� �������� Ÿ���� ����
        agent.SetDestination(tower.position);
        //���� ���� �ȿ� ������ ���� ���·� ��ȯ
        if (Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DroneState.Attack;
            agent.enabled = false;
        }
    }
    void Attack()
    {
        currentTime += Time.deltaTime; //�ð��� ���
        if (currentTime > attackDelayTime) //2�ʿ� �ѹ� �� ������ �����ϵ��� ��
        {
            //Ÿ���� ü���� ����
            Tower.Instance.HP--; //HP ������Ƽ�� set�Լ��� ����Ǿ� ���ڰŸ��� ȿ�� ���
      //      Tower.Instance.HP = Tower.Instance.HP - 1;
            //�ǰ� ����Ʈ ȿ��
            currentTime = 0;
        }
    }
    public void OnDamageProcess(int damage)
    {
        hp--; //ü���� 1����, ���� �ʾҴٸ� Damage ���·� ����
        if (hp > 0)
        {
            state = DroneState.Damage;
            HpUI.SetActive(true);
            HpUI.GetComponentInChildren<Image>().fillAmount = (float)hp / 3;
            StopAllCoroutines(); //����ǰ� �ִ� �ڷ�ƾ �Լ��� �ִٸ� ������Ŵ
            StartCoroutine(Damage());
        }
        else //�׾��ٸ� ���� ����Ʈ ���, ��� �ı�
        {
            Die();
        }
    }
    IEnumerator Damage()
    {
        agent.enabled = false; //��ã�� ����
                               //�ڽ� ��ü�� MeshRenderer���� Material ������
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color; //���� �� ����
        mat.color = Color.red; //������ ���� ���������� ����
   //     GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.1f); //0.1�� �ڿ�
   //     GetComponentInChildren<MeshRenderer>().enabled = true;
        mat.color = originalColor; //���� ������ ����
        state = DroneState.Idle; // ���¸� Idle�� ����
        currentTime = 0; //��� �ð� �ʱ�ȭ 
    }
    void Die()
    { 
        agent.enabled = false;
        
        Debug.Log($"DroneAI Die() ȣ���: {transform.position}");
        
        // ������ ��� ó��
        if (ItemDropManager.Instance != null)
        {
            Debug.Log("ItemDropManager.Instance�� ������ - ������ ��� �õ�");
            print("����� ���� ��ġ: " + transform.position);
            ItemDropManager.Instance.OnEnemyDeath("Drone", transform.position);
            
        }
        else
        {
            Debug.LogError("ItemDropManager.Instance�� null�Դϴ�! ���� ItemDropManager�� �ִ��� Ȯ���ϼ���.");
        }
            
        //����ȿ�� ��ġ ����
        explosion.position = transform.position;
        //����Ʈ ���
        expEffect.Play();
        expAudio.Play(); //����Ʈ ���� ���
        Destroy(gameObject); //��� ���ֱ�
    }
    void GameOver()
    {
        state = DroneState.Die;
            
    }

}
