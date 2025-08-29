using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [SerializeField] float _stealthMultiplier = 5;
    [SerializeField] LayerMask _detectionLayer;
    private Rigidbody _rb;
    float _detectionRadious;
    Collider[] _colliders;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();    
    }

    private void FixedUpdate()
    {
        _detectionRadious = _rb.linearVelocity.magnitude * _stealthMultiplier;
        _colliders = Physics.OverlapSphere(transform.position, _detectionRadious , _detectionLayer);
        if (_colliders.Length > 0)
        {
           foreach(Collider enemyEaring in _colliders)
            {
                Debug.Log("Me detecto el enemigo llamado:" + enemyEaring.gameObject.name);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (_rb == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, _detectionRadious);
    }
}
