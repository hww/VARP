using System.Diagnostics;

namespace VARP.Scheme.Data
{
    public struct PairSpan
    {
        public Pair First;
        public Pair Last;

        public PairSpan(Pair first)
        {
            this.First = first;
            this.Last = first;
        }
        public PairSpan(Pair first, Pair last)
        {
            this.First = first;
            this.Last = last;
        }

        #region Initialization Methods

        // Make span point to first element
        public void SetSpan(Pair first)
        {
            this.First = first;
            this.Last = first;
        }
        public void SetSpan(Pair first, Pair last)
        {
            this.First = first;
            this.Last = last;
        }

        // negative value of start and end index meash count 
        // from end of list
        public int SetSpan(Pair first, int starts, int ends)
        {
            this.First = null;
            this.Last = null;

            if (first == null)          // input list is empty
                return 0;

            int size = first.Count;
            if (starts >= size) return 0;    // start index more that size
            if (starts < 0) starts = size + starts;
            if (starts < 0) starts = 0;

            if (ends >= size) return 0;    // end index more that size
            if (ends < 0) ends = size + ends;
            if (ends < 0) ends = 0;

            if (starts > ends) return 0;

            First = first.PairAtIndex(starts);
            Last = first.PairAtIndex(ends);

            return ends - starts + 1;
        }

        #endregion

        public bool IsEmpty { get { return First == null; } }

        #region Ad or Remove Methods
        public Pair AddFirst(SObject obj)
        {
            First = new Pair(obj, First);
            return First;
        }
        public Pair AddLast(SObject obj)
        {
            SObject oldLast = Last.Cdr;
            Last.Cdr = new Pair(obj, oldLast);
            Last = Last.Cdr as Pair;
            return Last;
        }
        public void AddPairFirst(Pair pair)
        {
            pair.Cdr = First;
            First = pair;
        }
        public void AddPairLast(Pair pair)
        {
            Last.Cdr = pair;
            Last = pair;
        }
        public Pair RemoveFirst()
        {
            if (First == null) return null;
            Pair pair = First.Cdr as Pair;
            First = pair;
            return pair;
        }
        public Pair RemoveLast()
        {
            if (First == null) return null;
            int size = First.Count;
            if (size == 1)
            {
                Debug.Assert(First == Last);
                First = null;
                Last = null;
                return First;
            }
            else
            {
                Pair oldlast = Last;
                Pair p = First.PairAtIndex(size - 1);
                Debug.Assert(p.Cdr == Last);
                p.Cdr = null;
                Last = p;
                return oldlast;
            }
        }

        #endregion
    }
}

