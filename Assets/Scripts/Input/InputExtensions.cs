using UnityEngine;
using UnityEngine.InputSystem;

public static class InputExtensions
{
	public static string TryGetControlType(this InputAction action)
	{
		// For Value and PassThrough types, check expectedControlType
		if (!string.IsNullOrEmpty(action.expectedControlType))
		{
			return action.expectedControlType;
		}

		// First check action type
		if (action.type == InputActionType.Button)
		{
			return "Button";
		}

		// If expectedControlType is null or empty, check bindings
		foreach (var binding in action.bindings)
		{
			if (binding.path.Contains("Stick") || binding.path.Contains("Vector2"))
			{
				return "Vector2";
			}
			else if (binding.path.Contains("Axis") || binding.path.Contains("Trigger"))
			{
				return "Float";
			}
		}

		// Default to the action type if we can't determine more specific info
		return action.type.ToString();
	}
}
