namespace TodoSeUsa.Application.Common.Events;

public sealed class AppEvents
{
    public event Action? LiquidationsChanged;

    public void RaiseLiquidationsChanged()
    {
        LiquidationsChanged?.Invoke();
    }
}
