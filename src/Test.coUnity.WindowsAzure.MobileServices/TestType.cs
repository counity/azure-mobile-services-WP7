using System;

namespace Test.coUnity.WindowsAzure.MobileServices
{
    class TestType
    {
        public int Id { get; set; }
        public int IntColumn { get; set; }
        public string TextColumn { get; set; }
        public DateTime DateTime { get; set; }
        public double Double { get; set; }
        public double Float { get; set; }
        public bool Bool { get; set; }
    }

    class TestTypeContainingComplexType
    {
        public int Id { get; set; }
        public TestType Complex { get; set; }
    }

    class OnlyId
    {
        public int Id { get; set; }
    }
}
