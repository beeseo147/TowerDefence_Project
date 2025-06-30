using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// 작성자 : 공통
// 사용하지않음
public class TeleportStraight : MonoBehaviour
{
    //�ڷ���Ʈ�� ǥ���� UI
    public Transform teleportCircleUI;
    LineRenderer lr; //���� �׸� ������ 
    //������ �ڷ���Ʈ UI�� ũ��
    Vector3 originScale = Vector3.one * 0.02f;
    public bool isWarp = false; //���� ��� ����
    public float warpTime = 0.1f; //������ �ɸ��� �ð�
    public PostProcessVolume post; // ����ϰ� �ִ� ����Ʈ���μ��� ���� ������Ʈ

    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //��ư�� �����ٸ�
        if (ARAVRInput.GetDown(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true; //���η������� ������Ʈ Ȱ��ȭ
        }
        //��ư�� ���� ����
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false;//���η������� ������Ʈ ��Ȱ��ȭ
            if (teleportCircleUI.gameObject.activeSelf)
            {
                if (isWarp == false) //���� ����� ����� �ƴ� �� �����̵�
                {
                    //ĳ���� ��Ʈ�ѷ� ������Ʈ ��Ȱ��ȭ
                    GetComponent<CharacterController>().enabled = false;
                    //�ڷ���Ʈ UI ��ġ�� ���� �̵�
                    transform.position = teleportCircleUI.position +
                        Vector3.up;
                    //ĳ���� ��Ʈ�ѷ� ������Ʈ ��Ȱ��ȭ
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    StartCoroutine(Warp());
                }
               
            }

            teleportCircleUI.gameObject.SetActive(false);
        }
            //���� ��Ʈ�ѷ��� One��ư ������ ���� ��
            if (ARAVRInput.Get(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            //�޼��� �������� ���̸� ����
            Ray ray = new Ray(ARAVRInput.LHandPosition,
                ARAVRInput.LHandDirection);
            RaycastHit hitInfo;
            //Terrain�� Ray �浹 ����
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            if (Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                //�ε��� ������ �ڷ���Ʈ UI ǥ��
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                //�ڷ���Ʈ UI Ȱ��ȭ
                teleportCircleUI.gameObject.SetActive(true);
                //�ڸ���Ʈ UI ��ġ�� ���� ����
                teleportCircleUI.position = hitInfo.point;
                teleportCircleUI.forward = hitInfo.normal;
                //�ڷ���Ʈ�� UI�� �Ÿ������� �����ǵ��� ����
                teleportCircleUI.localScale = originScale *
                    Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                //Ray�浹�� �߻����� ������ 
                //���η������� ������ 200���� ����
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandPosition * 200);
                //�ڷ���Ʈ�� ȭ�鿡�� ��Ȱ��ȭ 
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
        
    }
    IEnumerator Warp()
    {
        print("���� �ڷ�ƾ �Լ�");
        MotionBlur blur; //���� ������ ǥ���� ��Ǻ���
        Vector3 pos = transform.position; //���� �������� 
        Vector3 targetPos = teleportCircleUI.position + Vector3.up; //������
        float currentTime = 0; //���� ��� �ð�
        //����Ʈ ���μ��̿��� ��� ���� �������Ͽ��� ��Ǻ��� ������
        post.profile.TryGetSettings<MotionBlur>(out blur);
        blur.active = true;//���� ������ ���� Ȱ��ȭ
        GetComponent<CharacterController>().enabled = false;
        //��� �ð��� �������� ª�� �ð����� �̵� ó��
        while (currentTime < warpTime)
        {
            currentTime += Time.deltaTime; //��� �ð� ���
            //���� ���������� �������� �����ϱ� ���� �����ð� ���� �̵�
            transform.position = Vector3.Lerp(pos, targetPos,
                currentTime / warpTime);
            yield return null; //�ڷ�ƾ ���
        }
        //�ڷ���Ʈ UI��ġ�� ���� �̵�
        transform.position = teleportCircleUI.position + Vector3.up;
        //ĳ���� ��Ʈ�ѷ� �ٽ� �ѱ�
        GetComponent<CharacterController>().enabled = true;
        blur.active = false; //����Ʈ ȿ�� ũ��
    }

}
