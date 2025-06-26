using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFrozenObject
{
    void Freeze();
    void Unfreeze();
    IEnumerator UnfreezeCoroutine();
}