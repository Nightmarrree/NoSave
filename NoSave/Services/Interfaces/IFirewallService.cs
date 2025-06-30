namespace NoSave.Services.Interfaces
{
    public interface IFirewallService
    {
        bool IsFirewallEnabled();
        bool CheckFirewallRule(string ruleName);
        void AddRule(string ruleName, string remoteIp);
        void RemoveRule(string ruleName);
    }
}