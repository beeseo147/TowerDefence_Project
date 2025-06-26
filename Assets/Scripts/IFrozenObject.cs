using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFrozenObject
{
    void Freeze();
    void Unfreeze();
    IEnumerator UnfreezeCoroutine();
}

    //public void Freeze()
    //{
    //    print("Freeze() 호출됨");
    //    //얼음 효과 재생
    //    //속도 감소
    //    agent.speed *= 0.2f;
    //}
    //public void Unfreeze()
    //{
    //    print("Unfreeze() 호출됨");
    //    //얼음 효과 종료
    //    //속도 증가
    //    agent.speed *= 5f;
    //}
    //public IEnumerator UnfreezeCoroutine()
    //{
    //    Freeze();
    //    yield return new WaitForSeconds(10f);
    //    Unfreeze();
    //}