using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }
    
        public string Title { get; set; }

        public string Company { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DueDate { get; set; }

        public List<ProjectRoleModel> Roles { get; set; }

        public ProjectModel()
        {
            Roles = new List<ProjectRoleModel>();
        }

        public bool Add()
        {
            string sql = @"INSERT INTO[dbo].[Projects]([Title],[Company],[Email],[Phone],[DueDate])
                           VALUES (@Title, @Company, @Email, @Phone, @DueDate)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@Title", this.Title));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DueDate", this.DueDate));
            int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (r >= 1)
            {
                foreach (var role in Roles)
                {
                    //TODO: Get Project ID and add transaction to all this
                    if (!role.Add())
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            string sql = @"UPDATE [dbo].[Projects]
                           SET Title = @Title, Company = @Company, Email = @Email, Phone = @Phone, DueDate = @DueDate
                           WHERE ID = @ID";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Title", this.Title));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DueDate", this.DueDate));
            int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (r >= 1)
            {
                foreach (var role in Roles)
                {
                    if (!role.Update())
                    {
                        return false;
                    }
                }

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

        public static List<ProjectModel> Get()
        {
            List<ProjectModel> list = new List<ProjectModel>();

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Projects WITH (NOLOCK)");

            foreach (DataRow row in dt.Rows)
            {
                var project = new ProjectModel();
                project.ID = Convert.ToInt32(row["Id"]);
                project.Title = Convert.ToString(row["Title"]);
                project.Company = Convert.ToString(row["Company"]);
                project.Email = Convert.ToString(row["Email"]);
                project.Phone = Convert.ToString(row["Phone"]);
                
                if (row["DueDate"] != DBNull.Value)
                    project.DueDate = Convert.ToDateTime(row["DueDate"]);

                list.Add(project);
            }

            return list;
        }

        public static ProjectModel GetByID(int id)
        {
            ProjectModel project = new ProjectModel();

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Projects WITH (NOLOCK) WHERE ID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                project.ID = Convert.ToInt32(row["Id"]);
                project.Title = Convert.ToString(row["Title"]);
                project.Company = Convert.ToString(row["Company"]);
                project.Email = Convert.ToString(row["Email"]);
                project.Phone = Convert.ToString(row["Phone"]);

                if (row["DueDate"] != DBNull.Value)
                    project.DueDate = Convert.ToDateTime(row["DueDate"]);

                project.Roles = ProjectRoleModel.GetByProject(project.ID);

                break;
            }

            return project;
        }
    }
}