using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI uiVolumeText;
    public Image uiVolumeRadialFill;
    
    [Header("Display Settings")]
    public bool uiShowPercentage = true;
    public string uiVolumeFormat = "0";
    
    private VolumeController uiVolumeController;
    
    void Start()
    {
        uiVolumeController = VolumeController.Instance;
        if (uiVolumeController == null)
        {
            Debug.LogError("VolumeController를 찾을 수 없습니다!");
            return;
        }
        
        UpdateVolumeDisplay();
    }
    
    void Update()
    {
        UpdateVolumeDisplay();
    }
    
    void UpdateVolumeDisplay()
    {
        if (uiVolumeController == null) return;
        
        float uiCurrentVolume = uiVolumeController.UICurrentVolume;
        
        // 텍스트 업데이트 (100~0 범위)
        if (uiVolumeText != null)
        {
            if (uiShowPercentage)
            {
                int uiVolumePercentage = Mathf.RoundToInt(uiCurrentVolume * 100f);
                uiVolumeText.text = uiVolumePercentage.ToString(uiVolumeFormat);
            }
            else
            {
                uiVolumeText.text = uiCurrentVolume.ToString("F1");
            }
        }
        
        // Radial Fill 업데이트 (0~1 범위)
        if (uiVolumeRadialFill != null)
        {
            uiVolumeRadialFill.fillAmount = uiCurrentVolume;
        }
    }
    
    // 외부에서 강제로 업데이트할 때 사용
    public void ForceUpdateDisplay()
    {
        UpdateVolumeDisplay();
    }
} 