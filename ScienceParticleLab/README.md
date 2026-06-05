
# Science Particle Lab - Unity 2D Simulator

A realistic science lab experiment simulator built in Unity 2022+ featuring particle physics, chemical reactions, temperature & pressure controls.

## Features

### Core Systems
- **Particle Spawning**: 5 basic elements (H, O, Na, Cl, C)
- **Physics Engine**: 2D collisions, gravity, velocity
- **Temperature System**: Heat transfer, particle behavior changes
- **Pressure Control**: Affects particle velocity and reactions
- **Chemical Reactions**:
  - H + O → H₂O (Water) - Exothermic
  - Na + Cl → NaCl (Salt)
  - C + O → CO₂ (Combustion) - Explosive at high temp
- **Explosion Effects**: Force radius, particle destruction
- **Real-time UI**: Temperature, pressure, element selection

## Setup Instructions

### 1. Create Unity Project
```
- Unity 2022 LTS or newer
- 2D Project template
```

### 2. Project Structure
Create these folders in `Assets/`:
```
Assets/
├── Scripts/
│   ├── ParticleBehaviour.cs
│   ├── ParticleManager.cs
│   ├── ReactionManager.cs
│   └── UIManager.cs
├── Prefabs/
│   └── Particle.prefab
└── Scenes/
    └── MainScene.unity
```

### 3. Create Particle Prefab

1. Create empty GameObject: `Particle`
2. Add components:
   - **SpriteRenderer**
     - Sprite: Any circle sprite (or create white circle)
   - **Rigidbody2D**
     - Body Type: Dynamic
     - Gravity Scale: 0
     - Collision Detection: Continuous
   - **CircleCollider2D**
     - Radius: 0.2
   - Attach **ParticleBehaviour.cs** script
3. Drag into `Assets/Prefabs/` folder as `Particle.prefab`
4. Delete from scene

### 4. Setup Scene

1. Create new scene: `MainScene`
2. Create Canvas for UI (optional)
3. Create empty GameObject: `Managers`
4. Add these scripts to `Managers`:
   - `ParticleManager.cs`
   - `ReactionManager.cs` (auto-added)
5. Configure in Inspector:
   - **ParticleManager**
     - Particle Prefab: Drag `Particle.prefab`
     - Spawn Point: Leave empty or set position
6. Add **UIManager.cs** to another empty GameObject
7. Configure UI elements if using Canvas

### 5. Controls

| Key | Action |
|-----|--------|
| **1** | Select Hydrogen (H) |
| **2** | Select Oxygen (O) |
| **3** | Select Sodium (Na) |
| **4** | Select Chlorine (Cl) |
| **5** | Select Carbon (C) |
| **Space** | Spawn selected element |
| **W** | Increase temperature |
| **S** | Decrease temperature |
| **A** | Increase pressure |
| **D** | Decrease pressure |
| **C** | Clear all particles |

## Game Flow

### Sandbox Mode
1. Spawn particles using number keys + Space
2. Particles move based on velocity and gravity
3. When particles get close, reactions can trigger:
   - **Green + White** → Blue water particle
   - **Yellow + Green** → White salt particle
   - **Black + Red** (hot) → Explosion + gas particle
4. Use W/S to heat/cool, A/D to adjust pressure
5. Watch reactions occur in real-time

### Experiment Mode (Future)
- Guided experiments with objectives
- Temperature/pressure requirements
- Success conditions
- Educational descriptions

## Chemistry Implementation

### Element System
Each element has:
```csharp
symbol          // Chemical symbol (H, O, etc)
atomicNumber    // Atomic number
atomicMass      // Atomic mass
color           // Visual representation
reactionRadius  // Detection radius
```

### Reaction System
```csharp
if (H particle near O particle)
    → Create H2O product
    → Release heat
    → Heat nearby particles
```

### Temperature Effects
- Particles move faster at higher temps
- Size increases with heat
- Combustion reactions require temperature > 100°C
- Explosions spread heat in radius

## Advanced Features (Expandable)

### Future Enhancements
1. **More Elements**: Add 100+ from periodic table
2. **JSON Reactions Database**: Load complex reactions
3. **Molecule Formation**: Bind atoms together
4. **Lab Equipment**: Bunsen burner, beaker, pipette
5. **Particle System VFX**: Explosions, reactions
6. **Save/Load Experiments**: Persist sandbox states
7. **Mobile Touch Controls**: For Android/iOS
8. **Leaderboard**: Best reaction chains

### Expansion Template
```csharp
// Add new element in InitializeElements()
elements["Fe"] = new ParticleBehaviour.ElementData
{
    symbol = "Fe",
    atomicNumber = 26,
    atomicMass = 55.845f,
    color = new Color(0.7f, 0.6f, 0.5f),
    reactionRadius = 0.5f
};

// Add new reaction in CheckReaction()
if ((p1 == "Fe" && p2 == "O") || (p1 == "O" && p2 == "Fe"))
    reactionManager.TriggerReaction("Fe2O3", position);

// Register in ReactionManager
reactions["Fe2O3"] = new ReactionData { ... };
```

## Technical Details

### Physics
- **Collision Detection**: CircleCollider2D
- **Movement**: Rigidbody2D with Physics2D
- **Heat Transfer**: Radial falloff
- **Explosion**: Physics2D.OverlapCircleAll + AddForce

### Performance
- Optimized particle count handling
- Efficient collision queries
- Heat propagation with distance checks
- Clean object destruction

### Audio (Optional)
```csharp
// Add in ReactionManager.cs
AudioSource.PlayClipAtPoint(reactionSound, position);
```

## Troubleshooting

**Particles not spawning?**
- Check Particle prefab assigned in Inspector
- Verify Rigidbody2D is set to Dynamic

**Reactions not triggering?**
- Increase reactionRadius in ElementData
- Lower particle spawn speed to allow collision
- Check temperature requirements for combustion

**Performance issues?**
- Reduce particle count
- Simplify collision detection
- Use object pooling for high-spawn scenarios

**Physics too fast/slow?**
- Adjust Time.timeScale in Edit > Project Settings > Time
- Modify Rigidbody2D velocity multipliers

## Credits

Built with **Unity 2022 LTS**
Physics: Built-in 2D Physics Engine
Educational: Real science principles applied

---

**Enjoy experimenting!** 🔬⚗️🧪
