using System;
using UnityEngine;

[Serializable]
class BlockData {
    [SerializeField]
    Inventory inventory;
    public virtual void onTick() {

    }
    public Inventory getInventory() {
        return inventory;
    }
}
