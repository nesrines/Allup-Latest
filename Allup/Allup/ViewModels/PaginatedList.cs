namespace Allup.ViewModels;
public class PaginatedList<T> : List<T>
{
    public int CurrentPage { get; }
    public bool HasPrev { get; }
    public bool HasNext { get; }
    public int Start { get; }
    public int End { get; }
    public int ElementCount { get; }

    public PaginatedList (IQueryable<T> query, int currentPage, int totalPage, int elementCount, int pageItemCount)
    {
        if (totalPage < pageItemCount) pageItemCount = totalPage;
        CurrentPage = currentPage;
        ElementCount = elementCount;
        HasPrev = CurrentPage > 1;
        HasNext = CurrentPage < totalPage;
        Start = CurrentPage - (int)Math.Floor((decimal)(pageItemCount - 1) / 2);
        End = CurrentPage + (int)Math.Ceiling((decimal)(pageItemCount - 1) / 2);
        if (Start <= 0)
        {
            Start = 1;
            End = pageItemCount;
        }
        if (End > totalPage)
        {
            Start = totalPage - (pageItemCount - 1);
            End = totalPage;
        }
        
        AddRange(query);
    }

    public static PaginatedList<T> Create(IQueryable<T> query, int currentPage, int elementCount, int pageItemCount)
    {
        int totalPage = (int)Math.Ceiling((decimal)query.Count() / elementCount);
        query = query.Skip((currentPage - 1) * elementCount).Take(elementCount);

        return new PaginatedList<T>(query, currentPage, totalPage, elementCount, pageItemCount);
    }
}