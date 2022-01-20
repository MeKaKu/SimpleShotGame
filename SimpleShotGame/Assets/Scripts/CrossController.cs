using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossController : MonoBehaviour
{
    public LayerMask targetMask;
    float speed = 40;//自旋速度
    SpriteRenderer dotRenderer;//准星中间的瞄准点
    Color originColor;//瞄准点的默认颜色
    public Color highlightColor;
    // Start is called before the first frame update
    void Start(){
        dotRenderer = transform.Find("Point").GetComponent<SpriteRenderer>();//获取准星中间的瞄准点
        originColor = dotRenderer.color;
    }

    // Update is called once per frame
    void Update(){
        transform.Rotate(-Vector3.forward * speed * Time.deltaTime);//顺时针自转
    }

    public void DetectTargets(Ray ray){
        if(Physics.Raycast(ray,100,targetMask)){
            dotRenderer.color = highlightColor;
        }
        else{
            dotRenderer.color = originColor;
        }
    }
}
