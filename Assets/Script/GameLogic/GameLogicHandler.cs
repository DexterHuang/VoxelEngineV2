using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogicHandler {

    public static void onUpdate() {
#if UNITY_EDITOR
        if (GameManager.getLocalPlayer().isInInventory() == false) {
            if (Input.GetKey(KeyCode.Mouse0)) {
                onLeftClick(Input.mousePosition);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                onRightClick(Input.mousePosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            GameManager.getLocalPlayer().dropItemInHand();
        }
#endif
    }
    public static void onRightClick(Vector2 position) {
        Player player = GameManager.getLocalPlayer();
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5)) {
            GameObject o = hit.transform.gameObject;
            if (o.tag.Equals("Block")) {
                Location loc = new Location((ray.direction * 0.01f) + hit.point);
                Block clickedBlock = loc.getBlock();
                Location diff = loc.subtract(clickedBlock.getLocation());
                Direction direction = Direction.DOWN;
                if (diff.getX() >= 0.99f) {
                    direction = Direction.EAST;
                } else if (diff.getX() <= 0.01f) {
                    direction = Direction.WEST;
                }
                if (diff.getY() >= 0.99f) {
                    direction = Direction.UP;
                } else if (diff.getY() <= 0.01f) {
                    direction = Direction.DOWN;
                }
                if (diff.getZ() >= 0.99f) {
                    direction = Direction.NORTH;
                } else if (diff.getZ() <= 0.01f) {
                    direction = Direction.SOUTH;
                }
                onPlayerRightClickBlock(player, clickedBlock, direction);
                return;
            } else if (o.tag.Equals("Item")) {
                EntityUltility entityUltility = o.GetComponent<EntityUltility>();
                onPlayerRightClickItem(player, (Item)entityUltility.entity);
                return;
            }
        }
    }
    public static void onLeftClick(Vector2 position) {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5)) {
            Location loc = new Location((ray.direction * 0.01f) + hit.point);
            Block block = loc.getBlock();
            block.damageBlock(10);
        }
    }
    public static void onPlayerRightClickBlock(Player player, Block clickedBlock, Direction direction) {
        ItemStack item = player.getItemInHand();
        if (item != null && item.getType().isBlock()) {
            //Debug.Log(diff + " ===> " + direction);
            Block block = clickedBlock.getRelative(direction);
            if (block.getType() == MyMaterial.AIR) {
                block.setType(item.getType(), true);
            }
        }
    }
    public static void onPlayerRightClickItem(Player player, Item item) {
        WorldSpaceUI ui = WorldSpaceUIHandler.showUI(item.getLocation().getVector3());
        ui.addButton("Pick Up", () => {
            Debug.Log("item removed");
            item.remove();
            player.giveItem(item.getItemStack());
            WorldSpaceUIHandler.hideUI();
        });
        foreach (CraftingRecipe recipe in CraftingHandler.getCraftableRecipes(CraftingRecipeType.FLOOR, item.getNearbyItemStack(3f))) {
            ui.addButton("Craft " + recipe.requiredItems.GetType(), () => {
                if (CraftingHandler.tryFloorCraft(recipe, item.getLocation().getVector3()) == null) {
                    Debug.Log("failed!");
                }
                WorldSpaceUIHandler.hideUI();
            });
        }
    }
}
