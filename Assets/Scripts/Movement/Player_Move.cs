using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(Rigidbody2D))]

    public class Player_Move : MonoBehaviour
    {
        [Header("HorizontalMove")]
        [SerializeField] private float _speed;
        [SerializeField] private bool _faceRight;

        [Header("Jump")]
        [SerializeField] private float _jumpPower;
        [SerializeField] private Transform _graundChecker;
        [SerializeField] private float _graundCheckerRadius;
        [SerializeField] private LayerMask _whatIsGround;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        private AnimationState _currentState;
        private string _animatorParameterName;

        private Rigidbody2D _rigidbody2D;
        private float _direction;
        private bool _jump;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animatorParameterName = _animator.GetParameter(0).name;
        }

        private void Update()
        {
            _direction= Input.GetAxisRaw("Horizontal");
            if(Input.GetButtonDown("Jump"))
            _jump =true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_graundChecker.position, _graundCheckerRadius);
        }

        private void FixedUpdate()
        {
            bool isGrounded = Physics2D.OverlapCircle(_graundChecker.position, _graundCheckerRadius, _whatIsGround);

            Move(_direction);
            SetDirection() ;
            if(_jump && isGrounded)
                Jump();
            _jump = false;

            PlayAnimation(AnimationState.Jump, !isGrounded && _rigidbody2D.velocity.y > 0);
            PlayAnimation(AnimationState.Fall, !isGrounded && _rigidbody2D.velocity.y < 0);
            }

        private void Move(float direction)
        {
            _rigidbody2D.velocity = new Vector2(_speed * direction, _rigidbody2D.velocity.y);
            //PlayAnimation(AnimationState.Run);
        }

        private void Jump()
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);
        }

        private void PlayAnimation(AnimationState animationState, bool active)
        {
            if (animationState < _currentState)
                return;

            if (!active)
            {
                if (animationState == _currentState)
                {
                    _animator.SetInteger(_animatorParameterName, (int)AnimationState.Idle);
                    _currentState = AnimationState.Idle;
                }
                return;
            }

            _animator.SetInteger(_animatorParameterName, (int)animationState);
            _currentState = animationState;
        }

        private void SetDirection()
        {
            if (_faceRight && _direction < 0)
                Flip();
            else if (!_faceRight && _direction > 0)
                Flip();
        }

        private void Flip()
        {
            _faceRight = !_faceRight;
            transform.Rotate(0, 180, 0);
        }

        private enum AnimationState
        {
            Idle = 0,
            Run = 1,
            Jump = 2,
            Fall = 3,
        }

    }

}