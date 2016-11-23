using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IconRenderer : MonoBehaviour {
    public static void init() {
        GameObject set = GameManager.getGameManager().iconRederSet;
        Camera camera = set.GetComponent<Camera>();
        RenderTexture renderTexture = new RenderTexture(64, 64, 64);
        camera.targetTexture = renderTexture;
        GameObject itemModel = set.transform.FindChild("ItemModel").gameObject;
        GameObject cubeModel = set.transform.FindChild("CubeModel").gameObject;
        MeshRenderer CubeMeshRenderer = cubeModel.GetComponent<MeshRenderer>();
        foreach (MyMaterial m in Enum.GetValues(typeof(MyMaterial))) {
            if (m == MyMaterial.AIR) {
                continue;
            }
            MaterialSetting ms = m.getSetting();
            if (ms.iconSprite == null) {
                GameObject tempObject = null;
                if (ms.itemRenderStyle == ItemRenderStyle.BLOCK) {
                    CubeMeshRenderer.enabled = true;
                    CubeMeshRenderer.material = m.getMaterial(Direction.UP);
                    //int x = 0;
                    //int i = 13 / x;
                } else if (ms.itemRenderStyle == ItemRenderStyle.ITEM) {
                    tempObject = Instantiate<GameObject>(ms.itemPrefab);
                    tempObject.transform.SetParent(itemModel.transform, false);
                }


                camera.Render();
                CubeMeshRenderer.enabled = false;
                if (tempObject != null) {
                    for (int i = 0; i < tempObject.transform.childCount; i++) {
                        GameObject o = tempObject.transform.GetChild(i).gameObject;
                        GameObject.DestroyImmediate(o);
                    }
                    GameObject.DestroyImmediate(tempObject);
                }
                RenderTexture.active = renderTexture;
                Texture2D imageOverview = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
                imageOverview.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                RenderTexture.active = null;
                //Replacing Black;
                for (int x = 0; x < renderTexture.width; x++) {
                    for (int y = 0; y < renderTexture.height; y++) {
                        Color c = imageOverview.GetPixel(x, y);
                        if (c.b == 0 && c.r == 0 && c.b == 0) {
                            imageOverview.SetPixel(x, y, new Color(0, 0, 0, 0));
                        }
                    }
                }
                imageOverview.Apply();
                Sprite sprite = Sprite.Create(imageOverview, new Rect(0, 0, renderTexture.width, renderTexture.height), Vector2.zero);
                ms.iconSprite = sprite;
            }
        }
        Destroy(set);
    }
}
