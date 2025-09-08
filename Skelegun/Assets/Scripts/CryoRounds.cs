using UnityEngine;

public class PyroRounds : Item, IEnemyHitEffect
{
    public void ApplyEnemyHitEffect(GameObject Enemy)
    {
        Enemy.GetComponent<EnemyStats>().ApplyStatusEffect("ice");
    }
}
