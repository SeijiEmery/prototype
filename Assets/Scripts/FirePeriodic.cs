using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePeriodic : MonoBehaviour {
    public LaserWeapon weaponPrefab;
    public Targetable target;
    public float fireInterval = 1f;
    public float damage = 10f;
    private float lastFireTime = -10f;

    void Update() {
        if (Time.time > lastFireTime + fireInterval) {
            lastFireTime = Time.time;
            var projectile = GameObject.Instantiate(weaponPrefab);
            projectile.Fire(transform, target, damage);
        }
    }
}
