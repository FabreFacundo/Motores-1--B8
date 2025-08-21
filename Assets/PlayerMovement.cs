using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region: atributes

    [SerializeField][Range(-1, 1)] private float mousewheel;
    [SerializeField] [Range(0,100)] private float _maxMovSpeed = 25;
    [SerializeField][Range(0, 360)] private float _maxAngularSpeed = 180;
    [SerializeField][Range(0, 100)] private float _minMovSpeed = 5;
    [SerializeField][Range(0, 360)] private float _minAngularSpeed = 25;
    [SerializeField] [Range(0, 10000)] private float _jumpForce = 500;
    [SerializeField] private LayerMask _floorLayer;
    
    private float _movementSpeed;
    private float _angularSpeed;
    private Vector3 _moveV;
    private Vector3 _moveH;
    private float _rotationAngle;
    private Vector3 _movementOffset;
    private Vector3 _newPosition;
    private Quaternion _rotation;


    private Rigidbody _rb;
    #endregion

    #region properties
    public float Speed
    {
        get { return _maxMovSpeed; }
        set { _maxMovSpeed = value < 0 ? 0 : value; }
    }
    public float JumpForce
    {
        get { return _jumpForce; }
        set { _jumpForce = value<0? 0:value; }
    }
    public LayerMask FloorLayer
    {
        get { return _floorLayer; }
        set { _floorLayer = value; }
    }
    #endregion


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    
    void FixedUpdate()
    {

        _movementSpeed = Mathf.Lerp(_minMovSpeed, _maxMovSpeed, 0.5f + mousewheel); //0.5f+Input.GetAxis("Mouse ScrollWheel"));
        _angularSpeed = Mathf.Lerp(_maxAngularSpeed, _minAngularSpeed, 0.5f + mousewheel);
        _moveV = transform.forward * Input.GetAxis("Vertical");
        _moveH = transform.right * Input.GetAxis("Horizontal_displacement");
        _rotationAngle = Input.GetAxis("Horizontal") * _angularSpeed * Time.deltaTime;
        _rotation = transform.rotation * Quaternion.Euler(0, _rotationAngle, 0);

        _movementOffset = (_moveH + _moveV) * _movementSpeed * Time.deltaTime;
        _newPosition = transform.position + _movementOffset  ;
        
        _rb.MovePosition(_newPosition);
        _rb.MoveRotation(_rotation);

    }
}
