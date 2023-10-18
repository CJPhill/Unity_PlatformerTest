using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public Camera cameraComponent;
    public GameObject virtualCam;
    public Transform playerTransform;
    public Transform respawn;
    public float distanceThreshold = 20f;
    public bool isActive = false;


    
    private void Update()
    {
        if (!isActive && GameManager.Instance.roomManager == this)
        {
            Vector3 bottomBorder = cameraComponent.ViewportToWorldPoint(new Vector3(0.5f, 0f, cameraComponent.nearClipPlane));

            float distanceY = Mathf.Abs(playerTransform.position.y - bottomBorder.y);

            if (distanceY > distanceThreshold)
            {
                RespawnPlayer();
                Debug.Log("should be moving");
            }
        }

    }

    public void RespawnPlayer()
    {
        playerTransform.position = respawn.position;
        playerTransform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
            isActive = true;
            GameManager.Instance.roomManager = this;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(false);
            isActive = false;
            
        }
    }


}
