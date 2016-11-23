using UnityEngine;
using System.Collections;

public class EntityUltility : MonoBehaviour {
    private Vector3 position;
    public Entity entity;
    Rigidbody rigi;
    void Start() {
        setPosition();
        rigi = GetComponent<Rigidbody>();
        if (entity.firstTimeSpawn == false && rigi != null) {
            rigi.Sleep();
            Scheduler.scheduleSyncDelayedTask(() => {
                if (entity.isRemoved() == false) {
                    rigi.WakeUp();
                }
            }, 5);
        }
    }
    void Update() {
        setPosition();
    }
    private void setPosition() {
        position = transform.position;
        if (entity != null) {
            entity.registerLocation(position);
            entity.registerRotation(transform.rotation.eulerAngles);
        }
    }
    public Vector3 getPosition() {
        return position;
    }

    public void OnCollisionEnter(Collision collision) {
        if (entity != null) {
            if (collision.gameObject.tag.Equals("Item")) {
                EntityUltility eu = collision.gameObject.GetComponent<EntityUltility>();
                if (eu.entity.isInitiated() && eu.entity.isRemoved() == false) {
                    entity.OnCollisionEnter(collision, eu);
                }
            }
        }
    }
}
