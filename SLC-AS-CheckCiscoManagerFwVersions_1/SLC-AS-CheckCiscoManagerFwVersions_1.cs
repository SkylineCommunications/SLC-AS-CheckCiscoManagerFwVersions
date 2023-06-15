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
using System.Text.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Library.Automation;
using Skyline.DataMiner.Library.Common;
using Skyline.DataMiner.Library.Common.Selectors;
using Skyline.DataMiner.Net.DMSState.Agents;

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
		var scriptParams = new ScriptParams
		{
			ProtocolName = engine.GetScriptParam("Protocol Name").Value,
			ParameterId = engine.GetScriptParam("Parameter ID").Value,
		};

		var fields = scriptParams.ParseInputFields(scriptParams, engine);

		DateTime now = DateTime.UtcNow;
		string desPath = @"C:\Skyline DataMiner\Documents\Exports";
		Directory.CreateDirectory(desPath);
		string fileName = $"Active_Cisco_Elements_{now.ToString("ddMMyyyy_HHmmssfff")}.json";
		string desFilePath = Path.Combine(desPath, fileName);

		var dms = engine.GetDms();

		var elements = dms.GetElements().Where(e => e.Protocol.Name == fields.ProtocolName && e.State == ElementState.Active);

		if (!elements.Any())
		{
			engine.ExitFail("No active elements found with protocol name: " + fields.ProtocolName);
		}

		var systemDescriptions = new List<string>();

		var firstElement = elements.FirstOrDefault();
		IDma agent = null;

		if (firstElement != null)
		{
			agent = dms.GetAgent(firstElement.AgentId);
		}

		HashSet<string> uniqueDescriptions = new HashSet<string>(); // Store unique system descriptions

		foreach (var element in elements)
		{
			var systemDescription = element.GetStandaloneParameter<string>(fields.ParameterId);

			string description = string.Empty;

			try
			{
				description = string.IsNullOrEmpty(systemDescription.GetValue()) ? string.Empty : systemDescription.GetValue();
			}
			catch (ParameterNotFoundException)
			{
				engine.ExitFail($"No parameter found with ID: {fields.ParameterId}");
			}
			catch (Exception)
			{
				engine.ExitFail($"Failed getting value from Parameter: {fields.ParameterId}");
			}

			if (!uniqueDescriptions.Contains(description)) // Check if the description is unique
			{
				uniqueDescriptions.Add(description);
				systemDescriptions.Add(description);
			}
		}

		var jsonData = new Dictionary<string, object>
		{
			{ "versionInfo", agent.VersionInfo ?? string.Empty },
			{ "elements", systemDescriptions },
		};

		var jsonOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
		};
		string jsonString = JsonSerializer.Serialize(jsonData, jsonOptions);

		File.WriteAllText(desFilePath, jsonString);
	}

	public class ScriptParams
	{
		public string ProtocolName { get; set; }

		public string ParameterId { get; set; }

		/// <summary>
		/// Parse Script Parameters based on the provided input data.
		/// </summary>
		/// <param name="scriptParams">The input data.</param>
		/// <param name="engine">Link with SLAutomation process.</param>
		/// <returns>A CreateFields object.</returns>
		public ScriptParamsParsed ParseInputFields(ScriptParams scriptParams, IEngine engine)
		{
			if (IsNullOrEmptyOrWhiteSpace(scriptParams.ProtocolName))
			{
				engine.ExitFail("Protocol Name is null or empty.");
				return null;
			}

			if (!int.TryParse(scriptParams.ParameterId, out var intParameterId))
			{
				engine.ExitFail("Parameter ID isn't an integer.");
				return null;
			}

			try
			{
				var parsedFields = new ScriptParamsParsed
				{
					ProtocolName = scriptParams.ProtocolName,
					ParameterId = intParameterId,
				};

				return parsedFields;
			}
			catch
			{
				engine.ExitFail("Failed to parse Script Parameters.");
				return null;
			}
		}

		private static bool IsNullOrEmptyOrWhiteSpace(string value)
		{
			return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
		}
	}

	public class ScriptParamsParsed
	{
		public string ProtocolName { get; set; }

		public int ParameterId { get; set; }
	}
}