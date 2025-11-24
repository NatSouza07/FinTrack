using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

public class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var modelName = context.ModelName;
        var valueProviderResult = context.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            if (context.ModelMetadata.IsNullableValueType)
            {
                context.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
 
            return Task.CompletedTask;
        }

        value = value.Replace("R$", "")
                     .Replace(" ", "")
                     .Replace(".", "");

        value = value.Replace(",", ".");

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            context.Result = ModelBindingResult.Success(parsed);
        }
        else
        {
            context.ModelState.TryAddModelError(modelName, "O valor de moeda digitado não é um número válido.");
        }

        return Task.CompletedTask;
    }
}