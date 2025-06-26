//using JetBrains.Annotations;
//using Oculus.Interaction.Editor.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DroneAI : MonoBehaviour//, IFrozenObject
{
    protected enum DroneState //드론의 상태 상수 정의
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }
    protected DroneState state = DroneState.Idle; //초기 시작 상태는 Idle로 설정
    public float idleDelayTime = 2f; //대기 상태의 지속시간
    protected float currentTime; //경과 시간

    public float moveSpeed = 1; //공격 속도
    public int attackPower = 1;
    public Transform tower; //타워위치(타겟위치)
    protected NavMeshAgent agent; //내비매쉬 에이전트 컴포넌트
    public float attackRange = 3; //타워와 3미터 거리면 공격 시작
    public float attackDelayTime = 2; //공격 딜레이 시간

    public GameObject HpUI;
    //private 변수도 유니티 에디터에서 보이게 하는 어트리뷰트
    [SerializeField] //private속성 이지만 에디터에 노출이 된다.
    private int maxHp = 3;
    private int currentHp = 0;
    //폭발효과 오브젝트
   
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    // Start is called before the first frame update
    void Start()
    {
        //타워 오브젝트를 찾는다(목적지)
        tower = GameObject.Find("Tower").transform;
        explosion = GameObject.Find("Explosion").transform;
        agent = GetComponent<NavMeshAgent>(); 
        agent.enabled = false; //내비게이션을 할당 받고 바로 비활성화
        agent.speed = moveSpeed; // 움직이는 속도 1

        //explosion 오브젝트의 파티클과 오디오 컴포넌트 얻어오기
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
        currentTime += Time.deltaTime; //시간을 잰다
        if (currentTime > idleDelayTime) //경과 시간이 대기 시간을 초과했다면
        {
            state = DroneState.Move; //상태를 이동으로 전환
            agent.enabled = true; //agent 활성화
        }
    }
    protected virtual void Move()
    {
        //내비게이션의 목적지를 타워로 설정
        agent.SetDestination(tower.position);
        //공격 범위 안에 들어오면 공격 상태로 전환
        if (Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DroneState.Attack;
            agent.enabled = false;
        }
    }
    protected virtual void Attack(int attackPower)
    {
        currentTime += Time.deltaTime; //시간을 재고
        if (currentTime > attackDelayTime) //2초에 한번 씩 공격이 가능하도록 함
        {
            StartCoroutine(AttackMotion(attackPower));
            
            currentTime = 0;
        }
    }

    IEnumerator AttackMotion(int attackPower)
    {
        Vector3 originPos = transform.position;
        Vector3 direction = (tower.position - transform.position).normalized;
        Vector3 attackPos = originPos + direction * 0.5f;

        float duration = 0.1f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(originPos, attackPos, t);
            yield return null;
        }

        Tower.Instance.HP -= attackPower;

        elapsed = 0;
        while (elapsed < duration) // 원래 위치로
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(attackPos, originPos, t);
            yield return null;
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
            StopAllCoroutines(); //실행되고 있는 코루틴 함수가 있다면 중지시킴
            StartCoroutine(Damage());

            CancelInvoke(nameof(HideHpUI));
            Invoke(nameof(HideHpUI), 1.5f);
        }
        else //죽었다면 폭발 이펙트 재생, 드론 파괴
        {
            Die();
        }
    }

    private void HideHpUI()
    {
        HpUI.SetActive(false);
    }

    IEnumerator Damage()
    {
        agent.enabled = false; //길찾기 중지
                               //자식 객체의 MeshRenderer에서 Material 얻어오기
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color; //원래 색 저장
        mat.color = Color.black;  //재질의 색을 검은색으로 변경
   //     GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.3f); //0.1초 뒤에
   //     GetComponentInChildren<MeshRenderer>().enabled = true;
        mat.color = originalColor; //원래 색으로 변경
        state = DroneState.Idle; // 상태를 Idle로 저장
        currentTime = 0; //경과 시간 초기화 
    }
    void Die()
    { 
        agent.enabled = false;
        
        Debug.Log($"DroneAI Die() 호출됨: {transform.position}");
        
        // 아이템 드롭 처리
        if (ItemDropManager.Instance != null)
        {
            Debug.Log("ItemDropManager.Instance가 존재함 - 아이템 드롭 시도");
            print("드론의 현재 위치: " + transform.position);
            ItemDropManager.Instance.OnEnemyDeath("Drone", transform.position);
            
        }
        else
        {
            Debug.LogError("ItemDropManager.Instance가 null입니다! 씬에 ItemDropManager가 있는지 확인하세요.");
        }
            
        //폭발효과 위치 지정
        explosion.position = transform.position;
        //이펙트 재생
        expEffect.Play();
        expAudio.Play(); //이펙트 사운드 재생
        Destroy(gameObject); //드론 없애기
    }
    void GameOver()
    {
        state = DroneState.Die;
            
    }

    public void Heal(int amount)
    {
        if (currentHp >= maxHp) return;
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
        HpUI.SetActive(true);
        CancelInvoke(nameof(HideHpUI));
        Invoke(nameof(HideHpUI), 1.5f);
    }
}
