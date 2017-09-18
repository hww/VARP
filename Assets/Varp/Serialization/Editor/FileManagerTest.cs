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


using NUnit.Framework;

namespace VARP.Serialization.Test
{
    using System.Collections.Generic;
    using VARP.Serialization;

    public class FileManagerTest
    {
        public class TestData : IArchived
        {
            public bool v2 = false;
            public char v3 = 'a';
            public sbyte v4 = -10;
            public byte v5 = 10;
            public short v6 = -1000;
            public ushort v7 = 1000;
            public int v8 = -1000;
            public uint v9 = 1000;
            public float v10 = 3.14f;
            public double v11 = 3.14;

            public TestData()
            {

            }

            public void Serialize(FArchive ar)
            {
                ar.Ar(ref v2);
                ar.Ar(ref v3);
                ar.Ar(ref v4);
                ar.Ar(ref v5);
                ar.Ar(ref v6);
                ar.Ar(ref v7);
                ar.Ar(ref v8);
                ar.Ar(ref v9);
                ar.Ar(ref v10);
                ar.Ar(ref v11);
            }

        }


        string filename = "FileManager_Test_File.x";

        private void WriteFile(FFileManagerGeneric manager, TestData writeData = null)
        {
            // create new file
            var writer = manager.CreateFileWriter(filename, EFileWrite.ReadOnly);

            Assert.IsTrue(manager.FileExist(filename), "CreateFileWriter: File was not created: " + filename);
            Assert.AreEqual(0, manager.FileSize(filename), "FileSize: expected file with 0 lenght");

            // write to te file
            if (writeData == null) writeData = new TestData();
            writeData.Serialize(writer);
            // close the file
            writer.Close();
        }

        [Test]
        public void SerializeDeserializeFile()
        {
            var manager = new FFileManagerGeneric();
            var writeData = new TestData();
            WriteFile(manager, writeData);

            // open file to read
            var reader = manager.CreateFileReader(filename);
            var readData = new TestData();
            readData.Serialize(reader);
            reader.Close();
            Assert.AreEqual(writeData.v2, readData.v2);
            Assert.AreEqual(writeData.v3, readData.v3);
            Assert.AreEqual(writeData.v4, readData.v4);
            Assert.AreEqual(writeData.v5, readData.v5);
            Assert.AreEqual(writeData.v6, readData.v6);
            Assert.AreEqual(writeData.v7, readData.v7);
            Assert.AreEqual(writeData.v8, readData.v8);
            Assert.AreEqual(writeData.v9, readData.v9);
            Assert.AreEqual(writeData.v10, readData.v10);
            Assert.AreEqual(writeData.v11, readData.v11);

            Assert.AreNotEqual(0, manager.FileSize(filename), "FileSize: expected file more that 0 lenght");
        }

        [Test]
        public void CopyMoveApiTest()
        {
            var manager = new FFileManagerGeneric();
            manager.Delete(filename, false, true);
            Assert.IsFalse(manager.FileExist(filename), "FileExist: File should not exist: " + filename);

            var filenameCopy = filename + ".copy";
            manager.Delete(filenameCopy, false, true);
            Assert.IsFalse(manager.FileExist(filenameCopy), "FileExist: File should not exist: " + filenameCopy);

            var filenameMove = filename + ".move";
            manager.Delete(filenameMove, false, true);
            Assert.IsFalse(manager.FileExist(filenameMove), "FileExist: File should not exist: " + filenameMove);

            // create new file
            WriteFile(manager);

            // copy file
            manager.Copy(filenameCopy, filename, true, true);
            Assert.IsTrue(manager.FileExist(filenameCopy), "FileExist: File not exist: " + filenameCopy);

            // move file
            manager.Move(filenameMove, filenameCopy, true, true);
            Assert.IsTrue(manager.FileExist(filenameMove), "FileExist: File not exist: " + filenameMove);
            Assert.IsFalse(manager.FileExist(filenameCopy), "FileExist: File not exist: " + filenameCopy);

            // delete files
            manager.Delete(filename, false, true);
            manager.Delete(filenameMove, false, true);
        }

        [Test]
        public void FindFiles()
        {
            var manager = new FFileManagerGeneric();
            List<string> foundNames = new List<string>();
            manager.FindFiles(ref foundNames, "Assets", false, true);
            Assert.AreEqual(1, foundNames.Count, "Quantity of directories with name Assets is wrong");

            manager.FindFilesRecursive(ref foundNames, manager.GetDefaultDirectory(), "FileManagerTest.cs", true, false);
            Assert.GreaterOrEqual(foundNames.Count,1, "Quantity of files is wrong");

            
        }

        [Test]
        public void CreateDeleteDirectory()
        {
            string dirName = "FileManager_Test";
            string dirNameFull = "FileManager_Test/Test1/Test2";

            var manager = new FFileManagerGeneric();
            manager.DeleteDirectory(dirName, false, true);
            Assert.IsFalse(manager.DirectoryExists(dirName), "DirectoryExists: File should not exist: " + dirName);

            manager.MakeDirectory(dirNameFull, true);
            Assert.IsTrue(manager.DirectoryExists(dirName), "DirectoryExists: File should not exist: " + dirName);
            Assert.IsTrue(manager.DirectoryExists(dirNameFull), "DirectoryExists: File should not exist: " + dirNameFull);

            manager.DeleteDirectory(dirName, false, true);
            Assert.IsFalse(manager.DirectoryExists(dirName), "DirectoryExists: File should not exist: " + dirName);
        }
    }
}