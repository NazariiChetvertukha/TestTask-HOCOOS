using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Tasks;

public class FieldsExpressionBackendTask(ILogger<FieldsExpressionBackendTask> logger) : ExpressionBackendTask
{
    protected override async Task<IQueryable<T>> LoadRecords<T>(params string[] fields)
    {
        var records = await base.LoadRecords<T>(fields);

        if (fields == null || fields.Length == 0)
        {
            return records;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var bindings = fields.Select(field =>
        {
            var property = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{field}' not found on type '{typeof(T)}'");
            }

            return Expression.Bind(property, Expression.Property(parameter, property));
        });

        var newExpression = Expression.MemberInit(Expression.New(typeof(T)), bindings);
        var lambda = Expression.Lambda<Func<T, T>>(newExpression, parameter);

        return records.Select(lambda);
    }

    protected override void WriteRecord<T>(T record, params string[] fields)
    {
        if (fields == null || fields.Length == 0)
        {
            logger.LogInformation("Record {RecordId}: {Record}", record.Id, record);
            return;
        }

        var fieldMessages = fields.Select(field =>
        {
            var property = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
            return property != null ? $"{field} = {property.GetValue(record)}" : $"{field} = null";
        });

        logger.LogInformation("Record {RecordId}: {FieldMessages}", record.Id, string.Join("; ", fieldMessages));
    }
}