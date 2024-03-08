using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav2D{
    public class Pathfinding : MonoBehaviour{
        public Transform seeker, target;
        NavigationGrid navGrid;

        void Awake(){
            navGrid = GetComponent<NavigationGrid>();
        }

        void Update(){
            FindPath(seeker.position, target.position);
        }

        void FindPath(Vector3 start, Vector3 target){
            Node startNode = navGrid.worldToNavGrid(start);
            Node targetNode = navGrid.worldToNavGrid(target);

            List<Node> open = new List<Node>();
            HashSet<Node> closed = new HashSet<Node>();

            open.Add(startNode);

            while (open.Count > 0){
                Node currentNode = open[0];
                for (int i = 1; i < open.Count; i++){
                    if (open[i].fCost < currentNode.fCost || open[i].fCost == currentNode.fCost && open[i].hCost < currentNode.hCost){
                        currentNode = open[i];
                    }
                }

                open.Remove(currentNode);
                closed.Add(currentNode);

                if (currentNode == targetNode){
                    tracePath(startNode, targetNode);
                    return;
                }
                foreach (Node neighbor in navGrid.getNeighbors(currentNode)){
                    if (neighbor.traversable && !closed.Contains(neighbor)){
                        int newCost = currentNode.gCost + getDistance(currentNode, neighbor);
                        if (newCost < neighbor.gCost || !open.Contains(neighbor)){
                            neighbor.gCost = newCost;
                            neighbor.hCost = getDistance(neighbor, targetNode);
                            neighbor.pastNode = currentNode;
                            if (!open.Contains(neighbor)) open.Add(neighbor);
                        }
                    }
                }
            }
        }

        private void tracePath(Node startNode, Node endNode){
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode){
                path.Add(currentNode);
                currentNode = currentNode.pastNode;
            }
            path.Reverse();

            navGrid.path = path;
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
    }
}