using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

// 작성자 : 윤여진
// 타워 HUD 클래스
// 기능 : 타워 체력 표시, 타워 쉴드 표시, 타워 쉴드 시간 표시, 타워 쉴드 개수 표시
public class TowerHUD : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] Image hpFillBarIamage;
    [SerializeField] Image shieldIamage;
    [SerializeField] Image hpFillBarImagaeInventory;
    [SerializeField] Text hpText;
    [SerializeField] Text shieldTimeText;
    [SerializeField] Text shieldCountText;

    // 타워 HUD 시작
    void Start()
    {
        if (Tower.Instance && Tower.Instance.Runtime)
        {
            var rt = Tower.Instance.Runtime;
            //Tower.Instance.Runtime.OnHpChanged.AddListener(UpdateHP);
            //Tower.Instance.Runtime.OnShield.AddListener(UpdateShieldTime);
            //Tower.Instance.Runtime.OnShieldAdded.AddListener(UpdateShieldCount);
            UpdateHP(rt.CurHp, rt.CurHp);
            //UpdateShieldTime(rt.maxShieldTime, rt.maxShieldTime);
            shieldCountText.text = Tower.Instance.Runtime.ShieldCharge.ToString();
        }
        else
            Debug.LogError("PlayerHUD: target or Runtime missing");
    }

    // 타워 체력 업데이트
    public void UpdateHP(float curHp, float maxHp)
    {
        hpFillBarIamage.fillAmount = curHp / maxHp;
        hpFillBarImagaeInventory.fillAmount = curHp / maxHp;
        hpText.text = $"{curHp}/{maxHp}";
    }

    // 타워 쉴드 시간 업데이트
    public void UpdateShieldTime(float remain, float maxvalue)
    {
        Debug.Log($"TowerHUD UpdateShield() : remainShield -> {remain}");
        // Start Shield
        if (0 < remain)
        {
            shieldIamage.fillAmount = remain / maxvalue;
            shieldTimeText.text = $"{remain:F1}";
        }
        // Finish Shield
        else
        {
            shieldIamage.fillAmount = maxvalue;
            shieldTimeText.text = maxvalue.ToString();
        }
    }

    // 타워 쉴드 개수 업데이트
    public void UpdateShieldCount(int count)
    {
        shieldCountText.text = count.ToString();
    }
}