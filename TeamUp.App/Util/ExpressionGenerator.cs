
using System.Linq.Expressions;
using Models;

namespace Utils;
public static class ExpressionGenerator
{
    public static Expression<Func<Project, bool>> GenerateTeamSizeFilterExpression(string[] filters)
    {
        var parameter = Expression.Parameter(typeof(Project), "p");
        var property = Expression.Property(parameter, "TeamSize");

        Expression? filterExpression = null;

        foreach (var filter in filters)
        {
            var range = filter.Split('-');
            if (range.Length == 1)
            {
                int minValue = int.Parse(range[0].Trim('+'));
                var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, Expression.Constant(minValue));
                filterExpression = filterExpression == null ? greaterThanOrEqual : Expression.OrElse(filterExpression, greaterThanOrEqual);
            }
            else if (range.Length == 2)
            {
                int minValue = int.Parse(range[0]);
                int maxValue = int.Parse(range[1]);
                var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, Expression.Constant(minValue));
                var lessThanOrEqual = Expression.LessThanOrEqual(property, Expression.Constant(maxValue));
                var rangeExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
                filterExpression = filterExpression == null ? rangeExpression : Expression.OrElse(filterExpression, rangeExpression);
            }
            else
            {
                throw new ArgumentException("Invalid filter range format.");
            }
        }

        if (filterExpression == null)
        {
            throw new ArgumentException("At least one valid filter range is required.");
        }

        var lambda = Expression.Lambda<Func<Project, bool>>(filterExpression, parameter);
        return lambda;
    }
}
