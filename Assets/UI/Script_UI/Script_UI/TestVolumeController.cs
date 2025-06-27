using UnityEngine;

public class TestVolumeController : MonoBehaviour
{
    void Start()
    {
        // VolumeController가 존재하는지 확인
        if (VolumeController.Instance != null)
        {
            Debug.Log("VolumeController가 정상적으로 작동합니다!");
        }
        else
        {
            Debug.LogError("VolumeController를 찾을 수 없습니다!");
        }
    }
} 