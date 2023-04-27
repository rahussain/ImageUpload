<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PictureUpload1._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section id="main" runat="server">
            <h1><strong>File upload</strong></h1>

            <div class="form-group">
                <label for="title">Title</label>
                <input type="text" name="title" id="litTitle" class="form-controll" required="required" runat="server" />
            </div>
            <div class="form-group file-area">
                <label for="images">Images <span>Your images should be at least 985x445 wide</span></label>
                <input type="file" name="imagesUpload" id="fupImageUpload" required="required" runat="server" accept="image/*"  />
                <div class="file-dummy">
                    <div class="success">Great, your file is selected.</div>
                    <div class="default">Please select Image</div>
                </div>
            </div>
            <div class="form-group">
                <label id="lblError"></label>
            </div>
            <div class="form-group">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" OnClientClick="CallBackFunction();" />
            </div>
        </section>
        <section id="msg" runat="server" visible="false">
            <div class="form-group">
                <label for="title"><asp:Literal ID="lblUploadResult" runat="server"></asp:Literal></label>
                
            </div>
            <div class="form-group">
                <input type="button" name="GoBackToHome" value="Go Back To Home" onclick="GoHome();" />
            </div>
        </section>
    </main>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#MainContent_fupImageUpload").bind('change', function () {
                var allowedFiles = [".jpg", ".png", ".gif", ".jpeg"];
                var fileUpload = $(this);
                var lblError = $('#lblError')
                var regex = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(" + allowedFiles.join('|') + ")$");
                if (!regex.test(fileUpload.val().toLowerCase())) {
                    lblError.html("<b>Only image files are allowed.</b>");
                    return false;
                }
                lblError.html('');
                return true;
            });

        });
        function GoHome() {
            window.location = "/";
        }
    </script>
</asp:Content>
