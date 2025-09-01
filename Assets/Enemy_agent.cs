using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy_agent : MonoBehaviour
{
    public enum ENEMY_STATE { PATROLLING, INVESTIGATING, SEARCHING, ATTACKING }
    public enum ENEMY_PATROL_TYPE { ONE_WAY, CIRCULAR }
    #region INSPECTOR_ATTRIBUTES

  

    [Header("Patrol attributes")]
    [SerializeField] private WaypointPath _patrolSector;
    [SerializeField] private ENEMY_PATROL_TYPE _patrolType;

    [Header("NavMeshAgent attributes")]
    [SerializeField][Range(0.1f, 5f)] private float _stoppingDistance = 0.5f;
    [SerializeField][Range(0.1f, 50f)] private float _patrolSpeed = 15f;
    [SerializeField][Range(0.1f, 50f)] private float _searchingSpeed = 10f;
    [SerializeField][Range(0.1f, 50f)] private float _attackingSpeed = 20f;
    [SerializeField][Range(0.1f, 50f)] private float _aceleration = 5f;

    [Header("Attack attributes")]
    [SerializeField][Range(0.1f, 50f)] private float _minimalAttacKDistance = 2;

    [Header("Searching attributes")]
    [SerializeField][Range(0.1f, 50f)] private float _searchingTime = 5;

    [Header("GUI attributes")]
    [SerializeField] private Color _stoppingDistanceVisualization = Color.red;
    #endregion
    #region INTERNAL_ATTRIBUTES
    private ENEMY_STATE _actualState = ENEMY_STATE.PATROLLING;
    private Vector3 _startWaypoint;
    private Vector3 _nextDestination;
    private int _nextWaypoint=1;
    private NavMeshAgent _agent;
    private Vector3 _lastPlayerPosition;
    private bool _onSearch;
    private bool _invertPatrol;
    private float _elapsedTime=0;
    private Transform _player;
    #endregion
    #region PROPERTIES
    public bool OnSearch
    {
        get { return _onSearch; }
    }
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
    public NavMeshAgent Agent
    {
        get { return _agent; }
    }
    public Transform Player
    {
        get { return _player; }
    }
    #endregion

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.acceleration = _aceleration;
        _agent.stoppingDistance = _stoppingDistance;
        _player = GameObject.Find("Player").transform;
    }
    void Update()
    {
      
        switch (_actualState)
        {
            case ENEMY_STATE.PATROLLING:
                 Patrol();
                break;
            case ENEMY_STATE.INVESTIGATING:
                Investigate(_lastPlayerPosition);
                break;
            case ENEMY_STATE.SEARCHING:
                Search();
                break;
            case ENEMY_STATE.ATTACKING:
                Attack();
                break;
        }
       
    }

    private void Attack()
    {
        if (_player == null) return;
        _agent.speed = _attackingSpeed;
        _agent.destination = _player.position;
        if (_agent.remainingDistance < _minimalAttacKDistance)
        {
            _agent.isStopped = true;
            Debug.Log("Atacando al jugador");
        }
        else _agent.isStopped = false;
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
        }


    }
    private void Search()
    {
        if (!Agent.isStopped) Agent.isStopped = true;
       _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _searchingTime)
        {
            
            _onSearch = false;
            _actualState = ENEMY_STATE.PATROLLING;
            _elapsedTime = 0;
             Agent.isStopped = false;
        }
    }
    private void Patrol()
    {

        if(_patrolSector == null) return;
        if (_patrolSector.Waypoints.Count == 0) return;
        if (!_agent.hasPath)
        {
            _startWaypoint = _patrolSector.Waypoints[0];
            _agent.speed = _patrolSpeed;
            _agent.destination=_startWaypoint;
            _startWaypoint = _agent.destination;
        }
        else
        {
            switch (_patrolType)
            {
                case ENEMY_PATROL_TYPE.ONE_WAY:
                    OneWayPatrol();
                    break;
                case ENEMY_PATROL_TYPE.CIRCULAR:
                    CircularPatrol();
                    break;
            }
        }
     

    }

   private void CircularPatrol()
    {
      
        
        if (!_agent.pathPending && _agent.remainingDistance <= _stoppingDistance)
        {
            
            if (_nextWaypoint == _patrolSector.Waypoints.Count)
            {
                _nextWaypoint = 0;
                _actualState = ENEMY_STATE.SEARCHING;
            }
            else if (_startWaypoint == _agent.destination)
            {
                _actualState = ENEMY_STATE.SEARCHING;
            }
            _agent.destination = _patrolSector.Waypoints[_nextWaypoint];
            _nextWaypoint++;
        }
    
    }   

private void OneWayPatrol()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _stoppingDistance)
        {
            if (_nextWaypoint == _patrolSector.Waypoints.Count)
            {
                _actualState = ENEMY_STATE.SEARCHING;
                return;
            }
            _agent.destination = _patrolSector.Waypoints[_nextWaypoint];
            _nextWaypoint++;

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
            case ENEMY_STATE.SEARCHING:
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere((transform.position + new Vector3(0, transform.localScale.y, 0)), 0.5f);
                Gizmos.color = Color.white;
                break;
            case ENEMY_STATE.ATTACKING:
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((transform.position + new Vector3(0, transform.localScale.y, 0)), 0.5f);
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
