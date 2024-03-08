using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav2D{
    public class Node{
        public bool traversable;
        public Vector3 position;
        public int x;
        public int y;
        public int gCost;
        public int hCost;
        public Node pastNode;

        public Node(bool _traversable, Vector3 _position, int _x, int _y){
            traversable = _traversable;
            position = _position;
            x = _x;
            y = _y;
        }

        public int fCost{
            get{
                return gCost + hCost;
            }
        }
    }
}