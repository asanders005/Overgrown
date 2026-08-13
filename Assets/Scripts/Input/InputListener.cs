using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Bridges between input system events and Unity Events.
/// Listens to InputActionSO events and forwards them to UnityEvents
/// that can be connected in the Inspector.
/// </summary>
public class InputListener : MonoBehaviour
{
	/// <summary>
	/// Reference to the ScriptableObject that handles input actions.
	/// This must be assigned in the inspector for the component to function.
	/// </summary>
	[SerializeField] InputActionSO inputAction;

	[Header("Unity Events")]
	/// <summary>
	/// Unity Event that gets invoked when Vector2 input is received.
	/// Connect game object behaviors in the inspector to react to directional input.
	/// </summary>
	public UnityEvent<Vector2> OnVector2Input;

	/// <summary>
	/// Unity Event that gets invoked when float input is received.
	/// Useful for axis inputs such as triggers or analog sticks.
	/// </summary>
	public UnityEvent<float> OnFloatInput;

	/// <summary>
	/// Unity Event that gets invoked when a button is pressed.
	/// Use this to trigger actions on button down events.
	/// </summary>
	public UnityEvent OnButtonPressed;

	/// <summary>
	/// Unity Event that gets invoked when a button is released.
	/// Use this to trigger actions on button up events.
	/// </summary>
	public UnityEvent OnButtonReleased;

	/// <summary>
	/// Public accessor for the InputActionSO assigned to this listener.
	/// </summary>
	public InputActionSO InputAction => inputAction;

	/// <summary>
	/// Called when the component is enabled. Initializes the input action and 
	/// subscribes the Unity Events to the corresponding input action events.
	/// </summary>
	private void OnEnable()
	{
		// Skip initialization if no input action is assigned
		if (inputAction == null)
		{
			Debug.LogWarning("InputAction is not assigned in " + gameObject.name);
			return;
		}

		// Initialize the input action (sets up the underlying input system connections)
		inputAction.Initialize();

		// Register Unity Event handlers to corresponding input action events		
		inputAction.OnVector2Input += OnVector2Input.Invoke;
		inputAction.OnFloatInput += OnFloatInput.Invoke;
		inputAction.OnButtonPressed += OnButtonPressed.Invoke;
		inputAction.OnButtonReleased += OnButtonReleased.Invoke;
	}

	/// <summary>
	/// Called when the component is disabled. Unsubscribes events and 
	/// deinitializes the input action to prevent memory leaks.
	/// </summary>
	private void OnDisable()
	{
		// Skip cleanup if no input action is assigned
		if (inputAction == null) return;

		// Unregister Unity Event handlers from input action events
		// This prevents memory leaks and ensures proper cleanup
		inputAction.OnVector2Input -= OnVector2Input.Invoke;
		inputAction.OnFloatInput -= OnFloatInput.Invoke;
		inputAction.OnButtonPressed -= OnButtonPressed.Invoke;
		inputAction.OnButtonReleased -= OnButtonReleased.Invoke;

		// Clean up the input action (releases any resources held by the input system)
		inputAction.Deinitialize();
	}
}