using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMeshCreator {
    public Dictionary<Material, List<int>> materialTriangles = new Dictionary<Material, List<int>>();
    public Dictionary<Material, List<Vector3>> materialVertices = new Dictionary<Material, List<Vector3>>();
    public Dictionary<Material, List<Vector2>> materialUVs = new Dictionary<Material, List<Vector2>>();
    public List<Vector3> colliderVertices = new List<Vector3>();
    public List<int> colliderTriangles = new List<int>();
    bool[,,] blockRendered;
    Chunk chunk;
    public static Queue<Chunk> nearbyChunksToRender = new Queue<Chunk>();
    public static Queue<Chunk> readyToDisplayChunks = new Queue<Chunk>();
    public static Queue<Chunk> immediateRenderChunks = new Queue<Chunk>();
    public static Queue<Chunk> immediateDisplayChunks = new Queue<Chunk>();
    public static Dictionary<string, Material> materials = new Dictionary<string, Material>();
    public static Material defaultMat;
    public static void onDisplayChunkTick() {
        if (readyToDisplayChunks.Count > 0) {
            Chunk chunk = readyToDisplayChunks.Dequeue();
            if (chunk != null) {
                chunk.display();
            }
        }
    }
    public static void onImmediateConstractMeshTick() {
        try {
            while (immediateRenderChunks.Count > 0) {
                Chunk chunk = immediateRenderChunks.Dequeue();
                if (chunk != null) {
                    pushMeshData(chunk);
                    immediateDisplayChunks.Enqueue(chunk);
                }
            }
        } catch (Exception e) {
            Debug.LogError(e);
        }

    }
    public static void onImmediateDisplayChunksTick() {
        if (immediateDisplayChunks.Count > 0) {
            Chunk chunk = immediateDisplayChunks.Dequeue();
            if (chunk != null) {
                chunk.display();
            }
        }
    }
    public static void onConstractMeshTick() {
        Vector3 l = GameManager.getLocalPlayer().getLocation().getChunkVector().getVector3();
        if (nearbyChunksToRender.Count > 0) {
            Chunk chunk = nearbyChunksToRender.Dequeue();
            if (chunk != null) {
                if (Vector3.Distance(l, chunk.getChunkLocation().getVector3()) <= GameManager.getGameManager().viewDistance) {
                    pushMeshData(chunk);
                    readyToDisplayChunks.Enqueue(chunk);
                }
            }
        }
    }
    public ChunkMeshCreator(Chunk chunk) {
        this.chunk = chunk;
    }
    public static void pushMeshData(Chunk chunk) {
        ChunkMeshCreator rh = new ChunkMeshCreator(chunk);
        rh.prepareMesh();
        chunk.materialVertices = rh.materialVertices;
        chunk.materialUVs = rh.materialUVs;
        chunk.materialTriangles = rh.materialTriangles;
        chunk.colliderVerices = rh.colliderVertices.ToArray();
        chunk.colliderTriangles = rh.colliderTriangles.ToArray();
    }
    private void prepareMesh() {
        chunk.update = false;
        if (chunk.hasBlock == false) {
            return;
        }
        materialVertices.Clear();
        materialTriangles.Clear();
        materialUVs.Clear();
        for (int _d = 0; _d < 6; _d++) {
            Direction direction = (Direction)_d;
            for (int i1 = 0; i1 < GameManager.chunkSize; i1++) {
                blockRendered = new bool[GameManager.chunkSize, GameManager.chunkSize, GameManager.chunkSize];
                for (int i2 = 0; i2 < GameManager.chunkSize; i2++) {
                    for (int i3 = 0; i3 < GameManager.chunkSize; i3++) {
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
                        Block block = chunk.getBlock(x, y, z);
                        if (shouldRenderBlock(block, direction)) {
                            Block originalBlock = block;
                            MyMaterial m = block.getType();
                            Material mat = m.getMaterial(direction);
                            BlockFace[] blockFaces = new BlockFace[4];
                            BlockFace blockFace = originalBlock.getBlockFace(direction);
                            blockFaces[2] = blockFace;
                            blockFaces[3] = blockFace;
                            int belowIncrease = 0;
                            int nextIncrease = 0;
                            while (getBelowBlock(block, direction) != null) {
                                Block lastBlock = block;
                                block = getBelowBlock(block, direction);
                                if (block.getType() != m || shouldRenderBlock(block, direction) == false) {
                                    block = lastBlock;
                                    break;
                                }
                                setRendered(block);
                                belowIncrease++;
                            }
                            BlockFace bf = block.getBlockFace(direction);
                            blockFaces[0] = bf;
                            blockFaces[1] = bf;
                            block = originalBlock;
                            bool exit = false;
                            while (getNextBlock(block, direction) != null && exit == false) {
                                Block oriBlock = getNextBlock(block, direction);
                                block = getNextBlock(block, direction);
                                if (block.getType() != m || shouldRenderBlock(block, direction) == false) {
                                    break;
                                }
                                for (int bi = 1; bi <= belowIncrease; bi++) {
                                    if (getBelowBlock(block, direction) != null) {
                                        block = getBelowBlock(block, direction);
                                        if (block.getType() != m || shouldRenderBlock(block, direction) == false) {
                                            exit = true;
                                            break;
                                        }
                                    } else {
                                        exit = true;
                                        break;
                                    }
                                }
                                if (exit == false) {
                                    block = oriBlock;
                                    setRendered(block);
                                    for (int bi = 1; bi <= belowIncrease; bi++) {
                                        block = getBelowBlock(block, direction);
                                        setRendered(block);
                                    }
                                    nextIncrease++;
                                    blockFaces[2] = oriBlock.getBlockFace(direction);
                                    blockFaces[1] = block.getBlockFace(direction);
                                    block = oriBlock;
                                }
                            }
                            creatFace(mat, blockFaces, nextIncrease, belowIncrease);
                        }
                    }
                }
            }
        }

    }
    private bool shouldRenderBlock(Block b, Direction direction) {
        if (b.getType() == MyMaterial.AIR || isRendered(b) || b.getBlockFace(direction).isVisible(b) == false) {
            return false;
        }
        return true;
    }
    private Block getBelowBlock(Block block, Direction direction) {
        Vector v = block.getVectorInChunk();
        int x = (int)v.getX();
        int y = (int)v.getY();
        int z = (int)v.getZ();
        switch (direction) {
            case Direction.UP:
            case Direction.DOWN: {
                    if (z + 1 >= GameManager.chunkSize) {
                        return null;
                    }
                    return chunk.getBlock(x, y, z + 1);
                }
            case Direction.NORTH:
            case Direction.SOUTH:
            case Direction.EAST:
            case Direction.WEST: {
                    if (y + 1 >= GameManager.chunkSize) {
                        return null;
                    }
                    return chunk.getBlock(x, y + 1, z);
                }
        }
        return null;
    }
    private Block getNextBlock(Block block, Direction direction) {
        Vector v = block.getVectorInChunk();
        int x = (int)v.getX();
        int y = (int)v.getY();
        int z = (int)v.getZ();
        switch (direction) {
            case Direction.UP:
            case Direction.DOWN: {
                    if (x + 1 >= GameManager.chunkSize) {
                        return null;
                    }
                    return chunk.getBlock(x + 1, y, z);
                }
            case Direction.NORTH:
            case Direction.SOUTH: {
                    if (x + 1 >= GameManager.chunkSize) {
                        return null;
                    }
                    return chunk.getBlock(x + 1, y, z);
                }
            case Direction.EAST:
            case Direction.WEST: {
                    if (z + 1 >= GameManager.chunkSize) {
                        return null;
                    }
                    return chunk.getBlock(x, y, z + 1);
                }
        }
        return null;
    }
    private bool isRendered(Block block) {
        Vector l = block.getVectorInChunk();
        if (blockRendered[(int)l.getX(), (int)l.getY(), (int)l.getZ()] == false) {
            return false;
        }
        return true;
    }
    private void setRendered(Block block) {
        Vector l = block.getVectorInChunk();
        blockRendered[(int)l.getX(), (int)l.getY(), (int)l.getZ()] = true;
    }
    public void creatFace(Material mat, BlockFace[] blockFaces, int nextIncrease, int belowIncrease, float scale = 1f) {
        List<int> triangles;
        if (materialTriangles.TryGetValue(mat, out triangles) == false) {
            triangles = new List<int>();
            materialTriangles.Add(mat, triangles);
        }
        List<Vector3> vertices;
        if (materialVertices.ContainsKey(mat) == false) {
            materialVertices.Add(mat, new List<Vector3>());
        }
        materialVertices.TryGetValue(mat, out vertices);
        List<Vector2> uvs;
        if (materialUVs.ContainsKey(mat) == false) {
            materialUVs.Add(mat, new List<Vector2>());
        }
        materialUVs.TryGetValue(mat, out uvs);

        int triOffSet = vertices.Count;
        int cTriOffSet = colliderVertices.Count;

        vertices.Add(blockFaces[0].getVertices()[0] * scale);
        vertices.Add(blockFaces[1].getVertices()[1] * scale);
        vertices.Add(blockFaces[2].getVertices()[2] * scale);
        vertices.Add(blockFaces[3].getVertices()[3] * scale);
        //COLLIDER
        colliderVertices.Add(blockFaces[0].getVertices()[0] * scale);
        colliderVertices.Add(blockFaces[1].getVertices()[1] * scale);
        colliderVertices.Add(blockFaces[2].getVertices()[2] * scale);
        colliderVertices.Add(blockFaces[3].getVertices()[3] * scale);

        nextIncrease += 1;
        belowIncrease += 1;
        uvs.Add(new Vector2(0 * nextIncrease, 1 * belowIncrease));
        uvs.Add(new Vector2(1 * nextIncrease, 1 * belowIncrease));
        uvs.Add(new Vector2(1 * nextIncrease, 0 * belowIncrease));
        uvs.Add(new Vector2(0 * nextIncrease, 0 * belowIncrease));

        switch (blockFaces[0].direction) {
            case Direction.WEST:
            case Direction.NORTH: {
                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 2);
                    triangles.Add(triOffSet + 1);

                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 3);
                    triangles.Add(triOffSet + 2);

                    colliderTriangles.Add(cTriOffSet + 0);
                    colliderTriangles.Add(cTriOffSet + 2);
                    colliderTriangles.Add(cTriOffSet + 1);

                    colliderTriangles.Add(cTriOffSet + 0);
                    colliderTriangles.Add(cTriOffSet + 3);
                    colliderTriangles.Add(cTriOffSet + 2);
                    break;
                }
            default: {
                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 1);
                    triangles.Add(triOffSet + 2);

                    triangles.Add(triOffSet + 0);
                    triangles.Add(triOffSet + 2);
                    triangles.Add(triOffSet + 3);

                    colliderTriangles.Add(cTriOffSet + 0);
                    colliderTriangles.Add(cTriOffSet + 1);
                    colliderTriangles.Add(cTriOffSet + 2);

                    colliderTriangles.Add(cTriOffSet + 0);
                    colliderTriangles.Add(cTriOffSet + 2);
                    colliderTriangles.Add(cTriOffSet + 3);
                    break;
                }
        }


    }
}