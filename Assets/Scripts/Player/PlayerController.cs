using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Interaction Settings")]
    [SerializeField] private Transform carryTransform;

    private PlayerInputActions inputActions;
    private Rigidbody2D rb;

    private List<IInteractable> inRange = new List<IInteractable>();
    private GameObject carriedObj;

    private bool isTending = false;

    private Vector2 movementDirection;
    private Vector2 currentSpeed;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector2 targetSpeed = movementDirection * moveSpeed;

        float acceleration = 10f;

        if (Mathf.Abs(targetSpeed.magnitude) > 0.01f)
        {
            currentSpeed = Vector2.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Vector2.Lerp(currentSpeed, Vector2.zero, acceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = currentSpeed;
    }

    public void Move(Vector2 input)
    {
        movementDirection = isTending ? Vector2.zero : input.normalized;
    }

    public void Interact()
    {
        var closest = GetClosest();
        if (closest != null)
        {
            switch (closest.Type)
            {
                case InteractableType.Carryable:
                    if (carriedObj == null)
                    {
                        closest.Interact();
                        carriedObj = ((MonoBehaviour)closest).gameObject;
                        carriedObj.transform.SetParent(carryTransform);
                        carriedObj.transform.localPosition = Vector3.zero;
                        inRange.Remove(closest);
                    }
                    break;
                case InteractableType.Tendable:
                    Tendable tendable = closest as Tendable;
                    Debug.Log($"Tending {closest} for {tendable.TimeToInteract} seconds.");
                    isTending = true;
                    StartCoroutine(TendCoroutine(tendable.TimeToInteract, closest));
                    break;
                case InteractableType.Talkable:
                    closest.Interact();
                    break;
            }
        }
    }

    public void StopInteract()
    {
        if (isTending)
        {
            StopCoroutine(TendCoroutine(0, null));
            Debug.Log("Stopped tending.");
            isTending = false;
        }
    }

    public void Drop()
    {
        if (carriedObj != null)
        {
            carriedObj.GetComponent<IInteractable>().Interact();
            carriedObj.transform.SetParent(null);
            var rb = carriedObj.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
            carriedObj = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            IInteractable interactable = collision.GetComponent<IInteractable>();
            if (interactable != null)
            {
                inRange.Add(interactable);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            inRange.Remove(interactable);
        }
    }

    private IInteractable GetClosest()
    {
        if (inRange.Count == 0)
        {
            return null;
        }

        IInteractable closest = null;
        float closestDistance = Mathf.Infinity;
        foreach (IInteractable interactable in inRange)
        {
            if (interactable == null || !(interactable is MonoBehaviour mb) || !mb.enabled || !mb.gameObject.activeInHierarchy)
            {
                inRange.Remove(interactable);
                continue;
            }

            float distance = Vector2.SqrMagnitude(transform.position - mb.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }
        return closest;
    }

    private IEnumerator TendCoroutine(float duration, IInteractable interactable)
    {
        yield return new WaitForSeconds(duration);
        interactable.Interact();
        isTending = false;
    }
}
