using MySql.Data.MySqlClient;
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

        [Display(Name = "Agent")]
        public ListItemModel Representative { get; set; }

        public ListItemModel Talent { get; set; }

        public string RepDisplayName { get; set; }

        public string Height { get; set; }

        [Display(Name = "Eye Color")]
        public ListItemModel EyeColor { get; set; }

        [Display(Name = "Hair Color")]
        public ListItemModel HairColor { get; set; }

        public ListItemModel Ethnicity { get; set; }

        [Display(Name = "Shoe")]
        public string ShoeSize { get; set; }

        [Display(Name = "Waist")]
        public string WaistSize { get; set; }

        [Display(Name = "Bust")]
        public string BustSize { get; set; }

        [Display(Name = "Hip")]
        public string HipSize { get; set; }

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
            this.HipSize = Convert.ToString(row["HipSize"]);
            this.BustSize = Convert.ToString(row["BustSize"]);
            this.Notes = Convert.ToString(row["Notes"]);

            this.Country = new ListItemModel(Convert.ToString(row["Nationality"]));
            this.Representative = new ListItemModel(Convert.ToString(row["Representative"]));
            this.Talent = new ListItemModel(Convert.ToString(row["Talent"]));
            this.Ethnicity = new ListItemModel(Convert.ToString(row["Ethnicity"]));
            this.HairColor = new ListItemModel(Convert.ToString(row["HairColor"]));
            this.EyeColor = new ListItemModel(Convert.ToString(row["EyeColor"]));
            this.Gender = new ListItemModel(Convert.ToString(row["Gender"]));

            if (row["DateOfBirth"] != DBNull.Value)
                this.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
        }

        public bool Add()
        {
            string sql = @"INSERT INTO Talent(FirstName,LastName,Gender,DateOfBirth,Nationality,Representative,Talent,Height,EyeColor
                                           ,HairColor,Ethnicity,ShoeSize,WaistSize,HipSize,BustSize,Instagram,Phone,Email,Notes)
                        VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @Nationality, @Representative, @Talent, @Height, @EyeColor
                              , @HairColor, @Ethnicity, @ShoeSize, @WaistSize, @HipSize, @BustSize, @Instagram, @Phone, @Email, @Notes); SELECT LAST_INSERT_ID()";

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
                              ,Talent = @Talent
                              ,Height = @Height
                              ,EyeColor = @EyeColor
                              ,HairColor = @HairColor
                              ,Ethnicity = @Ethnicity
                              ,ShoeSize = @ShoeSize
                              ,WaistSize = @WaistSize
                              ,HipSize = @HipSize
                              ,BustSize = @BustSize
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

            DeletePhotos();

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

        private void DeletePhotos()
        {
            if (string.IsNullOrEmpty(this.BookPictures))
            {
                this.BookPictures = string.Empty;
            }

            var newPhotos = new List<string>(this.BookPictures.Split(','));
            var currentphotos = new Dictionary<string, string>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Photo, Thumbnail FROM TalentPhotos WHERE TalentID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                currentphotos.Add(Convert.ToString(row["Photo"]), Convert.ToString(row["Thumbnail"]));
            }

            foreach (var photo in currentphotos)
            {
                if (!newPhotos.Contains(photo.Key))
                {
                    try
                    {
                        File.Delete(HostingEnvironment.MapPath(photo.Key));
                        File.Delete(HostingEnvironment.MapPath(photo.Value));
                    }
                    catch { }
                }
            }
        }

        private void SaveProfilePicture()
        {
            if (DeleteCurrentProfilePicture())
            {
                MovePhotoToTalentFolder(this.ProfilePicture);

                var pl = new List<MySqlParameter>();
                pl.Add(DatabaseHelper.CreateSqlParameter("@ProfilePicture", GetPhotoUrl(this.ProfilePicture)));
                pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));

                DatabaseHelper.ExecuteNonQuery("UPDATE Talent SET ProfilePicture = @ProfilePicture WHERE ID = @ID", pl);
            }
        }

        private bool DeleteCurrentProfilePicture()
        {
            bool deleted = false;

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));

            string currentProfilePicture = Convert.ToString(DatabaseHelper.ExecuteScalar("SELECT ProfilePicture FROM Talent WHERE ID = @ID", pl));
            this.ProfilePicture = string.IsNullOrEmpty(this.ProfilePicture) ? string.Empty : this.ProfilePicture;

            if (!currentProfilePicture.Equals(this.ProfilePicture))
            {
                try
                {
                    File.Delete(HostingEnvironment.MapPath(currentProfilePicture));
                }
                catch { }
                
                return true;
            }

            return deleted;
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
                    using (Image image = Image.FromFile(to))
                    {
                        double reductionPercent = 250.0 / Math.Max(image.Width, image.Height);
                        int width = (int)(image.Width * reductionPercent);
                        int height = (int)(image.Height * reductionPercent);

                        using (Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero))
                        {
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
            }
        }

        public bool Delete()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM Talent WHERE ID = @ID", pl);

            if (r >= 1)
            {
                pl.Clear();
                pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
                DatabaseHelper.ExecuteNonQuery("DELETE FROM TalentPhotos WHERE TalentID = @ID", pl);

                string path = HostingEnvironment.MapPath(string.Format("/TalentPhotos/{0}", this.ID));
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void GetImages()
        {
            var bookPictures = new List<string>();
            var thumbnails = new List<string>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TalentPhotos WHERE TalentID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                bookPictures.Add(Convert.ToString(row["Photo"]));
                thumbnails.Add(Convert.ToString(row["Thumbnail"]));
            }

            this.BookPictures = string.Join(",", bookPictures);
            this.Thumbnails = string.Join(",", thumbnails);
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
            talent.GetImages();
            
            return talent;
        }

        public static List<TalentModel> GetByProjectRoleID(int id)
        {
            List<TalentModel> list = new List<TalentModel>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT t.*, r.Company FROM ProjectTalent JOIN Talent t ON t.ID = TalentID LEFT JOIN Representatives r ON r.ID = t.Representative WHERE ProjectRoleID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                var obj = new TalentModel(row);
                obj.RepDisplayName = Convert.ToString(row["Company"]);
                list.Add(obj);
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
            pl.Add(DatabaseHelper.CreateSqlParameter("@HipSize", this.HipSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@BustSize", this.BustSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Instagram", this.Instagram));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Notes", this.Notes));

            return pl;
        }
    }
}