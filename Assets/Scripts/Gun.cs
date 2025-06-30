using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
#endif
using UnityEngine;

// 작성자 : 김동균
// 총 클래스
// 기능 : 총 발사, 총 효과 적용, 총 사용
public class Gun : MonoBehaviour
{
    public Transform bulletImpact; // 총 효과 위치
    ParticleSystem bulletEffect; // 총 효과 파티클
    AudioSource bulletAudio; // 총 사운드
    public Transform crosshair; // 총 커서
    
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
        
        // 아이템 수집 이벤트 등록
        ItemWorldObject.OnItemCollected += HandleItemCollected;
    }
    
    void OnDestroy()
    {
        // 아이템 수집 이벤트 등록 해제
        ItemWorldObject.OnItemCollected -= HandleItemCollected;
    }
    
    // 아이템 수집 이벤트 처리
    private void HandleItemCollected(ItemData itemData, GameObject player)
    {
        Debug.Log($"아이템 수집: {itemData.itemName} by {player.name}");
        // 아이템 효과 적용
    }

    void Update()
    {
        // 총 커서 그리기
        ARAVRInput.DrawCrosshair(crosshair);

        Shot();
    }
    public void FreezeShot()
    {
        // 냉동 사격 효과 적용
        Debug.Log("FreezeShot");
        // 냉동 사격 효과 적용
        bIsFreezeShot = true;
    }
    void Shot()
    {
        // 총 발사
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // 총 발사 진동 효과 적용
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);
            // 총 발사 데미지 계산
            int damage = playerStats.GetCurrentAttack();
            Fire(damage);
        }
    }
    // 총 발사
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
                    // ?궗留? ?뙋?떒 諛? ScoreManager.Instance.AddKill() ?샇異?
                    //DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if(bIsFreezeShot)
                    {
                        drone.OnDamageProcess(0);
                        bIsFreezeShot = false;
                        drone.StartCoroutine(drone.UnfreezeCoroutine());
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
    // 총 발사 효과 적용
    void PlayFireEffect(RaycastHit hitinfo)
    {
        // 총 효과 파티클 재생
        bulletEffect.Stop();
        bulletEffect.Play();
        // 총 효과 방향 설정
        bulletImpact.forward = hitinfo.normal;
        // 총 효과 위치 설정
        bulletImpact.position = hitinfo.point;
    }
    
}
