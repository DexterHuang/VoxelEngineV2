using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Item : Entity {

    public static float ItemMergeDistance = 5f;
    [SerializeField]
    ItemStack itemStack;
    [NonSerialized]
    MeshCollider collider;
    public Item(ItemStack itemStack, Location location) : base(location) {
        this.itemStack = itemStack;
        firstTimeSpawn = true;
        init();
    }
    public MyMaterial getType() {
        return itemStack.getType();
    }
    public void init() {
        setRecycleCustomeName(getType() + "");
        this.reuseableGameObject = ReuseableGameObject.ITEM_ENTITY;
        spawn(getType().getSetting().itemPrefab);
        if (getGameObject().GetComponent<Rigidbody>() == null) {
            rigidbody = getGameObject().AddComponent<Rigidbody>();
            rigidbody.mass = 1;
        }
        collider = getGameObject().GetComponent<MeshCollider>();
        collider.material = GameManager.getGameManager().itemPhysicsMaterial;
        getGameObject().transform.SetParent(GameManager.getGameManager().itemHolder.transform);
        setInitiated(true);
    }
    public override void remove() {
        GameObject.Destroy(getGameObject().GetComponent<Rigidbody>());
        base.remove();
    }
    public ItemStack getItemStack() {
        return itemStack;
    }
    public List<ItemStack> getNearbyItemStack(float radius) {
        return getItemStackInRadius(this.getLocation().getVector3(), radius);
    }
    public static List<ItemStack> getItemStackInRadius(Vector3 pos, float radius) {
        List<ItemStack> items = new List<ItemStack>();
        foreach (Item item in GameManager.getEntityInRadius(ReuseableGameObject.ITEM_ENTITY, pos, radius)) {
            items.Add(item.getItemStack());
        }
        return items;
    }
    public override void onTick() {
        foreach (Entity e in GameManager.getEntityInRadius(this.reuseableGameObject, getPositionVector3(), ItemMergeDistance)) {
            if (e != this && e.isRemoved() == false && e.isInitiated()) {
                Item item = (Item)e;
                if (item.getType() == getType()) {
                    e.addVelocity((this.getPositionVector3() - e.getPositionVector3()).normalized * (((ItemMergeDistance - Vector3.Distance(this.getPositionVector3(), e.getPositionVector3())) * 0.5f)));
                }
            }

        }
    }
    public override void OnCollisionEnter(Collision collision, EntityUltility eu) {
        if (eu.entity.isRemoved() == false) {
            if (eu.entity is Item) {
                Item item = (Item)eu.entity;
                if (item.getType() == getType()) {
                    merge(item);
                }
            }
        }
    }
    public void merge(Item item) {
        if (isRemoved() == false && item.isRemoved() == false) {
            getItemStack().setAmount(getItemStack().getAmount() + item.getItemStack().getAmount());
            item.remove();
        }
    }
}
