using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [Header("카메라 회전 설정")]
    [SerializeField] private float uiRotationSpeed = 10f; // 회전 속도 (도/초)
    [SerializeField] private bool uiAutoRotate = true; // 자동 회전 활성화
    
    private Transform uiCameraTransform;
    
    void Start()
    {
        // 카메라 Transform 컴포넌트 가져오기
        uiCameraTransform = transform;
        
        // 초기 설정 확인
        if (uiCameraTransform == null)
        {
            Debug.LogError("CameraRotator: 카메라 Transform을 찾을 수 없습니다.");
        }
    }
    
    void Update()
    {
        // 자동 회전이 활성화되어 있을 때만 회전
        if (uiAutoRotate)
        {
            RotateCamera();
        }
    }
    
    /// <summary>
    /// 카메라를 Y축을 중심으로 회전시킵니다.
    /// </summary>
    private void RotateCamera()
    {
        // Y축을 중심으로 회전 (위쪽 방향)
        uiCameraTransform.Rotate(0f, uiRotationSpeed * Time.deltaTime, 0f, Space.World);
    }
    
    /// <summary>
    /// 회전 속도를 설정합니다.
    /// </summary>
    /// <param name="speed">회전 속도 (도/초)</param>
    public void SetUIRotationSpeed(float speed)
    {
        uiRotationSpeed = speed;
    }
    
    /// <summary>
    /// 자동 회전을 켜거나 끕니다.
    /// </summary>
    /// <param name="enabled">활성화 여부</param>
    public void SetUIAutoRotate(bool enabled)
    {
        uiAutoRotate = enabled;
    }
    
    /// <summary>
    /// 현재 회전 속도를 반환합니다.
    /// </summary>
    /// <returns>현재 회전 속도</returns>
    public float GetUIRotationSpeed()
    {
        return uiRotationSpeed;
    }
    
    /// <summary>
    /// 자동 회전 상태를 반환합니다.
    /// </summary>
    /// <returns>자동 회전 활성화 여부</returns>
    public bool GetUIAutoRotate()
    {
        return uiAutoRotate;
    }
} 