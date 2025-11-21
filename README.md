# MxM Motion Matching x Third Person Controller (Starter Assets Bridge)

A custom character controller script that seamlessly bridges **Unity's Starter Assets (New Input System)** with the **Motion Matching (MxM)** animation system. 

This controller replaces the default logic to allow MxM to handle animation, trajectory, and root motion, while retaining the robust camera and input handling from Unity's Starter Assets.

## Prerequisites

To use this script, you must have the following installed in your Unity project:

1.  **[Motion Matching for Unity (MxM)](https://assetstore.unity.com/packages/tools/animation/motion-matching-for-unity-145624)** - (Free)
2.  **[Starter Assets - Third Person](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526)** - (Free)

## Download

Download and Import MxM_Motion_Matching-StarterAssets_v1.0.0.unitypackage into your project.

[Download the latest .unitypackage here](https://github.com/ru4ls/UNITY-MxM-Motion-Matching-for-Starter-Assets/releases/latest)

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

*Note: This script relies on the Unity Starter Assets and MxM Motion Matching packages, which are subject to their own licenses.*