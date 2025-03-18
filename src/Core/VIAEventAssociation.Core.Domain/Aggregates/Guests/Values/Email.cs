using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Guests;

public class Email : ValueObject
{
    public string Value { get; }

    internal Email(string value)
    {
        Value = value.ToLower();
    }

    public static OperationResult<Email> Create(string email)
    {
        var regex = new Regex(@"^(?i)[a-zA-Z]{3,4}@via\.dk$|^\d{6}@via\.dk$");
        if (!regex.IsMatch(email))
            return OperationResult<Email>.Failure("InvalidEmail", "Email must be a valid VIA domain email address.");

        return OperationResult<Email>.Success(new Email(email));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}