using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/TowerStatBase")]
public class TowerStatBaseSO : ScriptableObject
{
    /* ---------- Constancy Data ---------- */

    [Header("¹æ¾î")]
    public float baseMaxHP          = 100;
    public bool  baseGodMode        = false;
    public int   baseShieldCharges  = 1;
    public float baseShieldDuration = 3f;
}
