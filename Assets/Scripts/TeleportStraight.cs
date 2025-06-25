using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TeleportStraight : MonoBehaviour
{
    //텔레포트를 표시할 UI
    public Transform teleportCircleUI;
    LineRenderer lr; //선을 그릴 렌더러 
    //최초의 텔레포트 UI의 크기
    Vector3 originScale = Vector3.one * 0.02f;
    public bool isWarp = false; //워프 사용 여부
    public float warpTime = 0.1f; //워프에 걸리는 시간
    public PostProcessVolume post; // 사용하고 있는 포스트프로세싱 볼륨 컴포넌트

    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //버튼을 눌렀다면
        if (ARAVRInput.GetDown(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true; //라인렌더러의 컴포넌트 활성화
        }
        //버튼에 손을 떼면
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false;//라인렌더러의 컴포넌트 비활성화
            if (teleportCircleUI.gameObject.activeSelf)
            {
                if (isWarp == false) //워프 기능이 사용중 아닐 때 순간이동
                {
                    //캐릭터 컨트롤러 컴포넌트 비활성화
                    GetComponent<CharacterController>().enabled = false;
                    //텔레포트 UI 위치로 순간 이동
                    transform.position = teleportCircleUI.position +
                        Vector3.up;
                    //캐릭터 컨트롤러 컴포넌트 비활성화
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    StartCoroutine(Warp());
                }
               
            }

            teleportCircleUI.gameObject.SetActive(false);
        }
            //왼쪽 컨트롤러의 One버튼 누르고 있을 때
            if (ARAVRInput.Get(ARAVRInput.Button.One,
            ARAVRInput.Controller.LTouch))
        {
            //왼손을 기준으로 레이를 생성
            Ray ray = new Ray(ARAVRInput.LHandPosition,
                ARAVRInput.LHandDirection);
            RaycastHit hitInfo;
            //Terrain만 Ray 충돌 검출
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            if (Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                //부딪힌 지점에 텔레포트 UI 표시
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                //텔레포트 UI 활성화
                teleportCircleUI.gameObject.SetActive(true);
                //텔리포트 UI 위치와 방향 설정
                teleportCircleUI.position = hitInfo.point;
                teleportCircleUI.forward = hitInfo.normal;
                //텔레포트의 UI가 거리에따라 보정되도록 설정
                teleportCircleUI.localScale = originScale *
                    Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                //Ray충돌이 발생하지 않으면 
                //라인렌더러의 끝점을 200으로 설정
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandPosition * 200);
                //텔레포트는 화면에서 비활성화 
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
        
    }
    IEnumerator Warp()
    {
        print("워프 코루틴 함수");
        MotionBlur blur; //워프 느낌을 표현할 모션블러
        Vector3 pos = transform.position; //워프 시작지점 
        Vector3 targetPos = teleportCircleUI.position + Vector3.up; //목적지
        float currentTime = 0; //워프 경과 시간
        //포스트 프로세싱에서 사용 중인 프로프일에서 모션블러 얻어오기
        post.profile.TryGetSettings<MotionBlur>(out blur);
        blur.active = true;//워프 시작전 블러 활성화
        GetComponent<CharacterController>().enabled = false;
        //경과 시간이 워프보다 짧은 시간동안 이동 처리
        while (currentTime < warpTime)
        {
            currentTime += Time.deltaTime; //경과 시간 재기
            //워프 시작점에서 도착점에 도착하기 위해 워프시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos,
                currentTime / warpTime);
            yield return null; //코루틴 대기
        }
        //텔레포트 UI위치로 순간 이동
        transform.position = teleportCircleUI.position + Vector3.up;
        //캐릭터 컨트롤러 다시 켜기
        GetComponent<CharacterController>().enabled = true;
        blur.active = false; //포스트 효과 크기
    }

}
