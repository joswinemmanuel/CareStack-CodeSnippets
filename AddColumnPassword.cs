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
            //this.DataSource = GetData();
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

                    dataTable.Columns.Add("password", typeof(string));

                    var random = new Random();
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var charsLength = chars.Length;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var stringBuilder = new StringBuilder(8);
                        for (int i = 0; i < 8; i++)
                        {
                            stringBuilder.Append(chars[random.Next(charsLength)]);
                        }

                        var finalString = stringBuilder.ToString();
                        row["password"] = finalString;
                    }
                }
                catch (Exception ex)
                {
                    // Handle or log the exception as needed
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                return dataTable;
            }

        }
    }
}
