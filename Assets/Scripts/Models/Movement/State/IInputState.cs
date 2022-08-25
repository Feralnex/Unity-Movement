using UnityEngine;

partial class Movement : MonoBehaviour
{
    public interface IInputState
    {
        float Speed { get; }
        float DirectionAngle { get; }
        bool FacingDirection { get; }
        float FacingDirectionAngle { get; }
        float SpeedMultiplier { get; }
        float RotationMultiplier { get; }

        public static IInputState Create(float speed, float directionAngle, bool facingDirection, float facingDirectionAngle, float speedMultiplier, float rotationMultiplier) => new InputState(speed, directionAngle, facingDirection, facingDirectionAngle, speedMultiplier, rotationMultiplier);
    }
}
