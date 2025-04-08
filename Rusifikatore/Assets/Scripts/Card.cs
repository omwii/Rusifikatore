using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    // Сериализованные поля
    [Header("Параметры карты")]
    [SerializeField] private float neededSpeed;
    [SerializeField] private float swipeTime;
    [SerializeField] private float returnTime;
    
    // Публичные поля
    public UnityEvent swiped = new();
    
    // Приватные поля
    private Vector2 startPosition;
    private float startTime;
    private bool isReturning;
    private Vector3 touchOffset;
    
    // MONO
    private void OnMouseDown()
    {
        // Получение touchOffset для передвижения карты
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        touchOffset = transform.position - mouseWorldPos;
        
        // Установка значений при начале свайпа
        startPosition = transform.position;
        startTime = Time.time;
    }

    private void OnMouseDrag()
    {
        // Перемещение карты с учётом touchOffset
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos + touchOffset;
    }

    private void OnMouseUp()
    {
        TryToSwipe();
    }
    
    // Приватные методы
    private void TryToSwipe()
    {
        if (GetSwipeSpeed() >= neededSpeed && !isReturning)
        {
            // Перемещаем карту за границы экрана, при удачном свайпе
            var swipeDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0, 0)), Vector2.zero) * 2;
            var swipeDirection = GetPositionDifference(startPosition, transform.position).normalized * swipeDistance;
            transform.DOMove(swipeDirection, swipeTime);
            
            // Запуск события конца свайпа
            Invoke(nameof(EndSwipe), swipeTime);
        }
        else
        {
            // Возвращаем карту в центр при неудачном свайпе
            isReturning = true;
            transform.DOMove(Vector3.zero, returnTime);
            
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