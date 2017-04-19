
var Dialog = function()
{
    var me = this;
    this.MaskImage = null;
    this.Content = null;
    this.Text = null;
    this.Container = null;
    this.ImagePath = "images/dialog/";
    this.posX = 0;
    this.posY = 0;
    this.IsDown = false;
    this.Width = 300;
    this.Height = 0;
    this.DocVisibleWidth = 0;
    this.DocVisibleHeight = 0;
    this.DocMaxWidth = 0;
    this.DocMaxHeight = 0;
    this.ImgZIndex = 101;
    this.DialogZIndex = 102;
    this.ButtonOK = null;
    this.ButtonCancel = null;
    this.ButtonRetry = null;

    this.Icon =
    {
        Close_Normal: this.ImagePath + "close_normal.png",
        Close_Higthlight: this.ImagePath + "close_highlight.png",
        Mask_Image: this.ImagePath + "mask.png",
        Dialog_Icon: this.ImagePath + "icon.png"
    };

    this.Remove = function()
    {
        document.body.removeChild(this.Container);
        document.body.removeChild(this.MaskImage);
    }

    this.OK = function()
    {
        me.Close();
    }

    this.Retry = function()
    {

    }

    this.Close = function()
    {
        this.Hide();
    }

    this.Hide = function()
    {
        this.Container.style.display = "none";
        this.MaskImage.style.display = "none";
    }

    this.MaskImage = document.createElement("img");
    this.MaskImage.style.position = "absolute";
    this.MaskImage.style.left = 0;
    this.MaskImage.style.top = 0;
    this.MaskImage.src = this.Icon.Mask_Image;
    document.body.appendChild(this.MaskImage);


    this.Container = document.createElement("div");
    this.Container.style.position = "absolute";
    document.body.appendChild(this.Container);

    this.borderLine = createDiv();
    this.borderLine.className = "Dialog_border";
    this.Container.appendChild(this.borderLine);

    this.titleBar = createDiv();
    this.titleBar.className = "Dialog_titleBar";
    this.borderLine.appendChild(this.titleBar);

    var dialogIco = createImg();
    dialogIco.className = "Dialog_ico";
    dialogIco.src = this.Icon.Dialog_Icon;
    this.titleBar.appendChild(dialogIco);

    this.titleText = createDiv();
    this.titleText.className = "Dialog_titleText";
    this.titleBar.appendChild(this.titleText);

    this.titleCloseButton = createImg();
    this.titleCloseButton.className = "Dialog_titleCloseButton";
    this.titleCloseButton.title = "关闭";
    this.titleCloseButton.src = this.Icon.Close_Normal;
    this.titleBar.appendChild(this.titleCloseButton);

    this.dialogContent = createDiv();
    this.dialogContent.className = "Dialog_content";
    this.borderLine.appendChild(this.dialogContent);

    this.buttonPanel = createDiv();
    this.buttonPanel.className = "Dialog_buttonPanel";
    this.borderLine.appendChild(this.buttonPanel);

    this.ButtonOK = createBtn();
    this.ButtonOK.value = "确 定";
    this.ButtonOK.className = "Dialog_commandButton";
    this.buttonPanel.appendChild(this.ButtonOK);

    this.ButtonRetry = createBtn();
    this.ButtonRetry.value = "重 试";
    this.ButtonRetry.className = "Dialog_commandButton";
    this.ButtonRetry.style.diaplay = "none";
    this.buttonPanel.appendChild(this.ButtonRetry);

    this.ButtonCancel = createBtn();
    this.ButtonCancel.value = "取 消";
    this.ButtonCancel.className = "Dialog_commandButton";
    this.buttonPanel.appendChild(this.ButtonCancel);

    this.Hide();

    this.GetSize = function()
    {
        var cmpMd = document.compatMode == 'CSS1Compat';

        this.MaskImage.style.zIndex = this.ImgZIndex;

        this.dialogContent.innerHTML = this.Content;
        this.Container.style.zIndex = this.DialogZIndex;
        this.Container.style.width = this.Width + "px";
        this.titleText.innerHTML = this.Text;

        this.Height = Math.max(this.Container.offsetHeight, this.Container.clientHeight);

        if (cmpMd)
        {
            this.DocVisibleWidth = document.documentElement.clientWidth;
            this.DocVisibleHeight = document.documentElement.clientHeight;
        }
        else
        {
            this.DocVisibleWidth = document.body.clientWidth;
            this.DocVisibleHeight = document.body.clientHeight;
        }

        if (this.DocVisibleWidth < 10 || this.DocVisibleHeight < 10)
        {
            this.DocVisibleWidth = document.body.clientWidth;
            this.DocVisibleHeight = document.body.clientHeight;
        }

        if (cmpMd)
        {
            this.DocMaxWidth = document.documentElement.scrollWidth; //Math.max(document.documentElement.clientWidth, document.documentElement.scrollWidth);
            this.DocMaxHeight = document.documentElement.scrollHeight;
        }
        else
        {
            this.DocMaxWidth = document.body.scrollWidth;
            this.DocMaxHeight = document.body.scrollHeight;
        }
    }

    this.SetProperty = function()
    {
        this.GetSize();
        this.MaskImage.style.width = this.DocMaxWidth + "px";
        this.MaskImage.style.height = this.DocMaxHeight + "px";

        this.Container.style.left = (this.DocVisibleWidth - this.Width) / 2 + "px";

        if (this.DocVisibleWidth < this.Width)
        {
            this.Container.style.left = "0px";
        }

        this.Container.style.top = (this.DocVisibleHeight - this.Height) / 2 + "px";

        if (this.DocVisibleHeight < this.Height)
        {
            this.Container.style.top = "0px";
        }
    }

    this.RegisteEvent = function()
    {
        this.titleCloseButton.onmouseover = function()
        {
            this.src = me.Icon.Close_Higthlight;
        }

        this.titleCloseButton.onmouseout = function()
        {
            this.src = me.Icon.Close_Normal;
        }

        this.titleCloseButton.onclick = function()
        {
            me.Close();
        }

        this.ButtonOK.onclick = function()
        {
            me.OK();
        }

        this.ButtonCancel.onclick = function()
        {
            me.Close();
        }

        this.ButtonRetry.onclick = function()
        {
            me.Retry();
        }

        this.titleBar.onmousedown = function(e)
        {
            if (e == null) e = window.event;

            me.posX = e.clientX - parseInt(me.Container.style.left);
            me.posY = e.clientY - parseInt(me.Container.style.top);

            me.IsDown = true;
            return false;
        }

        this.titleBar.onselectstart = this.titleBar.ondrag = function()
        {
            return false;
        }

        this.ReleaseCapture = function()
        {
            me.IsDown = false;
        }

        this.MoveDialog = function(e)
        {
            if (me.IsDown)
            {
                if (!e) e = window.event;

                me.Container.style.left = (e.clientX - me.posX) + "px";
                me.Container.style.top = (e.clientY - me.posY) + "px";

                if (parseInt(me.Container.style.top) < 2)
                {
                    me.Container.style.top = "2px";
                }

                if (parseInt(me.Container.style.left) < 2)
                {
                    me.Container.style.left = "2px";
                }

                if (e.clientY < 2 || e.clientX < 2)
                {
                    me.IsDown = false;
                }

                if (parseInt(me.Container.style.left) > me.DocMaxWidth - me.Width - 2)
                {
                    me.Container.style.left = me.DocMaxWidth - me.Width - 2 + "px";
                }

                if (parseInt(me.Container.style.top) > me.DocMaxHeight - me.Height - 2)
                {
                    me.Container.style.top = me.DocMaxHeight - me.Height - 2 + "px";
                }
            }
        }

        if (document.attachEvent)
        {
            document.attachEvent("onmousemove", this.MoveDialog);
            document.attachEvent("onmouseup", this.ReleaseCapture);
        }
        else if (document.addEventListener)
        {
            document.addEventListener("mousemove", this.MoveDialog, false);
            document.addEventListener("mouseup", this.ReleaseCapture, false);
        }
    }

    this.Show = function(btnCount)
    {
        switch (btnCount)
        {
            case 1:
                this.ButtonRetry.style.display = "none";
                this.ButtonCancel.style.display = "none";
                break;

            case 2:
                this.ButtonRetry.style.display = "none";
                this.ButtonCancel.style.display = "";
                break;

            case 3:
                this.ButtonRetry.style.display = "";
                this.ButtonCancel.style.display = "";
                break;

            default:
                this.ButtonRetry.style.display = "none";
                this.ButtonCancel.style.display = "";
                break;
        }

        this.MaskImage.style.display = "";
        this.Container.style.display = "";

        this.SetProperty();
        this.RegisteEvent();
    }
}