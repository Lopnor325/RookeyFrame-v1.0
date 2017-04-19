
var fileDivID = "fileList";
var imgPath = "images/";
var currentNode = null;
var pathID = "pathString";

var image =
    {
        Empty: imgPath + "empty.gif",
        Plus: imgPath + "plus_m.gif",
        Minus: imgPath + "minus_m.gif",
        Folder: imgPath + "folder.gif",
        Unknown: imgPath + "unknow.gif",
        Rename: imgPath + "rename.gif"
    };

var textFileType = new Array("txt", "aspx", "config", "js", "html", "htm", "xml", "cshtml", "master", "css", "asax", "", "", "", "", "", "");

function TreeNode()
{
    var self = this;
    var fileContainer = $(fileDivID);
    this.depth = 1;
    this.childrenNodes = null;
    this.folders = null;
    this.files = null;
    this.text = "";
    this.container = null;
    this.childArea = null;
    this.parent = null;
    this.path = "~/";

    this.Show = function()
    {
        for (var i = 0; i < this.depth - 1; i++)
        {
            this.container.appendChild(createImage(image.Empty));
        }

        this.imgPlus = createImage(image.Plus);
        this.imgPlus.onclick = self.clickNode;
        try { this.imgPlus.style.cursor = "pointer"; } catch (e) { this.imgPlus.style.cursor = "hand"; }
        this.container.appendChild(this.imgPlus);

        var imgFoder = createImage(image.Folder);
        imgFoder.onclick = self.clickNode;
        try { imgFoder.style.cursor = "pointer"; } catch (e) { imgFoder.style.cursor = "hand"; }
        this.container.appendChild(imgFoder);

        this.displayText = createNodeText(this.text);
        this.displayText.onclick = self.clickNode;
        try { this.displayText.style.cursor = "pointer"; } catch (e) { this.displayText.style.cursor = "hand"; }
        this.container.appendChild(this.displayText);


        this.childArea = createDiv();
        this.childArea.style.display = "none";
        this.container.appendChild(this.childArea);
    }

    this.clickNode = function()
    {
        self.ClearCurrentStatus();
        currentNode = self;
        self.SetCurrentStatus();
        self.CreateChildren();
    }

    this.ClearCurrentStatus = function()
    {
        if (currentNode != null)
        {
            currentNode.displayText.style.backgroundColor = "transparent";
            currentNode.displayText.style.color = "#000";
        }
    }

    this.GotoParentNode = function()
    {
        if (currentNode.parent != null)
        {
            currentNode.ClearCurrentStatus();
            currentNode = currentNode.parent
            currentNode.SetCurrentStatus();
            currentNode.childArea.innerHTML = "";
            currentNode.childArea.style.display = "none";
            currentNode.childrenNodes = null;
            currentNode.CreateChildren();
        }
    }

    this.SetCurrentStatus = function()
    {
        currentNode.displayText.style.backgroundColor = "#316acf";
        currentNode.displayText.style.color = "#fff";
    }

    this.Refersh = function()
    {
        if (currentNode.childrenNodes != null && currentNode.childrenNodes.length > 0)
        {
            currentNode.childArea.innerHTML = "";
            currentNode.childArea.style.display = "none";
        }

        currentNode.childrenNodes = null;
        currentNode.CreateChildren();
    }

    this.CreateChildren = function()
    {
        if (this.childrenNodes == null)
        {
            this.childrenNodes = new Array();
            var u = defaultURL + "?action=LIST&value1=" + encodeURIComponent(self.path) + "&value2=";
            var result = executeHttpRequest("GET", u, null);

            if (result == "ERROR")
            {
                this.GotoParentNode();
                return;
            }

            try
            {
                window.eval(result);
            }
            catch (e)
            {
                window.location.href = "/User/Login.html";
            }

            this.folders = GetList.Directory;
            this.files = GetList.File;

            for (var i = 0; i < this.folders.length; i++)
            {
                var node = new TreeNode();
                node.depth = this.depth + 1;
                node.text = this.folders[i].Name;
                node.container = createDiv();
                this.childArea.appendChild(node.container);
                node.parent = this;
                node.path = node.parent.path + this.folders[i].Name + "/";
                this.childrenNodes.push(node);
            }

            for (var i = 0; i < this.childrenNodes.length; i++)
            {
                this.childrenNodes[i].Show();
            }
        }

        fileContainer.innerHTML = "";

        for (var i = 0; i < this.folders.length; i++)
        {
            fileContainer.appendChild(createFileView(this.folders[i].Name, true, null, this.folders[i].LastModify));
        }

        for (var i = 0; i < this.files.length; i++)
        {
            fileContainer.appendChild(createFileView(this.files[i].Name, false, this.files[i].Size, this.files[i].LastModify));
        }

        //伸展折叠节点
        if (self.childArea.style.display == "")
        {
            self.childArea.style.display = "none";

            if (self.childrenNodes != null && self.childrenNodes.length < 1)
            {
                self.imgPlus.src = image.Empty;
            }
            else
            {
                self.imgPlus.src = image.Plus;
            }
        }
        else
        {
            self.childArea.style.display = "";

            if (self.childrenNodes != null && self.childrenNodes.length < 1)
            {
                self.imgPlus.src = image.Empty;
            }
            else
            {
                self.imgPlus.src = image.Minus;
            }
        }

        //更新当前路径
        $(pathID).innerHTML = self.path;
    }
}

var createImage = function(imgSource)
{
    var img = createImg();
    img.src = imgSource;
    img.align = "absmiddle";

    return img;
}

var createNodeText = function(text)
{
    var button = createBtn();
    button.style.border = "none";
    button.style.fontSize = "12px";
    button.style.backgroundColor = "transparent";
    button.value = text;
    button.style.textAlign = "left";
    button.style.paddingLeft = "2px";

    return button;
}

var createFileView = function(fileName, isFolder, fileSize, modifyDate)
{
    var fileItem = createDiv("div");
    fileItem.className = "fileItem";

    var fileChk = document.createElement("input");
    fileChk.type = "checkbox";
    fileChk.className = "chkColumn";
    fileChk.title = fileName;
    fileItem.appendChild(fileChk);

    var fileType = createDiv();
    fileType.className = "fileType";

    var imgSource = fileName.substring(fileName.lastIndexOf(".") + 1);
    var fileImg;

    if (isFolder)
    {
        fileImg = createImage(imgPath + "dir.gif");
        fileImg.alt = "文件夹";
    }
    else
    {
        fileImg = createImage(imgPath + imgSource + ".gif");
        fileImg.alt = imgSource + "文件";
    }

    fileImg.onerror = function()
    {
        try
        {
            this.src = image.Unknown;
            this.alt = "未知类型文件";
        }
        catch (e)
        { }
    }

    fileType.appendChild(fileImg);
    fileItem.appendChild(fileType);

    var fileListName = createDiv();
    fileListName.className = "fileName";
    fileItem.appendChild(fileListName);

    var fileNameSpan = createSpan();
    fileNameSpan.innerHTML = fileName;
    try { fileNameSpan.style.cursor = "pointer"; } catch (e) { fileNameSpan.style.cursor = "hand"; }
    fileListName.appendChild(fileNameSpan);

    fileNameSpan.onmouseover = function()
    {
        this.style.color = "#f00";
        this.style.textDecoration = "underline";
    }

    fileNameSpan.onmouseout = fileNameSpan.onmouseout = function()
    {
        this.style.color = "#000";
        this.style.textDecoration = "none";
    }

    fileNameSpan.onclick = function()
    {
        if (isFolder)
        {
            for (var i = 0; i < currentNode.childrenNodes.length; i++)
            {
                if (currentNode.childrenNodes[i].text == this.innerHTML)
                {

                    clickDirectory(currentNode.childrenNodes[i]);
                    break;
                }
            }
        }
        else
        {
            clickFile(this.innerHTML);
        }
    }

    if (!isFolder)
    {
        for (var i = 0; i < textFileType.length; i++)
        {
            if (textFileType[i] == imgSource.toLowerCase())
            {
                var editFileSpan = createSpan();
                editFileSpan.innerHTML = "[编辑]";
                editFileSpan.style.color = "#ccc";
                editFileSpan.style.paddingLeft = "5px";
                editFileSpan.title = fileName;
                try { editFileSpan.style.cursor = "pointer"; } catch (e) { editFileSpan.style.cursor = "hand"; }
                fileListName.appendChild(editFileSpan);

                editFileSpan.onmouseover = function()
                {
                    this.style.color = "#f00";
                    this.style.textDecoration = "underline";
                }

                editFileSpan.onmouseout = function()
                {
                    this.style.color = "#ccc";
                    this.style.textDecoration = "none";
                }

                editFileSpan.onclick = function()
                {
                    editFile(this.title);
                }

                break;
            }
        }
    }

    var fileListSize = createDiv();
    fileListSize.className = "fileSize";

    if (!isFolder)
    {
        fileListSize.innerHTML = fileSize;
    }

    fileItem.appendChild(fileListSize);

    var fileDate = createDiv();
    fileDate.className = "lastUpdate";
    fileDate.innerHTML = modifyDate;
    fileItem.appendChild(fileDate);

    var fileRename = createDiv();
    fileRename.className = "rename";

    var fileRenameImg = createImage(image.Rename);
    fileRenameImg.title = fileName;
    try { fileRenameImg.style.cursor = "pointer"; } catch (e) { fileRenameImg.style.cursor = "hand"; }

    fileRename.appendChild(fileRenameImg);
    fileItem.appendChild(fileRename);

    fileRenameImg.onclick = function()
    {
        renameFile(this.title);
    }

    var clear = createDiv();
    clear.style.clear = "both";
    fileItem.appendChild(clear);

    return fileItem;
}

var clickDirectory = function(cNode)
{
    currentNode.ClearCurrentStatus();
    currentNode.childArea.style.display = "";
    currentNode = cNode;
    currentNode.SetCurrentStatus();
    currentNode.Refersh();
}

var clickFile = function(fName)
{
}

var editFile = function(fName)
{
}

var renameFile = function(fName)
{
}