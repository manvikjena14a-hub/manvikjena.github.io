using UnityEngine;
using System.Collections.Generic;

public class ParticleBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class ElementData
    {
        public string symbol;
        public int atomicNumber;
        public float atomicMass;
        public Color color;
        public float reactionRadius = 0.5f;
    }

    public ElementData elementData;
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    private float temperature = 25f; // Celsius
    private float heatCapacity = 1f;
    private ParticleManager particleManager;
    private List<ParticleBehaviour> nearbyParticles = new List<ParticleBehaviour>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        particleManager = FindObjectOfType<ParticleManager>();
        GetComponent<SpriteRenderer>().color = elementData.color;
    }

    void Update()
    {
        UpdateTemperature();
        CheckNearbyParticles();
    }

    public void SetElement(ElementData element)
    {
        elementData = element;
        GetComponent<SpriteRenderer>().color = element.color;
    }

    public void ApplyHeat(float heatAmount)
    {
        temperature += heatAmount / heatCapacity;
        
        // Speed increases with temperature
        float speedMultiplier = 1f + (temperature - 25f) * 0.01f;
        if (rb.velocity.magnitude > 0)
        {
            rb.velocity = rb.velocity.normalized * Mathf.Max(1f, speedMultiplier);
        }

        // Visual feedback
        float scale = 1f + (temperature - 25f) * 0.001f;
        transform.localScale = Vector3.one * Mathf.Clamp(scale, 0.5f, 2f);
    }

    public void CoolDown(float coolAmount)
    {
        temperature -= coolAmount / heatCapacity;
        temperature = Mathf.Max(-273f, temperature);
    }

    void UpdateTemperature()
    {
        // Natural heat loss
        temperature = Mathf.Lerp(temperature, 25f, Time.deltaTime * 0.1f);
    }

    void CheckNearbyParticles()
    {
        nearbyParticles.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, elementData.reactionRadius);
        
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                ParticleBehaviour particle = col.GetComponent<ParticleBehaviour>();
                if (particle != null)
                {
                    nearbyParticles.Add(particle);
                }
            }
        }

        // Check for reactions
        if (particleManager != null)
        {
            particleManager.CheckReaction(this, nearbyParticles);
        }
    }

    public float GetTemperature()
    {
        return temperature;
    }

    public void Explode()
    {
        // Explosion effect
        Rigidbody2D[] nearbyRbs = GetComponentsInChildren<Rigidbody2D>();
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        
        foreach (Collider2D col in nearbyColliders)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
                rb.AddForce(direction * 10f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    public string GetSymbol()
    {
        return elementData.symbol;
    }
}