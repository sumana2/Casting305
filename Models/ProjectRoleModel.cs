using MySql.Data.MySqlClient;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
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

        public int TalentCount { get; set; }

        public IPagedList<TalentModel> TalentPagedList { get; set; }

        public string NameNoSpace { get { return Name.Replace(" ",""); } }

        public ProjectRoleModel()
        {
            Talent = new List<TalentModel>();
        }

        public bool Add()
        {
            string sql = @"INSERT INTO ProjectRoles(ProjectID,Name,AgeMin,AgeMax,HeightMin,HeightMax,HairColor)
                           VALUES (@ProjectID, @Name, @AgeMin, @AgeMax, @HeightMin, @HeightMax, @HairColor)";

            var pl = new List<MySqlParameter>();
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
            string sql = @"UPDATE ProjectRoles
                           SET ProjectID = @ProjectID, Name = @Name, AgeMin = @AgeMin, AgeMax = @AgeMax, HeightMin = @HeightMin, HeightMax = @HeightMax, HairColor = @HairColor
                           WHERE ID = @ID";

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
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

        public bool Delete()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ProjectID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM ProjectRoles WHERE ProjectID = @ID", pl);

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

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM ProjectRoles WHERE ID = @ID", pl);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
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
                    var roleTalent = TalentModel.GetByProjectRoleID(id);

                    foreach (var t in obj.Talent)
                    {
                        t.Checked = roleTalent.Exists(x => x.ID == t.ID);
                    }
                }
                else
                {
                    obj.Talent = TalentModel.GetByProjectRoleID(id);
                    obj.Talent.ForEach(x => x.Checked = true);
                }

                obj.Talent.ForEach(x => x.ShowCheck = true);
            }

            return obj;
        }

        public static List<ProjectRoleModel> GetByProject(int projectID)
        {
            List<ProjectRoleModel> list = new List<ProjectRoleModel>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProjectID", projectID));

            DataTable dt = DatabaseHelper.ExecuteQuery(@"SELECT * FROM ProjectRoles pr
                                                            LEFT JOIN (
	                                                            SELECT ProjectRoleID, COUNT(ID) AS TalentCount 
	                                                            FROM ProjectTalent
	                                                            GROUP BY ProjectRoleID 
                                                            ) pt ON pt.ProjectRoleID = pr.ID
                                                         WHERE pr.ProjectID = @ProjectID", pl);

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

                if (row["TalentCount"] != DBNull.Value)
                    role.TalentCount = Convert.ToInt32(row["TalentCount"]);

                list.Add(role);
            }

            return list;
        }

        public static bool DeleteByProject(int id)
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));
            DatabaseHelper.ExecuteNonQuery("DELETE FROM ProjectTalent WHERE ProjectRoleID IN (SELECT ID FROM ProjectRoles WHERE ProjectID = @ID)", pl);

            pl.Clear();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));
            DatabaseHelper.ExecuteNonQuery("DELETE FROM ProjectRoles WHERE ProjectID = @ID", pl);

            return true;
        }

        public static bool AddTalent(int projectRoleID, int talentID)
        {
            string sql = @"INSERT INTO ProjectTalent(ProjectRoleID,TalentID)
                           VALUES (@ProjectRoleID, @TalentID)";

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProjectRoleID", projectRoleID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@TalentID", talentID));
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

        public static bool RemoveTalent(int projectRoleID, int talentID)
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ProjectRoleID", projectRoleID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@TalentID", talentID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM ProjectTalent WHERE ProjectRoleID = @ProjectRoleID AND TalentID = @TalentID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}