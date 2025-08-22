using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class cameraMovement : MonoBehaviour
{
    #region Inspector attributes
    [Header("Sensitivity attributes")]
    
    [SerializeField][Range(0,100)] private float _mouseHorizontalSensitivity = 10;
    [SerializeField][Range(0, 100)] private float _mouseVerticallSensitivity = 10;
    [Header("Camera attributes")]
    [SerializeField][Range(0, 100)] private float _zoomLevel = 2;
    [Header("Cursor attributes")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Locked;
    [Header("References")]
    [SerializeField] private Transform _target;
    private Vector3 _startPosition;
    #endregion

    private float _verticalReference;
    private Quaternion _startRotation;
    private float _Xaxis = 0;
    private float _Yaxis = 0;
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

        _Yaxis = Input.GetAxis("Mouse Y") * _mouseVerticallSensitivity ;
        _Xaxis = Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity ;
        
        // Limitar el angulo maximo y minimo de la camara
        _verticalReference = Vector3.Dot(_target.up, (transform.position - _target.position).normalized);
        if(_verticalReference < 0 ) 
        {
            _Yaxis = Mathf.Clamp(_Yaxis, 0, 1);
        }
        else if(_verticalReference >= 0.75)
        {
            _Yaxis = Mathf.Clamp(_Yaxis, -1, 0);
        }

        transform.LookAt(_target.position);
        transform.RotateAround(_target.position, _target.up, _Xaxis);
        transform.RotateAround(_target.position, transform.right, _Yaxis);

    }
}
