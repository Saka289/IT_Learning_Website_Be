using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LW.Shared.SeedWork;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, long totalItems, int pageIndex, int pageSize)
    {
        _metaData = new MetaData
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = pageIndex,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
        AddRange(items);
    }

    private MetaData _metaData { get; }

    public MetaData GetMetaData()
    {
        return _metaData;
    }

    public static async Task<PagedList<T>> ToPageListAsync(IQueryable<T> source, int pageIndex, int pageSize,string? orderBy = null, bool isAscending = true)
    {
        var count = await source.CountAsync();
        if (!string.IsNullOrEmpty(orderBy))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);
        
            source = isAscending ? source.OrderBy(lambda) : source.OrderByDescending(lambda);
        }
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedList<T>(items, count, pageIndex, pageSize);
    }
}