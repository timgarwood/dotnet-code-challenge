using System;

namespace CodeChallenge.Exceptions
{
    /// <summary>
    /// An exception throw when a Compensation is
    /// created for an Employee that already has a Compensation associated
    /// </summary>
    public class CompensationAlreadyExistsException : Exception
    {

    }
}
