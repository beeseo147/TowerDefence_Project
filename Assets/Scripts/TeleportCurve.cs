using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    public Transform teleportCircleUI; //텔레포트를 표시할 UI
    LineRenderer lr;//선을 그릴 라인렌더러
    Vector3 originScale = Vector3.one * 0.02f; //최초 텔레포트 UI크기
    public int lineSmooth = 40; //커브의 부드러운 정도
    public float curveLength = 50; //커브의 길이
    public float gravity = -60; // 커브의 중력
    public float simulateTime = 0.02f;//곡션 시뮬레이션의 간격 및 시간
    List<Vector3> lines = new List<Vector3>();//곡선을 이루는 점들을 기억할 리스트
    // Start is called before the first frame update
    void Start()
    {
        //시작할 때 비활성화 한다.
        teleportCircleUI.gameObject.SetActive(false);
        //라인 렌더러 컴포넌트 얻어오기
        lr = GetComponent<LineRenderer>();
        //라인렌더러의 선 너비 지정
        lr.startWidth = 0.0f;
        lr.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true; //렌더러 활성화
        }
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false; //렌더러 비활성화
            if (teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;
            }
            teleportCircleUI.gameObject.SetActive(false);
        }
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            MakeLines();
        }
    }
    void MakeLines() //라인렌더러를 이용해 점은 만들고 선을 그린다.
    {
        lines.RemoveRange(0, lines.Count); //리스트에 담긴 위치 정보들을 비워준다.
        Vector3 dir = ARAVRInput.LHandDirection * curveLength; //선이 진행될 방향 설정
        Vector3 pos = ARAVRInput.LHandPosition; //선이 그려질 위치 초기값 설정
        lines.Add(pos); //최초 위치를 리스트에 등록

        for(int i=0; i<lineSmooth; i++) {
            Vector3 lastPos = pos; //현재 위치 기억
            //미래의 속도(v) = 현재 속도(v0) + 가속도(a) * 시간
            //v = v0+at 
            dir.y += gravity * simulateTime;
            pos += dir * simulateTime; // 등속 운동으로 다음 위치 계산
            if (CheckHitRay(lastPos, ref pos)) //Ray 충돌체크가 일어났으면..
            {
                lines.Add(pos); //충돌 지점을 등록하고 종료
                break;
            }
            else
            {
                //텔레포트 UI 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
        //라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당
        lr.positionCount = lines.Count;
        //라인 렌더러에 구해진 점의 정보를 저장
        lr.SetPositions(lines.ToArray());
    }
    private bool CheckHitRay(Vector3 lastPos,  ref Vector3 pos)
    {
        //앞 점의 위치와 다음 점의 위치를 받아 레이의 충돌을 체크
        Vector3 rayDir = pos - lastPos;
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;
        //RayCast할 때 레이의 크기를 앞 점과 다음 점 사이의 거리로 한정한다.
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            pos = hitInfo.point;
            int layer = LayerMask.NameToLayer("Terrain");
            //Terrain 레이어와 충돌했을 경우에만 텔레포트 UI가 표시되도록 한다.
            if (hitInfo.transform.gameObject.layer == layer)
            {
                //텔레포트 UI활성화
                teleportCircleUI.gameObject.SetActive(true);
                //텔레포트 UI의 위치 지정
                teleportCircleUI.position = pos;
                //텔레포트 UI의 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos-ARAVRInput.LHandPosition).magnitude;
                //텔레포트 UI가 보일 크기를 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);
            }
            return true;
        }
        return false;
    }
}
