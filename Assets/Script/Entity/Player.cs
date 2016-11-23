using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : LivingEntity {
    [SerializeField]
    int selectedhotbarIndex = 0;
    [SerializeField]
    public Inventory inventory;
    [SerializeField]
    public Inventory hotbar;

    public Camera camera;

    public Player(Location location) : base(location) {

    }
    public void init(GameObject prefab) {
        teleport(getLocation().add(0, 10, 0));
        spawn(prefab);
        camera = getGameObject().GetComponentInChildren<Camera>();
        inventory.init(this.getPositionVector3());
        hotbar.init(this.getPositionVector3());
    }

    internal bool isInInventory() {
        return GameManager.UIHandler.isInInventory;
    }
    public Inventory getHotbar() {

        if (inventory == null) {
            inventory = new Inventory(Inventory.playerHotbarSlotAmount, InventoryType.PLAYER_HOTBAR);
            inventory.setItem(0, new ItemStack(MyMaterial.DIRT, 1));
            inventory.setItem(2, new ItemStack(MyMaterial.GRASS, 100));
            inventory.setItem(4, new ItemStack(MyMaterial.CHEST, 10));
        }
        return inventory;
    }
    public Inventory getInventory() {
        if (hotbar == null) {
            hotbar = new Inventory(Inventory.playerInvSlotAmount, InventoryType.PLAYER_INVENTORY);
            hotbar.setItem(0, new ItemStack(MyMaterial.LEAVES, 1));
            hotbar.setItem(2, new ItemStack(MyMaterial.LOG, 100));
            hotbar.setItem(3, new ItemStack(MyMaterial.POTATO, 1));
            hotbar.setItem(4, new ItemStack(MyMaterial.SWORD, 100));
            hotbar.setItem(5, new ItemStack(MyMaterial.COOKIE, 100));
        }
        return hotbar;
    }
    public ItemStack getItemInHand() {
        return getHotbar().getItem(getSelectedHotbarIndex());
    }
    public void setSelectedHotbarIndex(int index) {
        selectedhotbarIndex = index;
    }
    public int getSelectedHotbarIndex() {
        return selectedhotbarIndex;
    }
    private Slot[] getSlots() {
        List<Slot> slots = new List<Slot>();
        foreach (Slot slot in getHotbar().getContents()) {
            slots.Add(slot);
        }
        foreach (Slot slot in getInventory().getContents()) {
            slots.Add(slot);
        }
        return slots.ToArray();
    }
    public ItemStack[] getAllItems() {
        List<ItemStack> items = new List<ItemStack>();
        foreach (Slot slot in getSlots()) {
            items.Add(slot.getItem());
        }
        return items.ToArray();
    }
    public bool hasItem(MyMaterial mat, int amount) {
        foreach (ItemStack item in getAllItems()) {
            if (item != null && item.getType() == mat) {
                amount -= item.getAmount();
            }
        }

        if (amount > 0) {
            return false;
        } else {
            return true;
        }
    }
    public bool removeItem(MyMaterial mat, int amount) {
        if (hasItem(mat, amount)) {
            foreach (Slot slot in getSlots()) {
                ItemStack item = slot.getItem();
                if (item != null && item.getType() == mat) {
                    int itemAmount = item.getAmount();
                    if (itemAmount > amount) {
                        item.setAmount(itemAmount - amount);
                    } else {
                        amount -= itemAmount;
                        slot.setItem(null);
                    }
                }
            }
            return true;
        } else {
            return false;
        }
    }

    public void dropItemInHand() {
        ItemStack item = getItemInHand();
        if (item != null) {
            dropItem(item);
        }
    }
    public bool giveItem(ItemStack itemStack) {
        return giveItem(itemStack.getType(), itemStack.getAmount());
    }
    internal bool giveItem(MyMaterial material, int amount) {
        Inventory hotInv = getHotbar();
        foreach (Slot slot in hotInv.getContents()) {
            ItemStack i = slot.getItem();
            if (i != null && i.getType() == material) {
                i.setAmount(i.getAmount() + amount);
                return true;
            }
        }
        Inventory inv = getInventory();
        foreach (Slot slot in inv.getContents()) {
            ItemStack i = slot.getItem();
            if (i != null && i.getType() == material) {
                i.setAmount(i.getAmount() + amount);
                return true;
            }
        }
        ItemStack item = new ItemStack(material, amount);
        foreach (Slot slot in hotInv.getContents()) {
            if (slot.isEmpty()) {
                slot.setItem(item);
                return true;
            }
        }
        foreach (Slot slot in inv.getContents()) {
            if (slot.isEmpty()) {
                slot.setItem(item);
                return true;
            }
        }
        return false;
    }
    public Vector3 getDirection() {
        return Camera.main.transform.forward;
    }
    public void dropItem(ItemStack item) {
        Item i = GameManager.spawnItem(item.clone(), this.getLocation().add(getDirection().normalized));
        i.setVelocity(getDirection().normalized * 2);
    }
}