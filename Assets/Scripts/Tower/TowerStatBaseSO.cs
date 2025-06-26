using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/TowerStatBase")]
public class TowerStatBaseSO : ScriptableObject
{
    /* ---------- Constancy Data ---------- */

    [Header("���")]
    public int baseMaxHP = 100;
    public int baseShieldCharges = 1;
    public float baseShieldDuration = 3f;
}
