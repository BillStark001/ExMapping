using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMapping.Tests
{
    [TestClass]
    public class OopParsingTests
    {

        [TestMethod]
        public void KeyConflictTest()
        {
            var str = @"
the first value
\#LINE1=the second value
";
            Assert.ThrowsException<KeyConflictException>(() =>
            {
                var ans = ExmPlainParser.Parse(str, false);
            });
            Assert.ThrowsException<KeyConflictException>(() =>
            {
                ExmEventDrivenParser.ForceParse(str, false);
            });
        }

        [TestMethod]
        public void EventTest()
        {
            var triggerCount = 0;
            var str = @"
a=a
b=b
c=c
d=d
#e!=e
";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            List<string> keys = new() { "a", "b", "c", "d" };
            using (var parser = new ExmEventDrivenParser(new StreamReader(stream)))
            {
                parser.KeyValuePairParsed += (s, e) =>
                {
                    ++triggerCount;
                    Assert.IsTrue(keys.Contains(e.Key));
                    Assert.AreEqual(e.Key, e.Value);
                };
                parser.Parse();
            }
            Assert.AreEqual(triggerCount, 4);
        }
    }
}
