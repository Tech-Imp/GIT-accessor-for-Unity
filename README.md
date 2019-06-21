# GIT-accessor-for-Unity
This accessory class allows the recursive retrieval of files from GitHub for a unity project using the API
Usage:



gitAccessor myFiles = new gitAccessor(); //Create a new instance of the class

***Setters:***
-------------------------------------
--Common--

myFiles.setProj("[OWNER/REPO-NAME]"); //Sets the git repository you will be accessing

myFiles.setFiles(".txt"); //Select the files by setting their ending suffix in this case it will grab all .txt files
myFiles.setFiles(".cs");  //This adds c# files to the list of valid download targets 

myFiles.setStartDir("[/subfolder 1/subfolder 2/target folder]"); //if you need only a specific subfolder you can set that with the starting directory function (default is the root of the repository)

myFiles.setIgnoreFolders("Bad Folder"); //this excludes the folder "Bad Folder" from any recursive search

--Less common--

myFiles.setAPIBase("[API URL]"); //In the event that github.com isnt the keeper of this repository (Default is https://api.github.com/repos)

myFiles.setDownloadLoc("[Base folder of download]"); //the base location for download (default is Application.dataPath)

**Getting data back from this class**
----------------------------------------

myFiles.gatherInfo(); //This will write the download links to the console instead of to file

myFiles.gatherInfo("", true); //this will write links contents to file, use setDownloadLoc() to change the default location

myFiles.downloadParticular("[Github dowload URL]", "[Specified download location]"); //Usually not called by itself but if you get the download_url from the API you can pass it here to specifically only grab that file.



