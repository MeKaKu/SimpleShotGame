using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;//枪口
    public float projectileSpeed = 35;//子弹深速度
    public Projectile projectile;//子弹对象
    public float msBetweenShoot = 100;//射击间隔

    private float nextShootTime;

    public void Shoot(){
        if(Time.time > nextShootTime){
            nextShootTime = Time.time + msBetweenShoot / 1000;//Time.time单位是s
            Projectile newProjectile = Instantiate<Projectile>(projectile, muzzle.position,muzzle.rotation);
            newProjectile.SetSpeed(projectileSpeed);
            
        }
    }
}
