
// Specs from :
// https://github.com/blockchaingamealliance/BBA_ProofOfConcept/tree/master/Storage

// DataTypes to describe :
// BBA
// Registry
// Option

// _hash is a string
// _uri is a string

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
	return null;
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
	// First, create a new Configuration and set everything
	var Config = {QueryOrders : [], MasterRegistry : ""};
	const QueryOrders = [QueryTypes.Cache, QueryTypes.Centralized, QueryTypes.IPFS, QueryTypes.Swarm, QueryTypes.OtherUsers];
	Config = SetQueryPriorities(Config, QueryOrders);
	const MasterRegistry = "";
	Config = SetMasterRegistry(Config, MasterRegistry);

	// Then, try and query some BBAs
	var BBA1 = await QueryAsset(Config, "");
	//var BBA2 = QueryAsset(Config, "");
	//var BBA3 = QueryAsset(Config, "");

	// Display the results
	console.log("BBA1 : ", BBA1);
	//console.log("BBA2 : ", BBA2);
	//console.log("BBA3 : ", BBA3);
}

start();








