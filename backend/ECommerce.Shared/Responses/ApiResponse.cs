namespace ECommerce.Shared.Responses;

/// <summary>
/// Generic API response envelope — standardizes all API outputs.
/// Every endpoint returns this type so the Angular frontend always gets
/// a consistent structure: { success, message, data, errors }.
///
/// Success:  ApiResponse&lt;ProductDto&gt;.Ok(dto)
/// Failure:  ApiResponse&lt;object&gt;.Fail("Something went wrong.")
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    // --- Factory Methods (cleaner than constructors at call sites) ---

    /// <summary>Creates a successful response with data and an optional message.</summary>
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.")
        => new() { Success = true, Message = message, Data = data };

    /// <summary>Creates a failure response with a message and optional validation errors.</summary>
    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
