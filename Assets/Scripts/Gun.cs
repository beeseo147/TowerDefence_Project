using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
#endif
using UnityEngine;

// �ۼ��� : �赿��
// �� Ŭ����
// ��� : �� �߻�, �� ȿ�� ����, �� ���
public class Gun : MonoBehaviour
{
    public Transform bulletImpact; // �� ȿ�� ��ġ
    ParticleSystem bulletEffect; // �� ȿ�� ��ƼŬ
    AudioSource bulletAudio; // �� ����
    public Transform crosshair; // �� Ŀ��
    
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
        
        // ������ ���� �̺�Ʈ ���
        ItemWorldObject.OnItemCollected += HandleItemCollected;
    }
    
    void OnDestroy()
    {
        // ������ ���� �̺�Ʈ ��� ����
        ItemWorldObject.OnItemCollected -= HandleItemCollected;
    }
    
    // ������ ���� �̺�Ʈ ó��
    private void HandleItemCollected(ItemData itemData, GameObject player)
    {
        Debug.Log($"������ ����: {itemData.itemName} by {player.name}");
        // ������ ȿ�� ����
    }

    void Update()
    {
        // �� Ŀ�� �׸���
        ARAVRInput.DrawCrosshair(crosshair);

        Shot();
    }
    public void FreezeShot()
    {
        // �õ� ��� ȿ�� ����
        Debug.Log("FreezeShot");
        // �õ� ��� ȿ�� ����
        bIsFreezeShot = true;
    }
    void Shot()
    {
        // �� �߻�
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // �� �߻� ���� ȿ�� ����
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);
            // �� �߻� ������ ���
            int damage = playerStats.GetCurrentAttack();
            Fire(damage);
        }
    }
    // �� �߻�
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
                    // ?���? ?��?�� �? ScoreManager.Instance.AddKill() ?���?
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
    // �� �߻� ȿ�� ����
    void PlayFireEffect(RaycastHit hitinfo)
    {
        // �� ȿ�� ��ƼŬ ���
        bulletEffect.Stop();
        bulletEffect.Play();
        // �� ȿ�� ���� ����
        bulletImpact.forward = hitinfo.normal;
        // �� ȿ�� ��ġ ����
        bulletImpact.position = hitinfo.point;
    }
    
}
