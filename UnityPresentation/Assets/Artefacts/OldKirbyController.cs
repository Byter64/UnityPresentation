using UnityEngine;
using UnityEngine.InputSystem;

public class OldKirbyController : MonoBehaviour
{
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

	public void OnMove(InputAction.CallbackContext context)
	{
		if (!context.performed) return;
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
