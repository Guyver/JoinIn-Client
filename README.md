JoinIn-Client
=============

This is a research client side application that streams kinect data via localhost to the browser.
The SDK that needs to be used is [Microsofts Beta SDK](http://www.microsoft.com/en-us/download/details.aspx?id=27876)
You can follow the instructions from that portal which also has the system requirements there.

###Software Requirements

* Microsoft� Visual Studio� 2010 Express or other Visual Studio 2010 edition __To compile__
* NET Framework 4.0

###Hardware Requirements

*32 bit (x86) or 64 bit (x64) processor


*Dual-core 2.66-GHz or faster processor


*Dedicated USB 2.0 bus


*2 GB RAM


*A retail Kinect __for Xbox 360� sensor__, which includes special USB/power cabling


###Use with Join-In applications

1. Uninstall all previous drivers that are associated with the Kinect, Primesense / OpenNi.   

2. Unplug the Kinect if not unplugged already.  

3. Install the Kinect SDK from the link above. 

4. Plug in the Kinect via usb.

5. Test that the drivers have been installed correctly by running the sample skeleton viewer application that comes with the SDK.

6. Compile the application in VS2010 if there isn't an .exe in the \KinectServer\bin folder. __Note:__ Once compiled you can just run the .exe file or start it as a process on PC start up.

7. Open the game in the browser, chrome, once the application runs.

###Known bugs

* The need for recompiling, this is being worked on at the minute and will eventually just upload the executable and script to run on start up.

* If there is any remaining drivers on the PC before the SDK installation is present it wont find the device when ran. Uninstall all drivers and start again.

