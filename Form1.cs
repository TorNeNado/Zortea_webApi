using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            public string DisplayInfo => $"{Nome} - {Prezzo:C}";
        }

        public class PizzeResponse
        {
            public List<Pizza> Pizze { get; set; }
        }


        private async Task<string> SendRequestAsync(HttpMethod method, string url, object data = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


        // TASTO GET (Carica Pizze)
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "http://localhost:5243/api/Pizze";
                string json = await SendRequestAsync(HttpMethod.Get, url);

                // Deserializzazione come da tua richiesta specifica
                pizzeResponse = JsonSerializer.Deserialize<PizzeResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                listBox1.DataSource = null; // Reset necessario per aggiornare la vista

                if (pizzeResponse?.Pizze != null)
                {
                    listBox1.DataSource = pizzeResponse.Pizze;
                    listBox1.DisplayMember = "DisplayInfo";
                    listBox1.ValueMember = "Id"; 
                }
                else
                {
                    MessageBox.Show("Nessuna pizza trovata o struttura JSON diversa da { \"pizze\": [] }");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is Pizza pizzaSelezionata)
            {
                // Riempie area Modifica
                textBox2.Text = pizzaSelezionata.Id.ToString();
                textBox3.Text = pizzaSelezionata.Nome;
                numericUpDown2.Value = pizzaSelezionata.Prezzo;

                // Riempie area Elimina
                textBox4.Text = pizzaSelezionata.Id.ToString();
            }
        }

        // TASTO POST (Aggiungi)
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var nuovaPizza = new Pizza
                {
                    Nome = textBox1.Text,
                    Prezzo = numericUpDown1.Value
                };

                await SendRequestAsync(HttpMethod.Post, "http://localhost:5243/api/Pizze", nuovaPizza);

                MessageBox.Show("Pizza aggiunta!");
                textBox1.Text = "";
                numericUpDown1.Value = 0;

                button1_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore POST: {ex.Message}");
            }
        }

        // TASTO PUT (Modifica)
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var pizzaAggiornata = new Pizza
                {
                    Id = int.Parse(textBox2.Text),
                    Nome = textBox3.Text,        
                    Prezzo = numericUpDown2.Value
                };

                string url = $"http://localhost:5243/api/Pizze/{pizzaAggiornata.Id}";
                await SendRequestAsync(HttpMethod.Put, url, pizzaAggiornata);

                MessageBox.Show("Pizza aggiornata!");
                button1_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore PUT: {ex.Message}");
            }
        }

        // TASTO DELETE (Elimina)
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox4.Text)) return;

                int id = int.Parse(textBox4.Text);
                string url = $"http://localhost:5243/api/Pizze/{id}";

                await SendRequestAsync(HttpMethod.Delete, url);

                MessageBox.Show("Pizza eliminata!");

                textBox4.Text = "";
                button1_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore DELETE: {ex.Message}");
            }
        }
    }
}