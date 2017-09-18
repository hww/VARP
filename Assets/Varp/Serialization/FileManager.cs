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
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
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
    [Flags]
    public enum EFileWrite
    {
        None = 0x00,
        NoFail = 0x01,
        NoReplaceExisting = 0x02,
        ReadOnly = 0x04,
        Unbuffered = 0x08,
        Append = 0x10,
        AllowRead = 0x20
    };
    [Flags]
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
        public abstract FArchive CreateFileReader(string filename, EFileRead readFlags = EFileRead.None, IOutputDevice error = null);
        public abstract FArchive CreateFileWriter(string filename, EFileWrite writeFlags = EFileWrite.None, IOutputDevice error = null);
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
        public abstract bool DirectoryExists(string path);
        public abstract bool MakeDirectory(string path, bool tree = false);
        public abstract bool DeleteDirectory(string path, bool requireExists = false, bool tree = false);

        // ---------------------------------------------------------------
        // Default directory
        // ---------------------------------------------------------------
        public abstract bool SetDefaultDirectory(string filename, bool requireTarget);
        public abstract string GetDefaultDirectory();
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public abstract string ConvertToRelativePath(string absolutePath);
        public abstract string ConvertToAbsolutePath(string relativePath);
    }

    // ===================================================================
    //! FileManager implementation 
    // ===================================================================
    public class FFileManagerGeneric : FFileManager
    {
        public FFileManagerGeneric()
        {

        }

        // ---------------------------------------------------------------
        // Makes reader writer
        // ---------------------------------------------------------------
        public override FArchive CreateFileReader(string filename, EFileRead readFlags = EFileRead.None, IOutputDevice error = null)
        {

            if (filename == null) throw new ArgumentNullException("filename");
            FileStream fs = null;

            var fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else
            {
                if (readFlags.IsFlagSet(EFileRead.NoFail))
                    fs = new FileStream(filename, FileMode.Create, FileAccess.Read, FileShare.ReadWrite);
                else
                    throw new FileNotFoundException(filename);
            }
            return new FBinaryStreamReader(fs, error);
        }
        public override FArchive CreateFileWriter(string filename, EFileWrite writeFlags = EFileWrite.None, IOutputDevice error = null)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            FileStream fs = null;

            var fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                if (writeFlags.IsFlagSet(EFileWrite.NoReplaceExisting))
                    throw new FileLoadException("Can't replace existing file: " + filename);

                if (fileInfo.IsReadOnly)
                {
                    if (writeFlags.IsFlagSet(EFileWrite.ReadOnly))
                        fileInfo.IsReadOnly = false;
                    else
                        throw new FileLoadException("Can't write to read only file: " + filename);
                }

                if (writeFlags.IsFlagSet(EFileWrite.Append))
                {
                    fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
                }
                else
                {
                    fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                }
            }
            else
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            return new FBinaryStreamWriter(fs, error);
        }
        // ---------------------------------------------------------------
        // File operations
        // ---------------------------------------------------------------
        public override bool IsReadOnly([NotNull] string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            var fileInfo = new FileInfo(filename);
            return fileInfo.Exists ? fileInfo.IsReadOnly : false;
        }
        public override long FileSize([NotNull] string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            var fileInfo = new FileInfo(filename);
            return fileInfo.Exists ? fileInfo.Length : 0;
        }
        //todo: add or remove argument: attributes
        //todo: add or remove argument: progress
        public override bool Copy(string dest, string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, OnProgressDelegate progress = null)
        {
            if (dest == null) throw new ArgumentNullException("dest");
            if (source == null) throw new ArgumentNullException("source");
            if (FileExist(dest))
            {
                var fileInfo = new FileInfo(dest);

                if (replace)
                {
                    if (fileInfo.IsReadOnly)
                    {
                        if (evenReadOnly)
                            Delete(dest, false, true);
                        else
                            return false;
                    }
                    else
                    {
                        Delete(dest, false, true);
                    }
                }
                else
                    return false;
            }
            File.Copy(source, dest);
            return true;
        }
        //todo: add or remove argument: attributes
        //todo: add or remove argument: bDoNotRetryOrError
        public override bool Move([NotNull] string dest, [NotNull] string source, bool replace = true, bool evenReadOnly = false, bool attributes = false, bool bDoNotRetryOrError = false)
        {
            if(dest == null) throw new ArgumentNullException("dest");
            if(source == null) throw new ArgumentNullException("source");
            if (FileExist(dest))
            {
                var fileInfo = new FileInfo(dest);

                if (replace)
                {
                    if (fileInfo.IsReadOnly)
                    {
                        if (evenReadOnly)
                            Delete(dest, false, true);
                        else
                            return false;
                    }
                    else
                    {
                        Delete(dest, false, true);
                    }
                }
                else
                    return false;
            }
            File.Move(source, dest);
            return true;
        }
        public override bool Delete([NotNull] string filename, bool requireExists = false, bool evenReadOnly = false, bool quiet = false)
        {
            if (filename == null) throw new ArgumentNullException("filename");

            if (!FileExist(filename))
            {
                if (!requireExists)
                    return true;
                if (quiet)
                    Debug.LogFormat("Delete non existed file: '{0}'", filename);
                else
                    throw new FileNotFoundException("File is not exist", filename);
            }

            var f = new FileInfo(filename);
            if (f.IsReadOnly)
            {
                if (evenReadOnly)
                {
                    f.IsReadOnly = false; //File.SetAttributes(filename, FileAttributes.Normal);
                }
                else
                {
                    if (!quiet) Debug.LogFormat("Can't delete read only file: '{0}'", filename);
                    return false;
                }
            }
            if (!quiet) Debug.LogFormat("Delete file: '{0}'", filename);
            File.Delete(filename);
            return true;
        }
        public override bool FileExist([NotNull] string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            return File.Exists(filename);
        }
        // ---------------------------------------------------------------
        // directory Listing
        // ---------------------------------------------------------------
        public override void FindFiles([NotNull] ref List<string> foundNames, [NotNull] string path, bool files, bool directories)
        {
            if (foundNames == null) throw new ArgumentNullException("foundNames");
            if (path == null) throw new ArgumentNullException("path");

            var startDirectory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            if (fileName == null) fileName = "*";
            if (string.IsNullOrEmpty(startDirectory))
                startDirectory = GetDefaultDirectory();

            if (files)
            {
                var list = Directory.GetFiles(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var f in list) foundNames.Add(f);
            }
            if (directories)
            {
                var list = Directory.GetDirectories(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var d in list) foundNames.Add(d);
            }
        }
        public override void FindFilesRecursive([NotNull] ref List<string> foundNames, [NotNull] string startDirectory,
            string fileName, bool files, bool directories)
        {
            if (foundNames == null) throw new ArgumentNullException("foundNames");
            if (startDirectory == null) throw new ArgumentNullException("startDirectory");

            if (string.IsNullOrEmpty(startDirectory))
                startDirectory = GetDefaultDirectory();

            if (fileName == null) fileName = "*";
            if (files)
            {   // find files in this directory
                var list = Directory.GetFiles(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var f in list) foundNames.Add(f);
            }
            if (directories)
            {
                // find directories to match pattern
                var list = Directory.GetDirectories(startDirectory, fileName, SearchOption.TopDirectoryOnly);
                foreach (var d in list) foundNames.Add(d);
            }
            // find all directories, to look inside
            var dirs = Directory.GetDirectories(startDirectory);
            foreach (var d in dirs)
                FindFilesRecursive(ref foundNames, d, fileName, files, directories);
        }
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public override bool DirectoryExists(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            return Directory.Exists(path);
        }
        // Create directory with @path. Create all directory tree if @tree is true 
        public override bool MakeDirectory([NotNull] string path, bool tree = false)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (tree)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                var dirPath = Path.GetDirectoryName(path);
                if (Directory.Exists(dirPath))
                {
                    var info = Directory.CreateDirectory(path);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool DeleteDirectory(string path, bool requireExists = false, bool tree = false)
        {
            if (path == null) throw new ArgumentNullException("path");

            if (DirectoryExists(path))
            {
                if (tree)
                   Directory.Delete(path, true);
                else
                   Directory.Delete(path);
                return true;
            }
            else
            {
                if (requireExists)
                    throw new DirectoryNotFoundException("Directory is not exists " + path);
                return false;
            }
        }
        // ---------------------------------------------------------------
        // Default directory
        // ---------------------------------------------------------------
        public override bool SetDefaultDirectory([NotNull] string dirPath, bool requireTarget)
        {
            if (dirPath == null) throw new ArgumentNullException("dirPath");
            if (DirectoryExists(dirPath))
            {
                Directory.SetCurrentDirectory(dirPath);
                return true;
            }
            if (requireTarget)
                throw new DirectoryNotFoundException("Directory is not found " + dirPath);
            return false;
        }
        public override string GetDefaultDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
        // ---------------------------------------------------------------
        // Directories
        // ---------------------------------------------------------------
        public override string ConvertToRelativePath([NotNull] string absolutePath)
        {
            if (absolutePath == null) throw new ArgumentNullException("absolutePath");
            var fileUri = new Uri(absolutePath);
            var cd = new Uri(GetDefaultDirectory());
            return cd.MakeRelativeUri(fileUri).ToString();
        }
        public override string ConvertToAbsolutePath([NotNull] string relativePath)
        {
            if (relativePath == null) throw new ArgumentNullException("relativePath");
            return Path.GetFullPath(relativePath);
        }

        // ---------------------------------------------------------------
        // Utilities
        // ---------------------------------------------------------------
        //private string GetFullPath(string path)
        //{
        //    return Path.GetFullPath(path);
        //}
        //private string GetPathRoot(string path)
        //{
        //    return Path.GetPathRoot(path);
        //}
        //private bool IsRootPath(string path)
        //{
        //    return path == GetPathRoot(path);
        //}
        
    };



}
