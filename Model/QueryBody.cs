
using System.Linq.Expressions;

namespace MovieAPI.Model;
public class QueryBody
{
    public string? Title { get; set; }
    public string? Director { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
    public Expression<Func<T, bool>> ToPredicate<T>()
    {
        var parameter = Expression.Parameter(typeof(T), "mov");
        Expression? predicate = null;
        if (!string.IsNullOrEmpty(Title))
        {
            var titleProp = Expression.Property(parameter, "Title");
            var titleToLower = Expression.Call(titleProp, nameof(string.ToLower), null);
            var titleVal = Expression.Constant(Title.ToLower());
            var contains = Expression.Call(titleToLower, nameof(String.Contains), null, titleVal);
            predicate = predicate == null ? contains : Expression.AndAlso(predicate, contains);
        }
        if (!string.IsNullOrEmpty(Director))
        {
            var dirProp = Expression.Property(parameter, "Director");
            var dirVal = Expression.Constant(Director.ToLower());
            var dirToLower = Expression.Call(dirProp, nameof(string.ToLower), null);
            var contains = Expression.Call(dirToLower, nameof(String.Contains), null, dirVal);
            predicate = predicate == null ? contains : Expression.AndAlso(predicate, contains);
        }
        if (From.HasValue)
        {
            var releaseProp = Expression.Property(parameter, "ReleaseYear");
            var fromVal = Expression.Constant(From.Value);
            var greaterThan = Expression.GreaterThanOrEqual(releaseProp, fromVal);
            predicate = predicate == null ? greaterThan : Expression.AndAlso(predicate, greaterThan);
        }
        if (To.HasValue)
        {
            var releaseProp = Expression.Property(parameter, "ReleaseYear");
            var toVal = Expression.Constant(To.Value);
            var lessThan = Expression.LessThanOrEqual(releaseProp, toVal);
            predicate = predicate == null ? lessThan : Expression.AndAlso(predicate, lessThan);
        }
        return predicate == null ? mov => true : Expression.Lambda<Func<T, bool>>(predicate, parameter);

    }
}
