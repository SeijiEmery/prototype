using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {
    public enum Owner { PlayerFleet, EnemyFleet };

    public Vector2 bounds = Vector2.one;
    public Owner owner = Owner.EnemyFleet;

    public bool IsHostileTowards (Owner other)
    {
        return owner != other;
    }

    public Vector3 GetRandomPointOnTarget() {
        return transform.position + transform.rotation * (
            Vector3.forward * Random.Range(-bounds.x, bounds.x) +
            Vector3.right * Random.Range(-bounds.y, bounds.y)
        );
    }

    public void ApplyDamage(float damage) { }
}
