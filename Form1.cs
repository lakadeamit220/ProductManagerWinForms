using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProductManagerWinForms
{
    public partial class ProductManagerForm : Form
    {
        // Connection string to connect to the SQL Server database
        private static string connectionString = "Server=AMITLAKADE\\SQLEXPRESS;Database=sqldb;Trusted_Connection=True;";

        public ProductManagerForm()
        {
            InitializeComponent();
            LoadData(); // Load data when the form is initialized
        }

        // Event handler for the Insert button
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Retrieve input values from text boxes
            string productName = txtProductName.Text;
            string category = txtCategory.Text;
            decimal price;
            int quantity;

            // Validate and parse the price
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Please enter a valid price.");
                return; // Stop execution if the price is invalid
            }

            // Validate and parse the quantity
            if (!int.TryParse(txtQuantity.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                return; // Stop execution if the quantity is invalid
            }

            // Insert the record into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (ProductName, Category, Price, Quantity) VALUES (@ProductName, @Category, @Price, @Quantity)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    try
                    {
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the insert command
                        MessageBox.Show("Record inserted successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            // Clear text fields and reload data to reflect changes
            ClearTextFields();
            LoadData();
        }

        // Event handler for the Update button
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int productId;
            string productName = txtProductName.Text;
            string category = txtCategory.Text;
            decimal price;
            int quantity;

            // Validate ProductID from selected row in DataGridView
            if (dataGridViewProducts.CurrentRow == null || !int.TryParse(dataGridViewProducts.CurrentRow.Cells[0].Value.ToString(), out productId))
            {
                MessageBox.Show("Invalid Product ID.");
                return;
            }

            // Validate Price
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Invalid price format. Please enter a valid decimal number.");
                return;
            }

            // Validate Quantity
            if (!int.TryParse(txtQuantity.Text, out quantity))
            {
                MessageBox.Show("Invalid quantity format. Please enter a valid integer.");
                return;
            }

            // Update the record in the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET ProductName = @ProductName, Category = @Category, Price = @Price, Quantity = @Quantity WHERE ProductID = @ProductID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    try
                    {
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the update command
                        MessageBox.Show("Record updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        MessageBox.Show($"Error updating record: {ex.Message}");
                    }
                }
            }

            // Clear text fields and reload data to reflect changes
            ClearTextFields();
            LoadData();
        }

        // Event handler for the Delete button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int productId;

            // Validate selected product from DataGridView
            if (dataGridViewProducts.CurrentRow == null || !int.TryParse(dataGridViewProducts.CurrentRow.Cells[0].Value.ToString(), out productId))
            {
                MessageBox.Show("Select a record to delete.");
                return;
            }

            // Delete the record from the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameter to avoid SQL injection
                    command.Parameters.AddWithValue("@ProductID", productId);

                    try
                    {
                        connection.Open(); // Open the connection
                        command.ExecuteNonQuery(); // Execute the delete command
                        MessageBox.Show("Record deleted successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        MessageBox.Show($"Error deleting record: {ex.Message}");
                    }
                }
            }

            // Clear text fields and reload data to reflect changes
            ClearTextFields();
            LoadData();
        }

        // Event handler for the Load button
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData(); // Reload data from the database
        }

        // Method to load data from the database into the DataGridView
        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill DataTable with data from the database
                    dataGridViewProducts.DataSource = dataTable; // Bind DataTable to DataGridView
                }
            }
        }

        // Method to clear all text fields
        private void ClearTextFields()
        {
            txtProductName.Clear();
            txtCategory.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
        }

        private void SearchByCategory(string category)
        {
            // Define the query to select ProductName and Price based on category
            //string query = "SELECT ProductName, Price FROM Products WHERE Category = @Category";
            string query = "SELECT ProductName, Price FROM Products WHERE LOWER(Category) = LOWER(@Category)";


            // Use a DataTable to store the results
            DataTable searchResults = new DataTable();

            // Establish a connection using the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Create a SqlDataAdapter with the query and connection
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        // Add the @Category parameter to prevent SQL injection
                        adapter.SelectCommand.Parameters.AddWithValue("@Category", category);

                        // Fill the DataTable with the search results
                        adapter.Fill(searchResults);
                    }

                    //Provide user feedback when no records match the search criteria
                    if (searchResults.Rows.Count == 0)
                    {
                        MessageBox.Show("No products found in the specified category.");
                    }

                    // Bind the search results to the DataGridView
                    dataGridViewProducts.DataSource = searchResults;

                    // Optional: Adjust DataGridView columns (e.g., set headers)
                    dataGridViewProducts.Columns["ProductName"].HeaderText = "Product Name";
                    dataGridViewProducts.Columns["Price"].HeaderText = "Price";
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the search
                    MessageBox.Show("An error occurred while searching: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Retrieve the category entered by the user
            string category = txtSearchCategory.Text.Trim();

            // Validate the input
            if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Please enter a category to search.");
                return;
            }

            // Call the search method
            SearchByCategory(category);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear the search TextBox
            txtSearchCategory.Clear();

            // Reload all data
            LoadData();
        }
    }
}
