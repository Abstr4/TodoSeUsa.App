namespace TodoSeUsa.BlazorServer.UI.State;

public sealed class RecoveryCodeSession
{
    private string? _plainCode;

    public void Set(string code) => _plainCode = code;

    public bool TryTake(out string code)
    {
        if (_plainCode is null)
        {
            code = string.Empty;
            return false;
        }

        code = _plainCode;
        _plainCode = null;
        return true;
    }
}