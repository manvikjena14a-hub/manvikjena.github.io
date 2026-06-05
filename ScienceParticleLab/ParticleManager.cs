using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float temperature = 25f;
    [SerializeField] private float pressure = 1f;

    private Dictionary<string, ParticleBehaviour.ElementData> elements = new Dictionary<string, ParticleBehaviour.ElementData>();
    private ReactionManager reactionManager;
    private string selectedElement = "H";
    private List<GameObject> spawnedParticles = new List<GameObject>();

    void Start()
    {
        InitializeElements();
        reactionManager = GetComponent<ReactionManager>();
        if (reactionManager == null)
        {
            reactionManager = gameObject.AddComponent<ReactionManager>();
        }
    }

    void Update()
    {
        HandleInput();
    }

    void InitializeElements()
    {
        // Hydrogen
        elements["H"] = new ParticleBehaviour.ElementData
        {
            symbol = "H",
            atomicNumber = 1,
            atomicMass = 1.008f,
            color = Color.white,
            reactionRadius = 0.5f
        };

        // Oxygen
        elements["O"] = new ParticleBehaviour.ElementData
        {
            symbol = "O",
            atomicNumber = 8,
            atomicMass = 16f,
            color = Color.red,
            reactionRadius = 0.5f
        };

        // Sodium
        elements["Na"] = new ParticleBehaviour.ElementData
        {
            symbol = "Na",
            atomicNumber = 11,
            atomicMass = 22.99f,
            color = new Color(1f, 0.85f, 0f),
            reactionRadius = 0.6f
        };

        // Chlorine
        elements["Cl"] = new ParticleBehaviour.ElementData
        {
            symbol = "Cl",
            atomicNumber = 17,
            atomicMass = 35.45f,
            color = Color.green,
            reactionRadius = 0.5f
        };

        // Carbon
        elements["C"] = new ParticleBehaviour.ElementData
        {
            symbol = "C",
            atomicNumber = 6,
            atomicMass = 12f,
            color = new Color(0.2f, 0.2f, 0.2f),
            reactionRadius = 0.5f
        };
    }

    void HandleInput()
    {
        // Element selection
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedElement = "H";
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedElement = "O";
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedElement = "Na";
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedElement = "Cl";
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedElement = "C";

        // Spawn particle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnParticle(selectedElement);
        }

        // Temperature control
        if (Input.GetKey(KeyCode.W)) IncreaseTemperature();
        if (Input.GetKey(KeyCode.S)) DecreaseTemperature();

        // Pressure control
        if (Input.GetKey(KeyCode.A)) IncreasePressure();
        if (Input.GetKey(KeyCode.D)) DecreasePressure();

        // Clear all
        if (Input.GetKeyDown(KeyCode.C)) ClearAllParticles();
    }

    public void SpawnParticle(string elementSymbol)
    {
        if (!elements.ContainsKey(elementSymbol))
        {
            Debug.LogError("Element not found: " + elementSymbol);
            return;
        }

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0);
        GameObject newParticle = Instantiate(particlePrefab, randomPos, Quaternion.identity);
        
        ParticleBehaviour pb = newParticle.GetComponent<ParticleBehaviour>();
        if (pb != null)
        {
            pb.SetElement(elements[elementSymbol]);
            pb.ApplyHeat(temperature - 25f);
        }

        Rigidbody2D rb = newParticle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Random.insideUnitCircle * 2f;
        }

        spawnedParticles.Add(newParticle);
    }

    public void CheckReaction(ParticleBehaviour particle1, List<ParticleBehaviour> nearbyParticles)
    {
        if (reactionManager == null) return;

        foreach (ParticleBehaviour particle2 in nearbyParticles)
        {
            string reactants = particle1.GetSymbol() + particle2.GetSymbol();
            
            // H + O → H2O (Water)
            if ((particle1.GetSymbol() == "H" && particle2.GetSymbol() == "O") ||
                (particle1.GetSymbol() == "O" && particle2.GetSymbol() == "H"))
            {
                if (reactionManager.TriggerReaction("H2O", particle1.transform.position))
                {
                    Destroy(particle1.gameObject);
                    Destroy(particle2.gameObject);
                    return;
                }
            }

            // Na + Cl → NaCl (Salt)
            if ((particle1.GetSymbol() == "Na" && particle2.GetSymbol() == "Cl") ||
                (particle1.GetSymbol() == "Cl" && particle2.GetSymbol() == "Na"))
            {
                if (reactionManager.TriggerReaction("NaCl", particle1.transform.position))
                {
                    Destroy(particle1.gameObject);
                    Destroy(particle2.gameObject);
                    return;
                }
            }

            // C + O → CO2 (Combustion)
            if ((particle1.GetSymbol() == "C" && particle2.GetSymbol() == "O") ||
                (particle1.GetSymbol() == "O" && particle2.GetSymbol() == "C"))
            {
                if (particle1.GetTemperature() > 100 || particle2.GetTemperature() > 100)
                {
                    if (reactionManager.TriggerReaction("CO2", particle1.transform.position, true))
                    {
                        Destroy(particle1.gameObject);
                        Destroy(particle2.gameObject);
                        return;
                    }
                }
            }
        }
    }

    void IncreaseTemperature()
    {
        temperature += Time.deltaTime * 10f;
        foreach (GameObject particle in spawnedParticles)
        {
            if (particle != null)
            {
                ParticleBehaviour pb = particle.GetComponent<ParticleBehaviour>();
                if (pb != null)
                {
                    pb.ApplyHeat(1f);
                }
            }
        }
    }

    void DecreaseTemperature()
    {
        temperature -= Time.deltaTime * 10f;
        foreach (GameObject particle in spawnedParticles)
        {
            if (particle != null)
            {
                ParticleBehaviour pb = particle.GetComponent<ParticleBehaviour>();
                if (pb != null)
                {
                    pb.CoolDown(1f);
                }
            }
        }
    }

    void IncreasePressure()
    {
        pressure += Time.deltaTime * 0.5f;
        // Apply pressure effect to particles
        foreach (GameObject particle in spawnedParticles)
        {
            if (particle != null)
            {
                Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity *= 1.01f; // Slight velocity increase with pressure
                }
            }
        }
    }

    void DecreasePressure()
    {
        pressure -= Time.deltaTime * 0.5f;
        pressure = Mathf.Max(0.1f, pressure);
    }

    void ClearAllParticles()
    {
        foreach (GameObject particle in spawnedParticles)
        {
            if (particle != null)
            {
                Destroy(particle);
            }
        }
        spawnedParticles.Clear();
    }

    public float GetTemperature() => temperature;
    public float GetPressure() => pressure;
    public string GetSelectedElement() => selectedElement;
}