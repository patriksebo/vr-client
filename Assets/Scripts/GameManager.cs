using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;
using File = System.IO.File;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players;
    public GameObject ava1;
    public GameObject ava2;
    public GameObject ava3;
    public GameObject env1;
    public GameObject env2;
    public GameObject env3;
    public GameObject chatCanvas;
    public GameObject steamVr;
    public GameObject steamVrAr;
    public GameObject cameraRigRemote;
    
    private void Awake()
    {
        players = new Dictionary<int, PlayerManager>();
        
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, int _avatar, string _avatarUrl, Vector3 _position, Quaternion _rotation)
    {
        Debug.Log("Spawning player with id: " + _id + " username: " + _username + " avatarId: " + _avatar + " avatarUrl: " + _avatarUrl);
        GameObject _player;
        
        if (_avatar == 0)
        {
            // male
            _player = Instantiate(ava1, _position, _rotation);
        }
        else if(_avatar == 1)
        {
            // female
            _player = Instantiate(ava2, _position, _rotation);
        }
        else if(_avatar == 2)
        {
            // robot
            _player = Instantiate(ava3, _position, _rotation);
        }
        else if (_avatar == 3)
        {
            //custom
            Debug.Log("Using custom avatar.");
            string xname = CreateMD5(_avatarUrl);

            //StartCoroutine( DownloadAvatar(_avatarUrl, xname) );
            _player = InstantiateExternal(xname);
            if (_player == null)
            {
                Debug.Log("Cannot load custom avatar, loading avatar 2 instead");
                SpawnPlayer(_id,  _username,  2,  _avatarUrl,  _position, _rotation);
                return;
            }
        }
        else
        {
            _player = Instantiate(ava1, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
        
        if (_id == Client.instance.myId)
        {
            GameObject _chatCanvas;
            GameObject _steamVr;
            _chatCanvas = Instantiate(chatCanvas, _position, _rotation);
            _chatCanvas.transform.parent = _player.transform;

            if (UIManager.instance.arvr.value == 0)
            {
                // vr environment
                _steamVr = Instantiate(steamVr, _position, _rotation);
                _steamVr.transform.parent = _player.transform;
            }
            else
            {
                // ar environment
                _steamVr = Instantiate(steamVrAr, _position, _rotation);
                _steamVr.transform.parent = _player.transform;
            }
            
            GameObject head = _steamVr.transform.GetChild(2).gameObject; 
            GameObject rightHand = _steamVr.transform.GetChild(1).gameObject; 
            GameObject leftHand = _steamVr.transform.GetChild(0).gameObject; 
        
            Rig rig = _player.GetComponentInChildren<Rig>();
            rig.head.vrTarget = head.transform;
            rig.rightHand.vrTarget = rightHand.transform;
            rig.leftHand.vrTarget = leftHand.transform;

            if (_avatar == 0)
            {
                rig.useRig2 = true;
            }
            if (_avatar == 1)
                rig.useRig3 = true;
        
            _player.GetComponent<PlayerController>().head = head;
            _player.GetComponent<PlayerController>().right = rightHand;
            _player.GetComponent<PlayerController>().left = leftHand;
        }
        else
        {
            GameObject _steamVrRemote;
            _steamVrRemote = Instantiate(cameraRigRemote, _position, _rotation);
            _steamVrRemote.transform.parent = _player.transform;
            
            GameObject headRemote = _steamVrRemote.transform.GetChild(2).gameObject; 
            GameObject rightHandRemote = _steamVrRemote.transform.GetChild(1).gameObject; 
            GameObject leftHandRemote = _steamVrRemote.transform.GetChild(0).gameObject; 
            
            Rig rigRemote = _player.GetComponentInChildren<Rig>();
            rigRemote.head.vrTarget = headRemote.transform;
            rigRemote.rightHand.vrTarget = rightHandRemote.transform;
            rigRemote.leftHand.vrTarget = leftHandRemote.transform;
            
            _player.GetComponent<PlayerController>().head = headRemote;
            _player.GetComponent<PlayerController>().right = rightHandRemote;
            _player.GetComponent<PlayerController>().left = leftHandRemote;
        }
    }

    public void SpawnEnvironment(int _id)
    {
        if (UIManager.instance.arvr.value != 0)
            return;

        if (_id == 0)
        {
            Instantiate(env1, new Vector3(0,0,0), Quaternion.identity);
        }
        else if(_id == 1)
        {
            Instantiate(env2, new Vector3(0,0,0), Quaternion.identity);
        }
        else
        {
            Instantiate(env3, new Vector3(0,0,0), Quaternion.identity);
        }
    }

    public void BeginDownloadAvatar(string url)
    {
        StartCoroutine(DownloadAvatar(url));
    }
    
    public IEnumerator DownloadAvatar(string url)
    {
        string name = CreateMD5(url);
        var savePath = Path.Combine(Application.streamingAssetsPath + "/AssetBundles/", name);
        
        UnityWebRequest www = new UnityWebRequest(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            File.WriteAllBytes(savePath, results);
        }
        
        Debug.Log("Done downloading.");
    }
    
    GameObject InstantiateExternal(string name)
    {
        string objectNameToLoad = "Avatar";
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        filePath = System.IO.Path.Combine(filePath, name);
        var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
        AssetBundle asseBundle = assetBundleCreateRequest.assetBundle;
        try
        {
            AssetBundleRequest asset = asseBundle.LoadAssetAsync<GameObject>(objectNameToLoad);
            GameObject loadedAsset = asset.asset as GameObject;
            
            GameObject go = Instantiate(loadedAsset, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject model = go.transform.GetChild(0).gameObject;
            go.AddComponent<PlayerManager>();
            go.AddComponent<PlayerController>();
            
            Rig rig = model.AddComponent<Rig>();
            rig.External();
        
            GameObject constraints = model.transform.GetChild(2).gameObject;
            GameObject right = constraints.transform.GetChild(0).gameObject;
            GameObject left = constraints.transform.GetChild(1).gameObject;
            GameObject head = constraints.transform.GetChild(2).gameObject;
        
            GameObject righTarget =  right.transform.GetChild(0).gameObject;
            GameObject leftTarget =  left.transform.GetChild(0).gameObject;

            rig.head.rigTarget = head.transform;
            rig.leftHand.rigTarget = righTarget.transform;
            rig.rightHand.rigTarget = leftTarget.transform;
            rig.headConstraint = head.transform;

            return go;
        }
        catch
        {
            Debug.Log("Cannot load custom avatar.");
            return null;
        }
    }
    
    public static string CreateMD5(string input)
    {
        // Form hash
        System.Security.Cryptography.MD5 h = System.Security.Cryptography.MD5.Create();
        byte[] data = h.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
        // Create string representation
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < data.Length; ++i) {
            sb.Append(data[i].ToString("x2"));
        }
        return sb.ToString();
        
        /*
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = Crypto.ComputeMD5Hash(bytes);
        string t = "";
        for (int i = 0; i < hash.Length; i++)
            t += string.Format("{0:x2}", hash[i]);
        return t;
        */
        //return "test";
    }
}
