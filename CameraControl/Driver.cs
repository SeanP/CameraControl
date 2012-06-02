using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl
{
    public class Driver : INotifyPropertyChanged
    {
        private string carNumber = "63";
        private string driverName = "Sean Pivek";
        private int position = 1;
        private int positionInClass = 1;
        private string carClass = "LM P2";

        public Driver() { }
        public Driver(string number, string name, string carClazz)
        {
            carNumber = number;
            driverName = name;
            carClass = carClazz;
        }

        public string CarNumber
        {
            get
            {
                return carNumber;
            }
            set
            {
                if (carNumber != value)
                {
                    carNumber = value;
                    OnPropertyChanged("CarNumber");
                }
            }
        }

        public string DriverName
        {
            get { return driverName; }
            set
            {
                if (driverName != value)
                {
                    driverName = value;
                    OnPropertyChanged("DriverName");
                }
            }
        }

        public int Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        public int PositionInClass
        {
            get { return positionInClass; }
            set
            {
                if (positionInClass != value)
                {
                    positionInClass = value;
                    OnPropertyChanged("PositionInClass");
                }
            }
        }
        public string CarClass
        {
            get { return carClass; }
            set
            {
                if (carClass != value)
                {
                    carClass = value;
                    OnPropertyChanged("CarClass");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class DriverList : ObservableCollection<Driver> { }


    public abstract class DriverComparer : System.Collections.IComparer
    {
        protected bool isAscending = true;
        public bool IsAscending {
            set { isAscending = value; }
        }
        public abstract int Compare(object ox, object oy);
    }

    public class DriverNumberComparer : DriverComparer
    {
        public DriverNumberComparer() {}
        public override int Compare(object ox, object oy)
        {
            Driver x;
            Driver y;
            if (isAscending) {
                x = ox as Driver;
                y = oy as Driver;
            } else {
                y = ox as Driver;
                x = oy as Driver;
            }
            int x_num = Convert.ToInt16(x.CarNumber);
            int y_num = Convert.ToInt16(y.CarNumber);

            if (x_num < y_num) return -1;
            else if (x_num > y_num) return 1;
            else return y.CarNumber.Length - x.CarNumber.Length;
        }
    }

    public class DriverCarClassComparer : DriverComparer
    {
        public DriverCarClassComparer() { }
        public override int Compare(object ox, object oy)
        {
            Driver x;
            Driver y;
            if (isAscending)
            {
                x = ox as Driver;
                y = oy as Driver;
            }
            else
            {
                y = ox as Driver;
                x = oy as Driver;
            }
            int x_class_id;
            switch (x.CarClass)
            {
                case "P2":
                    x_class_id = 0;
                    break;
                case "GT1":
                    x_class_id = 1;
                    break;
                case "GT2":
                    x_class_id = 2;
                    break;
                default:
                    x_class_id = 3;
                    break;
            }

            int y_class_id;
            switch (y.CarClass)
            {
                case "P2":
                    y_class_id = 0;
                    break;
                case "GT1":
                    y_class_id = 1;
                    break;
                case "GT2":
                    y_class_id = 2;
                    break;
                default:
                    y_class_id = 3;
                    break;
            }

            if (x_class_id == y_class_id)
            {
                return x.PositionInClass - y.PositionInClass;
            }
            else
            {
                return x_class_id - y_class_id;
            }
        }
    }
}
