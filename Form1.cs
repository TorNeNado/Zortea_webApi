using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Tecnologie
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private PizzeResponse pizzeResponse;

        public Form1()
        {
            InitializeComponent();
        }

        public class Pizza
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public decimal Prezzo { get; set; }
        }

        public class PizzeResponse
        {
            public List<Pizza> Pizze { get; set; }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string json = await GetDataAsync("http://localhost:5243/api/Pizze");

                
                pizzeResponse = JsonSerializer.Deserialize<PizzeResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                listBox1.Items.Clear();

                if (pizzeResponse?.Pizze != null)
                {
                    foreach (var pizza in pizzeResponse.Pizze)
                    {
                        listBox1.Items.Add($"{pizza.Nome} - {pizza.Prezzo}€");
                    }
                }
                else
                {
                    MessageBox.Show("Nessuna pizza trovata!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il parsing JSON: {ex.Message}");
            }
        }

        public async Task<string> GetDataAsync(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostDataAsync(string url, object data)
        {
            // Serializza l’oggetto in JSON
            var json = JsonSerializer.Serialize(data);

            // Prepara il contenuto da inviare
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Manda la richiesta POST
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            // Verifica se è andata a buon fine
            response.EnsureSuccessStatusCode();

            // Ritorna la risposta come stringa
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PutDataAsync(string url, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task DeleteDataAsync(string url)
        {
            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var nuovaPizza = new Pizza
                {
                    Nome = textBox1.Text,
                    Prezzo = decimal.Parse(numericUpDown1.Value.ToString())
                };

                string url = "http://localhost:5243/api/Pizze";

                string response = await PostDataAsync(url, nuovaPizza);

                MessageBox.Show("Pizza aggiunta con successo!");

                button1_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la POST: {ex.Message}");
            }

            textBox1.Text = string.Empty;
            numericUpDown1.Value = 0;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var pizzaAggiornata = new Pizza
                {
                    Id = int.Parse(textBox2.Text),
                    Nome = textBox2.Text,
                    Prezzo = decimal.Parse(numericUpDown2.Text)
                };

                string url = $"http://localhost:5243/api/Pizze/{pizzaAggiornata.Id}";

                string response = await PutDataAsync(url, pizzaAggiornata);

                MessageBox.Show("Pizza aggiornata con successo!");
                button1_Click(sender, e); // Ricarica la lista
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la PUT: {ex.Message}");
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(textBox4.Text);
                string url = $"http://localhost:5243/api/Pizze/{id}";

                await DeleteDataAsync(url);

                MessageBox.Show("Pizza eliminata con successo!");
                button1_Click(sender, e); // Ricarica la lista
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la DELETE: {ex.Message}");
            }
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            // inserisce informazioni della pizza selezionata nel put
            textBox2.Text = pizzeResponse.Pizze[index].Id.ToString();
            textBox3.Text = pizzeResponse.Pizze[index].Nome;
            numericUpDown2.Value = pizzeResponse.Pizze[index].Prezzo;

            // inserisce informazioni della pizza selezionata nel delete
            textBox4.Text = pizzeResponse.Pizze[index].Id.ToString();

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
