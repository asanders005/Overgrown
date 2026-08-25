using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitCountUIController : MonoBehaviour
{
    [SerializeField] private Image fruitTypeImage;
    [SerializeField] private TMP_Text fruitCountText;
    [SerializeField] private Image fruitDeliveredImage;
    [SerializeField] private List<FruitIconData> fruitIconDataList;
    [SerializeField] private Sprite defaultFruitIcon;

    public void Initialize(FruitType fruitType, int count)
    {
        fruitTypeImage.sprite = fruitIconDataList?.FirstOrDefault(data => data.FruitType == fruitType)?.FruitIcon ?? defaultFruitIcon;
        fruitCountText.text = count.ToString();
        fruitDeliveredImage.enabled = false;
    }

    public void UpdateCount(int count)
    {
        if (count <= 0)
        {
            fruitDeliveredImage.enabled = true;
            fruitCountText.enabled = false;
        }
        else
        {
            fruitDeliveredImage.enabled = false;
            fruitCountText.text = count.ToString();
            fruitCountText.enabled = true;
        }
    }
}
