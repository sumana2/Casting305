using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
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
            string sql = @"INSERT INTO Projects (Title,Company,Email,Phone,DueDate)
                           VALUES (@Title, @Company, @Email, @Phone, @DueDate); SELECT LAST_INSERT_ID()";

            this.ID = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, GetParams()));

            if (this.ID >= 1)
            {
                return SaveRoles();
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            string sql = @"UPDATE Projects
                           SET Title = @Title, Company = @Company, Email = @Email, Phone = @Phone, DueDate = @DueDate
                           WHERE ID = @ID";

            int r = DatabaseHelper.ExecuteNonQuery(sql, GetParams());

            if (r >= 1)
            {
                return SaveRoles();
            }
            else
            {
                return false;
            }
        }

        public bool Delete()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM Projects WHERE ID = @ID", pl);

            if (r >= 1)
            {
                ProjectRoleModel.DeleteByProject(this.ID);

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

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Projects");

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

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Projects WHERE ID = @ID", pl);

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

        private List<MySqlParameter> GetParams()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Title", this.Title));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@DueDate", this.DueDate));

            return pl;
        }

        public bool SaveRoles()
        {
            bool ok = true;

            var currentRoles = ProjectRoleModel.GetByProject(this.ID);

            if (Roles == null)
            {
                Roles = new List<ProjectRoleModel>();
            }

            foreach (var role in currentRoles)
            {
                if (!Roles.Exists(x => x.ID == role.ID))
                {
                    ok = ok && role.Delete();
                }
            }

            foreach (var role in Roles)
            {
                if (role.ID == 0)
                {
                    role.ProjectID = this.ID;
                    ok = ok && role.Add();
                }
                else
                {
                    ok = ok && role.Update();
                }
                
            }
         
            return ok;
        }
    }
}