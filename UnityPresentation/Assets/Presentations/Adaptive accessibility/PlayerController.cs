using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 movement;
    [SerializeField] private float moveforce;
	[SerializeField] private float jumpForce;
	private Rigidbody rigidbody;

	private bool isGrounded;
	private bool isTouchingCheese;
	private void OnEnable()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		rigidbody.AddForce(movement.normalized * moveforce * Time.fixedDeltaTime, ForceMode.Impulse);

		isGrounded = Physics.Raycast(transform.position, Vector3.down, 1);
	}

	private void OnTriggerEnter(Collider other)
	{
		isTouchingCheese = true;
	}

	private void OnTriggerExit(Collider other)
	{
		isTouchingCheese = false;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 input = context.ReadValue<Vector2>();
		movement = new Vector3(input.x, 0, input.y);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!context.performed) return;
		if (!isGrounded) return;
		rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
	}

	public void OnCollect(InputAction.CallbackContext context)
	{
		if(!context.performed) return;
		if(!isTouchingCheese) return;

		Debug.Log("You got the cheese");
	}
}
