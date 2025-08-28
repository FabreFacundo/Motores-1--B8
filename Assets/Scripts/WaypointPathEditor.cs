using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
    WaypointPath path = null;
    RaycastHit hit;
    public override void OnInspectorGUI()
    {
       
        base.OnInspectorGUI();
        if (target == null) return;
         path = (WaypointPath)target;
      
        if (GUILayout.Button("Add waypoint"))
        {
            SceneView actualSceneView = SceneView.lastActiveSceneView;
            if (actualSceneView != null)
            {
                
                if (Physics.Raycast(actualSceneView.camera.transform.position,actualSceneView.camera.transform.forward, out hit))
                {
                    path.AddWaypoint();
                }
            }
        }
        if (GUILayout.Button("Delete all waypoints"))
        {
            path.DeleteAllWaypoints();
        }
    }
 
}
