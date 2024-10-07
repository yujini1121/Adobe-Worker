using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    public class Slide : State
    {
        public override string StateName => "Slide";
        public override string StateIDName => "Slide";

        [Header("Slide Movement & Rotation")]

        [Tooltip("Lerp value for the Aligment to the surface")]
        public FloatReference OrientLerp = new(10f);

        [Tooltip("The rotation of the character while sliding will be ignored. This value is overriden by the Ground Slide Data")]
        public BoolReference ignoreRotation = new();
        private bool IgnoreRotation;

        [Tooltip("When Sliding the Animal will be able to orient towards the direction of this given angle")]
        public FloatReference RotationAngle = new(30f);
        [Tooltip("When Sliding the Animal will be able to Move horizontally with this value")]
        public FloatReference SideMovement = new(5f);


        [Header("Enter Conditions")]
        [Tooltip("If the Slope of the Slide ground is greater that this value, the Slide State be Activated. Zero value will ignore Enter by Slope")]
        public FloatReference EnterAngleSlope = new(0);

        [Header("Exit Conditions")]
        [Tooltip("If the Speed is lower than this value the Slide state will end.")]
        public FloatReference ExitSpeed = new(0.5f);

        [Tooltip("If the Slope of the Slide ground is greater that this value, the Slide State will exit")]
        public FloatReference ExitAngleSlope = new(60f);

        [Tooltip("When a Flat terrain is reached. it will wait this time to transition to Locomotion or Idle")]
        public FloatReference ExitWaitTime = new(0.5f);

        private float currentExitTime;

        //[Header("Exit Status Values")]
        //[Tooltip("The Exit Status will be set to 1 if the Exit Condition was the Exit Speed")]
        //public IntReference ExitSpeedStatus = new(1);
        //[Tooltip("The Exit Status will be set to 2 if the Exit Condition was that there's no longer a Ground Changer")]
        //public IntReference NoChangerStatus = new(2);




        public override bool TryActivate()
        {
            return TrySlideGround();
        }

        public override void OnPlataformChanged(Transform newPlatform)
        {
            //Debug.Log($"OnPlataformChanged {(newPlatform ? newPlatform.name : "null")}");

            if (!IsActiveState && TrySlideGround() && CanBeActivated)
            {
                Activate();
            }
            //else if (IsActiveState && !animal.InGroundChanger && CanExit)
            //{
            //    Debugging("[Allow Exit] No Ground Changer");
            //    //SetExitStatus(NoChangerStatus);
            //    SetExitStatus(NoChangerStatus);
            //    AllowExit();
            //}
        }


        /// <summary>  The State moves always forward  </summary>
        public override bool KeepForwardMovement => true;

        public override void Activate()
        {
            base.Activate();
            IgnoreRotation = ignoreRotation.Value; //Set the default value
            additiveSide = 0;  //Set the default value
            //Add the additional Speed
            if (animal.InGroundChanger)
            {
                animal.CurrentSpeedModifier.position.Value += animal.GroundChanger.SlideData.AdditiveForwardSpeed;
                IgnoreRotation = animal.GroundChanger.SlideData.IgnoreRotation;
                additiveSide = animal.GroundChanger.SlideData.AdditiveHorizontalSpeed;
            }

            currentExitTime = 0;
        }
        private float additiveSide;

        private bool TrySlideGround()
        {
            if (animal.InGroundChanger
                && animal.GroundChanger.SlideData.Slide                                     //Meaning the terrain is set to slide
                && animal.SlopeDirectionAngle > animal.GroundChanger.SlideData.MinAngle     //The character is looking at the Direction of the slope
                && animal.SlopeDirectionAngle < ExitAngleSlope     //The Slope is too deep to enter the slide
                )
            {
                //CHECK THE DIRECTION OF THE SLIDE
                if (Vector3.Angle(animal.Forward, animal.SlopeDirection) < animal.GroundChanger.SlideData.ActivationAngle / 2)
                {
                    return true;
                }
            }
            else //When is not using GroundChanger use the Enter AngleSlope
            {
                if (animal.Grounded && EnterAngleSlope > 0 && animal.SlopeDirectionAngle > EnterAngleSlope)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Override the Input Axis to match the State movement 
        /// </summary>
        public override void InputAxisUpdate()
        {
            var move = animal.RawInputAxis;


            if (AlwaysForward) animal.RawInputAxis.z = 1;

            DeltaAngle = move.x;
            var NewInputDirection = Vector3.ProjectOnPlane(animal.SlopeDirection, animal.UpVector);

            if (animal.MainCamera)
            {
                //Normalize the Camera Forward Depending the Up Vector IMPORTANT!
                var Cam_Forward = Vector3.ProjectOnPlane(animal.MainCamera.forward, UpVector).normalized;
                var Cam_Right = Vector3.ProjectOnPlane(animal.MainCamera.right, UpVector).normalized;

                move = (animal.RawInputAxis.z * Cam_Forward) + (animal.RawInputAxis.x * Cam_Right);
                DeltaAngle = Vector3.Dot(animal.Right, move);
            }

            NewInputDirection = Quaternion.AngleAxis(RotationAngle * DeltaAngle, animal.Up) * NewInputDirection;

            if (currentExitTime > 0) NewInputDirection = Vector3.zero;


            //NewInputDirection *= animal.RawInputAxis.z;
            animal.MoveFromDirection(NewInputDirection);  //Move using the slope Direction instead

            // MDebug.Draw_Arrow(transform.position, NewInputDirection, Color.green);

            HorizontalLerp = Vector3.Lerp(HorizontalLerp, Vector3.Project(move, animal.Right), animal.DeltaTime * CurrentSpeed.lerpPosition);

            if (GizmoDebug)
                MDebug.Draw_Arrow(transform.position, HorizontalLerp, Color.white);

        }

        /// <summary>Smooth Horizontal Lerp value to move left and right </summary>
        private Vector3 HorizontalLerp { get; set; }

        float DeltaAngle;

        public override Vector3 Speed_Direction()
        {
            var NewInputDirection = animal.SlopeDirection;

            if (!IgnoreRotation) //Use the Slope Direction to move the state
            {
                NewInputDirection = Quaternion.AngleAxis(RotationAngle * DeltaAngle, animal.Up) * NewInputDirection;
            }

            return NewInputDirection;
        }


        public override void OnStateMove(float deltatime)
        {
            if (InCoreAnimation)
            {
                //Calculate the Horizontal Direction
                var Right = Vector3.Cross(animal.Up, animal.SlopeDirection);
                Right = Vector3.Project(animal.MovementAxisSmoothed, Right);

                if (GizmoDebug)
                    MDebug.Draw_Arrow(transform.position, Right, Color.red);

                animal.AdditivePosition += (deltatime * (SideMovement + additiveSide)) * HorizontalLerp; //Move Left or right while sliding
                //Orient to the Ground
                animal.AlignRotation(animal.SlopeNormal, deltatime, OrientLerp);

                if (IgnoreRotation)
                {
                    animal.AlignRotation(animal.Forward, animal.SlopeDirection, deltatime, OrientLerp); //Make your own Aligment
                    animal.UseAdditiveRot = false; //Remove Rotations
                }

                if (!animal.Grounded)
                {
                    animal.UseGravity = true;
                }
            }
        }


        public override void TryExitState(float DeltaTime)
        {
            if (animal.SlopeDirectionAngle > ExitAngleSlope || !animal.MainRay)
            {
                animal.Grounded = false;
                Debugging("[Allow Exit] Exit to Fall. Terrain Slope is too deep");
                AllowExit(3, 0); //Exit to Fall!!!
            }
            //if we are on a ground changer
            else if (animal.InGroundChanger)
            {
                if (!animal.GroundChanger.SlideData.Slide)
                {
                    Debugging("[Allow Exit] No longer in a Slide Ground Changer");
                    //SetExitStatus(NoChangerStatus);
                    AllowExit();
                }
            }
            else
            {
                //There's no an angle slope
                if (EnterAngleSlope > 0 && animal.SlopeDirectionAngle < EnterAngleSlope)
                {
                    currentExitTime += DeltaTime;

                    if (currentExitTime > ExitWaitTime)
                    {
                        Debugging("[Allow Exit] No longer in a slope angle");
                        // SetExitStatus(ExitSpeedStatus);
                        AllowExit();
                        currentExitTime = 0;
                    }
                }
                else
                {
                    currentExitTime = 0;
                }
            }


            //Exit when there no more speed in the 
            if (animal.HorizontalSpeed <= ExitSpeed)
            {
                Debugging("[Allow Exit] Speed is Slow");
                //SetExitStatus(ExitSpeedStatus);
                AllowExit();
                return;
            }
        }


        internal override void Reset()
        {
            base.Reset();

            General = new AnimalModifier()
            {
                RootMotion = true,
                Grounded = true,
                Sprint = true,
                OrientToGround = false,
                CustomRotation = true,
                IgnoreLowerStates = true,
                AdditivePosition = true,
                AdditiveRotation = true,
                Gravity = false,
                modify = (modifier)(-1),
            };
        }
    }
}