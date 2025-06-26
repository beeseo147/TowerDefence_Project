using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact; //총알 파편 효과
    ParticleSystem bulletEffect; //총알 파편 파티클 시스템
    AudioSource bulletAudio; // 총알 발사 사운드
    public Transform crosshair; //조준점 표시
    
    [SerializeField] PlayerStatController playerStats;
    
    public bool bIsFreezeShot = false;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = GetComponent<AudioSource>();
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStatController>();
        if (playerStats == null)
            Debug.LogError("Gun Start() : PlayerStatController is Null");
        //    Cursor.visible = false;
    //    Cursor.visible = false;
        
        // 아이템 수집 이벤트 구독
        ItemWorldObject.OnItemCollected += HandleItemCollected;
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        ItemWorldObject.OnItemCollected -= HandleItemCollected;
    }
    
    // 아이템 수집 이벤트 핸들러
    private void HandleItemCollected(ItemData itemData, GameObject player)
    {
        Debug.Log($"아이템 수집됨: {itemData.itemName} by {player.name}");
        // 여기서 추가적인 효과나 사운드 등을 처리할 수 있습니다
    }

    void Update()
    {
        //크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

        Shot();
    }
    public void FreezeShot()
    {
        //FronzenShot 아이템 사용 시에 특별한 효과 처리
        //ex) 사운드 등등
        Debug.Log("FreezeShot");
        //맞은 적의 이동속도를 감소하는 효과 추가
        bIsFreezeShot = true;
    }
    void Shot()
    {
        //오른쪽 컨트롤러의 인덱스 트리거를 눌렀다면 
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            //컨트롤러의 진동 재생
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);
            // 데미지 계산값 받아오기
            int damage = playerStats.GetCurrentAttack();
            Fire(damage);
        }
    }
    void Fire(int damage)
    {
        Debug.LogWarning($"Gun Fire() : curDamage -> {damage}");
        var ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
        int ignore = (1 << LayerMask.NameToLayer("Player")) |
                     (1 << LayerMask.NameToLayer("Tower")) |
                     (1 << LayerMask.NameToLayer("Boundary"));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, ~ignore))
        {
            if (Physics.Raycast(ray, out hitInfo, 200, ~ignore))
            {
                DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                if (drone)
                {
                    // 사망 판단 및 ScoreManager.Instance.AddKill() 호출
                    //DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if(bIsFreezeShot)
                    {
                        drone.OnDamageProcess(0);
                        bIsFreezeShot = false;
                        //drone.StartCoroutine(drone.UnfreezeCoroutine());
                    }
                    else
                    {
                        drone.OnDamageProcess(damage);
                    }
                }
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Item"))
                {
                    ICollectible collectible = hitInfo.transform.GetComponent<ICollectible>();
                    if (collectible != null && collectible.CanBeCollected(gameObject))
                    {
                        collectible.Collect(gameObject);
                    }
                }
            }

            PlayFireEffect(hitInfo);
        }
    }
    void PlayFireEffect(RaycastHit hitinfo)
    {
        //총알 파편 처리
        //총알 이펙트가 진행 중이면 멈추고 재생
        bulletEffect.Stop();
        bulletEffect.Play();
        //부딪힌 지점의 방향으로 총알의 이펙트 방향을 설정
        bulletImpact.forward = hitinfo.normal;
        //부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
        bulletImpact.position = hitinfo.point;
    }
    
}
