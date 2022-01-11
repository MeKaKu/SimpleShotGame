using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;

    public void SetSpeed(float _speed){
        speed = _speed;
    }

    private void Update() {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

}
