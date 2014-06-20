//* Description *//
// Title: JList
// Author: Boom (9740)
// Defines a JSON Array List

//* Version *//
$Temp::Version = 1.0; // Do not change this unless you know what you are doing

if($Support::JSON::List::Version !$= "") // Check for other versions
	if($Support::JSON::List::Version >= $Temp::Version) // A newer version has been loaded already
		return;

// At this point, this is the newest Version
$Support::JSON::List::Version = $Temp::Version; // Update the version

//* Functions *//
// Trigger Methods:
// - onAdd(%this)
// - onRemove(%this)
// Exception Methods:
// - onException(%this, %type, %msg)
// - throwException(%this, %type, %msg)
// - debug(%this, %access, %header, %msg)
// Utility Methods:
// - clearList(%this)
// - getElementCount(%this)
// - deleteAll(%this)
// Search Methods:
// - indexOfElement(%this, %element, %pos)
// - indexOfValue(%this, %value, %pos, %marked)
// - elementExists(%this, %element)
// - valueExists(%this, %value, %marked)
// Element Methods:
// - addElement(%this, %element)
// - insertElement(%this, %index, %element)
// - removeElement(%this, %element)
// - removeIndex(%this, %index)
// - getElement(%this, %index)
// - setElement(%this, %index, %element)
// Value Methods:
// - addValue(%this, %value, %marked)
// - insertValue(%this, %index, %value, %marked)
// - removeValue(%this, %value, %marked)
// - getValue(%this, %index)
// - setValue(%this, %index, %value, %marked)
// Conversion Methods:
// - exportList(%this, %path)
// - toJSON(%this)

//* Trigger Methods *//
// Triggered when the List is Created
function JList::onAdd(%this)
{
	%this.size = 0;
	%this.isJSON = true;
}

// Triggered when the List is Destroyed
function JList::onRemove(%this)
{
}

//* Exception Methods *//
// Triggered when an Exception is Thrown
function JList::onException(%this, %type, %msg)
{
	// Use Debugger to Display Exception
	%this.debug(1, "\c2Exception\c4 (\c1" @ %type @ "\c4)", %msg);
	return true;
}

// Throws an Exception
function JList::throwException(%this, %type, %msg)
{
	// Update Last Exception Values
	%this.lastExceptionType = %type;
	%this.lastExceptionMessage = %msg;

	// Call the Exception Trigger
	%this.onException(%type, %msg);
}

// Prints the specified Message to the Console
function JList::debug(%this, %access, %header, %msg)
{
	if(%this.debugLevel >= %access)
		echo("\c3 + \c4[JList (\c1" @ %this @ "\c4) \c3|\c4 " @ %header @ "]\c0: " @ %msg);

	return %this.debugLevel >= %access;
}

//* Utility Methods *//
// Clears all Entries in the List
function JList::clearList(%this)
{
	// Iterate through the List
	for(%i = 0; %i < %this.size; %i++)
	{
		%this.element[%i].delete();
		%this.element[%i] = "";
	}

	%this.size = 0;

	return true;
}

// Returns the Number of Entries in the List
function JList::getElementCount(%this)
{
	return %this.size;
}

// Deletes the Object and its Descendants
function JList::deleteAll(%this)
{
	for(%i = 0; %i < %this.size; %i++)
		%this.element[%i].deleteAll();

	%this.delete();
}

//* Search Methods *//
// Returns the Index of the First Instance of the specified Element
function JList::indexOfElement(%this, %element, %pos)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Iterate through the List
	for(%i = %pos; %i < %this.size; %i++)
		if(%this.element[%i] == %element)
			return %i;

	return -1;
}

// Returns the Index of the First Instance of the specified Value
function JList::indexOfValue(%this, %value, %pos, %marked)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Specify Marked
	if(%marked $= "")
		%marked = false;

	// Iterate through the List
	for(%i = %pos; %i < %this.size; %i++)
		if(%this.element[%i].getValue() $= %value && %this.element[%i].isMarked() == %marked)
			return %i;

	return -1;
}

// Returns whether or not the specified Element exists within the List
function JList::elementExists(%this, %element)
{
	return %this.indexOfElement(%element) >= 0;
}

// Returns whether or not the specified Value exists within the List
function JList::valueExists(%this, %value, %marked)
{
	return %this.indexOfValue(%value, 0, %marked) >= 0;
}

//* Element Methods *//
// Adds the specified Element to the List
function JList::addElement(%this, %element)
{
	// Verify Element Type
	if(!isObject(%element) || %element.class !$= "JElement")
	{
		%this.throwException("ClassType", "An attempt to Add Element \"\c1" @ %element @ "\c0\" failed. The specified Element is not of JElement Type!");
		return false;
	}

	// Add the Element
	%this.element[%this.size] = %element;

	// Increase the Size
	%this.size++;

	return true;
}

// Inserts the specified Element at the specified Position into the List
function JList::insertElement(%this, %index, %element)
{
	// Verify Element Type
	if(!isObject(%element) || %element.class !$= "JElement")
	{
		%this.throwException("ClassType", "An attempt to Insert Element \"\c1" @ %element @ "\c0\" at Index \"\c1" @ %index @ "\c0\" failed. The specified Element is not of JElement Type!");
		return false;
	}

	// Validate Index
	if(%index < 0 || %index > %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Insert at Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Insert Element at the specified Position
	for(%i = %this.size; %i > %index; %i--)
		%this.element[%i] = %this.element[%i - 1];

	%this.element[%index] = %element;

	// Increase the Size
	%this.size++;

	return true;
}

// Removes the First Instance of the specified Element from the List
function JList::removeElement(%this, %element)
{
	// Determine the Index of the specified Element
	%index = %this.indexOfElement(%element);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("ElementNonExistant", "An attempt to Remove Element \"\c1" @ %element @ "\c0\" failed. The specified Element is not within the List!");
		return "";
	}

	// Remove the Element by Index
	%this.removeIndex(%index);

	// Return the Index
	return %index;
}

// Removes the specified Index from the List
function JList::removeIndex(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Remove Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Check for Last Index
	if(%index == %this.size - 1)
	{
		%this.element[%index] = "";

		%this.size--;
		return %old;
	}
	else
	{
		// Store the Original Value
		%original = %this.element[%index];

		// Shift Entries Downwards
		for(%i = %index; %i < %this.size - 1; %i++)
			%this.element[%i] = %this.element[%i + 1];

		// Clear the Last Element
		%this.element[%this.size - 1] = "";

		%this.size--;
		return %original;
	}
}

// Returns the Element at the specified Index
function JList::getElement(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Access Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Return the Element
	return %this.element[%index];
}

// Sets the Element at the specified Index
function JList::setElement(%this, %index, %element)
{
	// Verify Element Type
	if(!isObject(%element) || %element.class !$= "JElement")
	{
		%this.throwException("ClassType", "An attempt to Change Element \"\c1" @ %element @ "\c0\"  at Index \"\c1" @ %index @ "\c0\" failed. The specified Element is not of JElement Type!");
		return false;
	}

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Change Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Set the Element
	%old = %this.element[%index];
	%this.element[%index] = %element;

	return %old;
}

//* Value Methods *//
// Adds the specified Value to the List
function JList::addValue(%this, %value, %marked)
{
	// Specify Marked
	if(%marked $= "")
		%marked = false;

	// Add the Value
	%this.element[%this.size] = new ScriptObject()
	{
		class = JElement;
		elementValue = %value;
		elementMarked = %marked;
	};

	// Increase the Size
	%this.size++;

	return true;
}

// Inserts the specified Value at the specified Position into the List
function JList::insertValue(%this, %index, %value, %marked)
{
	// Specify Marked
	if(%marked $= "")
		%marked = false;

	// Validate Index
	if(%index < 0 || %index > %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Insert at Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Insert Value at the specified Position
	for(%i = %this.size; %i > %index; %i--)
		%this.element[%i] = %this.element[%i - 1];

	%this.element[%index] = new ScriptObject()
	{
		class = JElement;
		elementValue = %value;
		elementMarked = %marked;
	};

	// Increase the Size
	%this.size++;

	return true;
}

// Removes the First Instance of the specified Value from the List
function JList::removeValue(%this, %value, %marked)
{
	// Specify Marked
	if(%marked $= "")
		%marked = false;

	// Determine the Index of the specified Value
	%index = %this.indexOfValue(%value, %marked);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("ValueNonExistant", "An attempt to Remove Value \"\c1" @ %value @ "\c0\" failed. The specified Value is not within the List!");
		return "";
	}

	// Remove the Value by Index
	%this.removeIndex(%index);

	// Return the Index
	return %index;
}

// Returns the Value at the specified Index
function JList::getValue(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Access Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Return the Element
	return %this.element[%index].getValue();
}

// Sets the Value at the specified Index
function JList::setValue(%this, %index, %value, %marked)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Change Index \"\c1" @ %index @ "\c0\" failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Specify Marked
	if(%marked $= "")
		%marked = %this.element[%index].isMarked();

	// Set the Element
	%old = %this.element[%index].getValue();
	%this.element[%index].setValue(%value);
	%this.element[%index].setMarked(%marked);

	return %old;
}

//* Conversion Methods *//
// Exports the JList to the specified File Path
function JList::exportList(%this, %path)
{
	// Make sure the Path is Writeable
	if(!isWriteableFileName(%path))
	{
		%this.throwException("FileUnwriteable", "An attempt to Export the List File \"\c1" @ %path @ "\c0\" failed. This Object does not have permission to write to the specified Export Path!");
		return false;
	}

	// Make sure the List contains Entries
	if(%this.size == 0)
	{
		%this.throwException("InvalidState", "An attempt to Export the List File \"\c1" @ %path @ "\c0\" failed. This Object does not have any writable content!");
		return false;
	}

	// Sort the List if Needed
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
// Returns a JSON Representation of the JList
function JList::toJSON(%this)
{
	// Open List
	%json = "[";

	// Add Elements
	for(%i = 0; %i < %this.size; %i++)
	{
		if(!isObject(%this.element[%i]))
			continue;

		%json = %json @ %this.element[%i].toJSON();

		if(%i < %this.size - 1)
			%json = %json @ ", ";
	}

	// Close List
	%json = %json @ "]";

	return %json;
}