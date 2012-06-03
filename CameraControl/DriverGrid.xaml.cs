using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IEvent;

namespace CameraControl
{
    /// <summary>
    /// Interaction logic for DriverGrid.xaml
    /// </summary>
    public partial class DriverGrid : DataGrid, DriverInfoUpdateHandler, SessionInfoUpdateHandler
    {
        private Listener l;
        private Thread listenerThread;
        public DriverGrid()
        {
            InitializeComponent();
            Sorting += new DataGridSortingEventHandler(SortHandler);
            DataContextChanged += new DependencyPropertyChangedEventHandler(DataChanged);

            Listener.DriverHandlers.Add(this as DriverInfoUpdateHandler);
            Listener.SessionHandlers.Add(this as SessionInfoUpdateHandler);

            DriverList dl = new DriverList();
            this.DataContext = dl;

            l = new Listener();
            listenerThread = new Thread(l.listen);
            listenerThread.Name = "Listener";
            listenerThread.IsBackground = true;
            listenerThread.Start();


        }

        ~DriverGrid()
        {
            Console.WriteLine("Deconstructing");
            l.Running = false;
            listenerThread.Abort();
        }


        private DataGridColumn currentlySortedColumn = null;
        private void DataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("Data changed!");
            if (currentlySortedColumn == null)
            {
                foreach (DataGridColumn c in Columns)
                {
                    if (c.Header.ToString() == "Pos")
                    {
                        currentlySortedColumn = c;
                    }
                }
            }
            sort(currentlySortedColumn, false);
            /*
            if (currentlySortedColumn != null)
            {
                sort(currentlySortedColumn, false);
            }
            else
            {
                //sort();
            }*/
        }

        private bool sort(DataGridColumn column, bool doFlip = true)
        {
            DriverComparer comparer = null;
            String columnHeader = column.Header.ToString();
            if (columnHeader == "#")
            {
                comparer = new DriverNumberComparer();
            }
            else if (columnHeader == "Class" || columnHeader == "P.C.")
            {
                comparer = new DriverCarClassComparer();
            }
            else
            {
                return false;
            }
            ListSortDirection direction;
            if (doFlip)
            {
                direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            }
            else
            {
                direction = (column.SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            }


            column.SortDirection = direction;
            comparer.IsAscending = (direction == ListSortDirection.Ascending);

            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(this.ItemsSource);
            lcv.CustomSort = comparer;

            return true;
        }

        private void SortHandler(object sender, DataGridSortingEventArgs e)
        {
            DataGridColumn column = e.Column;
            if (sort(column))
            {
                e.Handled = true;
            }
            currentlySortedColumn = column;
        }

        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
        {
            //System.Console.Error.WriteLine("OnSelectedCellsChanged");
            base.OnSelectedCellsChanged(e);
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            //UnselectAllCells();
            //e.Handled = true;
            //base.OnSelectionChanged(e);
        }

        private void testDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }
            var selectedDriver = SelectedItem as Driver;
            Requester.focusCamera(selectedDriver.CarNumber);
        }

        public void driverInfoUpdate(List<DriverInfo> drivers)
        {
            Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        DriverList dl = DataContext as DriverList;
                        foreach (DriverInfo di in drivers)
                        {
                            Driver d = new Driver();
                            d.CarNumber = di.CarNumber;
                            d.DriverName = di.DriverName;
                            d.Position = 999;
                            d.PositionInClass = 999;

                            switch (di.CarClassID)
                            {
                                case 40:
                                    d.CarClass = "P2";
                                    break;
                                case 23:
                                    d.CarClass = "GT1";
                                    break;
                                case 41:
                                    d.CarClass = "GT2";
                                    break;
                                default:
                                    d.CarClass = Convert.ToString(di.CarClassID);
                                    break;
                            }
                            dl.Add(d);
                        }
                    }));
        }


        public void sessionInfoUpdate(SessionInfo sessionInfo)
        {
            Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        DriverList dl = DataContext as DriverList;
                        Session s = sessionInfo.Sessions[sessionInfo.CurrentSession];
                        if (s == null) return;

                        foreach (ResultEntry re in s.Results)
                        {
                            Driver d = dl.First(
                                delegate(Driver dr)
                                {
                                    return dr.CarNumber == re.CarNumber;
                                }
                                );
                            if (d != null)
                            {
                                d.Position = re.Position;
                                d.PositionInClass = re.PositionInClass;
                            }
                        }
                        dl.UpdateCollection();
                    }
            ));
        }
    }
}
