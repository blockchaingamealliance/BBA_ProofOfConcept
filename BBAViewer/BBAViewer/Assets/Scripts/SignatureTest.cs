using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nethereum.Util;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Ionic.Zip;

public class SignatureTest : MonoBehaviour
{
    private string currentBBAFilename = "";

    public void SetCurrentBBA(string _currentBBAFilename)
    {
        currentBBAFilename = _currentBBAFilename;
    }

    public void Sign()
    {
        Debug.LogFormat("Signing file: {0}", currentBBAFilename);

        string filenameWithPath = currentBBAFilename;

        string msg = ""; // = "Message for signing";

        using (ZipFile zip1 = ZipFile.Read(filenameWithPath))
        {
            foreach (ZipEntry e in zip1)
            {
                System.IO.MemoryStream tempS = new System.IO.MemoryStream();
                e.Extract(tempS);
                msg = msg + tempS.ToString();
            }
        }

        //var privKey = EthECKey.GenerateKey();
        var privKey = new EthECKey("97ddae0f3a25b92268175400149d65d6887b9cefaf28ea2c078e05cdc15a3c0a");
        byte[] pubKeyCompressed = new ECKey(privKey.GetPrivateKeyAsBytes(), true).GetPubKey(true);
        Debug.LogFormat("Private key: {0}", privKey.GetPrivateKey().Substring(4));
        Debug.LogFormat("Public key: {0}", privKey.GetPubKey().ToHex().Substring(2));
        Debug.LogFormat("Public key (compressed): {0}", pubKeyCompressed.ToHex());

        Debug.LogFormat("");


        byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
        byte[] msgHash = new Sha3Keccack().CalculateHash(msgBytes);
        var signature = privKey.SignAndCalculateV(msgHash);
        Debug.LogFormat("Msg: {0}", msg);
        Debug.LogFormat("Msg hash: {0}", msgHash.ToHex());
        Debug.LogFormat("Signature: [v = {0}, r = {1}, s = {2}]",
            signature.V[0] - 27, signature.R.ToHex(), signature.S.ToHex());

        Debug.LogFormat("");

        var pubKeyRecovered = EthECKey.RecoverFromSignature(signature, msgHash);
        Debug.LogFormat("Recovered pubKey: {0}", pubKeyRecovered.GetPubKey().ToHex().Substring(2));

        bool validSig = pubKeyRecovered.Verify(msgHash, signature);
        Debug.LogFormat("Signature valid? {0}", validSig);

    }

    public void CheckSignature()
    {

    }








}
