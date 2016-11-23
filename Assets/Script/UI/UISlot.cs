using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class UISlot : MonoBehaviour {
    [SerializeField]
    public Slot parentSlot;
    public Image image;
    public Text text;
    public InventoryType inventoryType;
    internal Slot getSlot() {
        return parentSlot;
    }
    public void setParentSlot(Slot slot) {
        parentSlot = slot;
        slot.setUISlot(this);
        updateUI(parentSlot.getItem());
    }
    public void setInventoryType(InventoryType type) {
        this.inventoryType = type;
    }
    public void updateUI(ItemStack item = null) {
        if (item == null) {
            item = parentSlot.getItem();
        }
        if (item != null && item.getType() != MyMaterial.AIR) {
            Sprite sprite = item.getTexture();
            image.color = new Color(255, 255, 255, 255);
            if (sprite != null) {
                image.sprite = sprite;
                text.text = item.getAmount() + "";
            } else {
                text.text = item.ToString();
            }
        } else {
            image.sprite = null;
            image.color = new Color(0, 0, 0, 0);
            text.text = "";
        }
    }
}
