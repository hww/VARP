using System.Collections.Generic;

namespace VARP
{
    public static class ReadLine
    {
        public delegate void OnReadLineDelegate(string line);
        private static OnReadLineDelegate onReadLineDelegate;

        public delegate string[] OnAutoCompletionDelegate(string line, int caretPosition);
        public static OnAutoCompletionDelegate AutoCompletionHandler { private get; set; }


        public static void Read(string promp, OnReadLineDelegate onReadLine)
        {
            ReadLine.onReadLineDelegate = onReadLine;
            VARP.Console.ReadLine(promp, delegate (string text)
            {
                Console.WriteLine(text);
                AddHistory(text);
                if (onReadLineDelegate != null)
                    onReadLineDelegate(text);
            });
        }

        #region Navigating trought history

        private static readonly List<string> History = new List<string>(100);
        private static int historyPosition;

        private static void HistoryPositionReset()
        {
            historyPosition = History.Count - 1;
        }

        private static void HistoryPositionNext()
        {
            if (historyPosition < History.Count - 1) historyPosition++;
        }

        private static void HistoryPositionPrevious()
        {
            if (historyPosition > 0) historyPosition--;
        }

        #endregion

        #region Add elements to history

        public static void AddHistory(string text)
        {
            History.Add(text);
            HistoryPositionReset();
        }

        public static void AddHistory(string[] history)
        {
            foreach (var s in history)
                History.Add(s);
            HistoryPositionReset();
        }

        public static void AddHistory(List<string> history)
        {
            foreach (var s in history)
                History.Add(s);
            HistoryPositionReset();
        }

        public static List<string> GetHistory()
        {
            return History;
        }

        public static void ClearHistory()
        {
            History.Clear();
        }

        #endregion
    }
}
