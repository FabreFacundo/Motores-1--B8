using UnityEngine;

public class SecurityGuard : Enemy
{

    [SerializeField] private Animator _animator;


    private Enemy_agent _agent;
    private Rigidbody _rb;
   
    private void Start()
    {
        _agent = GetComponent<Enemy_agent>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        //_animator.SetTrigger("attack");
    }

    public override void Disable()
    {
        _agent.Agent.enabled = false;
        _agent.enabled = false;
        _rb.excludeLayers = LayerMask.NameToLayer("Everything");
    }
}
