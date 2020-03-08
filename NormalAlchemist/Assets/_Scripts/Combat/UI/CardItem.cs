using MyBattle;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public Button CardItemBtn;
    public Text CardName;
    public Text CardDescription;

    public void InitData(CardData cardData)
    {
        CardName.text = cardData.cardName;
        CardDescription.text = cardData.cardDescription;

        CardItemBtn.onClick.AddListener(cardData.SelectCurrentCard);
    }
}
