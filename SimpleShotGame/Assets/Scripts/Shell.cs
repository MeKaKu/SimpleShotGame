using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//弹壳类
public class Shell : MonoBehaviour
{
    Rigidbody myRigidbody;
    public float minForce;
    public float maxForce;

    float lifetime = 4;//存在时间
    float fadetime = 2;//淡出时间
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        float force = Random.Range(minForce, maxForce);
        myRigidbody.AddForce(transform.right * force);//添加一个力
        myRigidbody.AddTorque(Random.insideUnitSphere * force);//添加一个扭矩

        StartCoroutine(Fade());
    }

    //淡出
    IEnumerator Fade(){
        yield return new WaitForSeconds(lifetime);

        float fadespeed = 1/fadetime;
        float percent = 0;
        Material mat = GetComponent<Renderer>().material;
        Color originColor = mat.color;

        while(percent < 1){
            mat.color = Color.Lerp(originColor, Color.clear, percent);
            percent += Time.deltaTime * fadespeed;
            yield return null;
        }

        Destroy(gameObject);
    }
}
