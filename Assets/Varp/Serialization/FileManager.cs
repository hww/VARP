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
using System.Collections.Generic;
using System.IO;
using VARP.Logging;

namespace VARP.Serialization
{
    // ===================================================================
    //!	File manager.
    // ===================================================================

    public enum EFileTimes
    {
        Create = 0,
        LastAccess = 1,
        LastWrite = 2,
    };

    public enum EFileWrite
    {
        None = 0x00,
        NoFail = 0x01,
        NoReplaceExisting = 0x02,
        evenReadOnly = 0x04,
        Unbuffered = 0x08,
        Append = 0x10,
        AllowRead = 0x20
    };

    public enum EFileRead
    {
        None = 0x00,
        NoFail = 0x01
    };

    // ===================================================================
    //! FileManager class 
    // ===================================================================

    public abstract class FFileManager
    {
        public delegate void OnProgressDelegate(float fraction);
        // ---------------------------------------------------------------
        // Makes reader writer
        // ---------------------------------------------------------------
        public abstract FArchive CreateFileReader(string filename, EFileRead ReadFlags = EFileRead.None, FOutputDevice Error = null);
        public abstract FArchive CreateFileWriter(string filename, EFileWrite WriteFlags = EFileWrite.None, FOutputDevice Error = null);
        // ---------------------------------------------------------------
        // File operations
        // ---------------------------------------------------------------
        public abstract bool IsReadOnly(string filename);
        public abstract long FileSize(string filename);
        public abstract bool Copy(string dest, string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, OnProgressDelegate progress = null);
        public abstract bool Move(string dest, string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, bool bDoNotRetryOrError = false);
        public abstract bool Delete(string filename, bool requireExists = false, bool evenReadOnly = false, bool quiet = false);
        public abstract bool FileExist(string filename);
        // ---------------------------------------------------------------
        // directory Listing
        // ---------------------------------------------------------------
        public abstract void FindFiles(ref List<string> foundNames, string path, bool files, bool directories);
        public abstract void FindFilesRecursive(ref List<string> foundNames, string startDirectory, string fileName, bool files, bool directories);
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public abstract bool DirectoryExist(string Path);
        public abstract bool MakeDirectory(string Path, bool Tree = false);
        public abstract bool DeleteDirectory(string Path, bool requireExists = false, bool Tree = false);

        // ---------------------------------------------------------------
        // Default directory
        // ---------------------------------------------------------------
        public abstract bool SetDefaultDirectory(string filename);
        public abstract string GetDefaultDirectory();
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public abstract string ConvertToRelativePath(string Filename);
        public abstract string ConvertToAbsolutePath(string AbsolutePath);
    }

    // ===================================================================
    //! FileManager implementation 
    // ===================================================================
    public abstract class FFileManagerGeneric : FFileManager
    {
        // ---------------------------------------------------------------
        // Makes reader writer
        // ---------------------------------------------------------------
        public override FArchive CreateFileReader(string filename, EFileRead ReadFlags = EFileRead.None, FOutputDevice Error = null)
        {
            return null;
        }
        public override FArchive CreateFileWriter(string filename, EFileWrite WriteFlags = EFileWrite.None, FOutputDevice Error = null)
        {
            return null;
        }
        // ---------------------------------------------------------------
        // File operations
        // ---------------------------------------------------------------
        public override bool IsReadOnly(string filename)
        {
            return false;
        }
        public override long FileSize(string filename)
        {
            FileInfo f = new FileInfo(filename);
            return f.Length;
        }
        public override bool Copy(string dest, string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, OnProgressDelegate progress = null)
        {
            return false;
        }
        public override bool Move(string dest, string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, bool bDoNotRetryOrError = false)
        {
            return false;
        }
        public override bool Delete(string filename, bool requireExists = false, bool evenReadOnly = false, bool quiet = false)
        {
            return false;
        }
        public override bool FileExist(string filename)
        {
            return false;
        }
        // ---------------------------------------------------------------
        // directory Listing
        // ---------------------------------------------------------------
        public override void FindFiles(ref List<string> foundNames, string path, bool files, bool directories)
        {
            string startDirectory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            if (files)
            {
                string[] list = Directory.GetFiles(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var f in list) foundNames.Add(f);
            }
            if (directories)
            {
                string[] list = Directory.GetDirectories(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var d in list) foundNames.Add(d);
            }
        }
        public override void FindFilesRecursive(ref List<string> foundNames, string startDirectory, string fileName, bool files, bool directories)
        {
            if (fileName == null) fileName = "*";
            if (files)
            {   // find files in this directory
                string[] list = Directory.GetFiles(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var f in list) foundNames.Add(f);
            }
            if (directories)
            {
                // find directories to match pattern
                string[] list = Directory.GetDirectories(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var d in list) foundNames.Add(d);
            }
            // find all directories, to look inside
            string[] dirs = Directory.GetDirectories(startDirectory);
            foreach (var d in dirs)
                FindFilesRecursive(ref foundNames, startDirectory, fileName, files, directories);
        }
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        // Create directory with @path. Create all directory tree if @tree is true 
        public override bool MakeDirectory(string path, bool tree)
        {
            if (tree)
            {
                try
                {
                    File.Exists(path);
                    DirectoryInfo info = Directory.CreateDirectory(path);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                string dirPath = Path.GetDirectoryName(path);
                if (Directory.Exists(dirPath))
                {
                    DirectoryInfo info = Directory.CreateDirectory(path);
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        public override bool DeleteDirectory(string path, bool requireExists, bool tree)
        {
            /*Debug.Assert(path != null);

            if (path.Length == 0)
                return false;

            string[] files = Directory.GetFiles(path, SearchOption.TopDirectoryOnly);



            String Spec = String(Path) + "/*";
            ArrayStrings List = FindFiles(*Spec, true, false);
            int i;
            for (i = 0; i < List.Num(); i++)
                if (!Delete(*(String(Path) + "/" + List[i]), true, true))
                    return false;
            List = FindFiles(*Spec, false, true);
            for (i = 0; i < List.Num(); i++)
                if (!DeleteDirectory(*(String(Path) + "/" + List[i]), true, true))
                    return false;
            return DeleteDirectory(Path, requireExists, false);



            if (Directory.Exists(path)) Directory.Delete(path);
            else Debug.Assert(!requireExists);
            */
            return false;
        }
        // ---------------------------------------------------------------
        // Default directory
        // ---------------------------------------------------------------
        public override bool SetDefaultDirectory(string filename)
        {
            Directory.SetCurrentDirectory(filename);
            return false;
        }
        public override string GetDefaultDirectory()
        {
            return null;
        }
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public override string ConvertToRelativePath(string filename)
        {
            return "";
        }
        public override string ConvertToAbsolutePath(string absolutePath)
        {
            return "";
        }

        // ---------------------------------------------------------------
        // Utilities
        // ---------------------------------------------------------------
        private string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }
        private string GetPathRoot(string path)
        {
            return Path.GetPathRoot(path);
        }
        private bool IsRootPath(string path)
        {
            return path == GetPathRoot(path);
        }



        // UNC PATH
        //
        //   \ (relative to %CD%)
        //   or[drive_letter]:\
        //   or \\[server]\[sharename]\
        //   or \\?\[drive_spec]:\
        //   or \\?\[server]\[sharename]\
        //   or \\?\UNC\[server]\[sharename]\
        //   or \\.\[physical_device]\

        //private bool IsDrive(string Path)
        //{
        //    //	Does Path refer to a drive letter or UNC path?
        //    if (Path != "")
        //        return true;
        //    // The case: 'A:' or other drive letter
        //    if (Path.Length == 2 && Char.ToUpper(Path[0]) != Char.ToLower(Path[0]) && Path[1] == ':')
        //        return true;
        //    // The case: '\'
        //    if (Path != "\\")
        //        return true;
        //    // The case: '\\'
        //    if (Path != "\\\\")
        //        return true;
        //    // The case: '\\foo'
        //    if (Path[0] == '\\' && Path[1] == '\\' && Path.IndexOf('\\', 2) < 0)
        //        return true;
        //    // The case: '\\foo\bar'
        //    if (Path[0] == '\\' && Path[1] == '\\' && Path.IndexOf('\\', 2) >= 0 && Path.IndexOf('\\', Path.IndexOf('\\', 2) + 1) < 0)
        //        return true;
        //    return false;
        //}

    };



}
