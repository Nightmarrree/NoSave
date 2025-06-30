using System;
using NoSave.Services.Interfaces;
using NetFwTypeLib;
using System.Diagnostics;

namespace NoSave.Services
{
    public class FirewallService : IFirewallService
    {
        private INetFwPolicy2 GetFirewallPolicy()
        {
            Type policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            return (INetFwPolicy2)Activator.CreateInstance(policyType);
        }

        public bool IsFirewallEnabled()
        {
            try
            {
                Type managerType = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                INetFwMgr firewallManager = (INetFwMgr)Activator.CreateInstance(managerType);

                INetFwProfile currentProfile = firewallManager.LocalPolicy.CurrentProfile;

                return currentProfile.FirewallEnabled;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking firewall status: {ex.Message}");
                return false;
            }
        }

        public bool CheckFirewallRule(string ruleName)
        {
            if (string.IsNullOrEmpty(ruleName))
                return false;

            try
            {
                INetFwPolicy2 firewallPolicy = GetFirewallPolicy();
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking firewall rule '{ruleName}': {ex.Message}");
                return false;
            }
        }

        public void AddRule(string ruleName, string remoteIp)
        {
            if (CheckFirewallRule(ruleName))
                return;

            try
            {
                Type ruleType = Type.GetTypeFromProgID("HNetCfg.FwRule");
                INetFwRule2 firewallRule = (INetFwRule2)Activator.CreateInstance(ruleType);

                firewallRule.Name = ruleName;
                firewallRule.Enabled = true;
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.RemoteAddresses = remoteIp;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;

                INetFwPolicy2 firewallPolicy = GetFirewallPolicy();
                firewallPolicy.Rules.Add(firewallRule);
                Debug.WriteLine($"Successfully added firewall rule '{ruleName}'.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding firewall rule '{ruleName}': {ex.Message}");
                throw;
            }
        }

        public void RemoveRule(string ruleName)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = GetFirewallPolicy();
                firewallPolicy.Rules.Remove(ruleName);
                Debug.WriteLine($"Successfully removed firewall rule '{ruleName}'.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not remove firewall rule '{ruleName}'. It might not exist. Details: {ex.Message}");
            }
        }
    }
}