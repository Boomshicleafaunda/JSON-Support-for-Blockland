//* Description *//
// Title: JSON
// Author: Boom (9740)
// Adds JSON Functionality

//* Support Functions *//
// Decodes a JSON String into a Hash Map
function JSON_decode(%json)
{
	// Find the First JSON Element
	%map = strNonEscapedPos(%json, "{");
	%list = strNonEscapedPos(%json, "[");

	if(%map >= 0 && (%map < %list || %list < 0)) // Parse as Map
		return JSON_decodeMap(getMarkedSubStr(%json, "{", "}", 0, true));
	else if(%list >= 0 && (%list < %map || %map < 0)) // Parse as List
		return JSON_decodeList(getMarkedSubStr(%json, "[", "]", 0, true));
	else // Do not Parse
		return 0;
}

function JSON_decodeMap(%json)
{
	// Create the Map
	%map = new ScriptObject()
	{
		class = JMap;
		debugLevel = 3;
	};

	// Verify Contents
	if(strLen(%json) < 2)
	{
		echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 Map (\c1" @ %map @ "\c4) \c3|\c4 \c2Warning\c4]\c0: Could not Decode Map \"\c1" @ %json @ "\c0\"");
		return %map;
	}

	// Strip Map Brackets
	%json = getSubStr(%json, 1, strLen(%json) - 2);

	// Iterate Elements
	%hasNext = true;

	while(%hasNext)
	{
		// Find the Next Pair
		%pair = JSON_getNextPair(%json);

		// Ensure that a Pair was Found
		if(!isObject(%pair))
			break;

		// Add the Value to the Map
		%map.addPair(%pair);

		// Skip Past the Element
		%json = JSON_skipNextElement(%json);

		if(%pair.isMarked())
		{
			if(%pair.getValue().class $= "JList")
			{
				%nestList = getMarkedSubStr(%json, "[", "]", 0, true);
				%json = getSubStr(%json, strPos(%json, %nestList) + strLen(%nestList), strLen(%json));
			}
			else if(%pair.getValue().class $= "JMap")
			{
				%nestMap = getMarkedSubStr(%json, "{", "}", 0, true);
				%json = getSubStr(%json, strPos(%json, %nestMap) + strLen(%nestMap), strLen(%json));
			}
			else
				break;
		}

		// Determine whether or not there is a Next Element
		%hasNext = getSubStr(%json, 0, 1) $= ",";

		// Skip Past the Comma
		if(%hasNext)
			%json = getSubStr(%json, 1, strLen(%json));
	}
	
	return %map;
}

function JSON_decodeList(%json)
{
	// Create the List
	%list = new ScriptObject()
	{
		class = JList;
	};

	// Verify Contents
	if(strLen(%json) < 2)
	{
		echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 List (\c1" @ %list @ "\c4) \c3|\c4 \c2Warning\c4]\c0: Could not Decode List \"\c1" @ %json @ "\c0\"");
		return %list;
	}

	// Strip List Brackets
	%json = getSubStr(%json, 1, strLen(%json) - 2);

	// Iterate Elements
	%hasNext = true;

	while(%hasNext)
	{
		// Find the Next Element
		%element = JSON_getNextElement(%json);

		// Ensure that an Element was Found
		if(!isObject(%element))
			break;

		// Add the Element to the List
		%list.addElement(%element);

		// Skip Past the Element
		%json = JSON_skipNextElement(%json);

		if(%element.isMarked())
		{
			if(%element.getValue().class $= "JList")
			{
				%nestList = getMarkedSubStr(%json, "[", "]", 0, true);
				%json = getSubStr(%json, strPos(%json, %nestList) + strLen(%nestList), strLen(%json));
			}
			else if(%element.getValue().class $= "JMap")
			{
				%nestMap = getMarkedSubStr(%json, "{", "}", 0, true);
				%json = getSubStr(%json, strPos(%json, %nestMap) + strLen(%nestMap), strLen(%json));
			}
			else
				break;
		}

		// Determine whether or not there is a Next Element
		%hasNext = getSubStr(%json, 0, 1) $= ",";

		// Skip Past the Comma
		if(%hasNext)
			%json = trim(getSubStr(%json, 1, strLen(%json)));
	}

	return %list;
}

// Skips the Next Element and Returns the Remaining JSON
function JSON_skipNextElement(%json)
{
	// Determine Potential Markers
	%s[0] = strNonEscapedPos(%json, ","); // Regular Element
	%s[1] = strNonEscapedPos(%json, "}"); // Last Element of Map
	%s[2] = strNonEscapedPos(%json, "]"); // Last Element of List
	%s[3] = strNonEscapedPos(%json, "["); // Start of List
	%s[4] = strNonEscapedPos(%json, "{"); // Start of Map

	// Take Closest Value
	%value = strLen(%json);

	for(%i = 0; %i < 5; %i++)
		if(%value > %s[%i] && %s[%i] >= 0)
			%value = %s[%i];

	%json = trim(getSubStr(%json, %value, strLen(%json)));
}

// Returns the Next JSON Element within the specified JSON
function JSON_getNextElement(%json)
{
	// Parse the Value
	%value = JSON_getNextValue(":" @ %json); // Use Map Decoder

	// Validate Value
	if(getFieldCount(%value) < 2)
	{
		echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 Element \c3|\c4 \c2Warning\c4]\c0: Could not find Value for Element near \"\c1" @ %json @ "\c0\"!");
		return 0;
	}

	// Create the Element
	%element = new ScriptObject()
	{
		class = JElement;
		elementValue = getFields(%value, 1, getFieldCount(%value));
		elementMarked = getField(%value, 0);
	};

	return %element;
}

// Returns the Next JSON Pair within the specified JSON
function JSON_getNextPair(%json)
{
	// Determine the Start of the Name
	%pos = strNonEscapedPos(%json, "\"");

	// Verify Pos
	if(%pos < 0)
	{
		echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 Pair \c3|\c4 \c2Warning\c4]\c0: Could not find Name for Pair near \"\c1" @ %json @ "\c0\"!");
		return 0;
	}

	// Jump to the Start of the Name
	%json = getSubStr(%json, %pos, strLen(%json));
	%name = JSON_getNextString(%json);

	// Skip Past Name
	%json = getSubStr(%json, strLen(%name) + 2, strLen(%json));

	// Parse the Value
	%value = JSON_getNextValue(%json);

	// Validate Value
	if(getFieldCount(%value) < 2)
	{
		echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 Pair \c3|\c4 \c2Warning\c4]\c0: Could not find Value for Pair near \"\c1" @ %json @ "\c0\"!");
		return 0;
	}

	// Create the Pair
	%pair = new ScriptObject()
	{
		class = JPair;
		pairName = %name;
		pairValue = getFields(%value, 1, getFieldCount(%value));
		pairMarked = getField(%value, 0);
	};

	return %pair;
}

// Returns the Next String within the specifeid JSON
function JSON_getNextString(%json)
{
	return getBoundedSubStr(%json, "\"", "\"");
}

// Returns the Next Value within the specified JSON
function JSON_getNextValue(%json)
{
	// Determine Possible Values
	%s[0] = getBoundedSubStr(%json, ":", ",", 0, true); // Regular Element
	%s[1] = getBoundedSubStr(%json, ":", "}", 0, true); // Last Element of Map
	%s[2] = getBoundedSubStr(%json, ":", "]", 0, true); // Last Element of List
	%s[3] = getBoundedSubStr(%json, ":", "[", 0, true); // Start of List
	%s[4] = getBoundedSubStr(%json, ":", "{", 0, true); // Start of Map

	%value = strMin(%s[0], %s[1], %s[2], %s[3], %s[4]);

	// In case nothing was Found
	if(%value $= "")
		%value = %json;

	// Strip Colon Delimiter and Spacing
	%value = trim(getSubStr(%value, 1, strLen(%value)));

	// Strip any Whitespaces
	%value = trim(%value);

	// Parse Value
	if(strBeginsWith(%value, "{")) // Map
		return true TAB JSON_decodeMap(getMarkedSubStr(%json, "{", "}", 0, true));
	else if(strBeginsWith(%value, "[")) // List
		return true TAB JSON_decodeList(getMarkedSubStr(%json, "[", "]", 0, true));
	else if(strEndsWith(%value, ",") || strEndsWith(%value, "}") || strEndsWith(%value, "]")) // String or Real
	{
		%value = trim(getSubStr(%value, 0, strLen(%value) - 1));

		// Remove Quotes (if Any)
		if(getBoundedSubStr(%value, "\"", "\"") !$= "")
			%value = getBoundedSubStr(%value, "\"", "\"");
		else if(%value $= "true")
			return false TAB true;
		else if(%value $= "false")
			return false TAB false;
		else if(stripChars(%value, "+-0123456789.eE#INF") !$= "") // Check String without Quotes
		{
			echo("\c3 + \c4[JSON \c3|\c4 Decode \c3|\c4 Value \c3|\c4 \c2Warning\c4]\c0: Found Unbounded String near \"\c1" @ %json @ "\c0\"!");
			return "";
		}

		return false TAB %value;
	}
	else // End of Object
		return false TAB %value;
}