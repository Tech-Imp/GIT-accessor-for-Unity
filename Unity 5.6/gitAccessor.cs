using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class gitAccessor : MonoBehaviour
{
    private string baseProj = "";
    private string APIpreamble ="https://api.github.com/repos";
    private string startLocation ="/";
    private string downloadLocationBase = Application.dataPath;
    private List<string> fileTypesWanted;
    private List<string> avoidFolders;
    //Unity specific function stubs
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //constructor
    public gitAccessor(){
        this.fileTypesWanted=new List<string>();
        this.avoidFolders=new List<string>();
    }
    //Setters
    public void setDownloadLoc(string dlPath){  //Set the overall download path
        this.downloadLocationBase=dlPath;
    }
    public void setProj(string baseProjUrl){
        this.baseProj=baseProjUrl;
    } 
    public void setAPIBase(string APIurl){
        this.APIpreamble=APIurl;
    }
    public void setStartDir (string newLoc){    //Set the starting directory for the search on github
        this.startLocation=newLoc.Replace(' ', "%20");
    }
    public void setFiles(string fileEndingTypes){   //individually add the file types wanted (ie: .doc .txt etc)
        fileTypesWanted.Add(fileEndingTypes);
    }
    public void setIgnoreFolders(string ignoreFolder){  //individually add folders that should be skipped on recursion
        avoidFolders.Add(ignoreFolder.Replace(' ', "%20"));
    }
    //--------------------------------------------------------------------------------------------------
    public IEnumerator  gatherInfo(string startUrl="", bool download=false){
        if(baseProj==""){Console.WriteLine("No project was set. Call setProj() first to set what git project you wish to gather info on");
        Console.WriteLine("Format expected: /OWNER/REPOSITORY");
        yield break;}
        if(startUrl==""){startUrl = APIpreamble+baseProj+"/contents"+startLocation;} //set URL in event it isnt already
        //Commented out code is for a newer version of unity than 5.6, other attribute names are different
        //using ( UnityWebRequest request = UnityWebRequest.Get(startUrl)){
        //yield return request.SendWebRequest();
            UnityWebRequest request = new UnityWebRequest(startUrl);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.Send();
            if(request.isError){
                Console.WriteLine("There was an error with the request at:"+startUrl);
                Console.WriteLine("Error:" + request.error);
            }
            else{
                if(request.isDone){
                    string json=toGitBlob(request.downloadHandler.text);
                    gitBlob currentBlob = JsonUtility.FromJson(json);
                    foreach (gitObj gitEntity in currentBlob.gitData){
                        bool skipItem=false;
                        if(gitEntity.type=="dir"){
                            foreach(string folders in avoidFolders){
                                if(gitEntity.name.startsWith(folders)){
                                    skipItem=true;
                                    break;
                                }
                            }
                            if(skipItem){ //ignore unwanted directories
                                continue;
                            }
                            else{
                                gatherInfo(gitEntity.url, download);            //Dig deeper, recursively
                            }
                        }
                        else if(gitEntity.type=="file"){
                            foreach(string fileEnding in fileTypesWanted){
                                if(gitEntity.name.EndsWith(fileEnding)){
                                    if(download){
                                        downloadParticular(gitEntity.download_url, downloadLocationBase+gitEntity.name);
                                    }
                                    else{
                                        Console.WriteLine(gitEntity.download_url);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        //}

    }
    //requires download URL and the name of the file (or include full file location)
    public IEnumerator downloadParticular(string entityURL="", string fileLoc=""){
        if(entityURL!="" && fileLoc!=""){
            //Commented out code is for later version than Unity 5.6, other attribute names are different
            //using ( UnityWebRequest request = UnityWebRequest.Get(entityURL)){
            //yield return request.SendWebRequest();
            UnityWebRequest request = new UnityWebRequest(entityURL);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.Send();
            if(request.isError){
                Console.WriteLine("There was an error with the request at:"+entityURL);
                Console.WriteLine("Error:" + request.error);
            }
            else{
                if(request.isDone){
                    File.WriteAllText(fileLoc, request.downloadHandler.text);
                }
            }
            //}
        }
    }
//----------------------------------------------------------------------------------------------------------
//-------------------DATA structure and manipulation---------------------------------------------------------
//----------------------------------------------------------------------------------------------------------
    //adjust the JSON accordingly
    public string toGitBlob(string json){
        return "{ \"gitData\":"+json+"}";
    }
    [Serializable]
    public class gitBlob{
        public List<gitObj> gitData;
    }
    //Structure of the JSON coming back from Github API
    [Serializable]
    public class Links
    {
        public string self ;
        public string git ;
        public string html ;
    }
    [Serializable]
    public class gitObj
    {
        public string name ;
        public string path ;
        public string sha ;
        public int size ;
        public string url ;
        public string html_url ;
        public string git_url ;
        public string download_url ;
        public string type ;
        public Links _links ;
    }
}
