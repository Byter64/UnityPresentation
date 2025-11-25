using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class KirbyController1 : MonoBehaviour
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

	private bool leftInput;
	private bool rightInput;
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
	private bool isForwardJumping;
	private bool isGrounded;

	private Animator animator;

	private Coroutine flying;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		float horInput = -Convert.ToInt32(leftInput) + Convert.ToInt32(rightInput);
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

		if (!isCrouching && !isSucking && !isForwardJumping)
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

	public void OnUp(InputAction.CallbackContext context)
	{
		if (context.performed)
			flyInput = !duckInput;
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		if (context.performed)
			duckInput = !duckInput;
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			leftInput = !leftInput;
			if(leftInput && rightInput)
			{
				rightInput = false;
				leftInput = false;
			}
		}
	}
	public void OnRight(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			rightInput = !rightInput;
			if (leftInput && rightInput)
			{
				rightInput = false;
				leftInput = false;
			}
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		actionInput = context.ReadValueAsButton();


		if (context.performed && isGrounded)
		{
			if (flying != null) StopCoroutine(flying);
			flying = StartCoroutine(Jump());
		}

	}

	public void OnForwardJump(InputAction.CallbackContext context)
	{
		if (context.performed && isGrounded)
		{
			if (flying != null) StopCoroutine(flying);
			flying = StartCoroutine(ForwardJump());
			leftInput = false;
			rightInput = false;
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
		while (time < jumpTime)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
			Vector3 pos = transform.position;
			pos.y = groundHeight + jumpCurve.Evaluate(time / jumpTime) * jumpHeight;
			transform.position = pos;
		}

		isJumping = false;
	}

	private IEnumerator ForwardJump()
	{
		isForwardJumping = true;
		isJumping = true;
		float time = 0;
		while (time < jumpTime)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
			Vector3 pos = transform.localPosition;
			pos.x += walkSpeed * Mathf.Sign(transform.localScale.x) * Time.fixedDeltaTime;
			pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
			pos.y = groundHeight + jumpCurve.Evaluate(time / jumpTime) * jumpHeight;
			transform.localPosition = pos;
		}

		isForwardJumping = false;
		isJumping = false;
	}

	private IEnumerator Fly()
	{
		isFlying = true;
		flyInput = false;

		float time = 0;
		float flyingHeight = transform.localPosition.y;
		while (time < minFlightTime)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
			Vector3 pos = transform.position;
			pos.y += flightSpeed * Time.fixedDeltaTime;
			transform.position = pos;
		}

		float puffDelta = transform.localPosition.y - flyingHeight;
		flyingHeight = transform.localPosition.y;


		while (!action2Input)
		{
			time = 0;
			yield return new WaitForFixedUpdate();
			while (time < minFlightTime)
			{
				yield return new WaitForFixedUpdate();
				time += Time.fixedDeltaTime;
				Vector3 pos = transform.position;
				pos.y += flightSpeed * Time.fixedDeltaTime;
				transform.position = pos;
			}

			if(flyInput)
			{
				flyInput = false;
				flyingHeight = transform.localPosition.y;
			}
			if(duckInput)
			{
				duckInput = false;
				flyingHeight -= puffDelta;
				if(flyingHeight < groundHeight)
				{
					action2Input = true;
				}
			}

			while (transform.localPosition.y >= flyingHeight)
			{
				yield return new WaitForFixedUpdate();

				Vector3 pos = transform.position;
				pos.y -= slowFallSpeed * Time.fixedDeltaTime;
				transform.position = pos;

				if (action2Input)
				{
					break;
				}
			}
		}

		action2Input = false;

		isFlying = false;
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
