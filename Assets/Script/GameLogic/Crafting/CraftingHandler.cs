using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CraftingHandler {
    public static float floorCraftingRadius = 1f;
    public static bool canCraftRecipe(CraftingRecipe recipe, ICollection<ItemStack> items) {
        foreach (ItemStack item in recipe.requiredItems) {
            if (hasItem(items, item.getType(), item.getAmount()) == false) {
                Debug.Log("dont have " + item);
                return false;
            }
        }
        return true;
    }
    public static bool hasItem(ICollection<ItemStack> items, MyMaterial mat, int amount) {
        foreach (ItemStack item in items) {
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
    public static List<CraftingRecipe> getCraftableRecipes(CraftingRecipeType CraftingRecipeType, ICollection<ItemStack> items) {
        List<CraftingRecipe> craftableRecipes = new List<CraftingRecipe>();
        foreach (CraftingRecipe recipe in GameManager.database.getCraftingRecipes(CraftingRecipeType)) {
            if (canCraftRecipe(recipe, items)) {
                craftableRecipes.Add(recipe);
            }
        }
        return craftableRecipes;
    }
    public static bool tryInventoryCraft(Inventory inv, CraftingRecipe recipe, Inventory inv2 = null) {
        List<ItemStack> items = new List<ItemStack>();
        items.AddRange(inv.getItems());
        if (inv2 != null) {
            items.AddRange(inv2.getItems());
        }
        if (canCraftRecipe(recipe, items)) {
            List<ItemStack> results = recipe.resultTable.getDrops();
            foreach (ItemStack resultItem in results) {
                inv.addItem(resultItem);
                foreach (ItemStack item in recipe.requiredItems) {
                    int amount = item.getAmount();
                    amount = inv.removeItem(item.getType(), amount);
                    if (amount > 0) {
                        inv2.removeItem(item.getType(), amount);
                    }
                }
                return true;

            }

        }
        return false;
    }
    public static bool tryFloorCraft(CraftingRecipe recipe, Vector3 pos) {
        List<ItemStack> itemStacks = Item.getItemStackInRadius(pos, floorCraftingRadius);
        if (canCraftRecipe(recipe, itemStacks)) {
            List<Entity> items = GameManager.getEntityInRadius(ReuseableGameObject.ITEM_ENTITY, pos, floorCraftingRadius);
            foreach (ItemStack neededItem in recipe.requiredItems) {
                int neededAmount = neededItem.getAmount();
                foreach (Entity e in items) {
                    Item item = (Item)e;
                    ItemStack itemStack = item.getItemStack();
                    if (itemStack.Equals(neededItem)) {
                        if (itemStack.getAmount() > neededAmount) {
                            itemStack.setAmount(itemStack.getAmount() - neededAmount);
                            neededAmount = 0;
                        } else {
                            neededAmount -= itemStack.getAmount();
                            item.remove();
                        }
                    }
                    if (neededAmount <= 0) {
                        break;
                    }
                }
            }
            foreach (ItemStack item in recipe.getResults()) {
                new Item(item, new Location(pos + new Vector3(0, 0.5f, 0))); ;
            }
            return true;
        }
        return false;
    }
}
