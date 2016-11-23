using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct VoxelFace {
    public Voxel voxel;
    public Direction direction;
    Vector3[] vertices;
    public bool isVisible(VoxelGroup group, Voxel voxel) {
        Vector v = voxel.getVectorInGroup();
        if (v.getX() <= 0 && direction == Direction.WEST) {
            return true;
        }
        if (v.getX() >= group.width - 1 && direction == Direction.EAST) {
            return true;
        }
        if (direction == Direction.DOWN && v.getY() <= 0) {
            return true;
        }
        if (direction == Direction.UP && v.getY() >= group.height - 1) {
            return true;
        }
        if (v.getZ() <= 0 && direction == Direction.SOUTH) {
            return true;
        }
        if (v.getZ() >= group.depth - 1 && direction == Direction.NORTH) {
            return true;
        }
        Voxel next = voxel.getRelative(group, direction);
        if (next == null) {
            return true;
        }
        return next.transparent;
    }
    public VoxelFace(Voxel voxel, Vector3 v, Direction direction) {
        this.voxel = voxel;
        vertices = new Vector3[4];
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
                    vertices[0] = v + new Vector3(0, 1, 1);
                    vertices[1] = v + new Vector3(0, 1, 0);
                    vertices[2] = v + new Vector3(0, 0, 0);
                    vertices[3] = v + new Vector3(0, 0, 1);
                    break;
                }
        }
    }
    public Vector3[] getVertices() {
        return vertices;
    }
}
