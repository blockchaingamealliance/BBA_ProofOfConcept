# BBA Viewer

This Unity project aims to help users visualize [Blockchain Bean Assets](https://medium.com/b2expand/bba-blockchain-bean-assets-2eb3b6b34812).

## Features
* Drag and drop .bba files to import
* Extracts them and parse
	* Metadata: Asset name, description, properties
	* Images
	* 3D Model (.obj file, and a reference to a material of the Unity project)
* Hash and sign the asset

## Future features
* Include the model's material in the .bba file.
* Write metadata as a JSON file.
* Edit and create new BBAs.
* Load the priv-key from a wallet (e.g. Metamask). Currently it is an hardcoded private address, only used for signing.

## BBA Creation
You can extract the sample BBA's (Assets/Models/BBAs) with any zip extractor. This lets you understand their structure.
