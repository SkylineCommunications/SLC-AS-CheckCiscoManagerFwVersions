/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2023	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;

/// <summary>
/// Represents a DataMiner Automation script.
/// </summary>
public class Script
{
	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process.</param>
	public void Run(IEngine engine)
	{
		string directoryPath = @"C:\Skyline DataMiner\Documents\Exports";
		Directory.CreateDirectory(directoryPath);
		string[] filePaths = Directory.GetFiles(directoryPath, "Active_Cisco_Elements_*.json");

		DateTime now = DateTime.UtcNow;
		string mergedFileName = $"Merged_Active_Cisco_Elements_{now:ddMMyyyy_HHmmssfff}.json";
		string desFilePath = Path.Combine(directoryPath, mergedFileName);

		List<Root> mergedObjects = new List<Root>();

		foreach (string filePath in filePaths)
		{
			string fileContent = File.ReadAllText(filePath);
			Root jsonObject = JsonConvert.DeserializeObject<Root>(fileContent);
			mergedObjects.Add(jsonObject);
		}

		RootObject rootObject = new RootObject { MergedObjects = mergedObjects };
		string mergedJson = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

		File.WriteAllText(desFilePath, mergedJson);
	}

	public class RootObject
	{
		[JsonProperty("mergedObjects")]
		public List<Root> MergedObjects { get; set; }
	}

	public class Root
	{
		[JsonProperty("versionInfo")]
		public string VersionInfo { get; set; }

		[JsonProperty("elements")]
		public List<string> Elements { get; set; }
	}
}