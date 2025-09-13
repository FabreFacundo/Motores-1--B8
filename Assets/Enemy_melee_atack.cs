using UnityEngine;
using UnityEngine.AI;

public class Enemy_melee_atack : MonoBehaviour
{
    [Header("Weapon collider reference")]
    [SerializeField] private Collider _hitCollider;
    [Header("Enemy agent reference")]
    [SerializeField] private Enemy_agent _enemy_Agent;

    public void ActivateHitCOllider()
    {
        _hitCollider.enabled = true;
    }
    public void DeactivateHitCOllider()
    {
        _hitCollider.enabled = false;
    }

    public void AllowMovement()
    {
            _enemy_Agent.Agent.isStopped = false;
    }


}
