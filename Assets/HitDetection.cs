using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private float _damage = 25f;
    [SerializeField] private LayerMask _playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _playerLayer) != 0)
        {
            Debug.Log("Provocando " + _damage + " puntos de daño XD");
            //other.GetComponent<Player>().TakeDamage(_damage);
        }
    }

}
