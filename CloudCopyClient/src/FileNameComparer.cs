namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public class FileNameComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo x, FileInfo y)
        {
            if ( x == y )
            {
                return true;
            }

            if ( x == null || y == null )
            {
                return false;
            }

            return x.FullName == y.FullName;
        }

        public int GetHashCode([DisallowNull] FileInfo obj)
        {
            return obj.FullName.GetHashCode();
        }
    }





}