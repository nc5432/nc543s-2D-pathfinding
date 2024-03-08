using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav{
    public class Node{
        public bool traversable;
        public Vector3 position;

        public Node(bool _traversable, Vector3 _position){
            traversable = _traversable;
            position = _position;
        }
    }
}