using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WindowsFormsApp4.Models;

namespace WindowsFormsApp4.Services
{
    class StudentsProvider
    {
        private SqlConnection _connection;
        public StudentsProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Student> GetAllWithGroups()
        {
            List<Student> result = new List<Student>();

            try
            {
                _connection.Open();
                var command = new SqlCommand(
                    cmdText: @"
                    SELECT [Students].[id], [Students].[name], [Students].[surname], [Students].[group_id], [Groups].[name], [Groups].[year], [Groups].[specialty_id]
                    FROM [Students]
                    LEFT JOIN [Groups]
                    ON [Students].[group_id] = [Groups].[id]
                    ",
                    connection: _connection
               );

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var student = new Student();
                        var group = new Group();
                        student.Id = reader.GetInt32(0);
                        student.Name = reader.GetString(1);
                        student.Surname = reader.GetString(2);
                        student.GroupId = reader.GetInt32(3);
                        group.Id = reader.GetInt32(3);
                        group.Name = reader.GetString(4);
                        group.Year = reader.GetInt32(5);
                        group.SpecialtyId = reader.GetInt32(6);
                        student.Group = group;
                        result.Add(student);
                    }
                }
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public bool Add(Student student)
        {
            bool result = false;

            try
            {
                _connection.Open();

                var command = new SqlCommand(
                    cmdText: @"
                        INSERT INTO [Students]
                            ([name], [surname], [group_id]
                        VALUES
                            (@Name, @Surname, @Group_id
                        ",
                    _connection
                );
                command.Parameters.AddWithValue(@"Name", student.Name);
                command.Parameters.AddWithValue(@"Surname", student.Surname);
                command.Parameters.AddWithValue(@"Group_id", student.GroupId);

                int affected = command.ExecuteNonQuery();
                result = affected > 0;
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        public bool Update(Student student)
        {
            bool result = false;

            try
            {
                _connection.Open();
                var command = new SqlCommand(
                    @"
                        UPDATE [Students]
                        SET
                            [name] = @Name,
                            [surname] = @Surname,
                            [group_id] = @Group_id,
                        WHERE
                            [id] = @Id
                        ",
                    _connection
                );
                command.Parameters.AddWithValue("@Name", student.Name);
                command.Parameters.AddWithValue("@Surname", student.Surname);
                command.Parameters.AddWithValue("@Id", student.Id);

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
                        DELETE FROM [Students]
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
