using Nebula.PlayerMovement;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Nebula.PlayerMovement
{
    [TestFixture]
    public class PlayerMovementTests
    {
        private const float Speed = 2f;
        private const float Input = 2f;
        private PlayerController _playerController;
        private IPlayerObject _playerObjectMock;

        [SetUp]
        public void SetUp()
        {
            _playerObjectMock = MockRepository.GenerateMock<IPlayerObject>();
            _playerController = new PlayerController
            {
                PlayerObject = _playerObjectMock,
                Speed = Speed,                
            };
        }

        [Test]
        public void MoveHorizontallyIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveHorizontally(Speed*Input));

            _playerController.MoveHorizontally(Input);            
        }

        [Test]
        public void MoveForwardIsMultipliedBySpeed()
        {
            _playerObjectMock.Expect(q => q.MoveForward(Speed * Input));

            _playerController.MoveForward(Input);            
        }

        [TearDown]
        public void TearDown()
        {
            _playerObjectMock.VerifyAllExpectations();
        }
    }
}
