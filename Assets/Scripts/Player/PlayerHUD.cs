using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static Meta.WitAi.WitTexts;

/* Player UI */
public class PlayerHUD : MonoBehaviour
{
    [Header("Widgets")]
    [SerializeField] Text nameText;
    [SerializeField] Text inventoryNameText;
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

    // ������ �ߺ� ��� �� �ߺ� �Լ� ȣ�� ����
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
        nameText.text = target.Runtime.Name;
        inventoryNameText.text = target.Runtime.Name;
        attackDamageText.text = target.Runtime.Attack.ToString();
        critChanceText.text = target.Runtime.CritChance.ToString("P0");
    }
}
