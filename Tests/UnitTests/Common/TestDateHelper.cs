namespace UnitTests.Common;

public class TestDateHelper
{
    public static DateTime GetFutureDate()
    {
        return DateTime.Now.AddYears(1);
    }
    public static DateTime CurrentDate()
    {
        return DateTime.Now;
    }
}