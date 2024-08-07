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
using TextBox = Telerik.Reporting.TextBox;

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
            var data = GetData();
            this.table1.DataSource = data;
            AddDynamicColumns(data);
        }

        private void AddDynamicColumns(DataTable dataTable)
        {
            int existingColumns = this.table1.Body.Columns.Count;

            for (int i = existingColumns; i < dataTable.Columns.Count; i++)
            {
                string columnName = dataTable.Columns[i].ColumnName;

                this.table1.Body.Columns.Add(new TableBodyColumn(Unit.Cm(2)));

                var headerTextBox = new TextBox
                {
                    Name = $"textBox{i + 1}",
                    Size = new SizeU(Unit.Cm(2), Unit.Cm(0.5)),
                    StyleName = "Normal.TableHeader",
                    Value = columnName
                };

                var tableGroup = new TableGroup();
                tableGroup.ReportItem = headerTextBox;
                this.table1.ColumnGroups.Add(tableGroup);

                var bodyTextBox = new TextBox
                {
                    Name = $"textBox{i + existingColumns + 1}",
                    Size = new SizeU(Unit.Cm(2), Unit.Cm(0.5)),
                    StyleName = "Normal.TableBody",
                    Value = $"= Fields.{columnName}"
                };
                this.table1.Body.SetCellContent(0, i, bodyTextBox);
            }

            this.table1.Size = new SizeU(Unit.Cm(2 * dataTable.Columns.Count), this.table1.Size.Height);
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

                    var columnsToAdd = new List<(string Name, Type Type, Func<DataRow, int, object> ValueGenerator)>
                    {
                        ("Password", typeof(string), (DataRow row, int index) => GenerateRandomString(8, random)),
                        ("Gratitude", typeof(string), (DataRow row, int index) => $"Thank God {index + 1}"),
                        ("Number", typeof(int), (DataRow row, int index) => random.Next(1, 101))
                    };

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
