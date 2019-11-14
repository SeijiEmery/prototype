using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMotion : MonoBehaviour {
    public float moveSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);    
    }
}
