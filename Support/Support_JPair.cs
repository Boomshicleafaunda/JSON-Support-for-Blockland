//* Description *//
// Title: JSON Pair
// Author: Boom (9740)
// Defines a JSON Pair

//* Version *//
$Temp::Version = 1.0; // Do not change this unless you know what you are doing

if($Support::JSON::Pair::Version !$= "") // Check for other versions
	if($Support::JSON::Pair::Version >= $Temp::Version) // A newer version has been loaded already
		return;

// At this point, this is the newest Version
$Support::JSON::Pair::Version = $Temp::Version; // Update the version

//* Functions *//
// Trigger Methods:
// - onAdd(%this)
// - onRemove(%this)
// Utility Methods:
// - deleteAll(%this)
// Name Methods:
// - getName(%this)
// - setName(%this, %name)
// Value Methods:
// - getValue(%this)
// - setValue(%this, %value)
// Marking Methods:
// - isMarked(%this)
// - mark(%this)
// - unmark(%this)
// - setMarked(%this, %marked)
// Conversion Methods:
// - toJSON(%this)

//* Trigger Methods *//
// Triggered when the JPair is Created
function JPair::onAdd(%this)
{
	%this.isJSON = true;
}

// Triggered when the JPair is Destroyed
function JPair::onRemove(%this)
{
}

//* Utility Methods *//
// Deletes the Object and its Descendants
function JPair::deleteAll(%this)
{
	if(%this.pairMarked && isObject(%this.pairValue))
		if(%this.pairValue.isJSON)
			%this.pairValue.deleteAll();
		else
			%this.pairValue.delete();

	%this.delete();
}

//* Name Methods *//
// Returns the Name within the Pair
function JPair::getName(%this)
{
	return %this.pairName;
}

// Sets the Name within the Pair
function JPair::setName(%this, %name)
{
	%old = %this.pairName;
	%this.pairName = %name;
	return %old;
}

//* Value Methods *//
// Returns the Value within the Pair
function JPair::getValue(%this)
{
	return %this.pairValue;
}

// Sets the Value within the Pair
function JPair::setValue(%this, %value)
{
	%old = %this.pairValue;
	%this.pairValue = %value;
	return %old;
}

//* Marking Methods *//
// Returns whether or not the Pair is Marked
function JPair::isMarked(%this)
{
	return %this.pairMarked;
}

// Marks the Pair for JSON Encoding
function JPair::mark(%this)
{
	%this.pairMarked = true;
}

// Unmarks the Pair for JSON Encoding
function JPair::unmark(%this)
{
	%this.pairMarked = false;
}

// Sets the Mark Status of the Pair for JSON Encoding
function JPair::setMarked(%this, %marked)
{
	%this.pairMarked = %marked;
}

//* Conversion Methods *//
// Returns a JSON Representation of the JPair
function JPair::toJSON(%this)
{
	if(!%this.pairMarked || !isObject(%this.pairValue) || !%this.pairValue.isJSON)
		return "\"" @ %this.pairName @ "\": \"" @ %this.pairValue @ "\"";
	else
		return "\"" @ %this.pairName @ "\": " @ %this.pairValue.toJSON();
}