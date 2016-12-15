namespace DotNet.Commands
{
    interface ICommand
    {
        void Execute(CommandContext context);
    }
}