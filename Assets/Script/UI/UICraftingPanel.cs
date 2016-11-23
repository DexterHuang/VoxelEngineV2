using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICraftingPanel : MonoBehaviour {

    public Transform gridParent;

    private CraftingRecipeType recipeType;

    Inventory inventory;

    Inventory inventory2;

    List<UICraftingIcon> currentIcons = new List<UICraftingIcon>();
    public Inventory getInventory() {
        return inventory;
    }
    public Inventory getSecondaryInventory() {
        return inventory2;
    }
    public void setTargetInventory(Inventory inv) {
        inventory = inv;
    }
    public void setScondaryTargetInventory(Inventory inv) {
        inventory2 = inv;
    }
    public void setRecipeType(CraftingRecipeType type) {
        recipeType = type;
    }
    public CraftingRecipeType getRecipeType() {
        return recipeType;
    }
    public void refreshRecipes() {
        for (int i = 0; i < gridParent.childCount; i++) {
            Transform child = gridParent.GetChild(i);
            ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.CRAFTING_ICON, child.gameObject);
        }
        List<ItemStack> items = new List<ItemStack>();
        if (inventory != null) {
            items.AddRange(inventory.getItems());
        }
        if (inventory2 != null) {
            items.AddRange(inventory2.getItems());
        }
        foreach (CraftingRecipe recipe in GameManager.database.getCraftingRecipes(recipeType)) {
            GameObject o = GameManager.spawnPrefab(GameManager.UIHandler.UICraftingIcon, Vector3.zero, ReuseableGameObject.CRAFTING_ICON);
            o.transform.SetParent(gridParent);
            UICraftingIcon icon = o.GetComponent<UICraftingIcon>();
            currentIcons.Add(icon);
            icon.setRecipe(recipe);
            if (CraftingHandler.canCraftRecipe(recipe, items)) {
                icon.setCanCraft(true);
            } else {
                icon.setCanCraft(false);
            }
        }
    }
}
