
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


public class WaypointPath : MonoBehaviour
{
    #region INSPECTOR_ATTRIBUTES
    [Header("Waypoints")]
    [SerializeField] private List<Vector3> _waypoints = new List<Vector3>();
    [Header("UI Visualization Attributes")]
    [SerializeField][Range(0.01f,1f)] private float _waypointMarkerRadious=0.2f;
    [SerializeField][Range(0.01f, 1f)] private float _crosshairRadious = 0.2f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color _startWaypointColor = Color.green;
    [SerializeField] private Color _midWaypointColor = Color.red;
    [SerializeField] private Color _endWaypointColor = Color.yellow;
    #endregion
    #region PROPERTIES
    public List<Vector3> Waypoints
    {
        get { return _waypoints; }
    }
    #endregion
    #region INTERNAL_ATTRIBUTES
    RaycastHit hit;
    #endregion
  
    
    public void AddWaypoint()
    {
      
        _waypoints.Add(hit.point);
    }
    public void DeleteAllWaypoints()
    {
        _waypoints.Clear();
    }
    private void OnDrawGizmosSelected()
    {
        // Obtains the scene view in editor runtime to get the camera
        SceneView actualSceneView = SceneView.lastActiveSceneView;
        Camera cam = actualSceneView.camera;
        // Shots a ray in the direction of the forward vector of the camera
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            // Draws a black sphere on the hitpoint of the shoted ray
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(hit.point + offset, _crosshairRadious);
        }

        if (_waypoints.Count < 2 ) return;

       // Draws a sphere in the waypoints and a line between using the asigned colors
            for (int i = 0; i < _waypoints.Count - 1; i++)
            {

                if (_waypoints[i] == null || _waypoints[i + 1] == null) break;

                Gizmos.color = _midWaypointColor;

                if (i == 0) Gizmos.color = _startWaypointColor;

                if (i != _waypoints.Count - 1)
                {
                    Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);
                    Gizmos.DrawSphere(_waypoints[i], _waypointMarkerRadious);
                }
            }
            // Draws a sphere into the last waypoint on the list
        if(_waypoints[_waypoints.Count - 1]!= null)
        {
            Gizmos.color = _endWaypointColor;
            Gizmos.DrawSphere(_waypoints[_waypoints.Count - 1], _waypointMarkerRadious);
        }
       
    }

}
