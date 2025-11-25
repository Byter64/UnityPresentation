using UnityEngine;
using UnityEngine.InputSystem;

public class OldKirbyController : MonoBehaviour
{
	[SerializeField] private float walkSpeed;
	[SerializeField] private float groundHeight;

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

	private Animator animator;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		horSpeed = horInput * walkSpeed;
		isCrouching = duckInput;
		isSucking = action2Input;
		isFlying = flyInput;
		isJumping = actionInput;


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
		if (!context.performed) return;
		actionInput = context.ReadValue<bool>();
	}

	public void OnAction2(InputAction.CallbackContext context)
	{
		if (!context.performed) return;
		action2Input = context.ReadValue<bool>();
	}
}
