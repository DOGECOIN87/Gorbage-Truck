# Adding Your Garbage Truck 3D Model to Trash Runner

This guide provides step-by-step instructions for importing your garbage truck 3D model and setting it up as the player character in the Trash Runner game.

---

## Prerequisites

Before you begin, ensure you have:

- Your garbage truck 3D model file (supported formats: `.fbx`, `.obj`, `.blend`, `.dae`, `.3ds`, `.dxf`, `.max`)
- Unity project opened with the Trash Runner scene
- All scripts from the implementation already in place

---

## Step 1: Import the 3D Model into Unity

### 1.1 Create Models Folder

In the Unity Project window, create a folder structure for your model:

1. Navigate to **Assets** folder
2. Right-click and select **Create > Folder**
3. Name it `Models`
4. Inside `Models`, create another folder called `GarbageTruck`

Your structure should look like:
```
Assets/
└── Models/
    └── GarbageTruck/
```

### 1.2 Import the Model File

1. Locate your garbage truck model file on your computer
2. Drag and drop the file into the `Assets/Models/GarbageTruck/` folder in Unity
3. Wait for Unity to finish importing (you'll see a progress bar)

**Alternative method:**
- Right-click in the `GarbageTruck` folder
- Select **Import New Asset...**
- Browse to your model file and click **Import**

### 1.3 Configure Import Settings

1. Click on the imported model file in the Project window
2. In the Inspector, you'll see the **Import Settings**
3. Configure the following settings:

#### Model Tab
- **Scale Factor**: Adjust if your model is too large/small (try `1` first, then adjust as needed)
- **Mesh Compression**: `Off` (for better quality)
- **Read/Write Enabled**: `Checked` (allows CharacterController to work properly)
- **Optimize Mesh**: `Checked`
- **Generate Colliders**: `Unchecked` (we'll add custom colliders)

#### Rig Tab
- **Animation Type**: `None` (unless your model has animations)

#### Materials Tab
- **Material Creation Mode**: `Standard (Legacy)`
- **Location**: `Use Embedded Materials`

4. Click **Apply** at the bottom of the Inspector

---

## Step 2: Create the TruckPlayer Prefab

### 2.1 Add Model to Scene

1. Drag your imported model from the Project window into the **Hierarchy** window
2. Rename it to `TruckPlayer`
3. Set its position to `(0, 0, 0)` in the Inspector

### 2.2 Adjust Model Orientation and Scale

Your model might need rotation or scaling adjustments:

1. Select `TruckPlayer` in the Hierarchy
2. In the Inspector, adjust the **Transform** component:
   - **Position**: `(0, 0, 0)`
   - **Rotation**: `(0, 0, 0)` or `(0, 180, 0)` if facing backward
   - **Scale**: Start with `(1, 1, 1)` and adjust if too large/small

**Typical adjustments:**
- If the truck is too large: Try scale `(0.5, 0.5, 0.5)` or `(0.3, 0.3, 0.3)`
- If the truck is too small: Try scale `(2, 2, 2)` or `(3, 3, 3)`
- If facing wrong direction: Rotate Y-axis by `180` degrees

### 2.3 Organize Model Hierarchy

Your garbage truck model might have multiple child objects (wheels, body, etc.). Organize them properly:

1. Select `TruckPlayer` in the Hierarchy
2. Expand it to see all child objects
3. Your hierarchy should look something like:

```
TruckPlayer
├── Body (mesh)
├── Wheels (mesh)
├── Windows (mesh)
└── Details (mesh)
```

**Important:** Keep all visual meshes as children of the main `TruckPlayer` GameObject.

---

## Step 3: Set Up the Player Layer

1. Select the `TruckPlayer` GameObject in the Hierarchy
2. In the Inspector, at the top, find the **Layer** dropdown
3. Select **Player** (Layer 6)
4. When prompted "Do you want to set layer to Player for all child objects as well?", click **Yes, change children**

---

## Step 4: Add CharacterController Component

The CharacterController handles the player's physics and movement.

1. With `TruckPlayer` selected, click **Add Component** in the Inspector
2. Search for and add **Character Controller**
3. Configure the CharacterController settings:

### CharacterController Settings

Adjust these based on your truck's size:

- **Height**: `2` (adjust to match truck height)
- **Radius**: `0.8` (adjust to match truck width)
- **Center**: 
  - **X**: `0`
  - **Y**: `1` (half of the height, so the controller sits on the ground)
  - **Z**: `0`
- **Slope Limit**: `45`
- **Step Offset**: `0.3`
- **Skin Width**: `0.08`
- **Min Move Distance**: `0.001`

**Visual Check:** In the Scene view, you should see a green capsule outline around your truck. Adjust Height and Radius until it fits well.

---

## Step 5: Add Player Scripts

Now we'll add all the custom scripts that control the player.

### 5.1 Add PlayerRunnerController

1. With `TruckPlayer` selected, click **Add Component**
2. Search for **PlayerRunnerController** and add it
3. Configure the settings:

#### Movement Settings
- **Base Forward Speed**: `5`
- **Lane Width**: `3`
- **Lane Change Speed**: `10`

#### Jump & Gravity
- **Jump Force**: `10`
- **Gravity**: `-20`

#### Slide Settings
- **Slide Duration**: `1`

#### References (drag and drop from Hierarchy/Project)
- **Character Controller**: Drag the CharacterController component (same GameObject)
- **Score Manager**: Drag the GameManager GameObject, then select ScoreManager component
- **Game Manager**: Drag the GameManager GameObject, select GameManager component
- **Difficulty Controller**: Drag the GameManager GameObject, select DifficultyController component
- **Player Input Controller**: Leave empty for now (we'll add it next)
- **Audio Manager**: Drag the GameManager GameObject, select AudioManager component

### 5.2 Add PlayerInputController

1. With `TruckPlayer` selected, click **Add Component**
2. Search for **PlayerInputController** and add it
3. Configure the settings:

#### Input Settings
- **Runner Input Asset**: Drag the `RunnerInput.inputactions` file from `Assets/Input/` folder

#### Swipe Settings
- **Min Swipe Distance**: `50`
- **Max Swipe Time**: `1`
- **Direction Threshold**: `0.5`

4. **Go back to PlayerRunnerController** and drag the PlayerInputController component into the **Player Input Controller** field

### 5.3 Add PlayerCollisionHandler

1. With `TruckPlayer` selected, click **Add Component**
2. Search for **PlayerCollisionHandler** and add it
3. Configure the references:

#### References
- **Player Runner Controller**: Drag the PlayerRunnerController component (same GameObject)
- **Score Manager**: Drag the GameManager GameObject, select ScoreManager component

---

## Step 6: Add Collision Trigger

The CharacterController doesn't trigger collision events by itself, so we need a separate trigger collider.

### 6.1 Create CollisionTrigger Child Object

1. Right-click on `TruckPlayer` in the Hierarchy
2. Select **Create Empty**
3. Rename it to `CollisionTrigger`
4. Set its **Transform**:
   - **Position**: `(0, 1, 0)` (relative to parent)
   - **Rotation**: `(0, 0, 0)`
   - **Scale**: `(1, 1, 1)`

### 6.2 Add Box Collider

1. Select `CollisionTrigger` in the Hierarchy
2. Click **Add Component**
3. Add **Box Collider**
4. Configure the Box Collider:
   - **Is Trigger**: `✓ Checked` (very important!)
   - **Center**: `(0, 0, 0)`
   - **Size**: Adjust to match your truck size, for example:
     - **X**: `1.5` (width)
     - **Y**: `2` (height)
     - **Z**: `2` (length)

**Visual Check:** In the Scene view, you should see a green wireframe box around your truck. Adjust the Size values until it covers the truck properly.

### 6.3 Set Layer

1. With `CollisionTrigger` selected, set its **Layer** to **Player**

---

## Step 7: Position the Truck Correctly

### 7.1 Adjust Vertical Position

The truck should sit on the ground properly:

1. Select `TruckPlayer` in the Hierarchy
2. In Scene view, look at the truck from the side
3. Adjust the **Y position** so the wheels/bottom touch the ground
4. Typical values: `Y = 0`, `Y = 0.5`, or `Y = 1` (depends on your model's pivot point)

### 7.2 Test Ground Contact

1. Make sure the CharacterController's bottom capsule touches the ground
2. The green capsule outline should just touch the ground plane

---

## Step 8: Create the Prefab

Now that everything is set up, save it as a prefab for easy reuse.

1. Create a **Prefabs** folder in Assets if it doesn't exist
2. Drag the `TruckPlayer` GameObject from the Hierarchy into the `Assets/Prefabs/` folder
3. You should see a blue cube icon appear, indicating it's now a prefab
4. You can now delete the `TruckPlayer` from the Hierarchy (we'll add it back properly in the next step)

---

## Step 9: Add TruckPlayer to the Scene

### 9.1 Place in Scene

1. Drag the `TruckPlayer` prefab from `Assets/Prefabs/` into the Hierarchy
2. Set its position to the starting position:
   - **Position**: `(0, 0, 0)` or `(0, 0.5, 0)`
   - **Rotation**: `(0, 0, 0)`
   - **Scale**: `(1, 1, 1)` (or whatever scale you determined earlier)

### 9.2 Connect to GameManager

1. Select the **GameManager** GameObject in the Hierarchy
2. Find the **GameManager** component in the Inspector
3. Drag the `TruckPlayer` GameObject into the **Player Runner Controller** field

### 9.3 Connect to SegmentSpawner

1. Select the **SegmentSpawner** GameObject in the Hierarchy
2. Find the **SegmentSpawner** component in the Inspector
3. Drag the `TruckPlayer` GameObject into the **Player Transform** field

### 9.4 Connect to Camera

1. Select the **Main Camera** GameObject in the Hierarchy
2. Find the **RunnerCameraController** component in the Inspector
3. Drag the `TruckPlayer` GameObject into the **Player Transform** field

---

## Step 10: Test the Setup

### 10.1 Enter Play Mode

1. Click the **Play** button at the top of Unity
2. The game should start in the main menu
3. Click the **PLAY** button

### 10.2 Test Controls

- **A** or **Left Arrow**: Move left
- **D** or **Right Arrow**: Move right
- **Space**: Jump
- **S** or **Left Ctrl**: Slide

### 10.3 Check for Issues

**If the truck doesn't move forward:**
- Check that GameManager is in "Running" state
- Verify DifficultyController has a DifficultyConfig assigned

**If the truck is floating or sinking:**
- Adjust the Y position of TruckPlayer
- Check CharacterController height and center values

**If controls don't work:**
- Verify RunnerInput.inputactions is assigned to PlayerInputController
- Check that all script references are properly set

**If the truck is too big/small:**
- Adjust the Scale in the Transform component
- Remember to apply changes to the prefab

---

## Step 11: Fine-Tune Visual Appearance

### 11.1 Adjust Materials

If your truck's materials look wrong:

1. Expand the `TruckPlayer` in the Hierarchy
2. Select child objects with mesh renderers
3. In the Inspector, find the **Materials** section
4. Adjust or replace materials as needed

### 11.2 Add Visual Effects (Optional)

You can enhance the truck's appearance:

- **Particle effects** for exhaust smoke
- **Trail renderer** for tire marks
- **Lights** for headlights/taillights

### 11.3 Optimize for Performance

If the model has too many polygons:

1. Select the model file in the Project window
2. In Import Settings > Model tab
3. Increase **Mesh Compression** to `Low` or `Medium`
4. Click **Apply**

---

## Step 12: Save Everything

1. **Save the Scene**: `File > Save` or `Ctrl+S`
2. **Save the Prefab**: Click **Apply** on the TruckPlayer prefab in the Inspector if you made any changes
3. **Save the Project**: `File > Save Project`

---

## Common Issues and Solutions

### Issue: Truck is sideways or upside down

**Solution:** 
- Adjust the Rotation in Transform (try `Y = 90`, `Y = 180`, or `Y = 270`)
- Or create an empty parent GameObject, rotate the model inside it

### Issue: Truck is way too big or too small

**Solution:**
- Adjust the Scale in Transform
- Or re-import the model with different Scale Factor in Import Settings

### Issue: Collisions not working

**Solution:**
- Ensure CollisionTrigger has "Is Trigger" checked
- Verify Layer is set to "Player"
- Check that Box Collider size covers the truck

### Issue: Truck falls through the ground

**Solution:**
- Increase the Y position
- Check that CharacterController is properly configured
- Ensure track segments have colliders

### Issue: Materials are pink/missing

**Solution:**
- Check that textures were imported with the model
- In model Import Settings, try different Material Creation Mode
- Manually assign materials to mesh renderers

---

## Advanced Customization

### Adding Animations

If your garbage truck model has animations (wheels spinning, etc.):

1. In model Import Settings > Rig tab, set **Animation Type** to **Generic**
2. Add an **Animator** component to TruckPlayer
3. Create an Animator Controller and assign animations
4. Control animations from PlayerRunnerController script

### Adding Sound Effects

The truck can have engine sounds:

1. Add an **Audio Source** component to TruckPlayer
2. Assign an engine sound AudioClip
3. Set it to **Loop** and **Play On Awake**
4. Adjust **Volume** based on speed in PlayerRunnerController

### Custom Collision Shapes

For more precise collisions:

1. Instead of Box Collider, use **Mesh Collider** on CollisionTrigger
2. Set **Convex** to `true`
3. Set **Is Trigger** to `true`
4. Assign the truck's mesh

---

## Next Steps

After successfully adding your garbage truck model:

1. **Create obstacle prefabs** following the same import process
2. **Create pickup prefabs** (coins, trash items)
3. **Create track segment prefabs** with proper spawn points
4. **Set up the complete scene** as described in UNITY_SCENE_SETUP.md

---

## Need Help?

If you encounter issues not covered in this guide:

1. Check the Unity Console for error messages (Window > General > Console)
2. Verify all script references are assigned (no "None" or "Missing" references)
3. Review the UNITY_SCENE_SETUP.md for complete scene configuration
4. Test with a simple cube first to verify the scripts work correctly

---

**Congratulations!** Your garbage truck is now ready to run through the endless runner game!
