using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FloatEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting float values to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Upgrade Event")]
public class UpgradeEvent : ScriptableObjectBase
{
	/// <summary>
	/// Unity Action that holds references to all subscribed methods.
	/// Allows dynamically calling multiple functions when the event is raised.
	/// </summary>
	public UnityAction<UpgradeType, FruitType?> OnEventRaised;

	/// <summary>
	/// Raises the event with the specified float value.
	/// </summary>
	/// <param name="value">The float value to pass to subscribers.</param>
	public void RaiseEvent(UpgradeType value, FruitType? fruitVal = null)
	{
		OnEventRaised?.Invoke(value, fruitVal);
	}

	/// <summary>
	/// Subscribes a listener to the event.
	/// </summary>
	/// <param name="listener">The method that will be called when the event is raised.</param>
	public void Subscribe(UnityAction<UpgradeType, FruitType?> listener)
	{
		OnEventRaised += listener;
	}

	/// <summary>
	/// Unsubscribes a listener from the event.
	/// </summary>
	/// <param name="listener">The method that should no longer be called when the event is raised.</param>
	public void Unsubscribe(UnityAction<UpgradeType, FruitType?> listener)
	{
		OnEventRaised -= listener;
	}
}