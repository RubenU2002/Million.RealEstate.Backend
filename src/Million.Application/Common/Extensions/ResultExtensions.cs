using FluentResults;

namespace Million.Application.Common.Extensions;

public static class ResultExtensions
{
    public static class ErrorTypes
    {
        public const string Unauthorized = nameof(Unauthorized);
        public const string NotFound = nameof(NotFound);
        public const string BadRequest = nameof(BadRequest);
        public const string Forbidden = nameof(Forbidden);
        public const string InternalServerError = nameof(InternalServerError);
        public const string ValidationError = nameof(ValidationError);
    }

    public static Result<T> WithError<T>(string message, int statusCode, string errorType)
    {
        return Result.Fail(new Error(message)
            .WithMetadata("StatusCode", statusCode)
            .WithMetadata("ErrorType", errorType));
    }

    public static Result WithError(string message, int statusCode, string errorType)
    {
        return Result.Fail(new Error(message)
            .WithMetadata("StatusCode", statusCode)
            .WithMetadata("ErrorType", errorType));
    }

    public static Result<T> Unauthorized<T>(string message = "Unauthorized") => 
        WithError<T>(message, 401, ErrorTypes.Unauthorized);

    public static Result Unauthorized(string message = "Unauthorized") => 
        WithError(message, 401, ErrorTypes.Unauthorized);

    public static Result<T> NotFound<T>(string message = "Not found") => 
        WithError<T>(message, 404, ErrorTypes.NotFound);

    public static Result NotFound(string message = "Not found") => 
        WithError(message, 404, ErrorTypes.NotFound);

    public static Result<T> BadRequest<T>(string message = "Bad request") => 
        WithError<T>(message, 400, ErrorTypes.BadRequest);

    public static Result BadRequest(string message = "Bad request") => 
        WithError(message, 400, ErrorTypes.BadRequest);

    public static Result<T> Forbidden<T>(string message = "Forbidden") => 
        WithError<T>(message, 403, ErrorTypes.Forbidden);

    public static Result Forbidden(string message = "Forbidden") => 
        WithError(message, 403, ErrorTypes.Forbidden);

    public static Result<T> InternalServerError<T>(string message = "Internal server error") => 
        WithError<T>(message, 500, ErrorTypes.InternalServerError);

    public static Result InternalServerError(string message = "Internal server error") => 
        WithError(message, 500, ErrorTypes.InternalServerError);

    public static Result<T> ValidationError<T>(string message) => 
        WithError<T>(message, 400, ErrorTypes.ValidationError);

    public static Result ValidationError(string message) => 
        WithError(message, 400, ErrorTypes.ValidationError);

    public static Result<T> ValidationError<T>(IEnumerable<string> errors) => 
        Result.Fail<T>(errors.Select(error => new Error(error)
            .WithMetadata("StatusCode", 400)
            .WithMetadata("ErrorType", ErrorTypes.ValidationError)));

    public static Result ValidationError(IEnumerable<string> errors) => 
        Result.Fail(errors.Select(error => new Error(error)
            .WithMetadata("StatusCode", 400)
            .WithMetadata("ErrorType", ErrorTypes.ValidationError)));

    public static int GetStatusCode(this ResultBase result)
    {
        if (result.IsSuccess) return 200;

        var firstError = result.Errors.FirstOrDefault();
        if (firstError?.Metadata.TryGetValue("StatusCode", out var statusCode) == true)
        {
            return (int)statusCode;
        }

        return firstError?.Metadata.TryGetValue("ErrorType", out var errorType) == true
            ? errorType.ToString() switch
            {
                ErrorTypes.Unauthorized => 401,
                ErrorTypes.NotFound => 404,
                ErrorTypes.BadRequest => 400,
                ErrorTypes.Forbidden => 403,
                ErrorTypes.InternalServerError => 500,
                ErrorTypes.ValidationError => 400,
                _ => 400
            }
            : 400;
    }

    public static string GetFirstErrorMessage(this ResultBase result)
    {
        if (!result.Errors.Any()) return "An error occurred";
        
        var validationErrors = result.Errors
            .Where(e => e.Metadata.ContainsKey("ErrorType") && 
                       e.Metadata["ErrorType"].ToString() == ErrorTypes.ValidationError)
            .ToList();
            
        if (validationErrors.Count > 1)
        {
            return string.Join("; ", validationErrors.Select(e => e.Message));
        }
        
        return result.Errors.First().Message;
    }
}
