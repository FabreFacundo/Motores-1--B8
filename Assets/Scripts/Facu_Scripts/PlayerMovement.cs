using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum TYPE_OF_MOVEMENT { WITH_LATERAL_DISPLACEMENT, WITHOUT_LATERAL_DISPLACEMENT }

    #region INSPECTOR ATTRIBUTES

    [Header("Displacement attributes")]
    [SerializeField] private TYPE_OF_MOVEMENT _typeOfDisplacement = TYPE_OF_MOVEMENT.WITH_LATERAL_DISPLACEMENT;
    [SerializeField][Range(0, 100)] private float _scrollWheelAceleration = 10;
    [SerializeField][Range(100, 10000)] private float _maxForceApplied = 500;
    [SerializeField][Range(100, 10000)] private float _minForceApplied = 150;
    [Header("Rotation attributes")]
    [SerializeField][Range(0, 360)] private float _maxAngularSpeed = 180;
    [SerializeField][Range(0, 360)] private float _minAngularSpeed = 25;
    /*
    [Header("Jump attributes")]
    [SerializeField][Range(0, 9999)] private float _jumpForce = 500;
    [SerializeField] private LayerMask _floorLayer;*/
    #endregion
    #region INTERNAL_ATTRIBUTES
    private float _forceVectorMagnitude;
    private float _angularSpeed;
    private float _rotationAngle;
    private float _interpolationValue;
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
    /* public float JumpForce
     {
         get { return _jumpForce; }
         set { _jumpForce = value<0? 0:value; }
     }
     public LayerMask FloorLayer
     {
         get { return _floorLayer; }
         set { _floorLayer = value; }
     }*/
    #endregion


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
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


    }

    void FixedUpdate()
    {

        _rb.AddForce(_forceVector);
        if (!_onShoulderCam && _typeOfDisplacement == TYPE_OF_MOVEMENT.WITHOUT_LATERAL_DISPLACEMENT)
            _rb.MoveRotation(_rotation);


    }

}
