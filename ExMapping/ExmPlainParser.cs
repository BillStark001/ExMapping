using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExMapping
{
    using DSS = Dictionary<string, string>;

    public static class ExmPlainParser
    {

        public static DSS Parse(string text, bool allowKeyConflict = true)
        {
            DSS context = new();
            var strs = text.Split('\n');
            string? lastKey = null;
            for (int i = 0; i < strs.Length; ++i)
            {
                // read and pre-process the current line
                var str = strs[i];
                var (k, v) = FormatUtils.ParseLine(str);
                if (k is string newKey)
                {
                    string newValue = v!;
                    if (!allowKeyConflict && context.ContainsKey(newKey))
                        throw new KeyConflictException(lastKey!, i);
                    lastKey = newKey;
                    context[newKey] = newValue;
                }
                else if (v is string newValue)
                {
                    if (lastKey == null)
                    {
                        var lastKeyOrig = lastKey = $"#LINE{i}";
                        int c = 0;
                        while (context.ContainsKey(lastKey))
                        {
                            if (!allowKeyConflict)
                                throw new KeyConflictException(lastKey!, i);
                            lastKey = lastKeyOrig + "_" + (c++).ToString();
                        }
                    }
                    var flag1 = str.StartsWith('$') || str.StartsWith('&');
                    var flag2 = context.ContainsKey(lastKey);
                    if (!flag2)
                        context[lastKey] = "";
                    if (flag1)
                        context[lastKey] += newValue;
                    else if (flag2)
                        context[lastKey] += "\n" + newValue;
                    else
                        context[lastKey] = newValue;

                }
                // else do nothing
            }
            return context;
        }
    }
}
