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
        private const float _speed = 2f;
        private const float _input = 2f;

        [SetUp]
        public void SetUp()
        {
            _playerObjectMock = MockRepository.GenerateMock<IPlayerObject>();
            _playerController = new PlayerController(_speed) {PlayerObject = _playerObjectMock};
        }

        [Test]
        public void MoveHorizontallyIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveHorizontally(_speed*_input));

            _playerController.MoveHorizontally(_input);            
        }

        [Test]
        public void MoveVerticallyIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveVertically(_speed * _input));

            _playerController.MoveVertically(_input);            
        }

        [TearDown]
        public void TearDown()
        {
            _playerObjectMock.VerifyAllExpectations();
        }
    }
}
