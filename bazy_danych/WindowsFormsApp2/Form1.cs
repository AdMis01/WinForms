using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        //string conn = "Data Source = DESKTOP-VKSCRU9\\SQLEXPRESS;Initial Catalog = student;Integranted Security = true";
        public static string server_name = "DESKTOP-VKSCRU9\\SQLEXPRESS";
        public static string database_name = "student";
        public static string nazwa_tabeli = "studenci";
        SqlConnection con = new SqlConnection($"data source={server_name}; database={database_name}; integrated security=SSPI");
        

        public static string server_name2 = "(localdb)\\MSSQLLocalDB";
        public static string database_name2 = "bazaDanych";
        public static string nazwa_tabeli2 = "ucz";
        SqlConnection con2 = new SqlConnection($"Data Source={server_name};Initial Catalog={database_name};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");


        public Form1()
        {
            InitializeComponent();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();//otworzenie połączenia 

            string query = $"Select * from {nazwa_tabeli}";//stworzenie stringa z zapytaniem 
            SqlCommand cmd = new SqlCommand(query, con);//wysłanie zapytania do bazy danych 
            SqlDataReader reader;
            reader = cmd.ExecuteReader();//odczytanie wyników zapytania

            while (reader.Read())//jeżeli jest następny wiesz do odczytu to idze 
            {
                listBox1.Items.Add(reader["Nazwisko"]);
            }
            reader.Close();//zamkniecie 
            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con.Open();

            String q1 = $"Select * from {nazwa_tabeli} Where wiek = 20";
            String q2 = $"Select * from {nazwa_tabeli} Where wiek = 21";
            SqlCommand cmd = new SqlCommand();//stworzenie obiektu komendowego
            cmd.CommandText = q1 + ";" + q2;//połączenie dwóch zapytań
            cmd.CommandTimeout = 15;//ustanie oczekiwania czasu/na wygaśnięcie 
            cmd.CommandType = CommandType.Text;//ustalenie typy komenty
            cmd.Connection = con;//dokąd ma być wysłane zapytanie 
            SqlDataReader rdr = cmd.ExecuteReader();
            bool readNext = true;
            while (readNext)
            {
                while (rdr.Read())
                {
                    MessageBox.Show(rdr.GetString(1));
                }
                readNext = rdr.NextResult();
            }
            rdr.Close();
            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            string cmdText = $"Select Nazwisko,Imie From {nazwa_tabeli}";
            SqlCommand cmd = new SqlCommand(cmdText, con);
            SqlDataReader rdr;
            rdr = cmd.ExecuteReader();
            rdr.Read();
            string tytul;
            tytul = (string)rdr.GetString(1);
            MessageBox.Show(tytul);
            tytul = (string)rdr.GetValue(1);
            MessageBox.Show(tytul);
            tytul = (string)rdr["Imie"];
            MessageBox.Show(tytul);
            tytul = (string)rdr[1];
            MessageBox.Show(tytul);

            //pobieranie nazw kolumn
            for (int k = 0; k < rdr.FieldCount; k++)
            {
                MessageBox.Show(rdr.GetName(k));
            }
            rdr.Close();
            con.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //wyswietal dane o kazdej kolumnie z kazdego wiersz || jednak to jest bardzo dlugie bo pokazuje kazda nazwe kolumny i jej indeks
            con.Open();
            string cmdText = $"Select * From {nazwa_tabeli}";
            SqlCommand cmd = new SqlCommand(cmdText, con);
            SqlDataReader rdr;
            rdr = cmd.ExecuteReader();
            DataTable schemat_tabeli = rdr.GetSchemaTable();
            int licznik = 0;
            foreach (DataRow r in schemat_tabeli.Rows)
            {
                foreach (DataColumn c in schemat_tabeli.Columns)
                {
                    MessageBox.Show(licznik.ToString() + " " + c.ColumnName + ": " + r[c]);
                }
                licznik++;
            }
            con.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            XmlReader xml = XmlReader.Create(@"C:\Users\Dell\Desktop\zad.xml");//tworzenie xml

            DataSet kontener = new DataSet();

            DataColumn kluczGlowny;
            DataColumn kluczObcy;

            kluczGlowny = kontener.Tables["znajomi"].Columns["pesel"];
            kluczObcy = kontener.Tables["ksiazki"].Columns["pesel_w"];


            ForeignKeyConstraint organ = new ForeignKeyConstraint(kluczGlowny, kluczObcy);
            kontener.Tables["ksiazki"].Constraints.Add(organ);


            kontener.ReadXml(@"C:\Users\Dell\Desktop\zad.xml");//wczytanie pliku xml i stworzenie z tego tabeli w dataset

            MessageBox.Show("Liczba tabel w obiekcie dataset po wczytanie dokumentu xml: " + kontener.Tables.Count.ToString());

            string infot = "Tabele w obiekcie po wykorzystaniu metidy readxml obiektu dataset\n";
            foreach(DataTable tab1 in kontener.Tables)//wykona sie dla kazdej tabeli 
            {
                infot += tab1.TableName + "\n" + "liczba wierszy " + tab1.Rows.Count.ToString() + "\n";
            }
            MessageBox.Show(infot);
            
            dataGridView1.DataSource = kontener;
            //dataGridView1.DataMember = "znajmow_miejscowosc";

            //con.Open();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand rozkaz = new SqlCommand();
            rozkaz.Connection = con;
            rozkaz.CommandTimeout = 15;
            rozkaz.CommandType = CommandType.StoredProcedure;
            rozkaz.CommandText = $"dbo.{nazwa_tabeli}";

            SqlParameter par1 = new SqlParameter();
            par1.ParameterName = "@id";
            par1.DbType = DbType.Int32;
            par1.Direction = ParameterDirection.Input;
            par1.Value = 4;//ustalenie wartości parametru
            rozkaz.Parameters.Add(par1);//dodanie do zbioru parametrów w zapytaniu 
            rozkaz.Parameters.Add(new SqlParameter("@Returnvalue", DbType.Int32));
            rozkaz.Parameters["@Returnvalue"].Direction = ParameterDirection.ReturnValue;


            string parw = "Parametry tworzace koleckje: \n";

            foreach(SqlParameter item in rozkaz.Parameters)
            {
                parw += item.ParameterName + "\n";
            }
            if (rozkaz.Parameters.Contains("@dane"))
            {
                MessageBox.Show("Parametry o nazwie @dane jest w kolekcji");
            }
            MessageBox.Show(parw);


            MessageBox.Show("wartosc zwracana: " + rozkaz.Parameters["@Returnvalue"].Value.ToString());

            rozkaz.Parameters.Remove(par1);
            rozkaz.Parameters.RemoveAt(0);

            MessageBox.Show("liczba parametow " + rozkaz.Parameters.Count.ToString());
            rozkaz.CommandText = "odb.obliczenia";


            SqlParameter par2 = new SqlParameter();
            par2.ParameterName = "@Nazwisko";
            par2.DbType = DbType.String;
            par2.Direction = ParameterDirection.Input;
            par2.Value = "Nowak";
            rozkaz.Parameters.Add(par2);

            SqlParameter par3 = new SqlParameter();
            par3.ParameterName = "@liczba";
            par3.DbType = DbType.Int32;
            par3.Direction = ParameterDirection.Output;
            rozkaz.Parameters.Add(par3);

            rozkaz.Parameters.Add(new SqlParameter("@ReturnVaule", DbType.Int32));
            rozkaz.Parameters["@ReturnVaule"].Direction = ParameterDirection.ReturnValue;

            //rozkaz.ExecuteNonQuery(); //nie wiem czemy nie dziala jak trzeba na poleceniu/commend asldkfjasdlkfjdlk
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            DataTable tabela1 = new DataTable("Nowa_tabela");
            DataColumn kolumna1 = new DataColumn("Nazwisko",Type.GetType("System.String"));
            kolumna1.AllowDBNull = true;

            tabela1.Columns.Add(kolumna1);

            kolumna1 = new DataColumn("Imie", Type.GetType("System.String"));
            tabela1.Columns.Add(kolumna1);

            kolumna1 = new DataColumn("Pesel", Type.GetType("System.Int16"));
            tabela1.Columns.Add(kolumna1);

            kolumna1 = new DataColumn("Adres", Type.GetType("System.String"));
            tabela1.Columns.Add(kolumna1);

            string inf = "";
            tabela1.Columns.Add("Imie", typeof(string));

            DataColumn kolumna2 = new DataColumn("Nazwisko");
            kolumna2.DataType = typeof(string);
            tabela1.Columns.Add(kolumna2);


            DataColumn kolw = tabela1.Columns.Add();
            kolw.ColumnName = "Nowa";
            kolw.DataType = typeof(int);
            kolw.AllowDBNull = true;
            kolw.ColumnMapping = MappingType.Element;

            foreach(DataColumn kol in tabela1.Columns)
            {
                inf = inf + kol.ColumnName + "\n";
            }
            MessageBox.Show(inf);
            DataColumn[] kol1 = { tabela1.Columns["Pesel"] };
            tabela1.PrimaryKey = kol1;

            DataRow wiersz;
            wiersz = tabela1.NewRow();
            wiersz["Nazwisko"] = "Kowalski";
            wiersz["Imie"] = "Jan";
            wiersz["Pesel"] = 1113;
            wiersz["Adres"] = "Poznan";
            tabela1.Rows.Add(wiersz);

            MessageBox.Show(tabela1.Rows [0]["Nazwisko"].ToString());
            DataSet zasob = new DataSet();
            zasob.Tables.Add(tabela1);
            MessageBox.Show(zasob.Tables[0].TableName.ToString());

            DataSet kontener = new DataSet();
            kontener.Tables["tabela1"].Clear();
            kontener.Relations.Remove("powiazanie1");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DataTable tabela1 = new DataTable("nowa tabela");
            DataColumn kolumna1 = new DataColumn("nazwisko",Type.GetType("System.String"));
            kolumna1.AllowDBNull = true;

            tabela1.Columns.Add(kolumna1);
            kolumna1 = new DataColumn("imie", Type.GetType("System.String"));
            tabela1.Columns.Add(kolumna1);
            kolumna1 = new DataColumn("pesel", Type.GetType("System.Int16"));
            tabela1.Columns.Add(kolumna1);
            kolumna1 = new DataColumn("adres", Type.GetType("System.String"));
            tabela1.Columns.Add(kolumna1);

            string inf = "";
            DataTable tab1 = new DataTable("tabela");
            tab1.Columns.Add("imie", typeof(string));

            DataColumn kol1 = new DataColumn("nazwisko");
            kol1.DataType = typeof(string);
            tab1.Columns.Add(kol1);

            DataColumn kolw = tab1.Columns.Add();
            kolw.ColumnName = "Nowa";
            kolw.DataType = typeof(int);
            kolw.AllowDBNull = true;
            kolw.ColumnMapping = MappingType.Element;

            //ta ostatnia wlasciwosc decyduje w jaki sposob zostana zapisane dane w dokumencie sml w postaci elementu

            foreach(DataColumn kol in tabela1.Columns)
            {
                inf = inf + kol.ColumnName + "\n";
            }
            MessageBox.Show(inf);

            //tworzenie klucza podstawowego

            DataColumn[] col1 = { tabela1.Columns["pesel"] };
            tabela1.PrimaryKey = col1;

            //dodawanie wierszy z danymi

            DataRow wiersz;
            wiersz = tabela1.NewRow();
            wiersz["nazwisko"] = "kowaliski";
            wiersz["imie"] = "jan";
            wiersz["pesel"] = 111;
            wiersz["adres"] = "poznan";
            tabela1.Rows.Add(wiersz);

            MessageBox.Show(tabela1.Rows[0]["nazwisko"].ToString());

            DataSet zasob = new DataSet();
            zasob.Tables.Add(tabela1);
            MessageBox.Show(zasob.Tables[0].TableName.ToString());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //dwa zapytania i jeden obiekt Data reader

            SqlConnectionStringBuilder konstr = new SqlConnectionStringBuilder();
            string wspo = "";
            konstr.DataSource = @"DELL";
            konstr.InitialCatalog = "student";
            konstr.IntegratedSecurity = true;

            SqlConnection pol3 = new SqlConnection(konstr.ConnectionString);
            SqlCommand polecenie2 = new SqlCommand();
            polecenie2.Connection = pol3;
            polecenie2.CommandType = CommandType.Text;
            //dwa rozne zapytania
            string zap1 = "Select * from studeci where stypendium < 1000";
            string zap2 = "Select * from studenci";

            polecenie2.CommandText = zap1 + " " + zap2;

            SqlDataAdapter pomost = new SqlDataAdapter();
            pomost.SelectCommand = polecenie2;

            // osobne jakies millera widzimisie nie wiem ale troche sie zagniezdza to przyklad 
            DataSet zbiornik = new DataSet();
            //SqlDataAdapter pomost = new SqlDataAdapter(); to
            //pomost.SelectCommand = polecenie2; jakies inne polecenie 
            pomost.Fill(zbiornik);

            // zamiast fill 
            //pomost.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            //pomost.TableMappings.Add("Table","Studenci");
            //pomost.Fill(zbiornik);



            MessageBox.Show("nazwa pierwszej tabeli" + zbiornik.Tables[0].TableName);
            DataTable tabela = zbiornik.Tables[0];
            foreach(DataColumn xx in tabela.Columns)
            {
                if(xx.Unique != true)
                {
                    wspo += xx.Caption + "\n";

                }
            }

            MessageBox.Show(zbiornik.Tables.Count.ToString(), "liczba tabel w dataset");
            MessageBox.Show(zbiornik.Tables[0].PrimaryKey.Count().ToString());
            MessageBox.Show(zbiornik.Tables[0].PrimaryKey[0].ColumnName);

        }

        private void button11_Click(object sender, EventArgs e)
        {
            DataSet kontener = new DataSet("zbiornik");
            // tworzenie obiektu datatable i dodawanie go do obiektu dataset

            MessageBox.Show("liczba obiektow datatable w dataset = " + kontener.Tables.Count.ToString());

            DataTable tab1 = new DataTable();
            tab1.TableName = "Moja tabela";

            DataColumn id = tab1.Columns.Add("id", typeof(int));
            tab1.Columns.Add("imie", typeof(string));
            DataColumn kolumna1 = new DataColumn("nazwisko");
            kolumna1.DataType = typeof(string);
            tab1.Columns.Add(kolumna1);


            tab1.PrimaryKey = new DataColumn[] { id };
            tab1.Rows.Add(new object[] { 1,"jan","kowlaski"});
            tab1.Rows.Add(new object[] { 2,"anna","nowak"});
            tab1.Rows.Add(new object[] { 3,"miller","kowlaski"});

            //stan wiersza po dadaniu ale przed uruchomieniem metody acceptchanges()

            MessageBox.Show("stan wiersza po jego utworzeniu i dodaniu go do kolekcji a przed zaakceptowaniem " + tab1.Rows[0].RowState.ToString());

            MessageBox.Show(tab1.Rows[0][1].ToString());
            DataRow wiersz;
            wiersz = tab1.NewRow();
            wiersz["id"] = 4;
            wiersz["imie"] = "leo";

            // stan wiersza przed dodaniem go do kolekcji - detached - oderwany

            //DataRowVersion.Current - brak

            MessageBox.Show("Wiersz dodany ale nie dodany jeszcze do kolekcji" + wiersz.RowState.ToString());
            MessageBox.Show("wartosc odczytana " + wiersz["imie", DataRowVersion.Proposed].ToString());
            MessageBox.Show("wartosc odczytana " + wiersz["imie", DataRowVersion.Default].ToString());

            var row1 = tab1.NewRow();//tworzenie wiersza i dodanie go do tabeli i sprawdzanie jaki stan ma
            row1.ItemArray = "5,Adam,Krzyk".Split(',').ToArray();
            tab1.Rows.Add(row1);
            tab1.Rows.Add(wiersz);
            tab1.Rows[4].AcceptChanges();
            MessageBox.Show("Wiersz dodany ale nie dodany jeszcze do kolekcji" + tab1.Rows[4].RowState);

            tab1.AcceptChanges();//akceptowanie zmiena
            MessageBox.Show("Wiersz dodany ale nie dodany jeszcze do kolekcji" + tab1.Rows[1].RowState);

            tab1.Rows[1].Delete();//usuniecie zmian

            MessageBox.Show("Wiersz dodany ale nie dodany jeszcze do kolekcji" + tab1.Rows[1].RowState);

            tab1.RejectChanges();// nie przyjęcie zmian 

            MessageBox.Show("Wiersz dodany ale nie dodany jeszcze do kolekcji" + tab1.Rows[1].RowState);


            //mozliwosc przerzucenia danych z wiesza do tablicy

            object[] daneww;
            daneww = tab1.Rows[2].ItemArray;

            MessageBox.Show("liczby elemnotw w tablicy" + daneww.Length.ToString());
            MessageBox.Show("wartosc" + daneww[1].ToString());

            //dodawanie do obiektu dataset

            kontener.Tables.Add(tab1);
            MessageBox.Show("Liczba obiektow datatable w dataset " + kontener.Tables.Count.ToString());

            //przeksztalcenie obiektu datateble w obiekt datareader

            DataTableReader zbior = kontener.CreateDataReader(tab1);
            string info = "";

            while (zbior.Read())
            {
                for(int i = 0; i < zbior.FieldCount; i++)
                {
                    info += zbior.GetName(i) + "\t" + zbior[i].ToString() + "\t";
                }
                info += "\n";
            }

            MessageBox.Show(info);
            kontener.Dispose();
        }

        private void button12_Click(object sender, EventArgs e)
        {

            // musisz miec utworzone tabelie baze z danymi i sie z tym polaczyc 
            SqlConnection pol4 = new SqlConnection();
            pol4.ConnectionString = "";
            pol4.Open();

            DataSet kontener = new DataSet();

            SqlDataAdapter most1 = new SqlDataAdapter();

            SqlCommand polecenie = new SqlCommand();
            polecenie.Connection = pol4;
            polecenie.CommandType = CommandType.Text;
            polecenie.CommandText = "Select * from znajomi";

            /*
            SqlPrameter por1 = new SqlParamerter();
            por1.ParameterName = @Nazwisko;
            por1.Value = "";
            polecenie.Parameters.Add(por1);
              */
            most1.SelectCommand = polecenie;
            most1.Fill(kontener,"znajomi");

            polecenie.CommandText = "select * from mieszkanie";
            most1.SelectCommand = polecenie;
            most1.Fill(kontener, "mieszkanie");

            polecenie.CommandText = "select * from ksiazki";
            most1.SelectCommand = polecenie;
            most1.Fill(kontener, "ksiazki");
            

            bool flaga_1 = true;
            DataRelation relacja1 = new DataRelation("Powiazanie", kontener.Tables["mieszkanie"].Columns["pesel"],kontener.Tables["mieszkania"].Columns["pesel"], flaga_1);
            kontener.Relations.Add(relacja1);


            var organ1 = kontener.Relations[0].ParentKeyConstraint;

            MessageBox.Show(organ1.ConstraintName,"nazwa ograniczenia");
            MessageBox.Show("relacja utworono wykorzystuajc tylko trzy argumenty " + kontener.Relations["Powiazanie"].ChildKeyConstraint);
            MessageBox.Show("liczba tabeli " + kontener.Tables.Count.ToString());
            MessageBox.Show("liczba relacji " + kontener.Relations.Count.ToString());
            //Dane_2.Items.Add("zsumowanie informacji"); znikad sie wzielo nigdzie nie zadeklarowane w kodzie millera  XD
            if(relacja1.ChildKeyConstraint != null)
            {
                MessageBox.Show("wanze liczba kolumn klucza obcego " + kontener.Tables["mieszkania"].Columns.Count);
            }
        }
        DataSet pojemnik = new DataSet();
        SqlDataAdapter pomost;
        SqlDataAdapter pomost2;
        SqlConnection pol5 = new SqlConnection("");
        SqlCommandBuilder automat;
        SqlCommandBuilder automat2;

        

        private void button13_Click(object sender, EventArgs e)
        {
            string sql = "select * from znajomi5";
            pomost = new SqlDataAdapter(sql, pol5);
            pomost.Fill(pojemnik, "znajomi");

            string sql_1 = "select * from mieszkania";

            automat = new SqlCommandBuilder(pomost);
            pomost2 = new SqlDataAdapter(sql_1, pol5);
            pomost2.Fill(pojemnik, "mieszkanie");
            automat2 = new SqlCommandBuilder(pomost2);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DataSet dataset = new DataSet("dataset");
            DataTable table = new DataTable("table");
            DataColumn column = new DataColumn("col1");

            column.Unique = true;
            table.Columns.Add(column);
            dataset.Tables.Add(table);
            DataRow row;
            for(int i = 0; i < 5; i++)
            {
                row = table.NewRow();
                row["col1"] = i;
                table.Rows.Add(row);

            }
            table.AcceptChanges();

            dataset.EnforceConstraints = false;
            foreach(DataRow thisRow in table.Rows)
            {
                thisRow["col1"] = 1;
                MessageBox.Show(thisRow[0].ToString(), "wartosci");

            }
            try
            {
                dataset.EnforceConstraints = true;
            }
            catch(System.Data.ConstraintException e1)
            {
                //nie dokonczył przykładu
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SqlCommand obiekt_c = new SqlCommand();
            obiekt_c.Connection = new SqlConnection("string con");
            obiekt_c.CommandType = CommandType.Text;
            obiekt_c.CommandText = "select * from znajomi";

            DataSet pojemnik1 = new DataSet();
            SqlDataAdapter most = new SqlDataAdapter();
            most.SelectCommand = obiekt_c;

            most.Fill(pojemnik, "moj znajomy");
            DataTable tabela_a = pojemnik1.Tables["moj znajomy"];
            DataView widok = new DataView(tabela_a);

            widok.Sort = "nazwisko,imie";
            widok.RowFilter = "Majatek > 62000";
            widok.RowStateFilter = DataViewRowState.CurrentRows;

            MessageBox.Show(widok.Count.ToString(), "liczba wierszy");

            object[] kryteria = new object[] { "nowak", "andrzej" };
            MessageBox.Show(kryteria.GetLength(0).ToString(), "liczba");
            DataRowView[] wiersze = widok.FindRows(kryteria);

            MessageBox.Show("liczba zwroconych weirszy " + wiersze.LongLength.ToString());
            string info = "";
            MessageBox.Show(wiersze.GetLength(0).ToString(), "wykorzystanie");

            //nie dokonczony
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DataTable zbior;
            DataTable tabela1;
            string info_mw = "";
            tabela1 = pojemnik.Tables["znajomi"];
            DataRow wiersz_mw = tabela1.Rows[5];

            zbior = tabela1.GetChanges(DataRowState.Modified);
            if(zbior == null)
            {
                MessageBox.Show("brak zmodyfikowanych wierszy");
                return;
            }
            string dane = "";
            foreach(DataRow r1 in zbior.Rows)
            {
                foreach(DataColumn k1 in zbior.Columns)
                {
                    dane += r1[k1].ToString() + "\t";

                }
                dane += "\n";
            }
            MessageBox.Show("to sa poprzednie wartosci \n" + dane);
        }
    }
}
