using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Configuration;

namespace ServerService
{
    //Part of the code  was taken from the  Microsoft Documentation

    // State object for reading client data asynchronously  
   
    public class Info
    {
        public const int DATA_FRAME_SIZE = 4;
        public const int COMMAND_TYPE_FRAME_SIZE = 4;
    }

    public class SocketServer
    {
        // Thread signal.  Like a semaphore
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private ServiceConfig serviceConfig = new ServiceConfig();


        public SocketServer() { }

        public void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(200);

                while (true)
                {
                    allDone.Reset();
                    Library.WriteLine("Waiting for a connection...");

                    listener.BeginAccept(
                           new AsyncCallback(AcceptCallback),
                           listener
                       );
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Library.WriteLine(e.ToString());
            }
            Library.WriteLine("\nPress Enter to continue");

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);


            StateObject state = new StateObject
            {
                workSocket = handler
            };

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);


        }

        private void SendImages() {
            Library.WriteLine("Sending....");
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);           
            PackageMessage packageMessage = bytesRead > 0 ? new PackageMessage(state.buffer, Info.DATA_FRAME_SIZE, Info.COMMAND_TYPE_FRAME_SIZE) : null;
            //check if all data  was read
            if (packageMessage != null && Info.COMMAND_TYPE_FRAME_SIZE + Info.DATA_FRAME_SIZE + packageMessage.DataLength == bytesRead) //each time a image is sent?
            {

                //Check for command type
                switch (packageMessage.CommandType) {
                    case Command.ChangeImagePerSecondQuantity:
                        break;
                    default: break;
                }


                //TODO: Send X images per second

                byte[] bmpBytes = new byte[0];
                try
                {
                    bmpBytes = File.ReadAllBytes(ConfigurationManager.AppSettings["imagesPath"] + "\\background.jpg");

                }
                catch (Exception e)
                {
                    Library.WriteLine(e.ToString());

                }

                Library.WriteLine($"B {bytesRead} Decoded Data Length: " +
                    $"{packageMessage.DataLength} \n Decoded Command Type: " +
                    $"{packageMessage.CommandType} \n Decoded Data: {packageMessage.Data} ");


                // Echo the data back to the client.  
                Send(handler, bmpBytes);
               
            }


        }

        private void Send(Socket handler, byte[] data)
        {

            handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallBack), handler);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Library.WriteLine($"Sent {bytesSent} bytes to client.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Library.WriteLine(e.ToString());

            }
        }

    }


}
