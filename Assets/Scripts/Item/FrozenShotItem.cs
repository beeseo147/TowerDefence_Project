using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenShotItem : MonoBehaviour, IUseItem
{
    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("UseItem")]
    public void UseItem(GameObject player)
    {
        Debug.Log("FrozenShotItem Use");
        //Gun무기에 효과 부여
        var gun = player.GetComponent<Gun>();
        if(gun != null)
        {
            print("Gun found, applying FrozenShot effect.");
            gun.FreezeShot();
        }
        else
        {
            print("Gun not found.");
        }
    }
}
