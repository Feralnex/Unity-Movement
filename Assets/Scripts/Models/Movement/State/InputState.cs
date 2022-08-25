using UnityEngine;

partial class Movement : MonoBehaviour
{
    class InputState : IInputState
    {
        public float Speed { get; set; }
        public float DirectionAngle { get; set; }
        public bool FacingDirection { get; set; }
        public float FacingDirectionAngle { get; set; }
        public float SpeedMultiplier { get; set; }
        public float RotationMultiplier { get; set; }

        public InputState()
        {
            Speed = 0;
            DirectionAngle = 0;
            FacingDirectionAngle = 0;
            SpeedMultiplier = 0;
            RotationMultiplier = 0;
        }

        public InputState(float speed, float directionAngle, bool facingDirection, float facingDirectionAngle, float speedMultiplier, float rotationMultiplier)
        {
            Speed = speed;
            DirectionAngle = directionAngle;
            FacingDirectionAngle = facingDirectionAngle;
            SpeedMultiplier = speedMultiplier;
            RotationMultiplier = rotationMultiplier;
        }
    }
}
