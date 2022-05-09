using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExMapping;
using System.Collections.Generic;

namespace ExMapping.Tests
{
    using DSS = Dictionary<string, string>;

    [TestClass]
    public class ParsingTests
    {

        private static DSS Parse(string str)
        {
            return ExmPlainParser.Parse(str);
        }

        [TestMethod]
        public void TestTestItself()
        {
            Assert.AreNotEqual(114514, 1919810);
        }

        [TestMethod]
        public void TestBasicParsing()
        {
            var map = Parse(@"
key1=value1
key2=value2
key3!=value4
key4!=value3
");
            Assert.AreEqual(map["key3!"], "value4");
        }

        [TestMethod]
        public void TestTrickyParsing()
        {
            var inStr = @"# test
a=b
a = b
=####
&line1
$line2   
&continue line2
$
";
            var outMap = Parse(inStr);
            Assert.AreEqual(3, outMap.Count);
            Assert.AreEqual(outMap["a"], "b");
            Assert.AreEqual(outMap["a "], " b");
            Assert.AreEqual(outMap[""], "####line1\nline2   continue line2\n");
        }

        [TestMethod]
        public void TestEscape()
        {
            var inStr = @"# test
\\\\\\\\=\\\\\\\\
\\\\=\\\
a=\\n
b=\\\n
c\=\\=d
path=C:\a\\b\c\d\e\f\m\g
\#key\\==value
\#key\=value=value=value
";
            var outMap = Parse(inStr);
            Assert.AreEqual(outMap["a"], "\\n");
            Assert.AreEqual(outMap["b"], "\\\n");
            Assert.IsTrue(outMap.ContainsKey("c=\\"));
            Assert.AreEqual(outMap["path"], "C:\\a\\b\\c\\d\\e\f\\m\\g");
            Assert.AreEqual(outMap["#key\\"], "=value");
            Assert.AreEqual(outMap["#key=value"], "value=value");
            Assert.AreEqual(outMap["\\\\\\\\"], "\\\\\\\\");
            Assert.AreEqual(outMap["\\\\"], "\\\\");
        }

        [TestMethod]
        public void TestLongText()
        {
            var map = Parse(@"


&        
hello?
hello again?
longText=
&line1
$=line2
$$line3
 and its tail
noKeyText=

# there is one space in the next line
 
# there are 10 spaces in the next line
          
what the hell?");
            Assert.AreEqual(map["longText"], "line1\n=line2\n$line3\n and its tail");
            Assert.AreEqual(map["noKeyText"], "\nwhat the hell?");
            Assert.AreEqual(map["#LINE3"], "        \nhello?\nhello again?");
        }
    }
}
