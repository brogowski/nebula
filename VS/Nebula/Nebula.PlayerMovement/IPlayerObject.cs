namespace Nebula.PlayerMovement
{
    public interface IPlayerObject
    {
        void MoveHorizontally(float value);
        void MoveVertically(float value);
        void MoveForward(float value);
    }
}