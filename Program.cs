using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    private static bool alertShownPreviously = false;

    static void Main(string[] args)
    {
        Console.WriteLine("Mullvad VPN Controller started...");
        bool vpnDisconnectedDueToGaming = false; // Track if we've disconnected due to gaming

        while (true)
        {
            bool isFortniteRunning = Process.GetProcessesByName("FortniteClient-Win64-Shipping").Length > 0;
            bool isEasyAntiCheatRunning = Process.GetProcessesByName("EasyAntiCheat").Length > 0;
            bool isBattleEyeRunning = Process.GetProcessesByName("BEService").Length > 0;

            // If any gaming-related process is running and the VPN is connected, disconnect the VPN
            if ((isFortniteRunning || isEasyAntiCheatRunning || isBattleEyeRunning) && IsVpnConnected())
            {
                if (!alertShownPreviously)
                {
                    ShowToastNotification("Gaming-related process detected. Disconnecting Mullvad VPN for compliance.");
                    alertShownPreviously = true; // Prevents repeated notifications
                }
                DisconnectMullvad();
                vpnDisconnectedDueToGaming = true; // Indicate that we've disconnected due to gaming
            }
            // If no gaming-related processes are running, the VPN was previously disconnected due to gaming, and the VPN is not currently connected, reconnect the VPN
            else if (!isFortniteRunning && !isEasyAntiCheatRunning && !isBattleEyeRunning && vpnDisconnectedDueToGaming && !IsVpnConnected())
            {
                ReconnectMullvad();
                vpnDisconnectedDueToGaming = false; // Reset the flag as we've handled reconnection
                alertShownPreviously = false; // Reset alert flag for future detections
            }

            Thread.Sleep(10000); // Check every 10 seconds
        }
    }

    static bool IsVpnConnected()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = "-Command \"mullvad status\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        };

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                return result.Contains("Connected");
            }
        }
    }

    static void ShowToastNotification(string message)
    {
        string psScript = @$"
            [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null;
            $template = [Windows.UI.Notifications.ToastTemplateType]::ToastText01;
            $xml = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent($template);
            $texts = $xml.GetElementsByTagName('text');
            $texts[0].AppendChild($xml.CreateTextNode('{message}')) > $null;
            $toast = [Windows.UI.Notifications.ToastNotification]::new($xml);
            $notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier('MullvadVPNController');
            $notifier.Show($toast);
        ";

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{psScript}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process.Start(startInfo);
    }

    static void DisconnectMullvad()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = "-Command \"mullvad disconnect\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        };

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }
        }
    }
    static void ReconnectMullvad()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = "-Command \"mullvad connect\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        };

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine("Mullvad VPN Reconnected: " + result);
            }
        }
    }
}

