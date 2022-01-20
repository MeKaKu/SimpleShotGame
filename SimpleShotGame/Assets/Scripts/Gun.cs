using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single};
    public FireMode fireMode;
    public Transform muzzle;//枪口
    public float projectileSpeed = 35;//子弹的速度
    public Projectile projectile;//子弹对象
    public Shell shell;//弹壳
    Transform shellPoint;//弹壳弹出点
    public float timeBetweenShoot = 0.1f;//射击间隔

    private float nextShootTime;

    FireFlash fireFlash;

    private void Start() {
        shellPoint = transform.Find("ShellPoint");
        fireFlash = GetComponent<FireFlash>();
    }
    public void Shoot(){
        if(Time.time > nextShootTime){
            nextShootTime = Time.time + timeBetweenShoot;//Time.time单位是s
            Projectile newProjectile = Instantiate<Projectile>(projectile, muzzle.position,muzzle.rotation);
            newProjectile.SetSpeed(projectileSpeed);
            //弹出弹壳
            Instantiate<Shell>(shell, transform.position, transform.rotation);
            //枪口闪光
            fireFlash.Activate();
        }
    }
}
