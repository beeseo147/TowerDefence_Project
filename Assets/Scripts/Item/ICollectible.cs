using UnityEngine;

// 작성자 : 김동균
// 수집 가능한 아이템 인터페이스
// 기능 : 아이템 수집 처리, 아이템 수집 가능 여부 확인
public interface ICollectible
{
    void Collect(GameObject collector);
    bool CanBeCollected(GameObject collector);
} 