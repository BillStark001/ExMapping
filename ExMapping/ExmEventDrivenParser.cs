using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ExMapping
{
    using DSS = Dictionary<string, string>;

    public class KeyValuePairEventArgs: EventArgs, IEquatable<KeyValuePairEventArgs>
    {
        public string Key { get; }
        public string Value { get; }

        public KeyValuePairEventArgs(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public bool Equals([AllowNull] KeyValuePairEventArgs? other)
        {
            if (other == null)
                return false;
            return Key == other.Key && Value == other.Value;
        }
    }

    public class ExmEventDrivenParser: IDisposable
    {
        public StreamReader Reader { get; }
        private string? _lastKey = null;
        private string? _lastValue = null;
        private int _lastKeyUpdated = 0;

        public event EventHandler<KeyValuePairEventArgs>? KeyValuePairParsed;

        public ExmEventDrivenParser(StreamReader reader)
        {
            Reader = reader;
        }

        public ExmEventDrivenParser(string path): this(new StreamReader(path)) { }

        public void Dispose()
        {
            Reader.Dispose();
        }

        public void Parse()
        {
            _lastKey = null;
            _lastValue = null;
            string? line;
            int i = 0;
            while ((line = Reader.ReadLine()) != null)
            {
                var (k, v) = FormatUtils.ParseLine(line);
                if (k is string newKey)
                {
                    if (_lastKey != null && KeyValuePairParsed != null)
                        KeyValuePairParsed(this, new(_lastKey, _lastValue ?? ""));
                    _lastKey = newKey;
                    _lastValue = v!;
                    _lastKeyUpdated = i;
                }
                else if (v is string newValue)
                {
                    if (_lastKey == null)
                    {
                        _lastKey = $"#LINE{i}";
                        _lastKeyUpdated = i;
                    }
                    var flag1 = line.StartsWith('$') || line.StartsWith('&');
                    var flag2 = _lastValue != null;
                    if (!flag2)
                        _lastValue = "";
                    if (flag1)
                        _lastValue += newValue;
                    else if (flag2)
                        _lastValue += "\n" + newValue;
                    else
                        _lastValue = newValue;

                }
                ++i;
            }
            if (_lastKey != null && KeyValuePairParsed != null)
                KeyValuePairParsed(this, new(_lastKey, _lastValue ?? ""));
        }

        public static DSS ForceParse(string str, bool allowKeyConflict = true)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            DSS ans = new();
            using (var parser = new ExmEventDrivenParser(new StreamReader(stream)))
            {
                parser.KeyValuePairParsed += (s, e) =>
                {
                    if (!allowKeyConflict && ans.ContainsKey(e.Key))
                        throw new KeyConflictException(e.Key, parser._lastKeyUpdated);
                    ans[e.Key] = e.Value;
                };
                parser.Parse();
            }
            return ans;
        }

    }
}
