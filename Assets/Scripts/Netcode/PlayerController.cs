using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject head;
    public GameObject left;
    public GameObject right;

    public void Start()
    {
        if (this.GetComponent<PlayerManager>().id != Client.instance.myId)
        {
            left.GameObject().SetActive(false);
            right.GameObject().SetActive(false);
        }
    }
    
    public void Update()
    {
        if (this.GetComponent<PlayerManager>().id != Client.instance.myId)
            return;
        
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
        }
        if(Input.GetKey(KeyCode.A))
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
        }
        if(Input.GetKey(KeyCode.S))
        {
            this.transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
        }
        if(Input.GetKey(KeyCode.D))
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f);
        }
    }
    
    private void FixedUpdate()
    {
        if (this.GetComponent<PlayerManager>().id != Client.instance.myId)
            return;
        
        ClientSend.PlayerMovement();
    }
}