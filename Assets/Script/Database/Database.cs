using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "MyScript/Database", order = 1)]
public class Database : ScriptableObject {

    public List<MaterialSetting> materialSetting;

    public List<CraftingRecipe> floorCraftingRecipes;

    public List<CraftingRecipe> normalCraftingRecipes;

    public MaterialSetting getMaterialSetting(MyMaterial material) {
        foreach (MaterialSetting matSetting in materialSetting) {
            if (matSetting.myMaterial == material) {
                return matSetting;
            }
        }
        MaterialSetting ms = new MaterialSetting(material);
        materialSetting.Add(ms);
        return ms;
    }
    public List<CraftingRecipe> getCraftingRecipes(CraftingRecipeType CraftingRecipeType) {
        switch (CraftingRecipeType) {
            case (CraftingRecipeType.FLOOR): {
                    return floorCraftingRecipes;
                }
            case (CraftingRecipeType.PLAYER): {
                    return normalCraftingRecipes;
                }
            default: {
                    return null;
                }
        }
        return null;
    }
}
