using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -------------------- Constancy Data -------------------- */
[CreateAssetMenu(menuName = "Config/PlayerStatBase")]
public class PlayerStatBaseSO : ScriptableObject
{
    public string baseName       = "NoName";
    public int baseAttackDamage  = 10;
    public float baseCritChance  = 0.05f;
}
