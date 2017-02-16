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

using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using VARP.Scheme.Data;

public class SymbolTest : MonoBehaviour {
    private void Start () {
        var quantity = 1000000;

        UnityEngine.Debug.Log("INTERN STRING");
        var s = string.Empty;
        TestBegin();
        for (var i = 0; i < quantity; i++)
        {
            s = string.Intern(i.ToString());
        }
        UnityEngine.Debug.LogFormat("[String.Intern] duration: {0}", TestEnd());


        Symbol sym = null;
        TestBegin();
        for (var i = 0; i < quantity; i++)
        {
            sym = Symbol.Intern(i.ToString());
        }
        UnityEngine.Debug.LogFormat("[Symbol.FromName] duration: {0}", TestEnd());

        SymbolA syma = null;
        TestBegin();
        for (var i = 0; i < quantity; i++)
        {
            syma = SymbolA.Intern(i.ToString());
        }
        UnityEngine.Debug.LogFormat("[SymbolA.FromName] duration: {0}", TestEnd());

        SymbolB symb = null;
        TestBegin();
        for (var i = 0; i < quantity; i++)
        {
            symb = SymbolB.Intern(i.ToString());
        }
        UnityEngine.Debug.LogFormat("[SymbolB.FromName] duration: {0}", TestEnd());

        SymbolC symc = null;
        TestBegin();
        for (var i = 0; i < quantity; i++)
        {
            symc = SymbolC.Intern(i.ToString());
        }
        UnityEngine.Debug.LogFormat("[SymbolC.FromName] duration: {0}", TestEnd());


        UnityEngine.Debug.Log("COMPARISONG TEST");

        TestBegin();
        var s1 = string.Intern("QWERTYUIOP{{{{SDFGHJKL");
        for (var i = 0; i < quantity; i++)
        {
            var b = s == s1;
        }
        UnityEngine.Debug.LogFormat("[String.Intern comparison] duration: {0}", TestEnd());

        TestBegin();
        var sym1 = Symbol.Intern("QWERTYUIOP{{{{SDFGHJKL");
        for (var i = 0; i < quantity; i++)
        {
            var b = sym == sym1;
        }
        UnityEngine.Debug.LogFormat("[Symbol.FromName comparison] duration: {0}", TestEnd());

        TestBegin();
        var syma1 = SymbolA.Intern("QWERTYUIOP{{{{SDFGHJKL");
        for (var i = 0; i < quantity; i++)
        {
            var b = syma == syma1;
        }
        UnityEngine.Debug.LogFormat("[SymbolA.FromName comparison] duration: {0}", TestEnd());


        TestBegin();
        var symb1 = SymbolB.Intern("QWERTYUIOP{{{{SDFGHJKL");
        for (var i = 0; i < quantity; i++)
        {
            var b = symb.hash == symb1.hash;
        }
        UnityEngine.Debug.LogFormat("[SymbolB.FromName comparison] duration: {0}", TestEnd());


        TestBegin();
        var symc1 = SymbolC.Intern("QWERTYUIOP{{{{SDFGHJKL");
        for (var i = 0; i < quantity; i++)
        {
            var b = symc.id == symc1.id;
        }
        UnityEngine.Debug.LogFormat("[SymbolC.FromName comparison] duration: {0}", TestEnd());
    }

    private Stopwatch sw;

    private void TestBegin()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    private long TestEnd()
    {
        sw.Stop();

        var microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        //long nanoseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L * 1000L));
        return microseconds;
    }
}


public class SymbolA : object
{
    private static Dictionary<string, SymbolA> internedSymbols = new Dictionary<string, SymbolA>();
    private string name;
    public string Name { get { return name; } }
    private SymbolA(string name) { this.name = name; }

    public static SymbolA Intern(string name)
    {
        SymbolA value;
        if (internedSymbols.TryGetValue(name, out value))
        {
            return value;
        }
        else
        {
            value = new SymbolA(name);
            internedSymbols[name] = value;
            return value;
        }
    }
    public override string ToString() { return "SYM:" + name.ToString(); }


}

public class SymbolB : object
{
    private static Dictionary<int, SymbolB> internedSymbols = new Dictionary<int, SymbolB>();
    private string name;
    public int hash;
    public string Name { get { return name; } }
    private SymbolB(string name, int hash) { this.hash = hash;  this.name = name; }

    public static SymbolB Intern(string name)
    {
        SymbolB value;
        var hash = name.GetHashCode();
        if (internedSymbols.TryGetValue(hash, out value))
        {
            return value;
        }
        else
        {
            value = new SymbolB(name, hash);
            internedSymbols[hash] = value;
            return value;
        }
    }
    public override string ToString() { return "SYM:" + name.ToString(); }


}

public class SymbolC : object
{
    private static Dictionary<int, SymbolC> internedSymbols = new Dictionary<int, SymbolC>();
    private static List<SymbolC> internedSymbolsList = new List<SymbolC>();
    private string name;
    public int id;
    public string Name { get { return name; } }
    private SymbolC(string name, int id) { this.id = id;  this.name = name; }

    public static SymbolC Intern(string name)
    {
        SymbolC value;
        var hash = name.GetHashCode();
        if (internedSymbols.TryGetValue(hash, out value))
        {
            return value;
        }
        else
        {
            value = new SymbolC(name, internedSymbolsList.Count);
            internedSymbols[hash] = value;
            return value;
        }
    }
    public override string ToString() { return "SYM:" + name.ToString(); }


}