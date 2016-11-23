using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[Serializable]
public class Block {

    [SerializeField]
    MyMaterial material;
    [SerializeField]
    int damagedValue = 0;

    [NonSerialized]
    public Location location;
    [NonSerialized]
    public Vector vectorInChunk;
    [NonSerialized]
    public Vector chunkVector;
    [NonSerialized]
    BlockDamageOverlay damageOverlay;
    [NonSerialized]
    DateTime lastDamagedTime = DateTime.Now;

    public Block(Chunk chunk, Vector vectorInChunk, MyMaterial MyMaterial) {
        this.material = MyMaterial;
        this.vectorInChunk = vectorInChunk;
        //for (int i = 0; i < 6; i++) {
        //    blockFaces[i] = new BlockFace(this, (Direction)i);
        //}
    }
    public Location getLocation() {
        return new Location(location.toVector());
    }
    public Vector getVectorInChunk() {
        return vectorInChunk;
    }
    public Vector getChunkVector() {
        return chunkVector;
    }
    public BlockFace getBlockFace(Direction direction) {
        return new BlockFace(this, direction);//blockFaces[(int)direction];
    }
    public MyMaterial getType() {
        return material;
    }
    public void setType(MyMaterial MyMaterial, bool updateNow = true) {
        material = MyMaterial;
        Stack<Chunk> chunksToUpdate = new Stack<Chunk>();
        Vector chunkVector = getChunkVector();
        Vector vectorInChunk = getVectorInChunk();
        if (vectorInChunk.getX() >= GameManager.chunkSize - 1) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(1, 0, 0)));
            chunksToUpdate.Push(chunk);
        } else if (vectorInChunk.getX() <= 0) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(-1, 0, 0)));
            chunksToUpdate.Push(chunk);
        }
        if (vectorInChunk.getY() >= GameManager.chunkSize - 1) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(0, 1, 0)));
            chunksToUpdate.Push(chunk);
        } else if (vectorInChunk.getY() <= 0) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(0, -1, 0)));
            chunksToUpdate.Push(chunk);
        }
        if (vectorInChunk.getZ() >= GameManager.chunkSize - 1) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(0, 0, 1)));
            chunksToUpdate.Push(chunk);
        } else if (vectorInChunk.getZ() <= 0) {
            Chunk chunk = GameManager.getChunk(chunkVector.add(new Vector(0, 0, -1)));
            chunksToUpdate.Push(chunk);
        }
        Chunk thisChunk = getChunk();
        if (MyMaterial != MyMaterial.AIR) {
            thisChunk.hasBlock = true;
        }
        chunksToUpdate.Push(thisChunk);
        // Debug.Log(chunkVector + "    " + vectorInChunk + "   " + chunksToUpdate.Count);
        while (chunksToUpdate.Count > 0) {
            Chunk chunk = chunksToUpdate.Pop();
            if (updateNow) {
                Scheduler.runTaskAsynchronously(() => {
                    ChunkMeshCreator.pushMeshData(chunk);
                    Scheduler.runTaskSynchronously(() => {
                        chunk.display();
                    });
                });
            } else {
                ChunkMeshCreator.immediateRenderChunks.Enqueue(chunk);
            }
        }
        thisChunk.needSave = true;

        //Remove all block damage
        damagedValue = 0;
        thisChunk.damagedBlocksIndexs.Remove(Chunk.getIndex(getVectorInChunk()));
    }
    public Block getRelative(Direction direction) {
        Vector dv = direction.toVector();
        Location location = getLocation();
        return GameManager.getBlock(new Location(location.getX() + dv.getX(), location.getY() + dv.getY(), location.getZ() + dv.getZ()));
    }
    public Chunk getChunk() {
        return GameManager.getChunk(getChunkVector());
    }
    public override string ToString() {
        return material + " " + getLocation();
    }
    public void damageBlock(int dmg) {
        if (dmg > 0) {
            lastDamagedTime = DateTime.Now;
        } else if (lastDamagedTime != null && (DateTime.Now - lastDamagedTime).TotalMilliseconds < 1000) {
            return;
        }
        damagedValue += dmg;
        if (damagedValue >= getType().getSetting().blockHealth) {
            breakNatually();
        }
        if (damagedValue < 0) {
            damagedValue = 0;
        }
        displayDamage();
    }
    public void breakNatually() {
        if (GameManager.getLocalPlayer().giveItem(material, 1) == false) {
            GameManager.spawnItem(new ItemStack(material, 1), getLocation().add(0.5f, 0.5f, 0.5f));
        }
        setType(MyMaterial.AIR);
    }
    public void displayDamage() {
        if (damagedValue > 0) {
            if (damageOverlay == null) {
                GameObject o = GameManager.spawnPrefab(GameManager.getGameManager()._BlcokDamageOverlayPrefab, getLocation().getVector3(), ReuseableGameObject.BLOCK_DAMAGE_OVERLAY);
                damageOverlay = o.GetComponent<BlockDamageOverlay>(); ;
                getChunk().damagedBlocksIndexs.Add(Chunk.getIndex(getVectorInChunk()));
            }
            damageOverlay.setStage(damagedValue, getType().getSetting().blockHealth);
        } else if (damageOverlay != null) {
            ReuseGameObjectHandler.putToRecycle(ReuseableGameObject.BLOCK_DAMAGE_OVERLAY, damageOverlay.gameObject);
            getChunk().damagedBlocksIndexs.Remove(Chunk.getIndex(getVectorInChunk()));
            damageOverlay = null;
        }
    }
}