using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Сериализованные поля
    [Header("Параметры GameManager")]
    [SerializeField] private CardData[] cardData;
    [SerializeField] private GameObject cardPrefab;
    
    // MonoBehaviour
    private void Start()
    {
        for (int i = 0; i < cardData.Length; i++)
        {
            var cardGameObject = Instantiate(cardPrefab, Vector3.back * i, Quaternion.identity);
            var card = cardGameObject.GetComponent<Card>();
            card.Setup(cardData[i]);
        }
    }
}