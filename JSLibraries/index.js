
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

async function QueryAsset(_config)
{
	// Retrieve query orders from config.
	
	QueryOrders = _config.QueryOrders;
	
	for (var i = 0; i < Object.keys(QueryTypes).length; i++)
	{
		var BBA = await QueryAssetFromSource(_config, _source);
		if (BBA != null && CheckValid(BBA))
		{
			
		}
	}
	
	
	
	
	
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

async function QueryAssetFromSWARM(_hash)
{
	
}

async function QueryAssetFromOtherUsers(_hash)
{
	
}

async function QueryAssetFromRegistry(_registry, _hash)
{
	
}

async function SetQueryPriorities(_options)
{
	
}

async function Registry[] GetRegistryListFromMasterRegistry()
{
	
}

async function AddRegistry(_registry)
{
	
}































