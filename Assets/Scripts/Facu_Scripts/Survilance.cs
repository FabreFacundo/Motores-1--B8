using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Survilance : MonoBehaviour
{
    #region INSPECTOR_ATTRIBUTES
    [Header("Detection time attributes")]
    [SerializeField] private float _maxSuspectionTime = 3f;
    [SerializeField] private float _maxDetectionTime = 5f;
    [SerializeField][Range(0.01f,2f)]private float _cooldawnFactor = 2f;
    [Header("Physics attributes")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstaclesLayers;
    [Header("Search cone Attributes")]
    [SerializeField] private float _coneRotationSpeed;
    [SerializeField] [Range(0,179)] private float _maxAngle = 90f;
    [SerializeField] private float _minimalDetectionDistance = 2;
    
    [Header("Parent transform")]
    [SerializeField] private Transform _parent;
    #endregion
    #region INTERNAL_ATTRIBUTES
 
    private Enemy_agent _enemyAgent;
    private float _detectedTime;
    private bool _playerDetected;
    private bool _playerInSight;
    private bool _positiveRotation;
    private int _direction;
    Vector3[] _playerColliderLimits = new Vector3[7];
    #endregion
    #region PROPERTIES
    public float DetectedTime
    {
        get { return _detectedTime; }
        set {
            if (value < 0)
            {
                _detectedTime = 0;
            }
            else if (value > _maxDetectionTime)
            {
                _detectedTime = _maxDetectionTime;
            }
            else
            {
                _detectedTime = value;
            }
        }
    }
    #endregion
    private void Start()
    {
        _enemyAgent = GetComponentInParent<Enemy_agent>();
    }
     private void LateUpdate()
    {
        #region SIGHT_SEARCHING_PATTERN
        if (_playerDetected)
        {
            // en caso de que el jugador fuese detecado, el cono se fija sobre la ultima posicion conocida
            // se modifica el eje Y para que mantenga la misma altura del _parent asi no se arruina la rotacion.
            _parent.LookAt(new Vector3(_enemyAgent.LastPlayerPosition.x,_parent.position.y, _enemyAgent.LastPlayerPosition.z));
        }
        else
        {

            if (Quaternion.Angle(_parent.localRotation, Quaternion.Euler(0, _maxAngle, 0)) < 1f)
            {
                _positiveRotation = true;
            }
            else
            if (Quaternion.Angle(_parent.localRotation, Quaternion.Euler(0, -_maxAngle, 0)) < 1f)
            {
                _positiveRotation = false;
            }
            _direction = _positiveRotation ? -1 : 1;
            _parent.Rotate(_parent.up, _direction * _coneRotationSpeed * Time.deltaTime, Space.Self);
        }
        #endregion

        if (_playerInSight)
        { 
            DetectedTime += Time.deltaTime;
        }
        else
        {
            DetectedTime -= Time.deltaTime * _cooldawnFactor;
            // en caso que se haya perdido el contacto visual, se cancela el ataque y se investiga la ultima posicion
            if (_enemyAgent.ActualState == Enemy_agent.ENEMY_STATE.ATTACKING) CallToInvestigate();
        }

        if (DetectedTime >= _maxSuspectionTime && !_enemyAgent.OnInvestigation)
        {
            // en caso que el jugador haya estado lo suficente a la vista, y no se lo este buscando, se genera la busqueda
            CallToInvestigate();
        }
        // si ya se lo esta buscando al jugador, este permance a la vista mas tiempo o
        // la distancia que los separa es menor a el limite
        // se lo detecta visualmente y se habilita el ataque
        else if (_enemyAgent.OnInvestigation &&
            (DetectedTime >= _maxDetectionTime) ||
            Vector3.Distance(_enemyAgent.Player.transform.position,_parent.position) < _minimalDetectionDistance)
        {
            SightDetected();
        }
       if(DetectedTime<=0) 
            // en caso que se haya bajado el tiempo sin que el jugador haya sido detectado
            // se deja de detectarlo
        {
            _playerDetected = false;
            
        }

    }
    public void NoiseDetected(Vector3 position)
    {
        _enemyAgent.LastPlayerPosition = position;
        CallToInvestigate();
    }
    private void SightDetected()
    {
        _enemyAgent.ActualState = Enemy_agent.ENEMY_STATE.ATTACKING;
    }
    private void CallToInvestigate()
    {
        _enemyAgent.ActualState = Enemy_agent.ENEMY_STATE.INVESTIGATING;
    }

    bool checkPlayerCover(Collider playerCollider) // esta funcion permite saber si hay un obstaculo entre ambos objetos
    {
   
        _playerColliderLimits[0] = playerCollider.transform.position;
        _playerColliderLimits[1] = playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y, 0);
        _playerColliderLimits[2] = playerCollider.bounds.center - new Vector3(0, playerCollider.bounds.extents.y, 0);
        _playerColliderLimits[3] = playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0, 0);
        _playerColliderLimits[4] = playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0,0);
        _playerColliderLimits[5] = playerCollider.bounds.center + new Vector3(0,0, playerCollider.bounds.extents.z);
        _playerColliderLimits[6] = playerCollider.bounds.center - new Vector3(0, 0, playerCollider.bounds.extents.z);
        foreach (Vector3 boundPosition in _playerColliderLimits)
        {
            Debug.DrawLine(_parent.transform.position, boundPosition, Color.black);
            if (!Physics.Linecast(_parent.transform.position, boundPosition, _obstaclesLayers, QueryTriggerInteraction.Ignore))
            {
                return true;
            }
        }
       
        return false;
    }
     void OnTriggerStay(Collider collision)
    {
        // compara las layers entre el objeto que esta dentro del trigger, 
        // si es el jugador, lo detecta y va actualizando su posicion mientras sea detectado y no tenga obstaculos en el medio
        if((1 << collision.gameObject.layer & _playerLayer) != 0)
        {
            _playerInSight = checkPlayerCover(collision);
           
            if (_playerInSight)
            {
                if (!_playerDetected)
                {
                    _playerDetected = true;
                }
                _enemyAgent.LastPlayerPosition = collision.transform.position;
            }
          
        }
    }
    private void OnTriggerExit(Collider collision)

    {   // en caso de que salga del cono de vision, y no este obstaculizado, guarda la ultima posicion conocida del jugador.
        if (checkPlayerCover(collision))
        {
          _enemyAgent.LastPlayerPosition = collision.transform.position;
        }
        _playerInSight = false; // marca que el jugador no esta siendo visto
       
    }
}
