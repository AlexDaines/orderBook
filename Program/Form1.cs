using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        static HttpClient client = new HttpClient();
        static Uri bnbURI = new Uri("https://api.binance.com/");
        static Uri ftxURI = new Uri("https://ftx.com/api/");
        
        public class BNBpage
        {
            public float lastUpdateId { get; set; }
            public List<List<string>> bids { get; set; }
            public List<List<string>> asks { get; set; }
        }
        public class Result
        {
            public List<List<string>> bids { get; set; }
            public List<List<string>> asks { get; set; }
        }

        public class FTXpage
        {
            public bool success { get; set; }
            public Result result { get; set; }
        }

        public Form1()
        {
            InitializeComponent(); 
            client.DefaultRequestHeaders.Accept.Clear();
            Console.WriteLine("added api key");
            // Add Binance API Key.
            client.DefaultRequestHeaders.Add("X-MBX-APIKEY", "INSERT-API-KEY");
            client.DefaultRequestHeaders.Add("FTX-KEY", "INSERT-API-KEY");
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {


        }
        public string ticker
        {
            get { return textBox1.Text; }
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
                listView1.View = View.Details;
                listView2.View = View.Details;
                
                disp();
            }
        }
        private async void disp()
        {

            listView1.SelectedItems.Clear();
            Console.WriteLine(textBox1.Text);
            await exec(textBox1.Text);

            // Create sorter that knows how to sort ListViewItems
            Sorter sor = new Sorter();
            listView1.ListViewItemSorter = sor;
            listView2.ListViewItemSorter = sor;
            

            //Thread.Sleep(3500);
            //disp();
        }
        static async Task<string> getBNBBook(string ticker)
        {
            string link = "https://api.binance.com/api/v3/depth?symbol=" + ticker + "USDT";
            HttpResponseMessage response = await client.GetAsync(link);
            string outStr = await response.Content.ReadAsStringAsync();
            return outStr;

        }
        static async Task<string> getFTXBook(string ticker)
        {
            string link = "https://ftx.com/api/markets/" + ticker + "-PERP/orderbook";
            HttpResponseMessage response = await client.GetAsync(link);
            var idk = response.Content;
            string outStr = await response.Content.ReadAsStringAsync();
            Console.WriteLine("FTX");
            Console.WriteLine(outStr);
            return outStr;
        }
        async Task<int> exec(string ticker)
        {
            string ftxBook = await getFTXBook(ticker);
            string bnbBook = await getBNBBook(ticker);
            
            // Deserialize string into object handlers.
            BNBpage desBNB = JsonConvert.DeserializeObject<BNBpage>(bnbBook);
            FTXpage desFTX = JsonConvert.DeserializeObject<FTXpage>(ftxBook);
            await addBNB_ask(desBNB);
            await addFTX_ask(desFTX);
            await addBNB_bid(desBNB);
            await addFTX_bid(desFTX);
            
            return 0;
        }

        async Task<ListViewItem> addBNB_ask(BNBpage bk)
        {
            ListViewItem temp = new ListViewItem();
            for (int i = 0; i < 20; i++)
            {
                ListViewItem test1 = new ListViewItem(bk.asks[i][0]);
                test1.SubItems.Add(bk.asks[i][1]);
                test1.SubItems.Add("BNB");
                listView1.Items.Add(test1);
                temp = test1;
            }
            return temp;          
        }
        async Task<ListViewItem> addFTX_ask(FTXpage book)
        {
            ListViewItem temp = new ListViewItem();
            for (int i = 0; i < 20; i++)
            {
                ListViewItem test1 = new ListViewItem(book.result.asks[i][0]);
                test1.SubItems.Add(book.result.asks[i][1]);
                test1.SubItems.Add("FTX");
                listView1.Items.Add(test1);
            }
            return temp;
        }
        async Task<ListViewItem> addBNB_bid(BNBpage bk)
        {
            ListViewItem temp = new ListViewItem();
            for (int i = 0; i < 20; i++)
            {
                ListViewItem test1 = new ListViewItem(bk.bids[i][0]);
                test1.SubItems.Add(bk.bids[i][1]);
                test1.SubItems.Add("BNB");
                listView2.Items.Add(test1);
                temp = test1;
            }
            return temp;
        }
        async Task<ListViewItem> addFTX_bid(FTXpage book)
        {
            ListViewItem temp = new ListViewItem();
            for (int i = 0; i < 20; i++)
            {
                ListViewItem test1 = new ListViewItem(book.result.bids[i][0]);
                test1.SubItems.Add(book.result.bids[i][1]);
                test1.SubItems.Add("FTX");
                listView2.Items.Add(test1);
                temp = test1;
            }
            return temp;
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
         
        }
        class Sorter : System.Collections.IComparer
        {
            public int Column = 0;
            public System.Windows.Forms.SortOrder Order = SortOrder.Ascending;
            public int Compare(object x, object y) // IComparer Member
            {
                if (!(x is ListViewItem))
                    return (0);
                if (!(y is ListViewItem))
                    return (0);

                ListViewItem l1 = (ListViewItem)x;
                ListViewItem l2 = (ListViewItem)y;

                float fl1 = float.Parse(l1.SubItems[Column].Text);
                float fl2 = float.Parse(l2.SubItems[Column].Text);

                //TODO: Add column click event to switch to decending, because why not. 
                if (Order == SortOrder.Ascending)
                {
                    return fl1.CompareTo(fl2);
                }
                else
                {
                    return fl2.CompareTo(fl1);
                } 
            }
        }
    }
}
