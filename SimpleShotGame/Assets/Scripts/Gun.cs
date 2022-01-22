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

    //弹匣
    public int magazineCapacity = 10;//弹匣容量
    public float reloadingTime = 1;//换弹时间
    private int projectsRemainedInMagazine;//弹匣内剩余子弹数目
    bool isReloadingMagazine;//是否正在换弹


    FireFlash fireFlash;

    private void Start() {
        shellPoint = transform.Find("ShellPoint");
        fireFlash = GetComponent<FireFlash>();
        projectsRemainedInMagazine = magazineCapacity;
    }
    private void LateUpdate() {
        //处理后坐力，恢复回原状态
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref tempPositionRecoil, positionRecoilRecoverTime);
        gunRotationValue = Mathf.SmoothDamp(gunRotationValue, 0f, ref tempRotationRecoil, rotationRecoilRecoverTime);
        transform.localEulerAngles += Vector3.left * gunRotationValue;
        if(projectsRemainedInMagazine <= 0 && !isReloadingMagazine){
            ReloadMagazine();
        }
    }
    public void Shoot(){
        if(Time.time > nextShootTime && projectsRemainedInMagazine > 0 && !isReloadingMagazine){
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
            //弹匣里面的子弹数减一
            projectsRemainedInMagazine--;
        }
    }
    //换弹
    public void ReloadMagazine(){
        if(!isReloadingMagazine && projectsRemainedInMagazine < magazineCapacity){
            isReloadingMagazine = true;
            StartCoroutine(AnimateReloadMagazine());
        }
    }
    //换弹动画
    IEnumerator AnimateReloadMagazine(){
        //yield return new WaitForSeconds(.2f);
        float percent = 0;
        float reloadingSpeed = 1f/reloadingTime;
        float maxAngle = 45;
        Vector3 originAngles = transform.localEulerAngles;
        while(percent < 1){
            percent += Time.deltaTime * reloadingSpeed;
            float f = ((-percent*percent)+percent)*4;
            float ang = Mathf.Lerp(0, maxAngle, f);
            transform.localEulerAngles = Vector3.left * ang +  originAngles;
            yield return null;
        }
        isReloadingMagazine = false;
        projectsRemainedInMagazine = magazineCapacity;
    }
    //瞄准
    public void Aim(Vector3 _point){
        if(!isReloadingMagazine){
            transform.LookAt(_point);
        }
    }
}
