
using System.Collections.Generic;
using System.ComponentModel;

namespace CloudCopy
{
    class OutputOptions : IOutputOptions
    {
        string _sortAttribute;
        ListSortDirection _sortDirection;

        List<string> _outputFields;

        public OutputOptions() : this("",ListSortDirection.Ascending) {}

        public OutputOptions(string sortAttribute, ListSortDirection sortDirection)
        {
            this.sortAttribute = sortAttribute;
            this.sortDirection = sortDirection;
        }

        public string sortAttribute { get => _sortAttribute; set => _sortAttribute = value; }
        public ListSortDirection sortDirection { get => _sortDirection; set => _sortDirection = value; }
        public List<string> outputFields { get => _outputFields; set => _outputFields = value; }
    }

}