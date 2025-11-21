# MxM Third Person Controller (Starter Assets Bridge)

A custom character controller script that seamlessly bridges **Unity's Starter Assets (New Input System)** with the **Motion Matching (MxM)** animation system. 

This controller replaces the default logic to allow MxM to handle animation, trajectory, and root motion, while retaining the robust camera and input handling from Unity's Starter Assets.

## Features

*   **New Input System Integration:** Uses `StarterAssetsInputs` to drive Motion Matching.
*   **All-in-One Logic:** Handles Movement, Camera Rotation, Actions, and Audio in a single script.
*   **Procedural Footstep System:** Distance-based footstep sounds (no Animation Events required).
*   **Smart Landing Logic:** Detects landing impacts with "debounce" timers to prevent physics jitter/double sounds.
*   **Camera Pitch Fix:** Custom trajectory calculation ensures the character moves correctly even when the camera is looking down (fixes the common MxM "W/S stuck" bug).
*   **Action Support:** Built-in logic for Sprinting, Strafing, Jumping, Sliding, and Dancing.

## Prerequisites

To use this script, you must have the following installed in your Unity project:

1.  **[Motion Matching for Unity (MxM)](https://assetstore.unity.com/packages/tools/animation/motion-matching-for-unity-145624)** - (Free)
2.  **[Starter Assets - Third Person](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526)** - (Free)
3. Download the package and import it into your project.

## Controller Setup
In the `MxMThirdPersonController` inspector:

*   **Cinemachine Camera Target:** Drag your camera target object here.
*   **MxM Profiles:** Assign your General, Strafe, and Sprint profiles.
*   **MxM Events:** Assign your Jump, Slide, and Dance event definitions.
*   **Input Actions:** Drag your specific `InputActionReference` assets for Strafe, Slide, and Dance.

## Audio Setup

This controller uses a **Procedural Audio System**, meaning you do **not** need to add Animation Events to your clips.

1.  **Footstep Audio Clips:** Drag your audio files into the array.
2.  **Landing Audio Clip:** Assign a landing sound.
3.  **Footstep Stride:** Adjust this value (Default: `1.2`) to match your animation's stride length.
    *   *If footsteps play too fast, increase this number.*
    *   *If footsteps play too slow, decrease this number.*

## 🕹️ Controls

| Action | Input (Default) |
| :--- | :--- |
| **Move** | WASD / Left Stick |
| **Look** | Mouse / Right Stick |
| **Sprint** | Left Shift / Left Stick Press |
| **Jump** | Space / South Button |
| **Strafe** | Tab / Right Trigger (Requires binding) |
| **Slide** | X / East Button (Requires binding) |

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

*Note: This script relies on the Starter Assets and MxM packages, which are subject to their own licenses.*