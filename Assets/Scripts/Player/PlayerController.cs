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

    [Header("Upgrade Settings")]
    [SerializeField] private float moveSpeedMultiplierIncrement = 0.1f;
    [SerializeField] private float tendDurationMultiplierDecrement = 0.1f;

    [Header("Events")]
    [SerializeField] private Event onSellEvent;
    [SerializeField] private Event onOrderDeposit;
    [SerializeField] private FruitEvent onOrderUpdate;
    [SerializeField] private Event onOrderComplete;

    [SerializeField] private Event onMoveSpeedUpgrade;
    [SerializeField] private Event onTendDurationUpgrade;

    private Rigidbody2D rb;
    private TimerManager timerManager;

    private List<IInteractable> inRange = new List<IInteractable>();
    private GameObject carriedObj;
    private bool deliveryStarted = false;

    private bool isTending = false;

    private Vector2 movementDirection;
    private Vector2 currentSpeed;

    private Coroutine tendingCoroutine;

    private float moveSpeedMultiplier = 1f;
    private float tendDurationMultiplier = 1f;

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

    #region Carried Object Event Handlers
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
    #endregion


    #region Input Events
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
                    float duration = tendable.TimeToInteract * tendDurationMultiplier;
                    tendingCoroutine = StartCoroutine(TendCoroutine(duration, closest));
                    timerManager.SetTimer(duration);
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
    #endregion

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

    #region Upgrade Methods
    private void OnMoveSpeedUpgrade()
    {
        moveSpeedMultiplier += moveSpeedMultiplierIncrement;
        moveSpeed *= moveSpeedMultiplier;
    }

    private void OnTendDurationUpgrade()
    {
        tendDurationMultiplier -= tendDurationMultiplierDecrement;
    }

    #endregion
}
