using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nc543.Nav2D;

public class NavAgent : MonoBehaviour
{
    public Transform target;
    float speed = 5;
    Vector3[] path;
    int targetIndex;

    void Start(){
        PathRequestManager.requestPath(transform.position, target.position, onPathFound);
    }

    public void onPathFound(Vector3[] newPath, bool pathFound){
        if (pathFound){
            path = newPath;
            StopCoroutine("followPath");
            StartCoroutine("followPath");
        }
    }

    private IEnumerator followPath(){
        Vector3 currentPoint = path[0];

        while (true){
            if (transform.position == currentPoint){
                targetIndex++;
                if (targetIndex >= path.Length){
                    yield break;
                }
                currentPoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentPoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos(){
        if (path != null){
            for (int i = targetIndex; i < path.Length; i++){
                Gizmos.color = Color.red;
                Gizmos.DrawCube(path[i], new Vector3(0.2f, 0.2f, 0.2f));

                if (i == targetIndex){
                    Gizmos.DrawLine(transform.position, path[i]);
                }else{
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
