using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
  // This class is used for pagination purposes to return a response that can be used with pagination
  // the pagination info is returned in headers of the response and the client uses it to determine how to display the items and make a request for the next batch of items
  // this has props which represent info that is returned to the client to use for pagination
  public class PagedList<T> : List<T>
  {
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
      TotalCount = count;
      PageSize = pageSize;
      CurrentPage = pageNumber;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);
      this.AddRange(items); // AddRange adds items to the end of a list.  This adds the items to be paginated to this list which also has these pagination props to reference for the client.
      // the range of items is accessed directly since this class extends the List class
    }

    // this method takes an IQueryable passed in which allows you to defer execution of the query to the database and run skip and take operators to get the items based on the pagination details
    // performs takes and skips on the source (list of items from the database) to get the page of results desired and returns a paginated list (PagedList instance) which has pagination info we can then send to the client.
    // the client uses this info to determine what to display and how to request the next batch of items
    // a paged list is returned from the IDatingRepo 
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
      var count = await source.CountAsync();
      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
      return new PagedList<T>(items, count, pageNumber, pageSize);
    }
  }
}