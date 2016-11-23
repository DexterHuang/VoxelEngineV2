using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class CraftingRecipe {
    public List<ItemStack> requiredItems = new List<ItemStack>();

    public DropTable resultTable = new DropTable();

    public int time = 10;

    public Sprite getTexture() {
        if (resultTable.getPrimary() != null) {
            return resultTable.getPrimary().material.getIconSprite();
        }
        return null;
    }
    public string getRecipeName() {
        if (resultTable.getPrimary() == null) {
            return "[Result Not Set Yet!!]";
        } else {
            return resultTable.getPrimary().material + "";
        }
    }
    public List<ItemStack> getResults() {
        return resultTable.getDrops();
    }
}
