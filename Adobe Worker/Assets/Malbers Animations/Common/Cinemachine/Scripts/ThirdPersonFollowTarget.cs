using Cinemachine;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Camera/Third Person Follow Target (Cinemachine)")]
    //[AddComponentMenu("Malbers/Camera/Third Person Follow Target (CM3)")]
    public class ThirdPersonFollowTarget : MonoBehaviour
    {
        /// <summary> List of all the scene Third Person Follow Cameras (using the same brain)! </summary>
        public static HashSet<ThirdPersonFollowTarget> TPFCameras;

        [Tooltip("Cinemachine Brain Camera")]
        public CinemachineBrain Brain;

        //[Tooltip("Update mode for Camera Position Logic")]
        //public UpdateType updateMode = UpdateType.FixedUpdate;

        [Tooltip("The Camera can rotate independent of the Game Time")]
        public BoolReference unscaledTime = new(true);

        [Tooltip("Default Priority of this Cinemachine camera")]
        public int priority = 10;
        [Tooltip("Changes the Camera Side parameter on the Third Person Camera")]
        [Range(0f, 1f), SerializeField]
        private float cameraSide = 1f;
        [Tooltip("Default Camera Distance set to the Third Person Cinemachine Camera")]
        public FloatReference CameraDistance = new(6);

        [Tooltip("What object to follow")]
        public TransformReference Target;
        [Tooltip("Reference of a Transform to get the Up Vector, so the camera can be aligned with it vector")]
        public TransformReference upVector;

        public Transform CamPivot { get; set; }

        [Tooltip("Camera Input Values (Look X:Horizontal, Look Y: Vertical)")]
        public Vector2Reference look = new();

        [Header("Camera Properties")]
        [Tooltip("Sensitivity to rotate the X Axis")]
        public FloatReference XMultiplier = new(1);
        [Tooltip("Sensitivity to rotate the Y Axis")]
        public FloatReference YMultiplier = new(1);
        [Tooltip("How far in degrees can you move the camera up")]
        public FloatReference TopClamp = new(70.0f);
        [Tooltip("How far in degrees can you move the camera down")]
        public FloatReference BottomClamp = new(-30.0f);

        [Tooltip("Lerp Rotation to smooth out the movement of the camera while rotating.")]
        public FloatReference LerpRotation = new(15f);
        [Tooltip("Lerp Position to smooth out the movement of the camera while following the target.")]
        public FloatReference lerpPosition = new(0);
        [Tooltip("Invert X Axis of the Look Vector")]
        public BoolReference invertX = new();
        [Tooltip("Invert Y Axis of the Look Vector")]
        public BoolReference invertY = new();

        [Header("Mouse Keyboard and GamePad")]
        [Tooltip("Is the camera using Mouse Input (true) or a Gamepad (False)")]
        public BoolReference UsingMouse = new(true);
        [Tooltip("Extra Multiplier for the Rotation sensitivity when using a gamepad")]
        public FloatReference GamepadMult = new(100);

        public BoolEvent OnActiveCamera = new();

        #region Properties
        private float InvertX => invertX.Value ? -1 : 1;
        private float InvertY => invertY.Value ? 1 : -1;

        public float XSensibility { get => XMultiplier; set => XMultiplier.Value = value; }
        public float YSensibility { get => YMultiplier; set => YMultiplier.Value = value; }
        public float LerpPosition { get => lerpPosition; set => lerpPosition.Value = value; }
        public Transform UpVector { get => upVector; set => upVector.Value = value; }
        public bool UnScaledTime { get => unscaledTime; set => unscaledTime.Value = value; }


        /// <summary>  Active Camera using the same Cinemachine Brain </summary>
       // public ThirdPersonFollowTarget ActiveThirdPersonCamera { get; set; }

        public float CameraSide { get => cameraSide; set => cameraSide = value; }

        /// <summary>  Last Active Camera using the same Cinemachine Brain </summary>
        public bool LastThirdPersonCamera { get; set; }

        /// <summary>The Current Camera is not a Third Person Follow Target</summary>
        public ICinemachineCamera ActiveCM_NOT3rdPerson { get; set; }

        /// <summary>The Current Brain Active Camera</summary>
        private ICinemachineCamera BrainActiveCamera { get; set; }

        /// <summary> The Cinemachine Camera that is using this script</summary>    
        private ICinemachineCamera ThisCamera;

        private bool Active { get; set; }

        #endregion


        //CINEMACHINE 2
        private Cinemachine3rdPersonFollow CM3PFollow;
        ////CINEMACHINE 3
        //private CinemachineThirdPersonFollow CM3PFollow;


        [Disable] public float _cinemachineTargetYaw;
        [Disable] public float _cinemachineTargetPitch;
        private const float _threshold = 0.00001f;

        public void SetMouse(bool value) => UsingMouse.Value = value;
        public bool SetInvertX(bool value) => invertX.Value = value;
        public bool SetInvertY(bool value) => invertY.Value = value;

        // readonly WaitForFixedUpdate mWaitForFixedUpdate = new();
        /// readonly WaitForEndOfFrame mWaitForLateUpdate = new();

        // Start is called before the first frame update
        void Awake()
        {
            if (Brain == null) Brain = FindObjectOfType<CinemachineBrain>();

            //CINEMACHINE 2
            CM3PFollow = this.FindComponent<Cinemachine3rdPersonFollow>();

            ////CINEMACHINE 3
            //CM3PFollow = this.FindComponent<CinemachineThirdPersonFollow>();


            if (CM3PFollow != null)
            {
                CM3PFollow.CameraDistance = CameraDistance;
                CM3PFollow.CameraSide = CameraSide;
            }

            UsingMouse.Value = true;
        }

        private void OnEnable()
        {
            TPFCameras ??= new(); //Initialize the Cameras
            TPFCameras.Add(this);

            CreateCameraPivot();

            //CINEMACHINE 2
            //Find the Cinemachine camera target
            if (TryGetComponent(out ThisCamera) && ThisCamera.Follow == null)
                ThisCamera.Follow = CamPivot.transform;

            ////CINEMACHINE 3
            ////Find the Cinemachine camera Target
            //if (TryGetComponent(out ThisCamera))
            //    (ThisCamera as CinemachineCamera).Target.TrackingTarget = CamPivot.transform;


            //Set the Up Vector to the Camera Brain
            this.Delay_Action(1,
                () =>
                //CINEMACHINE 2
                Brain.m_WorldUpOverride = UpVector

                // //CINEMACHINE 3
                //Brain.WorldUpOverride = UpVector

                );

            CinemachineCore.CameraUpdatedEvent.AddListener(UpdateCameraEvent); //Listen to the Camera Updated Event

            CameraMove(0, 0); //Position (Late Update Only)

            StartCoroutine(ICameraRotation()); //Rotation (Late Update Only)
        }

        private void OnDisable()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateCameraEvent); //Remove Listener to the Camera Updated Event
            StopAllCoroutines();
            TPFCameras.Remove(this);
        }

        private void CreateCameraPivot()
        {
            //Search on the other TFP cameras to see if we are using the same Target...
            //if we are using the same Target use their Cam Pivot instead
            if (CamPivot == null)
            {
                foreach (var c in TPFCameras)
                {
                    if (c == this) continue; //Skip itself

                    //If another Camera is using the same 
                    if (c.Target.Value == Target.Value && c.CamPivot != null)
                    {
                        CamPivot = c.CamPivot; //Use the same Cam Pivot
                        break;
                    }
                }
            }

            if (CamPivot == null) //There's no CamPivot after searching in all other Cameras, let's create one
            {
                CamPivot = new GameObject($"CamPivot - [{(Target.Value != null ? Target.Value.name : name)}]").transform;
                CamPivot.ResetLocal();
                CamPivot.parent = null;
                //CamPivot.hideFlags = HideFlags.HideInHierarchy; //Hide it we do not need to see it
            }
        }


        private void UpdateCameraEvent(CinemachineBrain camBrain)
        {
            if (Brain == camBrain) //Update Same Brain Cameras
            {
                if (camBrain.ActiveVirtualCamera != BrainActiveCamera)
                {
                    BrainActiveCamera = camBrain.ActiveVirtualCamera;

                    var IsThirdPFT = (BrainActiveCamera as CinemachineVirtualCameraBase).GetComponent<ThirdPersonFollowTarget>();

                    ActiveCM_NOT3rdPerson = IsThirdPFT == null ? BrainActiveCamera : null; //Store the Active Camera if is NOT a Third Person Follow Camera
                }

                //Camera Movement-----

                //CINEMACHINE 2
                if (Brain.m_UpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
                ////CINEMACHINE 3
                //if (Brain.UpdateMethod == CinemachineBrain.UpdateMethods.LateUpdate)
                {
                    CameraPos(UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                }
                else
                {
                    CameraPos(UnScaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
                }
            }
        }

        private IEnumerator ICameraRotation()
        {
            while (true)
            {
                // yield return new WaitForEndOfFrame();
                CameraRotation(UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime, LerpRotation);
                // CameraRotation(UnScaledTime ? Time.fixedUnscaledTime : Time.fixedDeltaTime, LerpRotation);
                yield return null;
            }
        }


        private void CameraPos(float deltaTime)
        {
            //Update the Active Camera if we are the active camera
            if (ThisCamera == Brain.ActiveVirtualCamera)
            {
                if (!Active)
                {
                    Active = true;
                    OnActiveCamera.Invoke(Active);
                    CameraMove(LerpPosition, deltaTime);
                    LastThirdPersonCamera = false;
                    return;     //Skip this cycle
                }
            }
            else
            {
                //Make sure this to disable the Camera if is no longer the Brain Active Camera
                if (Active)
                {
                    LastThirdPersonCamera = true;
                    Active = false;
                    OnActiveCamera.Invoke(Active);
                }
            }

            if (Active)
            {
                //Skip if the TimeScale is zero
                if (!UnScaledTime && Time.timeScale == 0)
                {
                    look.Value = Vector2.zero;
                    return;
                }


                CameraMove(LerpPosition, deltaTime);
                SetCameraSide(CameraSide);
            }
        }

        private void CameraMove(float lerp, float deltatime)
        {
            if (Target.Value == null) return;

            if (lerp == 0)
                CamPivot.transform.position = Target.position;
            else
                CamPivot.transform.position = Vector3.Lerp(CamPivot.transform.position, Target.position, lerp * deltatime);

        }

        private void CameraRotation(float deltaTime, float lerp)
        {
            if (Active)
            {
                // if there is an input and camera position
                if (look.Value.sqrMagnitude >= _threshold)
                {
                    //Don't multiply mouse input by Time.deltaTime;
                    float deltaTimeMultiplier = UsingMouse ? 1.0f : (deltaTime * GamepadMult);

                    _cinemachineTargetYaw += look.x * InvertX * XMultiplier * deltaTimeMultiplier;
                    _cinemachineTargetPitch += look.y * InvertY * YMultiplier * deltaTimeMultiplier;
                }

                // clamp our rotations so our values are limited 360 degrees
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Cinemachine will follow this target
                var TargetRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

                if (UpVector) TargetRotation = Quaternion.FromToRotation(Vector3.up, UpVector.up) * TargetRotation;

                if (lerp > 0)
                    CamPivot.rotation = Quaternion.Lerp(CamPivot.rotation, TargetRotation, deltaTime * lerp); //NEEDED FOR SMOOTH CAMERA MOVEMENT
                else
                    CamPivot.rotation = TargetRotation;

                UpdateAllCamerasYawPitch(); //Update the other Cameras that are using the same Brain
            }
            else if (ActiveCM_NOT3rdPerson != null)
            {
                if (!Brain.IsBlending)
                {
                    _cinemachineTargetYaw = -Vector3.SignedAngle(Brain.transform.forward, Vector3.forward, Vector3.up);
                    _cinemachineTargetPitch = -Vector3.SignedAngle(Brain.transform.up, Vector3.up, Brain.transform.right);


                    //If this is the last Third Person Camera used , then update the CamPivot Transform
                    if (LastThirdPersonCamera)
                        CamPivot.SetPositionAndRotation(Target.Value.position, Brain.transform.rotation);
                }
            }
        }


        private void UpdateAllCamerasYawPitch()
        {
            foreach (var c in TPFCameras)
            {
                //Skip if the camera is using different pivots
                if (c.Target.Value != Target.Value || c.Brain != Brain || c.Active) continue;

                //Update Rotation Values to all other cameras
                c._cinemachineTargetYaw = _cinemachineTargetYaw;
                c._cinemachineTargetPitch = _cinemachineTargetPitch;
            }
        }

        public void SetLookX(float x) => look.x = x;
        public void SetLookY(float y) => look.y = y;
        public void SetLook(Vector2 look) => this.look.Value = look;



        public void SetTarget(Transform target) => Target.Value = target;

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }


        public void SetPriority(bool value)
        {
            if (ThisCamera == null) return;

#if UNITY_6000_0_OR_NEWER
            if (ThisCamera is CinemachineCamera cam)
                {
                    if (value)
                    {
                        cam.Priority.Value = priority;
                        cam.Priority.Enabled = true;
                    }
                    else
                    {
                        cam.Priority.Value = -1;
                        cam.Priority.Enabled = false;
                    }
                }            
#else 
            ThisCamera.Priority = value ? priority : -1;
#endif
        }

        public void SetCameraSide(bool value) => SetCameraSide(value ? 1 : 0);

        public void SetCameraSide(float value)
        {
            if (CameraSide != value)
            {
                CameraSide = value;
                CM3PFollow.CameraSide = CameraSide;
            }
        }

        public void TargetTeleport() => TargetTeleport(false);

        public void TargetTeleport(bool BehindTarget)
        {
            //Update the Active Camera if we are the active camera
            // if (ThisCamera == Brain.ActiveVirtualCamera)
            {
                //Remove the damping for 5 frames so the camera can teleport correctly
                var OldDamp = CM3PFollow.Damping;
                CM3PFollow.Damping = Vector3.zero;
                // Debug.Log("TELEPORT");


                CameraMove(0, 0); //Teleport the Camera to the Target

                //  Brain.ManualUpdate(); //Force the Brain to update the Camera

                this.Delay_Action(5, () => CM3PFollow.Damping = OldDamp);
            }

            if (BehindTarget) YawBehindTarget();
        }

        public void YawBehindTarget()
        {
            if (Target.Value != null)
            {
                _cinemachineTargetYaw = Vector3.SignedAngle(Vector3.forward, Target.Value.forward, UpVector != null ? UpVector.up : Vector3.up);
                Debug.DrawRay(Target.Value.position, Target.Value.forward * 10, Color.green, 2f);

                CameraRotation(0, 0);
            }
        }



        //private IEnumerator ICameraPosition()
        //{
        //    if (updateMode == UpdateType.FixedUpdate)
        //    {
        //        var wait = new WaitForFixedUpdate();

        //        while (true)
        //        {
        //            yield return wait;
        //            CameraPos(UnScaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
        //        }
        //    }
        //    else
        //    {
        //        while (true)
        //        {
        //            CameraPos(UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        //            yield return null;
        //        }
        //    }
        //}

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying && CM3PFollow != null)
                CM3PFollow.CameraSide = CameraSide;
        }

        private void Reset()
        {
            Target.UseConstant = false;
            Target.Variable = MTools.GetInstance<TransformVar>("Camera Target");


            if (CamPivot == null)
            {
                CamPivot = new GameObject("Pivot").transform;
                CamPivot.parent = transform;
                CamPivot.ResetLocal();
            }
        }
#endif
    }
}
