using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UpdDemo
{
  class Chat
  {
    public static IPAddress RemoteIpAddress { get; private set; }
    private static int _localPort;

    [Serializable]
    public class KinectJoint
    {
      public string JointType;
      public bool IsTracking;

      public float PositionX;
      public float PositionY;
      public float PositionZ;

      public float RotationX;
      public float RotationY;
      public float RotationZ;
      public float RotationW;

      public byte[] Serialize()
      {
        using (MemoryStream m = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter(m))
          {
            writer.Write(JointType);
            writer.Write(IsTracking);

            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);

            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
            writer.Write(RotationW);
          }
          return m.ToArray();
        }
      }
      public static KinectJoint Desserialize(byte[] data)
      {
        KinectJoint result = new KinectJoint();
        using (MemoryStream m = new MemoryStream(data))
        {
          using (BinaryReader reader = new BinaryReader(m))
          {
            result.JointType = reader.ReadString();
            result.IsTracking = reader.ReadBoolean();

            result.PositionX = reader.ReadSingle();
            result.PositionY = reader.ReadSingle();
            result.PositionZ = reader.ReadSingle();

            result.RotationX = reader.ReadSingle();
            result.RotationY = reader.ReadSingle();
            result.RotationZ = reader.ReadSingle();
            result.RotationW = reader.ReadSingle();
          }
        }
        return result;
      }

      public override string ToString()
      {
        return JointType + 
               " IsTracking (" + IsTracking + 
               ")\nPosition (" + PositionX + "," + PositionY + "," + RotationZ + 
               ")\nRotation (" + RotationX + "," + RotationY + "," + RotationZ + ", " + RotationW + ")";
      }
    }

    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        Console.WriteLine("Set local port");
        _localPort = Convert.ToInt16(Console.ReadLine());

        Console.WriteLine("set IP address");
        RemoteIpAddress = IPAddress.Parse(Console.ReadLine());

        Thread tRec = new Thread(Receiver);
        tRec.Start();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception: " + ex.ToString() + "\n  " + ex.Message);
      }
    }

    public static void Receiver()
    {
      UdpClient receivingUdpClient = new UdpClient(_localPort);
      IPEndPoint remoteIpEndPoint  = null;

      try
      {
        while (true)
        {
          var receiveBytes        = receivingUdpClient.Receive(ref remoteIpEndPoint);
          KinectJoint kinectJoint = KinectJoint.Desserialize(receiveBytes);
          Console.WriteLine(kinectJoint);
          Console.Clear();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception: " + ex.ToString() + "\n  " + ex.Message);
      }
    }
  }
}
