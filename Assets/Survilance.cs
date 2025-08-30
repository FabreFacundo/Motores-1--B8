using UnityEngine;

public class Survilance : MonoBehaviour
{

    #region INTERNAL_ATTRIBUTES
    private Enemy_agent _agent;
    #endregion
    private void Start()
    {
        _agent = GetComponent<Enemy_agent>();   
    }

    public void NoiseDetected(Vector3 Position)
    {
        _agent.LastPlayerPosition = Position; 
        _agent.ActualState = Enemy_agent.ENEMY_STATE.INVESTIGATING;
    }
}
