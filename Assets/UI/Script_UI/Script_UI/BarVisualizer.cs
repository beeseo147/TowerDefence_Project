using UnityEngine;
using UnityEngine.UI;

// 작성자 : 현주옥
// 바 비주얼라이저 클래스
// 기능 : 바 비주얼라이저 설정, 바 비주얼라이저 시작, 바 비주얼라이저 정지, 바 비주얼라이저 일시정지/재개, 바 비주얼라이저 최소값 설정, 바 비주얼라이저 최대값 설정, 바 비주얼라이저 변화 속도 설정, 바 비주얼라이저 현재 값 반환, 바 비주얼라이저 활성화 상태 반환, 바 비주얼라이저 바 이미지 설정, 볼륨 연동 기능 메서드들
public class BarVisualizer : MonoBehaviour
{
    [Header("바 비주얼라이저 설정")]
    [SerializeField] private Image uiBarImage; // 바 이미지
    [SerializeField] private float uiMinValue = 0f; // 최소값
    [SerializeField] private float uiMaxValue = 1f; // 최대값
    [SerializeField] private float uiChangeSpeed = 2f; // 변화 속도
    [SerializeField] private float uiSmoothness = 5f; // 부드러움 정도
    [SerializeField] private bool uiAutoStart = true; // 자동 시작
    
    private float uiCurrentValue; // 현재 값
    private float uiTargetValue; // 목표 값
    private bool uiIsActive = false;
    
    // 볼륨 연동 기능 추가
    [Header("볼륨 연동 설정")]
    [SerializeField] private bool uiUseVolumeReference = false; // 볼륨 참조 사용 여부
    [SerializeField] private bool uiUseVolumeAsMax = true; // 볼륨을 최대값으로 사용
    [SerializeField] private float uiVolumeMultiplier = 1f; // 볼륨 배수
    
    void Start()
    {
        // Image 컴포넌트 자동 찾기
        if (uiBarImage == null)
        {
            uiBarImage = GetComponent<Image>();
        }
        
        if (uiBarImage == null)
        {
            Debug.LogError("BarVisualizer: Image 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 초기값 설정
        uiCurrentValue = uiMinValue;
        uiTargetValue = uiMinValue;
        uiBarImage.fillAmount = uiCurrentValue;
        
        if (uiAutoStart)
        {
            uiStartVisualizer();
        }
    }

    // 바 비주얼라이저 업데이트
    void Update()
    {
        if (!uiIsActive || uiBarImage == null)
            return;
        
        // 볼륨 참조 사용 시 볼륨값에 따라 최대값 조정
        if (uiUseVolumeReference)
        {
            float uiVolumeValue = uiGetCurrentVolume();
            if (uiUseVolumeAsMax)
            {
                uiMaxValue = uiVolumeValue * uiVolumeMultiplier;
            }
            else
            {
                uiTargetValue = uiVolumeValue * uiVolumeMultiplier;
            }
        }
        
        // 목표값 업데이트 (랜덤)
        if (Mathf.Abs(uiCurrentValue - uiTargetValue) < 0.01f)
        {
            uiTargetValue = Random.Range(uiMinValue, uiMaxValue);
        }
        
        // 부드러운 값 변화
        uiCurrentValue = Mathf.Lerp(uiCurrentValue, uiTargetValue, Time.deltaTime * uiSmoothness);
        
        // 바 이미지 업데이트
        uiBarImage.fillAmount = uiCurrentValue;
    }
    
    /// <summary>
    /// 비주얼라이저 시작
    /// </summary>
    public void uiStartVisualizer()
    {
        uiIsActive = true;
    }
    
    /// <summary>
    /// 비주얼라이저 정지
    /// </summary>
    public void uiStopVisualizer()
    {
        uiIsActive = false;
    }
    
    /// <summary>
    /// 비주얼라이저 일시정지/재개
    /// </summary>
    public void uiToggleVisualizer()
    {
        uiIsActive = !uiIsActive;
    }
    
    /// <summary>
    /// 최소값 설정
    /// </summary>
    /// <param name="minValue">최소값</param>
    public void uiSetMinValue(float minValue)
    {
        uiMinValue = Mathf.Clamp01(minValue);
        uiMaxValue = Mathf.Max(uiMinValue, uiMaxValue);
    }
    
    /// <summary>
    /// 최대값 설정
    /// </summary>
    /// <param name="maxValue">최대값</param>
    public void uiSetMaxValue(float maxValue)
    {
        uiMaxValue = Mathf.Clamp01(maxValue);
        uiMinValue = Mathf.Min(uiMinValue, uiMaxValue);
    }
    
    /// <summary>
    /// 변화 속도 설정
    /// </summary>
    /// <param name="speed">변화 속도</param>
    public void uiSetChangeSpeed(float speed)
    {
        uiSmoothness = Mathf.Max(0.1f, speed);
    }
    
    /// <summary>
    /// 현재 값 반환
    /// </summary>
    /// <returns>현재 값</returns>
    public float uiGetCurrentValue()
    {
        return uiCurrentValue;
    }
    
    /// <summary>
    /// 활성화 상태 반환
    /// </summary>
    /// <returns>활성화 여부</returns>
    public bool uiIsVisualizerActive()
    {
        return uiIsActive;
    }
    
    /// <summary>
    /// 바 이미지 설정
    /// </summary>
    /// <param name="image">이미지 컴포넌트</param>
    public void uiSetBarImage(Image image)
    {
        uiBarImage = image;
    }
    
    // 볼륨 연동 기능 메서드들
    /// <summary>
    /// 현재 볼륨값 가져오기
    /// </summary>
    /// <returns>현재 볼륨값 (0~1)</returns>
    private float uiGetCurrentVolume()
    {
        if (VolumeController.Instance != null)
        {
            return VolumeController.Instance.UICurrentVolume;
        }
        return 1f; // VolumeController가 없으면 기본값 1 반환
    }
    
    /// <summary>
    /// 볼륨 참조 사용 설정
    /// </summary>
    /// <param name="useVolume">볼륨 참조 사용 여부</param>
    public void uiSetUseVolumeReference(bool useVolume)
    {
        uiUseVolumeReference = useVolume;
    }
    
    /// <summary>
    /// 볼륨을 최대값으로 사용 설정
    /// </summary>
    /// <param name="useAsMax">볼륨을 최대값으로 사용 여부</param>
    public void uiSetUseVolumeAsMax(bool useAsMax)
    {
        uiUseVolumeAsMax = useAsMax;
    }
    
    /// <summary>
    /// 볼륨 배수 설정
    /// </summary>
    /// <param name="multiplier">볼륨 배수</param>
    public void uiSetVolumeMultiplier(float multiplier)
    {
        uiVolumeMultiplier = Mathf.Max(0f, multiplier);
    }
    
    /// <summary>
    /// 볼륨 참조 사용 여부 반환
    /// </summary>
    /// <returns>볼륨 참조 사용 여부</returns>
    public bool uiIsUsingVolumeReference()
    {
        return uiUseVolumeReference;
    }
} 