namespace SFA.DAS.EmployerProfiles.Domain.RequestHandlers;

public class ValidationResult
{
    public Dictionary<string, string> ValidationDictionary { get; }
    private IEnumerable<string> ErrorList => BuildErrorList();
    public System.ComponentModel.DataAnnotations.ValidationResult DataAnnotationResult 
    {
        get
        {
            var errorMessages = ErrorList.Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");

            return new System.ComponentModel.DataAnnotations.ValidationResult(
                $"The following parameters have failed validation{Environment.NewLine}{errorMessages}",
                ErrorList);
        }
    }

    public ValidationResult()
    {
        ValidationDictionary = new Dictionary<string, string>();
    }

    public void AddError(string propertyName)
    {
        ValidationDictionary.Add(propertyName, $"{propertyName} has not been supplied");
    }

    public void AddError(string propertyName, string validationError)
    {
        ValidationDictionary.Add(propertyName, validationError);
    }

    public bool IsValid()
    {
        if (ValidationDictionary == null)
        {
            return false;
        }

        return !ValidationDictionary.Any();
    }

    private List<string> BuildErrorList()
    {
        return ValidationDictionary.Select(c => c.Key + "|" + c.Value).ToList();
    }
}