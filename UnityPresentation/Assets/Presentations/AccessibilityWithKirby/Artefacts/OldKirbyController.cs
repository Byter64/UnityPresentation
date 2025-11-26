using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldKirbyController : MonoBehaviour
{
	[SerializeField] private float walkSpeed;
	[SerializeField] private float slowWalkSpeed;
	[SerializeField] private float fallSpeed;
	[SerializeField] private float slowFallSpeed;
	[SerializeField] private float flightSpeed;
	[SerializeField] private float groundHeight;
	[SerializeField] private float jumpHeight;
	[SerializeField] private float jumpTime;
	[SerializeField] private float minFlightTime;
	[SerializeField] private AnimationCurve jumpCurve;
	[SerializeField] private Vector2 xBounds;

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
	private bool isGliding;
	private bool isJumping;
	private bool isGrounded;

	private Animator animator;

	private Coroutine flying;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		horSpeed = ((isGliding || isFlying) ? slowWalkSpeed : walkSpeed) * horInput;
		isCrouching = duckInput && isGrounded;
		isGrounded = transform.position.y == groundHeight;

		if(flyInput && !isSucking && !isFull && !isFlying)
		{
			StartCoroutine(Fly());
		}

		if(!isGrounded && !isJumping && !isFlying)
		{
			Vector3 pos = transform.position;
			if(!isGliding)
				pos.y -= fallSpeed * Time.fixedDeltaTime;
			else
				pos.y -= slowFallSpeed * Time.fixedDeltaTime;
			if (pos.y < groundHeight)
				pos.y = groundHeight;
			transform.position = pos;
		}

		if (!isCrouching && !isSucking)
		{
			Vector3 pos = transform.localPosition;
			pos.x += horSpeed * Time.fixedDeltaTime;
			pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
			transform.localPosition = pos;
		}

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
		animator.SetBool("isGliding", isGliding);
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


		if (context.performed && isGrounded)
		{
			if (flying != null) StopCoroutine(flying);
			flying = StartCoroutine(Jump());
		}

	}

	public void OnAction2(InputAction.CallbackContext context)
	{
		action2Input = context.ReadValueAsButton();
		if(context.performed && !isSucking && ! isFlying && ! isGliding)
		{
			StartCoroutine(Suck());
		}
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

	private IEnumerator Fly()
	{
		isFlying = true;

		float time = 0;
		while(time < minFlightTime)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
			Vector3 pos = transform.position;
			pos.y += flightSpeed * Time.fixedDeltaTime;
			transform.position = pos;
		}

		while(flyInput)
		{
			yield return new WaitForFixedUpdate();
			Vector3 pos = transform.position;
			pos.y += flightSpeed * Time.fixedDeltaTime;
			transform.position = pos;
		}
		isFlying = false;
		isGliding = true;

		while(!action2Input)
		{
			yield return new WaitForFixedUpdate();
		}

		isGliding = false;
		flying = null;
	}

	private IEnumerator Suck()
	{
		isSucking = true;

		while(action2Input)
		{
			yield return new WaitForFixedUpdate();
		}

		animator.SetBool("endSucking", true);
		yield return new WaitForSeconds(0.3f);

		animator.SetBool("endSucking", false);
		animator.SetBool("isSucking", false);
		isSucking = false;
	}
}
