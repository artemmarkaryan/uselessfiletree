using System;
using System.Collections.Generic;
using System.IO;

namespace FileTree
{
    public class Tag : IComparable<Tag>
    {
        public string Name => _name;

        private readonly HashSet<string> _files;
        private readonly string _name;

        public Tag(string name, HashSet<string> files)
        {
            _name = name;
            _files = files;
        }

        public int Length()
        {
            return _files.Count;
        }

        public void AddFile(string filename)
        {
            _files.Add(filename);
        }

        public HashSet<string> GetFiles()
        {
            return _files;
        }

        public int CompareTo(Tag other)
        {
            return Length() > other.Length() ? -1 : 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Node
    {
        public string Name;
        public List<Node> Children;
        public List<string> Files;

        public Node(string name)
        {
            Name = name;
            Files = new List<string>();
            Children = new List<Node>();
        }

        public bool ChildrenContainTag(string tagName, out Node newNode)
        {
            var contains = false;
            newNode = null;
            foreach (var child in Children)
            {
                if (child.Name != tagName) continue;
                contains = true;
                newNode = child;
                break;
            }

            return contains;
        }

        public override string ToString()
        {
            return $"Node \"{Name}\"";
        }
    }

    public class FileTree
    {
        public Node Root = new Node("root");
        private Dictionary<string, Tag> _allTags;
        private Dictionary<string, List<Tag>> _allFiles;

        public FileTree(Dictionary<string, Tag> allTags, Dictionary<string, List<Tag>> allFiles)
        {
            _allTags = allTags;
            _allFiles = allFiles;
        }

        public void AddFile(string filename, List<string> tags)
        {
            var thisFileTags = new List<Tag>();
            foreach (var tagName in tags)
            {
                bool exists = _allTags.TryGetValue(tagName, out var thisTag);
                if (!exists)
                {
                    thisTag = new Tag(tagName, new HashSet<string> {filename});
                    _allTags.Add(tagName, thisTag);
                }
                thisTag.AddFile(filename);
                thisFileTags.Add(thisTag);
            }

            _allFiles.Add(filename, thisFileTags);
        }

        public Node BuildTree()
        {
            Root = new Node("root");

            foreach (var file in _allFiles)
            {
                file.Value.Sort();
                var currentNode = Root;

                foreach (var tag in file.Value)
                {
                    if (!currentNode.ChildrenContainTag(tag.Name, out var newNode))
                    {
                        newNode = new Node(tag.Name);
                        currentNode.Children.Add(newNode);
                    }
                    currentNode = newNode;
                }

                currentNode.Files.Add(file.Key);
            }

            return Root;
        }

        private void ClearFiles()
        {
            try
            {
                Directory.Delete("./" + Root.Name, true);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory \"root\" doesn't exist, but its okay");
            }
        }

        private static void CreateNode(Node node, string where)
        {
            var thisPath = where + node.Name + "/";
            Directory.CreateDirectory(thisPath);
            foreach (var nodeFile in node.Files)
            {
                File.Create(thisPath + nodeFile);
            }
            foreach (var nodeChild in node.Children)
            {
                CreateNode(nodeChild, thisPath);
            }
        }

        public void CreateFiles()
        {
            ClearFiles();
            BuildTree();
            CreateNode(Root, "./");
            WriteFilesSummary();
        }

        private void WriteFilesSummary()
        {
            var filesString = "all files\n";
            foreach (var (key, value) in _allFiles)
            {
                filesString += $"file \"{key}\": {string.Join(", ", value)}\n";
            }

            var tagsString ="all tags\n";
            foreach (var (key, value) in _allTags)
            {
                tagsString += $"tag \"{key}\": {string.Join(", ", value.GetFiles())}\n";
            }

            File.WriteAllText("./files.txt", filesString);
            File.WriteAllText("./tags.txt", tagsString);
        }

        public void PrintTree(Node node)
        {
            Console.WriteLine($"{node}; " +
                              $"children: {string.Join(", ", node.Children)};" +
                              $"files: {string.Join(", ", node.Files)}");
            foreach (var nodeChild in node.Children)
            {
                PrintTree(nodeChild);
            }
        }

        public void PrintRawValues()
        {
            Console.WriteLine("all files\n");
            foreach (var (key, value) in _allFiles)
            {
                Console.WriteLine($"file \"{key}\": {string.Join(", ", value)}");
            }

            Console.WriteLine("\nall tags\n");
            foreach (var (key, value) in _allTags)
            {
                Console.WriteLine($"tag \"{key}\": {string.Join(", ", value.GetFiles())}");
            }
        }
    }
}