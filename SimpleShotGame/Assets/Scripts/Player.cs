using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : People {

    public float moveSpeed = 5;
    public CrossController cross;
    PlayerController controller;
    GunController gunController;
    Camera viewCamera;
    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update(){

        //移动输入
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //朝向
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);//从摄像机到屏幕上面的点的射线
        Plane groundPlane = new Plane(Vector3.up,Vector3.up * gunController.gunHeight);
        //print(gunController.gunHeight);
        float rayDist;
        if(groundPlane.Raycast(ray,out rayDist)){
            Vector3 point = ray.GetPoint(rayDist);//与地面的交点
            //Debug.DrawLine(ray.origin,point,Color.red);
            controller.LookAt(point);//玩家朝向
            cross.transform.position = point;//准星位置
            cross.DetectTargets(ray);//检查准星是否瞄准了敌人
            //武器瞄准
            if((new Vector2(transform.position.x,transform.position.z) - new Vector2(point.x,point.z)).sqrMagnitude
                - (new Vector2(transform.position.x,transform.position.z) - new Vector2(gunController.transform.position.x,gunController.transform.position.z)).sqrMagnitude > 1){
                    //print("okk");
                gunController.AimAt(point);//bug：head shoot
            }
        }
        //武器输入
        if(Input.GetMouseButton(0)){//开火
            gunController.Shoot();
        }
        if(Input.GetKeyDown(KeyCode.R)){//换弹
            gunController.ReloadMagazine();
        }
    }

    public override void Die(){
        //玩家死亡音效
        AudioManager.instance.PlaySound("PlayerDeath", transform.position);
        base.Die();
    }
}
