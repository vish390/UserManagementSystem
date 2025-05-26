using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace UserManagementSystem
{
    public partial class Form1 : Form
    {
        // Connection string to your database
        private string connectionString = "Server= .; Database=UserDB; Integrated Security=True;";


        public class Order
        {
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public bool Flagged { get; set; }
            public int UserId { get; set; }
        }


        public Form1()
        {
            InitializeComponent();
            LoadData(); // Load existing users when the form starts
            LoadUserCount();
            FlagHighValueOrders();
            LoadHighestAmount(); ;
            UpdateTotalSum();

        }

        // Load data from database to DataGridView
        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Orders", con);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    dataGridView2.DataSource = dt1;
                }

               // List<Order> orders = GetOrdersFromDatabase();
               // var flaggedOrders = orders.Where(o => o.Flagged).ToList();

                //var flaggedOrders = orders.Where(o => o.Flagged == true).ToList();

                //foreach (var order in flaggedOrders)
                //{
                //    Console.WriteLine($"Order ID: {order.OrderId}, Amount: {order.Amount}, Flagged: {order.Flagged}");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        // Add user to the database
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)", con);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs();    // Optional: clears the textboxes
                        LoadData();       // Refresh DataGridView
                        LoadUserCount();
                    }
                    else
                    {
                        MessageBox.Show("No user was added. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoadData();
                  

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
        }

        // Update user in the database
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Users SET Username = @Username, Password = @Password, Email = @Email WHERE UserID = @UserID", con);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to update.");
            }
        }

        // Delete user from the database
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", con);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }

        // Display data in textboxes when a row is clicked
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = row.Cells["Password"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
            }
        }

        private void LoadUserCount()
        {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetUserCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    con.Close();

                    txtUserCount.Text = count.ToString();
                }

        }
        //
        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure the user is selected and input fields are filled
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtAmount.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                // Get the UserID for the selected user (from the DataGridView)
                int userId = GetUserIDByUsername(txtUsername.Text);

                if (userId == -1)
                {
                    MessageBox.Show("User not found.");
                    return;
                }

                // Insert the new order into the Orders table
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Orders (UserID, OrderDate, Amount) VALUES (@UserID, @OrderDate, @Amount)", con);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@OrderDate", dtpOrderDate.Value);
                    cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Order added successfully!");

                    // Optionally, refresh the data to reflect the new order
                    LoadData(); // Reload users and orders (if required)
                    FlagHighValueOrders();
                    UpdateTotalSum();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding order: " + ex.Message);
            }
        }
        //getuserbyid
        private int GetUserIDByUsername(string username)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE Username = @Username", con);
                cmd.Parameters.AddWithValue("@Username", username);

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return -1; // User not found
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        //
        public void FlagHighValueOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Update high-value orders
                    string updateSql = "UPDATE Orders SET Flagged = 1 WHERE Amount >= 1000";
                    SqlCommand updateCmd = new SqlCommand(updateSql, conn, transaction);
                    int affectedRows = updateCmd.ExecuteNonQuery();

                    // 2. Log action (simulate audit/log table)
                    string logSql = "INSERT INTO AuditLog (Action, Timestamp) VALUES (@Action, GETDATE())";
                    SqlCommand logCmd = new SqlCommand(logSql, conn, transaction);
                    logCmd.Parameters.AddWithValue("@Action", $"Flagged {affectedRows} high-value orders.");
                    logCmd.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("Flagging complete and logged.");
                }


                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Failed: " + ex.Message);
                }
            }
        }

        private List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            string connectionString = "Server= .; Database=UserDB; Integrated Security=True;"; // Replace with actual connection string

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT OrderId, Amount, Flagged, UserId FROM Orders";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        Flagged = Convert.ToBoolean(reader["Flagged"]),
                        UserId = Convert.ToInt32(reader["UserId"])
                    });
                }
            }

            return orders;
        }


        private void LoadHighestAmount()
        {
            var orders = GetAllOrders();

            if (orders.Any())
            {
                var highestAmount = orders.Max(o => o.Amount);
                txtHighestAmount.Text = highestAmount.ToString("C"); // "C" for currency format (e.g., $1,000.00)
            }
            else
            {
                txtHighestAmount.Text = "No orders found.";
            }
        }

        private void UpdateTotalSum()
        {
            var orders = GetAllOrders();

            if (orders.Any())
            {
                decimal totalAmount = orders.Sum(o => o.Amount);
                textTotalAmount.Text = totalAmount.ToString("C"); // "C" = Currency format
            }
            else
            {
                textTotalAmount.Text = "No data.";
            }
        }



    }
}
