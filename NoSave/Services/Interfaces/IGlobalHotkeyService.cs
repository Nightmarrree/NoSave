using System;
using System.Windows.Input;

namespace NoSave.Services.Interfaces
{
    public interface IGlobalHotkeyService
    {
        event Action HotkeyPressed;

        bool Register(Key key);
    }
}