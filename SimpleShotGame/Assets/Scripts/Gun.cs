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
    //后坐力recoil
    [Header("--后坐力")]
    public Vector2  positionRecoil = new Vector2(.1f, .2f);//水平位移后坐力
    public float positionRecoilRecoverTime = .1f;//水平位移后坐力的恢复时间
    private Vector3 tempPositionRecoil;
    public Vector2 rotationRecoil = new Vector2(3, 6);//枪口上扬的后坐力
    public float maxRotationRecoil = 30;//枪口最大上扬角度
    public float rotationRecoilRecoverTime = .1f;//枪口上扬恢复时间
    private float tempRotationRecoil;
    private float gunRotationValue = 0;


    FireFlash fireFlash;

    private void Start() {
        shellPoint = transform.Find("ShellPoint");
        fireFlash = GetComponent<FireFlash>();
    }
    private void Update() {
        //处理后坐力，恢复回原状态
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref tempPositionRecoil, positionRecoilRecoverTime);
        gunRotationValue = Mathf.SmoothDamp(gunRotationValue, 0, ref tempRotationRecoil, rotationRecoilRecoverTime);
        transform.localEulerAngles += Vector3.left * gunRotationValue;
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
            //后坐力
            transform.localPosition += Vector3.back * Random.Range(positionRecoil.x, positionRecoil.y);
            gunRotationValue += Random.Range(rotationRecoil.x, rotationRecoil.y);
            gunRotationValue = Mathf.Clamp(gunRotationValue, 0, maxRotationRecoil);
        }
    }
    //瞄准
    public void Aim(Vector3 _point){
        transform.LookAt(_point);
    }
}
