using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Chunk {
    [SerializeField]
    public Block[] blocks;
    [SerializeField]
    Vector chunkLocation;
    [SerializeField]
    public bool hasBlock;
    [SerializeField]
    public bool update = true;
    [SerializeField]
    List<Item> entities;
    [SerializeField]
    internal List<int> damagedBlocksIndexs = new List<int>();

    [NonSerialized]
    public bool needSave = false;
    [NonSerialized]
    public Dictionary<Material, List<Vector2>> materialUVs = new Dictionary<Material, List<Vector2>>();
    [NonSerialized]
    public Dictionary<Material, List<Vector3>> materialVertices = new Dictionary<Material, List<Vector3>>();
    [NonSerialized]
    public Dictionary<Material, List<int>> materialTriangles = new Dictionary<Material, List<int>>();
    [NonSerialized]
    GameObject chunkHolder;
    [NonSerialized]
    public Vector3[] colliderVerices;
    [NonSerialized]
    public int[] colliderTriangles;
    [NonSerialized]
    MeshCollider meshCollider;
    [NonSerialized]
    public List<GameObject> childs = new List<GameObject>();
    [NonSerialized]
    ReflectionProbe reflectionProbe;

    public static List<GameObject> childList = new List<GameObject>();
    public static List<Chunk> displayedChunks = new List<Chunk>();
    public static int changed = 0;

    public static string getChunkName(Vector vector) {
        return vector + "";
    }
    public Chunk(int x, int y, int z) {
        init(new Vector(x, y, z));
    }
    public Chunk(Vector chunkLocation) {
        init(chunkLocation);
    }
    public void init(Vector chunkLocation) {
        this.chunkLocation = chunkLocation;
        this.update = true;
        materialTriangles = new Dictionary<Material, List<int>>();
        materialVertices = new Dictionary<Material, List<Vector3>>();
        materialUVs = new Dictionary<Material, List<Vector2>>();
        childs = new List<GameObject>();
    }
    public GameObject createChild(String name) {
        GameObject o = GameManager.spawnPrefab(GameManager.getGameManager()._chunkChildPrefab, Vector3.zero);
        o.transform.name = name;
        o.transform.SetParent(chunkHolder.transform);
        return o;
    }
    public GameObject createChunkGameObject() {
        return GameManager.spawnPrefab(GameManager.chunkPrefab, getChunkStartLocation().getVector3(), ReuseableGameObject.CHUNK);
    }
    public void display() {
        childs.Clear();
        int matCount = materialTriangles.Count;
        if (chunkHolder != null) {
            while (chunkHolder.transform.childCount > matCount) {
                GameObject o = chunkHolder.transform.GetChild(0).gameObject;
                ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.CHUNK_CHILD, o);
                childList.Remove(o);
            }
        }
        if (matCount > 0) {
            displayedChunks.Add(this);
            if (chunkHolder == null) {
                if (materialTriangles.Count == 0) {
                    return;
                }
                chunkHolder = createChunkGameObject();
                chunkHolder.transform.SetParent(GameManager.getGameManager().mapHolder.transform, true);
                meshCollider = chunkHolder.GetComponent<MeshCollider>();
            }
            while (chunkHolder.transform.childCount < matCount) {
                GameObject o = GameManager.spawnPrefab(GameManager.getGameManager()._chunkChildPrefab, Vector3.zero, ReuseableGameObject.CHUNK_CHILD);
                o.transform.SetParent(chunkHolder.transform, false);
            }
            int index = 0;
            foreach (Material mat in materialVertices.Keys) {
                GameObject child = chunkHolder.transform.GetChild(index).gameObject;
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                Mesh mesh = new Mesh();
                List<int> triangles;
                materialTriangles.TryGetValue(mat, out triangles);
                List<Vector3> vertices;
                materialVertices.TryGetValue(mat, out vertices);
                List<Vector2> uvs;
                materialUVs.TryGetValue(mat, out uvs);
                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();

                mesh.uv = uvs.ToArray();

                mesh.RecalculateNormals();

                //mesh.RecalculateBounds();
                mesh.Optimize();

                meshFilter.sharedMesh = mesh;

                meshRenderer.sharedMaterial = mat;
                childList.Add(child);
                childs.Add(child);
                index++;
            }

            Mesh colliderMesh = new Mesh();
            colliderMesh.vertices = this.colliderVerices;
            colliderMesh.triangles = this.colliderTriangles;
            meshCollider.sharedMesh = colliderMesh;

            //Initiate Reflection Probes
            if (GameManager.useReflection) {
                if (reflectionProbe == null) {
                    Vector chunlLoc = getChunkStartLocation();
                    Vector3 probePos = chunlLoc.add(new Vector(GameManager.chunkHalfSize, GameManager.chunkHalfSize, GameManager.chunkHalfSize)).getVector3();
                    for (int y = GameManager.chunkHalfSize; y < GameManager.chunkSize; y++) {
                        if (getBlock(GameManager.chunkHalfSize, y, GameManager.chunkHalfSize).getType() == MyMaterial.AIR) {
                            probePos.y = y + chunlLoc.getY() + 2;
                            //Debug.Log(probePos + "   " + y);
                            break;
                        }
                    }
                    GameObject rpo = GameManager.spawnPrefab(GameManager.getGameManager()._reflectionProbePrefab, probePos, ReuseableGameObject.REFLECTION_PROBE);
                    rpo.transform.SetParent(GameManager.getGameManager().reflectionProbeHolder.transform);
                    reflectionProbe = rpo.GetComponent<ReflectionProbe>();
                }
                reflectionProbe.RenderProbe();
            }

        }
        //Initiate ItemEntities
        if (entities != null) {
            foreach (Item e in entities) {
                e.init();
            }
        }
        entities = null;

        //Initiate DamageOverlay
        for (int i = 0; i < damagedBlocksIndexs.Count; i++) {
            int index = damagedBlocksIndexs[i];
            Block block = getBlock(index);
            block.displayDamage();
        }
        changed++;
    }
    public Vector getChunkLocation() {
        return chunkLocation;
    }
    public Block getBlock(int x, int y, int z) {
        return getBlock(new Vector(x, y, z));
    }
    public Block getBlock(Vector vector) {
        Block block = blocks[getIndex(vector)];
        if (block.location == null) {
            Vector chunkVector = this.getChunkLocation();
            block.chunkVector = chunkVector;
            block.location = new Location((int)(vector.getX() + (chunkVector.getX() * GameManager.chunkSize)), (int)(vector.getY() + (chunkVector.getY() * GameManager.chunkSize)), (int)(vector.getZ() + (chunkVector.getZ() * GameManager.chunkSize)));
            block.vectorInChunk = vector;
        }
        return block;
    }
    public Block getBlock(int index) {
        int x = (int)Mathf.Floor(index / (GameManager.chunkSize * GameManager.chunkSize));
        int y = (int)Mathf.Floor((index - (x * GameManager.chunkSize * GameManager.chunkSize)) / GameManager.chunkSize);
        int z = (int)Mathf.Floor(index % GameManager.chunkSize);
        Vector vector = new Vector(x, y, z);
        return getBlock(vector);
    }
    internal static int getIndex(Vector vector) {
        return ((int)vector.getX() * GameManager.chunkSize * GameManager.chunkSize) + ((int)vector.getY() * GameManager.chunkSize) + (int)vector.getZ();
    }
    public void setBlock(Vector vector, Block block) {
        blocks[getIndex(vector)] = block;
    }
    public Vector getChunkStartLocation() {
        return getChunkLocation().multiply(GameManager.chunkSize);
    }
    public override string ToString() {
        return "chunk: " + chunkLocation;
    }
    public void unload(bool async) {
        if (reflectionProbe != null) {
            ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.REFLECTION_PROBE, reflectionProbe.gameObject);
            reflectionProbe = null;
        }
        foreach (GameObject c in childs) {
            childList.Remove(c);
            ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.CHUNK_CHILD, c);
        }
        ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.CHUNK, chunkHolder);
        int x = (int)chunkLocation.getX() + GameManager.halfMaxChunkCount;
        int y = (int)chunkLocation.getY();
        int z = (int)chunkLocation.getZ() + GameManager.halfMaxChunkCount;
        displayedChunks.Remove(this);
        GameManager.chunks[x, y, z] = null;

        Bounds b = new Bounds();
        List<Entity> itemEntities = GameManager.getEntitiesByType(ReuseableGameObject.ITEM_ENTITY);
        b.min = getChunkStartLocation().getVector3();
        b.max = getChunkStartLocation().getVector3() + new Vector3(GameManager.chunkSize, GameManager.chunkSize, GameManager.chunkSize);
        this.entities = new List<Item>();
        foreach (Entity e in itemEntities.ToArray()) {
            if (b.Contains(e.getLocation().getVector3())) {
                e.remove();
                entities.Add((Item)e);
                needSave = true;
            }
        }

        if (needSave || damagedBlocksIndexs.Count > 0) {
            string filePath = ChunkLoader.getChunkSaveFilePath(chunkLocation);
            if (async) {
                Scheduler.runTaskAsynchronously(() => {
                    Serializer.serializeAndSave<Chunk>(this, filePath);
                });
            } else {
                Serializer.serializeAndSave<Chunk>(this, filePath);
            }
            needSave = false;
        }
        //blocks = new Block[0, 0, 0];
    }
}
