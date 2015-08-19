using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Input;

namespace TKQuake.Engine.Infrastructure.Input
{
    ///<summary>
    ///The InputSystem is used to process user input from
    ///keyboard and mouse button presses.
    ///</summary>
    public class InputSystem
    {
        ///<summary>
        ///A map of keys to command objects.
        ///</summary>
        public IDictionary<Key, ICommand> KeyboardMap;

        ///<summary>
        ///A map of mouse buttons to command objects.
        ///</summary>
        public IDictionary<MouseButton, ICommand> MouseButtonMap;

        ///<summary>
        ///Create an instance of the InputSystem class. If configs aren't
        ///provided a default configuration will be used.
        ///</summary>
        ///<param name="keyboardConfig">
        ///A custom map of keys to commands
        ///</param>
        ///<param name="mouseConfig">
        ///A custom map of mouse buttons to commands
        ///</param>
        public InputSystem(IDictionary<Key, ICommand> keyboardConfig = null,
                           IDictionary<MouseButton, ICommand> mouseConfig = null)
        {
            KeyboardMap = keyboardConfig ?? new Dictionary<Key, ICommand>();
            MouseButtonMap = mouseConfig ?? new Dictionary<MouseButton, ICommand>();
        }

        ///<summary>
        ///Executes the registered command for the given keyboard button.
        ///</summary>
        public void ProcessKeyboardInput(Key key)
        {
            if (KeyboardMap.ContainsKey(key))
                KeyboardMap[key].Execute();
        }

        ///<summary>
        ///Executes the registered command for the given mouse button.
        ///</summary>
        public void ProcessMouseInput(MouseButton button)
        {
            if (MouseButtonMap.ContainsKey(button))
                MouseButtonMap[button].Execute();
        }
    }
}
