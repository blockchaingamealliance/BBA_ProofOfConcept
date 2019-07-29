using System.Collections.Generic;
using UnityEngine;
using Ionic.Zip;
using System.IO;
using UnityEngine.UI;
using Crosstales.FB;
using System.Globalization;
using NAudio.Wave;
using NLayer.NAudioSupport;
using System.Text.RegularExpressions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Text;
using Nethereum.Util;
using System;
using System.Linq;

/// <summary>
/// This class handles
/// - .bba file extraction into stream.
/// - Metadata.txt parsing
///- Images loading
/// </summary>
public class LoadZip : MonoBehaviour {

    private static SignatureTest signer = null;
    private static string currentBBAFilename = "";

    private static EthECKey privKey;
    private static byte[] pubKey;

    /// <summary>
    /// Used as a UI toggle to show or not the 3D Model panel if there is or isn't an .obj file.
    /// </summary>
    private static bool showModel = false;
    /// <summary>
    /// Used as a UI toggle to show or not the Audio control panel if there is or isn't an .mp3 file.
    /// </summary>
    private static bool showAudioControl = false;
    private static bool autoPlay = false;
    private static int totalAudioBytes = 1;

    /// <summary>
    /// Used to keep track of the aspect ratio to preserve.
    /// </summary>
    private static float imageAspectRatio = 1;

    /// <summary>
    /// Load images from the scene or from the .bba file
    /// </summary>
    public static bool loadFromScene = false;
    private static MemoryStream material;
    public static MemoryStream tempAudioStream;
    public static WaveOut waveOut;
    /// <summary>
    /// MP3 reader to allow seeking based on the slider's position
    /// </summary>
    public static Mp3FileReader reader;

    /// <summary>
    /// Parses the Metadata.txt file and populate a Metadata struct.
    /// </summary>
    /// <param name="mem_stream">A MemoryStream to read the .txt file from</param>
    /// <returns>A Metadata struct to pass along to the CreateModelFromObj and PopulatePanels methods.</returns>
    private static Metadata ParseMetadata(System.IO.MemoryStream mem_stream)
    {
        Metadata metadata = new Metadata();

        mem_stream.Seek(0, SeekOrigin.Begin);
        StreamReader stream = new StreamReader(mem_stream);

        string entireText = stream.ReadToEnd();

        stream.Close();
        using (StringReader reader = new StringReader(entireText))
        {
            string currentText = reader.ReadLine();

            while (currentText != null)
            {
                var stringArr = currentText.Split(':');

                if (stringArr[0] == "Scale")
                {
                    Debug.Log(stringArr[1]);
                    metadata.Scale = float.Parse(stringArr[1], CultureInfo.InvariantCulture);
                }
                if (stringArr[0] == "Material")
                {
                    metadata.Material = stringArr[1];
                }
                if (stringArr[0] == "Name")
                {
                    metadata.Name = stringArr[1];
                }
                if (stringArr[0] == "Description")
                {
                    metadata.Description = stringArr[1];
                }
                if (stringArr[0] == "Signature")
                {
                    metadata.Signature = Regex.Unescape(stringArr[1]);
                }
                if (stringArr[0] == "Properties")
                {
                    metadata.Properties = new List<KeyValuePair<string, string>>();
                    var PropertiesArr = stringArr[1].Split(',');

                    foreach (var prop in PropertiesArr)
                    {
                        var propArr = prop.Split('=');
                        metadata.Properties.Add(new KeyValuePair<string, string>(propArr[0], propArr[1]));
                    }
                }

                if (stringArr[0] == "Images")
                {
                    metadata.Images = new List<string>();
                    var ImagesArr = stringArr[1].Split(',');
                    foreach (var image in ImagesArr)
                    {
                        metadata.Images.Add(image);
                    }
                }

                currentText = reader.ReadLine();
            }


        }
        return metadata;
    }

    /// <summary>
    /// Uses the Metadata struct to set UI elements to their values
    /// </summary>
    /// <param name="metadata">A Metadata struct to use to populate UI panels</param>
    private static void PopulatePanels(Metadata metadata, bool ValidSignature)
    {
        GameObject.Find("Name").GetComponent<Text>().text = metadata.Name;
        GameObject.Find("Desc").GetComponent<Text>().text = metadata.Description;

        if (ValidSignature)
        {
            GameObject.Find("Signature").GetComponent<Text>().text = "Valid signature! " + metadata.Signature;
            GameObject.Find("Signature").GetComponent<Text>().color = new Color(0,0.4f,0); // Dark green
        }
        else
        {
            GameObject.Find("Signature").GetComponent<Text>().text = "Invalid signature! " + metadata.Signature;
            GameObject.Find("Signature").GetComponent<Text>().color = new Color(0.4f, 0, 0); // Dark red
        }
        GameObject.Find("Properties").GetComponent<Text>().text = "";

        foreach (var prop in metadata.Properties)
        {
            GameObject.Find("Properties").GetComponent<Text>().text += prop.Key + ": " + prop.Value + "\n";
        }

        foreach (var image in metadata.Images)
        {
            /*if (loadFromScene == true)
            {
                GameObject ImageList = GameObject.Find("ImageList");

            GameObject image = (GameObject)Instantiate(Resources.Load("Prefabs/Image"), ImageList.transform);

            Image image_component = image.GetComponent<Image>();
            image_component.sprite = NewSprite;
            image_component.preserveAspect = true;

 
                image_component.sprite = (Sprite)Resources.Load(image.Split('.')[0], typeof(Sprite));
                image_component.preserveAspect = true;
            }*/
        }
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

        byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
        byte[] msgHash = new Sha3Keccack().CalculateHash(msgBytes);
        var signature = privKey.SignAndCalculateV(msgHash);

        Debug.LogFormat("Msg: {0}", msg);
        Debug.LogFormat("Msg hash: {0}", msgHash.ToHex());
        Debug.LogFormat("Signature: [v = {0}, r = {1}, s = {2}]", signature.V[0] - 27, signature.R.ToHex(), signature.S.ToHex());

        Debug.LogFormat("");

        System.IO.StreamWriter objWriter;
        objWriter = new System.IO.StreamWriter(filenameWithPath + "_Signature.txt");

        objWriter.WriteLine(msgHash.ToHex());
        objWriter.WriteLine(EthECDSASignature.CreateStringSignature(signature));
        objWriter.Close();
    }

    public static byte[] StringToByteArrayFastest(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new System.Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static int GetHexVal(char hex)
    {
        int val = (int)hex;
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    public static bool CheckSignature(byte[] _pubKey)
    {
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

        byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
        byte[] msgHash = new Sha3Keccack().CalculateHash(msgBytes);

        if (File.Exists(filenameWithPath + "_Signature.txt"))
        {
            System.IO.StreamReader objReader;
            objReader = new System.IO.StreamReader(filenameWithPath + "_Signature.txt");

            byte[] msgHashFromFile = StringToByteArrayFastest(objReader.ReadLine());

            EthECDSASignature signature = EthECDSASignatureFactory.ExtractECDSASignature(objReader.ReadLine());

            objReader.Close();

            var privKeyRecovered = EthECKey.RecoverFromSignature(signature, msgHash);
            var pubKeyRecovered = privKeyRecovered.GetPubKey();

            Debug.LogFormat("Recovered pubKey: {0}", pubKeyRecovered.ToHex().Substring(2));

            bool validSig = privKeyRecovered.Verify(msgHash, signature);
            bool pubKeyCorrect = pubKeyRecovered.SequenceEqual(_pubKey);

            Debug.LogFormat("Signature valid? {0}", validSig && pubKeyCorrect);
            
            return (validSig && pubKeyCorrect && (msgHashFromFile.SequenceEqual(msgHash))) ;
        }
        return false;

    }

    /// <summary>
    /// Loads, extracts the provided zip from its path. Calls the subsequent methods to display the asset correctly.
    /// </summary>
    /// <param name="filenameWithPath">The path of the file to load</param>
    /// <param name="x">The x coordinate for the position of the asset to load</param>
    /// <param name="y">The y coordinate for the position of the asset to load</param>
    public static void LoadZipFunc(string filenameWithPath, int x = 0, int y = 0) {

        currentBBAFilename = filenameWithPath;

        bool ValidSignature = CheckSignature(pubKey);

        AudioStop();

        using (ZipFile zip1 = ZipFile.Read(filenameWithPath))
        {
            // here, we extract every entry, but we could extract conditionally
            // based on entry name, size, date, checkbox status, etc.
            Metadata metadata = new Metadata();

            // Retrieve a Metadata stream from the .bba file.
            foreach (ZipEntry e in zip1)
            {
                if (e.FileName.Contains("Metadata.txt"))
                {
                    System.IO.MemoryStream tempS = new System.IO.MemoryStream();
                    e.Extract(tempS);

                    metadata = ParseMetadata(tempS);
                }
            }

            PopulatePanels(metadata, ValidSignature);

            showModel = false;
            showAudioControl = false;

            // Retrieve a Material stream from the .bba file.
            foreach (ZipEntry e in zip1)
            {
                if (e.FileName.Contains(".mat"))
                {
                    System.IO.MemoryStream tempS = new System.IO.MemoryStream();
                    e.Extract(tempS);

                    material = tempS;
                }
            }

            // Retrieve a .obj stream from the .bba file.
            foreach (ZipEntry e in zip1)
            {
                if (e.FileName.Contains(".obj"))
                {
                    showModel = true;

                    if (GameObject.Find("ModelPanel") == null && GameObject.Find("ModelPanel(Clone)") == null)
                    {
                        Instantiate(Resources.Load("Prefabs/ModelPanel"), GameObject.Find("Content").transform);
                    }

                    System.IO.MemoryStream tempS = new System.IO.MemoryStream();
                    e.Extract(tempS);

                    CreateModel.CreateModelFromOBJ(e.FileName, tempS, x, y, metadata);
                }
            }

            // Retrieve a .mp3 stream from the .bba file.
            foreach (ZipEntry e in zip1)
            {
                if (e.FileName.Contains(".mp3"))
                {
                    showAudioControl = true;
                    if (GameObject.Find("AudioPreview") == null && GameObject.Find("AudioPreview(Clone)") == null)
                    {
                        Instantiate(Resources.Load("Prefabs/AudioPreview"), GameObject.Find("Content").transform);
                    }

                    tempAudioStream = new System.IO.MemoryStream();
                    e.Extract(tempAudioStream);

                    tempAudioStream.Seek(0, SeekOrigin.Begin);

                    if (reader != null)
                    {
                        reader.Dispose();
                    }

                    var builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
                    
                    reader = new Mp3FileReader(tempAudioStream, builder);

                    totalAudioBytes = (int)reader.Length;
                    
                    waveOut = new WaveOut();
                    waveOut.Init(reader);

                    if (autoPlay == true)
                    {
                        AudioPlay();
                    }

                    // Add all the listeners for buttons and sliders events
                    GameObject.Find("ButtonPlay").GetComponent<Button>().onClick.AddListener(AudioPlay);
                    GameObject.Find("ButtonPause").GetComponent<Button>().onClick.AddListener(AudioPause);
                    GameObject.Find("ButtonStop").GetComponent<Button>().onClick.AddListener(AudioStop);
                    GameObject.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener(AudioSetVolume);
                    GameObject.Find("SliderPosition").GetComponent<Slider>().onValueChanged.AddListener(AudioSetPosition);
                }
            }

            if (showModel == false)
            {
                Destroy(GameObject.Find("ModelPanel"));
                Destroy(GameObject.Find("ModelPanel(Clone)"));
            }
            if (showAudioControl == false)
            {
                Destroy(GameObject.Find("AudioPreview"));
                Destroy(GameObject.Find("AudioPreview(Clone)"));
            }

            // Retrieve image streams from the .bba file and load them into the UI as sprites.
            if (loadFromScene == false)
            {
                foreach (ZipEntry e in zip1)
                {
                    if (e.FileName.Contains(".png"))
                    {
                        if (GameObject.Find("Image(Clone)") != null)
                        {
                            Destroy(GameObject.Find("Image(Clone)"));
                        }

                        byte[] data = null;
                        System.IO.MemoryStream tempS = new System.IO.MemoryStream();
                        e.Extract(tempS);

                        byte[] buffer = new byte[tempS.Length];

                        tempS.Seek(0, SeekOrigin.Begin); 
                        
                        int read;
                        while ((read = tempS.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            tempS.Write(buffer, 0, read);
                        }

                        data = tempS.ToArray();
                        Texture2D Tex2D = new Texture2D(10, 10);

                        Tex2D.LoadImage(data);

                        Sprite NewSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0, 0));

                        GameObject ImageList = GameObject.Find("ImageList");
                        GameObject Content = GameObject.Find("Content");

                        //GameObject image = (GameObject)Instantiate(Resources.Load("Prefabs/Image"), ImageList.transform);
                        GameObject image = (GameObject)Instantiate(Resources.Load("Prefabs/Image"), Content.transform);
                        
                        Image image_component = image.GetComponent<Image>();

                        image_component.sprite = NewSprite;

                        Image img = image_component;
                        Sprite sp = img.sprite;
                        Texture tx = sp.texture;
                        // Save the image aspect ratio to preserve it in the Update() method
                        imageAspectRatio = (float)tx.width / (float)tx.height;

                        // Set the image just below the "Preview" text
                        image_component.transform.SetSiblingIndex(1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Play the initialized audio track
    /// </summary>
    public static void AudioPlay()
    {
        Debug.Log("Inside AudioPlay()");
        Debug.Log(waveOut.ToString());
        Debug.Log(waveOut.PlaybackState);

        if (waveOut != null)
        {
            waveOut.Play();
        }
    }

    /// <summary>
    /// Play the audio track currently playing
    /// </summary>
    public static void AudioPause()
    {
        if (waveOut != null)
        {
            waveOut.Pause();
            waveOut.GetPosition();
        }
    }

    /// <summary>
    /// Allow for moving the audio position slider and preserving the image's ratio.
    /// </summary>
    public void Update()
    {

        GameObject image = GameObject.Find("Image(Clone)");
        if (image != null)
        {
            Image image_component = image.GetComponent<Image>();

            float w = image_component.GetComponent<RectTransform>().rect.width;
            image_component.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, w / imageAspectRatio);
        }

        if (GameObject.Find("SliderPosition") != null)
        {
            Slider slider = GameObject.Find("SliderPosition").GetComponent<Slider>();
            if (slider != null && reader != null && !GameObject.Find("SliderPosition").GetComponent<SliderDragEvent>().IsBeingDragged())
            {
                slider.value = (float)reader.Position / (float)totalAudioBytes;
            }
        }
    }

    /// <summary>
    /// Stop the audio currently playing and rewind it (wuth re-initialization)
    /// </summary>
    public static void AudioStop()
    {
        if (waveOut != null)
        {
            waveOut.Stop();

            tempAudioStream.Seek(0, SeekOrigin.Begin);

            var builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));

            var reader = new Mp3FileReader(tempAudioStream, builder);
            // play or process the file, e.g.:
            totalAudioBytes = (int)reader.Length;
            waveOut = new WaveOut();
            waveOut.Init(reader);
        }
    }

    /// <summary>
    /// Same as Audio stop but doesn't re-initialize the track.
    /// </summary>
    public static void AudioDestroy()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
            reader.Close();
            reader.Dispose();
        }
    }

    /// <summary>
    /// Change the audio's volume based on the corresponding slider.
    /// </summary>
    public static void AudioSetVolume(float volume)
    {
        if (waveOut != null)
        {
            waveOut.Volume = volume;
        }
    }

    /// <summary>
    /// Change the seeking audio position based on the correpsonding slider (if the user is dragging it)
    /// </summary>
    /// <param name="position"></param>
    public static void AudioSetPosition(float position)
    {
        if (waveOut != null && GameObject.Find("SliderPosition").GetComponent<SliderDragEvent>().IsBeingDragged())
        {
            waveOut.Stop();

            long positionBytes = (long)(position * totalAudioBytes);

            tempAudioStream.Seek(0, SeekOrigin.Begin);

            var builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));

            if (reader != null)
            {
                reader.Dispose();
            }

            reader = new Mp3FileReader(tempAudioStream, builder);

            reader.Seek(positionBytes, SeekOrigin.Begin);

            // Init and Play the audio

            waveOut = new WaveOut();
            waveOut.Init(reader);
        }
    }

    // If the file is choosen in the File explorer, call the LoadZipFunc function in the callback below
    public static void LoadZipFromFileBrowser()
    {
        string path = FileBrowser.OpenSingleFile("bba");
        Debug.LogFormat("Selected file: {0}", path);
    }

    public static void FileSelectedCallback(string path) {
        LoadZipFunc(path, 0, 0);
    }

	// Uncomment lines to use in the Unity Editor
    void Start () {

        signer = new SignatureTest();

        //var privKey = EthECKey.GenerateKey();
        privKey = new EthECKey("97ddae0f3a25b92268175400149d65d6887b9cefaf28ea2c078e05cdc15a3c0a");
        byte[] pubKeyCompressed = new ECKey(privKey.GetPrivateKeyAsBytes(), true).GetPubKey(true);
        Debug.LogFormat("Private key: {0}", privKey.GetPrivateKey().Substring(4));
        Debug.LogFormat("Public key: {0}", privKey.GetPubKey().ToHex().Substring(2));
        Debug.LogFormat("Public key (compressed): {0}", pubKeyCompressed.ToHex());

        pubKey = privKey.GetPubKey();



        //LoadZipFunc("Assets/Models/BBAs/Wallk2AndMusic.bba", 400, 50);

        /*LoadZipFunc("Assets/Models/BBAs/Diploma.bba", 400, 50);
        LoadZipFunc("Assets/Models/BBAs/GreenCow.bba", 400, 50);
        LoadZipFunc("Assets/Models/BBAs/Diploma.bba", 400, 50);
        LoadZipFunc("Assets/Models/BBAs/GreenCow.bba", 400, 50);
        LoadZipFunc("Assets/Models/BBAs/Diploma.bba", 400, 50);*/

        //LoadZipFunc("Assets/Models/BBAs/Wallk2.bba", 400, 50);

        // Used in file association to load the selected asset at startup

        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Length >= 2 && args[1].Contains(".bba"))
        {
            LoadZipFunc(args[1], 0, 0);
        }
    }

    private void OnDestroy()
    {
        AudioDestroy();
    }

}

