﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.Hosting;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class TalentModel : BaseModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public ListItemModel Gender { get; set; }

        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        [Display(Name = "Nationality")]
        public ListItemModel Country { get; set; }

        public ListItemModel Representative { get; set; }

        public string RepDisplayName { get; set; }

        public string Height { get; set; }

        [Display(Name = "Eye Color")]
        public ListItemModel EyeColor { get; set; }

        [Display(Name = "Hair Color")]
        public ListItemModel HairColor { get; set; }

        public ListItemModel Ethnicity { get; set; }

        [Display(Name = "Shoe Size")]
        public string ShoeSize { get; set; }

        [Display(Name = "Waist Size")]
        public string WaistSize { get; set; }

        [Display(Name = "Shirt Size")]
        public string ShirtSize { get; set; }

        public string Instagram { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Notes { get; set; }

        public string ProfilePicture { get; set; }

        public string BookPictures { get; set; }

        public string Thumbnails { get; set; }

        public bool ShowCheck { get; set; }

        public bool Checked { get; set; }

        public string CheckClass
        {
            get
            {
                if (Checked) return "glyphicon-check";
                else return "glyphicon-unchecked";
            }
        }

        public TalentModel() { }

        public TalentModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["Id"]);
            this.FirstName = Convert.ToString(row["FirstName"]);
            this.LastName = Convert.ToString(row["LastName"]);
            this.Email = Convert.ToString(row["Email"]);
            this.Phone = Convert.ToString(row["Phone"]);
            this.Height = Convert.ToString(row["Height"]);
            this.Instagram = Convert.ToString(row["Instagram"]);
            this.ProfilePicture = Convert.ToString(row["ProfilePicture"]);
            this.ShoeSize = Convert.ToString(row["ShoeSize"]);
            this.WaistSize = Convert.ToString(row["WaistSize"]);
            this.ShirtSize = Convert.ToString(row["ShirtSize"]);
            this.Notes = Convert.ToString(row["Notes"]);

            this.Country = new ListItemModel(Convert.ToString(row["Nationality"]));
            this.Representative = new ListItemModel(Convert.ToString(row["Representative"]));
            this.Ethnicity = new ListItemModel(Convert.ToString(row["Ethnicity"]));
            this.HairColor = new ListItemModel(Convert.ToString(row["HairColor"]));
            this.EyeColor = new ListItemModel(Convert.ToString(row["EyeColor"]));
            this.Gender = new ListItemModel(Convert.ToString(row["Gender"]));

            if (row["DateOfBirth"] != DBNull.Value)
                this.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
        }

        public bool Add()
        {
            string sql = @"INSERT INTO Talent(FirstName,LastName,Gender,DateOfBirth,Nationality,Representative,Height,EyeColor
                                           ,HairColor,Ethnicity,ShoeSize,WaistSize,ShirtSize,Instagram,Phone,Email,Notes)
                        VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @Nationality, @Representative, @Height, @EyeColor
                              , @HairColor, @Ethnicity, @ShoeSize, @WaistSize, @ShirtSize, @Instagram, @Phone, @Email, @Notes)";

            var pl = new List<MySqlParameter>();
            this.ID = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, GetParams()));

            if (this.ID > 0)
            {
                return SavePhotos();
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            string sql = @"UPDATE Talent
                           SET FirstName = @FirstName
                              ,LastName = @LastName
                              ,Gender = @Gender
                              ,DateOfBirth = @DateOfBirth
                              ,Nationality = @Nationality
                              ,Representative = @Representative
                              ,Height = @Height
                              ,EyeColor = @EyeColor
                              ,HairColor = @HairColor
                              ,Ethnicity = @Ethnicity
                              ,ShoeSize = @ShoeSize
                              ,WaistSize = @WaistSize
                              ,ShirtSize = @ShirtSize
                              ,Instagram = @Instagram
                              ,Phone = @Phone
                              ,Email = @Email
                              ,Notes = @Notes
                         WHERE ID = @ID";

            
            int r = DatabaseHelper.ExecuteNonQuery(sql, GetParams());

            if (r >= 1)
            {
                return SavePhotos();
            }
            else
            {
                return false;
            }
        }

        private bool SavePhotos()
        {
            SaveProfilePicture();

            string sql = @"DELETE FROM TalentPhotos WHERE TalentID = @TalentID";

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@TalentID", this.ID));
            DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (!string.IsNullOrEmpty(this.BookPictures))
            {
                var photos = this.BookPictures.Split(',');

                foreach (var url in photos)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        MovePhotoToTalentFolder(url, true);
                        
                        sql = @"INSERT TalentPhotos (TalentID, PhotoType, Photo, Thumbnail)
                        VALUES (@TalentID, @PhotoType, @Photo, @Thumbnail)";

                        pl.Clear();
                        pl.Add(DatabaseHelper.CreateSqlParameter("@TalentID", this.ID));
                        pl.Add(DatabaseHelper.CreateSqlParameter("@PhotoType", "BookPhoto"));
                        pl.Add(DatabaseHelper.CreateSqlParameter("@Photo", GetPhotoUrl(url)));
                        pl.Add(DatabaseHelper.CreateSqlParameter("@Thumbnail", GetThumbUrl(url)));
                        DatabaseHelper.ExecuteNonQuery(sql, pl);
                    }
                }
            }

            return true;
        }

        private void SaveProfilePicture()
        {
            MovePhotoToTalentFolder(this.ProfilePicture);

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProfilePicture", GetPhotoUrl(this.ProfilePicture)));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));

            DatabaseHelper.ExecuteNonQuery("UPDATE Talent SET ProfilePicture = @ProfilePicture WHERE ID = @ID", pl);
        }

        private string GetPhotoUrl(string url)
        {
            if (url.Contains("/Temp/"))
            {
                return string.Format("/{0}/{1}/{2}", "TalentPhotos", this.ID, Path.GetFileName(url));
            }

            return url;
        }

        private string GetThumbUrl(string url)
        {
            return string.Format("/{0}/{1}/{2}/{3}", "TalentPhotos", this.ID, "Thumbs", Path.GetFileName(url));
        }

        private void MovePhotoToTalentFolder(string url, bool generateThum = false)
        {
            if (url.Contains("/Temp/"))
            {
                string from = HostingEnvironment.MapPath(url);
                string to = HostingEnvironment.MapPath(GetPhotoUrl(url));

                string directory = Path.GetDirectoryName(to);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Move(from, to);

                if (generateThum)
                {
                    Image image = Image.FromFile(to);

                    //TODO: get Thumb dims

                    Image thumb = image.GetThumbnailImage(250, 250, () => false, IntPtr.Zero);

                    string thumPath = HostingEnvironment.MapPath(GetThumbUrl(url));
                    string thumbDirectory = Path.GetDirectoryName(thumPath);
                    if (!Directory.Exists(thumbDirectory))
                    {
                        Directory.CreateDirectory(thumbDirectory);
                    }

                    thumb.Save(thumPath);
                }
            }
        }

        public bool Delete()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM Talent WHERE ID = @ID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> GetImages()
        {
            var images = new List<string>();

            return images;
        }

        public static List<TalentModel> Get()
        {
            List<TalentModel> list = new List<TalentModel>();

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT t.*, r.Company FROM Talent t LEFT JOIN Representatives r ON r.ID = t.Representative");

            foreach (DataRow row in dt.Rows)
            {
                var obj = new TalentModel(row);
                obj.RepDisplayName = Convert.ToString(row["Company"]);
                list.Add(obj);
            }

            return list;
        }

        public static TalentModel GetByID(int id)
        {
            var bookPictures = new List<string>();
            var thumbnails = new List<string>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Talent WHERE ID = @ID", pl);

            var talent = new TalentModel(dt.Rows[0]);
            talent.LoadLists();

            pl.Clear();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));
            dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TalentPhotos WHERE TalentID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                bookPictures.Add(Convert.ToString(row["Photo"]));
                thumbnails.Add(Convert.ToString(row["Thumbnail"]));
            }

            talent.BookPictures = string.Join(",", bookPictures);
            talent.Thumbnails = string.Join(",", thumbnails);

            return talent;
        }

        public static List<TalentModel> GetByProjectRoleID(int id)
        {
            List<TalentModel> list = new List<TalentModel>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT t.* FROM ProjectTalent JOIN Talent t ON t.ID = TalentID WHERE ProjectRoleID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new TalentModel(row));
            }

            return list;
        }

        private List<MySqlParameter> GetParams()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Gender", this.Gender.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DateOfBirth", this.DateOfBirth));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Nationality", this.Country.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Representative", this.Representative.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Height", this.Height));
            pl.Add(DatabaseHelper.CreateSqlParameter("@EyeColor", this.EyeColor.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HairColor", this.HairColor.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Ethnicity", this.Ethnicity.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShoeSize", this.ShoeSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@WaistSize", this.WaistSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShirtSize", this.ShirtSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Instagram", this.Instagram));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Notes", this.Notes));

            return pl;
        }
    }
}