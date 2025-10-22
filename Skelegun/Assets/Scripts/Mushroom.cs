using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class Mushroom : Enemy
{
    private EnemyStats stats;

    [Header("Duplication Settings")]
    [SerializeField] private float spawnInterval = 1f;   
    [SerializeField] private float spawnDistance = 1f;   
    [SerializeField] private int maxClones = 5;          

    private float timer;

    // Track how many mushrooms exist at once
    private static int currentCloneCount = 0;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        currentCloneCount++;
    }

    void OnDestroy()
    {
        // Make sure the counter decreases when this mushroom dies
        currentCloneCount--;
    }

    void Update()
    {
        if (stats == null || stats.currentHealth <= 0) return;

        // Base enemy behavior
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
        {
            Move();
        }

        Animate();
        FlipSprite();

        // Duplication timer
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            TryDuplicate();
            timer = 0f;
        }
    }

    private void TryDuplicate()
    {
        // Stop if the clone cap is hit
        if (currentCloneCount >= maxClones) return;

        // Picks a random direction
        Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnDistance;
        Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

        // Ensure the mushroom isn't already destroyed
        if (this == null || gameObject == null) return;

        // Spawns a new mushroom
        Instantiate(gameObject, spawnPosition, Quaternion.identity);
    }
}




