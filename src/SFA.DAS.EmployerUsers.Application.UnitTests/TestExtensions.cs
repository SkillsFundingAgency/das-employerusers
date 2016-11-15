namespace SFA.DAS.EmployerUsers.Application.UnitTests
{
    public static class TestExtensions
    {
        public static string ToInverseCase(this string value)
        {
            var array = new char[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                array[i] = char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);
            }
            return new string(array);
        }
    }
}
