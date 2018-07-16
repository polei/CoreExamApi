using System;

namespace CoreExamApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class ExamDomainException : Exception
    {
        public ExamDomainException()
        { }

        public ExamDomainException(string message)
            : base(message)
        { }

        public ExamDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
