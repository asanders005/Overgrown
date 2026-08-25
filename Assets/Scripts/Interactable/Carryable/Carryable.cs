using UnityEngine;

public enum CarryableType
{
    None,
    Seeds,
    Fertilizer,
    Fruit,
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Carryable : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Carryable;

    public bool CanInteract => true;

    public virtual void Interact()
    {
        _collider.enabled = !_collider.enabled;
        _rigidbody.bodyType = isCarried ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        isCarried = !isCarried;
    }

    protected bool isCarried = false;

    private Collider2D _collider;
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
}
