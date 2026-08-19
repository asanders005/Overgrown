using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FloatEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting float values to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Fruit Event")]
public class FruitEvent : ScriptableObjectBase
{
	/// <summary>
	/// Unity Action that holds references to all subscribed methods.
	/// Allows dynamically calling multiple functions when the event is raised.
	/// </summary>
	public UnityAction<FruitType> OnEventRaised;

	/// <summary>
	/// Raises the event with the specified float value.
	/// </summary>
	/// <param name="value">The float value to pass to subscribers.</param>
	public void RaiseEvent(FruitType value)
	{
		OnEventRaised?.Invoke(value);
	}

	/// <summary>
	/// Subscribes a listener to the event.
	/// </summary>
	/// <param name="listener">The method that will be called when the event is raised.</param>
	public void Subscribe(UnityAction<FruitType> listener)
	{
		OnEventRaised += listener;
	}

	/// <summary>
	/// Unsubscribes a listener from the event.
	/// </summary>
	/// <param name="listener">The method that should no longer be called when the event is raised.</param>
	public void Unsubscribe(UnityAction<FruitType> listener)
	{
		OnEventRaised -= listener;
	}
}