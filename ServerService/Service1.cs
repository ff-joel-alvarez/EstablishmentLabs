using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerService
{
    public partial class ServerService : ServiceBase
    {
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);
        SocketServer server;
        public ServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            server = new SocketServer();
            
            Worker = new Thread(()=> {
                server.StartListening();
            });
            Worker.Start();
           
        }

        protected override void OnStop()
        {
            // Signal worker to stop and wait until it does
            StopRequest.Set();
            server = null;
            Worker.Abort();
        }
    }
}
