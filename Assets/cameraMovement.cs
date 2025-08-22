using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    [SerializeField] private float _mouseHorizontalSensitivity = 10;
    [SerializeField] private float _mouseVerticallSensitivity = 10;
    [SerializeField] private float _zoomLevel = 2;
    [SerializeField] private Transform _target;
    
    private Vector3 _startPosition;
    private float _verticalReference;
    private Quaternion _startRotation;
    private float _Xaxis;
    private float _Yaxis;
    private Camera _camera;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _Yaxis = Input.GetAxis("Mouse Y") * _mouseVerticallSensitivity * Time.deltaTime;
        _Xaxis = Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity * Time.deltaTime;

        _verticalReference = Vector3.Dot(_target.up,(_target.position - transform.position).normalized);
        if(_verticalReference < 0 ) 
        {
            _Yaxis = Mathf.Clamp(_Yaxis, 0, 1);
        }

       // transform.LookAt( _target.position );
        transform.Rotate(_target.up,_Xaxis);
        transform.Rotate(_target.right, _Yaxis);

    }
}
