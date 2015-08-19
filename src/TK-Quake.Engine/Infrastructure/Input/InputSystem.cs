using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Input;

namespace TKQuake.Engine.Infrastructure.Input
{
    public class InputSystem
    {
        ///<summary>
        ///A map of keys to command objects.
        ///</summary>
        public IDictionary<Key, ICommand> KeyboardMap;
        public IDictionary<MouseButton, ICommand> MouseButtonMap;

        public InputSystem(IDictionary<Key, ICommand> kbConfig = null,
                           IDictionary<MouseButton, ICommand> mConfig = null)
        {
            KeyboardMap = kbConfig ?? new Dictionary<Key, ICommand>();
            MouseButtonMap = mConfig ?? new Dictionary<MouseButton, ICommand>();
        }

        public void ProcessKeyboardInput(Key key)
        {
            if (KeyboardMap.ContainsKey(key))
                KeyboardMap[key].Execute();
        }

        public void ProcessMouseInput(MouseButton button)
        {
            if (MouseButtonMap.ContainsKey(button))
                MouseButtonMap[button].Execute();
        }
    }
}
