using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class TalentModel
    {
        public int ID { get; set; }
    
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        public string Nationality { get; set; }

        public string Representative { get; set; }

        public string Height { get; set; }

        [Display(Name = "Eye Color")]
        public string EyeColor { get; set; }

        [Display(Name = "Hair Color")]
        public string HairColor { get; set; }

        public string Ethnicity { get; set; }

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

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        public bool Add()
        {
            string sql = @"INSERT INTO[dbo].[Talent]([FirstName],[LastName],[Gender],[DateOfBirth],[Nationality],[Representative],[Height],[EyeColor]
                                           ,[HairColor],[Ethnicity],[ShoeSize],[WaistSize],[ShirtSize],[Instagram],[Phone],[Email],[Notes],[ProfilePicture])
                        VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @Nationality, @Representative, @Height, @EyeColor
                              , @HairColor, @Ethnicity, @ShoeSize, @WaistSize, @ShirtSize, @Instagram, @Phone, @Email, @Notes, @ProfilePicture)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Gender", this.Gender));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DateOfBirth", this.DateOfBirth));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Nationality", this.Nationality));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Representative", this.Representative));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Height", this.Height));
            pl.Add(DatabaseHelper.CreateSqlParameter("@EyeColor", this.EyeColor));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HairColor", this.HairColor));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Ethnicity", this.Ethnicity));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShoeSize", this.ShoeSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@WaistSize", this.WaistSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShirtSize", this.ShirtSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Instagram", this.Instagram));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Notes", this.Notes));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProfilePicture", this.ProfilePicture));
            int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            string sql = @"UPDATE [dbo].[Talent]
                           SET [FirstName] = @FirstName
                              ,[LastName] = @LastName
                              ,[Gender] = @Gender
                              ,[DateOfBirth] = @DateOfBirth
                              ,[Nationality] = @Nationality
                              ,[Representative] = @Representative
                              ,[Height] = @Height
                              ,[EyeColor] = @EyeColor
                              ,[HairColor] = @HairColor
                              ,[Ethnicity] = @Ethnicity
                              ,[ShoeSize] = @ShoeSize
                              ,[WaistSize] = @WaistSize
                              ,[ShirtSize] = @ShirtSize
                              ,[Instagram] = @Instagram
                              ,[Phone] = @Phone
                              ,[Email] = @Email
                              ,[Notes] = @Notes
                              ,[ProfilePicture] = @ProfilePicture
                         WHERE ID = @ID";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Gender", this.Gender));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DateOfBirth", this.DateOfBirth));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Nationality", this.Nationality));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Representative", this.Representative));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Height", this.Height));
            pl.Add(DatabaseHelper.CreateSqlParameter("@EyeColor", this.EyeColor));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HairColor", this.HairColor));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Ethnicity", this.Ethnicity));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShoeSize", this.ShoeSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@WaistSize", this.WaistSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ShirtSize", this.ShirtSize));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Instagram", this.Instagram));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Notes", this.Notes));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProfilePicture", this.ProfilePicture));
            int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete()
        {
            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM dbo.Talent WHERE ID = @ID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<TalentModel> Get()
        {
            List<TalentModel> list = new List<TalentModel>();

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Talent WITH (NOLOCK)");

            foreach (DataRow row in dt.Rows)
            {
                var talent = new TalentModel();
                talent.ID = Convert.ToInt32(row["Id"]);
                talent.FirstName = Convert.ToString(row["FirstName"]);
                talent.LastName = Convert.ToString(row["LastName"]);
                talent.Email = Convert.ToString(row["Email"]);
                talent.Phone = Convert.ToString(row["Phone"]);
                talent.Height = Convert.ToString(row["Height"]);
                talent.HairColor = Convert.ToString(row["HairColor"]);
                talent.EyeColor = Convert.ToString(row["EyeColor"]);
                talent.Gender = Convert.ToString(row["Gender"]);
                talent.Instagram = Convert.ToString(row["Instagram"]);
                talent.ProfilePicture = Convert.ToString(row["ProfilePicture"]);

                if (row["DateOfBirth"] != DBNull.Value)
                    talent.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);

                list.Add(talent);
            }

            return list;
        }

        public static List<TalentModel> GetByProjectRoleID(int id)
        {
            List<TalentModel> list = new List<TalentModel>();

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT t.* FROM dbo.ProjectTalent WITH (NOLOCK) JOIN dbo.Talent t WITH (NOLOCK) ON t.ID = TalentID WHERE ProjectRoleID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                var talent = new TalentModel();
                talent.ID = Convert.ToInt32(row["Id"]);
                talent.FirstName = Convert.ToString(row["FirstName"]);
                talent.LastName = Convert.ToString(row["LastName"]);
                talent.Email = Convert.ToString(row["Email"]);
                talent.Phone = Convert.ToString(row["Phone"]);
                talent.Height = Convert.ToString(row["Height"]);
                talent.HairColor = Convert.ToString(row["HairColor"]);
                talent.EyeColor = Convert.ToString(row["EyeColor"]);
                talent.Gender = Convert.ToString(row["Gender"]);
                talent.Instagram = Convert.ToString(row["Instagram"]);
                talent.ProfilePicture = Convert.ToString(row["ProfilePicture"]);

                if (row["DateOfBirth"] != DBNull.Value)
                    talent.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);

                list.Add(talent);
            }

            return list;
        }
    }
}