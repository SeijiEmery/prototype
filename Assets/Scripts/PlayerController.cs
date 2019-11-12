using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour {
    public Transform pawn;
    public Camera camera;
    public float moveSpeed = 0f;
    public float maxMoveSpeed = 10f;
    public float minMoveSpeed = 1f;
    private new Rigidbody rigidbody;
    private Vector3 velocity = Vector3.zero;

    public float camDist = 0f;
    public float minCamDist = 10f;
    public float maxCamDist = 400f;
    public float camZoomSpeed = 1f;

    void Start() {
        rigidbody = pawn.GetComponent<Rigidbody>();
    }

    void Update() {
        var gamepad = Gamepad.current;
        if (gamepad != null) {
            var ls = gamepad.leftStick.ReadValue();
            var rs = gamepad.rightStick.ReadValue();

            velocity = Vector3.Lerp(
                velocity,
                velocity * Mathf.Clamp(1f - 10f * Time.deltaTime, 0f, 1f) 
                    + (Vector3.forward * ls.y).normalized * 10f
                    + (Vector3.right * ls.x).normalized * 5f
                ,
                Mathf.Clamp(Time.deltaTime * 10f, 0f, 1f));
            
            pawn.Rotate(Vector3.up, 180f * 0.5f * Time.deltaTime * rs.x);
            pawn.Translate(velocity * Time.deltaTime);

            var dist = 200f;
            camDist = Mathf.Clamp(
                camDist + rs.y * (maxCamDist - minCamDist) * Time.deltaTime * camZoomSpeed,
                minCamDist, maxCamDist);
            
            var camAngle = 45f;
            var offset = pawn.rotation * Vector3.back * dist * Mathf.Cos(camAngle)
                         + Vector3.up * dist * Mathf.Sin(camAngle);
            
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                pawn.position + offset,
                Mathf.Clamp(Time.deltaTime * 10f, 0f, 1f));
            camera.transform.LookAt(pawn.position);

//            rigidbody.velocity = velocity;
        }
        else {
            Debug.Log("no gamepad connected!!!");
        }
    }
}
