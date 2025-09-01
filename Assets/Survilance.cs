using UnityEngine;
using UnityEngine.UIElements;

public class Survilance : MonoBehaviour
{
    #region INSPECTOR_ATTRIBUTES
    [SerializeField] private float _maxDetectionTime = 3f;
    [SerializeField][Range(0.01f,1f)]private float _cooldawnFactor = 0.2f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private float _coneRotationSpeed;
    [SerializeField] private float _maxAngle = 90f;
    [SerializeField] private float _lookDampening = 5;
    [SerializeField] private float _minimalDetectionDistance = 2;
    [SerializeField] private Transform _parent;
    #endregion
    #region INTERNAL_ATTRIBUTES
 
    private Enemy_agent _agent;
    private Vector3 _playerDirection;
    private float _detectionTime;
    private bool _playerDetected;
    private RaycastHit hit;
    private bool _playerInSight;
    private bool _positiveRotation;
    private int _direction;
    private Quaternion _parentStartRotation;
    #endregion
    private void Start()
    {
        _agent = GetComponentInParent<Enemy_agent>();
        _parentStartRotation = _parent.localRotation;



    }
     private void Update()
    {
        // si el esta buscando al jugador, lo veo y se cumplen el tiempo de deteccion o la distancia es menor a la minima
        // inicia ataque.
        if ((_agent.OnSearch && _playerInSight) &&
            (_detectionTime > _maxDetectionTime ||
            _agent.Agent.remainingDistance < _minimalDetectionDistance))
        {
            SightDetected();
        }
        
       

       
        // si deja de ver al jugador pero no se acabo el cooldown, voy bajando el cooldown
        if(!_playerInSight && _detectionTime > 0 ) 
        {
            _detectionTime -= Time.deltaTime * _cooldawnFactor;
        }
        else if (_detectionTime<=0) 
        {
            if(_agent.OnSearch) // finaliza la busqueda y vuelve a patrulla
            {
                _agent.ActualState = Enemy_agent.ENEMY_STATE.PATROLLING;
            }
            _detectionTime = 0;
        }
        // si no veo al jugador retoma el patron visual de busqueda
        if (!_playerInSight)
        {
            
            if(Quaternion.Angle(_parent.localRotation, Quaternion.Euler(0, _maxAngle, 0))<1f)
            {
                _positiveRotation = true;
            }
            else
            if (Quaternion.Angle(_parent.localRotation, Quaternion.Euler(0, -_maxAngle, 0)) < 1f)
            {
                _positiveRotation = false;
            }
            _direction = _positiveRotation ? -1 : 1;
            _parent.Rotate(_parent.up , _direction * _coneRotationSpeed * Time.deltaTime,Space.Self);
        }
        else // si ve al jugador lo busca con la mirada, se interpolan para suavizar el seguimiento
        {
            _parent.LookAt(_agent.Player.position);
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
            Debug.Log("On enter: "+checkPlayerCover(collision.gameObject.transform.position));
           if (checkPlayerCover(collision.gameObject.transform.position))
            {
                CallToInvestigate(collision.gameObject.transform.position);
                _playerInSight = true;
            }
         
        }
    }
     void OnTriggerStay(Collider collision)
    {
        if(1<<collision.gameObject.layer == _playerLayer )
        {
            Debug.Log("On stay: " + checkPlayerCover(collision.gameObject.transform.position));
            if (checkPlayerCover(collision.gameObject.transform.position))
            {
                _detectionTime += Time.fixedDeltaTime;
            }

        }
    }
    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("On exit: " + checkPlayerCover(collision.gameObject.transform.position));
        if (1 << collision.gameObject.layer == _playerLayer)
        {
            if(_playerInSight)
            {
                _agent.LastPlayerPosition = collision.gameObject.transform.position;
                _playerInSight = false;
                _parent.localRotation = _parentStartRotation;
            }
         
        } 
    }

    bool checkPlayerCover(Vector3 playerPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (playerPosition - transform.position).normalized, out hit, _raycastLayers))
        {

            if (1 << hit.collider.gameObject.layer == _playerLayer)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
