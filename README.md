# SLC-AS-CheckCiscoManagerFwVersions
This solution can be used to gather data from active elements and store them in a JSON file. It was designed specifically for extracting system information from the Cisco Manager protocol, but it can also be used for other protocols, so we added two script parameters.

## How to use
The script expects two parameters: "Protocol Name" and "Parameter ID."
Protocol Name expects the protocol for which information will be extracted from.
Parameter ID expects the System Description Parameter, this is the parameter which we're extracting information from.

The SLC-AS-CheckCiscoManagerFwVersions Automation script will check all active elements that use the specified protocol and then extract the DMA version and all information from the specified parameter from all active elements inta JSON File, you can then use SLC-AS-MergeGeneratedFiles to merge the files. Because the automation script was designed for Cisco Manager, the files will begin with: "Active_Cisco_Elements_", but this can be changed later.
