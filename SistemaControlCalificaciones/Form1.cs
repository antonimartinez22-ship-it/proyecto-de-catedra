using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SistemaControlCalificaciones
{
    public partial class Form1 : Form
    {
        // Cadena de conexión tomada de App.config
        private readonly string conexion =
             ConfigurationManager.ConnectionStrings["cn"].ConnectionString;



        public Form1()
        {
            InitializeComponent();

            btnAgregar.Click += btnAgregar_Click;
            btnCalcular.Click += btnCalcular_Click;
            btnEliminar.Click += btnEliminar_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListarEstudiantes();
        }

        // ---------- MÉTODOS ----------

        // Guarda en la base de datos y recarga la tabla
        private void AgregarEstudiante()
        {
            if (txtNombre.Text.Trim() == "" || txtNota.Text.Trim() == "")
            {
                MessageBox.Show("Debe ingresar nombre y nota.");
                return;
            }

            if (!double.TryParse(txtNota.Text, out double nota) || nota < 0 || nota > 100)
            {
                MessageBox.Show("La nota debe ser un número entre 0 y 100.");
                return;
            }

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_InsertarEstudiante", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Nota", nota);
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Datos guardados correctamente.");
            txtNombre.Clear();
            txtNota.Clear();
            txtNombre.Focus();
            ListarEstudiantes();
        }

        // Carga los datos en el DataGridView
        private void ListarEstudiantes()
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter("sp_ListarEstudiantes", cn))
                {
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.DefaultView.Sort = "Nota DESC";
                    dgvEstudiantes.DataSource = dt.DefaultView;
                    dgvEstudiantes.DataSource = dt;
                }
            }
        }

        // Usa ARREGLOS + CICLOS + DECISIONES para calcular todo
        private void CalcularResultados()
        {
            if (dgvEstudiantes.Rows.Count == 0)
            {
                MessageBox.Show("No hay estudiantes registrados.");
                return;
            }

            // 1) Recorremos el DataGridView (esto es nuestro "arreglo" de notas)
            int total = dgvEstudiantes.Rows.Count;
            double[] notas = new double[total];

            for (int i = 0; i < total; i++)
            {
                notas[i] = Convert.ToDouble(dgvEstudiantes.Rows[i].Cells["Nota"].Value);
            }

            // 2) Cálculos con ciclos y decisiones
            double suma = 0, mayor = notas[0], menor = notas[0];
            int aprobados = 0;

            for (int i = 0; i < notas.Length; i++)
            {
                suma += notas[i];                      // ciclo acumulador

                if (notas[i] > mayor) mayor = notas[i]; // decisión
                if (notas[i] < menor) menor = notas[i]; // decisión
                if (notas[i] >= 70) aprobados++;        // aprobado con 70 o más
            }

            double promedio = suma / notas.Length;

            // 3) Mostramos resultados
            lblResultados.Text =
                $"Total estudiantes: {total}\n" +
                $"Promedio: {promedio:F2}\n" +
                $"Nota mayor: {mayor:F2}\n" +
                $"Nota menor: {menor:F2}\n" +
                $"Aprobados: {aprobados}\n" +
                $"Reprobados: {total - aprobados}";

            MessageBox.Show("Cálculo realizado correctamente.");
        }

        private void EliminarEstudiante()
        {
            if (dgvEstudiantes.CurrentRow == null) return;

            int id = Convert.ToInt32(
                dgvEstudiantes.CurrentRow.Cells["IdEstudiante"].Value);

            if (MessageBox.Show("¿Eliminar este estudiante?", "Confirmar",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_EliminarEstudiante", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdEstudiante", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                ListarEstudiantes();
                btnCalcular.PerformClick();
                MessageBox.Show("Datos borrados correctamente.");
            }
        }

        // ---------- EVENTOS ----------

        private void btnAgregar_Click(object sender, EventArgs e) => AgregarEstudiante();
        private void btnCalcular_Click(object sender, EventArgs e) => CalcularResultados();
        private void btnEliminar_Click(object sender, EventArgs e) => EliminarEstudiante();

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
