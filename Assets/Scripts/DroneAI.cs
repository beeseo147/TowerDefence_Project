//using JetBrains.Annotations;
//using Oculus.Interaction.Editor.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DroneAI : MonoBehaviour
{
    protected enum DroneState //����� ���� ��� ����
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }
    protected DroneState state = DroneState.Idle; //�ʱ� ���� ���´� Idle�� ����
    public float idleDelayTime = 2f; //��� ������ ���ӽð�
    protected float currentTime; //��� �ð�

    public float moveSpeed = 1; //���� �ӵ�
    public int attackPower = 1;
    public Transform tower; //Ÿ����ġ(Ÿ����ġ)
    protected NavMeshAgent agent; //����Ž� ������Ʈ ������Ʈ
    public float attackRange = 3; //Ÿ���� 3���� �Ÿ��� ���� ����
    public float attackDelayTime = 2; //���� ������ �ð�

    public GameObject HpUI;
    //private ������ ����Ƽ �����Ϳ��� ���̰� �ϴ� ��Ʈ����Ʈ
    [SerializeField] //private�Ӽ� ������ �����Ϳ� ������ �ȴ�.
    private int maxHp = 3;
    private int currentHp = 0;
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
        currentHp = maxHp;
        HpUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case DroneState.Idle: Idle(); break;
            case DroneState.Move: Move(); break;
            case DroneState.Attack: Attack(attackPower); break;
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
    protected virtual void Move()
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
    protected virtual void Attack(int attackPower)
    {
        currentTime += Time.deltaTime; //�ð��� ���
        if (currentTime > attackDelayTime) //2�ʿ� �ѹ� �� ������ �����ϵ��� ��
        {
            //Ÿ���� ü���� ����
            Tower.Instance.HP -= attackPower; //HP ������Ƽ�� set�Լ��� ����Ǿ� ���ڰŸ��� ȿ�� ���
      //      Tower.Instance.HP = Tower.Instance.HP - 1;
            //�ǰ� ����Ʈ ȿ��
            currentTime = 0;
        }
    }
    public void OnDamageProcess(int damage)
    {
        currentHp -= damage; 
        if (currentHp > 0)
        {
            state = DroneState.Damage;
            HpUI.SetActive(true);
            HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
            StopAllCoroutines(); //����ǰ� �ִ� �ڷ�ƾ �Լ��� �ִٸ� ������Ŵ
            StartCoroutine(Damage());
        }
        else //�׾��ٸ� ���� ����Ʈ ���, ��� �ı�
        {
            //����ȿ�� ��ġ ����
            explosion.position = transform.position;
            //����Ʈ ���
            expEffect.Play();
            expAudio.Play(); //����Ʈ ���� ���
            Destroy(gameObject); //��� ���ֱ�
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
    }
    void GameOver()
    {
        state = DroneState.Die;
            
    }
}
