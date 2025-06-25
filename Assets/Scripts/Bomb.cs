using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //폭발 효과
    Transform explosion; 
    ParticleSystem expEffect;
    AudioSource expAudio;
    //폭발 범위
    public float range = 5;
    // 폭탄 중심에서의 최대 데미지와 최소 데미지
    public float maxDamage = 5f;
    public float minDamage = 1f;

    // Start is called before the first frame update
    void Start()
    {
        explosion = GameObject.Find("Explosion").transform;
        expEffect = explosion.GetComponent<ParticleSystem>();
        expAudio = explosion.GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        //드론의 레이어마스크 가져오기 1<<14
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        //폭탄의 위치에서 반경 5미터안에 드론 오브젝트를 가져온다.
        //폭탄 범위에 따라 다른 데미지 적용
        Collider[] drones = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach (Collider drone in drones)
        {
            float distance = Vector3.Distance(transform.position, drone.transform.position);
            // 거리에 따라 데미지 선형 보간
            float t = Mathf.Clamp01(distance / range); // 0(중심) ~ 1(가장자리)
            print(t);
            float damage = Mathf.Lerp(maxDamage, minDamage, t);

            drone.GetComponent<DroneAI>().OnDamageProcess((int)damage);
        } 
        //폭발 효과 재생
        explosion.position = transform.position;
        expEffect.Play();
        expAudio.Play();
        //수류탄은 파괴
        Destroy(gameObject);
    }
}
