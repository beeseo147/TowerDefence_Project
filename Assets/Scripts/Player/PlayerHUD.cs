using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Meta.WitAi.WitTexts;

/* Player UI */
public class PlayerHUD : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] Text nameText;
    [SerializeField] Text attackDamageText;
    [SerializeField] Text critChanceText;

    [Header("Target")]
    [SerializeField] PlayerStatController target;

    void OnDisable() => target.Runtime.OnChanged -= Refresh;

    void Awake()
    {
        if (target == null)
            target = GetComponentInParent<PlayerStatController>();
    }
    void Start()
    {
        if (target && target.Runtime)
        {
            target.Runtime.OnChanged += Refresh;
            Refresh();
        }
        else
            Debug.LogError("PlayerHUD: target or Runtime missing");
    }

    void Refresh()
    {
        nameText.text         = ($"�̸� : {target.Runtime.name.ToString()}");
        attackDamageText.text = ($"���ݷ� : {target.Runtime.Attack.ToString()}");
        critChanceText.text   = ($"ġ��Ÿ Ȯ�� : {target.Runtime.CritChance:P0}%");
    }
}
