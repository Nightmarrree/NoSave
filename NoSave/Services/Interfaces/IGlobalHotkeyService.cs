using System;
using System.Windows.Input;

namespace NoSave.Services.Interfaces
{
    public interface IGlobalHotkeyService : IDisposable
    {
        event Action HotkeyPressed;

        bool Register(Key key);
    }
}