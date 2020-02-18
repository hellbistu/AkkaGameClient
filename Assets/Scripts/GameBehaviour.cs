using System;

using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using Protogen;
using UnityEngine;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{

    private BlockQueue<Packet> recvQueue = new BlockQueue<Packet>(100);

    private BlockQueue<Packet> sendQueue = new BlockQueue<Packet>(100);

    private Socket socket;
    
    public InputField acidField;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Login()
    {
        long acid = long.Parse(acidField.text);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1",28993);
        
        new Thread(RecvLoop).Start();
        new Thread(SendLoop).Start();

        //与服务器握手
        HandleShake handleShake = new HandleShake();
        handleShake.Acid = acid;
        Send(handleShake);
    }

    public void Send(IMessage msg)
    {
       byte[] pbytes = msg.ToByteArray();
       
       //TODO 用反射
       short ptype = 0;
       if (msg is HandleShake)
       {
           ptype = 1;
       }
       
       Packet packet = new Packet(ptype,pbytes);
       sendQueue.EnQueue(packet);
    }
    
    private void RecvLoop()
    {
        Debug.Log("start recv thread");
        
        while (true)
        {
            byte[] ptypeBytes = new byte[2];
            byte[] lengthBytes = new byte[4];


             readFull(ptypeBytes);
             readFull(lengthBytes);

            short ptype = BitConverter.ToInt16(ptypeBytes,0);
            int length = BitConverter.ToInt16(lengthBytes,0);

            byte[] pdata = new byte[length];
            readFull(pdata);
            
            Packet packet = new Packet(ptype,pdata);
            recvQueue.EnQueue(packet);
        }
    }

    private void readFull(byte[] buffer)
    {
        int r = 0;
        int remainBytesToRead = buffer.Length;
        
        while ((r = socket.Receive(buffer, buffer.Length - remainBytesToRead, remainBytesToRead, SocketFlags.None)) > 0)
        {
            remainBytesToRead -= r;
            if (remainBytesToRead <= 0)
            {
                break;
            }
        }
    }
    
    private void SendLoop()
    {
        Debug.Log("start send thread");
        while (true)
        {
            Packet packet = sendQueue.DeQueue();
            Debug.Log("send packet=" + packet.GetPtype());
            socket.Send(packet.Marshal());
        }
    }
}
