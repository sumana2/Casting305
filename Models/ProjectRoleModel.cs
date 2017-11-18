﻿using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ProjectRoleModel
    {
        public int ID { get; set; }
    
        public int ProjectID { get; set; }

        public string Name { get; set; }

        public int AgeMin { get; set; }

        public int AgeMax { get; set; }

        public decimal HeightMin { get; set; }

        public decimal HeightMax { get; set; }

        public string HairColor { get; set; }

        public List<TalentModel> Talent { get; set; }

        public IPagedList<TalentModel> TalentPagedList { get; set; }

        public ProjectRoleModel()
        {
            Talent = new List<TalentModel>();
        }

        public bool Add()
        {
            string sql = @"INSERT INTO[dbo].[ProjectRoles]([ProjectID],[Name],[AgeMin],[AgeMax],[HeightMin],[HeightMax],[HairColor])
                           VALUES (@ProjectID, @Name, @AgeMin, @AgeMax, @HeightMin, @HeightMax, @HairColor)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProjectID", this.ProjectID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Name", this.Name));
            pl.Add(DatabaseHelper.CreateSqlParameter("@AgeMin", this.AgeMin));
            pl.Add(DatabaseHelper.CreateSqlParameter("@AgeMax", this.AgeMax));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HeightMin", this.HeightMin));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HeightMax", this.HeightMax));
            pl.Add(DatabaseHelper.CreateSqlParameter("@HairColor", this.HairColor));
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
            //pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Gender", this.Gender));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@DateOfBirth", this.DateOfBirth));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Nationality", this.Nationality));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Representative", this.Representative));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Height", this.Height));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@EyeColor", this.EyeColor));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@HairColor", this.HairColor));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Ethnicity", this.Ethnicity));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@ShoeSize", this.ShoeSize));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@WaistSize", this.WaistSize));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@ShirtSize", this.ShirtSize));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Instagram", this.Instagram));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@Notes", this.Notes));
            //pl.Add(DatabaseHelper.CreateSqlParameter("@ProfilePicture", this.ProfilePicture));
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

        public static ProjectRoleModel GetByID(int id, bool loadAllTalent = false)
        {
            var obj = new ProjectRoleModel();

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.ProjectRoles WITH (NOLOCK) WHERE ID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                obj.ID = Convert.ToInt32(row["Id"]);
                obj.ProjectID = Convert.ToInt32(row["ProjectID"]);
                obj.Name = Convert.ToString(row["Name"]);
                obj.AgeMin = Convert.ToInt32(row["AgeMin"]);
                obj.AgeMax = Convert.ToInt32(row["AgeMax"]);
                obj.HeightMin = Convert.ToDecimal(row["HeightMin"]);
                obj.HeightMax = Convert.ToDecimal(row["HeightMax"]);
                obj.HairColor = Convert.ToString(row["HairColor"]);

                if (loadAllTalent)
                {
                    obj.Talent = TalentModel.Get();
                }
                else
                {
                    obj.Talent = TalentModel.GetByProjectRoleID(id);
                }
                    
                break;
            }

            return obj;
        }

        public static List<ProjectRoleModel> GetByProject(int projectID)
        {
            List<ProjectRoleModel> list = new List<ProjectRoleModel>();

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProjectID", projectID));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.ProjectRoles WITH (NOLOCK) WHERE ProjectID = @ProjectID", pl);

            foreach (DataRow row in dt.Rows)
            {
                var role = new ProjectRoleModel();
                role.ID = Convert.ToInt32(row["Id"]);
                role.ProjectID = Convert.ToInt32(row["ProjectID"]);
                role.Name = Convert.ToString(row["Name"]);
                role.AgeMin = Convert.ToInt32(row["AgeMin"]);
                role.AgeMax = Convert.ToInt32(row["AgeMax"]);
                role.HeightMin = Convert.ToDecimal(row["HeightMin"]);
                role.HeightMax = Convert.ToDecimal(row["HeightMax"]);
                role.HairColor = Convert.ToString(row["HairColor"]);

                list.Add(role);
            }

            return list;
        }
    }
}