using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem bloodParticlePrefab;
    private int currentHealth;

    public bool IsDead { get; private set; }

    public delegate void HealthChanged(int currentHealth, int maxHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void Death();
    public event Death OnDeath;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        IsDead = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        animator.SetTrigger("TakeDamage");
         // Play blood splatter effect
         
         ParticleSystem bloodEffect = Instantiate(bloodParticlePrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
         bloodEffect.Play();
        

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // send a debug message to the console
        Debug.Log("Healed for " + amount + " points. Current health: " + currentHealth);
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
        animator.SetTrigger("Death");
        // Additional death logic (e.g., triggering animations, disabling controls) can be added here.
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        IsDead = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
