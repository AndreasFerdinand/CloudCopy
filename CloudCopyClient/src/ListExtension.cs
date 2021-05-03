namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class ListExtension
    {
        public static List<T> RemoveNotMatchingRegex<T>(this List<T> source, Func<T, string > predicate, string pattern )
        {
            if ( source == null )
            {
                throw new ArgumentNullException("source", "The list is null and contains no elements.");
            }

            if ( predicate == null )
            {
                throw new ArgumentNullException("predicate", "The predicate function is null and cannot be executed.");
            }

            if ( pattern == null )
            {
                throw new ArgumentNullException("pattern", "Pattern is null therefore is's impossible to filter.");
            }

            var result = new List<T>();

            foreach( var item in source )
            {
                if ( Regex.IsMatch(predicate(item),pattern) )
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static List<T> RemoveNotMatchingWildcards<T>(this List<T> source, Func<T, string > property, string pattern )
        {
            if ( pattern == null )
            {
                throw new ArgumentNullException("pattern", "Pattern is null therefore is's impossible to filter.");
            }
            
            var regexpattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";

            return RemoveNotMatchingRegex(source, property, regexpattern);
        }

        public static List<T> SortByProperty<T>(this List<T> source, SortByOption property )
        {
            return SortByProperty<T>(source, property, ListSortDirection.Ascending);
        }

        // see https://stackoverflow.com/questions/27683904/how-do-i-sort-the-dynamic-list-in-c-sharp
        public static List<T> SortByProperty<T>(this List<T> source, SortByOption property, ListSortDirection direction )
        {
            List<T> result;

            var type = typeof(T);

            var sortProperty = type.GetProperty( property.ToString() );

            if ( direction == ListSortDirection.Ascending )
            {
                result = source.OrderBy(p => sortProperty.GetValue(p, null)).ToList();
            }
            else
            {
                result = source.OrderByDescending(p => sortProperty.GetValue(p, null)).ToList();
            }

            return result;
        }
    }
}