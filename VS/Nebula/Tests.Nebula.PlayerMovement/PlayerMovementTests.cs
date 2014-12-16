using Nebula.PlayerMovement;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Nebula.PlayerMovement
{
    [TestFixture]
    public class PlayerMovementTests
    {
        private PlayerController _playerController;
        private IPlayerObject _playerObjectMock;
        private const float _gravity = 9.81f;
        private const float _speed = 2f;
        private const float _input = 2f;

        [SetUp]
        public void SetUp()
        {
            _playerObjectMock = MockRepository.GenerateMock<IPlayerObject>();
            _playerController = new PlayerController
            {
                PlayerObject = _playerObjectMock,
                Speed = _speed,                
            };
        }

        [Test]
        public void MoveHorizontallyIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveHorizontally(_speed*_input));

            _playerController.MoveHorizontally(_input);            
        }

        [Test]
        public void MoveForwardIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveForward(_speed * _input));

            _playerController.MoveForward(_input);            
        }

        [Test]
        public void GravityIsApplied()
        {
            _playerObjectMock.Expect(q => q.MoveVertically(-_gravity));

            _playerController.ApplyGravity(_gravity);
        }

        [TearDown]
        public void TearDown()
        {
            _playerObjectMock.VerifyAllExpectations();
        }
    }
}
