using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact; //�Ѿ� ���� ȿ��
    ParticleSystem bulletEffect; //�Ѿ� ���� ��ƼŬ �ý���
    AudioSource bulletAudio; // �Ѿ� �߻� ����
    public Transform crosshair; //������ ǥ��

    [SerializeField] PlayerStatController playerStats;


    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = GetComponent<AudioSource>();
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStatController>();
        if (playerStats == null)
            Debug.LogError("Gun Start() : PlayerStatController is Null");
        //    Cursor.visible = false;
    }

    void Update()
    {
        //ũ�ν���� ǥ��
        ARAVRInput.DrawCrosshair(crosshair);

        //������ ��Ʈ�ѷ��� �ε��� Ʈ���Ÿ� �����ٸ� 
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            //��Ʈ�ѷ��� ���� ���
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);

            // ������ ��갪 �޾ƿ���
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
            // ���̿� �ε��� ������Ʈ�� ����̶��.. 
            if (hitInfo.transform.name.Contains("Drone"))
            {
                DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                if (drone)
                {
                    drone.OnDamageProcess(damage);

                    // ��� �Ǵ� �� ScoreManager.Instance.AddKill() ȣ��
                }
            }

            PlayFireEffect(hitInfo);
        }
    }

    void PlayFireEffect(RaycastHit hitinfo)
    {
        //�Ѿ� ���� ó��
        //�Ѿ� ����Ʈ�� ���� ���̸� ���߰� ���
        bulletEffect.Stop();
        bulletEffect.Play();
        //�ε��� ������ �������� �Ѿ��� ����Ʈ ������ ����
        bulletImpact.forward = hitinfo.normal;
        //�ε��� ���� �ٷ� ������ ����Ʈ�� ���̵��� ����
        bulletImpact.position = hitinfo.point;
    }
}
