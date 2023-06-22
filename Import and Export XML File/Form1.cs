using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Json.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using SpotifyAPI.Web;


namespace Import_and_Export_XML_File
{
    public partial class Form1 : Form
    {
        DataSet ds = new DataSet();
        DataTable dataTable = new DataTable();
        DataTable apidata;
        int indeks=0;
        DataColumn column;
        int playlist_length = 0;
        int list_initialized = 0;
        DataTable list = new DataTable();
        dbconnections dbc= new dbconnections();


        public Form1()
        {
            InitializeComponent();
        }

        private void import_Click(object sender, EventArgs e)//import xml
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Wybierz plik";
            openFileDialog1.Filter = "All files (*.*)|*.*|XML File (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                ds.ReadXml(openFileDialog1.FileName);
                dataTable = ds.Tables[0];
                dataGridView1.DataSource = ds.Tables[0];
                MessageBox.Show("XML Zaimportowany");
            }
        }

        private void export_Click(object sender, EventArgs e)//export xml
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Zapisz plik";
            sfd.Filter = "XML File (*.xml)|*.xml";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                ds.WriteXml(sfd.FileName);
                MessageBox.Show("Plik zapisany!");
            }
        }
        private void button1_Click(object sender, EventArgs e)//import json
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Wybierz plik";
            openFileDialog1.Filter = "All files (*.*)|*.*|Json files (.json) |.json";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {

                string jj = File.ReadAllText(openFileDialog1.FileName);
                dataTable = (DataTable)JsonConvert.DeserializeObject(jj, (typeof(DataTable)));
                ds.Tables.AddRange(new DataTable[] { dataTable });
                dataGridView1.DataSource = dataTable;
                MessageBox.Show("Zaimportowano");
            }
        }

        private void button2_Click(object sender, EventArgs e)//export json
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Zapisz plik";
            sfd.Filter = "JSON File (.json)|.json";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dataTable);
                File.WriteAllText(sfd.FileName, JSONString);
                MessageBox.Show("Plik zapisany!");
            }
        }

        private void button3_Click(object sender, EventArgs e)//Clear
        {
            ds.Tables.Clear();
            dataTable = new DataTable();
            apidata = new DataTable();
            dataGridView1.DataSource = dataTable;
        }

        private async void button4_Click(object sender, EventArgs e)//import from API
        {

            //OAuth2.0
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator("6e91657d75c84b92be4550a557dee03e", "1f1c0c14b66e4970a16b1028e0e2e865"));
            //RESTAPI

            var spotify = new SpotifyClient(config);

            //var track = await spotify.Tracks.Get("1V8aPypxFrIk5TSa8E3qM0");
            //var album = await spotify.Albums.Get("37i9dQZF1DX49bSMRljsho");
            Paging<SimpleTrack> tracks = await spotify.Albums.GetTracks("2Kh43m04B1UkVcpcRa1Zug");
            //MessageBox.Show(track.Artists[0].Name + " - " + track.Name + " - " + track.DurationMs);
            //string nazwy = "";
            //tracks.Items.ForEach(item => { nazwy = nazwy + item.Name + " "; });
            //MessageBox.Show(nazwy);

            //api to datatable
            apidata = new DataTable();
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "title";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            apidata.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "Artist";
            apidata.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "Artwork";
            apidata.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "url";
            apidata.Columns.Add(column);
            column = new DataColumn();
            dataGridView1.DataSource = apidata;
            //apidata.Rows.Add("wer","wer","wer","dfg",4);
            playlist_length = 0;
            tracks.Items.ForEach(item =>
            {
                apidata.Rows.Add(item.Name, item.Artists[0].Name, "place_holder_url", "place_holder_url");
                playlist_length += item.DurationMs;
            });
            MessageBox.Show("Dlugosc playlisty " + (playlist_length / 1000)/60 + "min " + (playlist_length / 1000) % 60 + " sek");
            //exportowanie
            dataTable = apidata;
            if (ds.Tables.CanRemove(dataTable))
            {
                ds.Tables.Remove(dataTable);
            }
            else
            {
                ds.Tables.AddRange(new DataTable[] { dataTable });
            }
        }

        private void button5_Click(object sender, EventArgs e)//tolist
        {
            dataGridView2.DataSource = list;
            if (list_initialized==0) {
                column = new DataColumn();
                column.ColumnName = "title";
                column.ReadOnly = false;
                column.Unique = false;
                list.Columns.Add(column);
                list_initialized = 1;
            }
            for (int i = 0;i<dataTable.Rows.Count;i++)
            {
                DataRow rw = list.AsEnumerable().FirstOrDefault(tt => tt.Field<string>("title") == dataTable.Rows[i].Field<string>(0));
                if (rw == null)
                {
                    list.Rows.Add(dataTable.Rows[i].Field<string>(0));
                }
            }
            MessageBox.Show("Dodano do listy nowe utwory");

        }

        private void button6_Click(object sender, EventArgs e)//copy to main list
        {
            dataTable = list;
            if (ds.Tables.CanRemove(dataTable))
            {
                ds.Tables.Remove(dataTable);
            }else
            {
                ds.Tables.AddRange(new DataTable[] { dataTable });
            }
            dataGridView1.DataSource = dataTable;
        }

        private void db_export_Click(object sender, EventArgs e)
        {
            dbc.export_to_database(dataTable);
        }

        private void db_import_Click(object sender, EventArgs e)
        {
            dataTable = dbc.import_from_database();
            if (ds.Tables.CanRemove(dataTable))
            {
                ds.Tables.Remove(dataTable);
            }
            else
            {
                ds.Tables.AddRange(new DataTable[] { dataTable });
            }
            dataGridView1.DataSource = dataTable;
        }
    }
}
