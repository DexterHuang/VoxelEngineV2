using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Entity {
    [SerializeField]
    Vector3 location;
    [SerializeField]
    Vector3 rotation;
    [SerializeField]
    public ReuseableGameObject reuseableGameObject;

    [NonSerialized]
    GameObject gameObject;
    [NonSerialized]
    EntityUltility ultility;
    [NonSerialized]
    public Rigidbody rigidbody;
    [NonSerialized]
    public bool firstTimeSpawn;
    [SerializeField]
    string recycleCustomName = "";
    bool ir = false;
    bool initiated = false;
    public Entity(Location location) {
        this.location = location.getVector3();
        initiated = false;
    }
    public void setInitiated(bool ini) {
        initiated = ini;
    }
    public bool isInitiated() {
        return initiated;
    }
    public void setRecycleCustomeName(string name) {
        recycleCustomName = name;
    }
    public void spawn(GameObject prefab) {
        GameObject o = GameManager.spawnPrefab(prefab, location, reuseableGameObject, customName: recycleCustomName);
        ultility = o.GetComponent<EntityUltility>();
        if (ultility == null) {
            ultility = o.AddComponent<EntityUltility>();
        }
        rigidbody = o.GetComponent<Rigidbody>();
        ultility.entity = this;
        o.transform.eulerAngles = rotation;
        gameObject = o;
        track();
    }
    public void track() {
        GameManager.getEntitiesByType(reuseableGameObject).Add(this);
    }
    public override string ToString() {
        return reuseableGameObject + "(" + gameObject.ToString() + ")";
    }
    public GameObject getGameObject() {
        return gameObject;
    }
    public Location getLocation() {
        return new Location(new Vector(location));
    }
    public Vector3 getPositionVector3() {
        return location;
    }
    public void registerLocation(Vector3 location) {
        this.location = location;
    }
    public void registerRotation(Vector3 rotation) {
        this.rotation = rotation;
    }
    public virtual void remove() {
        GameManager.getEntitiesByType(reuseableGameObject).Remove(this);
        GameObject.Destroy(gameObject.GetComponent<EntityUltility>());
        ReuseGameObjectHandler.putToRecycle(reuseableGameObject, gameObject, recycleCustomName);
        gameObject = null;
        ir = true;
    }
    public bool isRemoved() {
        return ir;
    }
    public virtual void onTick() {

    }

    public void teleport(Location location) {
        if (gameObject != null) {
            gameObject.transform.position = location.getVector3();
        }
        this.location = location.getVector3();
    }
    public void setVelocity(Vector3 velocity) {
        if (rigidbody != null) {
            rigidbody.velocity = velocity;
        } else {
            throw new Exception("There is no Rigidbody attached in " + this + " cannot set velocity.");
        }
    }
    public void addVelocity(Vector3 velocity) {
        if (rigidbody != null) {
            rigidbody.velocity += velocity;
        } else {
            throw new Exception("There is no Rigidbody attached in " + this + " cannot set velocity.");
        }
    }
    public Vector3 getRotation() {
        return rotation;
    }
    public List<Entity> getNearbyEntities(float radius) {
        return getNearbyEntities(this.reuseableGameObject, radius);
    }
    public List<Entity> getNearbyEntities(ReuseableGameObject ReuseableGameObject, float radius) {
        return GameManager.getEntityInRadius(ReuseableGameObject, this.getLocation().getVector3(), radius);
    }
    public virtual void OnCollisionEnter(Collision collision, EntityUltility eu) {

    }
}