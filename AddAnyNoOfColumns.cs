using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace TelerikReportWebAPI.Reports
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class Report1 : Telerik.Reporting.Report
    {
        public Report1()
        {
            InitializeComponent();
            this.table1.DataSource = GetData();
        }

        private DataTable GetData()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=countrydb;Integrated Security=True";
            string query = "SELECT CountryId, CountryName, Population, Area FROM Countries";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    var random = new Random();

                    // Define the columns to add
                    var columnsToAdd = new List<(string Name, Type Type, Func<DataRow, int, object> ValueGenerator)>
                    {
                        ("password", typeof(string), (DataRow row, int index) => GenerateRandomString(8, random)),
                        ("gratitude", typeof(string), (DataRow row, int index) => $"Thank God {index + 1}"),
                        ("randomNumber", typeof(int), (DataRow row, int index) => random.Next(1, 101))
                    };

                    // Add the new columns
                    AddColumnsToDataTable(dataTable, columnsToAdd);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                return dataTable;
            }
        }

        private void AddColumnsToDataTable(DataTable dataTable, List<(string Name, Type Type, Func<DataRow, int, object> ValueGenerator)> columnsToAdd)
        {
            foreach (var (name, type, valueGenerator) in columnsToAdd)
            {
                dataTable.Columns.Add(name, type);
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                foreach (var (name, _, valueGenerator) in columnsToAdd)
                {
                    row[name] = valueGenerator(row, i);
                }
            }
        }

        private static string GenerateRandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
