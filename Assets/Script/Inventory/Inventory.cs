
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory {
    [SerializeField]
    int size;
    [SerializeField]
    Slot[] slots;
    [SerializeField]
    InventoryType inventoryType;

    [NonSerialized]
    Vector3 overflowLocation;

    public static int playerInvSlotAmount = 32;
    public static int playerHotbarSlotAmount = 6;

    public Inventory(int size, InventoryType type) {
        slots = new Slot[size];
        for (int i = 0; i < size; i++) {
            slots[i] = new Slot(i);
        }
        this.size = size;
        this.inventoryType = type;
    }

    internal void init(Vector3 overflowLocation) {
        this.overflowLocation = overflowLocation;
    }

    internal Slot getSlot(int index) {
        return slots[index];
    }
    public void setItem(int index, ItemStack item) {
        slots[index].setItem(item);
    }
    public ItemStack getItem(int index) {
        return slots[index].getItem();
    }
    public int getSize() {
        return size;
    }
    public InventoryType getInventoryType() {
        return inventoryType;
    }
    public Slot[] getContents() {
        return slots;
    }
    public List<ItemStack> getItems() {
        List<ItemStack> items = new List<ItemStack>();
        foreach (Slot slot in getContents()) {
            items.Add(slot.getItem());
        }
        return items;
    }
    public int removeItem(MyMaterial m, int remvoeAmount) {
        int amount = remvoeAmount;
        foreach (Slot slot in slots) {
            ItemStack i = slot.getItem();
            if (i != null && i.getType() == m) {
                if (amount > 0) {
                    if (i.getAmount() > amount) {
                        i.setAmount(i.getAmount() - amount);
                    } else if (i.getAmount() == amount) {
                        amount -= slot.getItem().getAmount();
                        slot.setItem(null);
                    }
                }
            }
        }
        return amount;
    }
    public void addItem(ItemStack item) {
        foreach (Slot slot in slots) {
            if (slot.isEmpty()) {
                slot.setItem(item);
                return;
            } else if (slot.getItem().getType() == item.getType()) {
                slot.getItem().amount += item.getAmount();
                return;
            }
        }
        new Item(item, new Location(overflowLocation));

    }
}
