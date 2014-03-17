DefaultAudioSwitcher
====================

This is a .net app to help automate the process of switching default audio devices. Ideally you would use it via a batch file or a macro.


Credits
====================

I used the toaster code found at [https://github.com/nels-o/toaster](https://github.com/nels-o/toaster). This code was pretty cleanly organized and easy enough to use (after getting the references working). This code uses the [Windows API CodePack](http://archive.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx). I have included the DLLs in this project which I believe is OK as per the License for Windows API CodePack.

This code also makes use of [AudioEndPointController](https://github.com/promythyus/AudioEndPointController). This code was actually initially from [Dave Amenta's blog](http://www.daveamenta.com/2011/05/). AudioEndPointController simply makes it a C++ DLL to which I p/invoke. 

About
====================
DefaultAudioSwitcher is used to cycle/toggle the current audio device. This is great for people who use headphones and keep them plugged in all the time. I currently use it with Astro Headphones plugged in via an optical cable for instance. Its very hidden and does not run at all until you toggle it. Then it changes the device, pops up a toast and closes. Currently it only supports Windows 8 (tested on Windows 8.1) due to the toaster. This could be easily modified (remove the toaster feature) if you want it to support Windows 7.

This app is a simple headless app that can be ran via command prompt or any other means. It has three primary ways to use it.

* No inputs (command line args) will print the list of devices to the console.
* 2 integer values via the command line will toggle between the devices. Old deprecated method really and the 3rd option is better.
* Modify the DefaultAudioSwitcher.exe.config file. It currently has 2 devices added. You can add more as you wish. Keep the keys unique and the value is the description to print in the toast popup. The keys should either be the device name (found in the output with no inputs) or it should be the ID of the device (also found in that output). This method allows more than 2 devices and should toggle through them.

I currently have a macro set on my keyboard that when I press the macro key it simply runs the command and cycles my inputs. 