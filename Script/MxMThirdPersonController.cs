// Copyright © 2025 - Adapted for Motion Matching (MxM) with Starter Assets with New Input System
// @ru4ls

using UnityEngine;
using MxM;
using MxMGameplay;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    /// <summary>
    /// A bridge controller that integrates Unity's Starter Assets Input System 
    /// with the Motion Matching (MxM) Trajectory Generator.
    /// Handles Movement, Camera Rotation, Procedural Footsteps, and MxM Events.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class MxMThirdPersonController : MonoBehaviour
    {
        #region Inspector Settings

        [Header("Movement Settings")]
        [Tooltip("Walking speed (Should match your MxM Calibration, e.g. 4.3)")]
        public float MoveSpeed = 4.3f;

        [Tooltip("Sprinting speed (Should match your MxM Calibration, e.g. 6.7)")]
        public float SprintSpeed = 6.7f;

        [Header("Audio - Procedural Footsteps")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Range(0, 1)]
        public float FootstepAudioVolume = 1.0f;

        [Tooltip("How far (in meters) to walk before playing a footstep sound.")]
        public float FootstepStride = 1.2f;

        [Tooltip("Cooldown time in seconds to prevent double landing sounds due to physics jitter.")]
        public float LandingSoundCooldown = 0.5f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow.")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        [Header("MxM Configuration")]
        [Tooltip("Smoothing value for input vectors to prevent sudden snapping.")]
        public float InputSmoothTime = 0.12f;

        [Header("MxM Events")]
        public MxMEventDefinition JumpDefinition;
        public MxMEventDefinition SlideDefinition;
        public MxMEventDefinition DanceDefinition;

        [Header("MxM Profiles")]
        public MxMInputProfile GeneralLocomotion;
        public MxMInputProfile StrafeLocomotion;
        public MxMInputProfile SprintLocomotion;

        [Header("MxM Warp Modules")]
        public WarpModule NormalWarpModule;
        public WarpModule StrafeWarpModule;

        [Header("Extra Inputs")]
        public InputActionReference StrafeInput;
        public InputActionReference SlideInput;
        public InputActionReference DanceInput;

        #endregion

        #region Private Variables

        // --- Cinemachine ---
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;

        // --- MxM Components ---
        private MxMAnimator _mxmAnimator;
        private MxMTrajectoryGenerator _trajectoryGenerator;
        private MxMRootMotionApplicator _rootMotionApplicator;
        private LocomotionSpeedRamp _speedRamp;
        private GenericControllerWrapper _controllerWrapper;

        // --- Standard Components ---
        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;
        private GameObject _mainCamera;

        // --- Logic State ---
        private Vector2 _currentSmoothedInput;
        private Vector2 _smoothInputVelocity;
        private bool _wasSprinting;
        private bool _isDancing;

        // --- Grounding & Audio State ---
        private bool _isGrounded;
        private bool _wasGroundedLastFrame;
        private bool _isStrafing;
        private float _footstepDistanceCounter;
        private Vector3 _lastWorldPosition;
        private float _landingBlockTimer;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void OnEnable()
        {
            if (StrafeInput) StrafeInput.action.Enable();
            if (SlideInput) SlideInput.action.Enable();
            if (DanceInput) DanceInput.action.Enable();
        }

        private void OnDisable()
        {
            if (StrafeInput) StrafeInput.action.Disable();
            if (SlideInput) SlideInput.action.Disable();
            if (DanceInput) DanceInput.action.Disable();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();

            // Get MxM References
            _mxmAnimator = GetComponentInChildren<MxMAnimator>();
            _trajectoryGenerator = GetComponentInChildren<MxMTrajectoryGenerator>();
            _rootMotionApplicator = GetComponent<MxMRootMotionApplicator>();
            _speedRamp = GetComponent<LocomotionSpeedRamp>();
            _controllerWrapper = GetComponent<GenericControllerWrapper>();

            // Initialize Controller Wrapper (Handles CharacterController)
            if (_controllerWrapper != null)
                _controllerWrapper.Initialize();

            // Setup Trajectory Generator for manual input override
            if (_trajectoryGenerator != null)
            {
                _trajectoryGenerator.InputProfile = GeneralLocomotion;
                // Important: Set Camera to null so we can manually calculate flattening (fixes looking down issue)
                _trajectoryGenerator.RelativeCameraTransform = null;
                _trajectoryGenerator.ControlMode = ETrajectoryControlMode.UserInput;
            }

            _lastWorldPosition = transform.position;
            _wasGroundedLastFrame = true; // Default true to prevent landing sound on game start
        }

        private void Update()
        {
            // 1. Update MxM Helper Utilities
            if (_speedRamp != null)
                _speedRamp.UpdateSpeedRamp();

            // 2. Cache Ground Status from Wrapper
            if (_controllerWrapper != null)
                _isGrounded = _controllerWrapper.IsGrounded;

            // 3. Update Timers
            if (_landingBlockTimer > 0f)
                _landingBlockTimer -= Time.deltaTime;

            // 4. Core Logic Loop
            HandleLanding();
            HandleMovement();
            HandleActions();
            HandleFootsteps();

            // 5. Cache State for next frame
            _wasGroundedLastFrame = _isGrounded;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        #endregion

        #region Movement Logic

        private void HandleMovement()
        {
            if (_trajectoryGenerator == null) return;

            Vector2 rawInput = _input.move;

            // --- Sprint State Machine ---
            if (_input.sprint && !_wasSprinting)
            {
                _speedRamp.BeginSprint();
                _trajectoryGenerator.MaxSpeed = SprintSpeed;
                _trajectoryGenerator.PositionBias = 6f;
                _trajectoryGenerator.DirectionBias = 6f;
                _mxmAnimator.SetCalibrationData("Sprint");
                _trajectoryGenerator.InputProfile = SprintLocomotion;
                _wasSprinting = true;
            }
            else if (!_input.sprint && _wasSprinting)
            {
                _speedRamp.ResetFromSprint();
                _trajectoryGenerator.MaxSpeed = MoveSpeed;
                _trajectoryGenerator.PositionBias = 10f;
                _trajectoryGenerator.DirectionBias = 10f;
                _mxmAnimator.SetCalibrationData("General");
                _trajectoryGenerator.InputProfile = GeneralLocomotion;
                _wasSprinting = false;
            }

            // --- Strafe State Machine ---
            bool isStrafeHeld = StrafeInput != null && StrafeInput.action.IsPressed();

            if (isStrafeHeld && !_isStrafing)
            {
                // Enter Strafe
                _mxmAnimator.AddRequiredTag("Strafe");
                _mxmAnimator.SetCalibrationData("Strafe");
                _mxmAnimator.SetFavourCurrentPose(true, 0.95f);
                _speedRamp.ResetFromSprint();
                _mxmAnimator.SetWarpOverride(StrafeWarpModule);

                // Configure Warping for Strafe
                _mxmAnimator.AngularErrorWarpRate = 360f;
                _mxmAnimator.AngularErrorWarpThreshold = 270f;
                _mxmAnimator.AngularErrorWarpMethod = EAngularErrorWarpMethod.TrajectoryFacing;

                _trajectoryGenerator.TrajectoryMode = ETrajectoryMoveMode.Strafe;
                _trajectoryGenerator.InputProfile = StrafeLocomotion;
                _mxmAnimator.PastTrajectoryMode = EPastTrajectoryMode.CopyFromCurrentPose;
                _isStrafing = true;
            }
            else if (!isStrafeHeld && _isStrafing)
            {
                // Exit Strafe
                _mxmAnimator.RemoveRequiredTag("Strafe");
                _mxmAnimator.SetFavourCurrentPose(false, 1.0f);
                _mxmAnimator.SetCalibrationData(0);
                _mxmAnimator.SetWarpOverride(NormalWarpModule);

                // Reset Warping
                _mxmAnimator.AngularErrorWarpRate = 60.0f;
                _mxmAnimator.AngularErrorWarpThreshold = 90f;
                _mxmAnimator.AngularErrorWarpMethod = EAngularErrorWarpMethod.CurrentHeading;

                _trajectoryGenerator.TrajectoryMode = ETrajectoryMoveMode.Normal;
                _trajectoryGenerator.InputProfile = GeneralLocomotion;
                _mxmAnimator.PastTrajectoryMode = EPastTrajectoryMode.ActualHistory;
                _isStrafing = false;
            }

            // --- Input Calculation ---
            // 1. Smooth the raw input
            _currentSmoothedInput = Vector2.SmoothDamp(
                _currentSmoothedInput,
                rawInput,
                ref _smoothInputVelocity,
                InputSmoothTime
            );

            // 2. Flatten Camera Rotation (Ignore Pitch so looking down doesn't stop movement)
            Vector3 camFwd = _mainCamera.transform.forward;
            Vector3 camRight = _mainCamera.transform.right;
            camFwd.y = 0f;
            camRight.y = 0f;
            camFwd.Normalize();
            camRight.Normalize();

            // 3. Calculate final World Vector
            Vector3 worldMoveDir = (camFwd * _currentSmoothedInput.y) + (camRight * _currentSmoothedInput.x);

            // 4. Feed to MxM
            _trajectoryGenerator.InputVector = worldMoveDir;
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw,
                0.0f
            );
        }

        #endregion

        #region Action Logic

        private void HandleActions()
        {
            // -- Dancing Logic --
            if (_isDancing)
            {
                // Interrupt dance if moving or button pressed again
                if ((DanceInput != null && DanceInput.action.WasPressedThisFrame()) || _input.move.sqrMagnitude > 0.1f)
                {
                    _mxmAnimator.EndLoopEvent();
                    _isDancing = false;
                }
                return;
            }

            if (DanceInput != null && DanceInput.action.WasPressedThisFrame())
            {
                if (DanceDefinition != null)
                {
                    _mxmAnimator.BeginEvent(DanceDefinition);
                    _isDancing = true;
                }
            }

            // -- Slide Logic --
            if (SlideInput != null && SlideInput.action.WasPressedThisFrame())
            {
                if (SlideDefinition != null)
                    _mxmAnimator.BeginEvent(SlideDefinition);
            }

            // -- Jump Logic --
            if (_input.jump)
            {
                // Ensure we are physically grounded before allowing jump
                if (JumpDefinition != null && _controllerWrapper != null && _controllerWrapper.IsGrounded)
                {
                    JumpDefinition.ClearContacts();
                    JumpDefinition.AddDummyContacts(1);
                    _mxmAnimator.BeginEvent(JumpDefinition);

                    // Predict Landing spot for precise animation warping
                    ref readonly EventContact eventContact = ref _mxmAnimator.NextEventContactRoot_Actual_World;
                    Ray ray = new Ray(eventContact.Position + (Vector3.up * 3.5f), Vector3.down);

                    if (Physics.Raycast(ray, out RaycastHit rayHit, 10f) && rayHit.distance > 1.5f && rayHit.distance < 5f)
                        _mxmAnimator.ModifyDesiredEventContactPosition(rayHit.point);
                    else
                        _mxmAnimator.ModifyDesiredEventContactPosition(eventContact.Position);

                    // Disable Gravity (Let Root Motion handle the arc)
                    _rootMotionApplicator.EnableGravity = false;
                }
                _input.jump = false; // Consume Input
            }

            // Re-enable gravity after event finishes
            if (_mxmAnimator.IsEventComplete && !_rootMotionApplicator.EnableGravity)
            {
                _rootMotionApplicator.EnableGravity = true;
            }
        }

        #endregion

        #region Audio Logic

        private void HandleLanding()
        {
            // If we were NOT grounded last frame, but ARE grounded now -> We Landed.
            if (!_wasGroundedLastFrame && _isGrounded)
            {
                // Check debounce timer to avoid double-sounds from physics jitter
                if (_landingBlockTimer <= 0f)
                {
                    PlayLandingSound();

                    _landingBlockTimer = LandingSoundCooldown;

                    // Delay next footstep so it doesn't overlap with landing thud
                    _footstepDistanceCounter = -FootstepStride * 0.5f;
                }
            }
        }

        private void HandleFootsteps()
        {
            // Calculate distance moved since last frame
            float distanceMoved = Vector3.Distance(transform.position, _lastWorldPosition);
            _lastWorldPosition = transform.position;

            // Ignore large jumps in position (Teleport/Warping)
            if (distanceMoved > 0.5f) return;

            // Accumulate distance if moving
            if (_isGrounded && distanceMoved > 0.001f)
            {
                _footstepDistanceCounter += distanceMoved;

                // Adjust stride length for Sprint vs Walk
                float actualStride = _input.sprint ? FootstepStride * 1.3f : FootstepStride;

                if (_footstepDistanceCounter >= actualStride)
                {
                    PlayFootstepSound();
                    _footstepDistanceCounter = 0f;
                }
            }
        }

        private void PlayFootstepSound()
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }

        private void PlayLandingSound()
        {
            if (LandingAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
            }
        }

        #endregion

        #region Helpers

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        #endregion
    }
}