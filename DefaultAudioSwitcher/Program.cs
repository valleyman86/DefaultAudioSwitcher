using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ShellHelpers;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.IO;
using System.Configuration;


namespace DefaultAudioSwitcher
{
    class Program
    {
        private const String APP_ID = "MurderDev.DefaultAudioSwitcher";
        static void Main(string[] args)
        {
            TryCreateShortcut("DefaultAudioSwitcher");

            //Sort the appsettings so that we have an ordered list. Using the NameValueCollection(AppSettings) directly could be unpredictable.
            string[] appSettings = ConfigurationManager.AppSettings.AllKeys.OrderBy(key => key).ToArray();

            Dictionary<string, int> deviceMap = new Dictionary<string, int>();

            //Print the devices and create a lookup map.
            for (int i = 0; i < EndPointController.GetDeviceCount(); ++i) {
                deviceMap.Add(EndPointController.GetDeviceFriendlyName(i), i);
                deviceMap.Add(EndPointController.GetDeviceID(i), i);

                Console.WriteLine(string.Format("Audio Device {0}: {1} ID: {2}", i, EndPointController.GetDeviceFriendlyName(i), EndPointController.GetDeviceID(i)));
            }

            //If a config exist use that.
            if (args.Length == 0 && appSettings.Length > 0) {
                string firstDeviceName =  appSettings[0];
                string newDeviceName = (string)Application.UserAppDataRegistry.GetValue("defaultDeviceName", firstDeviceName);

                int index = Array.IndexOf(appSettings, newDeviceName);
                ++index;

                if (index >= appSettings.Length) {
                    index = Array.IndexOf(appSettings, firstDeviceName);
                }

                if (deviceMap.ContainsKey(appSettings[index])) {
                    Application.UserAppDataRegistry.SetValue("defaultDeviceName", appSettings[index]);

                    EndPointController.SetDefaultAudioPlaybackDevice(EndPointController.GetDeviceID(deviceMap[appSettings[index]]));

                    ToastNotification toast = ShowToast(ConfigurationManager.AppSettings[appSettings[index]]);
                }
            }

            //If arguments are passed in use those instead.
            if (args.Length == 2) {
                string device1 = args[0];
                string device2 = args[1];

                string newDeviceID = (string)Application.UserAppDataRegistry.GetValue("defaultDevice", device1);

                if (newDeviceID == device1)
                    newDeviceID = device2;
                else
                    newDeviceID = device1;

                Application.UserAppDataRegistry.SetValue("defaultDevice", newDeviceID);

                EndPointController.SetDefaultAudioPlaybackDevice(EndPointController.GetDeviceID(Convert.ToInt32(newDeviceID)));

                ToastNotification toast = ShowToast(EndPointController.GetDeviceFriendlyName(Convert.ToInt32(newDeviceID)));
            }
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. You must download and include the 
        // Windows API Code Pack for Microsoft .NET Framework for this code to function
        private static bool TryCreateShortcut(String shortcutName)
        {
            String shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\" + shortcutName + ".lnk";
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
                return true;
            }
            return false;
        }

        private static void InstallShortcut(String shortcutPath)
        {
            // Find the path to the current executable
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(APP_ID))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }

        private static ToastNotification ShowToast(String message)
        {
            return ShowToast(null, message, null);
        }

        private static ToastNotification ShowToast(String title, String message)
        {
            return ShowToast(title, message, null);
        }

        // Create and show the toast.
        // See the "Toasts" sample for more detail on what can be done with toasts
        private static ToastNotification ShowToast(String title, String message, String imageURI)
        {
            if (message == null) return null;
            // Get a toast XML template
            XmlDocument toastXml;

            //// Specify the absolute path to an image
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            if (imageURI != null)
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = "file:///" + imageURI;
                if (title != null)
                {
                    // Fill in the text elements
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    stringElements[0].AppendChild(toastXml.CreateTextNode(title));
                    stringElements[1].AppendChild(toastXml.CreateTextNode(message));
                }
                else
                {
                    // Fill in the text elements
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    stringElements[0].AppendChild(toastXml.CreateTextNode(message));
                }
            }
            else if (title != null)
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                // Fill in the text elements
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(title));
                stringElements[1].AppendChild(toastXml.CreateTextNode(message));
            }
            else
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                // Fill in the text elements
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(message));
            }
            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
            return toast;
        }

        private static void ToastActivated(ToastNotification sender, object e)
        {
            Console.WriteLine("Activated");
            Environment.Exit(0);
        }

        private static void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";
            int exitCode = -1;
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "Hidden";
                    exitCode = 1;
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "Dismissed";
                    exitCode = 2;
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "Timeout";
                    exitCode = 3;
                    break;
            }
            Console.WriteLine(outputText);
            Environment.Exit(exitCode);
        }

        private static void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Console.WriteLine("Error.");
            Environment.Exit(-1);
        }
    }
}
