namespace TKQuake.Engine.Infrastructure.Input
{
    ///<summary>
    ///A command that can be executed.
    ///</summary>
    public interface ICommand
    {
        ///<summary>
        ///Executes the implemented command.
        ///</summary>
        void Execute();
    }
}
