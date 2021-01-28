namespace CloudCopy
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class OutputOptions : IOutputOptions
    {
        private string sortAttribute;
        private ListSortDirection sortDirection;
        private List<string> outputFields;

        public OutputOptions()
            : this(string.Empty, ListSortDirection.Ascending)
        {
        }

        public OutputOptions(string sortAttribute, ListSortDirection sortDirection)
        {
            this.SortAttribute = sortAttribute;
            this.SortDirection = sortDirection;
        }

        public string SortAttribute { get => this.sortAttribute; set => this.sortAttribute = value; }

        public ListSortDirection SortDirection { get => this.sortDirection; set => this.sortDirection = value; }

        public List<string> OutputFields { get => this.outputFields; set => this.outputFields = value; }
    }
}