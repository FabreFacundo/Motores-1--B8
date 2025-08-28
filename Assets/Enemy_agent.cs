using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy_agent : MonoBehaviour
{
    [Header("Patrol asignment")]
    [SerializeField] private WaypointPath _patrolSector;
    [Header("NavMeshAgent attributes")]
    [SerializeField][Range(0.1f, 5f)] private float _stoppingDistance = 0.5f;
    [SerializeField][Range(0.1f, 50f)] private float _speed = 15f;
    [SerializeField][Range(0.1f, 50f)] private float _aceleration = 5f;

    [Header("GUI attributes")]
    [SerializeField] private Color _stoppingDistanceVisualization = Color.red;

    private Vector3 _startWaypoint;
    private int _nextWaypoint=1;
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _agent.acceleration = _aceleration;
        _agent.stoppingDistance = _stoppingDistance;
        
    }

    // Update is called once per frame
    void Update()
    {

        Patrol(_patrolSector);
    }

    private void Patrol(WaypointPath sector)
    {
        if(sector == null) return;
        if (sector.Waypoints.Count == 0) return;

        if(!_agent.hasPath)
        {
            _startWaypoint = sector.Waypoints[0];

            _agent.SetDestination(_startWaypoint);
        }
        else
        {

            if (!_agent.pathPending && _agent.remainingDistance<=_stoppingDistance)
            {
                if (_nextWaypoint == sector.Waypoints.Count) _nextWaypoint = 0;
                _agent.destination = sector.Waypoints[_nextWaypoint];
                _nextWaypoint++;

            }
                

        }
    }

     void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = _stoppingDistanceVisualization;
        Handles.DrawWireDisc(transform.position, transform.up, _stoppingDistance);
#endif
    }

}
