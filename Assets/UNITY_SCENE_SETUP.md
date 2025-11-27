# Unity Scene Setup Documentation

This document provides comprehensive instructions for setting up the Trash Runner game in Unity. Follow these steps to properly configure the scene, prefabs, and physics layers.

## Table of Contents
1. [Physics Layer Setup](#physics-layer-setup)
2. [MainScene Setup](#mainscene-setup)
3. [TruckPlayer Prefab Setup](#truckplayer-prefab-setup)
4. [TrackSegment Prefab Setup](#tracksegment-prefab-setup)
5. [Obstacle Prefab Setup](#obstacle-prefab-setup)
6. [Pickup Prefab Setup](#pickup-prefab-setup)
7. [UI Canvas Setup](#ui-canvas-setup)
8. [ScriptableObject Assets](#scriptableobject-assets)

---

## Physics Layer Setup

Configure the following physics layers in Unity's **Edit > Project Settings > Tags and Layers**:

- **Layer 6**: `Player`
- **Layer 7**: `Obstacle`
- **Layer 8**: `Pickup`
- **Layer 9**: `Track`

### Collision Matrix Configuration

In **Edit > Project Settings > Physics**, configure the collision matrix:

| Layer | Player | Obstacle | Pickup | Track |
|-------|--------|----------|--------|-------|
| **Player** | ❌ | ✅ | ✅ | ✅ |
| **Obstacle** | ✅ | ❌ | ❌ | ❌ |
| **Pickup** | ✅ | ❌ | ❌ | ❌ |
| **Track** | ✅ | ❌ | ❌ | ❌ |

---

## MainScene Setup

Create a new scene called **MainScene** and add the following GameObjects:

### 1. GameManager GameObject

- **Name**: `GameManager`
- **Components**:
  - `GameManager.cs`
  - `ScoreManager.cs`
  - `DifficultyController.cs`
  - `AudioManager.cs`

#### GameManager Component Configuration

- **SerializeField References**:
  - `scoreManager`: Reference to ScoreManager component (same GameObject)
  - `difficultyController`: Reference to DifficultyController component (same GameObject)
  - `segmentSpawner`: Reference to SegmentSpawner GameObject
  - `playerRunnerController`: Reference to TruckPlayer GameObject's PlayerRunnerController

#### ScoreManager Component Configuration

- **SerializeField Settings**:
  - `scorePerMeter`: `10`
  - `scorePerCoin`: `50`
  - `scorePerTrash`: `100`
  - `initialLives`: `3`

#### DifficultyController Component Configuration

- **SerializeField References**:
  - `difficultyConfig`: Reference to DifficultyConfig ScriptableObject asset

#### AudioManager Component Configuration

- **Add two AudioSource components to the GameManager GameObject**:
  - One for music (set to loop)
  - One for sound effects

- **SerializeField References**:
  - `musicSource`: Reference to music AudioSource
  - `sfxSource`: Reference to SFX AudioSource
  - `jumpClip`: AudioClip for jump sound
  - `slideClip`: AudioClip for slide sound
  - `coinClip`: AudioClip for coin pickup sound
  - `trashClip`: AudioClip for trash pickup sound
  - `hitObstacleClip`: AudioClip for obstacle hit sound
  - `buttonClickClip`: AudioClip for UI button click sound
  - `musicClip`: AudioClip for background music

### 2. SegmentSpawner GameObject

- **Name**: `SegmentSpawner`
- **Components**: `SegmentSpawner.cs`

#### SegmentSpawner Component Configuration

- **SerializeField Settings**:
  - `initialSegmentCount`: `5`
  - `segmentsAhead`: `3`
  - `segmentParent`: Create an empty GameObject called "Segments" and reference it here
  - `segmentPrefab`: Reference to TrackSegment prefab
  - `spawnConfig`: Reference to SpawnConfig ScriptableObject asset
  - `difficultyController`: Reference to DifficultyController component on GameManager
  - `playerTransform`: Reference to TruckPlayer GameObject's Transform

### 3. TruckPlayer GameObject

See [TruckPlayer Prefab Setup](#truckplayer-prefab-setup) section below.

### 4. Main Camera

- **Name**: `Main Camera`
- **Tag**: `MainCamera`
- **Components**: 
  - `Camera`
  - `RunnerCameraController.cs`

#### RunnerCameraController Component Configuration

- **SerializeField Settings**:
  - `playerTransform`: Reference to TruckPlayer GameObject's Transform
  - `offset`: `(0, 5, -10)`
  - `smoothSpeed`: `5`
  - `useSpeedBasedFOV`: `true`
  - `baseFOV`: `60`
  - `maxFOV`: `75`
  - `maxSpeedForFOV`: `20`
  - `difficultyController`: Reference to DifficultyController component on GameManager

### 5. Directional Light

- **Name**: `Directional Light`
- **Type**: Directional Light
- **Rotation**: `(50, -30, 0)`

### 6. UI Canvas

See [UI Canvas Setup](#ui-canvas-setup) section below.

---

## TruckPlayer Prefab Setup

Create a prefab called **TruckPlayer** with the following configuration:

### GameObject Hierarchy

```
TruckPlayer (Layer: Player)
├── Model (3D mesh/visual representation)
└── CollisionTrigger (child GameObject with trigger collider)
```

### TruckPlayer Root GameObject

- **Layer**: `Player`
- **Position**: `(0, 1, 0)`

#### Components

1. **CharacterController**
   - `Height`: `2`
   - `Radius`: `0.5`
   - `Center`: `(0, 1, 0)`

2. **PlayerRunnerController.cs**
   - **Movement Settings**:
     - `baseForwardSpeed`: `5`
     - `laneWidth`: `3`
     - `laneChangeSpeed`: `10`
   - **Jump & Gravity**:
     - `jumpForce`: `10`
     - `gravity`: `-20`
   - **Slide Settings**:
     - `slideDuration`: `1`
   - **References**:
     - `characterController`: Reference to CharacterController component
     - `scoreManager`: Reference to ScoreManager on GameManager
     - `gameManager`: Reference to GameManager component
     - `difficultyController`: Reference to DifficultyController on GameManager
     - `playerInputController`: Reference to PlayerInputController component (same GameObject)
     - `audioManager`: Reference to AudioManager on GameManager

3. **PlayerInputController.cs**
   - **Input Settings**:
     - `runnerInputAsset`: Reference to RunnerInput.inputactions asset
   - **Swipe Settings**:
     - `minSwipeDistance`: `50`
     - `maxSwipeTime`: `1`
     - `directionThreshold`: `0.5`

4. **PlayerCollisionHandler.cs**
   - **References**:
     - `playerRunnerController`: Reference to PlayerRunnerController component (same GameObject)
     - `scoreManager`: Reference to ScoreManager on GameManager

### CollisionTrigger Child GameObject

- **Layer**: `Player`
- **Position**: `(0, 1, 0)` (relative to parent)

#### Components

1. **Box Collider**
   - `Is Trigger`: `true`
   - `Size`: `(1, 2, 1)`
   - `Center`: `(0, 0, 0)`

---

## TrackSegment Prefab Setup

Create a prefab called **TrackSegment** with the following configuration:

### GameObject Hierarchy

```
TrackSegment (Layer: Track)
├── Ground (visual mesh)
├── LaneSpawnPoint_Left
├── LaneSpawnPoint_Center
└── LaneSpawnPoint_Right
```

### TrackSegment Root GameObject

- **Layer**: `Track`
- **Components**: `TrackSegment.cs`

#### TrackSegment Component Configuration

- **Segment Settings**:
  - `length`: `10`
- **Spawn Points**:
  - `laneSpawnPoints[0]`: Reference to LaneSpawnPoint_Left Transform
  - `laneSpawnPoints[1]`: Reference to LaneSpawnPoint_Center Transform
  - `laneSpawnPoints[2]`: Reference to LaneSpawnPoint_Right Transform

### Ground Child GameObject

- **Layer**: `Track`
- **Position**: `(0, 0, 5)` (relative to parent)
- **Scale**: `(10, 0.1, 10)`
- **Components**:
  - `Mesh Filter`: Cube or Plane mesh
  - `Mesh Renderer`: Apply track material
  - `Box Collider`: `Is Trigger = false`

### Lane Spawn Points

Create three empty GameObjects as children:

1. **LaneSpawnPoint_Left**
   - **Position**: `(-3, 0, 5)` (relative to parent)

2. **LaneSpawnPoint_Center**
   - **Position**: `(0, 0, 5)` (relative to parent)

3. **LaneSpawnPoint_Right**
   - **Position**: `(3, 0, 5)` (relative to parent)

---

## Obstacle Prefab Setup

Create obstacle prefabs (e.g., **Barrier**, **Cone**, **Trash Can**) with the following configuration:

### GameObject Structure

```
Obstacle (Layer: Obstacle)
└── Model (3D mesh/visual representation)
```

### Root GameObject

- **Layer**: `Obstacle`
- **Tag**: `Obstacle` (create if needed)

#### Components

1. **Rigidbody**
   - `Is Kinematic`: `true`
   - `Use Gravity`: `false`

2. **Box Collider** (or appropriate collider type)
   - `Is Trigger`: `true`
   - Adjust size to match visual mesh

3. **Obstacle.cs**
   - **References**:
     - `audioManager`: Reference to AudioManager on GameManager (set at runtime or via prefab override)

---

## Pickup Prefab Setup

Create pickup prefabs for **Coin** and **Trash** with the following configuration:

### GameObject Structure

```
Pickup (Layer: Pickup)
└── Model (3D mesh/visual representation)
```

### Root GameObject

- **Layer**: `Pickup`
- **Tag**: `Pickup` (create if needed)

#### Components

1. **Rigidbody**
   - `Is Kinematic`: `true`
   - `Use Gravity`: `false`

2. **Sphere Collider** (or appropriate collider type)
   - `Is Trigger`: `true`
   - Adjust radius to match visual mesh

3. **Pickup.cs**
   - **Pickup Settings**:
     - `pickupType`: `Coin` or `Trash` (depending on prefab)
   - **References**:
     - `audioManager`: Reference to AudioManager on GameManager (set at runtime or via prefab override)

### Coin Prefab Variant

- **Name**: `Pickup_Coin`
- `pickupType`: `Coin`

### Trash Prefab Variant

- **Name**: `Pickup_Trash`
- `pickupType`: `Trash`

---

## UI Canvas Setup

Create a **Canvas** GameObject with the following configuration:

### Canvas GameObject

- **Name**: `Canvas`
- **Render Mode**: `Screen Space - Overlay`
- **Components**:
  - `Canvas`
  - `Canvas Scaler`
  - `Graphic Raycaster`

#### Canvas Scaler Settings

- **UI Scale Mode**: `Scale With Screen Size`
- **Reference Resolution**: `1920 x 1080`
- **Screen Match Mode**: `Match Width Or Height`
- **Match**: `0.5`

### Canvas Hierarchy

```
Canvas
├── HUD
├── MainMenu
├── PauseMenu
└── GameOverMenu
```

---

### 1. HUD Panel

- **Name**: `HUD`
- **Active**: `true` (initially visible)
- **Components**: `HUDController.cs`

#### HUD Layout

Create the following child TextMeshProUGUI elements:

1. **DistanceText**
   - **Anchor**: Top-Left
   - **Position**: `(20, -20)`
   - **Text**: `"0m"`
   - **Font Size**: `36`

2. **ScoreText**
   - **Anchor**: Top-Center
   - **Position**: `(0, -20)`
   - **Text**: `"Score: 0"`
   - **Font Size**: `36`

3. **CoinsText**
   - **Anchor**: Top-Right
   - **Position**: `(-20, -20)`
   - **Text**: `"Coins: 0"`
   - **Font Size**: `28`

4. **TrashText**
   - **Anchor**: Top-Right
   - **Position**: `(-20, -60)`
   - **Text**: `"Trash: 0"`
   - **Font Size**: `28`

5. **LivesText**
   - **Anchor**: Top-Right
   - **Position**: `(-20, -100)`
   - **Text**: `"Lives: 3"`
   - **Font Size**: `28`

#### HUDController Component Configuration

- **UI Text References**:
  - `distanceText`: Reference to DistanceText
  - `scoreText`: Reference to ScoreText
  - `coinsText`: Reference to CoinsText
  - `trashText`: Reference to TrashText
  - `livesText`: Reference to LivesText
- **References**:
  - `scoreManager`: Reference to ScoreManager on GameManager

---

### 2. MainMenu Panel

- **Name**: `MainMenu`
- **Active**: `true` (initially visible)
- **Components**: `MainMenuUI.cs`

#### MainMenu Layout

1. **Background Panel**
   - Full-screen panel with semi-transparent background

2. **TitleText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 200)`
   - **Text**: `"TRASH RUNNER"`
   - **Font Size**: `72`

3. **BestScoreText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 100)`
   - **Text**: `"Best Score: 0"`
   - **Font Size**: `36`

4. **PlayButton** (Button - TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 0)`
   - **Size**: `(300, 80)`
   - **Text**: `"PLAY"`
   - **Font Size**: `48`

#### MainMenuUI Component Configuration

- **UI References**:
  - `playButton`: Reference to PlayButton
  - `bestScoreText`: Reference to BestScoreText
- **Game References**:
  - `gameManager`: Reference to GameManager component
  - `scoreManager`: Reference to ScoreManager component

---

### 3. PauseMenu Panel

- **Name**: `PauseMenu`
- **Active**: `false` (initially hidden)
- **Components**: `PauseMenuUI.cs`

#### PauseMenu Layout

1. **PausePanel** (Panel)
   - Full-screen panel with semi-transparent background
   - **Name**: `PausePanel`

2. **PausedText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 150)`
   - **Text**: `"PAUSED"`
   - **Font Size**: `64`

3. **ResumeButton** (Button - TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 50)`
   - **Size**: `(300, 80)`
   - **Text**: `"RESUME"`
   - **Font Size**: `48`

4. **MainMenuButton** (Button - TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, -50)`
   - **Size**: `(300, 80)`
   - **Text**: `"MAIN MENU"`
   - **Font Size**: `48`

#### PauseMenuUI Component Configuration

- **UI References**:
  - `resumeButton`: Reference to ResumeButton
  - `mainMenuButton`: Reference to MainMenuButton
  - `pausePanel`: Reference to PausePanel GameObject
- **Game References**:
  - `gameManager`: Reference to GameManager component

---

### 4. GameOverMenu Panel

- **Name**: `GameOverMenu`
- **Active**: `false` (initially hidden)
- **Components**: `GameOverUI.cs`

#### GameOverMenu Layout

1. **GameOverPanel** (Panel)
   - Full-screen panel with semi-transparent background
   - **Name**: `GameOverPanel`

2. **GameOverText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 250)`
   - **Text**: `"GAME OVER"`
   - **Font Size**: `64`

3. **FinalScoreText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 150)`
   - **Text**: `"Final Score: 0"`
   - **Font Size**: `48`

4. **FinalDistanceText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 100)`
   - **Text**: `"Distance: 0m"`
   - **Font Size**: `36`

5. **FinalCoinsText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 50)`
   - **Text**: `"Coins: 0"`
   - **Font Size**: `36`

6. **FinalTrashText** (TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, 0)`
   - **Text**: `"Trash: 0"`
   - **Font Size**: `36`

7. **RetryButton** (Button - TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, -100)`
   - **Size**: `(300, 80)`
   - **Text**: `"RETRY"`
   - **Font Size**: `48`

8. **MainMenuButton** (Button - TextMeshProUGUI)
   - **Anchor**: Center
   - **Position**: `(0, -200)`
   - **Size**: `(300, 80)`
   - **Text**: `"MAIN MENU"`
   - **Font Size**: `48`

#### GameOverUI Component Configuration

- **UI References**:
  - `finalScoreText`: Reference to FinalScoreText
  - `finalDistanceText`: Reference to FinalDistanceText
  - `finalCoinsText`: Reference to FinalCoinsText
  - `finalTrashText`: Reference to FinalTrashText
  - `retryButton`: Reference to RetryButton
  - `mainMenuButton`: Reference to MainMenuButton
  - `gameOverPanel`: Reference to GameOverPanel GameObject
- **Game References**:
  - `gameManager`: Reference to GameManager component
  - `scoreManager`: Reference to ScoreManager component

---

## ScriptableObject Assets

Create the following ScriptableObject assets in the **Assets/Data/** folder:

### 1. DifficultyConfig Asset

- **Path**: `Assets/Data/DefaultDifficultyConfig.asset`
- **Type**: `DifficultyConfig`

#### Configuration

Create animation curves for difficulty progression:

1. **speedOverTime**
   - Start: `(0, 5)` - 5 units/sec at time 0
   - Mid: `(60, 10)` - 10 units/sec at 60 seconds
   - End: `(120, 15)` - 15 units/sec at 120 seconds

2. **obstacleDensityOverTime**
   - Start: `(0, 0.2)` - 20% density at time 0
   - Mid: `(60, 0.4)` - 40% density at 60 seconds
   - End: `(120, 0.6)` - 60% density at 120 seconds

3. **pickupDensityOverTime**
   - Start: `(0, 0.3)` - 30% density at time 0
   - Mid: `(60, 0.25)` - 25% density at 60 seconds
   - End: `(120, 0.2)` - 20% density at 120 seconds

### 2. SpawnConfig Asset

- **Path**: `Assets/Data/DefaultSpawnConfig.asset`
- **Type**: `SpawnConfig`

#### Configuration

1. **obstacleEntries**
   - Add entries for each obstacle prefab:
     - `prefab`: Reference to obstacle prefab
     - `probability`: Weight for random selection (e.g., 1.0)
     - `minDistance`: Minimum distance between spawns (e.g., 5.0)

2. **pickupEntries**
   - Add entries for pickup prefabs:
     - **Coin Entry**:
       - `prefab`: Reference to Pickup_Coin prefab
       - `probability`: `0.7` (70% of pickups are coins)
       - `minDistance`: `2.0`
     - **Trash Entry**:
       - `prefab`: Reference to Pickup_Trash prefab
       - `probability`: `0.3` (30% of pickups are trash)
       - `minDistance`: `3.0`

---

## Final Steps

1. **Assign all references** in the Inspector for each component
2. **Test the scene** by pressing Play in Unity
3. **Verify input** works correctly (keyboard and touch/swipe)
4. **Check collision detection** between player and obstacles/pickups
5. **Verify UI updates** correctly during gameplay
6. **Test game state transitions** (Menu → Running → Paused → GameOver)

---

## Troubleshooting

### Common Issues

1. **Player not moving forward**
   - Check that GameManager state is set to `Running`
   - Verify DifficultyController has valid DifficultyConfig assigned

2. **Input not working**
   - Ensure RunnerInput.inputactions is assigned to PlayerInputController
   - Check that Input System package is installed

3. **Collisions not detected**
   - Verify physics layers are set correctly
   - Check collision matrix configuration
   - Ensure colliders are set to `Is Trigger = true`

4. **UI not updating**
   - Check that ScoreManager events are being subscribed to
   - Verify all UI text references are assigned

5. **Objects not spawning**
   - Check that SpawnConfig has valid prefab references
   - Verify SegmentSpawner has player transform assigned
   - Ensure object pools are initialized correctly

---

## Additional Notes

- All scripts use the appropriate namespaces (`TrashRunner.Core`, `TrashRunner.Player`, `TrashRunner.Environment`, `TrashRunner.UI`, `TrashRunner.Data`)
- The game uses object pooling for efficient memory management
- Audio clips should be imported as AudioClip assets
- Consider using TextMeshPro for better text rendering quality
- Adjust difficulty curves based on playtesting feedback

---

**End of Setup Documentation**
