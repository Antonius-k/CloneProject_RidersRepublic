using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    SphereCollider sphereCollider;
     Controller bicycle;
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        bicycle = GetComponentInParent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        float bikeVelocity = bicycle.currentTopSpeed;
        transform.Rotate(Vector3.right * bikeVelocity);       
    }
}
