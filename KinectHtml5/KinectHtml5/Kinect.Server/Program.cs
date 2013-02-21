using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Server
{
    class Program
    {
        static Runtime _nui;
        static List<IWebSocketConnection> _sockets;

        static bool _initialized = false;

        static void Main(string[] args)
        {
            if (Runtime.Kinects.Count <= 0) return;

            InitilizeKinect();
            InitializeSockets();
        }


        /// <summary>
        /// 
        /// </summary>
        private static void InitializeSockets()
        {
            _sockets = new List<IWebSocketConnection>();

            var server = new WebSocketServer("ws://localhost:8181");

            server.Start(socket =>
            {

                socket.OnOpen = () =>
                {
                    Console.WriteLine("Connected to " + socket.ConnectionInfo.ClientIpAddress);
                    _sockets.Add(socket);
                };


                socket.OnClose = () =>
                {
                    Console.WriteLine("Disconnected from " + socket.ConnectionInfo.ClientIpAddress);
                    _sockets.Remove(socket);
                };

                
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                };
            });

            _initialized = true;

            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitilizeKinect()
        {
            _nui = Runtime.Kinects[0];
            _nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
            _nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Nui_SkeletonFrameReady);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (!_initialized) return;

            List<SkeletonData> users = new List<SkeletonData>();

            foreach (var user in e.SkeletonFrame.Skeletons)
            {
                if (user.TrackingState == SkeletonTrackingState.Tracked)
                {
                    users.Add(user);
                }
            }

            if (users.Count > 0)
            {
                string json = users.Serialize();

                foreach (var socket in _sockets)
                {
                    socket.Send(json);
                }
            }
        }
    }
}
