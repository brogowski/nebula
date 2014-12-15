namespace Nebula.PlayerMovement
{
    public class PlayerController
    {
        private readonly float _speed;

        public PlayerController(float speed)
        {
            _speed = speed;
        }
        public IPlayerObject PlayerObject { get; set; }

        public void MoveHorizontally(float input)
        {
            PlayerObject.MoveHorizontally(_speed * input);
        }

        public void MoveVertically(float input)
        {
            PlayerObject.MoveVertically(_speed * input);
        }
    }
}