using System;
using System.Collections.Generic;
using System.Text;

namespace ExMapping
{
    public static class ExmCreator
    {
        public enum Type
        {
            Flatten = 0b0000, 
            DollarSeparated = 0b0001, 
            MultiLineReturn = 0b0010, 
            SeparateAndReturn = 0b0011
        }
        public static string Create(IDictionary<string, string> pairs, Type type = Type.DollarSeparated)
        {
            StringBuilder ret = new();
            foreach (var (key, value) in pairs)
            {

                // process keys
                var keyRes = FormatUtils.CreateKey(key);
                ret.Append(keyRes);
                ret.Append("=");
                if (value.Contains('\n') && (type & Type.DollarSeparated) > 0)
                {
                    var firstFlag = true;
                    foreach (var v in value.Split('\n'))
                    {
                        if (firstFlag)
                        {
                            if ((type & Type.MultiLineReturn) > 0)
                            {
                                ret.AppendLine();
                                ret.Append('&');
                            }
                            firstFlag = false;
                        }
                        else
                            ret.Append('$');
                        ret.AppendLine(FormatUtils.CreateValue(v));
                    }
                }
                else
                {
                    ret.AppendLine(FormatUtils.CreateValue(value));
                }
            }
            return ret.ToString();
        }

    }

}
