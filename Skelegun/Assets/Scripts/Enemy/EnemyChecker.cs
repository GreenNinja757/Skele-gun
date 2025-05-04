using UnityEngine;
using System.Collections.Generic;

public class EnemyChecker : MonoBehaviour
{

    //Parent transform under which all room enemies are spawned
    public Transform enemyAOE;
    // live?enemy list
    public List<EnemyHP> liveEnemies = new List<EnemyHP>();
    // True as long as there are zero living enemies left.
    public bool allEnemiesCleared=> liveEnemies.Count == 0;

    void Start()
    {

        if (enemyAOE == null)
        {
            Debug.LogError("EnemyChecker: enemyAOE not assigned!");
            return;
        }
        // Gather all EnemyHealth components under this room
        foreach (Transform child in enemyAOE)
        {
            var eh = child.GetComponent<EnemyHP>();
            if (eh != null)
            {
                liveEnemies.Add(eh);
                eh.OnDeath+= HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath(EnemyHP deadEnemy)
    {
        // Unsubscribe and remove
        deadEnemy.OnDeath -= HandleEnemyDeath;
        liveEnemies.Remove(deadEnemy);
    }

    private void OnDestroy()
    {
        // Clean up subscriptions
        foreach (var eh in liveEnemies)
        {
            if (eh != null)
                eh.OnDeath -= HandleEnemyDeath;
        }
    }
}
