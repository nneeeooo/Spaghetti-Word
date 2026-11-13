// Test-only stand-in for the VSTO-generated Globals so linked Word modules compile and run against a real Word.
[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelize]

namespace Word
{
    internal static class Globals
    {
        public static TestAddIn ThisAddIn = new TestAddIn();
    }

    internal class TestAddIn
    {
        public Microsoft.Office.Interop.Word.Application Application;
    }
}
