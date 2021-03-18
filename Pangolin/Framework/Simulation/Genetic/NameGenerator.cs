using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public static class NameGenerator
    {
        private static Engine _engine = new Sha256();

        private static List<NameCount> _names;

        static NameGenerator()
        {
            _names = new List<NameCount>();
            _names.Add(new NameCount() { Name = "James"});
            _names.Add(new NameCount() { Name = "John" });
            _names.Add(new NameCount() { Name = "Robert" });
            _names.Add(new NameCount() { Name = "Michael" });
            _names.Add(new NameCount() { Name = "William" });
            _names.Add(new NameCount() { Name = "David" });
            _names.Add(new NameCount() { Name = "Richard" });
            _names.Add(new NameCount() { Name = "Joseph" });
            _names.Add(new NameCount() { Name = "Thomas" });
            _names.Add(new NameCount() { Name = "Charles" });
            _names.Add(new NameCount() { Name = "Mary" });
            _names.Add(new NameCount() { Name = "Patricia" });
            _names.Add(new NameCount() { Name = "Jennifer" });
            _names.Add(new NameCount() { Name = "Linda" });
            _names.Add(new NameCount() { Name = "Elizabeth" });
            _names.Add(new NameCount() { Name = "Barbara" });
            _names.Add(new NameCount() { Name = "Susan" });
            _names.Add(new NameCount() { Name = "Jessica" });
            _names.Add(new NameCount() { Name = "Sarah" });
            _names.Add(new NameCount() { Name = "Karen" });
        }

        public static string GetName()
        {
            var name = _engine.GetRandomElement(_names);
            name.Count++;
            return $"{name.Name} #{name.Count}";
        }

        private class NameCount
        {
            public string Name { set; get; }
            public int Count { set; get; }
        }

    }
}
