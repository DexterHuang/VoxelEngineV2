using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.AI;
using System;

public class NodeComparer : IComparer<Node> {
    int IComparer<Node>.Compare(Node x, Node y) {
        if (x.total == y.total) {
            if (x.distanceToTarget > y.distanceToTarget) {
                return 1;
            }
            return 0;
        } else if (x.total > y.total) {
            return 1;
        } else {
            return -1;
        }
    }
}
public class AIManager {
    static int maxIteration = 500;
    public static void getShortestPath(Vector3 start, Vector3 target) {
        SortedList<Node, int> opened = new SortedList<Node, int>(new NodeComparer());
        List<Vector3> closed = new List<Vector3>();
        Node StartNode = new Node(start, target, 0);
        StartNode.total = 0;
        StartNode.minValue = 0;
        opened.Add(StartNode, 0);
        int ii = 1;
        while (opened.Count > 0) {
            ii++;
            if (ii > maxIteration) {
                return;
            }
            Node node = opened.Keys[0];
            closed.Add(node.pos);
            opened.RemoveAt(0);
            Vector3 pos = node.pos;
            for (int x = -1; x <= 1; x++) {
                for (int z = -1; z <= 1; z++) {
                    Vector3 v = new Vector3(pos.x + x, pos.y, pos.z + z);
                    if (v.Equals(target)) {
                        Node n = node.parent;
                        while (n != null) {
                            GameManager.spawnPrefab(GameManager.getGameManager()._ballPrefab, n.pos + new Vector3(0, 1, 0));
                            n = n.parent;
                        }
                        return;
                    }
                    if (closed.Contains(v) == false) {
                        if (GameManager.getBlock(new Location(new Vector(v))).getType() == MyMaterial.AIR) {

                        }
                        int i = 10;
                        if (x != 0 && z != 0) {
                            i = 14;
                        }
                        Node newNode = new Node(v, target, node.minValue + i);
                        newNode.parent = node;
                        if (opened.ContainsKey(newNode)) {
                            int index = opened.IndexOfKey(newNode);
                            Node oldNode = opened.Keys[index];
                            if (newNode.total < oldNode.total) {
                                oldNode.total = newNode.total;
                                oldNode.parent = newNode.parent;
                                oldNode.minValue = newNode.minValue;
                            }
                        } else {
                            opened.Add(newNode, newNode.minValue);
                        }
                    }
                }
            }
        }

    }
}
