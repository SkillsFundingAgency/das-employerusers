namespace SFA.DAS.EmployerProfiles.Data.UnitTests.DatabaseMock;

public class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator = enumerator ?? throw new ArgumentNullException();

    public T Current => _enumerator.Current;

    public ValueTask DisposeAsync()
    {
        _enumerator.Dispose();
        return new ValueTask();
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_enumerator.MoveNext());
    }
}