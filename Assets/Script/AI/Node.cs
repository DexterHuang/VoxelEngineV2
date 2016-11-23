using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.AI {
    class Node {
        public Vector3 pos;
        public int distanceToTarget;
        public int total;
        public int minValue;
        public Node parent;
        public Node(Vector3 pos, Vector3 target, int minValue) {
            this.pos = pos;
            this.minValue = minValue;
            distanceToTarget = (int)(Math.Abs(pos.x - target.x) + Math.Abs(pos.y - target.y) + Math.Abs(pos.z - target.z)) * 10;
            this.total = distanceToTarget + minValue;
        }
        public override bool Equals(object obj) {
            if (obj is Node) {
                if (((Node)obj).pos.Equals(this.pos)) {
                    return true;
                } else {
                    return false;
                }
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
