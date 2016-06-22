using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Repositories.Contract;

namespace Repositories.Impl
{
    public class AdoDbRepo:IRepository
    {
        private readonly string _conStr;

        public AdoDbRepo()
        {
            _conStr = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public void StoreTree(StoreTreeRequestDao data)
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                SqlTransaction transaction = sql.BeginTransaction();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.Transaction = transaction;

                    foreach (var node in data.Tree)
                    {
                        AddNodeInternal(node, cmd);
                    }
                }
                transaction.Commit();
                sql.Close();
            }
        }

        public GetTreeResponseDao GetTree(GetTreeRequestDao data)
        {
            GetTreeResponseDao result = new GetTreeResponseDao
            {
                Nodes=new List<GetNodeResponseDao>()
            };
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.GetTree;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@UserId", data.UserId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        GetNodeResponseDao value=new GetNodeResponseDao
                        {
                            Id=reader.GetGuid(0),
                            StringValue = reader.GetString(1),
                            UserId = Guid.Parse(reader.GetString(2)),
                            DigitValue = reader.GetInt32(3),
                            Generation = reader.GetInt32(5)
                        };
                        if (reader.IsDBNull(4))
                        {
                            value.ParentId = null;
                        }
                        else
                        {
                            value.ParentId = Guid.Parse(reader.GetString(4));
                        }
                        result.Nodes.Add(value);
                    }
                }
                sql.Close();
            }
            return result;
        }

        public void AddNode(AddNodeRequestDao addNode)
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                { 
                    cmd.Connection = sql;

                    AddNodeInternal(addNode, cmd);
                }
                sql.Close();
            }
        }

        private void AddNodeInternal(AddNodeRequestDao addNode,SqlCommand cmd)
        {
            cmd.CommandText = addNode.ParentId == null ? SqlQueries.InsertRootNode : SqlQueries.InsertFullNode;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@Id", addNode.Id);
            cmd.Parameters.AddWithValue("@Text", addNode.StringValue);
            cmd.Parameters.AddWithValue("@Value", addNode.DigitValue);
            cmd.Parameters.AddWithValue("@UserId", addNode.UserId);
            if (addNode.ParentId != null)
            {
                cmd.Parameters.AddWithValue("@ParentId", addNode.ParentId);
            }
            cmd.Parameters.AddWithValue("@Generation", addNode.Generation);
            cmd.Parameters.AddWithValue("@DateCreation", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public void RemoveNode(RemoveNodeRequestDao data)
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.RemoveNode;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@UserId", data.Id);

                    cmd.ExecuteNonQuery();
                }
                sql.Close();
            }
        }

        public void UpdateNode(UpdateNodeRequestDao data)
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.UpdateNode;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@Value", data.DigitValue);
                    cmd.Parameters.AddWithValue("@Text", data.StringValue);
                    cmd.Parameters.AddWithValue("@NodeId", data.NodeId);

                    cmd.ExecuteNonQuery();
                }
                sql.Close();
            }
        }

        public bool CheckIfValueExists(CheckIfValueExistsRequestDao data)
        {
            bool result = false;
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.CheckNodeId;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@Value", data.DigitValue);
                    cmd.Parameters.AddWithValue("@UserId", data.UserId);

                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.HasRows)
                        result = true;
                }
                sql.Close();
            }
            return result;
        }

        public void RemoveTree(RemoveTreeRequestDao data)
        {
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.RemoveTree;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@UserId", data.UserId);

                    cmd.ExecuteNonQuery();
                }
                sql.Close();
            }
        }

        public GetNodeResponseDao GetNode(GetNodeRequestDao data)
        {
            GetNodeResponseDao result=new GetNodeResponseDao();
            using (SqlConnection sql = new SqlConnection(_conStr))
            {
                sql.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = SqlQueries.GetNode;
                    cmd.Connection = sql;

                    cmd.Parameters.AddWithValue("@NodeId", data.NodeId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.HasRows)
                    {
                        reader.Read();
                        result = new GetNodeResponseDao
                        {
                            Id = reader.GetGuid(0),
                            StringValue = reader.GetString(1),
                            UserId = Guid.Parse(reader.GetString(2)),
                            DigitValue = reader.GetInt32(3),
                            Generation = reader.GetInt32(5)
                        };
                        if (reader.IsDBNull(4))
                        {
                            result.ParentId = null;
                        }
                        else
                        {
                            result.ParentId = Guid.Parse(reader.GetString(4));
                        }
                    }
                }
                sql.Close();
            }
            return result;
        }
    }
}
