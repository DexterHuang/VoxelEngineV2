using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ItemStack {
    public String itemName;
    public MyMaterial material;
    public int amount;

    public ItemStack(MyMaterial material, int amount) {
        this.material = material;
        this.amount = amount;
    }
    public Sprite getTexture() {
        return material.getIconSprite();
    }
    public override string ToString() {
        return material + " x " + amount;
    }
    public int getAmount() {
        return amount;
    }
    public MyMaterial getType() {
        return material;
    }
    public void setAmount(int amount) {
        this.amount = amount;
    }
    public override bool Equals(object obj) {
        if (obj is ItemStack) {
            ItemStack i = (ItemStack)obj;
            if (material == i.material) {
                return true;
            } else {
                return false;
            }
        }
        return base.Equals(obj);
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }
    public ItemStack clone() {
        return new ItemStack(material, amount);
    }
}
