//* Description *//
// Title: String
// Author: Boom (9740)
// Adds Additional String Support

//* String Functions *//
// Returns the Marked Sub-String within the specified String
function getMarkedSubStr(%str, %open, %close, %pos, %keepMarks)
{
	// Find the First Bounded Sub-String
	%sub = getBoundedSubStr(%str, %open, %close, %pos, true);

	// Verify that a Bounded Sub-String was Found
	if(%sub $= "")
		return "";

	// Strip Marks
	%sub = getSubStr(%sub, strLen(%open), strLen(%sub) - (strLen(%open) + strLen(%close)));

	// Determine the Bounds
	%start = strNonEscapedPos(%str, %open, %pos);
	%end = strNonEscapedPos(%str, %close, %start);

	// Search for Non-Escaped Open Markers within the Marked Sub-String
	%index = %start + 1;

	while(strNonEscapedPos(%sub, %open, %index) >= 0)
	{
		// Find the First Non-Escaped Close Maker after the Known one
		%end = strNonEscapedPos(%str, %close, %end + 1);

		// Update the Index
		%index = strNonEscapedPos(%sub, %open, %index) + 1;

		// Update the Sub-String
		%subPos = %start + strLen(%open);
		%len = %end - %subPos;

		// Make sure the Starting Position and Length are Valid
		if(%subPos < 0 || %len < 0)
			return "";

		%sub = getSubStr(%str, %subPos, %len);
	}

	if(%keepMarks)
		return %open @ %sub @ %close;
	else
		return %sub;
}

// Returns the Bounded Sub-String within the specified String
function getBoundedSubStr(%str, %open, %close, %pos, %keepMarks)
{
	// Validate Pos
	if(%pos < 0) return "";

	// Find the First Non-Escaped Open Marker
	%start = strNonEscapedPos(%str, %open, %pos);

	// Find the First Non-Escaped Close Marker
	%end = strNonEscapedPos(%str, %close, %start + 1);

	// Make sure the String Contains the Markers
	if(%start < 0 || %end < 0)
		return "";

	// Determine the Marked Sub-String
	%subPos = %start + strLen(%open);
	%len = %end - %subPos;

	// Make sure the Starting Position and Length are Valid
	if(%subPos < 0 || %len < 0)
		return "";

	if(%keepMarks)
		return %open @ getSubStr(%str, %subPos, %len) @ %close;
	else
		return getSubStr(%str, %subPos, %len);
}

// Finds the specified Non-Escaped String and Returns its Position
function strNonEscapedPos(%str, %sub, %pos)
{
	// Validate Pos
	if(%pos $= "") %pos = 0;
	else if(%pos < 0) return "";

	// Find the Sub-String
	%index = strPos(%str, %sub, %pos);

	// Make sure the Sub-String is not Escaped
	if(%index > 0)
		while(getSubStr(%str, %index - 1, 1) $= "\\")
		{
			%index = strPos(%str, %sub, %index + strLen(%sub));

			if(%index < 0)
				break;
		}

	return %index;
}

// Returns the Shortest of the specified Strings
function strMin(%s0, %s1, %s2, %s3, %s4, %s5, %s6, %s7, %s8, %s9, %s10, %s11, %s12, %s13, %s14, %s15)
{
	%short = %s[0];

	for(%i = 1; %i < 16; %i++)
		if(strLen(%s[%i]) < strLen(%short) && %s[%i] !$= "")
			%short = %s[%i];

	return %short;
}

// Returns whether or not the specified String Begins with the specified Sub-String
function strBeginsWith(%str, %sub)
{
	if(%str $= "" || %sub $= "")
		return false;

	return getSubStr(%str, 0, strLen(%sub)) $= %sub;
}

// Returns whether or not the specified String Ends with the specified Sub-String
function strEndsWith(%str, %sub)
{
	if(%str $= "" || %sub $= "")
		return false;

	%pos = strLen(%str) - strLen(%sub);

	if(%pos < 0)
		return false;

	return getSubStr(%str, %pos, strLen(%sub)) $= %sub;
}

// Splits the specified String using the specified Delimiter
function strSplit(%str, %delimiter)
{
	// Create the Split List
	%list = new ScriptObject()
	{
		class = ArrayList;
	};

	// Loop Indefinitely
	while(true)
	{
		// Determine the Position of the Next Delimiter
		%pos = strNonEscapedPos(%str, ",");

		// Make sure the Delimiter Exists
		if(%pos < 0 || %delimiter $= "")
		{
			// Add the Rest of the String to the Split List
			%list.addEntry(%str);
			return %list;
		}
		else
		{
			// Add the Next Sub-String and Update the String
			%list.addEntry(getSubStr(%str, 0, %pos));

			%start = %pos + strLen(%delimiter);
			%end = strLen(%str) - %start;

			%str = getSubStr(%str, %start, %end);
		}
	}
}