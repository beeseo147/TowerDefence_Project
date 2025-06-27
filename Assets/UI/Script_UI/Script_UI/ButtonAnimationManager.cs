using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonAnimationManager : MonoBehaviour
{
    public enum AnimationDirection
    {
        LeftToRight,    // 왼쪽에서 오른쪽
        RightToLeft,    // 오른쪽에서 왼쪽
        TopToBottom,    // 위에서 아래
        BottomToTop     // 아래에서 위
    }
    
    [Header("애니메이션 방향 설정")]
    [SerializeField] private AnimationDirection uiAnimationDirection = AnimationDirection.LeftToRight;
    
    [Header("UI 요소 설정")]
    [SerializeField] private GameObject[] uiUIObjects; // UI GameObject 배열
    [SerializeField] private float uiSlideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간
    [SerializeField] private float uiDelayBetweenButtons = 0.2f; // 버튼 간 딜레이
    [SerializeField] private float uiSlideDistance = 500f; // 슬라이드 거리 (픽셀)
    [SerializeField] private AnimationCurve uiSlideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 애니메이션 커브
    [SerializeField] private bool uiAutoStartOnEnable = true; // 활성화 시 자동 시작
    
    private Button[] uiButtons; // 런타임에 추출된 Button 컴포넌트들
    private RectTransform[] uiRectTransforms; // 런타임에 추출된 RectTransform 컴포넌트들
    private Vector2[] uiOriginalPositions; // 원래 위치들
    private Vector2[] uiStartPositions; // 시작 위치들
    private bool uiIsAnimating = false;
    private bool uiIsInitialized = false; // 초기화 완료 여부
    
    void OnEnable()
    {
        // 초기화가 완료된 후에만 자동 시작
        if (uiAutoStartOnEnable && uiIsInitialized)
        {
            uiStartSequentialAnimation();
        }
    }
    
    void Start()
    {
        uiInitializeButtons();
    }
    
    /// <summary>
    /// 버튼 초기화
    /// </summary>
    private void uiInitializeButtons()
    {
        // 안전장치: 이미 초기화되었다면 중복 실행 방지
        if (uiIsInitialized)
            return;
            
        try
        {
            // UI GameObject 배열이 비어있으면 자동으로 찾기
            if (uiUIObjects == null || uiUIObjects.Length == 0)
            {
                // 자식 GameObject들을 모두 가져오기
                Transform[] uiChildTransforms = GetComponentsInChildren<Transform>();
                List<GameObject> uiChildObjects = new List<GameObject>();
                
                foreach (Transform uiChild in uiChildTransforms)
                {
                    if (uiChild != transform) // 자기 자신 제외
                    {
                        uiChildObjects.Add(uiChild.gameObject);
                    }
                }
                
                uiUIObjects = uiChildObjects.ToArray();
            }
            
            // 초기 설정 확인
            if (uiUIObjects == null || uiUIObjects.Length == 0)
            {
                Debug.LogWarning("ButtonAnimationManager: UI GameObject을 찾을 수 없습니다.");
                uiIsInitialized = true;
                return;
            }
            
            // 배열 초기화
            uiButtons = new Button[uiUIObjects.Length];
            uiRectTransforms = new RectTransform[uiUIObjects.Length];
            uiOriginalPositions = new Vector2[uiUIObjects.Length];
            uiStartPositions = new Vector2[uiUIObjects.Length];
            
            // 각 UI GameObject에서 컴포넌트 추출 및 위치 설정
            for (int i = 0; i < uiUIObjects.Length; i++)
            {
                if (uiUIObjects[i] != null)
                {
                    // Button 컴포넌트 추출
                    uiButtons[i] = uiUIObjects[i].GetComponent<Button>();
                    
                    // RectTransform 컴포넌트 추출
                    uiRectTransforms[i] = uiUIObjects[i].GetComponent<RectTransform>();
                    
                    if (uiRectTransforms[i] != null)
                    {
                        uiOriginalPositions[i] = uiRectTransforms[i].anchoredPosition;
                        
                        // 방향에 따른 시작 위치 설정
                        uiStartPositions[i] = uiGetStartPosition(uiOriginalPositions[i]);
                        
                        // 초기 위치를 시작 위치로 설정
                        uiRectTransforms[i].anchoredPosition = uiStartPositions[i];
                    }
                }
            }
            
            uiIsInitialized = true;
            
            // 초기화 완료 후 자동 시작
            if (uiAutoStartOnEnable)
            {
                uiStartSequentialAnimation();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("ButtonAnimationManager 초기화 중 오류: " + e.Message);
            uiIsInitialized = true;
        }
    }
    
    /// <summary>
    /// 방향에 따른 시작 위치 계산
    /// </summary>
    /// <param name="originalPosition">원래 위치</param>
    /// <returns>시작 위치</returns>
    private Vector2 uiGetStartPosition(Vector2 originalPosition)
    {
        switch (uiAnimationDirection)
        {
            case AnimationDirection.LeftToRight:
                return originalPosition + Vector2.left * uiSlideDistance;
            case AnimationDirection.RightToLeft:
                return originalPosition + Vector2.right * uiSlideDistance;
            case AnimationDirection.TopToBottom:
                return originalPosition + Vector2.up * uiSlideDistance;
            case AnimationDirection.BottomToTop:
                return originalPosition + Vector2.down * uiSlideDistance;
            default:
                return originalPosition + Vector2.left * uiSlideDistance;
        }
    }
    
    /// <summary>
    /// 모든 버튼을 순차적으로 애니메이션 시작
    /// </summary>
    public void uiStartSequentialAnimation()
    {
        // 초기화가 완료되지 않았으면 초기화 먼저 실행
        if (!uiIsInitialized)
        {
            uiInitializeButtons();
            return;
        }
        
        if (!uiIsAnimating && uiUIObjects != null && uiUIObjects.Length > 0)
        {
            StartCoroutine(uiSequentialAnimationCoroutine());
        }
    }
    
    /// <summary>
    /// 순차적 애니메이션 코루틴
    /// </summary>
    private IEnumerator uiSequentialAnimationCoroutine()
    {
        if (uiIsAnimating)
            yield break;
            
        uiIsAnimating = true;
        
        // 각 UI 요소를 순차적으로 애니메이션
        for (int i = 0; i < uiUIObjects.Length; i++)
        {
            if (uiUIObjects[i] != null)
            {
                StartCoroutine(uiSlideButtonAnimation(i));
                yield return new WaitForSeconds(uiDelayBetweenButtons);
            }
        }
        
        // 모든 애니메이션이 완료될 때까지 대기
        yield return new WaitForSeconds(uiSlideDuration + uiDelayBetweenButtons);
        
        uiIsAnimating = false;
    }
    
    /// <summary>
    /// 개별 UI 요소 슬라이드 애니메이션
    /// </summary>
    /// <param name="buttonIndex">UI 요소 인덱스</param>
    private IEnumerator uiSlideButtonAnimation(int buttonIndex)
    {
        // 안전장치: 인덱스 범위 확인
        if (buttonIndex < 0 || buttonIndex >= uiUIObjects.Length || uiUIObjects[buttonIndex] == null)
            yield break;
        
        if (uiRectTransforms[buttonIndex] == null)
            yield break;
        
        float uiElapsedTime = 0f;
        
        while (uiElapsedTime < uiSlideDuration)
        {
            uiElapsedTime += Time.deltaTime;
            float uiProgress = uiElapsedTime / uiSlideDuration;
            
            // 진행률을 0-1 범위로 제한
            uiProgress = Mathf.Clamp01(uiProgress);
            
            // 애니메이션 커브 적용
            float uiCurvedProgress = uiSlideCurve.Evaluate(uiProgress);
            
            // 위치 보간
            uiRectTransforms[buttonIndex].anchoredPosition = Vector2.Lerp(
                uiStartPositions[buttonIndex], 
                uiOriginalPositions[buttonIndex], 
                uiCurvedProgress
            );
            
            yield return null;
        }
        
        // 정확한 최종 위치로 설정
        uiRectTransforms[buttonIndex].anchoredPosition = uiOriginalPositions[buttonIndex];
    }
    
    /// <summary>
    /// 모든 애니메이션을 즉시 재시작
    /// </summary>
    public void uiRestartAllAnimations()
    {
        if (!uiIsInitialized || uiUIObjects == null)
            return;
            
        // 모든 UI 요소를 시작 위치로 되돌리기
        for (int i = 0; i < uiUIObjects.Length; i++)
        {
            if (uiUIObjects[i] != null && uiRectTransforms[i] != null)
            {
                uiRectTransforms[i].anchoredPosition = uiStartPositions[i];
            }
        }
        
        // 애니메이션 재시작
        uiStartSequentialAnimation();
    }
    
    /// <summary>
    /// 애니메이션 방향 설정
    /// </summary>
    /// <param name="direction">애니메이션 방향</param>
    public void uiSetAnimationDirection(AnimationDirection direction)
    {
        uiAnimationDirection = direction;
        
        // 시작 위치 재계산
        if (uiStartPositions != null && uiOriginalPositions != null)
        {
            for (int i = 0; i < uiUIObjects.Length; i++)
            {
                if (uiUIObjects[i] != null)
                {
                    uiStartPositions[i] = uiGetStartPosition(uiOriginalPositions[i]);
                }
            }
        }
    }
    
    /// <summary>
    /// 슬라이드 지속 시간 설정
    /// </summary>
    /// <param name="duration">지속 시간</param>
    public void uiSetSlideDuration(float duration)
    {
        uiSlideDuration = Mathf.Max(0.1f, duration); // 최소 0.1초 보장
    }
    
    /// <summary>
    /// 버튼 간 딜레이 설정
    /// </summary>
    /// <param name="delay">딜레이 시간</param>
    public void uiSetDelayBetweenButtons(float delay)
    {
        uiDelayBetweenButtons = Mathf.Max(0f, delay); // 음수 방지
    }
    
    /// <summary>
    /// 슬라이드 거리 설정
    /// </summary>
    /// <param name="distance">슬라이드 거리</param>
    public void uiSetSlideDistance(float distance)
    {
        uiSlideDistance = Mathf.Max(0f, distance); // 음수 방지
        
        // 시작 위치 재계산
        if (uiStartPositions != null && uiOriginalPositions != null)
        {
            for (int i = 0; i < uiUIObjects.Length; i++)
            {
                if (uiUIObjects[i] != null)
                {
                    uiStartPositions[i] = uiGetStartPosition(uiOriginalPositions[i]);
                }
            }
        }
    }
    
    /// <summary>
    /// 자동 시작 설정
    /// </summary>
    /// <param name="autoStart">자동 시작 여부</param>
    public void uiSetAutoStart(bool autoStart)
    {
        uiAutoStartOnEnable = autoStart;
    }
    
    /// <summary>
    /// 현재 애니메이션 상태 반환
    /// </summary>
    /// <returns>애니메이션 진행 중 여부</returns>
    public bool uiIsAnimationPlaying()
    {
        return uiIsAnimating;
    }
    
    /// <summary>
    /// 초기화 상태 반환
    /// </summary>
    /// <returns>초기화 완료 여부</returns>
    public bool uiGetInitializationStatus()
    {
        return uiIsInitialized;
    }
    
    /// <summary>
    /// UI 요소 개수 반환
    /// </summary>
    /// <returns>UI 요소 개수</returns>
    public int uiGetButtonCount()
    {
        return uiUIObjects != null ? uiUIObjects.Length : 0;
    }
    
    /// <summary>
    /// 현재 애니메이션 방향 반환
    /// </summary>
    /// <returns>현재 애니메이션 방향</returns>
    public AnimationDirection uiGetAnimationDirection()
    {
        return uiAnimationDirection;
    }
} 