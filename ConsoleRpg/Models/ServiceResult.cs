namespace ConsoleRpg.Models;

/// <summary>
/// Represents the result of a service operation.
/// Follows the Result pattern to decouple business logic from UI concerns.
/// Contains success status, user-friendly message, and detailed output.
/// </summary>
public class ServiceResult
{
    // Whether the operation completed successfully</summary>
    public bool Success { get; }

    // Brief message suitable for status display
    public string Message { get; }

    // Detailed output for the main display area
    public string DetailedOutput { get; }

    /// <summary>
    /// Creates a new ServiceResult.
    /// </summary>
    /// <param name="success">Whether the operation succeeded</param>
    /// <param name="message">Brief status message</param>
    /// <param name="detailedOutput">Detailed output (defaults to message if not provided)</param>
    public ServiceResult(bool success, string message, string detailedOutput = null)
    {
        Success = success;
        Message = message;
        DetailedOutput = detailedOutput ?? message;
    }

    /// <summary>
    /// Factory method for successful results.
    /// </summary>
    /// <param name="message">Brief status message</param>
    /// <param name="detailedOutput">Optional detailed output</param>
    /// <returns>A successful ServiceResult</returns>
    public static ServiceResult Ok(string message = "", string detailedOutput = null)
    {
        return new ServiceResult(true, message, detailedOutput);
    }

    /// <summary>
    /// Factory method for failed results.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="detailedOutput">Optional detailed error information</param>
    /// <returns>A failed ServiceResult</returns>
    public static ServiceResult Fail(string message, string detailedOutput = null)
    {
        return new ServiceResult(false, message, detailedOutput);
    }
}

/// <summary>
/// Generic ServiceResult that includes a return value.
/// Used when operations need to return data along with status.
/// Example: MoveToRoom returns the new Room on success.
/// </summary>
/// <typeparam name="T">Type of the return value</typeparam>
public class ServiceResult<T> : ServiceResult
{
    // The value returned by the operation(null/default on failure)
    public T Value { get; }

    private ServiceResult(bool success, T value, string message, string detailedOutput = null)
        : base(success, message, detailedOutput)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method for successful results with a value.
    /// </summary>
    /// <param name="value">The result value</param>
    /// <param name="message">Brief status message</param>
    /// <param name="detailedOutput">Optional detailed output</param>
    /// <returns>A successful ServiceResult with value</returns>
    public static ServiceResult<T> Ok(T value, string message = "", string detailedOutput = null)
    {
        return new ServiceResult<T>(true, value, message, detailedOutput);
    }

    /// <summary>
    /// Factory method for failed results (value will be default).
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="detailedOutput">Optional detailed error information</param>
    /// <returns>A failed ServiceResult with default value</returns>
    public new static ServiceResult<T> Fail(string message, string detailedOutput = null)
    {
        return new ServiceResult<T>(false, default, message, detailedOutput);
    }
}
