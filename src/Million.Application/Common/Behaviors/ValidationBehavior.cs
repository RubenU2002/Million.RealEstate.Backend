using FluentValidation;
using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;

namespace Million.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(f => f.ErrorMessage)
                .ToList();

            if (failures.Any())
            {
                var errors = failures.Select(error => new Error(error)
                    .WithMetadata("StatusCode", 400)
                    .WithMetadata("ErrorType", ResultExtensions.ErrorTypes.ValidationError));

                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var failMethod = typeof(Result).GetMethods()
                        .Where(m => m.Name == "Fail" && m.IsGenericMethodDefinition)
                        .Where(m => m.GetParameters().Length == 1 && 
                               m.GetParameters()[0].ParameterType == typeof(IEnumerable<IError>))
                        .FirstOrDefault();
                    
                    if (failMethod != null)
                    {
                        var genericType = typeof(TResponse).GetGenericArguments()[0];
                        var genericFailMethod = failMethod.MakeGenericMethod(genericType);
                        return (TResponse)genericFailMethod.Invoke(null, new object[] { errors })!;
                    }
                }
                else
                {
                    return (TResponse)(object)Result.Fail(errors);
                }
            }
        }

        return await next();
    }
}
