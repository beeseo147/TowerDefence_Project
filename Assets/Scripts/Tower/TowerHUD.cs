using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TowerHUD : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] Image hpFillBarIamage;
    [SerializeField] Image shieldIamage;
    [SerializeField] Image hpFillBarImagaeInventory;
    [SerializeField] Text hpText;
    [SerializeField] Text shieldTimeText;
    [SerializeField] Text shieldCountText;

    // Start is called before the first frame update
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
        }
        else
            Debug.LogError("PlayerHUD: target or Runtime missing");
    }

    public void UpdateHP(float curHp, float maxHp)
    {
        hpFillBarIamage.fillAmount = curHp / maxHp;
        hpFillBarImagaeInventory.fillAmount = curHp / maxHp;
        hpText.text = $"{curHp}/{maxHp}";
    }

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
    public void UpdateShieldCount(int count)
    {
        shieldCountText.text = count.ToString();
    }
}