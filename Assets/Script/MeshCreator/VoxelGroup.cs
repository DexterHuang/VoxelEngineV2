using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Voxel {
    Vector loc;
    int width;
    int height;
    public bool transparent;
    public Voxel(Vector loc, bool transparent, int width, int height) {
        this.loc = loc;
        this.width = width;
        this.height = height;
        this.transparent = transparent;
    }
    public Vector2 getUV0() {
        return new Vector2((loc.getX() + 0f) / width, (loc.getY() + 1f) / height);
    }
    public Vector2 getUV1() {
        return new Vector2((loc.getX() + 1f) / width, (loc.getY() + 1f) / height);
    }
    public Vector2 getUV2() {
        return new Vector2((loc.getX() + 1f) / width, (loc.getY() + 0f) / height);
    }
    public Vector2 getUV3() {
        return new Vector2((loc.getX() + 0f) / width, (loc.getY() + 0f) / height);
    }
    public VoxelFace getVoxelFace(Direction direction) {
        return new VoxelFace(this, new Vector3(loc.getX(), loc.getY(), 1), direction);
    }
    public Vector getVectorInGroup() {
        return loc;
    }
    internal Voxel getRelative(VoxelGroup group, Direction direction) {
        Vector v = loc.add(direction.toVector());
        return group.getVoxel((int)v.getX(), (int)v.getY(), (int)v.getZ());
    }
}
public struct VoxelGroup {

    Texture2D texture;
    Voxel[,,] voxels;
    public int width;
    public int height;
    public int depth;
    public VoxelGroup(Texture2D texture) {
#if UNITY_EDITOR
        SetTextureImporterFormat(texture, true);
#endif
        depth = 1;
        this.texture = texture;
        this.width = texture.width;
        this.height = texture.height;

        voxels = new Voxel[texture.width, texture.height, depth];
        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
                Color color = texture.GetPixel(x, y);
                if (color.a > 0 && !(color.r == 255 && color.g == 255 && color.b == 255)) {
                    voxels[x, y, 0] = new Voxel(new Vector(x, y, 0), false, width, height);
                }
            }
        }
    }
    public Voxel getVoxel(int x, int y, int z) {
        try {
            return voxels[x, y, z];
        } catch {
            return null;
        }
    }
    public int getHighest() {
        if (width > height) {
            return width;
        } else {
            return height;
        }
    }
    public static void SetTextureImporterFormat(Texture2D texture, bool isReadable) {
#if UNITY_EDITOR
        if (null == texture) return;

        string assetPath = AssetDatabase.GetAssetPath(texture);
        var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (tImporter != null) {
            if (isReadable) {
                tImporter.textureType = TextureImporterType.Advanced;
                tImporter.isReadable = true;
                tImporter.filterMode = FilterMode.Point;
            } else {
                tImporter.textureType = TextureImporterType.Sprite;
            }

            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();
        }
#endif
    }
}
