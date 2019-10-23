
// Specs from :
// https://github.com/blockchaingamealliance/BBA_ProofOfConcept/tree/master/Storage

// DataTypes to describe :
// BBA
// Registry
// Option

// _hash is a string
// _uri is a string

const IPFS = require('ipfs');
var IPFS_Started = false;
var IPFS_node;

async function startIPFS()
{
	if (!IPFS_node)
	{
		IPFS_node = await IPFS.create();
	}
	IPFS_Started = true;
	return IPFS_node;
}

async function stopIPFS()
{
	try
	{
		await IPFS_node.stop();
		IPFS_Started = false;
		console.log('IPFS node stopped!');
	} 
	catch (error)
	{
		console.error('IPFS node failed to stop! ', error)
	}
}

	
/*
function makeBBA(name, creator) {
  var names = names.split(' ');
  var count = names.length;
  function constructor() {
    for (var i = 0; i < count; i++) {
      this[names[i]] = arguments[i];
    }
  }
  return constructor;
}*/

const QueryTypes = Object.freeze({"Cache":0, "Centralized":1, "IPFS":2, "Swarm":3, "OtherUsers":4});

function CheckValid(_bba)
{
	return true; //  For now
}

// Should be a prototype instead
function SetQueryPriorities(_config, _query_orders)
{
	_config.QueryOrders = _query_orders;
	
	return _config;
}

// Should be a prototype instead
function SetMasterRegistry(_config, _master_registry)
{
	_config.MasterRegistry = _master_registry;
	
	return _config;
}

function AddRegistry(_config, _registry)
{
	//_config.MasterRegistry;
	
	return;
}

function isURI(_arg)
{
	return false; // for now
}


async function QueryAsset(_config, _arg)
{
	// Retrieve query orders from config.
	var _source;
	
	for (var i = 0; i < Object.keys(QueryTypes).length; i++)
	{
		_source = _config.QueryOrders[i];
		var BBA = await QueryAssetFromSource(_config, _source, _arg);
		if (BBA != null && CheckValid(BBA))
		{
			return BBA;
		}
	}
	
	return null;	
}

async function QueryAssetFromSource(_config, _source, _arg)
{
	var BBA;
	if (_source == QueryTypes.Cache)
	{
		BBA = await QueryAssetFromCache(_arg);
	}
	else if (_source == QueryTypes.Centralized)
	{
		if (isURI(_arg))
		{
			BBA = await QueryAssetFromCentralizedServerWithURI(_arg);
		}
		else
		{
			BBA = await QueryAssetFromCentralizedServerWithHash(_arg);
		}
	}
    else if (_source == QueryTypes.IPFS)
	{
		BBA = await QueryAssetFromIPFS(_arg)
	}
	else if (_source == QueryTypes.Swarm)
	{
		BBA = await QueryAssetFromSwarm(_arg);
	}
    else if (_source == QueryTypes.OtherUsers)
	{
		BBA = await QueryAssetFromOtherUsers(_arg);
	}
	else if (_source == QueryTypes.Registry)
	{
		BBA = QueryAssetFromRegistry(_registry, _arg);
	}
	
	return BBA;
}

async function QueryAssetFromCache(_hash)
{
	
	
	return null;
}

async function QueryAssetFromCentralizedServerWithURI(_uri)
{
	return null;
}

async function QueryAssetFromCentralizedServerWithHash(_hash)
{
	return null;
}

async function QueryAssetFromIPFS(_hash)
{
	const ipfsPath = '/ipfs/' + _hash;
	
	const ipfs = IPFS_Node;
	const stream = await ipfs.get(ipfsPath)
	
	return stream;
}

async function QueryAssetFromSwarm(_hash)
{
	return null;
}

async function QueryAssetFromOtherUsers(_hash)
{
	return null;
}

async function QueryAssetFromRegistry(_registry, _hash)
{
	return null;
}

async function GetRegistryListFromMasterRegistry()
{
	return null;
}

async function start()
{
	IPFS_Node = await startIPFS();
	
	// First, create a new Configuration and set everything
	var Config = {QueryOrders : [], MasterRegistry : ""};
	const QueryOrders = [QueryTypes.Cache, QueryTypes.Centralized, QueryTypes.IPFS, QueryTypes.Swarm, QueryTypes.OtherUsers];
	Config = SetQueryPriorities(Config, QueryOrders);
	const MasterRegistry = "";
	Config = SetMasterRegistry(Config, MasterRegistry);

	// Then, try and query some BBAs
	var BBA1 = await QueryAsset(Config, "Qmaisz6NMhDB51cCvNWa1GMS7LU1pAxdF4Ld6Ft9kZEP2a");
	//var BBA2 = QueryAsset(Config, "");
	//var BBA3 = QueryAsset(Config, "");

	// Display the results
	console.log("BBA1 : ", BBA1);
	//console.log("BBA2 : ", BBA2);
	//console.log("BBA3 : ", BBA3);
	
	await stopIPFS();
	
	return;
}

start();


