using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputerInfo computerInfo = new ComputerInfo();
            Console.WriteLine(computerInfo.Output());

            Console.ReadKey();
        }
    }
}
