using UnityEngine;
using UnityEngine.AI;

public class SmartEnemy : Enemy
{
    private NavMeshAgent agent;
    private bool navMeshReady = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // NavMeshPlus uses XY, but Unity’s NavMeshAgent expects XZ → force it flat
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>(); // Get the enemy's Rigidbody2D
        playerRb = player.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D

    }

    void Update()
    {
        agent.SetDestination(player.position);
    }


    public void Attack()
    {
        Debug.Log("Smart Enemy Attack!");
        // You can add range-checks or collision-based damage here
    }



    public void EnableNavMesh()
    {
        gameObject.SetActive(true);
        agent.enabled = true;
        navMeshReady = true;
    }
}

