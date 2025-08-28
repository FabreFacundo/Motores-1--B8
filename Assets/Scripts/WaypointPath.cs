
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Vector3> _wayponts = new List<Vector3>();
    [SerializeField][Range(0.01f,1f)] private float _waypointMarkerRadious=0.2f;
    [SerializeField][Range(0.01f, 1f)] private float _crosshairRadious = 0.2f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Color _startWaypointColor = Color.green;
    [SerializeField] private Color _midWaypointColor = Color.red;
    [SerializeField] private Color _endWaypointColor = Color.yellow;
    public List<Vector3> Waypoints
    {
        get { return _wayponts; }
    }
    
    RaycastHit hit;

    public void AddWaypoint()
    {
      
        _wayponts.Add(hit.point);
    }

    public void DeleteWaypoint(Vector3 waypoint)
    {
        if (waypoint == null) return;
        _wayponts.Remove(waypoint);
    }
    public void DeleteAllWaypoints()
    {
        _wayponts.Clear();
    }
    private void OnDrawGizmosSelected()
    {

       
        SceneView actualSceneView = SceneView.lastActiveSceneView;
        Camera cam = actualSceneView.camera;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(hit.point + offset, _crosshairRadious);
        }
        if (_wayponts.Count < 2 ) return;
        for(int i=0; i< _wayponts.Count-1; i++)
        {

            if (_wayponts[i] == null||_wayponts[i+1] == null) break;

            Gizmos.color = _midWaypointColor;

            if (i == 0) Gizmos.color = _startWaypointColor;

            if (i != _wayponts.Count - 1)
            {
                Gizmos.DrawLine(_wayponts[i], _wayponts[i + 1]);
                Gizmos.DrawSphere(_wayponts[i], _waypointMarkerRadious);
            }   
        }
        if(_wayponts[_wayponts.Count - 1]!= null)
        {
            Gizmos.color = _endWaypointColor;
            Gizmos.DrawSphere(_wayponts[_wayponts.Count - 1], _waypointMarkerRadious);
        }
       
    }

}
