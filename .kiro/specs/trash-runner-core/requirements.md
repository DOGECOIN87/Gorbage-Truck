# Requirements Document

## Introduction

TrashRunner is a 3D endless runner Unity game featuring a garbage truck collecting trash on city streets. The game uses lane-based gameplay with three lanes, where players control a garbage truck that automatically moves forward while avoiding obstacles and collecting pickups. The game targets Unity 2022.3 LTS through Unity 6 LTS with URP and the New Input System.

## Glossary

- **TrashRunner_System**: The complete game application including all managers, controllers, and UI components
- **GameManager**: Core system component responsible for game state management and run lifecycle
- **ScoreManager**: System component tracking player statistics including score, distance, coins, trash, and lives
- **PlayerRunnerController**: Component controlling player truck movement, lane switching, jumping, and sliding
- **PlayerInputController**: Component handling keyboard and touch input, translating to game actions
- **SegmentSpawner**: Component managing infinite track generation through object pooling
- **TrackSegment**: Individual track piece with spawn points for obstacles and pickups
- **DifficultyController**: Component evaluating difficulty curves based on elapsed run time
- **Lane**: One of three horizontal positions (left=0, center=1, right=2) where the player can be positioned
- **CharacterController**: Unity component used for player movement (not Rigidbody)
- **Object Pool**: Pre-instantiated collection of reusable game objects to avoid runtime allocation

## Requirements

### Requirement 1: Game State Management

**User Story:** As a player, I want the game to manage different states (menu, running, paused, game over), so that I have a clear and responsive gameplay experience.

#### Acceptance Criteria

1. WHEN the player presses the Play button, THE GameManager SHALL transition from Menu state to Running state and invoke the OnRunStarted event.
2. WHEN the player loses all lives, THE GameManager SHALL transition to GameOver state and invoke the OnRunEnded event.
3. WHEN the player presses the Pause button during Running state, THE GameManager SHALL transition to Paused state, set Time.timeScale to 0, and invoke the OnPause event.
4. WHEN the player presses the Resume button during Paused state, THE GameManager SHALL transition to Running state, restore Time.timeScale to 1, and invoke the OnResume event.
5. WHEN StartRun is called, THE GameManager SHALL execute the 9-step reset sequence: reset player position, reset track, reset stats, reset difficulty timer, activate player, reset lane to center, return pooled objects, set Running state, and invoke OnRunStarted.

### Requirement 2: Player Movement and Controls

**User Story:** As a player, I want to control my garbage truck by switching lanes, jumping, and sliding, so that I can navigate obstacles and collect items.

#### Acceptance Criteria

1. WHILE the game is in Running state, THE PlayerRunnerController SHALL move the player forward along the +Z axis at the current speed provided by DifficultyController.
2. WHEN the OnMoveLeft event is received, THE PlayerRunnerController SHALL decrease currentLaneIndex by 1 (minimum 0) and smoothly lerp to the target X position.
3. WHEN the OnMoveRight event is received, THE PlayerRunnerController SHALL increase currentLaneIndex by 1 (maximum 2) and smoothly lerp to the target X position.
4. WHEN the OnJump event is received AND the player is grounded AND not sliding, THE PlayerRunnerController SHALL apply jumpForce upward velocity using CharacterController.Move().
5. WHEN the OnSlide event is received AND the player is grounded AND not jumping, THE PlayerRunnerController SHALL reduce CharacterController height to 50% of original and adjust center.y for slideDuration seconds.
6. THE PlayerRunnerController SHALL use CharacterController.Move() for all movement and manually apply gravity each frame.

### Requirement 3: Input Handling

**User Story:** As a player, I want to control the game using keyboard keys or touch swipes, so that I can play on both desktop and mobile devices.

#### Acceptance Criteria

1. WHEN the A key or Left Arrow is pressed, THE PlayerInputController SHALL invoke the OnMoveLeft event.
2. WHEN the D key or Right Arrow is pressed, THE PlayerInputController SHALL invoke the OnMoveRight event.
3. WHEN the Space key is pressed, THE PlayerInputController SHALL invoke the OnJump event.
4. WHEN the S key or Left Ctrl is pressed, THE PlayerInputController SHALL invoke the OnSlide event.
5. WHEN a touch swipe is detected with magnitude greater than minSwipeDistance pixels AND duration less than maxSwipeTime seconds, THE PlayerInputController SHALL determine direction and invoke the corresponding event (left/right swipe for lane change, up swipe for jump, down swipe for slide).
6. THE PlayerInputController SHALL reference InputActionAsset via SerializeField and retrieve actions by name from the Gameplay action map.

### Requirement 4: Score and Statistics Tracking

**User Story:** As a player, I want my score, distance, coins, trash collected, and lives to be tracked and displayed, so that I can measure my performance.

#### Acceptance Criteria

1. WHILE the game is in Running state, THE ScoreManager SHALL accumulate distance based on distanceDelta reported by PlayerRunnerController and invoke OnDistanceChanged when cumulative change exceeds 0.5 meters.
2. WHEN a coin pickup is collected, THE ScoreManager SHALL increment coin count, add scorePerCoin to score, and invoke OnCoinsChanged and OnScoreChanged events.
3. WHEN a trash pickup is collected, THE ScoreManager SHALL increment trash count, add scorePerTrash to score, and invoke OnTrashChanged and OnScoreChanged events.
4. WHEN the player hits an obstacle, THE ScoreManager SHALL decrement lives by 1 and invoke OnLivesChanged event.
5. WHEN the run ends AND current score exceeds best score, THE ScoreManager SHALL update best score and persist to PlayerPrefs using key "TrashRunner_BestScore".
6. WHEN ResetStats is called, THE ScoreManager SHALL set coins, trash, distance, and score to 0 and reset lives to initial value.

### Requirement 5: Infinite Track Generation

**User Story:** As a player, I want the track to generate infinitely ahead of me, so that I can play endless runs without interruption.

#### Acceptance Criteria

1. WHEN the game starts, THE SegmentSpawner SHALL pre-initialize object pools for segments, obstacles, and pickups in Awake or Start.
2. WHILE the game is in Running state, THE SegmentSpawner SHALL maintain segmentsAhead number of track segments spawned ahead of the player position.
3. WHEN the player passes a segment end trigger, THE SegmentSpawner SHALL recycle the oldest segment by deactivating child obstacles/pickups, returning them to pools, and returning the segment to the segment pool.
4. WHEN spawning a new segment, THE SegmentSpawner SHALL position it at lastSegment.position.z + lastSegment.Length using the TrackSegment.Length property.
5. WHEN populating a segment, THE SegmentSpawner SHALL query DifficultyController for current obstacle and pickup densities and use SpawnConfig to determine spawn probabilities and minimum distances.

### Requirement 6: Difficulty Scaling

**User Story:** As a player, I want the game difficulty to increase over time, so that the gameplay remains challenging and engaging.

#### Acceptance Criteria

1. WHILE the game is in Running state, THE DifficultyController SHALL increment elapsedRunTime each frame by Time.deltaTime.
2. WHEN queried for current speed, THE DifficultyController SHALL evaluate DifficultyConfig.speedOverTime AnimationCurve at elapsedRunTime and return the result.
3. WHEN queried for obstacle density, THE DifficultyController SHALL evaluate DifficultyConfig.obstacleDensityOverTime AnimationCurve at elapsedRunTime and return the result.
4. WHEN queried for pickup density, THE DifficultyController SHALL evaluate DifficultyConfig.pickupDensityOverTime AnimationCurve at elapsedRunTime and return the result.
5. WHEN ResetTime is called, THE DifficultyController SHALL set elapsedRunTime to 0.

### Requirement 7: Collision Detection

**User Story:** As a player, I want to collide with obstacles and pickups, so that the game responds to my navigation choices.

#### Acceptance Criteria

1. WHEN the player enters a trigger collider with an Obstacle component, THE PlayerRunnerController or PlayerCollisionHandler SHALL call obstacle.ApplyEffect(player, scoreManager).
2. WHEN the player enters a trigger collider with a Pickup component, THE PlayerRunnerController or PlayerCollisionHandler SHALL call pickup.ApplyEffect(player, scoreManager).
3. WHEN ApplyEffect is called on an Obstacle, THE Obstacle SHALL reduce player lives via ScoreManager, play hit sound, and deactivate itself for pool recycling.
4. WHEN ApplyEffect is called on a Pickup, THE Pickup SHALL increment appropriate counter via ScoreManager based on PickupType, play collect sound, and deactivate itself for pool recycling.
5. THE Obstacle and Pickup prefabs SHALL have trigger colliders (isTrigger=true) and kinematic Rigidbody (isKinematic=true, useGravity=false).

### Requirement 8: Camera Following

**User Story:** As a player, I want the camera to follow my truck smoothly, so that I have a clear view of the gameplay.

#### Acceptance Criteria

1. WHILE the game is in Running state, THE RunnerCameraController SHALL position the camera at playerTransform.position plus configurable offset Vector3.
2. THE RunnerCameraController SHALL use Lerp or SmoothDamp in LateUpdate to smoothly follow the player position.
3. WHERE speed-based FOV adjustment is enabled, THE RunnerCameraController SHALL interpolate camera.fieldOfView between baseFOV and maxFOV based on currentSpeed/maxSpeed ratio.

### Requirement 9: User Interface

**User Story:** As a player, I want clear UI screens for the main menu, HUD, pause menu, and game over, so that I can navigate the game and see my stats.

#### Acceptance Criteria

1. WHILE the game is in Menu state, THE MainMenuUI SHALL be active and display the Play button and best score.
2. WHILE the game is in Running state, THE HUDController SHALL display current distance, score, coins, trash, and lives by subscribing to ScoreManager events in OnEnable and unsubscribing in OnDisable.
3. WHILE the game is in Paused state, THE PauseMenuUI SHALL be active and display Resume and Main Menu buttons.
4. WHEN the game transitions to GameOver state, THE GameOverUI SHALL activate and display final score, distance, coins, and trash with Retry and Main Menu buttons.
5. THE UI scripts SHALL subscribe to events in OnEnable and unsubscribe in OnDisable (not Start or Awake).

### Requirement 10: Audio Management

**User Story:** As a player, I want sound effects and music, so that the game provides audio feedback and atmosphere.

#### Acceptance Criteria

1. THE AudioManager SHALL maintain two AudioSource components: one for looping background music and one for SFX using PlayOneShot.
2. WHEN PlayJump, PlaySlide, PlayPickupCoin, PlayPickupTrash, PlayHitObstacle, or PlayButtonClick is called, THE AudioManager SHALL play the corresponding AudioClip via the SFX AudioSource.
3. WHERE muteSFX is true, THE AudioManager SHALL not play sound effects.
4. WHERE muteMusic is true, THE AudioManager SHALL not play background music.

### Requirement 11: ScriptableObject Configuration

**User Story:** As a developer, I want game parameters configured via ScriptableObjects, so that I can tune gameplay without code changes.

#### Acceptance Criteria

1. THE DifficultyConfig ScriptableObject SHALL contain AnimationCurve fields for speedOverTime, obstacleDensityOverTime, and pickupDensityOverTime.
2. THE SpawnConfig ScriptableObject SHALL contain lists of ObstacleSpawnEntry and PickupSpawnEntry with prefab, spawnProbability, and minDistanceBetweenSpawns fields.
3. THE ScriptableObjects SHALL use CreateAssetMenu attribute and be assigned via Inspector (not loaded via Resources.Load at runtime).
