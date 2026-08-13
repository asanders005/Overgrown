using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for the InputListener component.
/// Provides a context-aware inspector that only shows relevant events based on the input type.
/// </summary>
[CustomEditor(typeof(InputListener))]
public class InputListenerEditor : Editor
{
	/// <summary>
	/// Draws the custom inspector GUI for the InputListener component.
	/// </summary>
	public override void OnInspectorGUI()
	{
		// Update the serialized object representation
		serializedObject.Update();

		// Draw the input action reference field
		var inputActionRefProp = serializedObject.FindProperty("inputAction");
		EditorGUILayout.PropertyField(inputActionRefProp);

		// Get a reference to the target component
		InputListener relay = (InputListener)target;

		// Try to access the underlying Unity input action through the property chain
		// This uses null conditional operators (?.) to safely access nested properties
		var inputAction = relay.InputAction?.inputActionReference?.action;

		// If we have a valid input action, display appropriate events based on its type
		if (inputAction != null)
		{
			// Get the expected control type from the input action
			string type = inputAction.TryGetControlType();

			// Add some space and a header for the events section
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Events for Input Type: " + type, EditorStyles.boldLabel);

			// Show only the relevant events for the current input type
			switch (type)
			{
				case "Vector2":
					// For Vector2 inputs (like joysticks), show only the Vector2 event
					EditorGUILayout.PropertyField(serializedObject.FindProperty("OnVector2Input"));
					break;
				case "Axis":
				case "Float":
					// For float-based inputs (like triggers), show only the float event
					EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFloatInput"));
					break;
				case "Button":
					// For button inputs, show both pressed and released events
					EditorGUILayout.PropertyField(serializedObject.FindProperty("OnButtonPressed"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("OnButtonReleased"));
					break;
				default:
					// Show a warning for any unsupported input types
					EditorGUILayout.HelpBox("Unsupported input type: " + type, MessageType.Warning);
					break;
			}
		}
		else
		{
			// If no input action is available, display a helpful message to guide the user
			EditorGUILayout.HelpBox("Assign an InputActionReference to configure events.", MessageType.Info);
		}

		// Apply any modified properties back to the serialized object
		serializedObject.ApplyModifiedProperties();
	}
}