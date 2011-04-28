// 
// AdbOperations.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2010 Novell, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using MonoDevelop.Core;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace MonoDevelop.MonoDroid
{
	public sealed class AdbCheckVersionOperation : AdbOperation
	{
		const string SUPPORTED_PROTOCOL = "001a";
		
		public AdbCheckVersionOperation ()
		{
			BeginConnect ();
		}
		
		protected override void OnConnected ()
		{
			WriteCommand ("host:version", () => GetStatus (() => ReadResponseWithLength (OnGotResponse)));
		}
		
		void OnGotResponse (string response)
		{
			if (response != SUPPORTED_PROTOCOL)
				throw new Exception ("Unsupported adb protocol: " + response);
			else
				SetCompleted (true);
		}
	}
	
	public sealed class AdbKillServerOperation : AdbOperation
	{
		public AdbKillServerOperation ()
		{
			BeginConnect ();
		}
		
		protected override void OnConnected ()
		{
			WriteCommand ("host:kill", () => SetCompleted (true));
		}
	}
	
	public sealed class AdbTrackDevicesOperation : AdbOperation
	{
		public AdbTrackDevicesOperation ()
		{
			BeginConnect ();
		}
		
		protected override void OnConnected ()
		{
			WriteCommand ("host:track-devices", () => GetStatus (() => ReadResponseWithLength (OnGotResponse)));
		}
		
		void OnGotResponse (string response)
		{
			Devices = Parse (response);
			var ev = DevicesChanged;
			if (ev != null)
				ev (Devices);
			ReadResponseWithLength (OnGotResponse);
		}
		
		List<AndroidDevice> Parse (string response)
		{
			var devices = new List<AndroidDevice> ();
			using (var sr = new StringReader (response)) {
				string s;
				while ((s = sr.ReadLine ()) != null) {
					s = s.Trim ();
					if (s.Length == 0)
						continue;
					string[] data = s.Split ('\t');
					devices.Add (new AndroidDevice (data[0].Trim (), data[1].Trim ()));
				}
			}
			return devices;
		}
		
		public List<AndroidDevice> Devices { get; private set; }
		public event Action<List<AndroidDevice>> DevicesChanged;
	}

	public sealed class AdbTrackLogOperation : AdbTransportOperation
	{	
		Action<string> output;
		string [] parameters;

		public AdbTrackLogOperation (AndroidDevice device, Action<string> output, params string [] parameters) : base (device)
		{
			if (output == null)
				throw new ArgumentNullException ("output");
			
			this.output = output;
			this.parameters = parameters;
			
			BeginConnect ();
		}

		protected override void OnGotTransport ()
		{
			string command = "shell:logcat " + string.Join (" ", parameters);

			WriteCommand (command, () => GetStatus (() => ReadResponseContinuous (GotResponseContinuous)));
		}

		string tmpLine;

		// Each log line marks its ending by a \r\n, so we use it
		// to detect incomplete lines and save, to merge them later.
		void GotResponseContinuous (string response)
		{
			int pos = 0;
			while (pos < response.Length) {

				int idx = response.IndexOf ('\n', pos);
				if (idx < 0) {
					// End of response, incomplete logline
					if (tmpLine != null)
						tmpLine += response.Substring (pos, response.Length - pos);
					else
						tmpLine = response.Substring (pos, response.Length - pos);

					break;
				}

				// Extract the line, including the newline chars.
				var logLine = response.Substring (pos, idx - pos + 1);
				pos = idx + 1;

				if (tmpLine != null) {
					logLine = tmpLine + logLine;
					tmpLine = null;
				}

				output (logLine);
			}
		}
	}
	
	public sealed class AdbGetPackagesOperation : AdbTransportOperation
	{
		const string DefaultPackagesFile = "/data/system/packages.xml";

		string packageFileName;

		public AdbGetPackagesOperation (AndroidDevice device) : this (device, DefaultPackagesFile)
		{
		}

		public AdbGetPackagesOperation (AndroidDevice device, string packageFileName) : base (device)
		{
			this.packageFileName = packageFileName;
			BeginConnect ();
		}
		
		protected override void OnGotTransport ()
		{
			var sr = new StringWriter ();
			WriteCommand ("shell:cat " + packageFileName, () => GetStatus (() => ReadResponse (sr, OnGotResponse)));
		}
		
		void OnGotResponse (TextWriter tw)
		{
			XDocument doc = XDocument.Parse (tw.ToString ());
			var list = new PackageList ();

			foreach (var elem in doc.Element ("packages").Elements ("package")) {
				var name = elem.Attribute ("name").Value;
				var apk = elem.Attribute ("codePath").Value;
				var version = int.Parse (elem.Attribute ("version").Value);

				list.Packages.Add (new InstalledPackage (name, apk, version));
			}

			PackageList = list;
			
			SetCompleted (true);
		}
		
		public PackageList PackageList { get; private set; }
	}
	
	class GetPackageListException : Exception
	{
		public GetPackageListException (string message) : base (message)
		{
		}
	}
	
	public sealed class AdbGetPropertiesOperation : AdbTransportOperation
	{
		public AdbGetPropertiesOperation (AndroidDevice device) : base (device)
		{
			BeginConnect ();
		}
		
		protected override void OnGotTransport ()
		{
			var sr = new StringWriter ();
			WriteCommand ("shell:getprop", () => GetStatus (() => ReadResponse (sr, OnGotResponse)));
		}
		
		void OnGotResponse (TextWriter tw)
		{
			var sr = new StringReader (tw.ToString ());
			var props = new Dictionary<string,string> ();
			
			string s;
			var split = new char[] { '[', ']' };
			while ((s = sr.ReadLine ()) != null) {
				var arr = s.Split (split);
				if (arr.Length != 5 || string.IsNullOrEmpty (arr[1]) || arr[2] != ": ")
					throw new Exception ("Unknown property format: '" + s + "'");
				props [arr[1]] = arr[3];
			}
			Properties = props;
			
			SetCompleted (true);
		}
		
		public Dictionary<string,string> Properties { get; private set; }
	}

	public sealed class AdbGetProcessIdOperation : AdbTransportOperation
	{
		int pid = -1;
		string packageName;

		public AdbGetProcessIdOperation (AndroidDevice device, string packageName) :
			base (device)
		{
			if (packageName == null)
				throw new ArgumentNullException ("packageName");

			this.packageName = packageName;
			BeginConnect ();
		}

		protected override void OnGotTransport ()
		{
			// Can't pass any info to 'ps', as it seems to have an irregular behaviour among versions.
			var sr = new StringWriter ();
			WriteCommand ("shell:ps", () => GetStatus (() => ReadResponse (sr, OnGotResponse)));
		}

		void OnGotResponse (TextWriter tw)
		{
			pid = ParseResponse (packageName, tw.ToString ());
			SetCompleted (true);
		}

		// Values:
		// -1 - value hasn't been computed, or an error happened
		// 0 - no associated process was found
		// any other - actual pid
		public int ProcessId {
			get {
				return pid;
			}
		}

		// The first line is the header, so we ignore it.
		// The information we are looking are is contained in the members:
		// 1 - Process ID
		// 8 - Package name (can be an empty string in some devices)
		static int ParseResponse (string packageName, string response)
		{
			using (var sr = new StringReader (response)) {
				string line = sr.ReadLine (); // ignore header
				if (line == null)
					throw new Exception ("'ps' output not recognized: '" + response + "'");

				char [] space = new char [] { ' ' };
				while ((line = sr.ReadLine ()) != null) {
					line = line.Trim ();
					if (line.Length == 0)
						continue;

					string [] stats = line.Split (space, StringSplitOptions.RemoveEmptyEntries);
					if (stats.Length < 8)
						throw new Exception ("'ps' output not recognized: '" + response + "'");

					// Some devices have *system* processes either with names containing spaces within,
					// or nameless. Ignore them.
					if (stats.Length != 9)
						continue;

					if (stats [8].Trim () == packageName)
						return Int32.Parse (stats [1]);
				}
			}

			return 0;
		}

	}

	public class AdbKillProcessOperation : AdbBaseShellOperation
	{
		public AdbKillProcessOperation (AndroidDevice device, string packageName) : 
			base (device, "am broadcast -a mono.android.intent.action.SEPPUKU " +
				"-c mono.android.intent.category.SEPPUKU." + packageName)
		{
			BeginConnect ();
		}
	}
	
	public sealed class AdbGetDateOperation : AdbBaseShellOperation
	{
		public AdbGetDateOperation (AndroidDevice device) : base (device, "date +%s")
		{
			BeginConnect ();
		}
		
		long? date;
		
		public override bool Success {
			get {
				if (!base.Success)
					return false;
				if (!date.HasValue) {
					long value;
					date = long.TryParse (Output, out value)? value : -1;
				}
				return date >= 0;
			}
		}
			
		public long Date {
			get {
				if (!Success)
					throw new InvalidOperationException ("Error getting date from device:\n" + Output);
				//Success will have parsed the value 
				return date.Value;
			}
		}
	}

	public sealed class AdbGetAvailableSpaceOperation : AdbBaseShellOperation
	{
		long externalSpace = -1;
		long internalSpace = -1;

		const long Kilobyte = 1024;
		const long Megabyte = 1024 * 1024;
		const long Gigabyte = 1024 * 1024 * 1024;

		const string InternalPartition = "/data";
		const string ExternalPartition = "/mnt/sdcard";

		public AdbGetAvailableSpaceOperation (AndroidDevice device) : base (device, "df")
		{
			BeginConnect ();
		}

		bool ParseDfOutput (string output)
		{
			return output.StartsWith ("Filesystem") ? ParseNewFormat (output) : ParseOldFormat (output);
		}

		bool ParseNewFormat (string output)
		{
			string s;
			var reader = new StringReader (output);

			reader.ReadLine (); //  header line

			// /data      496M    54M   442M   4096
			while ((s = reader.ReadLine ()) != null) {
				var parts = s.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 5)
					return false;

				string partition = parts [0];
				if (partition != InternalPartition && partition != ExternalPartition)
					continue;

				long size;
				if (!ParseToBytes (parts [3], out size))
					return false;

				if (partition == InternalPartition)
					internalSpace = size;
				else
					externalSpace = size;
			}

			// if /data was not found, then something went wrong.
			return internalSpace > -1;
		}

		bool ParseOldFormat (string output)
		{
			string s;
			var reader = new StringReader (output);
			while ((s = reader.ReadLine ()) != null) {
				int idx = s.IndexOf (':');
				if (idx < 0)
					return false;

				long size = 0;
				string partition = s.Substring (0, idx);
				if (partition != InternalPartition && partition != ExternalPartition)
					continue;

				// /data/: 508416K total, 98548K used, 409868K available (block size 4096)
				var parts = s.Split (new char [] { ',' });
				if (parts.Length != 3 || parts [2].IndexOf ("available") < 0)
					return false;

				// the actual available component
				parts = parts [2].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (!ParseToBytes (parts [0].Trim (), out size))
					return false;

				if (partition == InternalPartition)
					internalSpace = size;
				else
					externalSpace = size;
			}

			// if /data was not found, then something went wrong.
			return internalSpace > -1;
		}

		bool ParseToBytes (string s, out long value)
		{
			var unit = s [s.Length - 1];
			var available = s.Substring (0, s.Length - 1);
			if (!long.TryParse (available, out value))
				return false;

			switch (unit) {
				case 'K': value *= Kilobyte;
					  break;
				case 'M': value *= Megabyte;
					  break;
				case 'G': value *= Gigabyte;
					  break;
				default:
					  return false;
			}

			return true;
		}

		bool? success;

		public override bool Success {
			get {
				if (!base.Success)
					return false;
				if (!success.HasValue)
					success = ParseDfOutput (Output);

				return success.Value;
			}
		}

		public long InternalSpace {
			get {
				if (!Success)
					throw new InvalidOperationException ("Error getting partition size from device:\n" + Output);
				//Success will have parsed the value 
				return internalSpace;
			}
		}

		public long ExternalSpace {
			get {
				if (!Success)
					throw new InvalidOperationException ("Error getting partition size from device:\n" + Output);
				return externalSpace;
			}
		}
	}

	public abstract class AdbBaseShellOperation : AdbTransportOperation
	{
		string command;
		
		public AdbBaseShellOperation (AndroidDevice device, string command) : base (device)
		{
			this.command = command;
		}
		
		protected override void OnGotTransport ()
		{
			var sr = new StringWriter ();
			WriteCommand ("shell:" + command, () => GetStatus (() => ReadResponse (sr, OnGotResponse)));
		}

		void OnGotResponse (TextWriter tw)
		{
			Output = tw.ToString ();
			SetCompleted (true);
		}
		
		public string Output { get; private set; }
	}
	
	public sealed class AdbShellOperation : AdbBaseShellOperation
	{
		public AdbShellOperation (AndroidDevice device, string command) : base (device, command)
		{
			BeginConnect ();
		}
	}
}
