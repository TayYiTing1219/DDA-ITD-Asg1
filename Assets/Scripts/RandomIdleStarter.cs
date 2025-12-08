using UnityEngine;

public class RandomIdleStarter : MonoBehaviour
{
    private Animator animator;
    // Number of idle animations (e.g., 3: IDLE B1, IDLE A2, IDLE A1)
    public int idleCount = 3;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Pick a random index (0 to idleCount-1)
        int randomIdle = Random.Range(0, idleCount);
        // Send the index to the Animator
        animator.SetInteger("RandomIdleIndex", randomIdle);
    }
}