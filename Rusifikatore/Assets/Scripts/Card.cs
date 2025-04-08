using System;
using DefaultNamespace;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    // Сериализованные поля
    [Header("Параметры карты")]
    [SerializeField] private float neededSpeed;
    [SerializeField] private float flipTime;
    [SerializeField] private float swipeTime;
    [SerializeField] private float returnTime;
    [Header("Ссылки на компоненты карты")]
    [SerializeField] private TextMeshProUGUI wordText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    // Публичные поля
    public UnityEvent swiped { get; private set; } = new();
    
    // Приватные поля
    private Vector2 startPosition;
    private float startTime;
    private Vector3 touchOffset;
    private bool isReturning;
    private bool isChecked;
    
    // MonoBehaviour
    private void OnMouseDown()
    {
        // Выходим, если карта не проверена
        if (!isChecked) return;
        
        // Получение touchOffset для передвижения карты
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        touchOffset = transform.position - mouseWorldPos;

        // Установка значений при начале свайпа
        startPosition = transform.position;
        startTime = Time.time;
    }

    private void OnMouseDrag()
    {
        // Выходим, если карта не проверена
        if (!isChecked) return;
        
        // Перемещение карты с учётом touchOffset
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        transform.position = mouseWorldPos + touchOffset;
    }

    private void OnMouseUp()
    {
        if (isChecked)
            TryToSwipe();
        else
            Check();
    }
    
    // Публичные поля
    public void Setup(CardData cardData)
    {
        // Установка имени
        var wordName = cardData.word;
        foreach (var skippedLetterIndex in cardData.skippedLetters)
        {
            wordName = wordName.Substring(0, skippedLetterIndex)
                       + "\u25A1" + wordName.Substring(skippedLetterIndex+ 1);
        }
        wordText.text = wordName;
        
        // Установка описания
        descriptionText.text = cardData.description;
    }
    
    // Приватные методы
    private void Check()
    {
        // Проверяем карту
        transform.DORotate(Vector3.up * 180, flipTime);
        isChecked = true;
    }
    
    private void TryToSwipe()
    {
        if (GetSwipeSpeed() >= neededSpeed && !isReturning)
        {
            // Перемещаем карту за границы экрана, при удачном свайпе
            var swipeDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0, 0)), Vector2.zero) * 2;
            var swipeDirection = GetPositionDifference(startPosition, transform.position).normalized * swipeDistance;
            var targetPosition = new Vector3(swipeDirection.x, swipeDirection.y, transform.position.z);
            transform.DOMove(targetPosition, swipeTime);
            
            // Запуск события конца свайпа
            Invoke(nameof(EndSwipe), swipeTime);
        }
        else
        {
            // Возвращаем карту в центр при неудачном свайпе
            isReturning = true;
            var targetPosition = new Vector3(0, 0, transform.position.z);
            transform.DOMove(targetPosition, returnTime);
            
            // Запуск события конца возврата карты
            Invoke(nameof(ReturnCard), returnTime);
        }
    }
    
    private float GetSwipeSpeed()
    {
        // Получение расстояния и времени свайпа
        var movementDistance = GetPositionDifference(startPosition, transform.position).magnitude;
        var movementTime = Time.time - startTime;

        // Получение скорости свайпа
        return Mathf.Abs(movementDistance / movementTime);
    }

    private Vector2 GetPositionDifference(Vector2 startPosition, Vector2 endPosition)
    {
        return endPosition - startPosition;
    }

    private void ReturnCard()
    {
        isReturning = false;
    }

    private void EndSwipe()
    {
        swiped.Invoke();
    }
}