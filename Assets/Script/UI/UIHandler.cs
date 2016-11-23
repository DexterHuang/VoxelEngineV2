using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIHandler : MonoBehaviour {

    public bool isInInventory = false;
    GridLayoutGroup playerInventoryPane;
    public GameObject UISlotPrefab;
    public GameObject UICraftingIcon;
    public UICraftingPanel uiCraftingPanel;
    public UIInventory hotbarUIInventory;
    public UIInventory playerUIInventory;
    public UIInventory otherUIInventory;
    public Transform cursorLayerParent;
    public Transform hotbarSelect;
    public GameObject inventoryPane;


    public void openPlayerInventory() {
        inventoryPane.SetActive(true);
        isInInventory = true;
        uiCraftingPanel.setRecipeType(CraftingRecipeType.PLAYER);
        uiCraftingPanel.setTargetInventory(GameManager.getLocalPlayer().getInventory());
        uiCraftingPanel.setScondaryTargetInventory(GameManager.getLocalPlayer().getHotbar());
        uiCraftingPanel.refreshRecipes();
    }
    public void closeInventory() {
        inventoryPane.SetActive(false);
        isInInventory = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void prepareInventory(Inventory inv) {
        switch (inv.getInventoryType()) {
            case (InventoryType.PLAYER_HOTBAR): {
                    hotbarUIInventory.setInventory(inv);
                    break;
                }
            case (InventoryType.PLAYER_INVENTORY): {
                    playerUIInventory.setInventory(inv);
                    break;
                }
            default: {
                    otherUIInventory.setInventory(inv);
                    break;
                }
        }
    }
    public void init() {
        prepareInventory(GameManager.getLocalPlayer().getInventory());
        prepareInventory(GameManager.getLocalPlayer().getHotbar());
    }
    public static void onDragAndDropEvent(Slot initialSlot, Slot dropSlot) {
        initialSlot.swapItem(dropSlot);
    }
    void Update() {
        if (GameManager.getLocalPlayer() != null) {
            Transform selected = hotbarUIInventory.gridParent.GetChild(GameManager.getLocalPlayer().getSelectedHotbarIndex());
            hotbarSelect.position = Vector2.Lerp(hotbarSelect.position, selected.position, 0.1f);
        }
    }
    public void dropItem() {
        GameManager.getLocalPlayer().dropItemInHand();
    }
}