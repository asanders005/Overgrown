using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TimerManager))]
public class PlayerController : MonoBehaviour
{
    public bool isCarryingObject => carriedObj != null;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Interaction Settings")]
    [SerializeField] private Transform carryTransform;

    [Header("Events")]
    [SerializeField] private Event onSellEvent;
    [SerializeField] private Event onOrderDeposit;
    [SerializeField] private FruitEvent onOrderUpdate;
    [SerializeField] private Event onOrderComplete;

    private Rigidbody2D rb;
    private TimerManager timerManager;

    private List<IInteractable> inRange = new List<IInteractable>();
    private GameObject carriedObj;
    private bool deliveryStarted = false;

    private bool isTending = false;

    private Vector2 movementDirection;
    private Vector2 currentSpeed;

    private Coroutine tendingCoroutine;

    public bool SetCarriedObj(IInteractable obj)
    {
        if (carriedObj != null) return false;
        obj.Interact();
        carriedObj = ((MonoBehaviour)obj).gameObject;
        carriedObj.transform.SetParent(carryTransform);
        carriedObj.transform.localPosition = Vector3.zero;
        return true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        timerManager = GetComponent<TimerManager>();
    }

    private void OnEnable()
    {
        onOrderDeposit.Subscribe(OnFruitDeliver);
        onOrderComplete.Subscribe(OnDeliverComplete);
        onSellEvent.Subscribe(OnFruitSell);
    }

    private void OnDisable()
    {
        onOrderDeposit.Unsubscribe(OnFruitDeliver);
        onOrderComplete.Unsubscribe(OnDeliverComplete);
        onSellEvent.Unsubscribe(OnFruitSell);
    }

    private void OnFruitSell()
    {
        if (carriedObj == null) return;

        var fruit = carriedObj.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.Sell();
            carriedObj = null;
        }
    }

    private void OnFruitDeliver()
    {
        if (carriedObj == null) return;

        var fruit = carriedObj.GetComponent<Fruit>();
        if (fruit != null)
        {
            onOrderUpdate.RaiseEvent(fruit.Type);
            deliveryStarted = true;
        }
    }

    private void OnDeliverComplete()
    {
        if (!deliveryStarted || carriedObj == null) return;

        Destroy(carriedObj);
        carriedObj = null;
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
                        SetCarriedObj(closest);
                        inRange.Remove(closest);
                    }
                    break;
                case InteractableType.Tendable:
                    Tendable tendable = closest as Tendable;
                    isTending = true;
                    tendingCoroutine = StartCoroutine(TendCoroutine(tendable.TimeToInteract, closest));
                    timerManager.SetTimer(tendable.TimeToInteract);
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
            timerManager.StopTimer();
            StopCoroutine(tendingCoroutine);
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
            if (distance < closestDistance && interactable.CanInteract)
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
