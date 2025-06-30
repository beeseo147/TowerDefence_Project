using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 김동균
// 얼리기 인터페이스
// 기능 : 얼리기, 얼림 해제, 얼림 해제 코루틴
public interface IFrozenObject
{
    void Freeze();
    void Unfreeze();
    IEnumerator UnfreezeCoroutine();
}