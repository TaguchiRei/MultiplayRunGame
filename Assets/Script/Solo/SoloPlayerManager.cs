using UnityEngine;

public class SoloPlayerManager : MonoBehaviour
{
    private static readonly int FB = Animator.StringToHash("FB");
    private static readonly int LR = Animator.StringToHash("LR");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;

    [SerializeField] private Vector3 _defaultGravity;
    [SerializeField] private Vector3 _fallingGravity;

    private bool _gameStarted;
    private bool _onGround;
    private Vector3 _moveDirection;

    private void Start()
    {
        _gameStarted = false;
        _onGround = true;
    }

    private void FixedUpdate()
    {
        if (!_gameStarted) return;

        if (_rigidbody.linearVelocity.y > 2)
        {
            _rigidbody.AddForce(_defaultGravity, ForceMode.Acceleration);
        }
        else
        {
            _rigidbody.AddForce(_fallingGravity, ForceMode.Acceleration);
        }

        _rigidbody.linearVelocity = new Vector3(_moveDirection.x, _rigidbody.linearVelocity.y, 0);
    }

    public void GameStart()
    {
        _animator.SetFloat(FB, 1f);

        _inputManager.OnMove += MoveDirectionUpdate;
        _inputManager.OnMoveEnd += StopMovement;
        _inputManager.OnJump += JumpInput;

        _gameStarted = true;
    }

    private void MoveDirectionUpdate(Vector2 inputVector)
    {
        _animator.SetFloat(LR, inputVector.x);
        _moveDirection = new Vector3(inputVector.x, 0, 0) * _moveSpeed;
    }

    private void StopMovement()
    {
        _animator.SetFloat(LR, 0f);
        _moveDirection = Vector3.zero;
    }

    private void JumpInput()
    {
        if (_onGround)
        {
            _animator.SetBool(Jump, true);
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _onGround = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _onGround = true;
            _animator.SetBool(Jump, false);
        }
    }
}
