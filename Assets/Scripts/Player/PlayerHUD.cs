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

    private PlayerStatController target;


    void Awake()
    {
        target = GetComponent<PlayerStatController>();
        if (target == null)
        {
            Debug.LogError($"PlayerHUD Awake() : PlayerStatController is Null");
            return;
        }
    }


    void OnEnable()
    {
        if (target.Runtime != null)
        {
            target.Runtime.OnChanged.AddListener(Refresh);
            Refresh();
        }
    }

    // 리스너 중복 등록 및 중복 함수 호출 방지
    void OnDisable()
    {
        if (target != null && target.Runtime != null)
            target.Runtime.OnChanged.RemoveListener(Refresh);
    }

    void Start()
    {
        if (target && target.Runtime)
        {
            target.Runtime.OnChanged.AddListener(Refresh);
            Refresh();
        }
        else
            Debug.LogError("PlayerHUD: target or Runtime missing");
    }

    void Refresh()
    {
        nameText.text = ($"{target.Runtime.name.ToString()}");
        attackDamageText.text = ($"{target.Runtime.Attack.ToString()}");
        critChanceText.text = ($"{target.Runtime.CritChance:P0}%");
    }
}
