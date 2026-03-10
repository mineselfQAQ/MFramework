using System.IO;

namespace MFramework.Core.Internal
{
    internal readonly struct CallerLocation
    {
        private readonly string _file;
        private readonly int _line;
        private readonly string _member;

        private CallerLocation(string file, int line, string member)
        {
            _file = file;
            _line = line;
            _member = member;
        }

        public static CallerLocation From(string file, int line, string member)
            => new CallerLocation(Path.GetFileName(file), line, member);

        public override string ToString()
            => $"{_member} ({_file}:{_line})";
    }
}