using System;
using System.Collections.Generic;
using System.Linq;
using cloudscribe.Pagination.Models;
using PasswordManagerApp.Models;

namespace PasswordManagerApp.Handlers{
    public class ViewHelper{


        public static PagedResult<T> PaginateResult<T>(IEnumerable<T> records,int pageSize, int pageNumber) where T:class
        {
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var recordsPaginate = records.Skip(ExcludeRecords).Take(pageSize).ToList();
            var res =  new PagedResult<T>
            {
                Data = recordsPaginate.ToList(),
                TotalItems = records.ToList().Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return res;
        }
        public static IEnumerable<T> FilterResult<T>(IEnumerable<T> records,string searchString) where T:ISearchable
        {
            
             var recordsFiltered = records.Where(b => b.Name.Contains(searchString));
                if(recordsFiltered.ToList().Count<=0)
                    return records;
                return recordsFiltered;
                
            
        }

        
    }
}