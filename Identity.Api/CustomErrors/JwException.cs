using System;

namespace Identity.Api.CustomErrors;

public class JwtException : Exception
{
    public JwtException()
        : base() { }

    public JwtException(string message)
        : base(message) { }

    public JwtException(string message, Exception innerException)
        : base(message, innerException) { }
}
