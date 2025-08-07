' ViewModels/MainViewModel.vb
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Public Class MainViewModel
    Implements INotifyPropertyChanged

    Private ReadOnly _dbService As New DatabaseService()

    Private _planningList As ObservableCollection(Of PlanningModel)
    Public Property PlanningList As ObservableCollection(Of PlanningModel)
        Get
            Return _planningList
        End Get
        Set(value As ObservableCollection(Of PlanningModel))
            _planningList = value
            OnPropertyChanged()
        End Set
    End Property

    Private _jobFormingList As ObservableCollection(Of JobModel)
    Public Property JobFormingList As ObservableCollection(Of JobModel)
        Get
            Return _jobFormingList
        End Get
        Set(value As ObservableCollection(Of JobModel))
            _jobFormingList = value
            OnPropertyChanged()
        End Set
    End Property

    Public Sub New()
        LoadData()
    End Sub

    Private Async Sub LoadData()
        PlanningList = Await _dbService.GetPlanningListAsync()
        JobFormingList = Await _dbService.GetJobFormingListAsync()
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional name As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
