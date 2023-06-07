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

06/06/2023	1.0.0.1		EVA, Skyline	Initial version
****************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Library.Automation;
using Skyline.DataMiner.Library.Common;
using Skyline.DataMiner.Library.Common.Selectors;

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
		DateTime now = DateTime.UtcNow;
		string desPath = @"C:\Skyline DataMiner\Documents";
		string fileName = $"Active_Cisco_Elements_{now.ToString("ddMMyyyy_HHmmssfff")}.csv";
		string desFilePath = string.Format(@"{0}\{1}", desPath, fileName);
		if (File.Exists(desFilePath))
		{
			File.Delete(desFilePath);
		}


		//var elementsList = engine.FindElements(new ElementFilter { ProtocolName = "CISCO Manager", IncludePaused = false, IncludeHidden = false, IncludeStopped = false });

		//elementsList.

		var dms = engine.GetDms();

		var elements = dms.GetElements().Where(e => e.Protocol.Name == "CISCO Manager" && e.State == ElementState.Active);

		StringBuilder sb = new StringBuilder();
		//sb.AppendLine(CreateCsvRowFromData(CiscoManager.Header));
		foreach (var element in elements)
		{
			var systemDescription = element.GetStandaloneParameter<string>(5);
			//engine.GenerateInformation("dmaId:" + element.AgentId + " systemDescription:" + systemDescription.GetValue());
			//engine.GenerateInformation(element.AgentId + ":dma, " + element.Protocol.Name + element.State + " el. name: " + element.Name); // active state is 1

			var newRow = new CiscoManager(element.AgentId, systemDescription.GetValue());

			sb.AppendLine(CreateCsvRowFromData(newRow.GetDisplayData()));
		}
		File.WriteAllText(desFilePath, sb.ToString());
	}

	private static string CreateCsvRowFromData(string[] row)
	{
		return "\"" + string.Join("\",\"", row) + "\"";
	}

	public class CiscoManager
	{
		public CiscoManager(int dmaId, string systemDescription)
		{
			DmaID = dmaId.ToString();
			SystemDescription = systemDescription;
		}

		public static string[] Header
		{
			get
			{
				return new[] { "DMA ID", "System Description"};
			}
		}

		public string DmaID { get; private set; }

		public string SystemDescription { get; set; }

		public string[] GetDisplayData()
		{
			return new[]
			{
				DmaID,
				SystemDescription,
			};
		}

	}
}