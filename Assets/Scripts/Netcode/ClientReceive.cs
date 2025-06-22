using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ClientReceive : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        Client.instance.myId = _myId;
        ClientSend.Welcome();
        Client.instance.udp.Connect( ((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port );
        UIManager.instance.ErrorMessageClose();
        UIManager.instance.MenuToLobby();
    }
    
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        int _avatar = _packet.ReadInt();
        string _avatarUrl = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        
        GameManager.instance.SpawnPlayer(_id, _username, _avatar, _avatarUrl, _position, _rotation);
    }

    public static void SpawnEnvironment(Packet _packet)
    {
        int _id = _packet.ReadInt();
        
        UIManager.instance.lobby.SetActive(false);
        GameManager.instance.SpawnEnvironment(_id);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        
        Vector3 _position = _packet.ReadVector3();
        GameManager.players[_id].transform.position = _position;
        
        Vector3 _headPosition = _packet.ReadVector3();
        GameManager.players[_id].GetComponent<PlayerController>().head.transform.position = _headPosition;
        
        Vector3 _leftPosition = _packet.ReadVector3();
        GameManager.players[_id].GetComponent<PlayerController>().left.transform.position = _leftPosition;
        
        Vector3 _rightPosition = _packet.ReadVector3();
        GameManager.players[_id].GetComponent<PlayerController>().right.transform.position = _rightPosition;
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.players[_id].transform.rotation = _rotation;
        
        Quaternion _headRotation = _packet.ReadQuaternion();
        GameManager.players[_id].GetComponent<PlayerController>().head.transform.rotation = _headRotation;
        
        Quaternion _leftRotation = _packet.ReadQuaternion();
        GameManager.players[_id].GetComponent<PlayerController>().left.transform.rotation = _leftRotation;
        
        Quaternion _rightRotation = _packet.ReadQuaternion();
        GameManager.players[_id].GetComponent<PlayerController>().right.transform.rotation = _rightRotation;
    }
    
    public static void ChatMessage(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _senderId = _packet.ReadInt();
        
        Debug.Log($"CHAT from server: " + GameManager.players[_senderId].username + " Message: " + "{_msg}");
        ChatManager.instance.OnReceiveMessage(_msg, _senderId);
    }
    
    public static void VoiceMessage(Packet _packet)
    {
        int size = _packet.ReadInt();
        float[] data = new float[size];
        for (int x = 0; x < size; x++)
        {
            float value = _packet.ReadFloat();
            data[x] = value;
        }
        AudioManager.instance.ReceiveData(data);
    }

    public static void LobbyMessage(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int size = _packet.ReadInt();
        for (int x = 0; x < size; x++)
        {
            string value = _packet.ReadString();
            GameManager.instance.BeginDownloadAvatar(value);
            Debug.Log("Downloading avatar from url: " + value);
        }
        UIManager.instance.lobbynames.text = _msg;
    }
}