using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 윤여진
// 타워 기본 클래스
// 기능 : 타워 체력, 타워 쉴드 처리, 타워 쉴드 시간 표시, 타워 쉴드 개수 표시
[CreateAssetMenu(menuName = "Config/TowerStatBase")]
public class TowerStatBaseSO : ScriptableObject
{
    /* ---------- Constancy Data ---------- */

    [Header("타워 체력")]
    public float baseMaxHP          = 100; // 타워 체력
    public bool  baseGodMode        = false; // 타워 쉴드 처리
    public int   baseShieldCharges  = 1; // 타워 쉴드 개수
    public float baseShieldDuration = 3f;
}
