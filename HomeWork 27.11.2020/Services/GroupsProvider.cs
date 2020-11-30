using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp4.Models;

namespace WindowsFormsApp4.Services
{
    class GroupsProvider
    {
        private SqlConnection _connection;
        public GroupsProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Group> GetAll()
        {
            List<Group> result = new List<Group>();

            try
            {
                _connection.Open();
                var command = new SqlCommand(
                    cmdText: @"
                    SELECT [Groups].[id], [Groups].[name], [Groups].[year], [Groups].[specialty_id], [Specialties].[name], [Specialties].[Code]
                    FROM [Groups]
                    LEFT JOIN [Specialties]
                    ON [Groups].[specialty_id] = [Specialties].[id];
                    ",
                     connection: _connection
               );

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var group = new Group();
                        var specialty = new Specialty();
                        group.Id = reader.GetInt32(0);
                        group.Name = reader.GetString(1);
                        group.Year = reader.GetInt32(2);
                        group.SpecialtyId = reader.GetInt32(3);
                        specialty.Id = reader.GetInt32(3);
                        specialty.Name = reader.GetString(4);
                        specialty.Code = reader.GetString(5);
                        group.Specialty = specialty;
                        result.Add(group);
                    }
                }

            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public bool Add(Group group)
        {
            bool result = false;

            try
            {
                _connection.Open();

                var command = new SqlCommand(
                    cmdText: @"
                        INSERT INTO [Groups]
                            ([name], [year], [specialty_id])
                        VALUES
                            (@Name, @Year, @Specialty_id
                    ",              
                    _connection
                );
                command.Parameters.AddWithValue(@"Name", group.Name);
                command.Parameters.AddWithValue(@"Year", group.Year);
                command.Parameters.AddWithValue(@"SpecialtyId", group.SpecialtyId);

                int affected = command.ExecuteNonQuery();
                result = affected > 0;
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public bool Update(Group group)
        {
            bool result = false;

            try
            {
                _connection.Open();
                var command = new SqlCommand(
                    @"
                        UPDATE [Groups]
                        SET
                            [year] = @Year,
                            [name] = @Name,
                            [specialty_id] = @Specialty_id,
                        WHERE
                            [id] = @Id
                    ",              
                    _connection
                );
                command.Parameters.AddWithValue("@Name", group.Name);
                command.Parameters.AddWithValue("@Year", group.Year);
                command.Parameters.AddWithValue("@Id", group.Id);

                int affected = command.ExecuteNonQuery();
                result = affected > 0;
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public bool Delete(int id)
        {
            bool result = false;

            try
            {
                _connection.Open();
                var command = new SqlCommand(
                    @"
                        DELETE FROM [Groups]
                        WHERE [id] = @id
                        ",
                    _connection
                );
                command.Parameters.AddWithValue("@Id", id);
                int affected = command.ExecuteNonQuery();
                result = affected > 0;

            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

    }
}
