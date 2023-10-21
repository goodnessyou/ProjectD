using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public ThirdPersonMovement player;
    public float raycastDistance = 3f;

    private void Update()
    {
        // Выпускаем луч вниз из текущего объекта
        // RaycastHit hit;
        // if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        // {
        //     // Получаем объект, в который попал луч
        //     GameObject hitObject = hit.collider.gameObject;
            
        //     // Проверяем тег объекта
        //     if (hitObject.CompareTag("Ground"))
        //     {
        //         // Объект имеет нужный тег, выполните действия здесь
        //         player.GroundSet(true);
        //     }
        // }
        // else
        // {
        //     player.GroundSet(false);
        // }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Ground"))
        {
            
            player.GroundSet(true);
        }
        else
        {
            
        }
    }
    private void OnTriggerStay(Collider other) 
    {
       
        //Debug.Log("other.gameObject.name");

        if(other.CompareTag("Ground"))
        {
            
            player.GroundSet(true);
        }
        else
        {
            
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Ground"))
        {
            player.GroundSet(false);
        }
        else
        {
            
        }
    }
    
}
