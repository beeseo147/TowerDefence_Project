using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -------------------- Constancy Data -------------------- */
// 작성자 : 윤여진
// 플레이어 기본 클래스
// 기능 : 플레이어 이름, 플레이어 공격력, 플레이어 크리티컬 확률, 플레이어 크리티컬 데미지, 플레이어 적 처치 횟수
[CreateAssetMenu(menuName = "Config/PlayerStatBase")]
public class PlayerStatBaseSO : ScriptableObject
{
    public string baseName           = "NoName"; // 플레이어 이름
    public int    baseAttackDamage   = 2; // 플레이어 공격력
    public float  baseCritChance     = 0.03f; // 플레이어 크리티컬 확률
    public float  baseCritMultiplier = 2f;  // 플레이어 크리티컬 데미지
    public int    baseEnemyKillCount = 0; // 플레이어 적 처치 횟수
}
