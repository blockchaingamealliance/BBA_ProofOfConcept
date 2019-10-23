
// Specs from :
// https://github.com/blockchaingamealliance/BBA_ProofOfConcept/tree/master/Storage

// DataTypes to describe :
// BBA
// Registry
// Option

// _hash is a string
// _uri is a string

function makeBBA(name, creator) {
  var names = names.split(' ');
  var count = names.length;
  function constructor() {
    for (var i = 0; i < count; i++) {
      this[names[i]] = arguments[i];
    }
  }
  return constructor;
}

const QueryTypes = Object.freeze({"Cache":1, "Centralized":2, "IPFS":3, "Swarm":4, "OtherUsers":5});

var Config = {QueryOrders : [QueryTypes.Cache, QueryTypes.Centralized, QueryTypes.IPFS, QueryTypes.Swarm, QueryTypes.OtherUsers]};


function CheckValid(_bba)
{
	return true; //  For now
}

function SetQueryPriorities(_options)
{
	
}

function AddRegistry(_registry)
{
	
}


async function QueryAsset(_config)
{
	// Retrieve query orders from config.
	
	QueryOrders = _config.QueryOrders;
	
	for (var i = 0; i < Object.keys(QueryTypes).length; i++)
	{
		var BBA = await QueryAssetFromSource(_config, _source);
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
	
	
	return BBA;
}

async function QueryAssetFromCentralizedServerWithURI(_uri)
{
	
}

async function QueryAssetFromCentralizedServerWithHash(_hash)
{
	
}

async function QueryAssetFromIPFS(_hash)
{
	
}

async function QueryAssetFromSwarm(_hash)
{
	
}

async function QueryAssetFromOtherUsers(_hash)
{
	
}

async function QueryAssetFromRegistry(_registry, _hash)
{
	
}

async function GetRegistryListFromMasterRegistry()
{
	
}

var BBA1 = QueryAsset();
var BBA2 = QueryAsset();
var BBA3 = QueryAsset();

console.log("BBA1 : ", BBA1);
console.log("BBA2 : ", BBA2);
console.log("BBA3 : ", BBA3);