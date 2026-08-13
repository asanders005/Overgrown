using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlantAnimator : MonoBehaviour
{
    [SerializeField] private Sprite seedSprite;
    [SerializeField] private Sprite sproutSprite;
    [SerializeField] private Sprite matureSprite;
    [SerializeField] private Sprite spoiledSprite;
    [SerializeField] private Sprite witheredSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite(PlantState state)
    {
        switch (state)
        {
            case PlantState.Seed:
                spriteRenderer.sprite = seedSprite;
                break;
            case PlantState.Sprout:
                spriteRenderer.sprite = sproutSprite;
                break;
            case PlantState.Mature:
                spriteRenderer.sprite = matureSprite;
                break;
            case PlantState.Spoiled:
                spriteRenderer.sprite = spoiledSprite;
                break;
            case PlantState.Withered:
                spriteRenderer.sprite = witheredSprite;
                break;
        }
    }
}
