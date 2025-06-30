using Meta.WitAi.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

// 작성자 : 공통
//사용하지않음
public class GrabBomb : MonoBehaviour
{
    bool isGrabbing = false; //��ü�� ��� �ִ��� ����
    GameObject grabbedObject; // ��� �ִ� ��ü
    public LayerMask grabbedLayer; //���� ��ü�� ����
    public float grabRange = 0.2f; //���� �� �ִ� �Ÿ�

    Vector3 prevPos; //���� ��ġ
    Quaternion prevRot; //���� ����
    float throwPower = 10; //���� ��

    public bool isRemoteGrab = true; //���Ÿ����� ��ü�� ��� ��� Ȱ��ȭ ����
    public float remoteGrabDistance = 20; //���Ÿ����� ��ü�� ���� �� �ִ� �Ÿ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbing == false) //��ü�� ��� ���� ��
        {
            TryGrab();
        }
        else
        {
            TryUnGrab();
        }
    }
    void TryUnGrab()
    {
       //  print(ARAVRInput.RHandPosition + "    /     " + prevPos);
        //���� ����
        Vector3 throwDirection = (ARAVRInput.RHandPosition - prevPos);
        ///  print(ARAVRInput.RHandPosition + "    /     " + prevPos);
        //  prevPos = ARAVRInput.RHandPosition; //��ġ ���
        //��ư�� ���Ҵٸ�
        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger,
           ARAVRInput.Controller.RTouch))
        {

            isGrabbing = false; //���� ���� ���·� ��ȯ
            //���� ��� Ȱ��ȭ
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            //�տ��� ��ź �����
            grabbedObject.transform.parent = null;

            //������ 
            //grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;
            grabbedObject.GetComponent<Rigidbody>().velocity = ARAVRInput.RHandDirection * throwPower;

            //���� ��ü ������ ����
            grabbedObject = null;
            

        }
    }
    void TryGrab()
    {
        //������ �ڵ�Ʈ���� ��ư�� �����ٸ�
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger,
           ARAVRInput.Controller.RTouch))
        {

            if (isRemoteGrab) //���Ÿ� ��ü ��⸦ ����Ѵٸ�
            {
                //������ �������� Ray ����
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;
                //������ ��(0.5������)�� �߻��Ͽ� �浹�� ������Ʈ ������ hitInfo�� ����
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance,
                    grabbedLayer))
                {
                    isGrabbing = true;
                    grabbedObject = hitInfo.transform.gameObject;
                    StartCoroutine(GrabbingAnimator());
                }
                return;
            }


            //�������� �ȿ� �ִ� ��� ��ź ����
            Collider[] hitObjects = Physics.OverlapSphere
                (ARAVRInput.RHandPosition, grabRange, grabbedLayer);
            int closest = 0; //���� ����� ��Ż �ε���
            for (int i = 1; i < hitObjects.Length; i++)
            {
                //�հ� ���� ����� ��ü���� �Ÿ�
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos,
                    ARAVRInput.RHandPosition);
                //���� ��ü�� ���� �Ÿ�
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos,
                    ARAVRInput.RHandPosition);
                //���� ��ü���� �Ÿ��� �� �����ٸ�
                if (nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
            if (hitObjects.Length > 0) //����� ������Ʈ�� �ִ� ���
            {
                isGrabbing = true; //���� ���·� ��ȯ
                //���� ��ü ����
                grabbedObject = hitObjects[closest].gameObject;
                //���� ��ü�� �������� �ڽ����� ���
                grabbedObject.transform.parent = ARAVRInput.RHand;
                //���� ��� ����
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                //���� ������ ���ϱ� ���� ������ ���� ��ġ ����
                prevPos = ARAVRInput.RHandPosition; 
            }
        }
        IEnumerator GrabbingAnimator()
        {
            //���� ��� ����
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            prevPos = ARAVRInput.RHandPosition ; //�ʱ� ��ġ �� ����
            prevRot = ARAVRInput.RHand.rotation; //�ʱ� ȸ�� �� ����
            Vector3 startLocation = grabbedObject.transform.position;
            Vector3 targetLocation = ARAVRInput.RHandPosition +
                ARAVRInput.RHandDirection * 0.1f;

            float currentTime = 0; //�ð��� ��� ���� ����
            float finishTime = 0.2f; //0.2 �ʵ��� ��ü�� ����´�
            float elapsedRate = currentTime / finishTime;//�����
            while (elapsedRate < 1)
            {
                currentTime += Time.deltaTime; //�ð��� ���
                elapsedRate = currentTime / finishTime; //����ð�/0.2�� = �����
                grabbedObject.transform.position = Vector3.Lerp(startLocation,
                    targetLocation, elapsedRate);
                yield return null;
            }
            //���� ��ü�� ���� �ڽ����� ���
            grabbedObject.transform.position = targetLocation;
            grabbedObject.transform.parent = ARAVRInput.RHand;
        }
    }
}
