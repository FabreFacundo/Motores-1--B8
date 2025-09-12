using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum TYPE_OF_MOVEMENT { WITH_LATERAL_DISPLACEMENT, WITHOUT_LATERAL_DISPLACEMENT }

    #region INSPECTOR ATTRIBUTES

    [Header("Displacement attributes")]
    [SerializeField] private TYPE_OF_MOVEMENT _typeOfDisplacement = TYPE_OF_MOVEMENT.WITH_LATERAL_DISPLACEMENT;
    [SerializeField] public float _maxNormalVelocity = 15;
    [SerializeField] public float _maxSprintVelocity = 25;
    [SerializeField] public float _minNormalVelocity = 4;
    [SerializeField][Range(0, 100)] private float _scrollWheelAceleration = 10;
    [SerializeField] private float _maxForceApplied = 500;
    [SerializeField] private float _minForceApplied = 150;
    [Header("Rotation attributes")]
    [SerializeField][Range(0, 360)] private float _maxAngularSpeed = 180;
    [SerializeField][Range(0, 360)] private float _minAngularSpeed = 25;
    [Header("Animations attributes")]
    [SerializeField][Range(0, 2)] private float _stanceChangeMultiplier = 0.5f;
    [SerializeField][Range(0.1f, 5)] private float _animationSpeedMultiplier = 0.5f;
    [SerializeField] private Animator _animator;

    [Header("Colliders references")]
    [SerializeField] private BoxCollider _walkCollider;
    [SerializeField] private BoxCollider _crawlCollider;
    [SerializeField] private BoxCollider _collider;


    #endregion
    #region INTERNAL_ATTRIBUTES
    private float _forceVectorMagnitude;
    private float _angularSpeed;
    private float _rotationAngle;
    private float _maxVelocity;
    private float _interpolationValue;
    private float _stance;
    private Vector3 _moveV;
    private Vector3 _moveH;
    private Vector3 _forceVector;
    private Quaternion _rotation = Quaternion.identity;
    private bool _onShoulderCam = false;
    private Rigidbody _rb;
    #endregion
    #region PROPERTIES
    public float Speed
    {
        get { return _maxForceApplied; }
        set { _maxForceApplied = value < 0 ? 0 : value; }
    }
    public bool OnShoulderCam
    {
        get { return _onShoulderCam; }
    }
    #endregion


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _maxVelocity = _maxNormalVelocity;
        _rb.maxLinearVelocity = _maxVelocity;
    }

    void Update()
    {
        #region FORCE_MAGNITUDE
        _interpolationValue += (Input.GetAxis("Mouse ScrollWheel") * _scrollWheelAceleration * Time.deltaTime);
        _interpolationValue = Mathf.Clamp(_interpolationValue, 0f, 1f);

        _forceVectorMagnitude = Mathf.Lerp(_minForceApplied, _maxForceApplied, _interpolationValue);
        _angularSpeed = Mathf.Lerp(_maxAngularSpeed, _minAngularSpeed, _interpolationValue);
        #endregion
        #region LATERAL_ROTATION
        if (!Input.GetMouseButton(1))
        {
            _onShoulderCam = false;
            _rotationAngle = Input.GetAxis("Horizontal") * _angularSpeed * Time.deltaTime;
            _rotation = transform.rotation * Quaternion.Euler(0, _rotationAngle, 0);
        }
        else
        {
            _onShoulderCam = true;
        }
        #endregion
        #region LATERAL_DISPLACEMENT
        if (_typeOfDisplacement == TYPE_OF_MOVEMENT.WITH_LATERAL_DISPLACEMENT)
        {
            _moveH = transform.right * Input.GetAxis("Horizontal");
        }
        else
        {
            _moveH = transform.right * Input.GetAxis("Horizontal_displacement");
        }
        #endregion
        #region FORCE_VECTOR_CALCULATION
        _moveV = transform.forward * Input.GetAxis("Vertical");
        _forceVector = (_moveH + _moveV).normalized * _forceVectorMagnitude * Time.deltaTime;
        #endregion
        #region STANCE_MODIFICATION_&_COLLISION
        if (Input.GetButton("Crouch"))
        {
            _stance += Time.deltaTime * _stanceChangeMultiplier;
            if(_stance > 1) _stance = 1;    
            _animator.SetFloat("Stances_value", _stance);
        }
        if(Input.GetButton("StandUp"))
        {
            _stance -= Time.deltaTime * _stanceChangeMultiplier;
            if( _stance < 0 ) _stance = 0;
            _animator.SetFloat("Stances_value", _stance);
        }
        _maxVelocity = math.lerp(_maxNormalVelocity, _minNormalVelocity, _stance);
        _rb.maxLinearVelocity = _maxVelocity;
        _collider.center = Vector3.Lerp(_walkCollider.center, _crawlCollider.center, _stance);
        _collider.size = Vector3.Lerp(_walkCollider.size, _crawlCollider.size, _stance);

        #endregion

    }

    void FixedUpdate()
    {

        _rb.AddForce(_forceVector );
        if (!_onShoulderCam && _typeOfDisplacement == TYPE_OF_MOVEMENT.WITHOUT_LATERAL_DISPLACEMENT)
            _rb.MoveRotation(_rotation);
        _animator.speed = (_rb.linearVelocity.magnitude) / _maxVelocity; //  * _animationSpeedMultiplier
    }

}
