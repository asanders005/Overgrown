using UnityEngine;

[CreateAssetMenu(fileName = "FruitIconData", menuName = "Data/FruitIconData")]
public class FruitIconData : ScriptableObjectBase
{
    public FruitType FruitType { get => fruitType; }
    public Sprite FruitIcon { get => fruitIcon; }

    [SerializeField] private FruitType fruitType;
    [SerializeField] private Sprite fruitIcon;
}
