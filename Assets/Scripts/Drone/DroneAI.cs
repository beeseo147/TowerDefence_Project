//using JetBrains.Annotations;
//using Oculus.Interaction.Editor.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 작성자 : 박세영
// 드론 AI 클래스
// 기능 : 드론 AI 상태 전환, 드론 AI 이동, 드론 AI 공격, 드론 AI 데미지 처리, 드론 AI 사망, 드론 AI 체력 회복, 드론 AI 얼리기, 드론 AI 얼림 해제    
public class DroneAI : MonoBehaviour, IFrozenObject
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

    private string droneName {get; set;}
    [Header("적 타입 설정")]
    public string enemyType = "Drone"; // Inspector에서 설정 가능
    public EnemyType droneType = EnemyType.Drone; // Enum 버전 (더 안전)
    public float moveSpeed = 1; //공격 속도
    public int attackPower = 1;
    public Transform tower; //타워위치(타겟위치)
    protected NavMeshAgent agent; //내비매쉬 에이전트 컴포넌트
    public float attackRange = 3; //타워와 3미터 거리면 공격 시작
    public float attackDelayTime = 2; //공격 딜레이 시간

    public GameObject HpUI;
    //private 변수도 유니티 에디터에서 보이게 하는 어트리뷰트
    [SerializeField] //private속성 이지만 에디터에 노출이 된다.
    protected int maxHp = 3;
    protected int currentHp = 0;
    //폭발효과 오브젝트
   
    protected Transform explosion;
    public ParticleSystem expEffect;
    protected AudioSource expAudio;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    // 드론 AI 시작
    protected void Start()
    {
        //타워 오브젝트를 찾는다(목적지)
        GameObject towerObj = GameObject.Find("Tower");
        if (towerObj != null)
        {
            tower = towerObj.transform;
        }
        
        GameObject explosionObj = GameObject.Find("Explosion");
        if (explosionObj != null)
        {
            explosion = explosionObj.transform;
            //explosion 오브젝트의 파티클과 오디오 컴포넌트 얻어오기
            expEffect = explosion.GetComponent<ParticleSystem>();
            expAudio = explosion.GetComponent<AudioSource>();
        }
        
        agent = GetComponent<NavMeshAgent>(); 
        agent.enabled = false; //내비게이션을 할당 받고 바로 비활성화
        agent.speed = moveSpeed; // 움직이는 속도 1

        if (Tower.Instance != null)
        {
            Tower.Instance.onTowerDestroy += GameOver;  
        }
        
        currentHp = maxHp;
        HpUI.SetActive(false);
    }

    // 드론 AI 업데이트
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
    // 드론 AI 대기 상태
    void Idle() 
    {
        // Tower가 파괴되었으면 Die 상태로 전환
        if (tower == null)
        {
            state = DroneState.Die;
            return;
        }
        
        currentTime += Time.deltaTime; //시간을 잰다
        if (currentTime > idleDelayTime) //경과 시간이 대기 시간을 초과했다면
        {
            state = DroneState.Move; //상태를 이동으로 전환
            agent.enabled = true; //agent 활성화
        }
    }
    // 드론 AI 이동 상태
    protected virtual void Move()
    {
        // Tower가 파괴되었으면 Die 상태로 전환
        if (tower == null)
        {
            state = DroneState.Die;
            return;
        }
        
        // NavMeshAgent가 유효한지 확인
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"DroneAI Move(): NavMeshAgent가 유효하지 않음 - {gameObject.name}");
            // NavMeshAgent를 다시 활성화 시도
            if (agent != null && !agent.enabled)
            {
                agent.enabled = true;
            }
            return;
        }
        
        //내비게이션의 목적지를 타워로 설정
        agent.SetDestination(tower.position);
        //공격 범위 안에 들어오면 공격 상태로 전환
        if (Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DroneState.Attack;
            agent.enabled = false;
        }
    }
    // 드론 AI 공격 상태
    protected virtual void Attack(int attackPower)
    {
        currentTime += Time.deltaTime; //시간을 재고
        if (currentTime > attackDelayTime) //2초에 한번 씩 공격이 가능하도록 함
        {
            StartCoroutine(AttackMotion(attackPower));
            
            currentTime = 0;
        }
    }

    // 드론 AI 공격 모션
    IEnumerator AttackMotion(int attackPower)
    {
        // Tower가 파괴되었으면 공격 중단
        if (tower == null || Tower.Instance == null)
        {
            state = DroneState.Die;
            yield break;
        }
        
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

        // Tower가 여전히 존재할 때만 데미지 적용
        if (Tower.Instance != null)
        {
            Tower.Instance.TakeDamage(attackPower);
        }

        elapsed = 0;
        while (elapsed < duration) // 원래 위치로
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(attackPos, originPos, t);
            yield return null;
        }
    }

    // 드론 AI 데미지 처리
    public virtual void OnDamageProcess(int damage)
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

    // 드론 AI 체력 UI 숨기기
    private void HideHpUI()
    {
        HpUI.SetActive(false);
    }

    // 드론 AI 데미지 처리
    protected virtual IEnumerator Damage()
    {
        agent.enabled = false; //길찾기 중지
                               //자식 객체의 MeshRenderer에서 Material 얻어오기
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = Color.white; // 기본 색상 저장
        if(mat != null)
        {
            originalColor = mat.color; // 원래 색 저장
            mat.color = Color.black;  // 재질의 색을 검은색으로 변경
        }
       
        yield return new WaitForSeconds(0.1f); // 0.1초 뒤에
        if(mat != null)
        {
            mat.color = originalColor; // 원래 색으로 변경
        }
        state = DroneState.Idle; // 상태를 Idle로 저장
        currentTime = 0; //경과 시간 초기화 
    }
    // 드론 AI 사망 처리
    protected virtual void Die()
    { 
        agent.enabled = false;
        
        Debug.Log($"DroneAI Die() 호출됨: {transform.position}");
        
        // 플레이어 킬 카운트 증가
        PlayerStatController playerStatController = FindObjectOfType<PlayerStatController>();
        if (playerStatController != null)
        {
            playerStatController.OnEnemyKilled();
        }
        
        // 아이템 드롭 처리
        if (ItemDropManager.Instance != null)
        {
            Debug.Log("ItemDropManager.Instance가 존재함 - 아이템 드롭 시도");
            print("드론의 현재 위치: " + transform.position);
            // Enum 버전 사용 (더 안전하고 깔끔)
            ItemDropManager.Instance.OnEnemyDeath(droneType, transform.position);
            
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
    // 게임 오버 처리
    void GameOver()
    {
        state = DroneState.Die;
            
    }

    // 드론 AI 체력 회복
    public void Heal(int amount)
    {
        if (currentHp >= maxHp) return;
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
        HpUI.SetActive(true);
        CancelInvoke(nameof(HideHpUI));
        Invoke(nameof(HideHpUI), 1.5f);
    }
    // 드론 AI 얼리기
    public void Freeze()
    {
        print("Freeze() 호출됨");
        //얼음 효과 재생
        //속도 감소
        agent.speed *= 0.2f;
    }
    // 드론 AI 얼림 해제
    public void Unfreeze()
    {
        print("Unfreeze() 호출됨");
        //얼음 효과 종료
        //속도 증가
        agent.speed *= 5f;
    }
    // 드론 AI 얼림 해제 코루틴
    public IEnumerator UnfreezeCoroutine()
    {
        Freeze();
        yield return new WaitForSeconds(10f);
        Unfreeze();
    }
}
