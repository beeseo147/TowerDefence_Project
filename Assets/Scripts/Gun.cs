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
    // Start is called before the first frame update
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = GetComponent<AudioSource>();
    //    Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

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
            int boundaryLayer = 1 << LayerMask.NameToLayer("Boundary");
            int layerMask = playerLayer | towerLayer | boundaryLayer;
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                //레이와 부딪힌 오브젝트가 드론이라면.. 
                if (hitInfo.transform.name.Contains("Drone"))
                {
                    DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if (drone)
                    {
                        drone.OnDamageProcess(1);
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
