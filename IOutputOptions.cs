
using System.ComponentModel;
using System.Collections.Generic;

namespace CloudCopy
{
    interface IOutputOptions
    {
        public string sortAttribute{get;set;}

        public ListSortDirection sortDirection{get;set;}

        public List<string> outputFields{get;set;}
    }

}