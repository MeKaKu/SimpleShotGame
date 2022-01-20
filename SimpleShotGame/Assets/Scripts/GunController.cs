using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun initGun;
    public Transform weaponHolder;
    Gun equippedGun;
    void Start()
    {
        if(initGun!=null){
            EquipGun(initGun);
        }
    }

    public void EquipGun(Gun gunToEquip){
        if(equippedGun!=null){
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate<Gun>(gunToEquip,weaponHolder.position,weaponHolder.rotation);
        equippedGun.gameObject.transform.parent = weaponHolder;
    }

    public void Shoot(){
        if(equippedGun != null){
            equippedGun.Shoot();
        }
    }

    //武器的高度
    public float gunHeight{
        get{
            return weaponHolder.position.y;
        }
    }
}
