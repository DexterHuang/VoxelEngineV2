using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Location {
    [SerializeField]
    Vector vector;

    public Location(float x, float y, float z) {
        this.vector = new Vector(x, y, z);
    }
    public Location(Vector3 vector3) {
        this.vector = new Vector(vector3);
    }
    public Location(Vector vector) {
        this.vector = vector;
    }
    public Vector toVector() {
        return new Vector(vector.getVector3());
    }
    public Location clone() {
        return new Location(vector);
    }
    public Block getBlock() {
        return GameManager.getBlock(this);
    }
    public Vector getChunkVector() {
        return new Vector((int)Math.Floor((vector.getX() / GameManager.chunkSize)), (int)Math.Floor((vector.getY() / GameManager.chunkSize)), (int)Math.Floor((vector.getZ() / GameManager.chunkSize)));
    }
    public Location add(Direction direction) {
        return new Location(vector.add(direction.toVector()));
    }
    public Location add(Location location) {
        return new Location(vector.add(location.toVector()));
    }
    public Location add(Vector vector) {
        return new Location(this.vector.add(vector));
    }
    public Location add(float x, float y, float z) {
        return add(new Vector(x, y, z));
    }
    public Location add(Vector3 vector3) {
        return add(new Vector(vector3));
    }
    public Location subtract(Location location) {
        return new Location(vector.subtract(location.toVector()));
    }
    public override string ToString() {
        return vector + "";
    }
    public float getX() {
        return vector.getX();
    }
    public float getY() {
        return vector.getY();
    }
    public float getZ() {
        return vector.getZ();
    }
    public void setX(float x) {
        vector.setX(x);
    }
    public void setY(float y) {
        vector.setY(y);
    }
    public void setZ(float z) {
        vector.setZ(z);
    }
    public Vector3 getVector3() {
        return vector.getVector3();
    }
}
