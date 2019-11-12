using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
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

    private float lastTeleportTime = -10f;
    private bool lastTeleportButtonState = false;
    public Vector3 teleportVelocity = Vector3.zero;

    void Start() {
        rigidbody = pawn.GetComponent<Rigidbody>();
    }

    void Update() {
        var gamepad = Gamepad.current;
        if (gamepad != null) {
            if (gamepad.leftStickButton.isPressed && gamepad.rightStickButton.isPressed) {
                pawn.transform.position = Vector3.zero;
            }
            var ls = gamepad.leftStick.ReadValue();
            ls.x = Mathf.Pow(ls.x, 3f);
            ls.y = Mathf.Pow(ls.y, 3f);
            var rs = gamepad.rightStick.ReadValue();
            rs.x = Mathf.Pow(rs.x, 3f);
            rs.y = Mathf.Pow(rs.y, 3f);

            var MOVE_SPEED_BASE = 15f;
            var MOVE_SPEED_ACCEL = 0.8f;
            var DODGE_FACTOR = 1f;
            var DODGE_MULTIPLIER = 1f;
            var TELEPORT_DIST = 50f;
            var TELEPORT_COOLDOWN = 0.2f;
            var TELEPORT_DURATION = 0.2f;
            var TELEPORT_SPEED = TELEPORT_DIST / TELEPORT_DURATION;
            var TELEPORT_DECAY_RATE = 10f;

            bool teleport = gamepad.buttonSouth.isPressed;
            bool teleportPressed = teleport && !lastTeleportButtonState;
            lastTeleportButtonState = teleport;
            if (teleportPressed && Time.time > lastTeleportTime + TELEPORT_COOLDOWN &&
                ls.magnitude > 0.2f) {
                lastTeleportTime = Time.time;

                var dir = ls.normalized;
                teleportVelocity = TELEPORT_SPEED * (Vector3.forward * dir.y + Vector3.right * dir.x);
                Debug.Log("Teleporting " + teleportVelocity);
//                pawn.Translate(TELEPORT_DIST * ls.normalized);
            }

            velocity = Vector3.Lerp(
                velocity,
                velocity * Mathf.Clamp(1f - 10f * Time.deltaTime, 0f, 1f) 
                    + (Vector3.forward * ls.y).normalized * MOVE_SPEED_BASE
                    + (Vector3.right * ls.x).normalized * MOVE_SPEED_BASE
                ,
                Mathf.Clamp(Time.deltaTime * MOVE_SPEED_BASE * MOVE_SPEED_ACCEL, 0f, 1f));

            bool applyDirectionChanges = true;
            
            // handle fast velocity changes for when player switches directions w/ analog stick:
            // (ie. ship is moving forward but player now wants it moving backwards, NOW.
            // critical for good movement, dodging, etc
            var vnorm = velocity.normalized;
            var moveDir = Vector3.forward * ls.y + Vector3.right * ls.x;
            var dpAngle = Vector3.Dot(vnorm, moveDir);
            if (dpAngle < 0f && applyDirectionChanges) {
                var prevVel = velocity;
                dpAngle = Mathf.Clamp(-dpAngle * DODGE_FACTOR, 0f, 1f);
//                velocity = velocity * (1f - dpAngle) + moveDir * dpAngle * moveSpeed * 1.5f;
                velocity = velocity.magnitude * Vector3.Min(Vector3.one, Vector3.Max(-Vector3.one,
                               vnorm * (1f - dpAngle) // mix previous velocity
                               + moveDir * dpAngle * DODGE_MULTIPLIER));       // w/ new input direction * 1.5
//                                );
                Debug.Log("executing direction switch (intensity: "+dpAngle+"): "+prevVel+" => "+velocity);
            }
            
            pawn.Rotate(Vector3.up, 180f * 0.8f * Time.deltaTime * rs.x);
            pawn.Translate((velocity + teleportVelocity) * Time.deltaTime);
            teleportVelocity *= Mathf.Clamp(1f - Time.deltaTime * TELEPORT_DECAY_RATE, 0f, 1f);

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

        }
        else {
            Debug.Log("no gamepad connected!!!");
        }
    }
}
