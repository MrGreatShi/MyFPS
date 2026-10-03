using UnityEngine;

public abstract class Attacker : MonoBehaviour
{
    public int damage = 10;

    public bool isPermanent = false;
    public int attackTimes = 1;
    private int currentAttackTimes = 0;
    protected bool AvoidAbstacle = false;
    void OnEnable()
    {
        if (!isPermanent)
        {
            currentAttackTimes = 0;
        }
    }

    protected virtual bool isAttackable(GameObject go)
    {
        return go.CompareTag("Player") || go.CompareTag("Enemy");
    }
    private void ReturnObject()
    {
        PooledObject pooledObject = GetComponent<PooledObject>();
        if (pooledObject != null)
        {
            pooledObject.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isAttackable(other.gameObject))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TryTakeDamage(damage);
            }
            if (!isPermanent)
            {
                currentAttackTimes++;
                if(currentAttackTimes == attackTimes)
                {
                    ReturnObject();
                }
            }
        }
        else
        {
            if (!AvoidAbstacle)
            {
                ReturnObject();
            }
        }
    }
}
