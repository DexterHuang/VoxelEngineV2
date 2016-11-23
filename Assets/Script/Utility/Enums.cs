using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MyMaterial {
    AIR, DIRT, STONE, GRASS, LEAVES, LOG, SWORD, POTATO, COOKIE, CHEST, TWIG
}
public enum Direction {
    UP, DOWN, NORTH, EAST, SOUTH, WEST
}
public enum ReuseableGameObject {
    CHUNK, CHUNK_CHILD, REFLECTION_PROBE, ITEM_ENTITY, BLOCK_DAMAGE_OVERLAY,
    WORLD_SPACE_UI, CRAFTING_ICON
}
public enum InventoryType {
    PLAYER_INVENTORY, PLAYER_HOTBAR, CRAFTING_RECIPE
}
public enum ItemRenderStyle {
    BLOCK, ITEM
}
public enum CraftingRecipeType {
    FLOOR, PLAYER
}
public enum TextureType {
    NORMAL, SIDE, UP, DOWN, NORTH, SOUTH, WEST, EAST
}
public static class Extension {
    public static bool isBlock(this MyMaterial m) {
        switch (m) {
            case (MyMaterial.AIR):
            case (MyMaterial.POTATO):
            case (MyMaterial.SWORD): {
                    return false;
                }
        }
        return true;
    }
    public static Sprite getIconSprite(this MyMaterial m) {
        return GameManager.database.getMaterialSetting(m).iconSprite;
    }
    public static Material getMaterial(this MyMaterial m, Direction direction) {
        string name = m + "_" + direction;
        Material mat = getCachedMaterial(m, (TextureType)Enum.Parse(typeof(TextureType), direction + ""));
        if (mat == null && direction != Direction.UP && direction != Direction.DOWN) {
            mat = getCachedMaterial(m, TextureType.SIDE);
        }
        if (mat == null) {
            mat = getCachedMaterial(m, TextureType.NORMAL);
        }
        return mat;
    }
    public static Material getCachedMaterial(MyMaterial material, TextureType type) {
        Material mat = null;
        string name = material + "_" + type;
        if (ChunkMeshCreator.materials.ContainsKey(name)) {
            ChunkMeshCreator.materials.TryGetValue(name, out mat);
            return mat;
        }
        //if (mat == null) {
        //    if (name.Contains("_") == false) {
        //        Scheduler.runTaskSynchronously(() => {
        //            File.Copy(Application.dataPath + "/Resources/Material/DEFAULT.mat", Application.dataPath + "/Resources/Material/" + name + ".mat");
        //            mat = Resources.Load<Material>("Material\\" + name);
        //           ChunkMeshCreator.materials.Add(name, mat);
        //        });
        //
        //       throw new System.Exception("Could not fine material in Resources: " + name);
        //    }
        //}
        return mat;
    }
    public static void loadMaterial(this MyMaterial m) {
        MaterialSetting ms = m.getSetting();
        if (ms.material == null) {
            if (File.Exists(Application.dataPath + "/Resources/Material/" + m + ".mat") == false) {
                File.Copy(Application.dataPath + "/Resources/Material/DEFAULT.mat", Application.dataPath + "/Resources/Material/" + m + ".mat");
            }
            ms.material = Resources.Load<Material>("Material\\" + m);
        }
        bool found = false;
        foreach (TextureType type in Enum.GetValues(typeof(TextureType))) {
            if (ms.getTexture(type) != null) {
                Material mat = new Material(ms.material);
                mat.mainTexture = ms.getTexture(type);
                ChunkMeshCreator.materials.Add(m + "_" + type, mat);
                found = true;
            }
        }
        if (found == false) {
            Material mat = new Material(GameManager.getGameManager().itemMaterial);
            ChunkMeshCreator.materials.Add(m + "_" + TextureType.NORMAL, mat);
            //GameManager.getGameManager().webHandler.giveRandomTexture(mat, name.Replace("_", " "));
        }
    }
    public static MaterialSetting getSetting(this MyMaterial m) {
        return GameManager.database.getMaterialSetting(m);
    }
    public static bool isTransparent(this MyMaterial m) {
        switch (m) {
            case (MyMaterial.LEAVES):
            case (MyMaterial.AIR): {
                    return true;
                }
        }
        return false;
    }
    public static Vector toVector(this Direction dir) {
        switch (dir) {
            case (Direction.UP): {
                    return new Vector(0, 1, 0);
                }
            case (Direction.DOWN): {
                    return new Vector(0, -1, 0);
                }
            case (Direction.NORTH): {
                    return new Vector(0, 0, 1);
                }
            case (Direction.SOUTH): {
                    return new Vector(0, 0, -1);
                }
            case (Direction.EAST): {
                    return new Vector(1, 0, 0);
                }
            case (Direction.WEST): {
                    return new Vector(-1, 0, 0);
                }
        }
        return new Vector(0, 0, 0);
    }
}
