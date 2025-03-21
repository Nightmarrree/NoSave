﻿using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NoSave
{
    public static class Utilities
    {
        public static void SwitchFirewallRule()
        {
            SetFirewall(!CheckFirewallRule());
        }

        public static void SetFirewall(bool state)
        {
            if (CheckFirewallRule() == state)
                return;

            if (state) 
                EnableNoSavingMode(); 
            else 
                DisableNoSavingMode();
        }

        public static async Task StatusChecker(Button button)
        {
            while (true)
            {
                bool ruleExists = CheckFirewallRule();

                await button.Dispatcher.InvokeAsync(() =>
                    button.Content = ruleExists ? "Unblock R*" : "Block R*"
                );

                await Task.Delay(200);
            }
        }

        public static bool IsFirewallEnabled()
        {
            string output = Cmd("powershell -Command \"(Get-NetFirewallProfile).Enabled\"");
            return !output.Contains("False");
        }
        public static bool CheckFirewallRule()
        {
            App app = Application.Current as App;
            string output = Cmd($"netsh advfirewall firewall show rule name=\"{app.ruleName}\"");
            return output.Contains(app.ruleName);
        }

        public static void EnableNoSavingMode()
        {
            App app = Application.Current as App;
            string output = Cmd($"netsh advfirewall firewall add rule name=\"{app.ruleName}\" dir=out action=block remoteip={app.remoteIP} enable=yes");
        }

        public static void DisableNoSavingMode()
        {
            App app = Application.Current as App;
            string output = Cmd($"netsh advfirewall firewall delete rule name=\"{app.ruleName}\"");
        }

        public static string Cmd(string line)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {line}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return output;
            }
        }
    }
}
