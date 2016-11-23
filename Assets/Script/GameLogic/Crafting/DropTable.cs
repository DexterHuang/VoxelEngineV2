using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropTable {

    public List<ItemDrop> itemDrops = new List<ItemDrop>();

    public List<ItemStack> getDrops() {
        List<ItemStack> list = new List<ItemStack>();
        float r = UnityEngine.Random.Range(0f, 1f);
        foreach (ItemDrop drop in itemDrops) {
            if (drop.chance >= r) {
                list.Add(new ItemStack(drop.material, drop.getAmount()));
            }
        }
        return list;
    }
    public ItemDrop getPrimary() {
        if (itemDrops.Count > 0) {
            return itemDrops[0];
        }
        return null;
    }
}
