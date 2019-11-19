using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float lifetime = 10f;
    private float initialVelocity = 0.8f;
    private Vector3 initVelocity = Vector3.zero;
    private Vector3 maxVelocity = Vector3.zero;
    public float damage;
    private float spawnTime = 0f;
    private Targetable.Owner owner;
    private Rigidbody rb;
    public void Fire (GameObject origin, float _damage, float speed, Targetable.Owner _owner, Vector3 v0)
    {
        rb = GetComponent<Rigidbody>();
        var velocity = transform.rotation * Vector3.forward * speed + v0;
        initVelocity = velocity * initialVelocity;
        maxVelocity = velocity * Mathf.Clamp(1f - initialVelocity, 0f, 1f);
        rb.velocity = initVelocity;
        spawnTime = Time.time;
        damage = _damage;
        owner = _owner;
    }
    private void FixedUpdate()
    {
        var t = Time.time - spawnTime / lifetime;
        rb.velocity = Vector3.Lerp(initVelocity, maxVelocity, t);
    }

    public void Update()
    {
        if (Time.time > spawnTime + lifetime)
        {
            Destroy();
        }
    }
    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponentInParent<Targetable>();
        if (target != null && target.IsHostileTowards(owner))
        {
            target.ApplyDamage(damage);
        }
        Destroy();
    }
}
