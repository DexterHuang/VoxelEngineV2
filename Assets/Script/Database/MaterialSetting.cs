using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[Serializable]
public class MaterialSetting {
    public MyMaterial myMaterial;

    public ItemRenderStyle itemRenderStyle = ItemRenderStyle.ITEM;

    public Material material;

    public Texture2D texture;

    public Texture2D textureSIDE;

    public Texture2D textureUP;

    public Texture2D textureDOWN;

    public Texture2D textureNORTH;

    public Texture2D textureSOUTH;

    public Texture2D textureEAST;

    public Texture2D textureWEST;

    public GameObject itemPrefab;

    public Mesh blockMesh;

    public float itemScale = 1;

    public bool isPlaceable = false;

    public bool useCustomeBlockModel = false;

    public GameObject customeBlockModel = null;

    public Sprite iconSprite;

    public int blockHealth = 1000;

    public string desplayName = "A ItemName";

    public string description = "Description.....";

    public DropTable dropTable = new DropTable();

    public bool isCraftingMachine = false;

    public CraftingRecipeType craftingRecipeType;

    public MaterialSetting(MyMaterial m) {
        itemScale = 1f;
        this.myMaterial = m;
        itemRenderStyle = ItemRenderStyle.BLOCK;
        blockHealth = 1000;
    }
    public Texture2D getTexture(TextureType type) {
        switch (type) {
            case (TextureType.NORMAL): {
                    return texture;
                }
            case (TextureType.SIDE): {
                    return this.textureSIDE;
                }
            case (TextureType.UP): {
                    return this.textureUP;
                }
            case (TextureType.DOWN): {
                    return this.textureDOWN;
                }
            case (TextureType.NORTH): {
                    return this.textureNORTH;
                }
            case (TextureType.SOUTH): {
                    return this.textureSOUTH;
                }
            case (TextureType.WEST): {
                    return this.textureWEST;
                }
            case (TextureType.EAST): {
                    return this.textureEAST;
                }
            default: {
                    return this.texture;
                }
        }
    }
    public void setTexture(TextureType type, Texture2D texture) {
        switch (type) {
            case (TextureType.NORMAL): {
                    this.texture = texture;
                    break;
                }
            case (TextureType.SIDE): {
                    this.textureSIDE = texture;
                    break;
                }
            case (TextureType.UP): {
                    this.textureUP = texture;
                    break;
                }
            case (TextureType.DOWN): {
                    this.textureDOWN = texture;
                    break;
                }
            case (TextureType.NORTH): {
                    this.textureNORTH = texture;
                    break;
                }
            case (TextureType.SOUTH): {
                    this.textureSOUTH = texture;
                    break;
                }
            case (TextureType.WEST): {
                    this.textureWEST = texture;
                    break;
                }
            case (TextureType.EAST): {
                    this.textureEAST = texture;
                    break;
                }
            default: {
                    this.texture = texture;
                    break;
                }
        }
    }
}
