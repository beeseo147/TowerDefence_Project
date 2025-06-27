using UnityEngine;
using System.Collections;

public class ScaleAnimation : MonoBehaviour
{
    [Header("스케일 애니메이션 설정")]
    [SerializeField] private float uiAnimationDuration = 0.5f; // 애니메이션 지속 시간
    [SerializeField] private AnimationCurve uiScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 애니메이션 커브
    [SerializeField] private bool uiAutoStartOnEnable = true; // 활성화 시 자동 시작
    
    private Vector3 uiOriginalScale; // 원래 크기
    private bool uiIsAnimating = false;
    
    void OnEnable()
    {
        if (uiAutoStartOnEnable)
        {
            uiStartScaleAnimation();
        }
    }
    
    void Start()
    {
        // 원래 크기 저장
        uiOriginalScale = transform.localScale;
    }
    
    /// <summary>
    /// 스케일 애니메이션 시작
    /// </summary>
    public void uiStartScaleAnimation()
    {
        if (!uiIsAnimating)
        {
            StartCoroutine(uiScaleAnimationCoroutine());
        }
    }
    
    /// <summary>
    /// 스케일 애니메이션 코루틴
    /// </summary>
    private IEnumerator uiScaleAnimationCoroutine()
    {
        uiIsAnimating = true;
        
        // 시작: 가운데에서 시작 (Y축만 0, X축은 원래 크기)
        Vector3 uiStartScale = new Vector3(uiOriginalScale.x, 0f, uiOriginalScale.z);
        transform.localScale = uiStartScale;
        
        float uiElapsedTime = 0f;
        
        while (uiElapsedTime < uiAnimationDuration)
        {
            uiElapsedTime += Time.deltaTime;
            float uiProgress = uiElapsedTime / uiAnimationDuration;
            
            // 진행률을 0-1 범위로 제한
            uiProgress = Mathf.Clamp01(uiProgress);
            
            // 애니메이션 커브 적용
            float uiCurvedProgress = uiScaleCurve.Evaluate(uiProgress);
            
            // Y축만 애니메이션 (위아래로 퍼짐)
            float uiCurrentY = Mathf.Lerp(0f, uiOriginalScale.y, uiCurvedProgress);
            transform.localScale = new Vector3(uiOriginalScale.x, uiCurrentY, uiOriginalScale.z);
            
            yield return null;
        }
        
        // 정확한 최종 크기로 설정
        transform.localScale = uiOriginalScale;
        
        uiIsAnimating = false;
    }
    
    /// <summary>
    /// 애니메이션 즉시 재시작
    /// </summary>
    public void uiRestartAnimation()
    {
        StopAllCoroutines();
        uiStartScaleAnimation();
    }
    
    /// <summary>
    /// 애니메이션 지속 시간 설정
    /// </summary>
    /// <param name="duration">지속 시간</param>
    public void uiSetAnimationDuration(float duration)
    {
        uiAnimationDuration = Mathf.Max(0.1f, duration);
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
    /// 원래 크기로 즉시 복원
    /// </summary>
    public void uiResetToOriginalScale()
    {
        transform.localScale = uiOriginalScale;
        uiIsAnimating = false;
    }
} 