using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    public static void Welcome()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcome))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);
            _packet.Write(UIManager.instance.avatar.value);
            _packet.Write(UIManager.instance.avatarUrlField.text);
            SendTCPData(_packet);
        }
    }
    public static void Deploy(int _environment)
    {
        using (Packet _packet = new Packet((int)ClientPackets.deployMessage))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_environment);
            SendTCPData(_packet);
        }
    }

    public static void VoiceMessage(float[] _data)
    {
        using (Packet _packet = new Packet((int)ClientPackets.voiceMessage))
        {
            int size = _data.Length;
            _packet.Write(size);
            foreach (var x in _data)
            {
                _packet.Write(x);
            }
            SendTCPData(_packet);
        }
    }
    
    public static void PlayerMovement()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            // WHOLE
            _packet.Write(GameManager.players[Client.instance.myId].transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            
            // HEAD
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().head.transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().head.transform.rotation);
            
            // LEFT
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().left.transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().left.transform.rotation);
            
            // RIGHT
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().right.transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].GetComponent<PlayerController>().right.transform.rotation);
            
            SendUDPData(_packet);
            //SendTCPData(_packet);
        }
    }

    public static void ChatMessage(string _msg)
    {
        using (Packet _packet = new Packet((int)ClientPackets.chatMessage))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_msg);
            SendTCPData(_packet);
        }
    }
}
