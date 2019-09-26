using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace dotnet.IO
{
    public static class PathUtils
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory; // end with a backslash(\).

        public static readonly string ExeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName

        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        #region System.Environment

        public static readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static readonly string MyDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        #endregion
    }
}
