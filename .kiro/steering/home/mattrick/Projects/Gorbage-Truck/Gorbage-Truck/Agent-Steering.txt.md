---
inclusion: always
---

# TrashRunner Unity Project - AI Steering Rules

## Project Overview

TrashRunner is a 3D endless runner Unity game featuring a garbage truck collecting trash on city streets. The project targets Unity 2022.3 LTS through Unity 6 LTS with URP and the New Input System.

## Critical Constraints

### Line Limit (Non-Negotiable)
- Each C# script file MUST be ≤450 lines (including all content)
- Split large classes proactively: collision handlers, state managers, pool managers, input helpers

### Code Completeness
- NO placeholders, pseudocode, `// TODO`, or `...` incomplete methods
- Every method must have full, functional implementation
- All code must compile without errors in a fresh Unity project

## Code Style & Architecture

### Namespaces
- `TrashRunner.Core` - GameManager, ScoreManager, DifficultyController, AudioManager
- `TrashRunner.Player` - PlayerRunnerController, PlayerInputController, RunnerCameraController
- `TrashRunner.Environment` - TrackSegment, SegmentSpawner, Obstacle, Pickup
- `TrashRunner.UI` - HUDController, MainMenuUI, PauseMenuUI, GameOverUI
- `TrashRunner.Data` - DifficultyConfig, SpawnConfig (ScriptableObjects)

### Unity Patterns
- Use `[SerializeField]` for all inspector-exposed fields (never public fields)
- Reference ScriptableObjects/prefabs via Inspector (NEVER `Resources.Load()`)
- UI scripts: subscribe in `OnEnable()`, unsubscribe in `OnDisable()` (NOT `Start()`/`Awake()`)
- Player movement: `CharacterController.Move()` (not Rigidbody)
- Object pooling: `Dictionary<GameObject, Queue<GameObject>>` pattern

### Physics Rules
- Player uses `CharacterController` component (no Rigidbody)
- Obstacles/Pickups: trigger colliders + kinematic Rigidbody (`isKinematic = true`, `useGravity = false`)
- Identify objects via component scripts (`Obstacle`, `Pickup`), not just tags

## Event Signatures (Use Exact Names)

### GameManager
```csharp
public event System.Action OnRunStarted;
public event System.Action OnRunEnded;
public event System.Action OnPause;
public event System.Action OnResume;
```

### ScoreManager
```csharp
public event System.Action<int> OnCoinsChanged;
public event System.Action<int> OnTrashChanged;
public event System.Action<int> OnLivesChanged;
public event System.Action<float> OnDistanceChanged;
public event System.Action<int> OnScoreChanged;
```

### PlayerInputController
```csharp
public event System.Action OnMoveLeft;
public event System.Action OnMoveRight;
public event System.Action OnJump;
public event System.Action OnSlide;
```

### Obstacle & Pickup
```csharp
public void ApplyEffect(PlayerRunnerController player, ScoreManager scoreManager)
```

## Input System Setup
- Input Asset: `Assets/Input/RunnerInput.inputactions`
- Action Map: `Gameplay`
- Actions: `MoveLeft`, `MoveRight`, `Jump`, `Slide` (all type Button)
- InputActionAsset referenced via `[SerializeField]` (not loaded by path)
- Do NOT use Unity's auto-generated C# classes for input actions

## ScriptableObject Configs
- `DifficultyConfig` - AnimationCurves for speed, obstacle density, pickup density over elapsed time
- `SpawnConfig` - Obstacle/Pickup spawn entries with prefab, probability, min distance
- Both use `[CreateAssetMenu]` attribute, assigned via Inspector

## Performance Requirements
- Use object pooling for segments, obstacles, pickups
- Avoid allocations in `Update()`
- Update UI text on value change, not every frame
- Pre-initialize pools in `Awake`/`Start`

## Output Format
1. Begin with Architecture Overview including Script File Manifest with line counts
2. Present each script with full code block and "Wiring in Unity" instructions
3. No meta-commentary, apologies, or AI limitation notes
