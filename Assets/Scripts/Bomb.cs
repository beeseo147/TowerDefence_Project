using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrabObject;

public class Bomb : MonoBehaviour, IUseItem
{
    Transform explosion; 
    ParticleSystem expEffect;
    AudioSource expAudio;

    public float range = 4.5f;
    public float maxDamage = 5f;
    public float minDamage = 1f;

    private bool isGrabbed = false;

    public float throwSpeed = 100f; // 실제 던지는 속도

    void Start()
    {
        explosion = GameObject.Find("Explosion").transform;
        expEffect = explosion.GetComponent<ParticleSystem>();
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        isGrabbed = false;
        
        // 물리 상태 초기화
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // 재생성 시 물리 비활성화 (충돌 방지)
        }
        
        // 궤적 제거
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        if (line != null)
        {
            Destroy(line);
        }
        
        // ItemWorldObject 활성화 (흔들리는 효과를 위해)
        ItemWorldObject itemWorld = gameObject.GetComponent<ItemWorldObject>();
        if (itemWorld != null)
        {
            itemWorld.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        Collider[] drones = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach (Collider drone in drones)
        {
            float distance = Vector3.Distance(transform.position, drone.transform.position);
            float t = Mathf.Clamp01(distance / range);
            float damage = Mathf.Lerp(maxDamage, minDamage, t);
            drone.GetComponent<DroneAI>().OnDamageProcess((int)damage);
        }

        explosion.position = transform.position;
        expEffect.Play();
        expAudio.Play();
        print("collision Name : " + collision.gameObject.name);
        ItemObjectPool.Instance.ReturnItem(gameObject);
    }

    public void UseItem(GameObject player)
    {
        
        //PC 카메라 위치에 따라서 위치를 설정
        gameObject.transform.position = ARAVRInput.RHandPosition + new Vector3(0.3f,0.4f,0.5f);
        gameObject.transform.rotation = Camera.main.transform.rotation;
        gameObject.transform.parent = Camera.main.transform;

        //VR 손의 위치에 따라서 위치를 설정
        //gameObject.transform.position = ARAVRInput.RHandPosition;
        //gameObject.transform.rotation = ARAVRInput.RHand.rotation;
        //gameObject.transform.parent = ARAVRInput.RHand;
        

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<ItemWorldObject>().enabled = false;

        isGrabbed = true;
    }

    void Update()
    {
        if (isGrabbed)
        {
            DrawTrajectory();
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger))
            {
                isGrabbed = false;
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                gameObject.transform.parent = null;

                // 속도로 던지기
                rb.velocity = ARAVRInput.RHandDirection * throwSpeed;

                // 궤적 제거
                LineRenderer line = gameObject.GetComponent<LineRenderer>();
                if (line != null) Destroy(line);
            }
        }
    }

    private void DrawTrajectory()
    {
        LineRenderer trajectoryLine = gameObject.GetComponent<LineRenderer>();
        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
        }

        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.positionCount = 50;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        //색이 끝으로 갈수록 옅어짐 효과
        trajectoryLine.startColor = Color.white;
        trajectoryLine.endColor = Color.gray;

        //PC
        Vector3 startPosition = gameObject.transform.position;
        //VR 손 위치와 방향을 사용하여 궤적 계산
        //Vector3 startPosition = ARAVRInput.RHandPosition;
        Vector3 initialVelocity = ARAVRInput.RHandDirection * throwSpeed;
        Vector3 gravity = Physics.gravity;

        for (int i = 0; i < trajectoryLine.positionCount; i++)
        {
            float time = i * 0.1f;
            Vector3 position = startPosition + initialVelocity * time + 0.5f * gravity * time * time;
            trajectoryLine.SetPosition(i, position);
        }
    }
}
