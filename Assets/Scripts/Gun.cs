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
    
    public bool bIsFreezeShot = false;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = GetComponent<AudioSource>();
    //    Cursor.visible = false;
        
        // ������ ���� �̺�Ʈ ����
        ItemWorldObject.OnItemCollected += HandleItemCollected;
    }
    
    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        ItemWorldObject.OnItemCollected -= HandleItemCollected;
    }
    
    // ������ ���� �̺�Ʈ �ڵ鷯
    private void HandleItemCollected(ItemData itemData, GameObject player)
    {
        Debug.Log($"������ ������: {itemData.itemName} by {player.name}");
        // ���⼭ �߰����� ȿ���� ���� ���� ó���� �� �ֽ��ϴ�
    }

    // Update is called once per frame
    void Update()
    {
        //ũ�ν���� ǥ��
        ARAVRInput.DrawCrosshair(crosshair);

        Shot();
    }
    public void FreezeShot()
    {
        //FronzenShot ������ ��� �ÿ� Ư���� ȿ�� ó��
        //ex) ���� ���
        Debug.Log("FreezeShot");
        //���� ���� �̵��ӵ��� �����ϴ� ȿ�� �߰�
        bIsFreezeShot = true;
    }
    void Shot()
    {
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
         
            int layerMask = playerLayer | towerLayer;
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                //���̿� �ε��� ������Ʈ�� ����̶��.. 
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
