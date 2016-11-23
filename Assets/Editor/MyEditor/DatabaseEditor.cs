using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Database))]
public class DatabaseEditor : Editor {
    EditType editType = EditType.ITEM;
    Database database = null;


    public enum EditType {
        ITEM, RECIPE
    }
    public override void OnInspectorGUI() {
        database = (Database)target;
        editType = (EditType)EditorGUILayout.EnumPopup("Edit Type: ", editType);
        switch (editType) {
            case EditType.ITEM: {
                    displayEditItem();
                    break;
                }
            case EditType.RECIPE: {
                    displayEditRecipe();
                    break;
                }
        }
    }
    static int selectedItemIndex = 0;
    int webSearchSelected = 0;
    List<Texture> webSearchTexture = new List<Texture>();
    string searchKeyword = "";
    MyMaterial mat = (MyMaterial)selectedItemIndex;
    TextureType textureType = TextureType.NORMAL;
    private void displayEditItem() {
        GUILayout.BeginHorizontal();
        if (selectedItemIndex > 0) {
            if (GUILayout.Button("Privious")) {
                selectedItemIndex--;
            }
        }
        if (selectedItemIndex < Enum.GetValues(typeof(MyMaterial)).Length - 1) {
            if (GUILayout.Button("Next")) {
                selectedItemIndex++;
            }
        }
        if (GUILayout.Button("Last")) {
            selectedItemIndex = Enum.GetValues(typeof(MyMaterial)).Length - 1;
        }
        mat = (MyMaterial)EditorGUILayout.EnumPopup("Item: ", mat);
        MaterialSetting ms = database.getMaterialSetting(mat);
        GUILayout.EndHorizontal();

        ms.isPlaceable = GUILayout.Toggle(ms.isPlaceable, "Is placeable: ");
        if (ms.isPlaceable) {
            //IF IS BLOCK
            ms.useCustomeBlockModel = GUILayout.Toggle(ms.useCustomeBlockModel, "Use Custome Block Model");
            if (ms.useCustomeBlockModel == false) {
                textureType = (TextureType)EditorGUILayout.EnumPopup("Texture Type", textureType);
                switch (textureType) {
                    case (TextureType.NORMAL): {
                            ms.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.texture, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.SIDE): {
                            ms.textureSIDE = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureSIDE, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.UP): {
                            ms.textureUP = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureUP, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.DOWN): {
                            ms.textureDOWN = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureDOWN, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.NORTH): {
                            ms.textureNORTH = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureNORTH, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.SOUTH): {
                            ms.textureSOUTH = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureSOUTH, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.WEST): {
                            ms.textureWEST = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureWEST, typeof(Texture2D), false);
                            break;
                        }
                    case (TextureType.EAST): {
                            ms.textureEAST = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.textureEAST, typeof(Texture2D), false);
                            break;
                        }
                    default: {
                            ms.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.texture, typeof(Texture2D), false);
                            break;
                        }
                }

                //Search
                GUILayout.Space(10f);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Auto Fill")) {
                    searchKeyword = mat.ToString().Replace("_", " ").ToLower();
                }
                searchKeyword = GUILayout.TextField(searchKeyword);
                if (GUILayout.Button("Search Image")) {
                    WebHandler.getTextures(webSearchTexture, searchKeyword);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                webSearchSelected = GUILayout.SelectionGrid(webSearchSelected, webSearchTexture.ToArray(), 8);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Use This Image As Texture")) {
                    if (File.Exists("Assets/Resources/Texture/" + mat + "_" + textureType + ".png")) {
                        File.Delete("Assets/Resources/Texture/" + mat + "_" + textureType + ".png");
                    }
                    File.WriteAllBytes("Assets/Resources/Texture/" + mat + "_" + textureType + ".png", ((Texture2D)webSearchTexture[webSearchSelected]).EncodeToPNG());
                    AssetDatabase.Refresh();
                    Texture2D t = Resources.Load<Texture2D>("Texture\\" + mat + "_" + textureType);
                    t.filterMode = FilterMode.Point;
                    ms.setTexture(textureType, t);
                    if (ms.material == null) {
                        Material material = Resources.Load<Material>("Material\\" + mat);
                        if (material == null) {
                            File.Copy(Application.dataPath + "/Resources/Material/DEFAULT.mat", Application.dataPath + "/Resources/Material/" + mat + ".mat");
                            material = Resources.Load<Material>("Material\\" + mat);
                        }
                        ms.material = material;
                    }
                    AssetDatabase.Refresh();
                }
                editDropTable(ms.dropTable);
            } else {
                ms.customeBlockModel = (GameObject)EditorGUILayout.ObjectField("Custome Block Model", ms.customeBlockModel, typeof(GameObject), false);
            }
        } else {
            //IF IS ITEM
            ms.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ms.texture, typeof(Texture2D), false);
            //Search
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            searchKeyword = GUILayout.TextField(searchKeyword);
            if (GUILayout.Button("Auto Fill")) {
                searchKeyword = mat.ToString().Replace("_", " ").ToLower();
            }
            if (GUILayout.Button("Search Image")) {
                WebHandler.getTextures(webSearchTexture, searchKeyword, transparent: true);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            webSearchSelected = GUILayout.SelectionGrid(webSearchSelected, webSearchTexture.ToArray(), 8);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Use This Image As Texture")) {
                if (File.Exists("Assets/Resources/Texture/" + mat + "_" + textureType + ".png")) {
                    File.Delete("Assets/Resources/Texture/" + mat + "_" + textureType + ".png");
                }
                File.WriteAllBytes("Assets/Resources/Texture/" + mat + "_" + textureType + ".png", ((Texture2D)webSearchTexture[webSearchSelected]).EncodeToPNG());
                AssetDatabase.Refresh();
                Texture2D t = Resources.Load<Texture2D>("Texture\\" + mat + "_" + textureType);
                t.filterMode = FilterMode.Point;
                ms.setTexture(textureType, t);
                if (ms.material == null) {
                    Material material = Resources.Load<Material>("Material\\" + mat);
                    if (material == null) {
                        File.Copy(Application.dataPath + "/Resources/Material/DEFAULT.mat", Application.dataPath + "/Resources/Material/" + mat + ".mat");
                        material = Resources.Load<Material>("Material\\" + mat);
                    }
                    ms.material = material;
                }
                AssetDatabase.Refresh();
            }
        }
        //Name
        ms.desplayName = EditorGUILayout.TextField("Display Name: ", ms.desplayName);
        //DESCRIPTION
        ms.description = EditorGUILayout.TextField("Description: ", ms.description);

        GUILayout.Space(10f);
    }
    CraftingRecipeType recipeType = CraftingRecipeType.PLAYER;
    int selectedIndex = -1;
    Vector2 scrollPos = Vector2.zero;
    private void displayEditRecipe() {
        recipeType = (CraftingRecipeType)EditorGUILayout.EnumPopup("Recipe Type: ", recipeType);
        List<CraftingRecipe> recipeList = database.getCraftingRecipes(recipeType); GUILayout.BeginHorizontal();
        if (selectedIndex > 0) {
            if (GUILayout.Button("Privious")) {
                selectedIndex--;
            }
        }
        if (selectedIndex < recipeList.Count - 1) {
            if (GUILayout.Button("Next")) {
                selectedIndex++;
            }
        }
        if (recipeList.Count > 0) {
            if (GUILayout.Button("Last")) {
                selectedIndex = recipeList.Count - 1;
            }
        }
        List<String> strList = new List<string>();
        for (int i = 0; i < recipeList.Count; i++) {
            CraftingRecipe recipe = recipeList[i];
            strList.Add("[" + i + "] " + recipe.getRecipeName());
        }
        selectedIndex = EditorGUILayout.Popup(selectedIndex, strList.ToArray());
        GUILayout.EndHorizontal();
        if (selectedIndex >= recipeList.Count) {
            selectedIndex = recipeList.Count - 1;
        }
        if (GUILayout.Button("Add New Recipe")) {
            CraftingRecipe recipe = new CraftingRecipe();
            recipeList.Add(recipe);
            selectedIndex = recipeList.Count - 1;
        }
        if (selectedIndex >= recipeList.Count) {
            selectedIndex = recipeList.Count - 1;
        }
        if (selectedIndex >= 0) {
            CraftingRecipe recipe = recipeList[selectedIndex];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Recipe For " + recipe.getRecipeName());
            if (GUILayout.Button("Remove This Recipe")) {
                recipeList.RemoveAt(selectedIndex);
            }
            EditorGUILayout.EndHorizontal();
            recipe.time = EditorGUILayout.IntSlider("Crafting Time: ", recipe.time, 0, 60);
            GUILayout.Space(10);
            editDropTable(recipe.resultTable);
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("------Required Items------");
            for (int i = 0; i < recipe.requiredItems.Count; i++) {
                ItemStack item = recipe.requiredItems[i];
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Required Item " + i);
                if (GUILayout.Button("Remove This Required Item")) {
                    recipe.requiredItems.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
                item.material = (MyMaterial)EditorGUILayout.EnumPopup("Material: ", item.material);
                item.amount = (int)EditorGUILayout.IntSlider("Amount Needed: ", item.amount, 1, 10);
            }
            if (GUILayout.Button("Add Required Item")) {
                recipe.requiredItems.Add(new ItemStack(MyMaterial.STONE, 1));
            }
        }
    }
    public void editDropTable(DropTable dropTable) {
        if (dropTable.itemDrops.Count == 0) {
            dropTable.itemDrops.Add(new ItemDrop());
        }
        if (GUILayout.Button("Add New Result")) {
            dropTable.itemDrops.Add(new ItemDrop());
        }
        foreach (ItemDrop drop in dropTable.itemDrops) {
            drop.material = (MyMaterial)EditorGUILayout.EnumPopup("Material: ", drop.material);
            drop.chance = EditorGUILayout.Slider("Drop Chance:", drop.chance, 0f, 1f);
            EditorGUILayout.MinMaxSlider(new GUIContent("Amount: "), ref drop.minAmount, ref drop.maxAmount, 1, 99);
            drop.minAmount = (int)drop.minAmount;
            drop.maxAmount = (int)drop.maxAmount;
        }
    }
}