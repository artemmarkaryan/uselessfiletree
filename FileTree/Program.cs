using System;
using System.Collections.Generic;

namespace FileTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var ft = new FileTree(new Dictionary<string, Tag>(), new Dictionary<string, List<Tag>>() );
            ft.AddFile("math.doc", new List<string> {"difficult", "interesting"});
            ft.AddFile("physics.doc", new List<string> {"difficult", "complex"});
            ft.AddFile("programming.doc", new List<string> {"difficult", "very_difficult"});

            // ft.PrintRawValues();

            ft.CreateFiles();
        }
    }
}