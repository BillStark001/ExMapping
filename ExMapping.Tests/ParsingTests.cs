using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExMapping;
using System.Collections.Generic;

namespace ExMapping.Tests
{
    using DSS = Dictionary<string, string>;

    [TestClass]
    public class ParsingTests
    {

        

        [TestMethod]
        public void TestTestItself()
        {
            Assert.AreNotEqual(114514, 1919810);
        }

        [TestMethod]
        public void TestBasicParsing()
        {
            var inStr = @"# test
a=b
a = b
multi=
&line1
$line2   
&continue line2
$
";
            var outMap = Utils.Parse(inStr);
            Assert.AreEqual(3, outMap.Count);
            Assert.AreEqual(outMap["a"], "b");
            Assert.AreEqual(outMap["a "], " b");
            Assert.AreEqual(outMap["multi"], "line1\nline2   continue line2\n");
        }

        public void TestEscape()
        {
            var inStr = @"# test
a=b
a = b
multi=
&line1
$line2   
&continue line2
$
";
            var outMap = Utils.Parse(inStr);
            Assert.AreEqual(3, outMap.Count);
            Assert.AreEqual(outMap["a"], "b");
            Assert.AreEqual(outMap["a "], " b");
            Assert.AreEqual(outMap["multi"], "line1\nline2   continue line2\n");
        }
    }
}
