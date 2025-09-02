using Unity.VisualScripting;
using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    #region INSPECTIOR_ATTRIBUTES
    [Header("Stealth attributes")]
    [SerializeField] float _stealthMultiplier = 5;
    [SerializeField] LayerMask _detectionLayer;
    #endregion
    #region INTERNAL_ATTRIBUTES
    private Rigidbody _rb;
    float _detectionRadious;
    Collider[] _colliders;
    private Survilance _enemySurvilance;
    #endregion
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
                _enemySurvilance = enemyEaring.gameObject.GetComponentInChildren<Survilance>();
                if (_enemySurvilance == false) continue;
                _enemySurvilance.NoiseDetected(transform.position);
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
