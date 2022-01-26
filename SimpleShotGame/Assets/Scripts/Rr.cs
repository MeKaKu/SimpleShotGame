using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI自转
public class Rr : MonoBehaviour
{
    RectTransform self;
    float speed = 35;
    private void Update() {
        transform.eulerAngles += Vector3.forward * speed * Time.deltaTime;
    }
}
