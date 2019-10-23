
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

const QueryTypes = Object.freeze({"Cache":0, "Centralized":1, "IPFS":2, "Swarm":3, "OtherUsers":4, "Registry":5});

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


async function AddAsset(_config, _bba)
{
	// Retrieve query orders from config.
	var _source;
	
	for (var i = 0; i < Object.keys(QueryTypes).length; i++)
	{
		_source = _config.QueryOrders[i];
		var result = await AddAssetFromSource(_config, _source, _bba);
		if (result != null)
		{
			return result;
		}
	}
	return null;	
}

async function AddAssetFromSource(_config, _source, _bba)
{
	var result;
	switch (_source) {
		case QueryTypes.Cache:
			result = await AddAssetToCache(_bba);
			break;
		case QueryTypes.Centralized:
			/*if (isURI(_arg))
			{
				BBA = await QueryAssetFromCentralizedServerWithURI(_arg);
			}
			else
			{*/
				result = await AddAssetToCentralizedServerWithHash(_bba);
			/*}*/
			break;
		case QueryTypes.IPFS:
			result = await AddAssetToIPFS(_bba)
			break;
		case QueryTypes.Swarm:
			result = await AddAssetToSwarm(_bba);
			break;
		case QueryTypes.OtherUsers:
			result = await AddAssetToOtherUsers(_bba);
			break;
		case QueryTypes.Registry:
			result = await AddAssetToRegistry(_registry, _bba);
			break;
		}
	return result;
}

async function AddAssetToCache(_bba)
{
	
	
	return null;
}

async function AddAssetToCentralizedServerWithHash(_bba)
{
	return null;
}

async function AddAssetToIPFS(_bba)
{
	var buf = Buffer.from(_bba, 'utf8');
	
	const ipfs = IPFS_Node;
	const result = await ipfs.add(buf)
	
	return result;
}

async function AddAssetToSwarm(_bba)
{
	return null;
}

async function AddAssetToOtherUsers(_bba)
{
	return null;
}

async function AddAssetToRegistry(_registry, _bba)
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
	const QueryOrders = [QueryTypes.Cache, QueryTypes.Centralized, QueryTypes.IPFS, QueryTypes.Swarm, QueryTypes.OtherUsers, QueryTypes.Registry];
	Config = SetQueryPriorities(Config, QueryOrders);
	const MasterRegistry = "";
	Config = SetMasterRegistry(Config, MasterRegistry);

	// Then, try and query some BBAs
	var result1 = await AddAsset(Config, "THIS IS MY BBA!");
	//var BBA2 = QueryAsset(Config, "");
	//var BBA3 = QueryAsset(Config, "");

	// Display the results
	console.log("result1 : ", result1);
	//console.log("BBA2 : ", BBA2);
	//console.log("BBA3 : ", BBA3);
	
	//await stopIPFS();
	
	return;
}

start();


