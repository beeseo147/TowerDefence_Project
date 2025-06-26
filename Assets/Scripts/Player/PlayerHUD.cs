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
        nameText.text         = ($"이름 : {target.Runtime.name.ToString()}");
        attackDamageText.text = ($"공격력 : {target.Runtime.Attack.ToString()}");
        critChanceText.text   = ($"치명타 확률 : {target.Runtime.CritChance:P0}%");
    }
}
