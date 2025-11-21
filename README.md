# MxM Motion Matching x Third Person Controller (Starter Assets Bridge)

Motion Matching (MxM) has long been one of the most powerful animation tools for Unity. However, with the asset effectively unsupported recently, getting it to run smoothly with modern standards has become a challenge.

Many of us have hit walls trying to get it working with the New Input System or the latest Cinemachine updates.

To help the community keep utilizing this incredible tech, I’ve built an open-source bridge that integrates MxM directly with Unity’s standard Starter Assets Controller.

It solves the import errors and modernizes the workflow so you can focus on gameplay, not debugging.

Key features:
✅ Tested using Latest UNITY 6.2 LTS
✅ Full support for Unity’s New Input System
✅ Updated compatibility for Cinemachine 3.x
✅ Includes a working Demo Scene to help you learn the setup

If you are looking to study MxM Motion Matching or implement it into your current project, I hope this saves you some time!

https://github.com/user-attachments/assets/500b92cc-6f34-4f51-b58b-601321ccae0c

## Compatibility (Package Export using)

*   Unity Version: 6.2 (6000.2.12f1) or above
*   Render Pipeline: URP
*   Chinemachine: 3.x

## Prerequisites

To use this script, you must have the following installed and run in your Unity project:

1.  **[Motion Matching for Unity (MxM)](https://assetstore.unity.com/packages/tools/animation/motion-matching-for-unity-145624)** - (Free) - **[KNOWN ISSUE](https://github.com/ru4ls/UNITY-MxM-Motion-Matching-for-Starter-Assets?tab=readme-ov-file#known-issues--troubleshooting)**
2.  **[Starter Assets - Third Person](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526)** - (Free)
3.  **Cinamechine 3.x** Ensure you using the latest Cinemachine version.

## Download & Import

Download and Import MxM_Motion_Matching-StarterAssets_v1.0.0.unitypackage into your project.

[Download the latest .unitypackage here](https://github.com/ru4ls/UNITY-MxM-Motion-Matching-for-Starter-Assets/releases/latest)

<img width="191" height="81" alt="Screenshot 2025-11-21 151125" src="https://github.com/user-attachments/assets/b2a20676-47c2-4569-88b4-a17ad3c0d497" />

## Play Demo

Open Scene/Demo-Playground and hot play button.

## Controller Pre-Setup
In the `MxMThirdPersonController` inspector:

*   **Cinemachine Camera Target:** Drag your camera target object here.
*   **MxM Profiles:** Assign your General, Strafe, and Sprint profiles.
*   **MxM Events:** Assign your Jump, Slide, and Dance event definitions.
*   **Input Actions:** Drag your specific `InputActionReference` assets for Strafe, Slide, and Dance.
*   
<img width="372" height="754" alt="Screenshot 2025-11-21 154410" src="https://github.com/user-attachments/assets/58b318ef-5d02-4d25-a5e1-dad7b9626c3b" />

## Audio Pre-Setup

This controller uses a **Procedural Audio System**, meaning you do **not** need to add Animation Events to your clips.

1.  **Footstep Audio Clips:** Drag your audio files into the array.
2.  **Landing Audio Clip:** Assign a landing sound.
3.  **Footstep Stride:** Adjust this value (Default: `1.2`) to match your animation's stride length.
    *   *If footsteps play too fast, increase this number.*
    *   *If footsteps play too slow, decrease this number.*

## 🕹️ Controls Pre-Setup

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
