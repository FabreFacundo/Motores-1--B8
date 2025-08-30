using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy_agent : MonoBehaviour
{
    public enum ENEMY_STATE { PATROLLING, INVESTIGATING, SEARCHING, ATTACKING }
    #region INSPECTOR_ATTRIBUTES
    [Header("Patrol asignment")]
    [SerializeField] private WaypointPath _patrolSector;
    [Header("NavMeshAgent attributes")]
    [SerializeField][Range(0.1f, 5f)] private float _stoppingDistance = 0.5f;
    [SerializeField][Range(0.1f, 50f)] private float _patrolSpeed = 15f;
    [SerializeField][Range(0.1f, 50f)] private float _searchingSpeed = 10f;
    [SerializeField][Range(0.1f, 50f)] private float _attackingSpeed = 20f;
    [SerializeField][Range(0.1f, 50f)] private float _aceleration = 5f;
    [Header("Searching & investigating attributes")]
    [SerializeField][Range(0.1f, 50f)] private float _secBetweenTurn = 5;
    [Header("GUI attributes")]
    [SerializeField] private Color _stoppingDistanceVisualization = Color.red;
    #endregion
    #region INTERNAL_ATTRIBUTES
    private ENEMY_STATE _actualState = ENEMY_STATE.PATROLLING;
    private Vector3 _startWaypoint;
    private int _nextWaypoint=1;
    private NavMeshAgent _agent;
    private Vector3 _lastPlayerPosition;
    private bool _onSearch;
    private bool _haveReach;
    #endregion
    #region PROPERTIES
    public Vector3 LastPlayerPosition
    {
        get { return _lastPlayerPosition;}
        set
        {
            if (value != _lastPlayerPosition) _lastPlayerPosition = value;
        }
    }
    public ENEMY_STATE ActualState
    {
        get { return _actualState; }
        set
        {
            if (value != _actualState) _actualState = value;
        }
    }
    #endregion

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.acceleration = _aceleration;
        _agent.stoppingDistance = _stoppingDistance;  
    }
    // Update is called once per frame
    void Update()
    {
     
        switch(_actualState)
        {
            case ENEMY_STATE.PATROLLING:
                 Patrol(_patrolSector);
                break;
            case ENEMY_STATE.INVESTIGATING:
                Investigate(_lastPlayerPosition);
                break;
        }
       
    }

  
    private void Investigate(Vector3 playerPosition)
    {
        if (!_onSearch)
        {
            _agent.speed = _searchingSpeed;
            _agent.destination = playerPosition;
            _onSearch = true;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _stoppingDistance)
        {
            _actualState = ENEMY_STATE.SEARCHING;
            StartCoroutine(Search());

        }


    }
    IEnumerator Search()
    {
        transform.Rotate(0, 90, 0);
        yield return new WaitForSecondsRealtime(_secBetweenTurn);
        transform.Rotate(0, -180, 0);
        yield return new WaitForSecondsRealtime(_secBetweenTurn);
        _actualState = ENEMY_STATE.PATROLLING;
        _onSearch = false;
    }
    private void Patrol(WaypointPath sector)
    {
        if(sector == null) return;
        if (sector.Waypoints.Count == 0) return;
       
        if (!_agent.hasPath)
        {
            _startWaypoint = sector.Waypoints[0];
            _agent.speed = _patrolSpeed;
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

    #region ON_SCENE_VISUALIZATION
    private void OnDrawGizmos()
    {
        switch (_actualState)
        {
            case ENEMY_STATE.PATROLLING:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere((transform.position + new Vector3(0, transform.localScale.y, 0)), 0.5f);
                Gizmos.color = Color.white;
                break;
            case ENEMY_STATE.INVESTIGATING:
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere((transform.position + new Vector3(0, transform.localScale.y , 0)), 0.5f);
                Gizmos.color = Color.white;
                break;
        }
    }
    void OnDrawGizmosSelected()
    {
        
#if UNITY_EDITOR
        Handles.color = _stoppingDistanceVisualization;
        Handles.DrawWireDisc(transform.position, transform.up, _stoppingDistance);
#endif
    }
    #endregion
}
