namespace Nebula.PlayerMovement
{
    public class PlayerController
    {        
        public IPlayerObject PlayerObject { get; set; }
        public float Speed { get; set; }

        public void MoveHorizontally(float input)
        {
            PlayerObject.MoveHorizontally(Speed * input);
        }
        public void MoveForward(float input)
        {
            PlayerObject.MoveForward(Speed * input);
        }
        public void ApplyGravity(float value)
        {
            PlayerObject.MoveVertically(-value);
        }
    }
}