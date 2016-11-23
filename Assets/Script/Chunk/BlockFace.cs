using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockFace {
    public Direction direction;
    Vector3[] vertices;
    public bool isVisible(Block block) {
        //Vector v = block.getVectorInChunk();
        //if (v.getX() <= 0 && direction == Direction.WEST) {
        //    return true;
        //}
        //if (v.getX() >= GameManager.chunkSize - 1 && direction == Direction.EAST) {
        //    return true;
        //}
        if (direction == Direction.DOWN && block.getLocation().getY() <= 0) {
            return false;
        }
        //if (direction == Direction.UP && v.getY() >= GameManager.chunkSize - 1) {
        //    return true;
        //}
        //if (v.getZ() <= 0 && direction == Direction.SOUTH) {
        //    return true;
        //}
        //if (v.getZ() >= GameManager.chunkSize - 1 && direction == Direction.NORTH) {
        //    return true;
        //}

        Block nextBlock = block.getRelative(direction);
        if (nextBlock.getType().isTransparent()) {
            return true;
        }
        return false;
    }
    public Vector3[] getVertices() {
        return vertices;
    }
    public BlockFace(Block block, Direction direction) {
        vertices = new Vector3[4];
        Vector3 v = block.getVectorInChunk().getVector3();
        this.direction = direction;

        switch (direction) {
            case (Direction.UP): {
                    vertices[0] = v + new Vector3(0, 1, 1);
                    vertices[1] = v + new Vector3(1, 1, 1);
                    vertices[2] = v + new Vector3(1, 1, 0);
                    vertices[3] = v + new Vector3(0, 1, 0);
                    break;
                }
            case (Direction.DOWN): {
                    vertices[0] = v + new Vector3(0, 0, 0);
                    vertices[1] = v + new Vector3(1, 0, 0);
                    vertices[2] = v + new Vector3(1, 0, 1);
                    vertices[3] = v + new Vector3(0, 0, 1);
                    break;
                }
            case (Direction.NORTH): {
                    vertices[0] = v + new Vector3(0, 1, 1);
                    vertices[1] = v + new Vector3(1, 1, 1);
                    vertices[2] = v + new Vector3(1, 0, 1);
                    vertices[3] = v + new Vector3(0, 0, 1);
                    break;
                }
            case (Direction.EAST): {
                    vertices[0] = v + new Vector3(1, 1, 0);
                    vertices[1] = v + new Vector3(1, 1, 1);
                    vertices[2] = v + new Vector3(1, 0, 1);
                    vertices[3] = v + new Vector3(1, 0, 0);
                    break;
                }
            case (Direction.SOUTH): {
                    vertices[0] = v + new Vector3(0, 1, 0);
                    vertices[1] = v + new Vector3(1, 1, 0);
                    vertices[2] = v + new Vector3(1, 0, 0);
                    vertices[3] = v + new Vector3(0, 0, 0);
                    break;
                }
            case (Direction.WEST): {
                    vertices[1] = v + new Vector3(0, 1, 1);
                    vertices[0] = v + new Vector3(0, 1, 0);
                    vertices[3] = v + new Vector3(0, 0, 0);
                    vertices[2] = v + new Vector3(0, 0, 1);
                    break;
                }
        }
    }
}
