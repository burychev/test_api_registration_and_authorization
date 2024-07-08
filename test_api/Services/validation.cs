using System.Text.RegularExpressions;

public static class Validation
{
    public static string ValidateEmail(string emailtext)
    {
        Regex regex = new Regex(@"[^@\s]+@[^@\s]+\.[^@\s]+$");
        bool isValid = regex.IsMatch(emailtext);
        if (isValid)
        {
            return "email in correct format";
        }
        else
        {
            return "email is not in the correct format";
        }
    }
}

