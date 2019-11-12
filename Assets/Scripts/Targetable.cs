using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {
    public Vector2 bounds = Vector2.one;
    public Vector3 GetRandomPointOnTarget() {
        return transform.position + transform.rotation * (
            Vector3.forward * Random.Range(-bounds.x, bounds.x) +
            Vector3.right * Random.Range(-bounds.y, bounds.y)
        );
    }

    public void ApplyDamage(float damage) { }
}
