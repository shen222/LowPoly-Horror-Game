using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject hitTargetVFXPrefab;
    public GameObject hitEnemyVFXPrefab;
    public float speed = 20f;
    public float lifeTime = 4f;
    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    
    void OnTriggerEnter(Collider other)     
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            //play default hit effect
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            if (hitTargetVFXPrefab != null)
            {
                //play target hit effect
                Instantiate(hitTargetVFXPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (hitEnemyVFXPrefab != null)
            {
                //play target hit effect
                Instantiate(hitEnemyVFXPrefab, transform.position, Quaternion.identity);

            }
            // Check if the collided object has a ZombieAI component
            ZombieAI zombie = other.GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage); // Apply damage to the zombie
            }
            Destroy(gameObject);
        }
    }
}

