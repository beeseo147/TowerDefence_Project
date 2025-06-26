using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TowerHUD : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] Image hpBarIamage;
    [SerializeField] Image shieldIamage;

    // Start is called before the first frame update
    void Start()
    {
        if (Tower.Instance && Tower.Instance.Runtime)
        {
            Tower.Instance.Runtime.OnHpChanged.AddListener(UpdateHP);
            UpdateHP(Tower.Instance.Runtime.CurHp, Tower.Instance.Runtime.CurHp);
        }
        else
            Debug.LogError("PlayerHUD: target or Runtime missing");
    }

    void UpdateHP(float curHp, float maxHp)
    {
        hpBarIamage.fillAmount = (float)curHp / maxHp;
    }
}
