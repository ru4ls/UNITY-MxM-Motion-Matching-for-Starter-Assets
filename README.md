# MxM Motion Matching x Third Person Controller (Starter Assets Bridge)

A custom character controller script that seamlessly bridges **Unity's Starter Assets (New Input System)** with the **Motion Matching (MxM)** animation system. 

This controller replaces the default logic to allow MxM to handle animation, trajectory, and root motion, while retaining the robust camera and input handling from Unity's Starter Assets.

https://github.com/user-attachments/assets/500b92cc-6f34-4f51-b58b-601321ccae0c

## Compatibility (Package Export using)

*   Unity Version: 6.2 (6000.2.12f1) or above
*   Render Pipeline: URP | HDRP
*   Chinemachine: 3.x

## Prerequisites

To use this script, you must have the following installed and run in your Unity project:

1.  **[Motion Matching for Unity (MxM)](https://assetstore.unity.com/packages/tools/animation/motion-matching-for-unity-145624)** - (Free)
2.  **[Starter Assets - Third Person](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526)** - (Free)

## Download

Download and Import MxM_Motion_Matching-StarterAssets_v1.0.0.unitypackage into your project.

[Download the latest .unitypackage here](https://github.com/ru4ls/UNITY-MxM-Motion-Matching-for-Starter-Assets/releases/latest)

<img width="191" height="81" alt="Screenshot 2025-11-21 151125" src="https://github.com/user-attachments/assets/b2a20676-47c2-4569-88b4-a17ad3c0d497" />

## Controller Setup
In the `MxMThirdPersonController` inspector:

*   **Cinemachine Camera Target:** Drag your camera target object here.
*   **MxM Profiles:** Assign your General, Strafe, and Sprint profiles.
*   **MxM Events:** Assign your Jump, Slide, and Dance event definitions.
*   **Input Actions:** Drag your specific `InputActionReference` assets for Strafe, Slide, and Dance.
*   
<img width="372" height="754" alt="Screenshot 2025-11-21 154410" src="https://github.com/user-attachments/assets/58b318ef-5d02-4d25-a5e1-dad7b9626c3b" />

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

<img width="1175" height="693" alt="Screenshot 2025-11-21 154517" src="https://github.com/user-attachments/assets/88c75aad-2031-4f75-91a4-3b63ac03a8d2" />

## Known Issues & Troubleshooting

### 1. Compilation Error: `HelpUIControl.cs`
When installing MxM with newer versions of Unity/Cinemachine, you may encounter a `missing namespace Cinemachine` error in the `HelpUIControl.cs` script.

**Fix:**
Open `HelpUIControl.cs` and update the following lines to match the new Cinemachine namespace:

*   **Line 5:** Change the using directive.
    ```csharp
    // Change from:
    using Cinemachine;
    
    // To:
    using Unity.Cinemachine;
    ```

*   **Line 21:** Update the camera variable type.
    ```csharp
    // Change from:
    private CinemachineFreeLook m_freeLookCamera = null;

    // To:
    private CinemachineCamera m_freeLookCamera = null;
    ```

### 2. Standard MxM Demo Scenes Not Working
If you try to play the original demo scenes included with the MxM package, the character may not move. This is because the original demos rely on the **Old Input Manager**, while this project is configured for the **New Input System**.

**Fix:**
*   **Option A (Recommended):** Ignore the default MxM demos. This Bridge Asset replaces that logic entirely using the New Input System.
*   **Option B:** If you specifically need to test the original demos, go to **Edit → Project Settings → Player**, find **Active Input Handling**, and change it to **Both**.

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

*Note: This script relies on the Unity Starter Assets and MxM Motion Matching packages, which are subject to their own licenses.*
