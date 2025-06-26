using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrabObject;

public class Bomb : MonoBehaviour,IUseItem
{
    //폭발 효과
    Transform explosion; 
    ParticleSystem expEffect;
    AudioSource expAudio;
    //폭발 범위
    public float range = 4.5f;
    // 폭탄 중심에서의 최대 데미지와 최소 데미지
    public float maxDamage = 5f;
    public float minDamage = 1f;
    private bool isGrabbed = false;
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
        //수류탄은 리턴
        //Destroy(gameObject);
        ItemObjectPool.Instance.ReturnItem(gameObject);
    }
    public void UseItem(GameObject player)
    {
        //print("Bomb UseItem called");
        //Player 오른손 위치에 폭탄 오브젝트 지정
        gameObject.transform.position = ARAVRInput.RHandPosition;
        gameObject.transform.rotation = ARAVRInput.RHand.rotation;
        gameObject.transform.parent = player.transform;
        //print("Bomb instantiated at: " + gameObject.transform.position);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<ItemWorldObject>().enabled = false;
        isGrabbed = true;
    }
    void Update()
    {
        //폭탄이 손에 잡혀있다면
        if (isGrabbed)
        {
            //Input 발생시 폭탄을 전방으로 던진다.
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger))
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.transform.parent = null;
                // 폭탄을 던지기 전에 궤적을 그리기 위한 LineRenderer 설정
                // 실제 게임에서 어떻게 할지는 고민중
                {
                    LineRenderer trajectoryLine = gameObject.GetComponent<LineRenderer>();
                    if (trajectoryLine == null)
                    {
                        trajectoryLine = gameObject.AddComponent<LineRenderer>();
                    }

                    trajectoryLine.startWidth = 0.1f;
                    trajectoryLine.endWidth = 0.1f;
                    trajectoryLine.positionCount = 50;

                    // 물리 시뮬레이션으로 궤적 계산
                    Vector3 velocity = ARAVRInput.RHandDirection * 1000 / gameObject.GetComponent<Rigidbody>().mass;
                    Vector3 position = transform.position;
                    Vector3 gravity = Physics.gravity;

                    for (int i = 0; i < trajectoryLine.positionCount; i++)
                    {
                        float time = i * 0.1f; // 0.1초 간격으로 포인트 생성
                        Vector3 newPosition = position + velocity * time + 0.5f * gravity * time * time;
                        trajectoryLine.SetPosition(i, newPosition);
                    }
                }



                gameObject.GetComponent<Rigidbody>().AddForce(ARAVRInput.RHandDirection * 1000);
                isGrabbed = false;
            }
        }
    }
}
