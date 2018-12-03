﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OEV_Applikation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //Speichert den in string convertierten Inhalt der Eingabe in Variablen ab
            string startStation = txtStartstation.Text;
            string endStation = txtEndstation.Text;
            string date = dtpDate.Value.ToString("yyyy-MM-dd");
            string time = nbrHour.Value + ":" + nbrMinute.Value;

            //Erstellte ein Objekt der Klasse Transport nach Vorgaben der Struktur ITransport
            SwissTransport.Transport StationConnetions = new SwissTransport.Transport();
            try
            {
                //Erstellt eine Liste welche Objekte der Klasse Station beinhaltet, welche das Wort (Parameter) im Namen der Station enthält
                List<SwissTransport.Station> start = StationConnetions.GetStations(startStation).StationList;
                List<SwissTransport.Station> end = StationConnetions.GetStations(endStation).StationList;

                //Erstellt eine Liste welche Objekte der Klasse Connection beinhaltet, welche die Verbindungen zwischen zwei Stationen darstellt
                List<SwissTransport.Connection> startEndConnection = StationConnetions.GetConnectionsByDateTime(startStation, endStation, date, time).ConnectionList;

                dgvOutput.Rows.Clear();

                if (startEndConnection.Count != 0)
                {
                    foreach (SwissTransport.Connection connection in startEndConnection)
                    {
                        dgvOutput.Rows.Add(connection.From.Station.Name, TimeStampToTime(connection.From.DepartureTimestamp),
                            connection.From.Platform, TimeStampToTime(connection.To.ArrivalTimestamp), connection.To.Station.Name);
                    }
                }
                else
                {
                    MessageBox.Show("Keine Verbindung gefunden.\nÜberprüfen Sie Ihre Eingabe.");
                }
            }
            catch
            {
                MessageBox.Show("Bitte Überprüfen Sie Ihre Internetverbindung.");
            }

        }

        private void btnSuggestion_Click(object sender, EventArgs e)
        {
            string startStation = txtStartstation.Text;
            string endStation = txtEndstation.Text;

            SwissTransport.Transport StationConnetions = new SwissTransport.Transport();

            List<SwissTransport.Station> start = StationConnetions.GetStations(startStation).StationList;
            List<SwissTransport.Station> end = StationConnetions.GetStations(endStation).StationList;

            lstStartstation.Items.Clear();
            lstEndstation.Items.Clear();

            foreach (SwissTransport.Station station in start)
            {
                lstStartstation.Items.Add(station.Name);
            }

            foreach (SwissTransport.Station station in end)
            {
                lstEndstation.Items.Add(station.Name);
            }
        }

        private void lstEndstation_Click(object sender, EventArgs e)
        {
            if(lstEndstation.Items.Count != 0)
            {
                txtEndstation.Text = lstEndstation.SelectedItem.ToString();
            }
        }

        private void lstStartstation_Click(object sender, EventArgs e)
        {
            if(lstStartstation.Items.Count != 0)
            {
                txtStartstation.Text = lstStartstation.SelectedItem.ToString();
            }
        }

        private void btnSuggestionDelete_Click(object sender, EventArgs e)
        {
            ClearAllTabOne();
        }

        /// <summary>
        /// Wandelt den unix-Timestamp in System.DateTime
        /// </summary>
        /// <param name="unixTime">Übergabe des unix-Timestamps</param>
        /// Gibt die umgewandelte Zeit in {Hour:Minute} als string zurück
        /// <returns></returns>
        public string TimeStampToTime(string unixTime)
        {
            //https://coderwall.com/p/e8rzuq/how-to-convert-a-unix-timestamp-to-a-net-system-datetime-object
            double h = Convert.ToDouble(unixTime);
            System.DateTime s = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            s = s.AddSeconds(h);
            string k = s.ToString("H:mm");
            return k;
        }

        private void ClearAllTabOne()
        {
            txtEndstation.Clear();
            txtStartstation.Clear();
            lstStartstation.Items.Clear();
            lstEndstation.Items.Clear();
            dgvOutput.Rows.Clear();
            dtpDate.ResetText();
            nbrHour.Value = DateTime.Now.Hour;
            nbrMinute.Value = DateTime.Now.Minute;
        }

        private void ClearAllTabTwo()
        {
            txtStation.Clear();
            dgvOutputStation.Rows.Clear();
            lstSuggestionsStation.Items.Clear();
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            nbrHour.Value = DateTime.Now.Hour;
            nbrMinute.Value = DateTime.Now.Minute;
        }

        private void btnSearchStation_Click(object sender, EventArgs e)
        {
            string station = txtStation.Text;

            SwissTransport.Transport StationConnetions = new SwissTransport.Transport();
            List<SwissTransport.Station> stationList = StationConnetions.GetStations(station).StationList;
            if(stationList.Count != 0)
            {
                //Enthält das erste Objekt der Klasse Station von der Liste stationList
                SwissTransport.Station UsedStation = stationList[0];

                //Erstellt eine Liste welche Objekte der Klasse StationBoard beinhaltet, welche mit dem Namen und der Id der UsedStation übereinstimmen
                List<SwissTransport.StationBoard> EntriesConnections = StationConnetions.GetStationBoard(UsedStation.Name, UsedStation.Id).Entries;

                //Enthält das Objekt der Klasse Station, welches mit dem Namen und der Id der UsedStation übereinstimmt
                SwissTransport.Station EntriesStation = StationConnetions.GetStationBoard(UsedStation.Name, UsedStation.Id).Station;

                if (EntriesConnections.Count != 0)
                {
                    foreach (SwissTransport.StationBoard stationBoard in EntriesConnections)
                    {
                        dgvOutputStation.Rows.Add(EntriesStation.Name, stationBoard.To, stationBoard.Stop.Departure.ToShortTimeString(), stationBoard.Category, stationBoard.Number);
                    }
                }
                else
                {
                    messageError();
                }
            }
            else
            {
                messageError();
            }
        }

        //Enthält die Ausgabe bei einer fehlerhaften Eingabe
        private void messageError()
        {
            MessageBox.Show("Keine Anschlüsse gefunden.\nÜberprüfen Sie Ihre Eingabe.");
        }

        private void tbcChangeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Bestimmen auf welcher Ansicht der Benutzer ist, um den AcceptButton und den CancelButton zu setzen
            if(tbcChangeView.SelectedTab == sdStation)
            {
                AcceptButton = btnSearchStation;
                CancelButton = btnSuggestionsDeleteTab2;
            }
            else
            {
                AcceptButton = btnSearch;
                CancelButton = btnSuggestionDelete;
            }
        }

        private void btnSuggestionsDeleteTab2_Click(object sender, EventArgs e)
        {
            ClearAllTabTwo();
        }

        private void btnSuggestionStation_Click(object sender, EventArgs e)
        {
            string station = txtStation.Text;

            SwissTransport.Transport stationConnetions = new SwissTransport.Transport();
            List<SwissTransport.Station> stationList = stationConnetions.GetStations(station).StationList;

            foreach(SwissTransport.Station oneStation in stationList)
            {
                lstSuggestionsStation.Items.Add(oneStation.Name);
            }
        }

        private void lstSuggestionsStation_Click(object sender, EventArgs e)
        {
            if (lstSuggestionsStation.Items.Count != 0)
            {
                txtStation.Text = lstSuggestionsStation.SelectedItem.ToString();
            }
        }
    }
}
