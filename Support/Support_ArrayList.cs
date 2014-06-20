//* Description *//
// Title: Array List
// Author: Boom (9740)
// Fast and Simple Array List

//* Version *//
$Temp::Version = 1.0; // Do not change this unless you know what you are doing

if($Support::ArrayList::Version !$= "") // Check for other versions
	if($Support::ArrayList::Version >= $Temp::Version) // A newer version has been loaded already
		return;

// At this point, this is newer than others that may exist
$Support::ArrayList::Version = $Temp::Version; // Update the version

//* Functions *//
// Trigger Methods:
// - ArrayList::onAdd(%this)
// - ArrayList::onRemove(%this)
// - ArrayList::onEntryAdd(%this, %index, %entry)
// - ArrayList::onEntryRemove(%this, %index, %entry)
// - ArrayList::onEntryUpdate(%this, %index, %old, %new)
// - ArrayList::onEntryMove(%this, %old, %new, %entry)
// - ArrayList::onEntrySwap(%this, %oldIndex, %newIndex, %oldEntry, %newEntry)
// Exception Methods:
// - ArrayList::onException(%this, %type, %msg)
// - ArrayList::throwException(%this, %type, %msg)
// - ArrayList::debug(%this, %access, %header, %msg)
// Utility Methods:
// - ArrayList::autoSave(%this, %force)
// - ArrayList::clearList(%this)
// - ArrayList::getEntryCount(%this)
// - ArrayList::deleteAll(%this)
// - ArrayList::dump(%this)
// Search and Sort Methods:
// - ArrayList::indexOf(%this, %entry [[, %pos = 0], %marked = false])
// - ArrayList::entryExists(%this, %entry [, %marked = false])
// - ArrayList::sort(%this)
// - ArrayList::quickSort(%this, %left, %right)
// - ArrayList::search(%this, %index [, %pos = 0])
// - ArrayList::swap(%this, %indexA, %indexB)
// - ArrayList::compare(%this, %valueA, %valueB)
// Entry Methods:
// - ArrayList::addEntry(%this, %entry [, %marked = false])
// - ArrayList::removeEntry(%this, %entry [, %marked = false])
// - ArrayList::removeIndex(%this, %index)
// - ArrayList::getEntry(%this, %index)
// - ArrayList::setEntry(%this, %index, %entry [, %marked = isMarked()])
// Marking Methods:
// - ArrayList::markIndex(%this, %index)
// - ArrayList::unmarkIndex(%this, %index)
// - ArrayList::isMarked(%this, %index)
// Conversion Methods:
// - ArrayList_importList(%path)
// - ArrayList::exportList(%this, %path)
// - ArrayList_construct(%json)
// - ArrayList::toJSON(%this)

//* Trigger Methods *//
// Triggered when the Array List is Created
function ArrayList::onAdd(%this)
{
	// Initialize Variables
	if(%this.doAutoSort $= "") %this.doAutoSort = false;
	if(%this.doAutoSave $= "") %this.doAutoSave = false;
	if(%this.sortByValue $= "") %this.sortByValue = false;
	if(%this.autoSaveInterval $= "") %this.autoSaveInterval = 60*1000; // 1 Minute
	if(%this.debugLevel $= "") %this.debugLevel = 0;

	// Check for Auto Saving
	if(%this.doAutoSave)
		%this.autoSave(true);

	// Initial Container Variables
	%this.size = 0;
	%this.sorted = true;

	// JSON Variables
	%this.isJSON = true;
	%this.constructor = "ArrayList_construct";

	%this.debug(1, "Add", "\c5ArrayList Created\c0");
}

// Triggered when the Array List is Destroyed
function ArrayList::onRemove(%this)
{
	// Stop Auto Saving Schedule
	cancel(%this.schedule_autoSave);

	%this.debug(1, "Remove", "\c5ArrayList Destroyed\c0");
}

// Triggered when an Entry is Added
function ArrayList::onEntryAdd(%this, %index, %entry)
{
	%this.debug(2, "Entry \c3|\c4 Add", "Entry '" @ %entry @ "' Added at Index '" @ %index @ "'");
	return true;
}

// Triggered when an Entry is Removed
function ArrayList::onEntryRemove(%this, %index, %entry)
{
	%this.debug(2, "Entry \c3|\c4 Remove", "Entry '" @ %entry @ "' Removed at Index '" @ %index @ "'");
	return true;
}

// Triggered when an Entry is Updated
function ArrayList::onEntryUpdate(%this, %index, %old, %new)
{
	%this.debug(2, "Entry\c3|\c4 Update", "Index '" @ %index @ "' changed from '" @ %old @ "' to '" @ %new @ "'");
	return true;
}

// Triggered when an Entry is Moved
function ArrayList::onEntryMove(%this, %old, %new, %entry)
{
	%this.debug(3, "Entry \c3|\c4 Move", "Entry '" @ %entry @ "' Moved from Index '" @ %old @ "' to Index '" @ %new @ "'");
	return true;
}

// Triggered when Entries are Swapped
function ArrayList::onEntrySwap(%this, %oldIndex, %newIndex, %oldEntry, %newEntry)
{
	%this.debug(3, "Entry \c3|\c4 Swap", "Entries '" @ %oldEntry @ "' and '" @ %newEntry @ "' were Swapped (Between Indeces '" @ %oldIndex @ "' and '" @ %newIndex @ "' respectively)");
	return true;
}

//* Exception Methods *//
// Triggered when an Exception is Thrown
function ArrayList::onException(%this, %type, %msg)
{
	// Use Debugger to Display Exception
	%this.debug(1, "\c2Exception\c4 (\c1" @ %type @ "\c4)", %msg);
	return true;
}

// Throws an Exception
function ArrayList::throwException(%this, %type, %msg)
{
	// Update Last Exception Values
	%this.lastExceptionType = %type;
	%this.lastExceptionMessage = %msg;

	// Call the Exception Trigger
	%this.onException(%type, %msg);
}

// Prints the specified Message to the Console
function ArrayList::debug(%this, %access, %header, %msg)
{
	if(%this.debugLevel >= %access)
		echo("\c3 + \c4[Array List (\c1" @ %this @ "\c4) \c3|\c4 " @ %header @ "]\c0: " @ %msg);

	return %this.debugLevel >= %access;
}

//* Utility Methods *//
// Repeats a Scheudle to Save the Array List
function ArrayList::autoSave(%this, %force)
{
	// Stop all Auto Save Schedules
	cancel(%this.schedule_autoSave);

	// Perform Auto Save
	if(%this.autoSave)
	{
		if(!%force)
			%this.exportList(%this.path);

		%this.schedule_autoSave = %this.schedule(%this.autoSaveInterval, "autoSave");
	}
}

// Clears all Entries in the Array List
function ArrayList::clearList(%this)
{
	// Iterate through the List
	for(%i = 0; %i < %this.size; %i++)
	{
		%old = %this.entry[%i];
		%this.entry[%i] = "";
		%this.marked[%i] = "";

		// Call the Remove Trigger
		%this.onEntryRemove(%i, %old);
	}

	%this.size = 0;
	%this.sorted = true;

	return true;
}

// Returns the Number of Entries in the Array List
function ArrayList::getEntryCount(%this)
{
	return %this.size;
}

// Deletes the Object and its Descendants
function ArrayList::deleteAll(%this)
{
	for(%i = 0; %i < %this.size; %i++)
		if(%this.entry[%i].marked && isObject(%this.entry[%i]))
			if(%this.entry[%i].isJSON)
				%this.entry[%i].deleteAll();
			else
				%this.entry[%i].delete();

	%this.delete();
}

// Dumps the Object Properties to the Console
function ArrayList::dump(%this)
{
	%dump = "\c4Member Fields:\c0" NL
		"   class = \"\c1ArrayList\c0\"" NL
		"\c4Tagged Fields:\c0" NL
		"   doAutoSave = \c4" @ (%this.doAutoSave ? "true" : "false") @ "\c0" NL
		"   doAutoSort = \c4" @ (%this.doAutoSort ? "true" : "false") @ "\c0" NL
		"   sortByValue = \c4" @ (%this.sortByValue ? "true" : "false") @ "\c0" NL
		"   sorted = \c4" @ (%this.sorted ? "true" : "false") @ "\c0" NL
		"   autoSaveInterval = \c2" @ %this.autoSaveInterval @ "\c0" NL
		"   size = \c2" @ %this.size @ "\c0" NL
		"   debugLevel = \c2" @ %this.debugLevel @ "\c0" NL
		"   isJSON = \c4" @ (%this.isJSON ? "true" : "false") @ "\c0" NL
		"   constructor = \"\c1" @ %this.constructor @ "\c0\"" NL
		"\c4Methods:\c0" NL
		"   onAdd() - \c1Triggered when the ArrayList is Created\c0" NL
		"   onRemove() - \c1Triggered when the ArrayList is Destroyed\c0" NL
		"   onEntryAdd(\c5%index\c0, \c5%entry\c0) - \c1Triggered when an Entry is Added to the ArrayList\c0" NL
		"   onEntryRemove(\c5%index\c0, \c5%entry\c0) - \c1Triggered when an Entry is Removed from the ArrayList\c0" NL
		"   onEntryUpdate(\c5%index\c0, \c5%old\c0, \c5%new\c0) - \c1Triggered when an Entry within the ArrayList is Changed\c0" NL
		"   onEntryMove(\c5%old\c0, \c5%new\c0, \c5%entry\c0) - \c1Triggered when an Entry is Moved within the ArrayList\c0" NL
		"   onEntrySwap(\c5%oldIndex\c0, \c5%newIndex\c0, \c5%oldEntry\c0, \c5%newEntry\c0) - \c1Triggered when Entries within the ArrayList are Swapped\c0" NL
		"   onException(\c5%type\c0, \c5%msg\c0) - \c1Triggered when an Exception is Thrown\c0" NL
		"   throwException(\c5%type\c0, \c5%msg\c0) - \c1Throws an Exception\c0" NL
		"   autoSave(\c5%force\c0) - \c1Call a Repeating Schedule to Automatically Save\c0" NL
		"   clearList() - \c1Clears the Contents of the ArrayList\c0" NL
		"   getEntryCount() - \c1Returns the Number of Entries in the ArrayList\c0" NL
		"   indexOf(\c5%entry\c0 \c9[[, %pos = 0], %marked = false]\c0) - \c1Returns the Index of the First Instance of the specified Entry\c0" NL
		"   entryExists(\c5%entry \c9[, %marked = false]\c0) - \c1Returns whether or not the specified Entry is within the ArrayList\c0" NL
		"   sort() - \c1Sorts the Entries within the ArrayList\c0" NL
		"   swap(\c5%indexA\c0, \c5%indexB\c0) - \c1Swaps the specified Entries\c0" NL
		"   addEntry(\c5%entry \c9[, %marked = false]\c0) - \c1Adds the specified Entry to the ArrayList\c0" NL
		"   insertEntry(\c5%entry\c0, \c5%index \c9[, %marked = false]\c0) - \c1Inserts the specified Entry at the specifeid Index into the ArrayList\c0" NL
		"   removeEntry(\c5%entry\c0 \c9[, %marked = false]) - \c1Removes the First Instance of the specified Entry from the ArrayList\c0" NL
		"   removeIndex(\c5%index\c0) - \c1Removes the specified Index from the ArrayList\c0" NL
		"   getEntry(\c5%index\c0) - \c1Returns the Entry at the specified Index\c0" NL
		"   setEntry(\c5%index\c0, \c5%value \c9[, %marked = isMarked]\c0) - \c1Sets the Entry at the specified Index\c0" NL
		"   isMarked(\c5%index\c0) - \c1Returns whether or not the specified Index is Marked for JSON Export\c0" NL
		"   setMarked(\c5%index\c0, \c5%marked\c0) - \c1Sets whether or not the specified Index is Marked for JSON Export\c0" NL
		"   markIndex(\c5%index\c0) - \c1Marks the specified Index for JSON Export\c0" NL
		"   unmarkIndex(\c5%index\c0) - \c1Unmarks the specified Index for JSON Export\c0" NL
		"   exportList(\c5%path\c0) - \c1Exports the ArrayList to the specified File Path\c0" NL
		"   toJSON() - \c1Returns a JSON String representation of the ArrayList\c0" NL
		"   delete() - \c1Destroys the ArrayList\c0" NL
		"   deleteAll() - \c1Destroys the ArrayList and all Descendants\c0" NL
		"   dump() - \c1Displays this Text\c0";
	echo(%dump);
}

//* Search and Sort Methods *//
// Returns the Index of the First Instance of the specified Entry
function ArrayList::indexOf(%this, %entry, %pos, %marked)
{
	// Search for the Entry
	%index = %this.search(%entry, %pos, %marked);

	// Match the Outcome
	if(%index != -1 && %this.entry[%index] $= %entry)
		return %index;
	else
		return -1;
}

// Returns whether or not the specified Entry exists within the Array List
function ArrayList::entryExists(%this, %entry, %marked)
{
	return %this.indexOf(%entry, %marked) >= 0;
}

// Sorts the Enties in the ArrayList
function ArrayList::sort(%this)
{
	%this.quickSort(0, %this.size - 1);
}

// Performs the Quicksort Algorithm
function ArrayList::quickSort(%this, %left, %right)
{
	// Determine the Pivot Entry
	%pivot = %this.entry[mFloor((%left + %right)/2)];

	// Remap the Left and Right Indeces
	%i = %left;
	%j = %right;

	// Move Values across Pivot
	while(%i <= %j)
	{
		// Move Left to Unsorted Value
		while(%this.compare(%this.entry[%i], %pivot) < 0)
			%i++;

		// Move Right to Unsorted Value
		while(%this.compare(%this.entry[%j], %pivot) > 0)
			%j--;

		// Switch Unsorted Values
		if(%i <= %j)
		{
			%this.swap(%i, %j);
			%i++;
			%j--;
		}
	}

	// Check for Sorting on the Left
	if(%left < %j)
		%this.quickSort(%left, %j);

	// Check for Sorting on the Right
	if(%right > %i)
		%this.quickSort(%i, %right);
}

// Returns the Closest Index Matching the specified Entry
function ArrayList::search(%this, %entry, %pos, %marked)
{
	// Validate Position
	if(%pos $= "") // Not Specified
		%pos = 0;
	else
		%pos = mClamp(%pos, 0, %this.size - 1);

	// Check to see if Binary Search can be Performed
	if(%this.doAutoSort)
	{
		// Make sure the ArrayList is Sorted
		if(!%this.sorted)
			%this.sort(); // Sort the List

		// Perform Binary Search
		%left = %pos;
		%right = %this.size - 1;
		%middle = mFloor((%left + %right)/2);

		while(%left < %right)
		{
			// Determine the Middle Index and Value
			%middle = mFloor((%left + %right)/2);
			%value = %this.entry[%middle];

			%comparison = %this.compare(%value, %entry);

			if(%comparison == 0)
				return %middle;
			else if(%comparison < 0)
				%left = %middle + 1;
			else
				%right = %middle - 1;
		}

		return mFloor((%left + %right)/2);
	}
	else
	{
		// Iterate through the List
		for(%i = %pos; %i < %this.size; %i++)
			if(%this.entry[%i] $= %entry)
				return %i;

		return -1;
	}
}

// Swaps the specified Entries
function ArrayList::swap(%this, %indexA, %indexB)
{
	// Validate Indeces
	if(%indexA < 0 || %indexA >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Swap Indeces '" @ %indexA @ "' and '" @ %indexB @ "' failed. The First specified Index is not within the Range of the List");
		return false;
	}

	if(%indexB < 0 || %indexB >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Swap Indeces '" @ %indexA @ "' and '" @ %indexB @ "' failed. The Second specified Index is not within the Range of the List");
		return false;
	}

	// Swap the specified Entries
	%temp = %this.entry[%indexA];
	%this.entry[%indexA] = %this.entry[%indexB];
	%this.entry[%indexB] = %temp;

	// Swap Marks
	%temp = %this.marked[%indexA];
	%this.marked[%indexA] = %this.marked[%indexB];
	%this.marked[%indexB] = %temp;

	// Call the Swap Trigger
	%this.onEntrySwap(%indexA, %indexB, %this.entry[%indexB], %this.entry[%indexA]);

	return true;
}

// Returns an Integer Representation of a Comparison (-1, 0, 1)
function ArrayList::compare(%this, %valueA, %valueB)
{
	// Determine Compare Type
	if(%this.sortByValue)
	{
		if(%valueA == %valueB)
			return 0;
		else if(%valueA < %valueB)
			return -1;
		else
			return 1;
	}

	return strcmp(%valueA, %valueB);
}

//* Entry Methods *//
// Adds the specified Entry to the Array List
function ArrayList::addEntry(%this, %entry, %marked)
{
	// Specify Marked
	if(%marked $= "") %marked = false;

	// Determine whether or not to Insert by Sort
	if(%this.doAutoSort)
	{
		// Check for Trival Case
		if(%this.size == 0)
		{
			// Add the Entry
			%this.entry[0] = %entry;
			%this.marked[0] = %marked;

			// Call the Add Trigger
			%this.onEntryAdd(0, %entry);
			%this.size++;

			return true;
		}

		// Find the Closest Index
		%index = %this.search(%entry);
		%value = %this.entry[%index];

		// Insert near Closest Entry
		if(%this.compare(%entry, %value) < 0)
			return %this.insertEntry(%index, %entry, %marked, true);
		else
			return %this.insertEntry(%index + 1, %entry, %marked, true);
	}
	else
	{
		// Add the Entry
		%this.entry[%this.size] = %entry;
		%this.marked[%this.size] = %marked;

		// Call the Add Trigger
		%this.onEntryAdd(%this.size, %entry);
		%this.sorted = false;

		// Increase the Size
		%this.size++;
	}

	return true;
}

// Inserts the specified Entry at the specified Position into the Array List
function ArrayList::insertEntry(%this, %index, %entry, %marked, %keepSort)
{
	// Validate KeepSort
	if(%keepSort $= "") %keepSort = false;

	// Validate Index
	if(%index < 0 || %index > %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Insert at Index '" @ %index @ "' failed. The specified Index is not within the Range of the List");
		return false;
	}

	// Specify Marked
	if(%marked $= "") %marked = false;

	// Insert Entry at the specified Position
	for(%i = %this.size; %i > %index; %i--)
	{
		// Move Entries Forward
		%this.entry[%i] = %this.entry[%i - 1];
		%this.marked[%i] = %this.marked[%i - 1];

		// Call Move Trigger
		%this.onEntryMove(%i - 1, %i, %this.entry[%i]);
	}

	%this.entry[%index] = %entry;
	%this.marked[%index] = %marked;

	// Call the Add Trigger
	%this.onEntryAdd(%index, %entry);

	// Increase the Size
	%this.size++;

	// Check for Sort Invalidation
	if(%this.sorted && !%keepSort)
	{
		%this.sorted = false;
		%this.debug(1, "\c2Warning\c4", "\c0The ArrayList is no longer Sorted due to an Insertion at Index '" @ %index @ "'");
	}

	return true;
}

// Removes the First Instance of the specified Entry from the Array List
function ArrayList::removeEntry(%this, %entry, %marked)
{
	// Determine the Index of the specified Entry
	%index = %this.indexOf(%entry, %marked);

	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("EntryNonExistant", "An attempt to Remove Entry '" @ %entry @ "' failed. The specified Entry is not an Element within the List!");
		return "";
	}

	// Remove the Entry by Index
	%this.removeIndex(%index);

	// Return the Index
	return %index;
}

// Removes the specified Index from the Array List
function ArrayList::removeIndex(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Remove Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Check for Last Index
	if(%index == %this.size - 1)
	{
		%old = %this.entry[%index];
		%this.entry[%index] = "";
		%this.marked[%index] = "";

		// Call the Remove Trigger
		%this.onEntryRemove(%index, %old);

		%this.size--;
		return %old;
	}
	else
	{
		// Store the Original Value
		%original = %this.entry[%index];

		// Shift Entries Downwards
		for(%i = %index; %i < %this.size - 1; %i++)
		{
			%this.entry[%i] = %this.entry[%i + 1];
			%this.marked[%i] = %this.marked[%i + 1];

			// Call the Move Trigger
			%this.onEntryMove(%i, %i + 1, %this.entry[%i + 1]);
		}

		// Clear the Last Entry
		%this.entry[%this.size - 1] = "";
		%this.marked[%this.size - 1] = "";

		// Call the Remove Trigger
		%this.onEntryRemove(%index, %original);

		%this.size--;
		return %original;
	}
}

// Returns the Entry at the specified Index
function ArrayList::getEntry(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Access Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Return the Entry
	return %this.entry[%index];
}

// Sets the Entry at the specified Index
function ArrayList::setEntry(%this, %index, %entry, %marked)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Change Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return "";
	}

	// Specify Marked
	if(%marked $= "") %marked = %this.marked[%index];

	// Set the Entry
	%old = %this.entry[%index];
	%this.entry[%index] = %entry;
	%this.marked[%index] = %marked;

	// Call the Update Trigger
	%this.onEntryUpdate(%index, %old, %entry);

	// Check for Sort Invalidation
	if(%index != 0 && %this.compare(%entry, %this.entry[%index - 1]) > 0)
	{
		%this.sorted = false;
		%this.debug(1, "\c2Warning\c4", "\c0The ArrayList is no longer Sorted due to a Change at Index '" @ %index @ "'");
	}
	else if(%index != %this.size - 1 && %this.compare(%entry, %this.entry[%index + 1]) < 0)
	{
		%this.sorted = false;
		%this.debug(1, "\c2Warning\c4", "\c0The ArrayList is no longer Sorted due to a Change at Index '" @ %index @ "'");
	}

	return %old;
}

//* Marking Methods *//
// Returns whether or not the specified Index is Marked
function ArrayList::isMarked(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Access Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Return Mark Status
	return %this.marked[%index];
}

// Sets whether or not the specified Index is Marked
function ArrayList::isMarked(%this, %index, %marked)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Change Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Return Mark Status
	return %this.marked[%index] = %marked;
}

// Marks the specified Index for Export
function ArrayList::markIndex(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Mark Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Mark the Index
	%this.marked[%index] = true;
	return true;
}

// Unmarks the specified Index for Export
function ArrayList::unmarkIndex(%this, %index)
{
	// Validate the Index
	if(%index < 0 || %index >= %this.size)
	{
		%this.throwException("IndexOutOfBounds", "An attempt to Unmark Index '" @ %index @ "' failed. The specified Index is not within the Range of the List!");
		return false;
	}

	// Mark the Index
	%this.marked[%index] = false;
	return true;
}


//* Conversion Methods *//
// Imports the ArrayList from the specified File Path
function ArrayList_importList(%path)
{
	// Make sure the File Exists
	if(!isFile(%path))
	{
		echo("\c3 + \c4[ArrayList \c3|\c4 \c2Exception \c4(\c1FileNonExistant\c4)]\c0: An attempt to Import the List File '" @ %path @ "' failed. The specified File does not Exist!");
		return false;
	}

	// Create and Open the File
	%file = new FileObject();
	%file.openForRead(%path);

	// Create the JSON String
	%json = %file.readLine();

	while(!%file.isEOF())
		%json = %json SPC %file.readLine();

	// Close the File
	%file.close();
	%file.delete();

	// Import from JSON
	return JSON_decode_ArrayList(%json);
}

// Exports the ArrayList to the specified File Path
function ArrayList::exportList(%this, %path)
{
	// Make sure the Path is Writeable
	if(!isWriteableFileName(%path))
	{
		%this.throwException("FileUnwriteable", "An attempt to Export the List File '" @ %path @ "' failed. This Object does not have permission to write to the specified Export Path!");
		return false;
	}

	// Make sure the List contains Entries
	if(%this.size == 0)
	{
		%this.throwException("InvalidState", "An attempt to Export the List File '" @ %path @ "' failed. This Object does not have any writable content!");
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

// Returns a JSON Representation of the ArrayList
function ArrayList::toJSON(%this)
{
	// Convert List to JSON
	%json = new ScriptObject()
	{
		class = JMap;
	};

	%json.addValue("class", "ArrayList");
	%json.addValue("doAutoSort", %this.doAutoSort);
	%json.addValue("doAutoSave", %this.doAutoSave);
	%json.addValue("sortByValue", %this.sortByValue);
	%json.addValue("autoSaveInterval", %this.autoSaveInterval);
	%json.addValue("sorted", %this.sorted);
	%json.addValue("debugLevel", %this.debugLevel);
	%json.addValue("constructor", %this.constructor);

	// Add Entries
	%entries = new ScriptObject()
	{
		class = JList;
	};

	for(%i = 0; %i < %this.size; %i++)
		%entries.addValue(%this.entry[%i], %this.marked[%i]);

	%json.addValue("entries", %entries, true);

	// Flatten JSON
	return %json.toJSON();
}

// Returns an Array List constructed from the specified JSON
function JSON_decode_ArrayList(%json)
{
	// Decode JSON
	%json = JSON_decode(%json);

	// Make sure the Decoding Worked
	if(!isObject(%json))
		return 0;

	// Make sure the Object is the right Class
	if(%json.getValue("class") !$= "ArrayList")
		return 0;

	// Create a new Array List
	%list = new ScriptObject()
	{
		class = ArrayList;
	};

	// Iterate
	for(%i = 0; %i < %json.getPairCount(); %i++)
	{
		%pair = %json.getPair(%i);

		if(%pair.getName() $= "entries")
		{
			%entries = %pair.getValue();

			for(%j = 0; %j < %entries.getElementCount(); %j++)
			{
				%entry = %entries.getElement(%j);

				// Check for Embedded JSON Objects
				if(%entry.isMarked())
				{
					// Determine the Object
					%obj = %entry.getValue();

					// Determine how to Construct the Object
					%constructor = %obj.getValue("constructor");

					// Call Object Constructor
					if(isFunction(%constructor))
						eval(%obj @ " = " @ %constructor @ "(" @ %obj @ ");");
					else
						%obj = 0; // Can't Construct Object

					// Add the Object
					%list.addEntry(%obj, true);
				}
				else
					%list.addEntry(%entry.getValue());
			}
		}
		else
			eval(%list @ "." @ %pair.getName() @ " = " @ %pair.getValue() @ ";");
	}

	// Destroy all JSON Objects
	%json.deleteAll();

	return %list;
}

// Testing
function test()
{
	ArrayList_importList("config/ArrayList/test.json");
}