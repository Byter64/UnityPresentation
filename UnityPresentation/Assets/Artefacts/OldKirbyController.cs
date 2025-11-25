using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldKirbyController : MonoBehaviour
{
	[SerializeField] private float walkSpeed;
	[SerializeField] private float fallSpeed;
	[SerializeField] private float groundHeight;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float jumpTime;
	[SerializeField] private AnimationCurve jumpCurve;

	private float horInput;
	private bool duckInput;
	private bool flyInput;
	private bool actionInput;
	private bool action2Input;


	private float horSpeed;
	private bool isCrouching;
	private bool isSucking;
	private bool isFull;
	private bool isFlying;
	private bool isJumping;
	private bool isGrounded;

	private Animator animator;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		horSpeed = horInput * walkSpeed;
		isCrouching = duckInput && isGrounded;
		isSucking = action2Input;
		isFlying = flyInput;
		isGrounded = transform.position.y == groundHeight;

		if(!isGrounded && !isJumping && !isFlying)
		{
			Vector3 pos = transform.position;
			pos.y -= fallSpeed * Time.fixedDeltaTime;
			if(pos.y < groundHeight)
				pos.y = groundHeight;
			transform.position = pos;
		}

		if(!isCrouching)
			transform.position += new Vector3(horSpeed, 0, 0) * Time.fixedDeltaTime;

		if(horInput != 0)
		{
			Vector3 scale = transform.localScale;
			scale.x = Mathf.Abs(scale.x) * horInput;
			transform.localScale = scale;
		}

		animator.SetFloat("horSpeed_Abs", Mathf.Abs(horSpeed));
		animator.SetBool("isCrouching", isCrouching);
		animator.SetBool("isSucking", isSucking);
		animator.SetBool("isFull", isFull);
		animator.SetBool("isFlying", isFlying);
		animator.SetBool("isJumping", isJumping);
		animator.SetBool("isGrounded", isGrounded);
	}


	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 input = context.ReadValue<Vector2>();
		horInput = Mathf.RoundToInt(input.x);
		duckInput = input.y < 0;
		flyInput = input.y > 0;
	}

	public void OnAction(InputAction.CallbackContext context)
	{
		actionInput = context.ReadValueAsButton();


		if(context.performed && isGrounded)
			StartCoroutine(Jump());
	}

	public void OnAction2(InputAction.CallbackContext context)
	{
		action2Input = context.ReadValueAsButton();
	}

	private IEnumerator Jump()
	{
		isJumping = true;
		float time = 0;
		while(actionInput && time < jumpTime)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
			Vector3 pos = transform.position;
			pos.y = groundHeight + jumpCurve.Evaluate(time / jumpTime) * jumpHeight;
			transform.position = pos;
		}

		isJumping = false;
	}
}
