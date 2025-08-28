using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class cameraMovement : MonoBehaviour
{
    #region Inspector attributes
    [Header("Sensitivity attributes")]
    [SerializeField][Range(0,50)] private float _mouseHorizontalSensitivity = 10;
    [SerializeField][Range(0, 50)] private float _mouseVerticallSensitivity = 10;
    [Header("Axis direction attributes")]
    [SerializeField]private bool _xAxisInverted = false;
    [SerializeField]private bool _yAxisInverted = false;
    [Header("Camera attributes")]
    [SerializeField][Range(0, 100)] private float _zoomLevel = 2;
    [Header("Cursor attributes")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Locked;
    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _shoulderCamera;
    private Vector3 _startPosition;
    #endregion

    private float _verticalReference;
    private Quaternion _startRotation;
    private float _xAxis = 0;
    private float _yAxis = 0;
    private Camera _camera;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _camera = Camera.main;
        Cursor.lockState = _lockMode;
        Input.ResetInputAxes();
    }

    // Update is called once per frame
    void LateUpdate()
    {
    
        // agregar camara de arma

        _yAxis =(_yAxisInverted?-1:1) * -Input.GetAxis("Mouse Y") * _mouseVerticallSensitivity ;
        _xAxis = (_xAxisInverted ? -1 : 1) * Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity ;
        
        // Limitar el angulo maximo y minimo de la camara
        _verticalReference = Vector3.Dot(_target.up, (transform.position - _target.position).normalized);
        if(_verticalReference < 0 ) 
        {
            _yAxis = Mathf.Clamp(_yAxis, 0, 1);
        }
        else if(_verticalReference >= 0.75)
        {
            _yAxis = Mathf.Clamp(_yAxis, -1, 0);
        }


        if (Input.GetMouseButton(1))
        {
            _target.Rotate(_target.transform.up,_xAxis);
        }
        else
        {
            transform.RotateAround(_target.position, transform.right, _yAxis);
            transform.LookAt(_target.position);
        }
            transform.RotateAround(_target.position, _target.up, _xAxis);
        

    }
}
