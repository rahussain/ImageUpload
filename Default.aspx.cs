using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PictureUpload1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        public System.Drawing.Image ResizeImageFromFile(String OriginalFileLocation, int heigth, int width, Boolean keepAspectRatio, Boolean getCenter)
        {
            int newheigth = heigth;
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFileLocation);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (keepAspectRatio || getCenter)
            {
                int bmpY = 0;
                double resize = (double)FullsizeImage.Width / (double)width;//get the resize vector
                if (getCenter)
                {
                    bmpY = (int)((FullsizeImage.Height - (heigth * resize)) / 2);// gives the Y value of the part that will be cut off, to show only the part in the center
                    Rectangle section = new Rectangle(new Point(0, bmpY), new Size(FullsizeImage.Width, (int)(heigth * resize)));// create the section to cut of the original image
                                                                                                                                 //System.Console.WriteLine("the section that will be cut off: " + section.Size.ToString() + " the Y value is minimized by: " + bmpY);
                    Bitmap orImg = new Bitmap((Bitmap)FullsizeImage);//for the correct effect convert image to bitmap.
                    FullsizeImage.Dispose();//clear the original image
                    using (Bitmap tempImg = new Bitmap(section.Width, section.Height))
                    {
                        Graphics cutImg = Graphics.FromImage(tempImg);//              set the file to save the new image to.
                        cutImg.DrawImage(orImg, 0, 0, section, GraphicsUnit.Pixel);// cut the image and save it to tempImg
                        FullsizeImage = tempImg;//save the tempImg as FullsizeImage for resizing later
                        orImg.Dispose();
                        cutImg.Dispose();
                        return FullsizeImage.GetThumbnailImage(width, heigth, null, IntPtr.Zero);
                    }
                }
                else newheigth = (int)(FullsizeImage.Height / resize);//  set the new heigth of the current image
            }//return the image resized to the given heigth and width
            return FullsizeImage.GetThumbnailImage(width, newheigth, null, IntPtr.Zero);
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (fupImageUpload.PostedFile != null)
                {
                    string strFileName;
                    string strFilePath;
                    string strFolder = Server.MapPath("./") + ConfigurationManager.AppSettings.Get("ImageFolderName").ToString();
                    
                    // Retrieve the name of the file that is posted.
                    strFileName =  System.DateTime.Now.Ticks + fupImageUpload.PostedFile.FileName;
                    strFileName = Path.GetFileName(strFileName);

                    if (fupImageUpload.Value != "")
                    {

                        System.IO.Stream stream = fupImageUpload.PostedFile.InputStream;
                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                        int imgHeight = image.Height;
                        int imgWidth = image.Width;

                        // Create the folder if it does not exist.
                        if (!Directory.Exists(strFolder))
                        {
                            Directory.CreateDirectory(strFolder);
                        }
                        // Save the uploaded file to the server.
                        strFilePath = strFolder + "\\" + strFileName;
                        if (File.Exists(strFilePath))
                        {
                            main.Visible = false;
                            msg.Visible = true;
                            lblUploadResult.Text = strFileName + " already exists on the server!";
                        }
                        else
                        {
                            main.Visible = false;
                            msg.Visible = true;

                            fupImageUpload.PostedFile.SaveAs(strFilePath);
                            string fileExtension = Path.GetExtension(strFilePath);
                            string ResizeOption = ConfigurationManager.AppSettings.Get("ResizeImage").ToString();
                            if (imgWidth > 985)
                            {
                                if(ResizeOption == "True")
                                { 
                                    System.Drawing.Image resizedImage = ResizeImageFromFile(strFilePath, 445, 985, true, true);
                                    resizedImage.Save(strFilePath);
                                }                            
                                
                            }
                            if (ConfigurationManager.AppSettings.Get("SaveInDB").ToString() == "True")
                            {
                                //AddImageinDB(litTitle.Value, strFileName);
                                PictureUpload1.App_Start.Pecosbaseball objPBB = new App_Start.Pecosbaseball();
                                objPBB.Title = txtTitle.Value;
                                objPBB.Media_820_360 = txtImageName.Value;
                                objPBB.Media_Image = strFileName;
                                AddImageinDB(objPBB);                                
                            }
                            
                            lblUploadResult.Text = strFileName + " has been successfully uploaded.";
                        }
                    }
                    else
                    {
                        lblUploadResult.Text = "Click 'Browse' to select the file to upload.";
                    }
                    
                }
            }
            catch (Exception)
            {
                main.Visible = false;
                msg.Visible = true;
                lblUploadResult.Text = "An error occured in file upload.";
            }
        }
        public void AddImageinDB(PictureUpload1.App_Start.Pecosbaseball objPBB)
        {
            try
            {
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection();
                sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "sp_AddPecosbaseball";
                cmd.Parameters.Add("@content", System.Data.SqlDbType.VarChar, 7999).Value = objPBB.Content;
                cmd.Parameters.Add("@title", System.Data.SqlDbType.VarChar, 1000).Value = objPBB.Title;
                cmd.Parameters.Add("@active", System.Data.SqlDbType.Bit).Value = objPBB.Active;
                cmd.Parameters.Add("@datetime", System.Data.SqlDbType.DateTime).Value = DateTime.Now.ToString();
                cmd.Parameters.Add("@endtime", System.Data.SqlDbType.DateTime).Value = null;
                cmd.Parameters.Add("@team", System.Data.SqlDbType.Int).Value = objPBB.Team;
                cmd.Parameters.Add("@media_820_360", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Media_820_360;
                cmd.Parameters.Add("@media_image", System.Data.SqlDbType.VarChar, 2000).Value = objPBB.Media_Image;
                cmd.Parameters.Add("@media_image2", System.Data.SqlDbType.VarChar, 2000).Value = objPBB.Media_Image2;
                cmd.Parameters.Add("@media_image3", System.Data.SqlDbType.VarChar, 2000).Value = objPBB.Media_Image3;
                cmd.Parameters.Add("@media_image4", System.Data.SqlDbType.VarChar, 2000).Value = objPBB.Media_Image4;
                cmd.Parameters.Add("@media_image5", System.Data.SqlDbType.VarChar, 2000).Value = objPBB.Media_Image5;
                cmd.Parameters.Add("@cal_events_id", System.Data.SqlDbType.Int).Value = objPBB.Cal_Events_Id;
                cmd.Parameters.Add("@caption", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Caption;
                cmd.Parameters.Add("@caption2", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Caption2;
                cmd.Parameters.Add("@caption3", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Caption3;
                cmd.Parameters.Add("@caption4", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Caption4;
                cmd.Parameters.Add("@caption5", System.Data.SqlDbType.VarChar, 301).Value = objPBB.Caption5;
                cmd.Connection = sqlConnection;

                sqlConnection.Open();
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}