/* 
 * Copyright (c) 2016 Valery Alex P.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;


namespace VARP.Scheme.Data
{
    using Exception;
    using REPL;

    /// <summary>
    /// Class representing a scheme pair
    /// </summary>
    /// <remarks>Implements the IList interface, but note that pairs that form part of improper or self-referential lists will produce bad results.</remarks>
    public sealed partial class Pair : SObject, ICollection<SObject>, IList<SObject>
    {
        static Pair NullPair;

        #region Variables

        // The Car object for this pair
        public SObject Car = null;

        // The Cdr object for this pair
        public SObject Cdr = null;


        #endregion

        /// <summary>
        /// Constructs a pair (())
        /// </summary>
        public Pair()
        {
        }

        /// <summary>
        /// Constructs a pair with a given car and cdr
        /// </summary>
        /// <param name="car">The car object for this pair</param>
        /// <param name="cdr">The cdr object for this pair</param>
        public Pair(SObject car, SObject cdr)
        {
            this.Car = car;
            this.Cdr = cdr;
        }

        /// <summary>
        /// Constructs a pair with the contents of a collection (ie, create a list from the collection)
        /// </summary>
        /// <param name="collection">The collection to create a list from. This collection must have at least one object for this constructor to work.</param>
        /// <param name="improper">make list improper.</param>
        public Pair(ICollection<SObject> collection, bool improper = false) : this (collection, ref NullPair, improper)
        {
        }
        /// <summary>
        /// Constructs a pair with the contents of a collection (ie, create a list from the collection)
        /// </summary>
        /// <param name="collection">The collection to create a list from. This collection must have at least one object for this constructor to work.</param>
        /// <param name="last">return last pair.</param>
        /// <param name="improper">make list improper.</param>
        public Pair(ICollection<SObject> collection, ref Pair last, bool improper = false)
        {
            Pair thePair = this;

            // The collection must have at least one object for this to be valid
            // TODO: make this a proper scheme exception
            if (collection.Count <= 0) throw new System.Exception("In order to create a list from a collection, the collection must have at least one entry");

            // Add each object in the collection to this pair
            lock (SyncRoot)
            {
                int count = collection.Count;
                if (improper) count--;
                IEnumerator<SObject> collectionEnum = collection.GetEnumerator();

                for (int x = 0; x < count; x++)
                {
                    collectionEnum.MoveNext();
                    thePair.Car = collectionEnum.Current;

                    if (x == count - 1) break;

                    thePair.Cdr = new Pair();
                    thePair = (Pair)thePair.Cdr;
                }
                if (improper)
                {
                    collectionEnum.MoveNext();
                    thePair.Cdr = collectionEnum.Current;
                }
                last = thePair;
            }
        }

        /// <summary>
        /// Constructs a pair with the contents of a collection (ie, create a list from the collection)
        /// </summary>
        /// <param name="collection">The collection to create a list from. This collection must have at least one object for this constructor to work.</param>
        /// <param name="improper">make list improper.</param>
        public Pair(IList<SObject> collection, int offset, bool improper = false) : this(collection, offset, ref NullPair, improper)
        {

        }

        /// <summary>
        /// Constructs a pair with the contents of a collection (ie, create a list from the collection)
        /// </summary>
        /// <param name="collection">The collection to create a list from. This collection must have at least one object for this constructor to work.</param>
        /// <param name="last">return last pair.</param>
        /// <param name="improper">make list improper.</param>
        public Pair(IList<SObject> collection, int offset, ref Pair last, bool improper = false)
        {
            Pair thePair = this;

            // The collection must have at least one object for this to be valid
            // TODO: make this a proper scheme exception
            if (collection.Count <= 0) throw new System.Exception("In order to create a list from a collection, the collection must have at least one entry");

            // Add each object in the collection to this pair
            lock (SyncRoot)
            {
                int count = collection.Count;
                if (improper) count--;

                for (int x = offset; x < count; x++)
                {
                    thePair.Car = collection[x];

                    if (x == count - 1) break;

                    thePair.Cdr = new Pair();
                    thePair = (Pair)thePair.Cdr;
                }
                if (improper)
                    thePair.Cdr = collection[collection.Count - 1];

                last = thePair;
            }
        }

        #region Convienience functions

        public SObject Append(SObject right) { return Pair.Append(this, right); }
        public Pair GetLoopHead() { return Pair.GetLoopHead(this); }
        public Pair Reverse() { return Pair.Reverse(this); }
        public Pair GetLast() { return Pair.GetLast(this); }
        public Pair Duplicate() { return Pair.Duplicate(this); }
        public Pair Midpoint(Pair last) { return Pair.Midpoint(this, last); }
        public bool HasLoop() { return Pair.HasLoop(this); }
        public bool IsImproper() { return Pair.IsImproper(this); }
        public bool IsImproperOrLoop() { return Pair.IsImproperOrLoop(this); }
        public bool IsList { get { return Cdr is Pair || Cdr == null; } }
        public bool IsPair { get { return !(Cdr is Pair || Cdr == null); } }

        #endregion

        public override int GetHashCode()
		{
			int hashCode = typeof(Pair).GetHashCode();
			foreach (object element in this) hashCode ^= element.GetHashCode();
			return hashCode;
		}

		public override bool Equals(object obj) { return Pair.Equals(this, obj); }

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

        public Pair PairAtIndex(int index) { return Pair.PairAtIndex(this, index); }

        public SObject this[int index]
        {
            get
            {
                return Pair.PairAtIndex(this, index).Car;
            }
            set
            {
                Pair.PairAtIndex(this, index).Car = value;
            }
        }

        public int Count
        {
            get
            {
                return Pair.Length(this);
            }
        }

		public void CopyTo(SObject[] array, int index)
		{
			Pair currentPair = this;

			if (HasLoop(this)) throw new NotSupportedException("A Pair that contains a loop cannot be copied into an array");

			while (currentPair != null)
			{
				array.SetValue(currentPair.Car, index++);

				currentPair = (Pair)currentPair.Cdr;
			}
		}

		public object SyncRoot
		{
			get
			{
				// Pairs have to be globally synchronized at the moment
				return typeof(Pair);
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator<SObject> GetEnumerator()
		{
			return new PairEnumerator(this);
		}
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PairEnumerator(this);
        }

        #endregion

        #region IList Members

        public bool IsReadOnly { get { return false; } }

		public void RemoveAt(int index) { Pair.RemoveAt(this, index); }

        public void Insert(int index, SObject value) { Pair.Insert(this, index, value); }

        /// <summary>
        /// Removes the first ocurrance of the given object from the list formed by this Pair.
        /// </summary>
        /// <remarks>Does nothing if the object is not in the list. May fail if the object is the only one in the list. Removal is by copying.</remarks>
        public bool Remove(SObject value)
		{
			Pair pairToRemove = PairContaining(value);

            if (pairToRemove != null)
            {
                pairToRemove.RemoveAt(0);
                return true;
            }
            return false;
		}

		/// <summary>
		/// Returns true if the given object is in the list formed by this pair.
		/// </summary>
		/// <remarks>Infinite loop if the list is self-referential. Exception if the list is improper.</remarks>
		public bool Contains(SObject value)
		{
			return PairContaining(value)!=null;
		}

		/// <summary>
		/// Lists formed by Pairs cannot be cleared. Do not call this function.
		/// </summary>
		public void Clear()
		{
			throw new NotSupportedException("Pairs cannot sensibly be cleared");
		}

		/// <summary>
		/// Determines the Pair which is part of the list that this Pair forms and has the given object as a Car.
		/// </summary>
		public Pair PairContaining(SObject value)
		{
			Pair thisPair = this;
			int currentIndex = 0;

			while (thisPair != null)
			{
				if (thisPair.Car == value) break;

				thisPair = (Pair)thisPair.Cdr;
				currentIndex++;
			}

			return thisPair;
		}

		/// <summary>
		/// Determines the index of the element of the list with the given value as a Car
		/// </summary>
		public int IndexOf(SObject value)
		{
			// Search for the pair with the given object as the Car
			Pair thisPair = this;
			int currentIndex = 0;

			while (thisPair != null)
			{
				if (thisPair.Car == value) break;

				thisPair = (Pair)thisPair.Cdr;
				currentIndex++;
			}

			if (thisPair != null)
				return currentIndex;
			else
				return -1;
		}

		/// <summary>
		/// Adds an element to the end of the list
		/// </summary>
		/// <param name="value">The value to add</param>
		/// <returns>The index of the new element</returns>
		public int AddAndReturnIndex(SObject value)
		{
			Pair lastPair = this;
			int count = 0;

			while (lastPair.Cdr != null) 
			{
				lastPair = (Pair)lastPair.Cdr;
				count++;
			}

			lastPair.Cdr = new Pair(value, null);

			return count+1;
		}
        /// <summary>
        /// Adds an element to the end of the list
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <returns></returns>
        public void Add(SObject value)
        {
            AddAndReturnIndex(value);
        }

        /// <summary>
        /// Whether or not this is a fixed size IList (Pairs are never fixed size)
        /// </summary>
        public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

        #endregion

        #region SObject Methods
        public override bool IsLiteral { get { return false; } }
        public override SBool AsBool() { return SBool.True; }
        public override string AsString() { return AsString(this); }
        #endregion
    }
}
