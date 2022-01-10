using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidbody;
    // Start is called before the first frame update
    void Start(){
        myRigidbody = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 _velocity){
        velocity = _velocity;
    }
    public void LookAt(Vector3 point){
        Vector3 lookPoint = new Vector3(point.x,transform.position.y,point.z);
        transform.LookAt(lookPoint);
    }
    // Update is called once per frame
    public void FixedUpdate() {
        myRigidbody.MovePosition(myRigidbody.position + velocity*Time.fixedDeltaTime);
    }
}
