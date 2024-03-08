using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav2D{
    public class NavigationGrid : MonoBehaviour{
        public LayerMask blockingMask;
        public Vector2 gridSize;
        public float nodeSize;

        [Header("Debug")]
        public Transform seeker;
        public Transform target;
        public bool collisionDebug = false;
        public bool onlyDisplayPath = false;
        public List<Node> path;

        Node[,] navGrid;
        int nodesX;
        int nodesY;

        void Start(){
            nodesX = Mathf.RoundToInt(gridSize.x/nodeSize);
            nodesY = Mathf.RoundToInt(gridSize.y/nodeSize);
            generateNavigationGrid();
        }

        void Update(){
            FindPath(seeker.position, target.position);
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

        private void FindPath(Vector3 start, Vector3 target){
            Node startNode = worldToNavGrid(start);
            Node targetNode = worldToNavGrid(target);

            Heap<Node> open = new Heap<Node>(nodesX * nodesY);
            HashSet<Node> closed = new HashSet<Node>();

            open.add(startNode);

            while (open.getSize() > 0){
                Node currentNode = open.removeFirst();
                closed.Add(currentNode);

                if (currentNode == targetNode){
                    tracePath(startNode, targetNode);
                    return;
                }
                foreach (Node neighbor in getNeighbors(currentNode)){
                    if (neighbor.traversable && !closed.Contains(neighbor)){
                        int newCost = currentNode.gCost + getDistance(currentNode, neighbor);
                        if (newCost < neighbor.gCost || !open.contains(neighbor)){
                            neighbor.gCost = newCost;
                            neighbor.hCost = getDistance(neighbor, targetNode);
                            neighbor.pastNode = currentNode;
                            if (!open.contains(neighbor)) open.add(neighbor);
                        }
                    }
                }
            }
        }

        private void tracePath(Node startNode, Node endNode){
            path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode){
                path.Add(currentNode);
                currentNode = currentNode.pastNode;
            }
            path.Reverse();
        }

        private int getDistance(Node node1, Node node2){
            int distanceX = Mathf.Abs(node1.x - node2.x);
            int distanceY = Mathf.Abs(node1.y - node2.y);

            if (distanceX > distanceY){
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }else{
                return 14 * distanceX + 10 * (distanceY - distanceX);
            }
        }

        void OnDrawGizmos(){
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

            if (navGrid != null && collisionDebug){
                Node checkNode = null;
                if (seeker != null) checkNode = worldToNavGrid(seeker.position);
                foreach (Node node in navGrid){
                    if (onlyDisplayPath){
                        if (!path.Contains(node)) continue;
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(node.position, Vector3.one * (nodeSize - 0.1f));
                    }else{
                        Gizmos.color = (node.traversable) ? Color.green : Color.red;
                        if (checkNode == node) Gizmos.color = Color.blue;
                        if (path != null && path.Contains(node)) Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(node.position, Vector3.one * (nodeSize - 0.1f));
                    }
                }
            }
        }
    }
}