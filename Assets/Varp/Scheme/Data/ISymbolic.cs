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

namespace VARP.Scheme.Data
{
	/// <summary>
	/// Interface implemented by objects with 'symbol-like' properties: ie, objects that can be associated with a value in an environment.
	/// </summary>
	public interface ISymbolic
	{
		/// <summary>
		/// Gets the 'ordinary' symbol that this ISymbolic object represents.
		/// </summary>
		Symbol Symbol { get; }

		/// <summary>
		/// The value by which this symbol is hashed (how it is stored in the environment)
		/// </summary>
		/// <remarks>
		/// 'Real' symbols use their symbol number here. Symbols that have different meanings (eg, temporary symbols) should use a different hash value.
		/// 
		/// ISymbolic objects should also respond to the usual GetHashValue(), Equals() methods. Note that two symbols that are equal according
		/// to their HashValues may be different otherwise. (This means, for example, a LiteralSymbol != a Symbol, but they are stored in the
		/// same location in the environment)
		/// </remarks>
		object HashValue { get; }

		/// <summary>
		/// The environment in which this symbol should be bound, or null to specify the 'nearest' environment
		/// </summary>
		//GlobalEnvironment Location { get; }
	}
}