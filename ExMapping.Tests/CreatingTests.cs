using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExMapping;
using System.Collections.Generic;

namespace ExMapping.Tests
{
    using DSS = Dictionary<string, string>;

    [TestClass]
    public class CreatingTests
    {

        private static DSS Parse(string str)
        {
            return ExmPlainParser.Parse(str);
        }

        private static string Create(DSS map)
        {
            return ExmCreator.Create(map);
        }

        [TestMethod]
        public void TestBasicCreating()
        {
            var str = @"key1=value1
$2
key2=value2
";
            var strf = @"key1=value1\n2
key2=value2
";
            var strm = @"key1=
&value1
$2
key2=value2
";
            
            var map = Parse(str);

            Assert.AreEqual(str, ExmCreator.Create(map, ExmCreator.Type.DollarSeparated));
            Assert.AreEqual(strf, ExmCreator.Create(map, ExmCreator.Type.Flatten));
            Assert.AreEqual(strm, ExmCreator.Create(map, ExmCreator.Type.SeparateAndReturn));
        }
    }
}
