using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _shootDistance = 10f;
    [SerializeField] private LayerMask _enemyLayer;

    private PlayerMovement _playerMovement;
    private Vector3 _cameraPoint;
    private RaycastHit _hit;
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();   
    }
    private void Update()
    {
        _cameraPoint = GetCameraPoint();
        
        if(Input.GetMouseButtonDown(0) && _playerMovement.OnShoulderCam)
        {
            if(Physics.Raycast(_shootPosition.position,(_cameraPoint - _shootPosition.position), out _hit, _shootDistance,_enemyLayer))
            {
                _hit.collider.GetComponent<Enemy>().Disable();
            }
        }
    }

    Vector3 GetCameraPoint()
    {
        RaycastHit hit;
        if(Physics.Raycast(_camera.transform.position,_camera.transform.forward,out hit))
        {
            Debug.DrawLine(_shootPosition.position, hit.point);
            return hit.point; 
        }
        else
        {
            return Vector3.zero;
        }
    }

}
