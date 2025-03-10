using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Guests;

public class Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static OperationResult<Name> Create(string name)
    {
        if (name.Length < 2 || name.Length > 25)
            return OperationResult<Name>.Failure("InvalidName", "Name must be between 2 and 50 characters.");
        
        if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            return OperationResult<Name>.Failure("InvalidName", "Name can only contain letters from a-z and A-Z.");

        var nameParts = name.Split(' ');
        for (int i = 0; i < nameParts.Length; i++)
        {
            if (nameParts[i].Length > 0)
                nameParts[i] = char.ToUpper(nameParts[i][0]) + nameParts[i].Substring(1).ToLower();
        }

        return OperationResult<Name>.Success(new Name(string.Join(" ", nameParts)));
    }
}