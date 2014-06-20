//* Description *//
// Title: JSON Element
// Author: Boom (9740)
// Defines a JSON Element

//* Version *//
$Temp::Version = 1.0; // Do not change this unless you know what you are doing

if($Support::JSON::Element::Version !$= "") // Check for other versions
	if($Support::JSON::Element::Version >= $Temp::Version) // A newer version has been loaded already
		return;

// At this point, this is the newest Version
$Support::JSON::Element::Version = $Temp::Version; // Update the version

//* Functions *//
// Trigger Methods:
// - onAdd(%this)
// - onRemove(%this)
// Utility Methods:
// - deleteAll(%this)
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
// Triggered when the JElement is Created
function JElement::onAdd(%this)
{
	%this.isJSON = true;
}

// Triggered when the JElement is Destroyed
function JElement::onRemove(%this)
{
}

//* Utility Methods *//
// Deletes the Object and its Descendants
function JElement::deleteAll(%this)
{
	if(%this.elementMarked && isObject(%this.elementValue))
		if(%this.elementValue.isJSON)
			%this.elementValue.deleteAll();
		else
			%this.elementValue.delete();

	%this.delete();
}

//* Value Methods *//
// Returns the Value within the Element
function JElement::getValue(%this)
{
	return %this.elementValue;
}

// Sets the Value within the Element
function JElement::setValue(%this, %value)
{
	%old = %this.elementValue;
	%this.elementValue = %value;
	return %old;
}

//* Marking Methods *//
// Returns whether or not the Element is Marked
function JElement::isMarked(%this)
{
	return %this.elementMarked;
}

// Marks the Element for JSON Encoding
function JElement::mark(%this)
{
	%this.elementMarked = true;
}

// Unmarks the Element for JSON Encoding
function JElement::unmark(%this)
{
	%this.elementMarked = false;
}

// Sets the Mark Status of the Element for JSON Encoding
function JElement::setMarked(%this, %marked)
{
	%this.elementMarked = %marked;
}

//* Conversion Methods *//
// Returns a JSON Representation of the JElement
function JElement::toJSON(%this)
{
	if(!%this.elementMarked || !isObject(%this.elementValue) || !%this.elementValue.isJSON)
		return "\"" @ %this.elementValue @ "\"";
	else
		return %this.elementValue.toJSON();
}