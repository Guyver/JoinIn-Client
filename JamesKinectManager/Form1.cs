using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Fleck;
using Microsoft.Research.Kinect.Nui;

namespace JamesKinectManager
{
    public partial class Form1 : Form
    {
        // Are we listening at the minute?
        private bool mListening = false;

        // Port to listen to.
        private string mPort = "8000";

        // Ip address to listen to.
        private string mIpaddress = "127.0.0.1";

        // Microsofts jazz.
        Runtime mNui = new Runtime();

        Device mDevices = new Device();

        // A list of sockets that are connected.
        private List<IWebSocketConnection> mSockets;

        // Has the device been initalised.
        private bool mInitializedKinect = false;

        // Has the device been initalised.
        private bool mInitialisedSockets = false;

        public Form1()
        {
            InitializeComponent();

            // Timer set to 60fps.
            timer1.Interval = 1000;
            timer1.Start();
        }


        /// <summary>
        /// Create the server from the ip and port entered by the user.
        /// Defines the OnOpen, OnClose and OnMessage functions for fleck.
        /// </summary>
        private void InitializeSockets()
        {
            mSockets = new List<IWebSocketConnection>();

            try
            {
                var server = new WebSocketServer("ws://" + mIpaddress + ":" + mPort);

                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        lb_log.Items.Add("Connected to client : " + socket.ConnectionInfo.ClientIpAddress);
                        mSockets.Add(socket);
                    };


                    socket.OnClose = () =>
                    {
                        lb_log.Items.Add("Disconnected from client : " + socket.ConnectionInfo.ClientIpAddress);
                        mSockets.Remove(socket);
                    };


                    socket.OnMessage = message =>
                    {
                        lb_log.Items.Add("Message from client : " + message);
                    };
                });

                lb_log.Items.Add("Server is initalised");
                lb_log.Items.Add("Waiting to recieve data on : " + server.Location);

                mInitialisedSockets = true;
            }
            catch(Exception e)
            {
                lb_log.Items.Add("Failed to initalise the server.");
                lb_log.Items.Add("The error was : " + e.Message );
            }
        }


        /// <summary>
        /// Initalise the kinect and set it to track a skeleton.
        /// Provide a callback function that will fire when our skeleton is ready each frame.
        /// </summary>
        private void InitilizeKinect()
        {
            if (mDevices.Count <= 0)
            {
                lb_log.Items.Add("There are no Kinects detected.");
            }

            try
            {
                // Initalise the kinect to track multiple skeletons.
                mNui.Initialize( RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking );

                // Add a new callback event to frame ready.
                mNui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>( Nui_SkeletonFrameCallback );

                // We're ready to go
                mInitializedKinect = true;
                lb_log.Items.Add("Kinect is initalised.");
            }
            catch(Exception e)
            {
                // Error reporting.
                lb_log.Items.Add( "Error code : "+e.Message );
                lb_log.Items.Add( "Try plugging in the Kinect and make sure the drivers/SDK is installed." );
            }
        }


        /// <summary>
        /// Triggered from inside the Kinect library each time a skeleton frame is ready. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Nui_SkeletonFrameCallback(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (!mInitializedKinect) return;

            List<SkeletonData> users = new List<SkeletonData>();

            foreach (var user in e.SkeletonFrame.Skeletons)
            {
                if ( user.TrackingState == SkeletonTrackingState.Tracked )
                {
                    users.Add( user );
                }
            }

            if (users.Count > 0)
            {
                string json = users.ToString();

                foreach (var socket in mSockets)
                {
                    socket.Send(json);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_Ip_TextChanged(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_Port_TextChanged(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Run_Click(object sender, EventArgs e)
        {
            // Initalise the server.
            if ( !mListening && !mInitialisedSockets )
            {
                // Store the IP address.
                mIpaddress = tb_Ip.Text;
                // Store the Port.
                mPort = tb_Port.Text;

                // Initalise the server using the data above.
                InitializeSockets();                

                // We're listening for data.
                mListening = true;

                // Change the button to stop.
                btn_Run.Text = "Stop";
            }
            else
            {
                Application.Exit();
            }

            // Initalise the Kinect device.
            if( !mInitializedKinect )
            {
                InitilizeKinect();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mDevices.Count <= 0)
            {
                lb_log.Items.Add("There are no Kinects detected. Did you unplug the kinect?");
                mInitializedKinect = false;
            }
        }
    }
}
