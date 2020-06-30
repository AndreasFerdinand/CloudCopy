namespace CloudCopy
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IOutputOptions
    {
        public string SortAttribute { get; set; }

        public ListSortDirection SortDirection { get; set; }

        public List<string> OutputFields { get; set; }
    }
}