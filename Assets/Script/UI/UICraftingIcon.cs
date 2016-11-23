using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UICraftingIcon : MonoBehaviour, IPointerClickHandler {

    public Image image;

    private CraftingRecipe recipe;

    bool canCraft = false;

    public void setRecipe(CraftingRecipe recipe) {
        this.recipe = recipe;
        image.sprite = recipe.getTexture();
    }
    public CraftingRecipe getCurrentRecipe() {
        return recipe;
    }
    public void setCanCraft(bool canCraft) {
        this.canCraft = canCraft;
        if (canCraft) {
            image.color = Color.white;
        } else {
            image.color = Color.gray;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (canCraft) {
            if (CraftingHandler.tryInventoryCraft(GameManager.UIHandler.uiCraftingPanel.getInventory(), recipe, inv2: GameManager.UIHandler.uiCraftingPanel.getSecondaryInventory())) {
                Debug.Log("crafted");
            } else {
                Debug.Log("craft failed");
            }
        }
    }
}
