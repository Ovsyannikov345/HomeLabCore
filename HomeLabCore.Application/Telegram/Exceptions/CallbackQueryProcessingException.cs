namespace HomeLabCore.Application.Telegram.Exceptions;

public sealed class CallbackQueryProcessingException : Exception
{
    public bool ShowMessageToUser { get; }

    public CallbackQueryProcessingException(string message, bool showToUser = false) : base(message, null)
    {
        ShowMessageToUser = showToUser;
    }
}
