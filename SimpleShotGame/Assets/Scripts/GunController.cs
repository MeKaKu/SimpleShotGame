using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun initGun;
    public Transform weaponHolder;
    public Gun equippedGun{get;private set;}
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
    //射击
    public void Shoot(){
        if(equippedGun != null){
            equippedGun.Shoot();
        }
    }
    //瞄准
    public void AimAt(Vector3 _point){
        if(equippedGun != null){
            equippedGun.Aim(_point);
        }
    }
    //换弹
    public void ReloadMagazine(){
        if(equippedGun != null){
            equippedGun.ReloadMagazine();
        }
    }

    //武器的高度
    public float gunHeight{
        get{
            return weaponHolder.position.y;
        }
    }
}
