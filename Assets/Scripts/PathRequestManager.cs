using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nc543.Nav2D{
    public class PathRequestManager : MonoBehaviour{
        Queue<PathRequest> requestQueue = new Queue<PathRequest>();
        PathRequest currentRequest;
        private NavigationGrid navGrid;
        private bool isProcessing = false;
        
        public static PathRequestManager instance;

        void Awake(){
            if (instance == null){
                instance = this;
                navGrid = GetComponent<NavigationGrid>();
            }else{
                Destroy(this);
            }
        }

        public static void requestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback){
            PathRequest request = new PathRequest(start, end, callback);
            instance.requestQueue.Enqueue(request);
            instance.tryProcessNext();
        }

        public void finishedPath(Vector3[] path, bool success){
            currentRequest.callback(path, success);
            isProcessing = false;
            tryProcessNext();
        }

        private void tryProcessNext(){
            if (!isProcessing && requestQueue.Count > 0){
                isProcessing = true;
                currentRequest = requestQueue.Dequeue();
                navGrid.startPathfinding(currentRequest.start, currentRequest.end);
            }
        }

        struct PathRequest{
            public Vector3 start;
            public Vector3 end;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback){
                start = _start;
                end = _end;
                callback = _callback;
            }
        }
    }
}