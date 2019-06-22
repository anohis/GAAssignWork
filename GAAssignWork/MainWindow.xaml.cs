using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GAAssignWork.GA;

namespace GAAssignWork
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random _random = new Random();
        private ObservableCollection<WorkData> _workDatas = new ObservableCollection<WorkData>();
        private ObservableCollection<EmployeeData> _employeeDatas = new ObservableCollection<EmployeeData>();
        private ObservableCollection<AssignmentData> _assignmentDatas = new ObservableCollection<AssignmentData>();

        public MainWindow()
        {
            InitializeComponent();

            InitializeWorkDataGrid();
            InitializeCreateWorkButton();
            InitializeDeleteWorkButton();

            InitializeEmployeeDataGrid();
            InitializeCreateEmployeeDataButton();
            InitializeDeleteEmployeeDataButton();

            InitializeAssignmentDataGrid();
            InitializeCreateAssignButton();

            Test();
        }

        private void InitializeCreateWorkButton()
        {
            CreateWorkButton.Click += (e, s) =>
            {
                _workDatas.Add(new WorkData() { Index = _workDatas.Count + 1 });
            };
        }
        private void InitializeDeleteWorkButton()
        {
            DeleteWorkButton.Click += (e, s) =>
            {
                var index = WorkDataGrid.SelectedIndex;

                if (index >= 0 && index < _workDatas.Count)
                {
                    for (int i = index + 1; i < _workDatas.Count; i++)
                    {
                        _workDatas[i].Index--;
                    }
                    _workDatas.RemoveAt(index);
                }
            };
        }
        private void InitializeWorkDataGrid()
        {
            WorkDataGrid.AutoGenerateColumns = false;
            WorkDataGrid.CanUserAddRows = false;
            WorkDataGrid.CellEditEnding += OnWorkDataEditEnding;

            WorkDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Index",
                Binding = new Binding("Index"),
                IsReadOnly = true
            });

            WorkDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Name",
                Binding = new Binding("Name"),
            });

            WorkDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Value",
                Binding = new Binding("Value"),
            });

            WorkDataGrid.ItemsSource = _workDatas;
        }
        private void OnWorkDataEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    var textBox = e.EditingElement as TextBox;
                    var data = e.Row.DataContext as WorkData;
                    if (bindingPath == "Name")
                    {
                        data.Name = textBox.Text;
                    }
                    else if (bindingPath == "Value")
                    {
                        float value;
                        if (float.TryParse(textBox.Text, out value))
                        {
                            data.Value = value;
                        }
                        else
                        {
                            data.Value = data.Value;
                        }
                    }
                }
            }
        }

        private void InitializeCreateEmployeeDataButton()
        {
            CreateEmployeeButton.Click += (e, s) =>
            {
                _employeeDatas.Add(new EmployeeData() { Index = _employeeDatas.Count + 1 });
            };
        }
        private void InitializeDeleteEmployeeDataButton()
        {
            DeleteEmployeeButton.Click += (e, s) =>
            {
                var index = EmployeeDataGrid.SelectedIndex;

                if (index >= 0 && index < _employeeDatas.Count)
                {
                    for (int i = index + 1; i < _employeeDatas.Count; i++)
                    {
                        _employeeDatas[i].Index--;
                    }
                    _employeeDatas.RemoveAt(index);
                }
            };
        }
        private void InitializeEmployeeDataGrid()
        {
            EmployeeDataGrid.AutoGenerateColumns = false;
            EmployeeDataGrid.CanUserAddRows = false;
            EmployeeDataGrid.CellEditEnding += OnEmployeeDataEditEnding;

            EmployeeDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Index",
                Binding = new Binding("Index"),
                IsReadOnly = true
            });

            EmployeeDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Name",
                Binding = new Binding("Name"),
            });

            EmployeeDataGrid.ItemsSource = _employeeDatas;
        }
        private void OnEmployeeDataEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    var textBox = e.EditingElement as TextBox;
                    var data = e.Row.DataContext as EmployeeData;
                    if (bindingPath == "Name")
                    {
                        data.Name = textBox.Text;
                    }
                }
            }
        }

        private void InitializeAssignmentDataGrid()
        {
            AssignmentDataGrid.AutoGenerateColumns = false;
            AssignmentDataGrid.CanUserAddRows = false;

            AssignmentDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "EmployeeName",
                Binding = new Binding("EmployeeName"),
                IsReadOnly = true
            });

            AssignmentDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "WorkNames",
                Binding = new Binding("WorkNames"),
                IsReadOnly = true
            });

            AssignmentDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Value",
                Binding = new Binding("Value"),
                IsReadOnly = true
            });

            AssignmentDataGrid.ItemsSource = _assignmentDatas;
        }
        private void InitializeCreateAssignButton()
        {
            AssignButton.Click += (e, s) =>
            {
                int loopCount = int.Parse(LoopCountText.Text);
                int poolSize = int.Parse(PoolSizeText.Text);
                int selectCount = int.Parse(SelectCountText.Text);
                float terminateValue = float.Parse(TerminateValueText.Text);

                new Thread(()=> 
                {
                    GeneticAlgorithm ga = new GeneticAlgorithm(_workDatas.Count * _employeeDatas.Count, poolSize, selectCount, terminateValue);
                    ga.OnCreateChromosomeEvent += OnCreateChromosome;
                    ga.OnCalculateFitnessEvent += CalculateFitness;
                    ga.OnCrossOverEvent += OnCrossOver;
                    ga.OnMutationEvent += OnMutation;

                    var assign = ga.Execute(loopCount);

                    Dispatcher.Invoke(() => 
                    {
                        Parse(assign);
                    });
                }).Start();
            };
        }
        private void Parse(int[] assign)
        {
            _assignmentDatas.Clear();

            for (int i = 0; i < _employeeDatas.Count; i++)
            {
                AssignmentData data = new AssignmentData();
                data.Employee = _employeeDatas[i];
                for (int j = 0; j < _workDatas.Count; j++)
                {
                    int idx = i * _workDatas.Count + j;
                    if (assign[idx] == 1)
                    {
                        data.Works.Add(_workDatas[j]);
                    }
                }
                _assignmentDatas.Add(data);
            }
        }
        private void OnMutation(int index, int[] assign)
        {
            int workIndex = index % _workDatas.Count;
            int employeeIndex = index / _workDatas.Count;
            if (assign[index] == 0)
            {
                int idx = _random.Next(0, _employeeDatas.Count) * _workDatas.Count + workIndex;
                assign[idx] = 1;
            }
            else
            {
                for (int i = 0; i < _employeeDatas.Count; i++)
                {
                    if (i == employeeIndex)
                    {
                        continue;
                    }
                    int idx = i * _workDatas.Count + workIndex;
                    assign[idx] = 0;
                }
            }
        }
        private void OnCrossOver(int index, int[] a, int[] b)
        {
            int workIndex = index % _workDatas.Count;
            int employeeIndex = index / _workDatas.Count;
            for (int i = 0; i < _employeeDatas.Count; i++)
            {
                if (i == employeeIndex)
                {
                    continue;
                }

                int idx = i * _workDatas.Count + workIndex;
                if (a[idx] == 1 && a[index] == 1)
                {
                    a[idx] = 0;
                    b[idx] = 1;
                }
                else if (b[idx] == 1 && b[index] == 1)
                {
                    a[idx] = 1;
                    b[idx] = 0;
                }
            }
        }
        private float CalculateFitness(int[] assign)
        {
            float totalValue = 0;
            foreach (var w in _workDatas)
            {
                totalValue += w.Value;
            }
            float avgValue = totalValue / _employeeDatas.Count;

            var maxMse = (float)Math.Pow(avgValue - totalValue, 2)
               + (_employeeDatas.Count - 1) * (float)Math.Pow(avgValue - 0, 2);
            maxMse /= _employeeDatas.Count;

            float mse = 0;
            for (int i = 0; i < _employeeDatas.Count; i++)
            {
                float value = 0;
                for (int j = 0; j < _workDatas.Count; j++)
                {
                    value += assign[i * _workDatas.Count + j] * _workDatas[j].Value;
                }
                mse += (float)Math.Pow(avgValue - value, 2);
            }
            mse /= _employeeDatas.Count;
            mse = (maxMse - mse) / maxMse;
            return mse;
        }
        private void OnCreateChromosome(int[] assign)
        {
            List<WorkData> works = new List<WorkData>(_workDatas);

            int i = 0;
            for (; i < _employeeDatas.Count; i++)
            {
                int workCount = _random.Next(0, works.Count);
                while (workCount > 0)
                {
                    int index = _random.Next(0, works.Count);
                    var data = works[index];
                    works.RemoveAt(index);
                    assign[i * _workDatas.Count + data.Index - 1] = 1;
                    workCount--;
                }
            }

            i--;
            foreach (var w in works)
            {
                assign[i * _workDatas.Count + w.Index - 1] = 1;
            }
        }

        private void Test()
        {
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "a",
                Value = 1,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "b",
                Value = 2,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "c",
                Value = 3,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "d",
                Value = 4,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "e",
                Value = 5,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "f",
                Value = 6,
            });
            _workDatas.Add(new WorkData()
            {
                Index = _workDatas.Count + 1,
                Name = "i",
                Value = 7,
            });

            _employeeDatas.Add(new EmployeeData()
            {
                Index = _employeeDatas.Count + 1,
                Name = "A"
            });
            _employeeDatas.Add(new EmployeeData()
            {
                Index = _employeeDatas.Count + 1,
                Name = "B"
            });
            _employeeDatas.Add(new EmployeeData()
            {
                Index = _employeeDatas.Count + 1,
                Name = "C"
            });
        }
    }
}
