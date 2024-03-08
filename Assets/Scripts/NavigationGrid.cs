using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav2D{
    public class NavigationGrid : MonoBehaviour{
        public LayerMask blockingMask;
        public Vector2 gridSize;
        public float nodeSize;

        [Header("Debug")]
        public Transform checker;
        public bool collisionDebug = false;
        public List<Node> path;

        Node[,] navGrid;
        int nodesX;
        int nodesY;

        void Start(){
            nodesX = Mathf.RoundToInt(gridSize.x/nodeSize);
            nodesY = Mathf.RoundToInt(gridSize.y/nodeSize);
            generateNavigationGrid();
        }

        private void generateNavigationGrid(){
            navGrid = new Node[nodesX, nodesY];

            Vector3 bottomLeftLoc = transform.position - (Vector3.right * (gridSize.x / 2)) - (Vector3.up * (gridSize.y / 2));

            for (int i = 0; i < nodesX; i++){
                for (int j = 0; j < nodesY; j++){
                    Vector3 loc = bottomLeftLoc + (Vector3.right * ((i * nodeSize) + (nodeSize / 2))) + (Vector3.up * ((j * nodeSize) + (nodeSize / 2)));
                    bool traverse = !Physics2D.OverlapBox(loc, new Vector2(nodeSize / 2, nodeSize / 2), 0, blockingMask);
                    navGrid[i, j] = new Node(traverse, loc, i, j);
                }
            }
        }

        public List<Node> getNeighbors(Node node){
            List<Node> neighbors = new List<Node>();
            for (int i = -1; i <= 1; i++){
                for (int j = -1; j <= 1; j++){
                    if (i == 0 && j == 0) continue;
                    int x = node.x + i;
                    int y = node.y + j;

                    if (x >= 0 && x < nodesX && y >= 0 && y < nodesY){
                        neighbors.Add(navGrid[x, y]);
                    }
                }
            }

            return neighbors;
        }

        public Node worldToNavGrid(Vector3 pos){
            float percentX = Mathf.Clamp01(((pos.x * 1.025f - transform.position.x) / gridSize.x) + 0.5f);
            float percentY = Mathf.Clamp01(((pos.y * 1.025f - transform.position.y) / gridSize.y) + 0.5f);

            return navGrid[Mathf.RoundToInt((nodesX - 1) * percentX), Mathf.RoundToInt((nodesY - 1) * percentY)];
        }

        void OnDrawGizmos(){
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

            if (navGrid != null && collisionDebug){
                Node checkNode = null;
                if (checker != null) checkNode = worldToNavGrid(checker.position);
                foreach (Node node in navGrid){
                    Gizmos.color = (node.traversable) ? Color.green : Color.red;
                    if (checkNode == node) Gizmos.color = Color.blue;
                    if (path != null && path.Contains(node)) Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(node.position, Vector3.one * (nodeSize - 0.1f));
                }
            }
        }
    }
}