//
// FileTest.cs: Test cases for System.IO.File
//
// Author: 
//     Duncan Mak (duncan@ximian.com)
//     Ville Palo (vi64pa@kolumbus.fi)
//
// (C) 2002 Ximian, Inc. http://www.ximian.com
//

using NUnit.Framework;
using System;
using System.IO;
using System.Globalization;
using System.Threading;

namespace MonoTests.System.IO
{
	public class FileTest : Assertion
	{
		static string TempFolder = Path.Combine (Path.GetTempPath (), "MonoTests.System.IO.Tests");

		[SetUp]
		public void SetUp ()
		{
			if (Directory.Exists (TempFolder))
				Directory.Delete (TempFolder, true);
			Directory.CreateDirectory (TempFolder);
		
                        Thread.CurrentThread.CurrentCulture = new CultureInfo ("EN-us");
		}

		[TearDown]
		public void TearDown ()
		{
			if (Directory.Exists (TempFolder))
				Directory.Delete (TempFolder, true);
		}

		[Test]
		public void TestExists ()
		{
			int i = 0;
			try {
				Assert ("null filename should not exist", !File.Exists (null));
				i++;
				Assert ("empty filename should not exist", !File.Exists (""));
				i++;
				Assert ("whitespace filename should not exist", !File.Exists ("  \t\t  \t \n\t\n \n"));
				i++;				
				string path = TempFolder + Path.DirectorySeparatorChar + "AFile.txt";
				DeleteFile (path);
				File.Create (path).Close ();
				Assert ("File " + path + " should exists", File.Exists (path));
				i++;
				Assert ("File resources" + Path.DirectorySeparatorChar + "doesnotexist should not exist", !File.Exists (TempFolder + Path.DirectorySeparatorChar + "doesnotexist"));
			} catch (Exception e) {
				Fail ("Unexpected exception at i = " + i + ". e=" + e);
			}
			
			
		}

		[Test]
		public void TestCreate ()
		{
			FileStream stream;

			/* exception test: File.Create(null) */
			try {
				stream = File.Create (null);
				Fail ("File.Create(null) should throw ArgumentNullException");
			} catch (ArgumentNullException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Create(null) unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Create("") */
			try {
				stream = File.Create ("");
				Fail ("File.Create('') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Create('') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Create(" ") */
			try {
				stream = File.Create (" ");
				Fail ("File.Create(' ') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Create(' ') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Create(directory_not_found) */
			try {
				stream = File.Create (TempFolder + Path.DirectorySeparatorChar + "directory_does_not_exist" + Path.DirectorySeparatorChar + "foo");
				Fail ("File.Create(directory_does_not_exist) should throw DirectoryNotFoundException");
			} catch (DirectoryNotFoundException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Create(directory_does_not_exist) unexpected exception caught: e=" + e.ToString());
			}


			/* positive test: create resources/foo */
			try {
				stream = File.Create (TempFolder + Path.DirectorySeparatorChar + "foo");
				Assert ("File should exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "foo"));
				stream.Close ();
			} catch (Exception e) {
				Fail ("File.Create(resources/foo) unexpected exception caught: e=" + e.ToString());
			}

			/* positive test: repeat test above again to test for overwriting file */
			try {
				stream = File.Create (TempFolder + Path.DirectorySeparatorChar + "foo");
				Assert ("File should exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "foo"));
				stream.Close ();
			} catch (Exception e) {
				Fail ("File.Create(resources/foo) unexpected exception caught: e=" + e.ToString());
			}
		}

		[Test]
		public void TestCopy ()
		{
			/* exception test: File.Copy(null, b) */
			try {
				File.Copy (null, "b");
				Fail ("File.Copy(null, 'b') should throw ArgumentNullException");
			} catch (ArgumentNullException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy(null, 'b') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Copy(a, null) */
			try {
				File.Copy ("a", null);
				Fail ("File.Copy('a', null) should throw ArgumentNullException");
			} catch (ArgumentNullException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy('a', null) unexpected exception caught: e=" + e.ToString());
			}


			/* exception test: File.Copy("", b) */
			try {
				File.Copy ("", "b");
				Fail ("File.Copy('', 'b') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy('', 'b') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Copy(a, "") */
			try {
				File.Copy ("a", "");
				Fail ("File.Copy('a', '') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy('a', '') unexpected exception caught: e=" + e.ToString());
			}


			/* exception test: File.Copy(" ", b) */
			try {
				File.Copy (" ", "b");
				Fail ("File.Copy(' ', 'b') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy(' ', 'b') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Copy(a, " ") */
			try {
				File.Copy ("a", " ");
				Fail ("File.Copy('a', ' ') should throw ArgumentException");
			} catch (ArgumentException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy('a', ' ') unexpected exception caught: e=" + e.ToString());
			}


			/* exception test: File.Copy(doesnotexist, b) */
			try {
				File.Copy ("doesnotexist", "b");
				Fail ("File.Copy('doesnotexist', 'b') should throw FileNotFoundException");
			} catch (FileNotFoundException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("File.Copy('doesnotexist', 'b') unexpected exception caught: e=" + e.ToString());
			}

			/* positive test: copy resources/AFile.txt to resources/bar */
			try {
				DeleteFile (TempFolder + Path.DirectorySeparatorChar + "bar");
				DeleteFile (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");

				File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt").Close ();
				File.Copy (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", TempFolder + Path.DirectorySeparatorChar + "bar");
				Assert ("File AFile.txt should still exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"));
				Assert ("File bar should exist after File.Copy", File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar"));
			} catch (Exception e) {
				Fail ("#1 File.Copy('resources/AFile.txt', 'resources/bar') unexpected exception caught: e=" + e.ToString());
			}

			/* exception test: File.Copy(resources/AFile.txt, resources/bar) (default is overwrite == false) */
			try {
				File.Copy (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", TempFolder + Path.DirectorySeparatorChar + "bar");
				Fail ("File.Copy('resources/AFile.txt', 'resources/bar') should throw IOException");
			} catch (IOException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("#2 File.Copy('resources/AFile.txt', 'resources/bar') unexpected exception caught: e=" + e.ToString());
			}


			/* positive test: copy resources/AFile.txt to resources/bar, overwrite */
			try {
				Assert ("File bar should exist before File.Copy", File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar"));
				File.Copy (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", TempFolder + Path.DirectorySeparatorChar + "bar", true);
				Assert ("File AFile.txt should still exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"));
				Assert ("File bar should exist after File.Copy", File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar"));
			} catch (Exception e) {
				Fail ("File.Copy('resources/AFile.txt', 'resources/bar', true) unexpected exception caught: e=" + e.ToString());
			}


		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void DeleteArgumentNullException ()
		{
			File.Delete (null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void DeleteArgumentException1 ()
		{
			File.Delete ("");
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void DeleteArgumentException2 ()
		{
			File.Delete (" ");
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void DeleteDirectoryNotFoundException ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "directory_does_not_exist" + Path.DirectorySeparatorChar + "foo";
			if (Directory.Exists (TempFolder + Path.DirectorySeparatorChar + "directory_does_not_exist"))
				Directory.Delete (TempFolder + Path.DirectorySeparatorChar + "directory_does_not_exist", true);
			File.Delete (path);			
			DeleteFile (path);
		}


		[Test]
		public void TestDelete ()
		{
			string foopath = TempFolder + Path.DirectorySeparatorChar + "foo";
			DeleteFile (foopath);
			File.Create (foopath).Close ();

                        try {
                                File.Delete (foopath);
                        } catch (Exception e) {
                                Fail ("Unable to delete " + foopath + " e=" + e.ToString());
                        }
			Assert ("File " + foopath + " should not exist after File.Delete", !File.Exists (foopath));
			DeleteFile (foopath);
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void DeleteOpenStreamException ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "DeleteOpenStreamException";
			DeleteFile (path);			
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			File.Delete (path);
		}
		
		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void MoveException1 ()
		{
	                File.Move (null, "b");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void MoveException2 ()
		{
			File.Move ("a", null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void MoveException3 ()
		{
                	File.Move ("", "b");
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void MoveException4 ()
		{
                	File.Move ("a", "");
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void MoveException5 ()
		{
                        File.Move (" ", "b");			
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void MoveException6 ()
		{
                	File.Move ("a", " ");
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void MoveException7 ()
		{
                       	DeleteFile (TempFolder + Path.DirectorySeparatorChar + "doesnotexist");			
                        File.Move (TempFolder + Path.DirectorySeparatorChar + "doesnotexist", "b");
		}

		[Test]
		[ExpectedException(typeof (DirectoryNotFoundException))]
		public void MoveException8 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "foo";
                        DeleteFile (path);
			File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt").Close ();
                        File.Copy(TempFolder + Path.DirectorySeparatorChar + "AFile.txt", path, true);
                        DeleteFile (TempFolder + Path.DirectorySeparatorChar + "doesnotexist" + Path.DirectorySeparatorChar + "b");
                        File.Move (TempFolder + Path.DirectorySeparatorChar + "foo", TempFolder + Path.DirectorySeparatorChar + "doesnotexist" + Path.DirectorySeparatorChar + "b");
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void MoveException9 ()
		{
                	File.Move (TempFolder + Path.DirectorySeparatorChar + "foo", TempFolder);		
		}

		[Test]
		public void TestMove ()
		{
			if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar")) {
				FileStream f = File.Create(TempFolder + Path.DirectorySeparatorChar + "bar");
				f.Close();
			}
			
			Assert ("File " + TempFolder + Path.DirectorySeparatorChar + "bar should exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar"));
			File.Move (TempFolder + Path.DirectorySeparatorChar + "bar", TempFolder + Path.DirectorySeparatorChar + "baz");
			Assert ("File " + TempFolder + Path.DirectorySeparatorChar + "bar should not exist", !File.Exists (TempFolder + Path.DirectorySeparatorChar + "bar"));
			Assert ("File " + TempFolder + Path.DirectorySeparatorChar + "baz should exist", File.Exists (TempFolder + Path.DirectorySeparatorChar + "baz"));
		}

		[Test]
		public void TestOpen ()
		{
                        try {
				if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"))
					File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");
                                FileStream stream = File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.Open);
				stream.Close ();
                        } catch (Exception e) {
                                Fail ("Unable to open " + TempFolder + Path.DirectorySeparatorChar + "AFile.txt: e=" + e.ToString());
                        }

                        /* Exception tests */
			try {
				FileStream stream = File.Open (TempFolder + Path.DirectorySeparatorChar + "filedoesnotexist", FileMode.Open);
				Fail ("File 'filedoesnotexist' should not exist");
			} catch (FileNotFoundException) {
				// do nothing, this is what we expect
			} catch (Exception e) {
				Fail ("Unexpect exception caught: e=" + e.ToString());
			}
		}

                [Test]
                public void Open () 
                {
			if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"))
				File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt").Close ();

			FileStream stream = File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.Open);
                	
                	Assertion.AssertEquals ("test#01", true, stream.CanRead);
                	Assertion.AssertEquals ("test#02", true, stream.CanSeek);
                	Assertion.AssertEquals ("test#03", true, stream.CanWrite);
                	stream.Close ();
                	
                	stream = File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.Open, FileAccess.Write);
                	Assertion.AssertEquals ("test#04", false, stream.CanRead);
                	Assertion.AssertEquals ("test#05", true, stream.CanSeek);
                	Assertion.AssertEquals ("test#06", true, stream.CanWrite);
                	stream.Close ();
		                	
                	stream = File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.Open, FileAccess.Read);
                	Assertion.AssertEquals ("test#04", true, stream.CanRead);
                	Assertion.AssertEquals ("test#05", true, stream.CanSeek);
                	Assertion.AssertEquals ("test#06", false, stream.CanWrite);
                	stream.Close ();
                }
                
                [Test]
                [ExpectedException(typeof(ArgumentException))]
                public void OpenException1 ()
                {
                	// CreateNew + Read throws an exceptoin
                	File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.CreateNew, FileAccess.Read);
                }

                [Test]
                [ExpectedException(typeof(ArgumentException))]
                public void OpenException2 ()
                {
                	// Append + Read throws an exceptoin
			if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"))
				File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");

                	File.Open (TempFolder + Path.DirectorySeparatorChar + "AFile.txt", FileMode.Append, FileAccess.Read);
                }
                
                [Test]
                public void OpenRead ()
                {
			if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"))
				File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");
			FileStream stream = File.OpenRead (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");
                	Assertion.AssertEquals ("test#01", true, stream.CanRead);
                	Assertion.AssertEquals ("test#02", true, stream.CanSeek);
                	Assertion.AssertEquals ("test#03", false, stream.CanWrite);
                	stream.Close ();                				                	
                }

                [Test]
                public void OpenWrite ()
                {
			if (!File.Exists (TempFolder + Path.DirectorySeparatorChar + "AFile.txt"))
				File.Create (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");
			FileStream stream = File.OpenWrite (TempFolder + Path.DirectorySeparatorChar + "AFile.txt");
                	Assertion.AssertEquals ("test#01", false, stream.CanRead);
                	Assertion.AssertEquals ("test#02", true, stream.CanSeek);
                	Assertion.AssertEquals ("test#03", true, stream.CanWrite);
                	stream.Close ();                				                	
                }

		[Test]
		public void TestGetCreationTime ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "baz";
                	DeleteFile (path);
                	
			File.Create (path).Close();
                	DateTime time = File.GetCreationTime (path);
                	time = time.ToLocalTime ();
                        Assert ("GetCreationTime incorrect", (DateTime.Now - time).TotalSeconds < 10);
                	DeleteFile (path);
		}

                [Test]
                [ExpectedException(typeof(IOException))]
                public void TestGetCreationTimeException ()
                {
                        // Test nonexistent files
                        string path2 = TempFolder + Path.DirectorySeparatorChar + "filedoesnotexist";
			DeleteFile (path2);
                        // should throw an exception
                        File.GetCreationTime (path2);
                }
                


                [Test]
                public void CreationTime ()
                {
                        string path = TempFolder + Path.DirectorySeparatorChar + "creationTime";                	
                        if (File.Exists (path))
                        	File.Delete (path);
                        	
                       	FileStream stream = File.Create (path);
                	stream.Close ();                	
                	
                	File.SetCreationTime (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	DateTime time = File.GetCreationTime (path);
                	Assertion.AssertEquals ("test#01", 2002, time.Year);
                	Assertion.AssertEquals ("test#02", 4, time.Month);
                	Assertion.AssertEquals ("test#03", 6, time.Day);
                	Assertion.AssertEquals ("test#04", 4, time.Hour);
                	Assertion.AssertEquals ("test#05", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToLocalTime (File.GetCreationTimeUtc (path));
                	Assertion.AssertEquals ("test#06", 2002, time.Year);
                	Assertion.AssertEquals ("test#07", 4, time.Month);
                	Assertion.AssertEquals ("test#08", 6, time.Day);
                	Assertion.AssertEquals ("test#09", 4, time.Hour);
                	Assertion.AssertEquals ("test#10", 4, time.Second);                	

                	File.SetCreationTimeUtc (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	time = File.GetCreationTimeUtc (path);
                	Assertion.AssertEquals ("test#11", 2002, time.Year);
                	Assertion.AssertEquals ("test#12", 4, time.Month);
                	Assertion.AssertEquals ("test#13", 6, time.Day);
                	Assertion.AssertEquals ("test#14", 4, time.Hour);
                	Assertion.AssertEquals ("test#15", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToUniversalTime (File.GetCreationTime (path));
                	Assertion.AssertEquals ("test#16", 2002, time.Year);
                	Assertion.AssertEquals ("test#17", 4, time.Month);
                	Assertion.AssertEquals ("test#18", 6, time.Day);
                	Assertion.AssertEquals ("test#19", 4, time.Hour);
                	Assertion.AssertEquals ("test#20", 4, time.Second);
                }

                [Test]
                public void LastAccessTime ()
                {
                        string path = TempFolder + Path.DirectorySeparatorChar + "lastAccessTime";                	
                        if (File.Exists (path))
                        	File.Delete (path);
                        	
                       	FileStream stream = File.Create (path);
                	stream.Close ();                	
                	
                	File.SetLastAccessTime (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	DateTime time = File.GetLastAccessTime (path);
                	Assertion.AssertEquals ("test#01", 2002, time.Year);
                	Assertion.AssertEquals ("test#02", 4, time.Month);
                	Assertion.AssertEquals ("test#03", 6, time.Day);
                	Assertion.AssertEquals ("test#04", 4, time.Hour);
                	Assertion.AssertEquals ("test#05", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToLocalTime (File.GetLastAccessTimeUtc (path));
                	Assertion.AssertEquals ("test#06", 2002, time.Year);
                	Assertion.AssertEquals ("test#07", 4, time.Month);
                	Assertion.AssertEquals ("test#08", 6, time.Day);
                	Assertion.AssertEquals ("test#09", 4, time.Hour);
                	Assertion.AssertEquals ("test#10", 4, time.Second);                	

                	File.SetLastAccessTimeUtc (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	time = File.GetLastAccessTimeUtc (path);
                	Assertion.AssertEquals ("test#11", 2002, time.Year);
                	Assertion.AssertEquals ("test#12", 4, time.Month);
                	Assertion.AssertEquals ("test#13", 6, time.Day);
                	Assertion.AssertEquals ("test#14", 4, time.Hour);
                	Assertion.AssertEquals ("test#15", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToUniversalTime (File.GetLastAccessTime (path));
                	Assertion.AssertEquals ("test#16", 2002, time.Year);
                	Assertion.AssertEquals ("test#17", 4, time.Month);
                	Assertion.AssertEquals ("test#18", 6, time.Day);
                	Assertion.AssertEquals ("test#19", 4, time.Hour);
                	Assertion.AssertEquals ("test#20", 4, time.Second);
                }

                [Test]
                public void LastWriteTime ()
                {
                        string path = TempFolder + Path.DirectorySeparatorChar + "lastWriteTime";                	
                        if (File.Exists (path))
                        	File.Delete (path);
                        	
                       	FileStream stream = File.Create (path);
                	stream.Close ();                	
                	
                	File.SetLastWriteTime (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	DateTime time = File.GetLastWriteTime (path);
                	Assertion.AssertEquals ("test#01", 2002, time.Year);
                	Assertion.AssertEquals ("test#02", 4, time.Month);
                	Assertion.AssertEquals ("test#03", 6, time.Day);
                	Assertion.AssertEquals ("test#04", 4, time.Hour);
                	Assertion.AssertEquals ("test#05", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToLocalTime (File.GetLastWriteTimeUtc (path));
                	Assertion.AssertEquals ("test#06", 2002, time.Year);
                	Assertion.AssertEquals ("test#07", 4, time.Month);
                	Assertion.AssertEquals ("test#08", 6, time.Day);
                	Assertion.AssertEquals ("test#09", 4, time.Hour);
                	Assertion.AssertEquals ("test#10", 4, time.Second);                	

                	File.SetLastWriteTimeUtc (path, new DateTime (2002, 4, 6, 4, 6, 4));
                	time = File.GetLastWriteTimeUtc (path);
                	Assertion.AssertEquals ("test#11", 2002, time.Year);
                	Assertion.AssertEquals ("test#12", 4, time.Month);
                	Assertion.AssertEquals ("test#13", 6, time.Day);
                	Assertion.AssertEquals ("test#14", 4, time.Hour);
                	Assertion.AssertEquals ("test#15", 4, time.Second);
                	
                	time = TimeZone.CurrentTimeZone.ToUniversalTime (File.GetLastWriteTime (path));
                	Assertion.AssertEquals ("test#16", 2002, time.Year);
                	Assertion.AssertEquals ("test#17", 4, time.Month);
                	Assertion.AssertEquals ("test#18", 6, time.Day);
                	Assertion.AssertEquals ("test#19", 4, time.Hour);
                	Assertion.AssertEquals ("test#20", 4, time.Second);
                }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetCreationTimeException1 ()
		{
			File.GetCreationTime (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeException2 ()
		{
			File.GetCreationTime ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetCreationTimeException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetCreationTimeException3";                	
			DeleteFile (path);		
			File.GetCreationTime (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeException4 ()
		{
			File.GetCreationTime ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeException5 ()
		{
			File.GetCreationTime (Path.InvalidPathChars [0].ToString ());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetCreationTimeUtcException1 ()
		{
			File.GetCreationTimeUtc (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeUtcException2 ()
		{
			File.GetCreationTimeUtc ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetCreationTimeUtcException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetCreationTimeUtcException3";                	
			DeleteFile (path);		
			File.GetCreationTimeUtc (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeUtcException4 ()
		{
			File.GetCreationTimeUtc ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetCreationTimeUtcException5 ()
		{
			File.GetCreationTime (Path.InvalidPathChars [0].ToString ());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetLastAccessTimeException1 ()
		{
			File.GetLastAccessTime (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeException2 ()
		{
			File.GetLastAccessTime ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetLastAccessTimeException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetLastAccessTimeException3";                	
			DeleteFile (path);		
			File.GetLastAccessTime (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeException4 ()
		{
			File.GetLastAccessTime ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeException5 ()
		{
			File.GetLastAccessTime (Path.InvalidPathChars [0].ToString ());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetLastAccessTimeUtcException1 ()
		{
			File.GetLastAccessTimeUtc (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeUtcException2 ()
		{
			File.GetLastAccessTimeUtc ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetLastAccessTimeUtcException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetLastAccessTimeUtcException3";                	
			DeleteFile (path);			
			File.GetLastAccessTimeUtc (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeUtcException4 ()
		{
			File.GetLastAccessTimeUtc ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastAccessTimeUtcException5 ()
		{
			File.GetLastAccessTimeUtc (Path.InvalidPathChars [0].ToString ());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetLastWriteTimeException1 ()
		{
			File.GetLastWriteTime (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeException2 ()
		{
			File.GetLastWriteTime ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetLastWriteTimeException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetLastAccessTimeUtcException3";                	
			DeleteFile (path);			
			File.GetLastWriteTime (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeException4 ()
		{
			File.GetLastWriteTime ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeException5 ()
		{
			File.GetLastWriteTime (Path.InvalidPathChars [0].ToString ());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]	
		public void GetLastWriteTimeUtcException1 ()
		{
			File.GetLastWriteTimeUtc (null as string);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeUtcException2 ()
		{
			File.GetLastAccessTimeUtc ("");
		}
	
		[Test]
		[ExpectedException(typeof(IOException))]
		public void GetLastWriteTimeUtcException3 ()
		{
                        string path = TempFolder + Path.DirectorySeparatorChar + "GetLastWriteTimeUtcException3";
			DeleteFile (path);
			File.GetLastAccessTimeUtc (path);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeUtcException4 ()
		{
			File.GetLastAccessTimeUtc ("    ");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]	
		public void GetLastWriteTimeUtcException5 ()
		{
			File.GetLastAccessTimeUtc (Path.InvalidPathChars [0].ToString ());
		}		

		[Test]
		[ExpectedException(typeof(IOException))]
		public void FileStreamCloseException ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "FileStreamCloseException";
			DeleteFile (path);			
			FileStream stream = File.Create (path);
			File.Delete (path);
		}

		[Test]
		public void FileStreamClose ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "FileStreamClose";
			FileStream stream = File.Create (path);
			stream.Close ();
			File.Delete (path);
		}
		
		// SetCreationTime and SetCreationTimeUtc exceptions

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetCreationTimeArgumentNullException1 ()
		{
			File.SetCreationTime (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeArgumenException1 ()
		{
			File.SetCreationTime ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeArgumenException2 ()
		{
			File.SetCreationTime ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeArgumenException3 ()
		{
			File.SetCreationTime (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetCreationTimeFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetCreationTimeFileNotFoundException1";
			DeleteFile (path);
			
			File.SetCreationTime (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetCreationTimeArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetCreationTimeArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetCreationTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetCreationTimeIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "CreationTimeIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetCreationTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetCreationTimeUtcArgumentNullException1 ()
		{
			File.SetCreationTimeUtc (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeUtcArgumenException1 ()
		{
			File.SetCreationTimeUtc ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeUtcArgumenException2 ()
		{
			File.SetCreationTimeUtc ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCreationTimeUtcArgumenException3 ()
		{
			File.SetCreationTimeUtc (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetCreationTimeUtcFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetCreationTimeUtcFileNotFoundException1";
			DeleteFile (path);
			
			File.SetCreationTimeUtc (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetCreationTimeUtcArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetCreationTimeUtcArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetCreationTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetCreationTimeUtcIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetCreationTimeUtcIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetCreationTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		// SetLastAccessTime and SetLastAccessTimeUtc exceptions

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetLastAccessTimeArgumentNullException1 ()
		{
			File.SetLastAccessTime (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastAccessTimeArgumenException1 ()
		{
			File.SetLastAccessTime ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastAccessTimeArgumenException2 ()
		{
			File.SetLastAccessTime ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastAccessTimeArgumenException3 ()
		{
			File.SetLastAccessTime (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetLastAccessTimeFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastAccessTimeFileNotFoundException1";
			DeleteFile (path);
			
			File.SetLastAccessTime (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetLastAccessTimeArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastTimeArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetLastAccessTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetLastAccessTimeIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "LastAccessIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetLastAccessTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetLastAccessTimeUtcArgumentNullException1 ()
		{
			File.SetLastAccessTimeUtc (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCLastAccessTimeUtcArgumenException1 ()
		{
			File.SetLastAccessTimeUtc ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastAccessTimeUtcArgumenException2 ()
		{
			File.SetLastAccessTimeUtc ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastAccessTimeUtcArgumenException3 ()
		{
			File.SetLastAccessTimeUtc (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetLastAccessTimeUtcFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastAccessTimeUtcFileNotFoundException1";
			DeleteFile (path);
			
			File.SetLastAccessTimeUtc (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetLastAccessTimeUtcArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastAccessTimeUtcArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetLastAccessTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetLastAccessTimeUtcIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastAccessTimeUtcIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetLastAccessTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		// SetLastWriteTime and SetLastWriteTimeUtc exceptions

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetLastWriteTimeArgumentNullException1 ()
		{
			File.SetLastWriteTime (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastWriteTimeArgumenException1 ()
		{
			File.SetLastWriteTime ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastWriteTimeArgumenException2 ()
		{
			File.SetLastWriteTime ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastWriteTimeArgumenException3 ()
		{
			File.SetLastWriteTime (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetLastWriteTimeFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastWriteTimeFileNotFoundException1";
			DeleteFile (path);
			
			File.SetLastWriteTime (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetLastWriteTimeArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastWriteTimeArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetLastWriteTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetLastWriteTimeIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "LastWriteTimeIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetLastWriteTime (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SetLastWriteTimeUtcArgumentNullException1 ()
		{
			File.SetLastWriteTimeUtc (null as string, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetCLastWriteTimeUtcArgumenException1 ()
		{
			File.SetLastWriteTimeUtc ("", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastWriteTimeUtcArgumenException2 ()
		{
			File.SetLastWriteTimeUtc ("     ", new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void SetLastWriteTimeUtcArgumenException3 ()
		{
			File.SetLastWriteTimeUtc (Path.InvalidPathChars [1].ToString (), new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (FileNotFoundException))]
		public void SetLastWriteTimeUtcFileNotFoundException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastWriteTimeUtcFileNotFoundException1";
			DeleteFile (path);
			
			File.SetLastAccessTimeUtc (path, new DateTime (2000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SetLastWriteTimeUtcArgumentOutOfRangeException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastWriteTimeUtcArgumentOutOfRangeException1";
			DeleteFile (path);
			File.Create (path).Close ();
			File.SetLastWriteTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void SetLastWriteTimeUtcIOException1 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "SetLastWriteTimeUtcIOException1";
			DeleteFile (path);
			File.Create (path);
			File.SetLastAccessTimeUtc (path, new DateTime (1000, 12, 12, 11, 59, 59));
		}

		private void DeleteFile (string path)
		{
			if (File.Exists (path))
				File.Delete (path);
		}
	}
}
