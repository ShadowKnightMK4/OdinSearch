using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO.Enumeration;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using OdinSearchEngine;

namespace OdinSearchWin
{

    public partial class LandingForm : Form
    {
        public LandingForm()
        {
            InitializeComponent();
        }

        OdinSearch OdinSearch;
        SearchAnchor SearchAnchor;
        SearchTarget Target;
        BindingSource MyBindingSource = new BindingSource();
        MyGathererer myGathererer = new MyGathererer();
        private void Form1_Load(object sender, EventArgs e)
        {

            //this.dataGridView1.DataSource = myGathererer;
            OdinSearch = new OdinSearch();
            SearchAnchor = new SearchAnchor();
            Target = new SearchTarget();
            Target.FileName.Add(SearchTarget.MatchAnyFile);
            OdinSearch.AddSearchAnchor(SearchAnchor);
            OdinSearch.AddSearchTarget(Target);
            myGathererer.PropertyChanged += RefreshDataGrid;
            SearchAnchor.EnumSubFolders = true;

            
            
         
        }

        private void beginToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OdinSearch.Search(myGathererer);
        }

        /// <summary>
        /// Thanks chatgpt
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string[] GetPropertyNames(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();

            string[] propertyNames = new string[propertyInfos.Length];
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                propertyNames[i] = propertyInfos[i].Name;
            }

            return propertyNames;
        }


        /// <summary>
        /// Thanks chatgpt
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string[] GetPropertyValues(FileSystemInfo fileSystemInfo)
        {
            PropertyInfo[] propertyInfos = fileSystemInfo.GetType().GetProperties();

            string[] propertyValues = new string[propertyInfos.Length];
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                object value = propertyInfos[i].GetValue(fileSystemInfo);
                propertyValues[i] = value != null ? value.ToString() : string.Empty;
            }

            return propertyValues;
        }
        object[] FileEntryAsRow(FileSystemEntry e)
        {
            return GetPropertyNames(typeof(FileSystemEntry));
        }
        PropertyInfo[] DirProps = typeof(DirectoryInfo).GetProperties();
        PropertyInfo[] FilProps = typeof(FileInfo).GetProperties();
        private void RefreshDataGrid(object sender, EventArgs e)
        {
            

            DataGridViewResults.Invoke(new Action(() =>
            {



                DataGridViewResults.Invalidate(true);
                DataGridViewResults.Update();
                
            }));
            
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public class MyGathererer : OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputConsumerGatherResults, IBindingList
    {
        public object? this[int index]
        {
            get => Results[index];
            set
            {
                Results[index] = (FileSystemInfo)value;
            }
        }
        public bool AllowEdit
        {
            get { return false; }
        }

        public bool AllowNew
        {
            get
            {
                return false;
            }
        }

        public bool AllowRemove
        {
            get
            {
                return false;
            }
        }

        public bool IsSorted => throw new NotImplementedException();

        public ListSortDirection SortDirection => throw new NotImplementedException();

        public PropertyDescriptor? SortProperty => throw new NotImplementedException();

        public bool SupportsChangeNotification => throw new NotImplementedException();

        public bool SupportsSearching => throw new NotImplementedException();

        public bool SupportsSorting => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public event ListChangedEventHandler ListChanged;

        public int Add(object? value)
        {
            throw new NotImplementedException();
        }

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object? AddNew()
        {
            FileInfo ret = new FileInfo()l;
            Results.Add(ret);
            return ret;
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            
            throw new NotImplementedException();
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool Contains(object? value)
        {
            return Results.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object? value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object? value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object? value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }
    }
}