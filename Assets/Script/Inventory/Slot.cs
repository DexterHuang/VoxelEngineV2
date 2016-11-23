using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Slot {
    [SerializeField]
    public ItemStack itemStack = null;
    [SerializeField]
    public int id;
    [NonSerialized]
    UISlot UISlot;
    public Slot(int id) {
        this.id = id;
    }
    public void setItem(ItemStack itemStack) {
        this.itemStack = itemStack;
        if (UISlot != null) {
            UISlot.updateUI();
        }
    }
    public ItemStack getItem() {
        return this.itemStack;
    }
    public void swapItem(Slot slot) {
        ItemStack myItem = this.getItem();
        ItemStack otherItem = slot.getItem();
        this.setItem(otherItem);
        slot.setItem(myItem);
        Debug.Log(myItem + "  <==> " + otherItem);
        this.UISlot.updateUI();
        slot.UISlot.updateUI();
    }
    public void setUISlot(UISlot UISlot) {
        this.UISlot = UISlot;

    }
    public bool isEmpty() {
        return itemStack == null || itemStack.getType() == MyMaterial.AIR;
    }
}
