using FluentValidation.Results;

namespace NETMockServer.Extensions;

public static class Helpers
{
    // Helpers
    public static object? GetId<T>(T entity)
    {
        var prop = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("ID") ?? typeof(T).GetProperty("id");
        return prop?.GetValue(entity);
    }

    public static IDictionary<string, string[]> ValidationResultToDictionary(ValidationResult validation)
    {
        return validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }

    // Extension for ValidationProblem (used above)
    public static IDictionary<string, string[]> ToDictionary(this ValidationResult validation) =>
        validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}
