using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
    [System.Serializable]
    public class ReactionData
    {
        public string productName;
        public Color productColor;
        public float temperature;
        public bool isExplosive;
        public float explosionForce;
    }

    private Dictionary<string, ReactionData> reactions = new Dictionary<string, ReactionData>();
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private ParticleSystem reactionVFX;

    void Start()
    {
        InitializeReactions();
    }

    void InitializeReactions()
    {
        // H2O Reaction
        reactions["H2O"] = new ReactionData
        {
            productName = "Water (H₂O)",
            productColor = new Color(0.2f, 0.7f, 1f),
            temperature = -286f, // Exothermic
            isExplosive = false,
            explosionForce = 0f
        };

        // NaCl Reaction
        reactions["NaCl"] = new ReactionData
        {
            productName = "Salt (NaCl)",
            productColor = new Color(0.9f, 0.9f, 0.9f),
            temperature = -412f,
            isExplosive = false,
            explosionForce = 0f
        };

        // CO2 Reaction (Combustion)
        reactions["CO2"] = new ReactionData
        {
            productName = "Carbon Dioxide (CO₂)",
            productColor = new Color(0.8f, 0.8f, 0.8f),
            temperature = -393f,
            isExplosive = true,
            explosionForce = 15f
        };
    }

    public bool TriggerReaction(string productKey, Vector3 position, bool isExplosive = false)
    {
        if (!reactions.ContainsKey(productKey))
        {
            Debug.LogWarning("Reaction not found: " + productKey);
            return false;
        }

        ReactionData reaction = reactions[productKey];
        
        // Visual effect
        if (reactionVFX != null)
        {
            Instantiate(reactionVFX, position, Quaternion.identity);
        }

        // Create product particle
        if (productPrefab != null)
        {
            GameObject product = Instantiate(productPrefab, position, Quaternion.identity);
            SpriteRenderer sr = product.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = reaction.productColor;
            }

            Rigidbody2D rb = product.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Random.insideUnitCircle * 2f;
            }

            // Scale product based on reaction
            product.transform.localScale = Vector3.one * 1.2f;
        }

        // Explosion effect
        if (reaction.isExplosive || isExplosive)
        {
            CreateExplosion(position, reaction.explosionForce);
        }

        // Heat release
        ApplyHeatFromReaction(position, Mathf.Abs(reaction.temperature));

        Debug.Log($"Reaction occurred: {reaction.productName} at {position}");
        return true;
    }

    void CreateExplosion(Vector3 position, float force)
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(position, 3f);
        
        foreach (Collider2D col in nearbyColliders)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (rb.position - (Vector2)position).normalized;
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }

        // Destroy particles in explosion radius
        ParticleBehaviour[] particles = FindObjectsOfType<ParticleBehaviour>();
        foreach (ParticleBehaviour particle in particles)
        {
            if (Vector3.Distance(particle.transform.position, position) < 1.5f)
            {
                Destroy(particle.gameObject);
            }
        }
    }

    void ApplyHeatFromReaction(Vector3 position, float heatAmount)
    {
        ParticleBehaviour[] nearbyParticles = FindObjectsOfType<ParticleBehaviour>();
        
        foreach (ParticleBehaviour particle in nearbyParticles)
        {
            float distance = Vector3.Distance(particle.transform.position, position);
            if (distance < 2f)
            {
                float heat = heatAmount * (1f - distance / 2f);
                particle.ApplyHeat(heat);
            }
        }
    }

    public ReactionData GetReaction(string key)
    {
        return reactions.ContainsKey(key) ? reactions[key] : null;
    }

    public Dictionary<string, ReactionData> GetAllReactions()
    {
        return reactions;
    }
}