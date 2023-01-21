using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileInventoryEngine;
namespace FileInventory
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        object lockthis = new object();
        SearchTarget Example = new SearchTarget();
        MatchMaker FindThemAll = new MatchMaker();
        Task SearchHandler;
        DataTable Results = new DataTable();
        List<FileSystemInfo> List = new List<FileSystemInfo>();

        bool DirtyList = false;
       DataColumn InsertColumn(PropertyInfo Info)
        {
            if (Info.CanRead)
            {
                DataColumn newentry = new DataColumn();
                newentry.ColumnName = Info.Name;

                newentry.DataType = Info.PropertyType;
                newentry.AllowDBNull = true;
                return newentry;
            }
            return null;
        }
        
        void AddToList(FileSystemInfo Match)
        {
            List.Add(Match);
            DirtyList = true;
            return;
        }
        void SearchStart()
        {
            FindThemAll.Search(AddToList, lockthis);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SearchAnchor LocalStorage = new SearchAnchor();
            LocalStorage.EnumSubFolders = true;
            FindThemAll.Anchors.Add(LocalStorage);
            //Example.FileName.Add();
            Example.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            FindThemAll.SearchFor.Add(Example);
            
            SearchHandler  = new Task(SearchStart,  TaskCreationOptions.RunContinuationsAsynchronously);
            
            SearchHandler.Start();
            
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DisplayUpdator_Tick(object sender, EventArgs e)
        {
            List<PropertyInfo> FolderProps;

            if (DirtyList)
            {
                if (dataGridView1.Columns.Count == 0)
                {
                    // create the columns
                     FolderProps = new List<PropertyInfo>(new FileInfo(Application.StartupPath).GetType().GetProperties());
                     


                    
                    foreach (PropertyInfo I in FolderProps)
                    {
                        dataGridView1.Columns.Add(I.Name, I.Name);
                    }
                    dataGridView1.Columns.Add("Parent", null);
                    dataGridView1.Columns.Add("Root", null);
                    
                }

                lock (lockthis)
                {
                    foreach (FileSystemInfo I in List)
                    {

                        PropertyInfo[] Props = I.GetType().GetProperties();
                        int newrow = dataGridView1.Rows.Add();
                        for (int step = 0; step < Props.Length; step++)
                        {
                            PropertyInfo single = Props[step];

                            MethodInfo GetRoutine = single.GetGetMethod();
                            Type ReturnType = GetRoutine.ReturnType;
                            string value = null;

                            if (ReturnType == typeof(bool))
                            {
                                value = ((bool)GetRoutine.Invoke(I, null)).ToString();
                            }
                            else if (ReturnType == typeof(string))
                            {
                                value = (string)GetRoutine.Invoke(I, null);
                            }
                            else if (ReturnType == typeof(DateTime))
                            {
                                value = ((DateTime)GetRoutine.Invoke(I, null)).ToString();
                            }
                            else if (ReturnType == typeof(DirectoryInfo))
                            {
                                value = ((DirectoryInfo)GetRoutine.Invoke(I, null)).FullName;
                            }
                            else if (ReturnType == typeof(FileAttributes))
                            {
                                value = Enum.GetName(typeof(FileAttributes), GetRoutine.Invoke(I, null));
                            }
                            else if (ReturnType == typeof(Int64))
                            {
                                value = GetRoutine.Invoke(I, null).ToString();
                            }
                            else
                            {
                                throw new NotImplementedException("THe propertype " + ReturnType.Name + " does not have code to diplsay");
                            }
                            dataGridView1[single.Name, newrow].Value = value;






                        }

                    }
                    List.Clear();
                    dataGridView1.Refresh();
                }
            }
            
        }
    }
}
