using UnityEngine;

public class CryoRounds : Item, IEnemyHitEffect
{
    public void ApplyEnemyHitEffect(GameObject Enemy)
    {
        Enemy.GetComponent<EnemyStats>().ApplyStatusEffect("fire");
    }
}
