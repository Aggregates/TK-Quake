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
    public static class CommandCentre
    {
        private static readonly Queue<KeyValuePair<ICommand, IEntity>>
            _commandStream = new Queue<KeyValuePair<ICommand, IEntity>>();

        public static void PushCommand(ICommand command, IEntity entity)
        {
            var kvp = new KeyValuePair<ICommand, IEntity>(command, entity);
            _commandStream.Enqueue(kvp);
        }

        public static void ExecuteAllCommands()
        {
            while (_commandStream.Any())
            {
                var kvp = _commandStream.Dequeue();
                var command = kvp.Key;
                var entity = kvp.Value;

                command.Execute(entity);
            }
        }
    }
}
