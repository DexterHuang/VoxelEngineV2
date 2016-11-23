using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector {
    [SerializeField]
    float x;
    [SerializeField]
    float y;
    [SerializeField]
    float z;

    public Vector(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector(Vector3 v) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
    public float getX() {
        return x;
    }
    public float getY() {
        return y;
    }
    public float getZ() {
        return z;
    }
    public void setX(float x) {
        this.x = x;
    }
    public void setY(float y) {
        this.y = y;
    }
    public void setZ(float z) {
        this.z = z;
    }
    public Vector3 getVector3() {
        return new Vector3(x, y, z);
    }
    public Vector subtract(Vector v) {
        return subtract(v.x, v.y, v.z);
    }
    public Vector subtract(float x, float y, float z) {
        return new Vector(this.x - x, this.y - y, this.z - z);
    }
    public Vector add(Vector v) {
        return add(v.x, v.y, v.z);
    }
    public Vector add(float x, float y, float z) {
        return new Vector(this.x + x, this.y + y, this.z + z);
    }
    public Vector multiply(Vector v) {
        return multiply(v.x, v.y, v.z);
    }
    public Vector multiply(float x, float y, float z) {
        return new Vector(this.x * x, this.y * y, this.z * z);
    }
    public Vector multiply(float f) {
        return new Vector(this.x * f, this.y * f, this.z * f);
    }
    public override string ToString() {
        return "(" + this.x + ", " + this.y + ", " + this.z + ")";
    }
    public Vector getAbsolute() {
        return new Vector(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
    }
}
