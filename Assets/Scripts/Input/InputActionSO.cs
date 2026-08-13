using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// ScriptableObject that wraps an InputActionReference and provides Unity events
/// for different input types (Vector2, Float, Button).
/// </summary>
[CreateAssetMenu(fileName = "InputAction", menuName = "Scriptable Objects/InputAction")]
public class InputActionSO : ScriptableObject
{
	[Header("Input Action Asset")]
	[Tooltip("Reference to the Input Action that this ScriptableObject will wrap")]
	public InputActionReference inputActionReference;

	[Header("Unity Actions")]
	[Tooltip("Event triggered when receiving Vector2 input (like movement)")]
	public UnityAction<Vector2> OnVector2Input;

	[Tooltip("Event triggered when receiving float input (like analog triggers)")]
	public UnityAction<float> OnFloatInput;

	[Tooltip("Event triggered when a button is pressed")]
	public UnityAction OnButtonPressed;

	[Tooltip("Event triggered when a button is released")]
	public UnityAction OnButtonReleased;

	// Using [System.NonSerialized] to prevent Unity from serializing this value
	// This ensures it starts as false every time Unity loads the asset
	[System.NonSerialized]
	public bool initialized = false;

	/// <summary>
	/// Called by Unity when the ScriptableObject is loaded or enabled
	/// </summary>
	private void OnEnable()
	{
		// Reset the initialization flag to ensure proper setup
		initialized = false;
	}

	/// <summary>
	/// Sets up the appropriate event handlers based on the input action type
	/// and enables the input action.
	/// </summary>
	public void Initialize()
	{
		// Skip if already initialized or if no input action is assigned
		if (initialized || inputActionReference == null) return;

		InputAction action = inputActionReference.action;

		// Subscribe to appropriate events based on the expected control type
		switch (action.TryGetControlType())
		{
			case "Vector2":
				// For Vector2 controls (like joysticks, directional pads)
				action.performed += ctx => OnVector2Input?.Invoke(ctx.ReadValue<Vector2>());
				// When input stops, send zero vector to indicate no movement
				action.canceled += ctx => OnVector2Input?.Invoke(Vector2.zero);
				break;

			case "Axis":
			case "Float":
				// For float controls (like triggers, analog buttons)
				action.performed += ctx => OnFloatInput?.Invoke(ctx.ReadValue<float>());
				// When input stops, send zero to indicate no input
				action.canceled += ctx => OnFloatInput?.Invoke(0);
				break;

			case "Button":
				// For button controls (like keyboard keys, gamepad buttons)
				// Started occurs when button is first pressed
				action.started += ctx => OnButtonPressed?.Invoke();
				// Canceled occurs when button is released
				action.canceled += ctx => OnButtonReleased?.Invoke();
				break;
		}

		// Enable the input action to start receiving inputs
		action.Enable();
		// Mark as initialized to prevent duplicate subscriptions
		initialized = true;
	}

	/// <summary>
	/// Disables the input action and cleans up the initialization state
	/// </summary>
	public void Deinitialize()
	{
		// Return early if there's no input action reference
		if (inputActionReference == null) return;

		// Disable the input action to stop receiving inputs
		inputActionReference.action.Disable();
		// Reset initialization state
		initialized = false;
	}

	/// <summary>
	/// Called by Unity when the ScriptableObject is disabled
	/// </summary>
	private void OnDisable()
	{
		// Reset initialization flag
		initialized = false;

		// Clean up any event subscriptions and disable the action
		if (inputActionReference != null && inputActionReference.action != null)
		{
			inputActionReference.action.Disable();
		}
	}
}