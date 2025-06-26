using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact; //총알 파편 효과
    ParticleSystem bulletEffect; //총알 파편 파티클 시스템
    AudioSource bulletAudio; // 총알 발사 사운드
    public Transform crosshair; //조준점 표시
    
    public bool bIsFreezeShot = false;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = GetComponent<AudioSource>();
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

    // Update is called once per frame
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

            Ray ray = new Ray(ARAVRInput.RHandPosition,
                ARAVRInput.RHandDirection);
            RaycastHit hitInfo;
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
         
            int layerMask = playerLayer | towerLayer;
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                //레이와 부딪힌 오브젝트가 드론이라면.. 
                if (hitInfo.transform.name.Contains("Drone"))
                {
                    DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if (drone)
                    {
                        if(bIsFreezeShot)
                        {
                            drone.OnDamageProcess(0);
                            bIsFreezeShot = false;
                            //drone.StartCoroutine(drone.UnfreezeCoroutine());
                        }
                        else
                        {
                            drone.OnDamageProcess(1);
                        }
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
                //총알 파편 처리
                //총알 이펙트가 진행 중이면 멈추고 재생
                bulletEffect.Stop();
                bulletEffect.Play();
                //부딪힌 지점의 방향으로 총알의 이펙트 방향을 설정
                bulletImpact.forward = hitInfo.normal;
                //부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.position = hitInfo.point;
            }
        }
    }
    
}
