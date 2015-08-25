using System;
using TKQuake.Engine.Infrastructure.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TKQuake.Engine.Infrastructure.Input
{
    ///<summary>
    ///A factory class for generating commands to be executed.
    ///</summary>
    public static class CommandFactory
    {
        public static ICommand Create(Type commandType, params object[] args)
        {
            //make sure commandType implements ICommand
            System.Diagnostics.Debug.Assert(
                commandType.GetInterfaces().Any(i => i.Name == "ICommand"));

            return (ICommand)System.Activator.CreateInstance(commandType, args);
        }
    }
}
