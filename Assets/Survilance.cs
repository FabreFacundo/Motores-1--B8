using UnityEngine;
using UnityEngine.UIElements;

public class Survilance : MonoBehaviour
{
    #region INSPECTOR_ATTRIBUTES
    [SerializeField] private float _maxDetectionTime = 3f;
    [SerializeField][Range(0.01f,1f)]private float _cooldawnFactor = 0.2f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _coneRotationSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _minimalDetectionDistance = 5;
    [SerializeField] private Transform _parent;
    #endregion
    #region INTERNAL_ATTRIBUTES
 
    private Enemy_agent _agent;
    private Transform _player;
    private Vector3 _playerDirection;
    private float _detectionTime;
    private float _playerAngle;
    private bool _playerInCone;
    private RaycastHit hit;
    private bool _playerInSight;
    #endregion
    private void Start()
    {
        _agent = GetComponentInParent<Enemy_agent>();
        _player = _agent.Player;
        
    }
     private void Update()
    {
        Debug.Log(_detectionTime + " " + _agent.Agent.remainingDistance);
        if (_agent.OnSearch && (_detectionTime > _maxDetectionTime || _agent.Agent.remainingDistance < _minimalDetectionDistance))
        {
            SightDetected();
        }
        if(!_playerInSight && _detectionTime > 0 )
        {
            if (_detectionTime < 0)
            {
                _detectionTime = 0;
                CallToInvestigate(_agent.LastPlayerPosition);
            }
            _detectionTime -= Time.deltaTime * _cooldawnFactor;
        }
        if(!_playerInSight)
        {
            _coneRotationSpeed = Mathf.Abs(_parent.localRotation.eulerAngles.y) > _maxAngle ? _coneRotationSpeed * -1 : _coneRotationSpeed * 1;
            _parent.Rotate(new Vector3(0, _coneRotationSpeed * Time.deltaTime, 0));
        }
        else
        {
            _parent.LookAt(_player);
        }
        

    }
    public void NoiseDetected(Vector3 position)
    {
        CallToInvestigate(position);
    }
    private void SightDetected()
    {
        _agent.ActualState = Enemy_agent.ENEMY_STATE.ATTACKING;
    }

    private void CallToInvestigate(Vector3 position)
    {
        _agent.LastPlayerPosition = position;
        _agent.ActualState = Enemy_agent.ENEMY_STATE.INVESTIGATING;
    }

     void OnTriggerEnter(Collider collision)
    {
    
        if (1 << collision.gameObject.layer == _playerLayer)
        {
       
            CallToInvestigate(collision.gameObject.transform.position);
            _playerInSight = true;
        }
    }
     void OnTriggerStay(Collider collision)
    {
        if(1<<collision.gameObject.layer == _playerLayer )
        {
            //AGREGAR raycast
            _detectionTime += Time.fixedDeltaTime;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (1 << collision.gameObject.layer == _playerLayer)
        {
            _agent.LastPlayerPosition = _player.position;
            _playerInSight = false;
        } 
    }
}
