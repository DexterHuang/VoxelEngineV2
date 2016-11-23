using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeMeshCreator {
    public static GameObject getGameObject(MyMaterial m) {
        Block block = new Block(null, new Vector(0, 0, 0), m);
        BlockFace[] blockfaces = new BlockFace[4];
        ChunkMeshCreator cmc = new ChunkMeshCreator(null);

        for (int _d = 0; _d < 6; _d++) {
            Direction direction = (Direction)_d;
            blockfaces[0] = block.getBlockFace(direction);
            blockfaces[1] = block.getBlockFace(direction);
            blockfaces[2] = block.getBlockFace(direction);
            blockfaces[3] = block.getBlockFace(direction);
            cmc.creatFace(m.getMaterial(direction), blockfaces, 0, 0, scale: 0.4f);
        }

        GameObject rootObject = new GameObject("CubeItemEntity");
        foreach (Material material in cmc.materialVertices.Keys) {
            Mesh mesh = new Mesh();
            mesh.vertices = cmc.materialVertices[material].ToArray();
            mesh.triangles = cmc.materialTriangles[material].ToArray();
            mesh.uv = cmc.materialUVs[material].ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            mesh.Optimize();
            GameObject o = new GameObject(material + "");
            MeshFilter meshFilter = o.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = o.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = material;
            o.transform.SetParent(rootObject.transform);
        }

        MeshCollider meshCollider = rootObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        Mesh colliderMesh = new Mesh();
        colliderMesh.vertices = cmc.colliderVertices.ToArray();
        colliderMesh.triangles = cmc.colliderTriangles.ToArray();
        colliderMesh.RecalculateBounds();
        colliderMesh.RecalculateNormals();
        colliderMesh.MarkDynamic();
        colliderMesh.Optimize();
        meshCollider.sharedMesh = colliderMesh;
        rootObject.tag = "Item";
        return rootObject;
    }
}
