using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact; //�Ѿ� ���� ȿ��
    ParticleSystem bulletEffect; //�Ѿ� ���� ��ƼŬ �ý���
    AudioSource bulletAudio; // �Ѿ� �߻� ����
    public Transform crosshair; //������ ǥ��
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
        //ũ�ν���� ǥ��
        ARAVRInput.DrawCrosshair(crosshair);

        //������ ��Ʈ�ѷ��� �ε��� Ʈ���Ÿ� �����ٸ� 
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            //��Ʈ�ѷ��� ���� ���
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
                //���̿� �ε��� ������Ʈ�� ����̶��.. 
                if (hitInfo.transform.name.Contains("Drone"))
                {
                    DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if (drone)
                    {
                        drone.OnDamageProcess(1);
                    }
                }
                //�Ѿ� ���� ó��
                //�Ѿ� ����Ʈ�� ���� ���̸� ���߰� ���
                bulletEffect.Stop();
                bulletEffect.Play();
                //�ε��� ������ �������� �Ѿ��� ����Ʈ ������ ����
                bulletImpact.forward = hitInfo.normal;
                //�ε��� ���� �ٷ� ������ ����Ʈ�� ���̵��� ����
                bulletImpact.position = hitInfo.point;
            }
        }
    }
}
