using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {

    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera;
    // Start is called before the first frame update
    void Start(){
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update(){
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);//从摄像机到屏幕上面的点的射线
        Plane groundPlane = new Plane(Vector3.up,Vector3.zero);
        float rayDist;
        if(groundPlane.Raycast(ray,out rayDist)){
            Vector3 point = ray.GetPoint(rayDist);//与地面的交点
            //Debug.DrawLine(ray.origin,point,Color.red);
            controller.LookAt(point);
        }
    }
}
