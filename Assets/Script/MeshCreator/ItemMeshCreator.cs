using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemMeshCreator : MonoBehaviour {

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    bool[,,] voxelRendered;
    VoxelGroup voxelGroup;
    Texture2D texture;

    public ItemMeshCreator(Texture2D texture) {
        this.texture = texture;
    }
    public static void init() {

        foreach (MyMaterial m in Enum.GetValues(typeof(MyMaterial))) {
            MaterialSetting ms = m.getSetting();
            if (ms != null && ms.itemRenderStyle == ItemRenderStyle.ITEM) {
                if (ms.texture != null) {
                    ItemMeshCreator imc = new ItemMeshCreator(ms.texture);
                    ms.itemPrefab = imc.getGameObject();
                } else {
                    throw new Exception(m + " does not have a texture attached!");
                }
            } else if (ms != null && ms.isPlaceable) {
                ms.itemPrefab = CubeMeshCreator.getGameObject(m);
            }
        }
    }
    public GameObject getGameObject() {
        voxelGroup = new VoxelGroup(texture);
        GameObject rootObject = new GameObject("SpriteObject");
        //rootObject.AddComponent<Rigidbody>();
        MeshCollider meshCollider = rootObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        Material material = new Material(GameManager.getGameManager().itemMaterial);
        material.mainTexture = texture;
        Mesh colliderMesh = new Mesh();
        List<CombineInstance> meshs = new List<CombineInstance>();
        for (int _d = 0; _d < 6; _d++) {
            Direction direction = (Direction)_d;
            prepareMesh(direction);
            Mesh mesh = getItemMesh(direction);
            if (mesh.vertices.Length > 0) {
                GameObject o = new GameObject(direction + "");
                o.AddComponent<MeshFilter>().sharedMesh = mesh;
                o.AddComponent<MeshRenderer>().sharedMaterial = material;
                o.transform.SetParent(rootObject.transform);
                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.transform = o.transform.localToWorldMatrix;
                meshs.Add(ci);
            }
        }
        colliderMesh.CombineMeshes(meshs.ToArray());
        meshCollider.sharedMesh = colliderMesh;
        rootObject.tag = "Item";
        return rootObject;
    }
    public Mesh getItemMesh(Direction diretion) {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        mesh.MarkDynamic();
        return mesh;
    }
    public void prepareMesh(Direction direction) {
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();
        for (int i1 = 0; i1 < voxelGroup.getHighest(); i1++) {
            voxelRendered = new bool[voxelGroup.getHighest(), voxelGroup.getHighest(), voxelGroup.getHighest()];
            for (int i2 = 0; i2 < voxelGroup.getHighest(); i2++) {
                for (int i3 = 0; i3 < voxelGroup.getHighest(); i3++) {
                    int x = 0;
                    int y = 0;
                    int z = 0;
                    switch (direction) {
                        case Direction.UP:
                        case Direction.DOWN: {
                                x = i2;
                                y = i1;
                                z = i3;
                                break;
                            }
                        case Direction.NORTH:
                        case Direction.SOUTH: {
                                x = i3;
                                y = i2;
                                z = i1;
                                break;
                            }
                        case Direction.EAST:
                        case Direction.WEST: {
                                x = i1;
                                y = i3;
                                z = i2;
                                break;
                            }
                    }
                    Voxel voxel = voxelGroup.getVoxel(x, y, z);
                    if (voxel == null) {
                        continue;
                    }
                    if (shouldRenderVoxel(voxel, direction)) {
                        Voxel originalVoxel = voxelGroup.getVoxel(x, y, z);
                        VoxelFace[] voxelFaces = new VoxelFace[4];
                        VoxelFace voxelFace = originalVoxel.getVoxelFace(direction);
                        voxelFaces[2] = voxelFace;
                        voxelFaces[3] = voxelFace;
                        int belowIncrease = 0;
                        int nextIncrease = 0;
                        while (getBelowVoxel(voxel, direction) != null) {
                            Voxel lastVoxel = voxel;
                            voxel = getBelowVoxel(voxel, direction);
                            if (shouldRenderVoxel(voxel, direction) == false) {
                                voxel = lastVoxel;
                                break;
                            }
                            setRendered(voxel);
                            belowIncrease++;
                        }
                        VoxelFace vf = voxel.getVoxelFace(direction);
                        voxelFaces[0] = vf;
                        voxelFaces[1] = vf;
                        voxel = originalVoxel;
                        bool exit = false;
                        while (getNextVoxel(voxel, direction) != null && exit == false) {
                            Voxel oriVoxel = getNextVoxel(voxel, direction);
                            voxel = getNextVoxel(voxel, direction);
                            if (shouldRenderVoxel(voxel, direction) == false) {
                                break;
                            }
                            for (int bi = 1; bi <= belowIncrease; bi++) {
                                if (getBelowVoxel(voxel, direction) != null) {
                                    voxel = getBelowVoxel(voxel, direction);
                                    if (shouldRenderVoxel(voxel, direction) == false) {
                                        exit = true;
                                        break;
                                    }
                                } else {
                                    exit = true;
                                    break;
                                }
                            }
                            if (exit == false) {
                                voxel = oriVoxel;
                                setRendered(voxel);
                                for (int bi = 1; bi <= belowIncrease; bi++) {
                                    voxel = getBelowVoxel(voxel, direction);
                                    setRendered(voxel);
                                }
                                nextIncrease++;
                                voxelFaces[2] = oriVoxel.getVoxelFace(direction);
                                voxelFaces[1] = voxel.getVoxelFace(direction);
                                voxel = oriVoxel;
                            }
                        }
                        creatFace(voxelFaces, nextIncrease, belowIncrease);
                    }

                }
            }
        }
    }
    private bool shouldRenderVoxel(Voxel voxel, Direction direction) {

        if (voxel == null || isRendered(voxel) || voxel.getVoxelFace(direction).isVisible(voxelGroup, voxel) == false) {
            return false;
        }
        return true;
    }
    private bool isRendered(Voxel voxel) {
        Vector v = voxel.getVectorInGroup();
        if (voxelRendered[(int)v.getX(), (int)v.getY(), (int)v.getZ()] == false) {
            return false;
        }
        return true;
    }
    private void setRendered(Voxel voxel) {
        Vector v = voxel.getVectorInGroup();
        voxelRendered[(int)v.getX(), (int)v.getY(), (int)v.getZ()] = true;
    }
    private Voxel getBelowVoxel(Voxel voxel, Direction direction) {
        Vector v = voxel.getVectorInGroup();
        int x = (int)v.getX();
        int y = (int)v.getY();
        int z = (int)v.getZ();
        switch (direction) {
            case Direction.UP:
            case Direction.DOWN: {
                    if (z + 1 >= voxelGroup.getHighest()) {
                        return null;
                    }
                    return voxelGroup.getVoxel(x, y, z + 1);
                }
            case Direction.NORTH:
            case Direction.SOUTH:
            case Direction.EAST:
            case Direction.WEST: {
                    if (y + 1 >= voxelGroup.getHighest()) {
                        return null;
                    }
                    return voxelGroup.getVoxel(x, y + 1, z);
                }
        }
        return null;
    }
    private Voxel getNextVoxel(Voxel voxel, Direction direction) {
        Vector v = voxel.getVectorInGroup();
        int x = (int)v.getX();
        int y = (int)v.getY();
        int z = (int)v.getZ();
        switch (direction) {
            case Direction.UP:
            case Direction.DOWN: {
                    if (x + 1 >= voxelGroup.getHighest()) {
                        return null;
                    }
                    return voxelGroup.getVoxel(x + 1, y, z);
                }
            case Direction.NORTH:
            case Direction.SOUTH: {
                    if (x + 1 >= voxelGroup.getHighest()) {
                        return null;
                    }
                    return voxelGroup.getVoxel(x + 1, y, z);
                }
            case Direction.EAST:
            case Direction.WEST: {
                    if (z + 1 >= voxelGroup.getHighest()) {
                        return null;
                    }
                    return voxelGroup.getVoxel(x, y, z + 1);
                }
        }
        return null;
    }
    public void creatFace(VoxelFace[] voxelFaces, int nextIncrease, int belowIncrease) {

        int triOffSet = vertices.Count;

        vertices.Add(voxelFaces[0].getVertices()[0] / voxelGroup.getHighest());
        vertices.Add(voxelFaces[1].getVertices()[1] / voxelGroup.getHighest());
        vertices.Add(voxelFaces[2].getVertices()[2] / voxelGroup.getHighest());
        vertices.Add(voxelFaces[3].getVertices()[3] / voxelGroup.getHighest());


        uvs.Add(voxelFaces[0].voxel.getUV0());
        uvs.Add(voxelFaces[1].voxel.getUV1());
        uvs.Add(voxelFaces[2].voxel.getUV2());
        uvs.Add(voxelFaces[3].voxel.getUV3());



        switch (voxelFaces[0].direction) {
            case Direction.NORTH: {
                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 2);
                    triangles.Add(triOffSet + 1);

                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 3);
                    triangles.Add(triOffSet + 2);
                    break;
                }
            default: {
                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 1);
                    triangles.Add(triOffSet + 2);

                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 2);
                    triangles.Add(triOffSet + 3);
                    break;
                }
        }
    }
    public int getVerticeIndex(Vector3 v) {
        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i] == v) {
                return i;
            }
        }
        return -1;

    }
}
