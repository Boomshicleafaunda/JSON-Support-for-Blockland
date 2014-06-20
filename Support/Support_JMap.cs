//* Description *//
// Title: JMap
// Author: Boom (9740)
// Defines a JSON Array Map

//* Version *//
$Temp::Version = 1.0; // Do not change this unless you know what you are doing

if($Support::JSON::Map::Version !$= "") // Check for other versions
	if($Support::JSON::Map::Version >= $Temp::Version) // A newer version has been loaded already
		return;

// At this point, this is the newest Version
$Support::JSON::Map::Version = $Temp::Version; // Update the version

//* Functions *//
// Trigger Methods:
// - onAdd(%this)
// - onRemove(%this)
// Exception Methods:
// - onException(%this, %type, %msg)
// - throwException(%this, %type, %msg)
// - debug(%this, %access, %header, %msg)
// Utility Methods:
// - clearMap(%this)
// - getPairCount(%this)
// - deleteAll(%this)
// Search Methods:
// - indexOfPair(%this, %pair, %pos)
// - indexOfKey(%this, %key)
// - indexOfValue(%this, %value, %marked);
// - pairExists(%this, %pair)
// - keyExists(%this, %key)
// - valueExists(%this, %value)
// Pair Methods:
// - addPair(%this, %pair)
// - insertPair(%this, %index, %pair)
// - removePair(%this, %pair)
// - removeIndex(%this, %index)
// - getPair(%this, %index)
// - setPair(%this, %index, %pair)
// Value Methods:
// - addValue(%this, %key, %value, %marked)
// - removeValue(%this, %value, %marked)
// - getValue(%this, %key)
// - setValue(%this, %key, %value, %marked)
// Conversion Methods:
// - exportMap(%this, %path)
// - toJSON(%this)

//* Trigger Methods *//
// Triggered when the Map is Created
function JMap::onAdd(%this)
{
	%this.size = 0;
	%this.isJSON = true;
}

// Triggered when the Map is Destroyed
function JMap::onRemove(%this)
{
}

//* Exception Methods *//
// Triggered when an Exception is Thrown
function JMap::onException(%this, %type, %msg)
{
	// Use Debugger to Display Exception
	%this.debug(1, "\c2Exception\c4 (\c1" @ %type @ "\c4)", %msg);
	return true;
}

// Throws an Exception
function JMap::throwException(%this, %type, %msg)
{
	// Update Last Exception Values
	%this.lastExceptionType = %type;
	%this.lastExceptionMessage = %msg;

	// Call the Exception Trigger
	%this.onException(%type, %msg);
}

// Prints the specified Message to the Console
function JMap::debug(%this, %access, %header, %msg)
{
	if(%this.debugLevel >= %access)
		echo("\c3 + \c4[JMap (\c1" @ %this @ "\c4) \c3|\c4 " @ %header @ "]\c0: " @ %msg);

	return %this.debugLevel >= %access;
}

//* Utility Methods *//
// Clears all Entries in the Map
function JMap::clearMap(%this)
{
	// Iterate through the Map
	for(%i = 0; %i < %this.size; %i++)
	{
		%this.pair[%i].delete();
		%this.pair[%i] = "";
	}

	%this.size = 0;

	return true;
}

// Returns the Number of Entries in the Map
function JMap::getPairCount(%this)
{
	return %this.size;
}

// Deletes the Object and its Descendants
function JMap::deleteAll(%this)
{
	for(%i = 0; %i < %this.size; %i++)
		%this.pair[%i].deleteAll();

	%this.delete();
}

//* Search Methods *//
// Returns the Index of the First Instance of the specified Pair
function JMap::indexOfPair(%this, %pair, %pos)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Iterate through the Map
	for(%i = %pos; %i < %this.size; %i++)
		if(%this.pair[%i] $= %pair)
			return %i;

	return -1;
}

// Returns the Index of the First Instance of the specified Key
function JMap::indexOfKey(%this, %key)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Iterate through the Map
	for(%i = %pos; %i < %this.size; %i++)
		if(%this.pair[%i].getName() $= %key)
			return %i;

	return -1;
}

// Returns the Index of the First Instance of the specified Key
function JMap::indexOfValue(%this, %value, %marked)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Iterate through the Map
	for(%i = %pos; %i < %this.size; %i++)
		if(%this.pair[%i].getValue() $= %value && %this.pair[%i].isMarked() == %marked)
			return %i;

	return -1;
}

// Returns whether or not the specified Pair exists within the Map
function JMap::pairExists(%this, %pair)
{
	return %this.indexOfPair(%pair) >= 0;
}

// Returns whether or not the specified Key exists within the Map
function JMap::keyExists(%this, %key)
{
	return %this.indexOfKey(%key) >= 0;
}

// Returns whether or not the specified Pair exists within the Map
function JMap::valueExists(%this, %value, %marked)
{
	return %this.indexOfValue(%value, %marked) >= 0;
}

//* Pair Methods *//
// Adds the specified Pair to the Map
function JMap::addPair(%this, %pair)
{
	// Verify Pair Type
	if(!isObject(%pair) || %pair.class !$= "JPair")
	{
		%this.throwException("ClassType", "An attempt to Add Pair \"\c1" @ %pair @ "\c0\" failed. The specified Pair is not of JPair Type!");
		return false;
	}

	// Add the Pair
	%this.pair[%this.size] = %pair;

	// Increase the Size
	%this.size++;

	return true;
}

// Inserts the specified Pair at the specified Position into the Map
function JMap::insertPair(%this, %index, %pair)
{
	// Verify Pair Type
	if(!isObject(%pair) || %pair.class !$= "JPair")
	{
		%this.throwException("ClassType", "An attempt to Insert Pair \"\c1" @ %pair @ "\c0\" at Index '" @ %index @ "\c0\" failed. The specified Pair is not of JPair Type!");
		return false;
	}

	// Validate Index
	if(%index < 0 || %index > %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Insert at Index \"\c1" @ %index @ "' failed. The specified Index is not within the Range of the Map!");
		return false;
	}

	// Insert Pair at the specified Position
	for(%i = %this.size; %i > %index; %i--)
		%this.pair[%i] = %this.pair[%i - 1];

	%this.pair[%index] = %pair;

	// Increase the Size
	%this.size++;

	return true;
}

// Removes the First Instance of the specified Pair from the Map
function JMap::removePair(%this, %pair)
{
	// Determine the Index of the specified Pair
	%index = %this.indexOfPair(%pair);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("PairNonExistant", "An attempt to Remove Pair \"\c1" @ %pair @ "\c0\" failed. The specified Pair is not within the Map!");
		return "";
	}

	// Remove the Pair by Index
	%this.removeIndex(%index);

	// Return the Index
	return %index;
}

// Removes the specified Index from the Map
function JMap::removeIndex(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Remove Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the Map!");
		return "";
	}

	// Check for Last Index
	if(%index == %this.size - 1)
	{
		%this.pair[%index] = "";

		%this.size--;
		return %old;
	}
	else
	{
		// Store the Original Value
		%original = %this.pair[%index];

		// Shift Entries Downwards
		for(%i = %index; %i < %this.size - 1; %i++)
			%this.pair[%i] = %this.pair[%i + 1];

		// Clear the Last Pair
		%this.pair[%this.size - 1] = "";

		%this.size--;
		return %original;
	}
}

// Returns the Pair at the specified Index
function JMap::getPair(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Access Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the Map!");
		return "";
	}

	// Return the Pair
	return %this.pair[%index];
}

// Sets the Pair at the specified Index
function JMap::setPair(%this, %index, %pair)
{
	// Verify Pair Type
	if(!isObject(%pair) || %pair.class !$= "JPair")
	{
		%this.throwException("ClassType", "An attempt to Change Pair \"\c1" @ %pair @ "\c0\" at Index \"\c1" @ %index @ "\c0\" failed. The specified Pair is not of JPair Type!");
		return false;
	}

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Change Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the Map!");
		return "";
	}

	// Set the Pair
	%old = %this.pair[%index];
	%this.pair[%index] = %pair;

	return %old;
}

//* Value Methods *//
// Adds the specified Value to the Map
function JMap::addValue(%this, %key, %value, %marked)
{
	// Specify Marked
	if(%marked $= "")
		%marked = false;

	// Add the Value
	%this.pair[%this.size] = new ScriptObject()
	{
		class = JPair;
		pairName = %key;
		pairValue = %value;
		pairMarked = %marked;
	};

	// Increase the Size
	%this.size++;

	return true;
}

// Removes the First Instance of the specified Value from the Map
function JMap::removeValue(%this, %value)
{
	// Determine the Index of the specified Value
	%index = %this.indexOfValue(%value);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("ValueNonExistant", "An attempt to Remove Value \"\c1" @ %value @ "\c0\" failed. The specified Value is not within the Map!");
		return "";
	}

	// Remove the Value by Index
	%this.removeIndex(%index);

	// Return the Index
	return %index;
}

// Returns the Value at the specified Key
function JMap::getValue(%this, %key)
{
	// Determine the Index of the specified Value
	%index = %this.indexOfKey(%key);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("KeyNonExistant", "An attempt to Access Key \"\c1" @ %key @ "\c0\" failed. The specified Key is not within the Map!");
		return "";
	}

	// Return the Value
	return %this.pair[%index].getValue();
}

// Sets the Value at the specified Key
function JMap::setValue(%this, %key, %value, %marked)
{
	// Determine the Index of the specified Value
	%index = %this.indexOfKey(%key);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("KeyNonExistant", "An attempt to Access Key \"\c1" @ %key @ "\c0\" failed. The specified Key is not within the Map!");
		return "";
	}

	// Specify Marked
	if(%marked $= "")
		%marked = %this.pair[%index].isMarked();

	// Set the Value
	%old = %this.pair[%index].getValue();
	%this.pair[%index].setValue(%value);
	%this.pair[%index].setMarked(%marked);

	return %old;
}

//* Conversion Methods *//
// Exports the JMap to the specified File Path
function JMap::exportMap(%this, %path)
{
	// Make sure the Path is Writeable
	if(!isWriteableFileName(%path))
	{
		%this.throwException("FileUnwriteable", "An attempt to Export the Map File \"\c1" @ %path @ "\c0\" failed. This Object does not have permission to write to the specified Export Path!");
		return false;
	}

	// Make sure the Map contains Entries
	if(%this.size == 0)
	{
		%this.throwException("InvalidState", "An attempt to Export the Map File \"\c1" @ %path @ "\c0\" failed. This Object does not have any writable content!");
		return false;
	}

	// Sort the Map if Needed
	if(%this.doAutoSort && !%this.sorted)
		%this.sort();

	// Create and Open the File
	%file = new FileObject();
	%file.openForWrite(%path);

	// Write as JSON
	%file.writeLine(%this.toJSON());

	// Close and Delete the File
	%file.close();
	%file.delete();

	return true;
}

//* Conversion Methods *//
// Returns a JSON Representation of the JMap
function JMap::toJSON(%this)
{
	// Open Map
	%json = "{";

	// Add Pairs
	for(%i = 0; %i < %this.size; %i++)
	{
		if(!isObject(%this.pair[%i]))
			continue;

		%json = %json @ %this.pair[%i].toJSON();

		if(%i < %this.size - 1)
			%json = %json @ ", ";
	}

	// Close Map
	%json = %json @ "}";

	return %json;
}