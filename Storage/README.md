# Storage

As this proposal aims to be as generic as possible, a developer that wants to implement BBAs for his solutions should be able to put the actual content of the asset (with its images, 3D models or anything else related to it) anywhere he wants.

He may want to store them:
* On his centralized server,
* On a distributed storage solution, like [IPFS](https://ipfs.io) or [Swarm](https://swarm-guide.readthedocs.io/en/latest/introduction.html),
* Or on the local machines of the users of his application, with Peer to Peer sharing.

We can uniquely reference the asset with his hash. Based on previous work, we can reference the hash of an asset on the blockchain (for example, inside a smart contract). We can also use the Token_URI function of the [ERC-721 Non-Fongible Token Standard](https://github.com/ethereum/EIPs/blob/master/EIPS/eip-721.md).

If we standardize a field that stores a link "bba://[content_hash]", we can define a workflow in the application BBAs to query the content of the assets in several places.

Technically, if we have the hash of the asset and a way to query adn retrieve it, we only need one node to have the actual content of the asset. All other copies would be replication, in case a node goes down or doesn't want to respond to a specific query.

The goal is then to ensure an incentive for nodes to keep the asset files, to avoid data loss.





