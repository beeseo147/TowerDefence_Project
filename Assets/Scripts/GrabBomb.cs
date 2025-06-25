using Meta.WitAi.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class GrabBomb : MonoBehaviour
{
    bool isGrabbing = false; //물체를 잡고 있는지 여부
    GameObject grabbedObject; // 잡고 있는 물체
    public LayerMask grabbedLayer; //잡을 물체의 종류
    public float grabRange = 0.2f; //잡을 수 있는 거리

    Vector3 prevPos; //이전 위치
    Quaternion prevRot; //이전 방향
    float throwPower = 10; //던질 힘

    public bool isRemoteGrab = true; //원거리에서 물체는 잡는 기능 활성화 여부
    public float remoteGrabDistance = 20; //원거리에서 물체를 잡을 수 있는 거리

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbing == false) //물체를 잡고 있을 때
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
        //던질 방향
        Vector3 throwDirection = (ARAVRInput.RHandPosition - prevPos);
        ///  print(ARAVRInput.RHandPosition + "    /     " + prevPos);
        //  prevPos = ARAVRInput.RHandPosition; //위치 기억
        //버튼을 놓았다면
        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger,
           ARAVRInput.Controller.RTouch))
        {

            isGrabbing = false; //잡지 않은 상태로 전환
            //물리 기능 활성화
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            //손에서 폭탄 떼어내기
            grabbedObject.transform.parent = null;

            //던지기 
            //grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;
            grabbedObject.GetComponent<Rigidbody>().velocity = ARAVRInput.RHandDirection * throwPower;

            //잡은 물체 없도록 설정
            grabbedObject = null;
            

        }
    }
    void TryGrab()
    {
        //오른쪽 핸드트리거 버튼을 눌렀다면
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger,
           ARAVRInput.Controller.RTouch))
        {

            if (isRemoteGrab) //원거리 물체 잡기를 사용한다면
            {
                //오른손 방향으로 Ray 생성
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;
                //가상의 구(0.5반지름)를 발사하여 충돌한 오브젝트 정보를 hitInfo에 저장
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance,
                    grabbedLayer))
                {
                    isGrabbing = true;
                    grabbedObject = hitInfo.transform.gameObject;
                    StartCoroutine(GrabbingAnimator());
                }
                return;
            }


            //일정영역 안에 있는 모든 폭탄 검출
            Collider[] hitObjects = Physics.OverlapSphere
                (ARAVRInput.RHandPosition, grabRange, grabbedLayer);
            int closest = 0; //가장 가까운 폭탈 인덱스
            for (int i = 1; i < hitObjects.Length; i++)
            {
                //손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos,
                    ARAVRInput.RHandPosition);
                //다음 물체와 손의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos,
                    ARAVRInput.RHandPosition);
                //다음 물체와의 거리가 더 가깝다면
                if (nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
            if (hitObjects.Length > 0) //검출된 오브젝트가 있는 경우
            {
                isGrabbing = true; //잡은 상태로 전환
                //잡은 물체 저장
                grabbedObject = hitObjects[closest].gameObject;
                //잡은 물체를 오른손의 자식으로 등록
                grabbedObject.transform.parent = ARAVRInput.RHand;
                //물리 기능 정지
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                //던질 방향을 구하기 위한 오른손 이전 위치 저장
                prevPos = ARAVRInput.RHandPosition; 
            }
        }
        IEnumerator GrabbingAnimator()
        {
            //물리 기능 정지
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            prevPos = ARAVRInput.RHandPosition ; //초기 위치 값 지정
            prevRot = ARAVRInput.RHand.rotation; //초기 회전 값 지정
            Vector3 startLocation = grabbedObject.transform.position;
            Vector3 targetLocation = ARAVRInput.RHandPosition +
                ARAVRInput.RHandDirection * 0.1f;

            float currentTime = 0; //시간을 재기 위한 변수
            float finishTime = 0.2f; //0.2 초동안 물체를 끌어온다
            float elapsedRate = currentTime / finishTime;//경과율
            while (elapsedRate < 1)
            {
                currentTime += Time.deltaTime; //시간을 재고
                elapsedRate = currentTime / finishTime; //현재시간/0.2초 = 경과율
                grabbedObject.transform.position = Vector3.Lerp(startLocation,
                    targetLocation, elapsedRate);
                yield return null;
            }
            //잡은 물체를 손의 자식으로 등록
            grabbedObject.transform.position = targetLocation;
            grabbedObject.transform.parent = ARAVRInput.RHand;
        }
    }
}
